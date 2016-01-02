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
    public class NextorPlugin
    {
        private readonly string kernelFilePath;
        private IZ80Processor z80;
        private SlotNumber slotNumber;
        private IExternallyControlledSlotsSystem slotsSystem;
        private string diskImageFilePath;
        private FileStream diskImageFileStream;
        private long maxSectorNumber;
        private byte[] driverNameBytes;

        private IDictionary<ushort, Action> kernelRoutines;

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

            this.kernelFilePath = pluginConfig.GetPluginFilePath("kernelFile");
            this.diskImageFilePath = pluginConfig.GetValue<string>("diskImageFile").AsAbsolutePath();
            this.z80 = context.Cpu;
            this.slotsSystem = context.SlotsSystem;
            this.slotNumber = new SlotNumber(pluginConfig.GetValue<byte>("slot"));

            this.maxSectorNumber = ((new FileInfo(diskImageFilePath)).Length) / 512 - 1;
            this.diskImageFileStream = File.Open(diskImageFilePath, FileMode.Open, FileAccess.ReadWrite);

            z80.BeforeInstructionFetch += Z80_BeforeInstructionFetch;
            z80.MemoryAccess += Z80_MemoryAccess;

            this.driverNameBytes = Encoding.ASCII.GetBytes("NestorMSX Nextor plugin".PadRight(32));
        }

        private void Z80_MemoryAccess(object sender, MemoryAccessEventArgs e)
        {
            if(e.EventType != MemoryAccessEventType.BeforeMemoryRead)
                return;

            if(slotsSystem.GetCurrentSlot(1) != slotNumber)
                return;

            if(e.Address == 0x410E) {
                e.CancelMemoryAccess = true;
                e.Value = 1;    //Device-based driver
            }

            if(e.Address >= 0x4110 && e.Address < (0x4110 + 32)) {
                e.CancelMemoryAccess = true;
                e.Value = driverNameBytes[e.Address - 0x4110];
            }
        }

        public IMemory GetMemory()
        {
            return new Ascii8Rom(File.ReadAllBytes(kernelFilePath));
        }

        private void Z80_BeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs e)
        {
            if(slotsSystem.GetCurrentSlot(1) != slotNumber)
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
        }

        private void DEV_INFO()
        {
        }

        private void DEV_STATUS()
        {
        }

        private void LUN_INFO()
        {
        }
    }
}
