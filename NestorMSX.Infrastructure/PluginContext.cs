using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Host;
using Konamiman.Z80dotNet;
using System.Windows.Forms;

namespace Konamiman.NestorMSX
{
    public class PluginContext
    {
        public IZ80Processor Cpu { get; set; }

        public IExternallyControlledSlotsSystem SlotsSystem { get; set; }

        public IExternallyControlledTms9918 Vdp { get; set; }

        public Form HostForm { get; set; }

        public IKeyEventSource KeyEventSource { get; set; }
    }
}
