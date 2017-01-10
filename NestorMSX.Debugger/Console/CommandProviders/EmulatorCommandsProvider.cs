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

        private Dictionary<int, string> dosCallNames = new Dictionary<int, string>()
        {
            {0x00, "TERM0"},
            {0x01, "CONIN"},
            {0x02, "CONOUT"},
            {0x03, "AUXIN"},
            {0x04, "AUXOUT"},
            {0x05, "LSTOUT"},
            {0x06, "DIRIO"},
            {0x07, "DIRIN"},
            {0x08, "INNOE"},
            {0x09, "STROUT"},
            {0x0A, "BUFIN"},
            {0x0B, "CONST"},
            {0x0C, "CPMVER"},
            {0x0D, "DSKRST"},
            {0x0E, "SELDSK"},
            {0x0F, "FOPEN"},
            {0x10, "FCLOSE"},
            {0x11, "SFIRST"},
            {0x12, "SNEXT"},
            {0x13, "FDEL"},
            {0x14, "RDSEQ"},
            {0x15, "WRSEQ"},
            {0x16, "FMAKE"},
            {0x17, "FREN"},
            {0x18, "LOGIN"},
            {0x19, "CURDRV"},
            {0x1A, "SETDTA"},
            {0x1B, "ALLOC"},
            {0x21, "RDRND"},
            {0x22, "WRRND"},
            {0x23, "FSIZE"},
            {0x24, "SETRND"},
            {0x26, "WRBLK"},
            {0x27, "RDBLK"},
            {0x28, "WRZER"},
            {0x2A, "GDATE"},
            {0x2B, "SDATE"},
            {0x2C, "GTIME"},
            {0x2D, "STIME"},
            {0x2E, "VERIFY"},
            {0x2F, "RDABS"},
            {0x30, "WRABS"},
            {0x31, "DPARM"},
            {0x40, "FFIRST"},
            {0x41, "FNEXT"},
            {0x42, "FNEW"},
            {0x43, "OPEN"},
            {0x44, "CREATE"},
            {0x45, "CLOSE"},
            {0x46, "ENSURE"},
            {0x47, "DUP"},
            {0x48, "READ"},
            {0x49, "WRITE"},
            {0x4A, "SEEK"},
            {0x4B, "IOCTL"},
            {0x4C, "HTEST"},
            {0x4D, "DELETE"},
            {0x4E, "RENAME"},
            {0x4F, "MOVE"},
            {0x50, "ATTR"},
            {0x51, "FTIME"},
            {0x52, "HDELETE"},
            {0x53, "HRENAME"},
            {0x54, "HMOVE"},
            {0x55, "HATTR"},
            {0x56, "HFTIME"},
            {0x57, "GETDTA"},
            {0x58, "GETVFY"},
            {0x59, "GETCD"},
            {0x5A, "CHDIR"},
            {0x5B, "PARSE"},
            {0x5C, "PFILE"},
            {0x5D, "CHKCHR"},
            {0x5E, "WPATH"},
            {0x5F, "FLUSH"},
            {0x60, "FORK"},
            {0x61, "JOIN"},
            {0x62, "TERM"},
            {0x63, "DEFAB"},
            {0x64, "DEFER"},
            {0x65, "ERROR"},
            {0x66, "EXPLAIN"},
            {0x67, "FORMAT"},
            {0x68, "RAMD"},
            {0x69, "BUFFER"},
            {0x6A, "ASSIGN"},
            {0x6B, "GENV"},
            {0x6C, "SENV"},
            {0x6D, "FENV"},
            {0x6E, "DSKCHK"},
            {0x6F, "DOSVER"},
            {0x70, "REDIR"}
        };

        public EmulatorCommandsProvider(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;
            this.cpu = pluginContext.Cpu;
            this.slotsSystem = pluginContext.SlotsSystem;

            return; //WIP
            var cpu = pluginContext.Cpu;
            var r = cpu.Registers;

            cpu.BeforeInstructionExecution += (sender, args) => {
                if(args.Opcode.SequenceEqual(ldirBytes) 
                    && cpu.Registers.PC != oldpc
                    && cpu.Registers.BC >= 100
                    //&& cpu.Registers.PC != 0x0CFF
                    && cpu.Registers.DE.ToUShort() >= 0xC000)
                {
                    oldpc = cpu.Registers.PC;
                    Print($"At {r.PC:X4}: LDIR from {r.HL:X4} to {r.DE:X4}, {r.BC} bytes");
                }
            };

            cpu.BeforeInstructionFetch += (sender, args) => {
                mappedRam = mappedRam ?? pluginContext.SlotsSystem.GetSlotContents(new SlotNumber(3,2)) as MappedRam;
                if(cpu.Registers.PC == 5 && mappedRam.GetBlockInBank(0) == 3
                    && r.C != 2) {
                    Print($"DOS call: {r.C:X2}h - {DosCallName(r.C)}");
                    if (r.C == 0x5F) {
                        Print($"   B={r.B:X2}, D={r.D:X2}");
                        pluginContext.Cpu.ExecuteRet();
                    }
                }
            };
        }

        private string DosCallName(byte code)
        {
            return dosCallNames.ContainsKey(code) ?
                dosCallNames[code] :
                $"{code:X2}h";
        }

        private void Print(object value)
        {
            PrintMessageRequest?.Invoke(this, new PrintMessageRequestEventArgs(value));
        }

        public Func<string, object> EvaluateExpression { get; set; }

        public event EventHandler<PrintMessageRequestEventArgs> PrintMessageRequest;
    }
}
