using System;
using System.Collections.Generic;
using Konamiman.Z80dotNet;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using ConfigurationException = Konamiman.NestorMSX.Exceptions.ConfigurationException;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("MSX-DOS")]
    public class MsxDosPlugin : DiskImageBasedStoragePlugin
    {
        private IDictionary<ushort, Action> kernelRoutines;

        public MsxDosPlugin(PluginContext context, IDictionary<string, object> pluginConfig) 
            : base(context, pluginConfig)
        {
            kernelRoutines = new Dictionary<ushort, Action>
            {
                { 0x4010, DSKIO },
                { 0x4013, DSKCHG },
                { 0x4016, GETDPB },
                { 0x4019, CHOICE },
                { 0x401C, DSKFMT },
                { 0x401F, MTOFF  }
            };
        }

        protected override int MaxNumberOfDevices
        {
            get { return 2; }
        }

        protected override string PluginDisplayName
        {
            get { return "MSX-DOS"; }
        }

        protected override void BeforeZ80InstructionFetch(ushort instructionAddress)
        {
            if (memory.GetCurrentSlot(1) != slotNumber)
                return;

            if (kernelRoutines.ContainsKey(z80.Registers.PC))
            {
                var routine = kernelRoutines[z80.Registers.PC];
                routine();
                z80.Registers.CF = 1; //temp
                z80.Registers.A = 2;  //temp
                z80.ExecuteRet();
            }
        }

        protected override IMemory GetMemory(byte[] kernelFileContents)
        {
            return new PlainRom(kernelFileContents, 1);
        }

        protected override void ValidateKernelFileContents(byte[] kernelFileContents)
        {
            if(kernelFileContents.Length < 16*1024)
                throw new ConfigurationException(
                    "Invalid kernel file: a Nextor kernel always has a size of at most 16K.");
        }

        void DSKIO()
        {
        }

        void DSKCHG()
        {
        }

        void GETDPB()
        {
        }

        void CHOICE()
        {
        }

        void DSKFMT()
        {
        }

        void MTOFF()
        {
        }
    }
}
