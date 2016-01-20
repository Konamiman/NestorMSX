using System;
using System.Collections.Generic;
using System.IO;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using Konamiman.Z80dotNet;
using ConfigurationException = Konamiman.NestorMSX.Exceptions.ConfigurationException;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Nextor")]
    public class NextorPlugin : DiskImageBasedStoragePlugin
    {
        private Ascii8Rom kernelMemory;
        private IDictionary<ushort, Action> kernelRoutines;

        public NextorPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
            : base(context, pluginConfig)
        {
            kernelRoutines = new Dictionary<ushort, Action>
            {
                { 0x4133, DRV_VERSION },
                { 0x4136, DRV_INIT },
                { 0x4160, DEV_RW },
                { 0x4163, DEV_INFO },
                { 0x4166, DEV_STATUS },
                { 0x4169, LUN_INFO  }
            };
        }

        protected override int MaxNumberOfDevices { get { return 7; } }

        protected override IMemory GetMemory(byte[] kernelFileContents)
        {
            const int driverOffset = 112 * 1024;

            kernelFileContents[driverOffset + 0x010E] = 1; //Device-based driver
            Array.Copy(PaddedArrayFromString("NestorMSX Nextor plugin", 32), 0, kernelFileContents, driverOffset + 0x0110, 32);

            kernelMemory = new Ascii8Rom(kernelFileContents);
            return kernelMemory;
        }

        protected override string PluginDisplayName { get { return "Nextor"; } }

        protected override string defaultKernelFileName { get { return "NextorKernel.rom"; }
        }

        protected override void ValidateKernelFileContents(byte[] kernelFileContents)
        {
            if(kernelFileContents.Length < 112*1024)
                throw new ConfigurationException(
                    "Invalid kernel file: a Nextor kernel always has a size of at least 112K.");
        }

        protected override void BeforeZ80InstructionFetch(ushort instructionAddress)
        {
            if (memory.GetCurrentSlot(1) != slotNumber)
                return;

            if (kernelMemory.CurrentBlockInBank(1) != 14)
                return;

            if (kernelRoutines.ContainsKey(z80.Registers.PC))
            {
                var routine = kernelRoutines[z80.Registers.PC];
                routine();
                z80.ExecuteRet();
            }
        }

        private void DRV_VERSION()
        {
            z80.Registers.A = 1;
            z80.Registers.BC = 0;
        }

        private void DRV_INIT()
        {
            if (z80.Registers.A == 0)
            {
                z80.Registers.HL = 0;
                z80.Registers.CF = 0;
            }
        }

        private void DEV_RW()
        {
            var deviceIndex = z80.Registers.A;
            var numberOfSectors = z80.Registers.B;
            var logicalUnit = z80.Registers.C;
            var memoryAddress = z80.Registers.HL.ToUShort();
            var sectorAddress = z80.Registers.DE.ToUShort();

            if (!IsValidDevice(deviceIndex) || logicalUnit != 1)
            {
                z80.Registers.A = _IDEVL;
                z80.Registers.B = 0;
                return;
            }

            var sectorNumber =
                memory[sectorAddress]
                + 256 * memory[sectorAddress + 1]
                + 256 * 256 * memory[sectorAddress + 2]
                + 256 * 256 * 256 * memory[sectorAddress + 3];

            var file = imageFiles[deviceIndex - 1];
            if (sectorNumber > file.MaxSectorNumber)
            {
                z80.Registers.A = _RNF;
                z80.Registers.B = 0;
                return;
            }

            if (z80.Registers.CF && file.IsReadOnly)
            {
                z80.Registers.A = _WPROT;
                z80.Registers.B = 0;
                return;
            }

            file.Stream.Seek(sectorNumber * 512, SeekOrigin.Begin);

            try
            {
                if (z80.Registers.CF)
                    WriteSectors(sectorNumber, memoryAddress, numberOfSectors, file.Stream);
                else
                    ReadSectors(sectorNumber, memoryAddress, numberOfSectors, file.Stream);

                z80.Registers.B = numberOfSectors;
                z80.Registers.A = 0;
            }
            catch (Exception)
            {
                z80.Registers.B = 0;
                z80.Registers.A = _DISK;
            }
        }

        private void DEV_INFO()
        {
            var deviceIndex = z80.Registers.A;
            var infoBlockIndex = z80.Registers.B;
            var memoryAddress = z80.Registers.HL.ToUShort();

            if (!IsValidDevice(deviceIndex))
            {
                z80.Registers.A = 1; //Device not available
                return;
            }

            var file = imageFiles[deviceIndex - 1];
            string info = null;

            if (infoBlockIndex == 0)
            {
                memory[memoryAddress] = 1; //One logical unit
                memory[memoryAddress + 1] = 0; //Features
                z80.Registers.A = 0;
                return;
            }
            else if (infoBlockIndex == 1)
            {
                info = "Konamiman";
            }
            else if (infoBlockIndex == 2)
            {
                info = Path.GetFileName(file.FullPath);
            }
            else if (infoBlockIndex == 3)
            {
                info = "1";
            }
            else {
                z80.Registers.A = 1;
                return;
            }

            z80.Registers.A = 0;
            SetMemoryContents(memoryAddress, PaddedArrayFromString(info, 64));
        }

        private void DEV_STATUS()
        {
            var deviceIndex = z80.Registers.A;
            var logicalUnit = z80.Registers.B;

            if (!IsValidDevice(deviceIndex) || logicalUnit != 1)
                z80.Registers.A = 0; //Invalid device/LUN
            else if (imageFiles[deviceIndex - 1].HasChanged)
            {
                z80.Registers.A = 2; //Available and has changed
                imageFiles[deviceIndex - 1].HasChanged = false;
            }
            else
                z80.Registers.A = 1; //Available and has not changed
        }

        private void LUN_INFO()
        {
            var deviceIndex = z80.Registers.A;
            var logicalUnit = z80.Registers.B;
            var memoryAddress = z80.Registers.HL.ToUShort();

            if (!IsValidDevice(deviceIndex) || logicalUnit != 1)
            {
                z80.Registers.A = 1; //Invalid device/LUN
                return;
            }

            var file = imageFiles[deviceIndex - 1];
            var info = new byte[12];

            info[2] = 2; //sector size = 0x200
            info[7] = 1; //removable

            var numberOfSectors = BitConverter.GetBytes(file.MaxSectorNumber + 1);
            if (BitConverter.IsLittleEndian)
            {
                Array.Copy(numberOfSectors, 0, info, 3, 4);
            }
            else {
                info[0] = numberOfSectors[3];
                info[1] = numberOfSectors[2];
                info[2] = numberOfSectors[1];
                info[3] = numberOfSectors[0];
            }

            SetMemoryContents(memoryAddress, info);

            z80.Registers.A = 0;
        }
    }
}
