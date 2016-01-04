using System;
using System.Collections.Generic;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.BuiltInPlugins.Hardware
{
    [NestorMSXPlugin("Clock IC")]
    public class ClockIcPlugin
    {
        private static ClockIcPlugin Instance;

        private ClockIcPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            context.Cpu.MemoryAccess += Cpu_MemoryAccess;
        }

        public static ClockIcPlugin GetInstance(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            if(Instance == null)
                Instance = new ClockIcPlugin(context, pluginConfig);

            return Instance;
        }

        private void Cpu_MemoryAccess(object sender, Z80dotNet.MemoryAccessEventArgs e)
        {
            if(e.EventType == MemoryAccessEventType.BeforePortRead && e.Address == 0xB5)
            {
                e.Value = ReadData();
                e.CancelMemoryAccess = true;
            }
            else if(e.EventType == MemoryAccessEventType.BeforePortWrite)
            {
                if(e.Address == 0xB5)
                    WriteData(e.Value);
                else if(e.Address == 0xB4)
                    SelectRegister(e.Value);
            }
        }

        private int registerNumber;
        private int currentBlock;
        private byte currentModeRegisterValue;

        private void SelectRegister(byte registerNumber)
        {
            this.registerNumber = registerNumber & 0x0F;
        }

        private void WriteData(byte value)
        {
            if(registerNumber == 13)
            {
                currentBlock = value & 0x03;
                currentModeRegisterValue = (byte)(value & 0x0F);
            }
        }

        private byte ReadData()
        {
            if(registerNumber == 13)
                return currentModeRegisterValue;
            else if(currentBlock == 0)
                return (byte)(ReadDataComponent(registerNumber) & 0x0F);
            else
                return 0;
        }

        private int ReadDataComponent(int registerNumber)
        {
            var now = DateTime.Now;

            switch(registerNumber)
            {
                case 0:
                    return now.Second % 10;
                case 1:
                    return now.Second / 10;
                case 2:
                    return now.Minute % 10;
                case 3:
                    return now.Minute / 10;
                case 4:
                    return now.Hour % 10;
                case 5:
                    return now.Hour / 10;
                case 6:
                    return (int)now.DayOfWeek;
                case 7:
                    return now.Day % 10;
                case 8:
                    return now.Day / 10;
                case 9:
                    return now.Month % 10;
                case 10:
                    return now.Month / 10;
                case 11:
                    return (now.Year - 1980) % 10;
                case 12:
                    return (now.Year - 1980) / 10;
                default:
                    return 0;
            }
        }
    }
}
