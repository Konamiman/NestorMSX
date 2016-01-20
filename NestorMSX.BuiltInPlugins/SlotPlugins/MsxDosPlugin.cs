using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Konamiman.Z80dotNet;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using Konamiman.NestorMSX.Misc;
using ConfigurationException = Konamiman.NestorMSX.Exceptions.ConfigurationException;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("MSX-DOS")]
    public class MsxDosPlugin : DiskImageBasedStoragePlugin
    {
        private IDictionary<ushort, Action> kernelRoutines;
        private ushort addressOfCallInihrd;
        private ushort addressOfCallDrives;

        public MsxDosPlugin(PluginContext context, IDictionary<string, object> pluginConfig) 
            : base(context, pluginConfig)
        {
            kernelRoutines = new Dictionary<ushort, Action>
            {
                { 0x4010, DSKIO },
                { 0x4013, () => DSKCHG_GETDPB(true) },
                { 0x4016, () => DSKCHG_GETDPB(false) },
                { 0x4019, CHOICE },
                { 0x401C, DSKFMT },
                { 0x401F, MTOFF  }
            };
        }

        protected override void ApplyConfiguration(IDictionary<string, object> pluginConfig)
        {
            maxNumberOfDevices = pluginConfig.GetValueOrDefault("numberOfDrives", 2);
            if(maxNumberOfDevices < 1 || maxNumberOfDevices > 8)
                throw new ConfigurationException("numberOfDrives must be a number between 1 and 8");

            addressOfCallInihrd = pluginConfig.GetValueOrDefault<ushort>("addressOfCallInihrd", 0x176F);
            addressOfCallDrives = pluginConfig.GetValueOrDefault<ushort>("addressOfCallDrives", 0x1850); 
        }

        private int maxNumberOfDevices;
        protected override int MaxNumberOfDevices { get {return maxNumberOfDevices;} }

        protected override string PluginDisplayName
        {
            get { return "MSX-DOS"; }
        }

        protected override string defaultKernelFileName
        {
            get { return "MsxDosKernel.rom"; }
        }

        protected override void BeforeZ80InstructionFetch(ushort instructionAddress)
        {
            if (memory.GetCurrentSlot(1) != slotNumber)
                return;

            if (kernelRoutines.ContainsKey(z80.Registers.PC))
            {
                var routine = kernelRoutines[z80.Registers.PC];
                routine();
                z80.ExecuteRet();
            }
        }

        protected override IMemory GetMemory(byte[] kernelFileContents)
        {
            if(addressOfCallInihrd != 0) {
                kernelFileContents[addressOfCallInihrd] = 0; //Patch call to INIHRD with NOPs
                kernelFileContents[addressOfCallInihrd + 1] = 0;
                kernelFileContents[addressOfCallInihrd + 2] = 0;
            }

            if(addressOfCallDrives != 0) {
                kernelFileContents[addressOfCallDrives] = 0x2E; //Patch call to DRIVES with LD L,drives
                kernelFileContents[addressOfCallDrives + 1] = (byte)maxNumberOfDevices;
                kernelFileContents[addressOfCallDrives + 2] = 0;
            }

            return new PlainRom(kernelFileContents, 1);
        }

        protected override void ValidateKernelFileContents(byte[] kernelFileContents)
        {
            if(kernelFileContents.Length != 16*1024)
                throw new ConfigurationException(
                    "Invalid kernel file: a MSX-DOS kernel always has a size of exactly 16K. If you want to use MSX-DOS 2, configure a standalone MSX-DOS 2.20 kernel in ahother slot (with memory type Ascii16).");
        }

        void DSKIO()
        {
            var driveNumber = z80.Registers.A;
            var numberOfSectors = z80.Registers.B;
            var memoryAddress = z80.Registers.HL.ToUShort();
            var sectorNumber = (int)z80.Registers.DE.ToUShort();

            var mediaId = z80.Registers.C;
            if(mediaId < 128)
                sectorNumber += mediaId * 65536;

            bool isWrite = z80.Registers.CF;
            z80.Registers.CF = 1;

            if(!IsValidDevice(driveNumber + 1)) {
                z80.Registers.A = 2;
                return;
            }

            var file = imageFiles[driveNumber];
            if (sectorNumber > file.MaxSectorNumber)
            {
                z80.Registers.A = 6;
                z80.Registers.B = 0;
                return;
            }

            if (z80.Registers.CF && file.IsReadOnly)
            {
                z80.Registers.A = 10;
                z80.Registers.B = 0;
                return;
            }

            file.Stream.Seek(sectorNumber * 512, SeekOrigin.Begin);

            try
            {
                if(isWrite)
                    WriteSectors(sectorNumber, memoryAddress, numberOfSectors, file.Stream);
                else
                    ReadSectors(sectorNumber, memoryAddress, numberOfSectors, file.Stream);

                z80.Registers.B = numberOfSectors;
                z80.Registers.A = 0;
            }
            catch (Exception)
            {
                z80.Registers.B = 0;
                z80.Registers.A = 12;
            }

            z80.Registers.CF = (z80.Registers.A != 0);
        }

        void DSKCHG_GETDPB(bool checkFileChanged)
        {
            var driveNumber = z80.Registers.A;

            z80.Registers.CF = 1;

            if(!IsValidDevice(driveNumber + 1)) {
                z80.Registers.A = 2;
                return;
            }

            z80.Registers.A = 0;

            var file = imageFiles[driveNumber];
            bool fileHadChanged = false;
            if (checkFileChanged) {
                fileHadChanged = file.HasChanged;
                z80.Registers.B = (byte)(fileHadChanged ? -1 : 1);
                file.HasChanged = false;
            }

            if(file.Dpb == null) {
                var dpbOk = GenerateDpb(file);
                if(!dpbOk) {
                    z80.Registers.A = 12;
                    return;
                }
            }

            if(fileHadChanged)
                memory.SetContents(z80.Registers.HL + 1, file.Dpb);

            z80.Registers.CF = 0;
        }

        private bool GenerateDpb(ImageFileInfo file)
        {
            try {
                GenerateDpbCore(file);
                return true;
            }
            catch {
                return false;
            }
        }

        private void GenerateDpbCore(ImageFileInfo file)
        {
            var sector0 = new byte[24];

            file.Stream.Seek(0, SeekOrigin.Begin);
            file.Stream.Read(sector0, 0, 24);

            var dpb = new byte[18];

            dpb[0] = sector0[21]; //media descriptor
            dpb[1] = 0; //sector size, low
            dpb[2] = 2; //sector size, high
            dpb[3] = 15; //dirsmsk: (sector size/32)-1
            dpb[4] = 4; //dirshift: 1s in dirmsk
            var sectorsPerCluster = sector0[13];
            dpb[5] = (byte)(sectorsPerCluster-1); //clusmsk
            dpb[6] = (byte)(Math.Log(sectorsPerCluster, 2) + 1); //(1s in clusmsk) + 1
            var reservedSectors = NumberUtils.CreateUshort(sector0[14], sector0[15]);
            var firstFatSectorNumber = reservedSectors;
            dpb[7] = firstFatSectorNumber.GetLowByte();
            dpb[8] = firstFatSectorNumber.GetHighByte();
            var numberOfFats = sector0[16];
            dpb[9] = numberOfFats;
            var rootDirectoryEntries = NumberUtils.CreateUshort(sector0[17], sector0[18]);
            dpb[10] = (byte)(rootDirectoryEntries > 254 ? 254 : rootDirectoryEntries);
            var sectorsPerFat = NumberUtils.CreateUshort(sector0[22], sector0[23]);
            var rootDirectorySectors = rootDirectoryEntries/16;
            var fatSectors = numberOfFats*sectorsPerFat;
            var firstDataSectorNumber = (ushort)(reservedSectors + fatSectors + rootDirectorySectors);
            dpb[11] = firstDataSectorNumber.GetLowByte();
            dpb[12] = firstDataSectorNumber.GetHighByte();
            var numberOfDataSectors = (file.MaxSectorNumber + 1) - firstDataSectorNumber;
            var clusterCount = numberOfDataSectors/sectorsPerCluster;
            var maxClusterNumber = (ushort)(clusterCount + 1); //Note that the first cluster number is 2
            dpb[13] = maxClusterNumber.GetLowByte();
            dpb[14] = maxClusterNumber.GetHighByte();
            dpb[15] = (byte)sectorsPerFat;
            var firstDirectorySector = (ushort)(reservedSectors + fatSectors);
            dpb[16] = firstDirectorySector.GetLowByte();
            dpb[17] = firstDirectorySector.GetHighByte();

            file.Dpb = dpb;
        }

        void CHOICE()
        {
            z80.Registers.HL = 0;
        }

        void DSKFMT()
        {
            z80.Registers.A = 16;
            z80.Registers.CF = 1;
        }

        void MTOFF()
        {
        }
    }
}
