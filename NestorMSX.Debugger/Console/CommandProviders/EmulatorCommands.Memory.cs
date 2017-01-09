using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public partial class EmulatorCommandsProvider
    {
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

        private static readonly StringBuilder sbBytes = new StringBuilder();
        private static readonly StringBuilder sbChars = new StringBuilder();
        private static readonly StringBuilder sbResult = new StringBuilder();
        private static readonly byte[] dumpChar = {0};

        public string Dump(int startAddress, int length = 128, int width = 8)
        {
            if(startAddress < 0 || startAddress > 0xFFFF)
                return "*** Start address must be in the range 0-65535";
            var maxLength = 65536 - startAddress;
            if(length < 1 || length > maxLength) {
                return $"*** Length must be in the range 1-{maxLength}";
            }
            if(width < 1 || width > 255) {
                return "*** Width must be in the range 1-255";
            }

            sbBytes.Clear();
            sbChars.Clear();
            sbResult.Clear();
            var address = startAddress;
            var endAddress = startAddress + length;
            var inLinecount = 0;
            while(address < endAddress) {
                var data = pluginContext.SlotsSystem[address];
                sbBytes.AppendFormat("{0:X2} ", data);
                dumpChar[0] = data;
                sbChars.Append(data < 32 || data > 126 ? "." : Encoding.ASCII.GetString(dumpChar));

                inLinecount++;
                if(inLinecount == width) {
                    sbResult.AppendFormat("{0} {1}\r\n", sbBytes, sbChars);
                    sbBytes.Clear();
                    sbChars.Clear();
                    inLinecount = 0;
                }

                address++;
            }

            if(inLinecount > 0) {
                sbResult.AppendFormat("{0} {1}", sbBytes, sbChars);
            }

            return sbResult.ToString();
        }
    }
}
