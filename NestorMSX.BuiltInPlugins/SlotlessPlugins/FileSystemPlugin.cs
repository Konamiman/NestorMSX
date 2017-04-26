using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.SlotlessPlugins
{
    [NestorMSXPlugin("Filesystem integration")]
    public class FileSystemPlugin
    {
        private const byte _NODIR = 0xD6;
        private const byte _NOFIL = 0xD7;

        private static FileSystemPlugin Instance;
        private string currentFullDir;
        private string currentRelativeDir;

        private readonly string basePath;
        private readonly string volumeLabel;
        private readonly IZ80Processor cpu;
        private readonly IZ80Registers regs;
        private readonly IMemory mem;
        private readonly IExternallyControlledSlotsSystem slotsSystem;

        private Dictionary<int, Action> hookedMethods;

        public static FileSystemPlugin GetInstance(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            if (Instance == null)
                Instance = new FileSystemPlugin(context, pluginConfig);

            return Instance;
        }

        private void ALLOC()
        {
            regs.HL = 0;
            regs.DE = 0;
            regs.C = 1;
            regs.A = 0;
        }

        private FileSystemInfo[] fsinfos;

        private void FFIRST()
        {
            var searchPath = ExtractString(regs.IY, 63);
            string fullSearchPath, directory, fileName;
            if (searchPath != "")
            {
                fullSearchPath = Path.Combine(basePath, searchPath);
                directory = Path.GetDirectoryName(fullSearchPath);
                fileName = Path.GetFileName(fullSearchPath);
            }
            else
            {
                fullSearchPath = basePath;
                directory = basePath;
                fileName = "*.*";
            }

            DirectoryInfo di = new DirectoryInfo(directory);
            fsinfos = di.GetFileSystemInfos(fileName)
                .Where(e => e.Name.Length <= 12)
                .ToArray();
            
            FillFIB(0);
        }

        private void FillFIB(int entryIndex)
        {
            var fib = regs.IX.ToUShort();
            if (fsinfos == null || entryIndex >= fsinfos.Length)
            {
                regs.A = _NOFIL;
                return;
            }

            var entry = fsinfos[entryIndex];
            SetString(regs.IX + 1, entry.Name);
            long length = (entry as FileInfo)?.Length ?? 0;

            var lengthBytes = BitConverter.GetBytes(length);
            if (!BitConverter.IsLittleEndian) lengthBytes = lengthBytes.Reverse().ToArray();
            mem[fib + 21] = lengthBytes[0];
            mem[fib + 22] = lengthBytes[1];
            mem[fib + 23] = lengthBytes[2];
            mem[fib + 24] = lengthBytes[3];

            mem[fib + 14] = (byte)((int)entry.Attributes & 0x37); //Read only, Hidden, System, Directory, Archive

            var time = entry.LastWriteTime;
            mem[fib + 16] = (byte)((time.Hour << 3) | (time.Minute >> 3));
            mem[fib + 15] = (byte)((byte)((time.Minute & 7) << 5) | (time.Second / 2));
            mem[fib + 18] = (byte)((byte)(time.Year - 1980) << 1 | (time.Month >> 3 ));
            mem[fib + 17] = (byte)((time.Month << 5) | time.Day);

            regs.A = 0;
        }

        private void FNEXT()
        {
            var fib = regs.IX.ToUShort();
            var entryIndex = NumberUtils.CreateUshort(mem[fib + 26], mem[fib + 27]) + 1;
            FillFIB(entryIndex);
        }

        private void CHDIR()
        {
            var msxFullDirPath = ExtractString(regs.HL, 63);
            var myFullDirPath = Path.Combine(basePath, msxFullDirPath);
            if (!Directory.Exists(myFullDirPath))
            {
                regs.A = _NODIR;
                return;
            }

            currentRelativeDir = msxFullDirPath;
            currentFullDir = myFullDirPath;
            regs.A = 0;
        }

        private void GETCD()
        {
            SetString(regs.DE, currentRelativeDir);
            regs.A = 0;
            regs.CF = 0;
        }

        private void GETVOL()
        {
            SetString(regs.DE, volumeLabel);
            regs.A = 0;
            regs.CF = 0;
        }

        private FileSystemPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            var defaultConfig = new Dictionary<string, object>
            {
                {"integratedDirectory", "$MyDocuments$/NestorMSX/FileSystem"},
                {"volumeLabel", "NestorMSX"}
            };

            defaultConfig.MergeInto(pluginConfig);

            basePath = pluginConfig.GetValue<string>("integratedDirectory")
                .Replace("/","\\")
                .AsAbsolutePath();
            volumeLabel = pluginConfig.GetValue<string>("volumeLabel");
            currentFullDir = basePath;
            currentRelativeDir = "";

            this.cpu = context.Cpu;
            this.regs = cpu.Registers;
            this.slotsSystem = context.SlotsSystem;
            this.mem = this.slotsSystem;
            cpu.BeforeInstructionFetch += CpuOnBeforeInstructionFetch;

            hookedMethods = new Dictionary<int, Action>
            {
                {0x4030, ALLOC},
                {0x4033, FFIRST},
                {0x4036, FNEXT},
                {0x4039, CHDIR},
                {0x403C, GETCD},
                {0x403F, GETVOL}
            };
        }

        private int? segmentNumber = null;
        private IMappedRam mappedRam;

        private void CpuOnBeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs e)
        {
            if (!hookedMethods.ContainsKey(regs.PC))
                return;

            if (segmentNumber == null &&
                (mappedRam = slotsSystem.GetSlotContents(slotsSystem.GetCurrentSlot(1)) as IMappedRam) != null &&
                ExtractString(0x4000, 14) == "FileSysDriver")
            {
                segmentNumber = mappedRam.GetBlockInBank(1);
            }

            if (segmentNumber != null &&
                mappedRam?.GetBlockInBank(1) == segmentNumber.Value &&
                slotsSystem.GetSlotContents(slotsSystem.GetCurrentSlot(1)) == mappedRam)
            {
                hookedMethods[regs.PC]();
                cpu.ExecuteRet();
            }
        }

        private string ExtractString(int address, int maxLength = 256)
        {
            var bytes = new List<byte>();
            byte theByte;
            while (((theByte = cpu.Memory[address]) != 0) && bytes.Count < maxLength)
            {
                bytes.Add(theByte);
                address++;
            }

            return Encoding.ASCII.GetString(bytes.ToArray());
        }

        private void SetString(int address, string theString, bool zeroTerm = true)
        {
            var bytes = Encoding.ASCII.GetBytes(theString);
            foreach (var theChar in bytes)
            {
                cpu.Memory[address] = theChar;
                address++;
            }

            if (zeroTerm) cpu.Memory[address] = 0;
        }
    }
}
