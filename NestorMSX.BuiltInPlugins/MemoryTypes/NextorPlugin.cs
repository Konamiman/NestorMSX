using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes
{
    [NestorMSXPlugin("Nextor")]
    public class NextorPlugin : IDisposable
    {
        private const int _IDEVL = 0xB5;
        private const int _RNF = 0xF9;

        private readonly string kernelFilePath;
        private IZ80Processor z80;
        private SlotNumber slotNumber;
        private IExternallyControlledSlotsSystem memory;
        private string diskImageFilePath;
        private FileStream diskImageFileStream;
        private long maxSectorNumber;
        private string diskImageName;

        private IDictionary<ushort, Action> kernelRoutines;

        private Ascii8Rom kernel;

        public NextorPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
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

            this.kernelFilePath = pluginConfig.GetMachineFilePath(pluginConfig.GetValue<string>("kernelFile"));
            this.diskImageFilePath = pluginConfig.GetValue<string>("diskImageFile").AsAbsolutePath();
            this.diskImageName = Path.GetFileName(diskImageFilePath);
            this.z80 = context.Cpu;
            this.memory = context.SlotsSystem;
            this.slotNumber = new SlotNumber(pluginConfig.GetValue<byte>("slotNumber"));

            this.maxSectorNumber = ((new FileInfo(diskImageFilePath)).Length) / 512 - 1;
            this.diskImageFileStream = File.Open(diskImageFilePath, FileMode.Open, FileAccess.ReadWrite);

            z80.BeforeInstructionFetch += Z80_BeforeInstructionFetch;

            this.kernel = new Ascii8Rom(ReadKernelFile());
        }

        private byte[] ReadKernelFile()
        {
            const int driverOffset = 112*1024;

            var kernelContents = File.ReadAllBytes(kernelFilePath);

            kernelContents[driverOffset + 0x010E] = 1; //Device-based driver
            Array.Copy(PaddedArrayFromString("NestorMSX Nextor plugin", 32), 0, kernelContents, driverOffset + 0x0110, 32);

            return kernelContents;
        }

        private byte[] PaddedArrayFromString(string theString, int totalLength)
        {
            if(theString.Length > totalLength)
                theString = theString.Remove(totalLength);
            return Encoding.ASCII.GetBytes(theString.PadRight(totalLength));
        }

        public IMemory GetMemory()
        {
            return kernel;
        }

        private void Z80_BeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs e)
        {
            if(memory.GetCurrentSlot(1) != slotNumber)
                return;

            if(kernel.CurrentBlockInBank(1) != 14)
                return;

            if(kernelRoutines.ContainsKey(z80.Registers.PC)) {
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
            if(z80.Registers.A == 0) {
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

            if(deviceIndex != 1 || logicalUnit != 1) {
                z80.Registers.A = _IDEVL;
                z80.Registers.B = 0;
                return;
            }

            var sectorNumber =
                memory[sectorAddress]
                + 256 * memory[sectorAddress + 1]
                + 256 * 256 * memory[sectorAddress + 2]
                + 256 * 256 * 256 * memory[sectorAddress + 3];

            if(sectorNumber > maxSectorNumber) {
                z80.Registers.A = _RNF;
                z80.Registers.B = 0;
                return;
            }

            diskImageFileStream.Seek(sectorNumber * 512, SeekOrigin.Begin);

            if(z80.Registers.CF)
                WriteSectors(sectorNumber, memoryAddress, numberOfSectors);
            else
                ReadSectors(sectorNumber, memoryAddress, numberOfSectors);

            z80.Registers.B = numberOfSectors;
            z80.Registers.A = 0;
        }

        private void ReadSectors(int sectorNumber, ushort memoryAddress, byte numberOfSectors)
        {
            var data = new byte[numberOfSectors * 512];
            diskImageFileStream.Read(data, 0, data.Length);

            SetMemoryContents(memoryAddress, data);
        }

        private void WriteSectors(int sectorNumber, ushort memoryAddress, byte numberOfSectors)
        {
            var data = new byte[numberOfSectors * 512];

            for(var i = 0; i < data.Length; i++)
                data[i] = memory[memoryAddress + i];

            diskImageFileStream.Write(data, 0, data.Length);
        }

        private void DEV_INFO()
        {
            var deviceIndex = z80.Registers.A;
            var infoBlockIndex = z80.Registers.B;
            var memoryAddress = z80.Registers.HL.ToUShort();

            if(deviceIndex != 1) {
                z80.Registers.A = 1; //Device not available
                return;
            }

            string info = null;

            if(infoBlockIndex == 0) {
                memory[memoryAddress] = 1; //One logical unit
                memory[memoryAddress+1] = 0; //Features
                z80.Registers.A = 0;
                return;
            }
            else if(infoBlockIndex == 1) {
                info = "Konamiman";
            }
            else if(infoBlockIndex == 2) {
                info = diskImageName;
            }
            else if(infoBlockIndex == 3) {
                info = "1";
            }
            else {
                z80.Registers.A = 1;
                return;
            }

            z80.Registers.A = 0;
            SetMemoryContents(memoryAddress, PaddedArrayFromString(info, 64));
        }

        private void SetMemoryContents(int memoryAddress, byte[] contents)
        {
            for(var i = 0; i < contents.Length; i++)
                memory[memoryAddress + i] = contents[i];
        }

        private void DEV_STATUS()
        {
            var deviceIndex = z80.Registers.A;
            var logicalUnit = z80.Registers.B;

            if(deviceIndex != 1 || logicalUnit > 1)
                z80.Registers.A = 0; //Invalid device/LUN
            else
                z80.Registers.A = 1; //Available and has not changed
        }

        private void LUN_INFO()
        {
            var deviceIndex = z80.Registers.A;
            var logicalUnit = z80.Registers.B;
            var memoryAddress = z80.Registers.HL.ToUShort();

            if(deviceIndex != 1 || logicalUnit > 1) {
                z80.Registers.A = 1; //Invalid device/LUN
                return;
            }

            var info = new byte[12];

            info[2] = 2; //sector size = 0x200

            var numberOfSectors = BitConverter.GetBytes(maxSectorNumber + 1);
            if(BitConverter.IsLittleEndian) {
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

        public void Dispose()
        {
            diskImageFileStream.Dispose();
        }
    }
}
