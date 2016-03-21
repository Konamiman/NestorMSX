using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Menus;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    public abstract class DiskImageBasedStoragePlugin : IDisposable
    {
        protected const int _IDEVL = 0xB5;
        protected const int _WPROT = 0xF8;
        protected const int _RNF = 0xF9;
        protected const int _DISK = 0xFD;

        protected abstract int MaxNumberOfDevices { get; }

        private readonly string kernelFilePath;
        protected IZ80Processor z80 { get; }
        protected SlotNumber slotNumber { get; }
        protected IExternallyControlledSlotsSystem memory { get; }
        private string basePathForDiskImages;
        private OpenFileDialog openFileDialog;
        private Form hostForm;
        private Action<object, MenuEntry> setMenuEntry;
        private MenuEntry[] setFileMenuEntries;
        private MenuEntry[] removeFileMenuEntries;
        private MenuEntry removeFileMainMenuEntry;
        private MenuEntry separatorBeforeRemoveFileMenuEntry;
        private string[] filesListInConfig;
        private string[] filesListFromState;
        private string stateFileFullPath;

        protected ImageFileInfo[] imageFiles { get; }

        private byte[] kernelContents;

        protected class ImageFileInfo
        {
            public string FullPath { get; set; }
            public FileStream Stream { get; set; }
            public long MaxSectorNumber { get; set; }
            public bool IsReadOnly { get; set; }
            public bool HasChanged { get; set; }
            public byte[] Dpb { get; set; }
        }

        protected abstract IMemory GetMemory(byte[] kernelFileContents);

        protected abstract string PluginDisplayName { get; }

        protected abstract string defaultKernelFileName { get; } 

        protected abstract void ValidateKernelFileContents(byte[] kernelFileContents);

        protected virtual void ApplyConfiguration(IDictionary<string, object> pluginConfig)
        { }

        public DiskImageBasedStoragePlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            ApplyConfiguration(pluginConfig);

            this.basePathForDiskImages =
                pluginConfig.GetValueOrDefault<string>("diskImagesDirectory", "").AsAbsolutePath();

            filesListInConfig = pluginConfig
                .GetValueOrDefault("diskImageFiles", new string[0])
                .Take(MaxNumberOfDevices)
                .Select(f => StringExtensions.AsAbsolutePath(f, basePathForDiskImages))
                .ToArray();

            this.slotNumber = new SlotNumber(pluginConfig.GetValue<byte>("NestorMSX.slotNumber"));

            var machineName = pluginConfig.GetValue<string>("NestorMSX.machineName");
            stateFileFullPath = $"{machineName}/{PluginDisplayName} in slot {slotNumber}.dat".AsAbsolutePath();
            if(File.Exists(stateFileFullPath))
                filesListFromState = File.ReadAllLines(stateFileFullPath, Encoding.UTF8)
                    .Take(MaxNumberOfDevices)
                    .ToArray()
                    .WithEmptiesAsNulls();
            else
                filesListFromState = filesListInConfig.WithMinimumSizeOf(MaxNumberOfDevices);

            imageFiles = new ImageFileInfo[MaxNumberOfDevices];

            this.setMenuEntry = context.SetMenuEntry;
            CreateMenu();

            this.openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select disk image file";
            openFileDialog.InitialDirectory = basePathForDiskImages.Replace('/', Path.DirectorySeparatorChar);
            openFileDialog.Filter = "Disk image files|*.dsk;*.img|All files|*.*";
            this.hostForm = context.HostForm;

            this.kernelFilePath = pluginConfig.GetMachineFilePath(pluginConfig.GetValueOrDefault("kernelFile", defaultKernelFileName));

            for (int i = 0; i < filesListFromState.Length; i++) {
                if(filesListFromState[i] == null) continue;
                var diskImageFilePath = filesListFromState[i].AsAbsolutePath(basePathForDiskImages);
                SetFile(diskImageFilePath, i + 1, Path.GetFileName(diskImageFilePath));
            }

            for (int i = 0; i < MaxNumberOfDevices; i++)
                removeFileMenuEntries[i].IsVisible = imageFiles[i] != null;

            this.z80 = context.Cpu;
            this.memory = context.SlotsSystem;
            
            z80.BeforeInstructionFetch += (sender, args) => BeforeZ80InstructionFetch(z80.Registers.PC);

            this.kernelContents = File.ReadAllBytes(kernelFilePath);
            ValidateKernelFileContents(kernelContents);
        }

        private void SetAllFilesFromConfig()
        {
            for(int i=1; i<=MaxNumberOfDevices; i++)
                RemoveDiskImageFile(i, false);

            for(int i = 0; i < filesListInConfig.Length; i++) {
                var diskImageFilePath = filesListInConfig[i].AsAbsolutePath(basePathForDiskImages);
                SetFile(diskImageFilePath, i + 1, Path.GetFileName(diskImageFilePath));
            }

            if(File.Exists(stateFileFullPath))
                File.Delete(stateFileFullPath);
        }

        private void SetFile(string fullPath, int deviceIndex, string displayName)
        {
            var info = new ImageFileInfo();
            info.FullPath = fullPath;

            try {
                var fileAccess = new FileInfo(fullPath).IsReadOnly ? FileAccess.Read : FileAccess.ReadWrite;
                info.Stream = File.Open(fullPath, FileMode.Open, fileAccess);
                info.IsReadOnly = fileAccess == FileAccess.Read;
            } catch(Exception ex) {
                var message = $"Can't open disk image file:\r\n{fullPath}\r\n\r\n{ex.Message}";
                MessageBox.Show(message, $"NestorMSX - {PluginDisplayName} in slot {slotNumber}");
                return;
            }

            info.MaxSectorNumber = ((new FileInfo(fullPath)).Length) / 512 - 1;

            RemoveDiskImageFile(deviceIndex, false);
            imageFiles[deviceIndex - 1] = info;
            
            setFileMenuEntries[deviceIndex - 1].Title = $"{deviceIndex}: {Path.GetFileName(fullPath)}";
            removeFileMenuEntries[deviceIndex - 1].IsVisible = true;
            removeFileMainMenuEntry.IsVisible = true;
            separatorBeforeRemoveFileMenuEntry.IsVisible = true;

            UpdateStateFile();
        }

        private void CreateMenu()
        {
            var menuEntries = Enumerable.Range(1, MaxNumberOfDevices).Select(i =>
                new MenuEntry($"{i}: {(imageFiles[i-1] == null ? "(no file)" : Path.GetFileName(imageFiles[i-1].FullPath))}",
                    () => AskAndSetFileForDevice(i)))
                .ToList();

            separatorBeforeRemoveFileMenuEntry = MenuEntry.CreateSeparator(isVisible: false);
            menuEntries.Add(separatorBeforeRemoveFileMenuEntry);

            removeFileMenuEntries = Enumerable.Range(1, MaxNumberOfDevices).Select(i =>
                new MenuEntry(i.ToString(), () => RemoveDiskImageFile(i, true)) {IsVisible = false})
                .ToArray();

            removeFileMainMenuEntry = new MenuEntry("Remove image file", removeFileMenuEntries) { IsVisible = false };
            menuEntries.Add(removeFileMainMenuEntry);

            if(filesListInConfig.Length > 0) 
                menuEntries.Add(new MenuEntry("Set all from config", SetAllFilesFromConfig));

            var mainEntry = new MenuEntry($"{PluginDisplayName} in slot {slotNumber}", menuEntries);

            this.setFileMenuEntries = menuEntries.ToArray();
            setMenuEntry(this, mainEntry);
        }

        private void RemoveDiskImageFile(int deviceIndex, bool updateStateFile)
        {
            var fileIndex = deviceIndex - 1;

            if(imageFiles[fileIndex] != null) {
                try
                {
                    imageFiles[fileIndex].Stream.Dispose();
                }
                catch
                {
                }
                imageFiles[fileIndex] = null;
            }

            setFileMenuEntries[fileIndex].Title = $"{deviceIndex}: (no file)";
            removeFileMenuEntries[fileIndex].IsVisible = false;

            removeFileMainMenuEntry.IsVisible = removeFileMenuEntries.Any(m => m.IsVisible);
            separatorBeforeRemoveFileMenuEntry.IsVisible =
                removeFileMainMenuEntry.IsVisible ||
                filesListInConfig.Length > 0;

            if(updateStateFile)
                UpdateStateFile();
        }

        private void AskAndSetFileForDevice(int deviceIndex)
        {
            var dialogResult = openFileDialog.ShowDialog(hostForm);
            if(dialogResult != DialogResult.OK)
                return;

            var fileIndex = deviceIndex - 1;
            var hadPreviousFile = imageFiles[fileIndex] != null;
            SetFile(openFileDialog.FileName, deviceIndex, openFileDialog.SafeFileName);
            imageFiles[fileIndex].HasChanged = true;// hadPreviousFile;

            UpdateStateFile();
        }

        private void UpdateStateFile()
        {
            try {
                var directory = Path.GetDirectoryName(stateFileFullPath);
                if(!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var filesList = imageFiles.Select(f => f == null ? "" : f.FullPath).ToArray();
                File.WriteAllLines(stateFileFullPath, filesList);
                filesListFromState = filesList.WithEmptiesAsNulls();
            }
            catch(Exception ex) { }
        }

        protected byte[] PaddedArrayFromString(string theString, int totalLength)
        {
            if(theString.Length > totalLength)
                theString = theString.Remove(totalLength);
            return Encoding.ASCII.GetBytes(theString.PadRight(totalLength));
        }

        public IMemory GetMemory()
        {
            return GetMemory(kernelContents);
        }

        protected abstract void BeforeZ80InstructionFetch(ushort instructionAddress);
        
        protected bool IsValidDevice(int deviceIndex)
        {
            return deviceIndex >= 1 && deviceIndex <= MaxNumberOfDevices && imageFiles[deviceIndex - 1] != null;
        }

        protected void ReadSectors(int sectorNumber, ushort memoryAddress, byte numberOfSectors, FileStream stream)
        {
            var data = new byte[numberOfSectors * 512];
            stream.Read(data, 0, data.Length);

            SetMemoryContents(memoryAddress, data);
        }

        protected void WriteSectors(int sectorNumber, ushort memoryAddress, byte numberOfSectors, FileStream stream)
        {
            var data = new byte[numberOfSectors * 512];

            for(var i = 0; i < data.Length; i++)
                data[i] = memory[memoryAddress + i];

            stream.Write(data, 0, data.Length);
            stream.Flush(true);
        }
        
        protected void SetMemoryContents(int memoryAddress, byte[] contents)
        {
            for(var i = 0; i < contents.Length; i++)
                memory[memoryAddress + i] = contents[i];
        }

        public void Dispose()
        {
            foreach(var file in imageFiles.Where(f => f!=null))
                file.Stream.Dispose();
        }
    }
}