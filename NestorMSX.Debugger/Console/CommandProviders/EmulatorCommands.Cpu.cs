using System;
using System.Text;
using System.Threading.Tasks;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public partial class EmulatorCommandsProvider
    {
        EventHandler<BeforeInstructionFetchEventArgs> pauser = null;

        public string Pause()
        {
            if (cpu.State != ProcessorState.Running)
                return "Already paused";
            
            pauser = (sender, args) =>
                {
                    args.ExecutionStopper.Stop(true);
                    cpu.BeforeInstructionFetch -= pauser;
                    Print($"Paused at 0x{regs.PC:X2}");
                };

            cpu.BeforeInstructionFetch += pauser;
            return null;
        }

        public string Continue()
        {
            if (cpu.State == ProcessorState.Running)
                return "Already running";

            new Task(() => cpu.Continue()).Start();
            return "Ok";
        }

        public string Status()
        {
            var sb = new StringBuilder();

            sb.Append(cpu.State == ProcessorState.Running
                ? "Running\r\n\r\n"
                : $"Paused at 0x{regs.PC:X2}\r\n\r\n");

            sb.Append($"Slots: {slotsSystem.GetCurrentSlot(0)} {slotsSystem.GetCurrentSlot(1)} {slotsSystem.GetCurrentSlot(2)} {slotsSystem.GetCurrentSlot(3)}\r\n");
            sb.Append($"Segms: {mappedRam.GetBlockInBank(0)} {mappedRam.GetBlockInBank(1)} {mappedRam.GetBlockInBank(2)} {mappedRam.GetBlockInBank(3)}");
            return sb.ToString();
        }
    }
}
