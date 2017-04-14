using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Misc;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    [Name("emulator")]
    public partial class EmulatorCommandsProvider : IAsyncMessagePrinter
    {
        private readonly PluginContext pluginContext;
        private readonly IZ80Registers regs;

        private readonly byte[] ldirBytes = {0xED, 0xB0};
        private readonly byte[] breakpBytes = { 0xED, 0x00 };

        private MappedRam _mappedRam;

        private MappedRam mappedRam =>
            _mappedRam ?? (_mappedRam = slotsSystem.GetSlotContents(slotsSystem.GetCurrentSlot(3)) as MappedRam);


        private readonly IZ80Processor cpu;
        private readonly IExternallyControlledSlotsSystem slotsSystem;

        public EmulatorCommandsProvider(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;
            this.cpu = pluginContext.Cpu;
            this.slotsSystem = pluginContext.SlotsSystem;
            this.regs = cpu.Registers;

            cpu.BeforeInstructionExecution += (sender, args) =>
            {
                if (args.Opcode.SequenceEqual(breakpBytes))
                {
                    Print($"BREAKP: PC={regs.PC:X4}h, DE={regs.DE:X4}h, {ExtractString(regs.DE)}");
                }
            };

            //cpu.BeforeInstructionFetch += (sender, args) => {
            //    mappedRam = mappedRam ?? pluginContext.SlotsSystem.GetSlotContents(new SlotNumber(3,2)) as MappedRam;
            //    if(cpu.Registers.PC == 5 && mappedRam.GetBlockInBank(0) == 3
            //        && r.C != 2) {
            //        Print($"DOS call: {r.C:X2}h - {DosCallName(r.C)}");
            //        if (r.C == 0x5F) {
            //            Print($"   B={r.B:X2}, D={r.D:X2}");
            //            pluginContext.Cpu.ExecuteRet();
            //        }
            //    }
            //};
        }

        //private string DosCallName(byte code)
        //{
        //    return dosCallNames.ContainsKey(code) ?
        //        dosCallNames[code] :
        //        $"{code:X2}h";
        //}

        private void Print(object value)
        {
            PrintMessageRequest?.Invoke(this, new PrintMessageRequestEventArgs(value));
        }

        public Func<string, object> EvaluateExpression { get; set; }

        public event EventHandler<PrintMessageRequestEventArgs> PrintMessageRequest;
    }
}
