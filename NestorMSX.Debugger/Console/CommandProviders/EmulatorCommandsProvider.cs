using System;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    [Name("emulator")]
    public class EmulatorCommandsProvider : IAsyncMessagePrinter
    {
        private readonly PluginContext pluginContext;

        public EmulatorCommandsProvider(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;
            var cpu = pluginContext.Cpu;
            cpu.BeforeInstructionFetch += (sender, args) =>
            {
                if (cpu.Registers.PC == 5 && (cpu.Registers.C == 0x40 || cpu.Registers.C == 0x41))
                {
                    PrintMessageRequest?.Invoke(this, new PrintMessageRequestEventArgs(cpu.Registers.C));
                }
            };
        }

        public void Vpoke(int address, byte value)
        {
            pluginContext.Vdp.WriteVram(address, value);
        }

        public byte Vpeek(int address)
        {
            return pluginContext.Vdp.ReadVram(address);
        }

        public string Slots()
        {
            return $"{pluginContext.SlotsSystem.GetCurrentSlot(0)} {pluginContext.SlotsSystem.GetCurrentSlot(1)} {pluginContext.SlotsSystem.GetCurrentSlot(2)} {pluginContext.SlotsSystem.GetCurrentSlot(3)}";
        }

        public event EventHandler<PrintMessageRequestEventArgs> PrintMessageRequest;
    }
}
