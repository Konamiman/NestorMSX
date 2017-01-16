using System.Text;
using Konamiman.NestorMSX.Misc;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public partial class EmulatorCommandsProvider
    {
        public void Poke(int address, byte value, [RawExpression]string slot = null)
        {
            CheckRange(address);
            var memory = GetMemoryForSlot(slot);
            memory[address] = value;
        }
        public byte Peek(int address, [RawExpression]string slot = null)
        {
            CheckRange(address);
            var memory = GetMemoryForSlot(slot);
            return memory[address];
        }

        public short Wpeek(int address, [RawExpression]string slot = null)
        {
            CheckRange(address);
            var memory = GetMemoryForSlot(slot);
            return NumberUtils.CreateShort(memory[address], memory[address+1]);
        }

        public void Vpoke(int address, byte value)
        {
            CheckRange(address);
            pluginContext.Vdp.WriteVram(address, value);
        }

        private void CheckRange(int value, string name="Address", int min = 0, int max = 65535)
        {
            if(value < min || value > max)
                throw new CommandExecutionException($"{name} must be in the range {min}-{max}");
        }

        public byte Vpeek(int address)
        {
            CheckRange(address);
            return pluginContext.Vdp.ReadVram(address);
        }

        public string Slots()
        {
            return $"{slotsSystem.GetCurrentSlot(0)} {slotsSystem.GetCurrentSlot(1)} {slotsSystem.GetCurrentSlot(2)} {slotsSystem.GetCurrentSlot(3)}";
        }

        public string Segms()
        {
            return $"{mappedRam.GetBlockInBank(0)} {mappedRam.GetBlockInBank(1)} {mappedRam.GetBlockInBank(2)} {mappedRam.GetBlockInBank(3)}";
        }

        private static readonly StringBuilder sbBytes = new StringBuilder();
        private static readonly StringBuilder sbChars = new StringBuilder();
        private static readonly StringBuilder sbResult = new StringBuilder();
        private static readonly byte[] dumpChar = {0};

        public string Dump(int startAddress, int length = 128, int width = 8, [RawExpression]string slot = null)
        {
            CheckRange(startAddress, "Start addtess");
            var maxLength = 65536 - startAddress;
            CheckRange(length, "Length", 1, maxLength);
            CheckRange(width, "Width", 1, 255);

            var memory = GetMemoryForSlot(slot);

            sbBytes.Clear();
            sbChars.Clear();
            sbResult.Clear();
            var address = startAddress;
            var endAddress = startAddress + length;
            var inLinecount = 0;
            while(address < endAddress) {
                var data = memory[address];
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

        private IMemory GetMemoryForSlot(string slotNumber)
        {
            return string.IsNullOrEmpty(slotNumber) ? slotsSystem : slotsSystem.GetSlotContents(ParseSlotNumber(slotNumber));
        }

        private SlotNumber ParseSlotNumber(string value)
        {
            SlotNumber result;

            var ok = SlotNumber.TryParse(value, out result);
            if(ok)
                return result;

            try {
                var number = (decimal)EvaluateExpression(value);
                result = new SlotNumber((byte) number);
            } catch { 
                throw new CommandExecutionException($"{value} is not a valid representation of a slot number");
            }

            return result;
        }
    }
}
