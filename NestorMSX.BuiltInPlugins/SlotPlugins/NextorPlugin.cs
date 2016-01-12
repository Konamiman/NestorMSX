using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Menus;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;
using System.Linq;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Nextor")]
    public class NextorPlugin : IDisposable
    {
        private const int _IDEVL = 0xB5;
        private const int _RNF = 0xF9;
        private const int maxDeviceNumber = 7;

        private readonly string kernelFilePath;
        private IZ80Processor z80;
        private SlotNumber slotNumber;
        private IExternallyControlledSlotsSystem memory;
        private string diskImageName;
        private string[] diskImageNames;
        private FileStream[] diskImageFileStreams;
        private long[] maxSectorNumbers;
        private string basePathForDiskImages;
        private OpenFileDialog openFileDialog;
        private Form hostForm;
        private Action<object, MenuEntry> setMenuEntry;
        private MenuEntry[] menuEntries;
        private bool[] imageFilesChanged = new bool[maxDeviceNumber];
        private MenuEntry[] removeFileMenuEntries;

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

            this.diskImageNames = new string[maxDeviceNumber];
            this.diskImageFileStreams = new FileStream[maxDeviceNumber];
            this.maxSectorNumbers = new long[maxDeviceNumber];

            this.basePathForDiskImages =
                pluginConfig.GetValueOrDefault<string>("diskImagesDirectory", "").AsAbsolutePath();

            this.setMenuEntry = context.SetMenuEntry;
            CreateMenu();

            this.openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select disk image file";
            openFileDialog.InitialDirectory = basePathForDiskImages.Replace('/', Path.DirectorySeparatorChar);
            openFileDialog.Filter = "Disk image files|*.dsk;*.img|All files|*.*";
            this.hostForm = context.HostForm;

            this.kernelFilePath = pluginConfig.GetMachineFilePath(pluginConfig.GetValue<string>("kernelFile"));

            var imageFiles = pluginConfig.GetValue<string[]>("diskImageFiles");
            for(int i = 0; i < imageFiles.Length; i++) {
                var diskImageFilePath = imageFiles[i].AsAbsolutePath(basePathForDiskImages);
                SetFile(diskImageFilePath, i+1);
            }
            
            for(int i = 0; i < maxDeviceNumber; i++)
                removeFileMenuEntries[i].IsVisible = diskImageFileStreams[i] != null;

            this.z80 = context.Cpu;
            this.memory = context.SlotsSystem;
            this.slotNumber = new SlotNumber(pluginConfig.GetValue<byte>("slotNumber"));

            z80.BeforeInstructionFetch += Z80_BeforeInstructionFetch;

            this.kernel = new Ascii8Rom(ReadKernelFile());
        }

        private void SetFile(string fullPath, int deviceIndex)
        {
            this.diskImageNames[deviceIndex-1] = Path.GetFileName(fullPath);

            RemoveDiskImageFile(deviceIndex);

            this.diskImageFileStreams[deviceIndex-1] = File.Open(fullPath, FileMode.Open, FileAccess.ReadWrite);
            this.maxSectorNumbers[deviceIndex-1] = ((new FileInfo(fullPath)).Length) / 512 - 1;

            menuEntries[deviceIndex - 1].Title = $"{deviceIndex}: {openFileDialog.SafeFileName}";
            removeFileMenuEntries[deviceIndex - 1].IsVisible = true;
        }

        private void CreateMenu()
        {
            var menuEntries = Enumerable.Range(1, 7).Select(i =>
                new MenuEntry($"{i}: {(diskImageNames[i-1] == null ? "(no file)" : diskImageNames[i-1])}",
                () => AskAndSetFileForDevice(i)))
                .ToList();

            menuEntries.Add(new MenuEntry("-", () => { }));

            removeFileMenuEntries = Enumerable.Range(1, 7).Select(i =>
                new MenuEntry(i.ToString(), () => RemoveDiskImageFile(i)) {IsVisible = false})
                .ToArray();

            menuEntries.Add(new MenuEntry("Remove image file", removeFileMenuEntries));

            var mainEntry = new MenuEntry($"Nextor in slot {slotNumber}", menuEntries);

            this.menuEntries = menuEntries.ToArray();
            setMenuEntry(this, mainEntry);
        }

        private void RemoveDiskImageFile(int deviceIndex)
        {
            if(diskImageFileStreams[deviceIndex - 1] != null) {
                diskImageFileStreams[deviceIndex - 1].Dispose();
                diskImageFileStreams[deviceIndex - 1] = null;
            }

            menuEntries[deviceIndex - 1].Title = $"{deviceIndex}: (no file)";
            removeFileMenuEntries[deviceIndex - 1].IsVisible = false;
        }

        private void AskAndSetFileForDevice(int deviceIndex)
        {
            var dialogResult = openFileDialog.ShowDialog(hostForm);
            if(dialogResult != DialogResult.OK)
                return;

            var hadPreviousFile = diskImageFileStreams[deviceIndex - 1] != null;
            SetFile(openFileDialog.FileName, deviceIndex);
            
            imageFilesChanged[deviceIndex - 1] = hadPreviousFile;
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

            if(!IsValidDevice(deviceIndex) || logicalUnit != 1) {
                z80.Registers.A = _IDEVL;
                z80.Registers.B = 0;
                return;
            }

            var sectorNumber =
                memory[sectorAddress]
                + 256 * memory[sectorAddress + 1]
                + 256 * 256 * memory[sectorAddress + 2]
                + 256 * 256 * 256 * memory[sectorAddress + 3];

            if(sectorNumber > maxSectorNumbers[deviceIndex-1]) {
                z80.Registers.A = _RNF;
                z80.Registers.B = 0;
                return;
            }

            diskImageFileStreams[deviceIndex-1].Seek(sectorNumber * 512, SeekOrigin.Begin);

            if(z80.Registers.CF)
                WriteSectors(sectorNumber, memoryAddress, numberOfSectors, diskImageFileStreams[deviceIndex - 1]);
            else
                ReadSectors(sectorNumber, memoryAddress, numberOfSectors, diskImageFileStreams[deviceIndex - 1]);

            z80.Registers.B = numberOfSectors;
            z80.Registers.A = 0;
        }

        private bool IsValidDevice(byte deviceIndex)
        {
            return deviceIndex >= 1 && deviceIndex <= maxDeviceNumber && diskImageFileStreams[deviceIndex - 1] != null;
        }

        private void ReadSectors(int sectorNumber, ushort memoryAddress, byte numberOfSectors, FileStream stream)
        {
            var data = new byte[numberOfSectors * 512];
            stream.Read(data, 0, data.Length);

            SetMemoryContents(memoryAddress, data);
        }

        private void WriteSectors(int sectorNumber, ushort memoryAddress, byte numberOfSectors, FileStream stream)
        {
            var data = new byte[numberOfSectors * 512];

            for(var i = 0; i < data.Length; i++)
                data[i] = memory[memoryAddress + i];

            stream.Write(data, 0, data.Length);
            stream.Flush(true);
        }

        private void DEV_INFO()
        {
            var deviceIndex = z80.Registers.A;
            var infoBlockIndex = z80.Registers.B;
            var memoryAddress = z80.Registers.HL.ToUShort();

            if(!IsValidDevice(deviceIndex)) {
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
                info = diskImageNames[deviceIndex-1];
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

            if(!IsValidDevice(deviceIndex) || logicalUnit != 1)
                z80.Registers.A = 0; //Invalid device/LUN
            else if(imageFilesChanged[deviceIndex - 1]) {
                z80.Registers.A = 2; //Available and has changed
                imageFilesChanged[deviceIndex - 1] = false;
            }
            else
                z80.Registers.A = 1; //Available and has not changed
        }

        private void LUN_INFO()
        {
            var deviceIndex = z80.Registers.A;
            var logicalUnit = z80.Registers.B;
            var memoryAddress = z80.Registers.HL.ToUShort();

            if(!IsValidDevice(deviceIndex) || logicalUnit != 1) {
                z80.Registers.A = 1; //Invalid device/LUN
                return;
            }

            var info = new byte[12];

            info[2] = 2; //sector size = 0x200
            info[7] = 1; //removable

            var numberOfSectors = BitConverter.GetBytes(maxSectorNumbers[deviceIndex-1] + 1);
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
            foreach(var fileStream in diskImageFileStreams)
                if(fileStream != null)
                    fileStream.Dispose();
        }
    }
}
