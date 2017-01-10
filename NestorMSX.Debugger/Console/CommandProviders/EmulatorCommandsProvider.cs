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

        private readonly byte[] ldirBytes = {0xED, 0xB0};

        private ushort oldpc;
        private MappedRam mappedRam;
        private IZ80Processor cpu;
        private IExternallyControlledSlotsSystem slotsSystem;

        public EmulatorCommandsProvider(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;
            this.cpu = pluginContext.Cpu;
            this.slotsSystem = pluginContext.SlotsSystem;

            return; //WIP
            //var cpu = pluginContext.Cpu;
            //var r = cpu.Registers;

            //cpu.BeforeInstructionExecution += (sender, args) => {
            //    if(args.Opcode.SequenceEqual(ldirBytes) 
            //        && cpu.Registers.PC != oldpc
            //        && cpu.Registers.BC >= 100
            //        //&& cpu.Registers.PC != 0x0CFF
            //        && cpu.Registers.DE.ToUShort() >= 0xC000)
            //    {
            //        oldpc = cpu.Registers.PC;
            //        Print($"At {r.PC:X4}: LDIR from {r.HL:X4} to {r.DE:X4}, {r.BC} bytes");
            //    }
            //};

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
