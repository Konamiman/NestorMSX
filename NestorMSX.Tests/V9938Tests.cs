using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using Konamiman.NestorMSX.Hardware;
using Konamiman.Z80dotNet;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using System.Linq;

namespace Konamiman.NestorMSX.Tests
{
    public class V9938Tests
    {
        Fixture Fixture { get; set; }
        V9938 Sut { get; set; }
        Mock<IV9938DisplayRenderer> DisplayRenderer { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            DisplayRenderer = new Mock<IV9938DisplayRenderer>();
            CreateSut();
            DisplayRenderer.ResetCalls();
        }

        private void CreateSut(int vramSizeInKb = 16)
        {
            Sut = new V9938(DisplayRenderer.Object, new Configuration {VdpFrequencyMultiplier = 1.2M}, vramSizeInKb);
        }

        [Test]
        public void Can_create_instances_and_initializes_properly()
        {
            CreateSut();
            Verify(m => m.BlankScreen());
            Assert.AreEqual(ScreenMode.Graphic1, Sut.CurrentScreenMode);
        }

        #region Setting screen mode

        [Test]
        public void Screen_mode_is_set_properly()
        {
            WriteControlRegister(0, 2);
            Verify(m => m.SetScreenMode(ScreenMode.Graphic2));

            WriteControlRegister(0, 0);
            DisplayRenderer.ResetCalls();
            WriteControlRegister(1, 0x10);
            Verify(m => m.SetScreenMode(ScreenMode.Text1));

            WriteControlRegister(0, 0);
            WriteControlRegister(0, 4);
            WriteControlRegister(1, 0);
            DisplayRenderer.ResetCalls();
            WriteControlRegister(1, 0x010);
            Verify(m => m.SetScreenMode(ScreenMode.Text2));

            WriteControlRegister(1, 2);
            WriteControlRegister(1, 0);
            DisplayRenderer.ResetCalls();
            WriteControlRegister(0, 0);
            Verify(m => m.SetScreenMode(ScreenMode.Graphic1));
        }

        #endregion

        #region Indirect register access

        [Test]
        public void Can_write_registers_with_autoincrement()
        {
            var registersWrittenTo = new List<byte>();
            var valuesWritten = new List<byte>();

            Sut.ControlRegisterWritten += (sender, eventArgs) =>
            {
                registersWrittenTo.Add(eventArgs.RegisterNumber);
                valuesWritten.Add(eventArgs.Value);
            };

            WriteControlRegister(17, 10);

            Sut.WriteToPort(3, 200);
            Sut.WriteToPort(3, 201);
            Sut.WriteToPort(3, 202);

            var expectedRegistersWritten = new byte[] {17, 10, 11, 12};
            var expectedValuesWritten = new byte[] {100, 200, 201, 202};

            CollectionAssert.AreEqual(expectedValuesWritten, expectedValuesWritten);
        }

        [Test]
        public void Can_write_registers_without_autoincrement()
        {
            var registersWrittenTo = new List<byte>();
            var valuesWritten = new List<byte>();

            Sut.ControlRegisterWritten += (sender, eventArgs) =>
            {
                registersWrittenTo.Add(eventArgs.RegisterNumber);
                valuesWritten.Add(eventArgs.Value);
            };

            WriteControlRegister(17, 10 | 0x80);

            Sut.WriteToPort(3, 200);
            Sut.WriteToPort(3, 201);
            Sut.WriteToPort(3, 202);

            var expectedRegistersWritten = new byte[] { 17, 10, 10, 10 };
            var expectedValuesWritten = new byte[] { 100, 200, 201, 202 };

            CollectionAssert.AreEqual(expectedValuesWritten, expectedValuesWritten);
        }

        [Test]
        public void Writing_registers_with_autoincrement_wraps_register_number()
        {
            var registersWrittenTo = new List<byte>();
            var valuesWritten = new List<byte>();

            Sut.ControlRegisterWritten += (sender, eventArgs) =>
            {
                registersWrittenTo.Add(eventArgs.RegisterNumber);
                valuesWritten.Add(eventArgs.Value);
            };

            WriteControlRegister(17, 62);

            Sut.WriteToPort(3, 200);
            Sut.WriteToPort(3, 201);
            Sut.WriteToPort(3, 202);
            Sut.WriteToPort(3, 203);

            var expectedRegistersWritten = new byte[] { 17, 62, 63, 0, 1 };
            var expectedValuesWritten = new byte[] { 100, 200, 201, 202, 203 };

            CollectionAssert.AreEqual(expectedValuesWritten, expectedValuesWritten);
        }

        #endregion

        #region VRAM access

        [Test]
        public void Can_read_vram_from_port_after_setup()
        {
            var address = RandomVramAddress();
            var value = Fixture.Create<byte>();
            Sut.WriteVram(address, value);

            SetupVramRead(address);
            var actual = Sut.ReadFromPort(0);

            Assert.AreEqual(value, actual);
        }
        
        [Test]
        public void Reads_vram_Sequentially()
        {
            var address = RandomVramAddress();
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();
            Sut.WriteVram(address, value1);
            Sut.WriteVram(address + 1, value2);

            SetupVramRead(address);
            var actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value2, actual);
        }

        [Test]
        [TestCase(16)]
        [TestCase(64)]
        [TestCase(128)]
        public void Reads_vram_overlapping_from_max_address_to_zero(int sizeInKb)
        {
            CreateSut(sizeInKb);
            var lastAddress = (sizeInKb*1024) - 1;

            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();
            Sut.WriteVram(lastAddress, value1);
            Sut.WriteVram(0, value2);

            SetupVramRead(lastAddress);
            var actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadFromPort(0);
            Assert.AreEqual(value2, actual);
        }

        [Test]
        public void Can_write_to_vram_to_port_after_setup()
        {
            var address = RandomVramAddress();
            var oldValue = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            Sut.WriteVram(address, oldValue);

            SetupVramWrite(address);
            Sut.WriteToPort(0, value);

            var actual = Sut.ReadVram(address);
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void Writes_vram_Sequentially()
        {
            var address = RandomVramAddress();
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            SetupVramWrite(address);
            Sut.WriteToPort(0, value1);
            Sut.WriteToPort(0, value2);

            var actual = Sut.ReadVram(address);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadVram(address+1);
            Assert.AreEqual(value2, actual);
        }

        [Test]
        [TestCase(16)]
        [TestCase(64)]
        [TestCase(128)]
        public void Writes_vram_overlapping_from_max_address_to_zero(int sizeInKb)
        {
            CreateSut(sizeInKb);
            var lastAddress = (sizeInKb * 1024) - 1;

            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            SetupVramWrite(lastAddress);
            Sut.WriteToPort(0, value1);
            Sut.WriteToPort(0, value2);

            var actual = Sut.ReadVram(lastAddress);
            Assert.AreEqual(value1, actual);
            actual = Sut.ReadVram(0);
            Assert.AreEqual(value2, actual);
        }

        [Test]
        public void WriteVram_masks_address_for_16K_Vran()
        {
            CreateSut(16);

            var value = Fixture.Create<byte>();
            Sut.WriteVram((64 * 1024)+1, value);
            var actual = Sut.GetVramContents(0, 16384)[1];
            Assert.AreEqual(value, actual);

            value = Fixture.Create<byte>();
            Sut.WriteVram((16 * 1024) + 1, value);
            actual = Sut.GetVramContents(0, 16384)[1];
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void WriteVram_masks_address_for_64K_Vran()
        {
            CreateSut(64);

            var value = Fixture.Create<byte>();
            Sut.WriteVram((64 * 1024) + 1, value);
            var actual = Sut.GetVramContents(0, 65536)[1];
            Assert.AreEqual(value, actual);

            value = Fixture.Create<byte>();
            Sut.WriteVram((16 * 1024) + 1, value);
            actual = Sut.GetVramContents(0, 65536)[(16 * 1024) + 1];
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void WriteVram_masks_address_for_128K_Vran()
        {
            CreateSut(128);

            var value = Fixture.Create<byte>();
            Sut.WriteVram((64 * 1024) + 1, value);
            var actual = Sut.GetVramContents(0, (128 * 1024))[(64 * 1024) + 1];
            Assert.AreEqual(value, actual);

            value = Fixture.Create<byte>();
            Sut.WriteVram((16 * 1024) + 1, value);
            actual = Sut.GetVramContents(0, 65536)[(16 * 1024) + 1];
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void ReadVram_masks_address_for_16K_Vram()
        {
            CreateSut(16);
            var contents = new byte[16384];
            var value = Fixture.Create<byte>();
            contents[1] = value;
            Sut.SetVramContents(0, contents);

            var actual = Sut.ReadVram(1);
            Assert.AreEqual(value, actual);

            actual = Sut.ReadVram((16 * 1024) + 1);
            Assert.AreEqual(value, actual);

            actual = Sut.ReadVram((64 * 1024) + 1);
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void ReadVram_masks_address_for_64K_Vram()
        {
            CreateSut(64);
            var contents = new byte[65536];
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();
            contents[1] = value1;
            contents[(16*1024) + 1] = value2;
            Sut.SetVramContents(0, contents);

            var actual = Sut.ReadVram(1);
            Assert.AreEqual(value1, actual);

            actual = Sut.ReadVram((16 * 1024) + 1);
            Assert.AreEqual(value2, actual);

            actual = Sut.ReadVram((64 * 1024) + 1);
            Assert.AreEqual(value1, actual);
        }

        [Test]
        public void ReadVram_masks_address_for_128K_Vram()
        {
            CreateSut(128);
            var contents = new byte[128 * 1024];
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();
            var value3 = Fixture.Create<byte>();
            contents[1] = value1;
            contents[(16 * 1024) + 1] = value2;
            contents[(64 * 1024) + 1] = value3;
            Sut.SetVramContents(0, contents);

            var actual = Sut.ReadVram(1);
            Assert.AreEqual(value1, actual);

            actual = Sut.ReadVram((16 * 1024) + 1);
            Assert.AreEqual(value2, actual);

            actual = Sut.ReadVram((64 * 1024) + 1);
            Assert.AreEqual(value3, actual);
        }

        [Test]
        public void Event_is_fired_when_vram_is_written_to()
        {
            var address = Fixture.Create<int>();
            var value = Fixture.Create<byte>();
            int actualAddress = 0;
            int actualValue = 0;

            Sut.VramWritten += (sender, args) => {
                actualAddress = args.Address;
                actualValue = args.Value;
            };

            Sut.WriteVram(address, value);

            Assert.AreEqual(actualAddress, address);
            Assert.AreEqual(actualValue, value);
        }

        #endregion

        #region Screen on/off

        [Test]
        public void Notifies_renderer_of_blank_or_active_screen()
        {
            var value = Fixture.Create<byte>().WithBit(6, 1);
            WriteControlRegister(1, value);
            Verify(m => m.ActivateScreen(), true);

            value = value.WithBit(6, 0);
            WriteControlRegister(1, value);
            Verify(m => m.BlankScreen(), true);
        }

        [Test]
        public void Notifies_renderer_of_color_change()
        {
            var value = Fixture.Create<byte>();
            WriteControlRegister(7, value);

            Verify(m => m.SetTextColor((byte)(value >> 4)));
            Verify(m => m.SetBackdropColor((byte)(value & 0x0F)));
        }

        #endregion

        #region Pattern name and generator tables

        [Test]
        public void Notifies_write_to_pattern_name_table()
        {
            WriteControlRegister(1, ((byte)0).WithBit(4, 1));

            var nameTableBaseAddressHighBits = Fixture.Create<byte>() & 0x0F;
            var nameTableBaseAddress = nameTableBaseAddressHighBits << 10;
            var offset = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();

            WriteControlRegister(2, (byte)nameTableBaseAddressHighBits);

            SetupVramWrite(nameTableBaseAddress + offset);
            Sut.WriteToPort(0, value);

            Verify(m => m.WriteToNameTable(offset, value));
        }

        [Test]
        [TestCase(0, 768)]
        [TestCase(1, 960)]
        public void Does_not_notify_write_outside_pattern_name_table(int screenMode, int nameTableSize)
        {
            WriteControlRegister(1, ((byte)0).WithBit(4, screenMode));

            var nameTableBaseAddressHighBits = Fixture.Create<byte>() & 0x0F;
            var nameTableBaseAddress = nameTableBaseAddressHighBits << 10;
            var offset = nameTableSize - 1;
            var value = Fixture.Create<byte>();

            WriteControlRegister(2, (byte)nameTableBaseAddressHighBits);

            SetupVramWrite(nameTableBaseAddress + offset);
            Sut.WriteToPort(0, value);
            Verify(m => m.WriteToNameTable(offset, value), true);
            Sut.WriteToPort(0, value);
            DisplayRenderer.Verify(m => m.WriteToNameTable(It.IsAny<int>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Notifies_write_to_pattern_generator_table()
        {
            var patternTableBaseAddressHighBits = Fixture.Create<byte>() & 0x07;
            var patternTableBaseAddress = patternTableBaseAddressHighBits << 11;
            var offset = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();

            WriteControlRegister(4, (byte)patternTableBaseAddressHighBits);

            SetupVramWrite(patternTableBaseAddress + offset);
            Sut.WriteToPort(0, value);

            Verify(m => m.WriteToPatternGeneratorTable(offset, value));
        }

        [Test]
        [TestCase(0, 2048)]
        [TestCase(1, 2048)]
        public void Does_not_notify_write_outside_pattern_generator_table(int screenMode, int patternTableSize)
        {
            if(screenMode == 1)
                WriteControlRegister(1, ((byte)0).WithBit(4, 1));

            var patternTableBaseAddressHighBits = Fixture.Create<byte>() & 0x07;
            var patternTableBaseAddress = patternTableBaseAddressHighBits << 11;
            var offset = patternTableSize - 1;
            var value = Fixture.Create<byte>();

            WriteControlRegister(4, (byte)patternTableBaseAddressHighBits);

            SetupVramWrite(patternTableBaseAddress + offset);
            Sut.WriteToPort(0, value);
            Verify(m => m.WriteToPatternGeneratorTable(offset, value), true);
            Sut.WriteToPort(0, value);
            DisplayRenderer.Verify(m => m.WriteToPatternGeneratorTable(It.IsAny<int>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Notifies_write_to_color_table()
        {
            var colorTableBaseAddressHighBits = Fixture.Create<byte>();
            var colorTableBaseAddress = colorTableBaseAddressHighBits << 6;
            var offset = Fixture.Create<byte>() & 0x1F;
            var value = Fixture.Create<byte>();

            WriteControlRegister(3, colorTableBaseAddressHighBits);

            SetupVramWrite(colorTableBaseAddress + offset);
            Sut.WriteToPort(0, value);

            Verify(m => m.WriteToColourTable(offset, value));
        }

        [Test]
        public void Does_not_notify_write_outside_color_table()
        {
            var colorTableBaseAddressHighBits = Fixture.Create<byte>();
            var colorTableBaseAddress = colorTableBaseAddressHighBits << 6;
            var offset = 32 - 1;
            var value = Fixture.Create<byte>();

            WriteControlRegister(3, colorTableBaseAddressHighBits);

            SetupVramWrite(colorTableBaseAddress + offset);
            Sut.WriteToPort(0, value);
            Verify(m => m.WriteToColourTable(offset, value), true);
            Sut.WriteToPort(0, value);
            DisplayRenderer.Verify(m => m.WriteToColourTable(It.IsAny<int>(), It.IsAny<byte>()), Times.Never);
        }

        [Test]
        public void Notifies_writing_name_table_on_bulk_vram_write()
        {
            WriteControlRegister(1, ((byte)0).WithBit(4, 1));

            var nameTableBaseAddressHighBits = Fixture.Create<byte>() & 0x0F;
            var nameTableBaseAddress = nameTableBaseAddressHighBits << 10;
            var offset = Fixture.Create<byte>();
            var values = Fixture.Create<byte[]>();

            WriteControlRegister(2, (byte)nameTableBaseAddressHighBits);

            Sut.SetVramContents(nameTableBaseAddress + offset, values);

            for(int i=0; i<values.Length; i++)
                Verify(m => m.WriteToNameTable(offset + i, values[i]));
        }

        [Test]
        public void Notifies_writing_pattern_generator_table_on_bulk_vram_write()
        {
            var patternTableBaseAddressHighBits = Fixture.Create<byte>() & 0x07;
            var patternTableBaseAddress = patternTableBaseAddressHighBits << 11;
            var offset = Fixture.Create<byte>();
            var values = Fixture.Create<byte[]>();

            WriteControlRegister(4, (byte)patternTableBaseAddressHighBits);

            Sut.SetVramContents(patternTableBaseAddress + offset, values);

            for(int i=0; i<values.Length; i++)
                Verify(m => m.WriteToPatternGeneratorTable(offset + i, values[i]));
        }

        [Test]
        public void Notifies_writing_color_table_on_bulk_vram_write()
        {
            var colorTableBaseAddressHighBits = Fixture.Create<byte>();
            var colorTableBaseAddress = colorTableBaseAddressHighBits << 6;
            var offset = Fixture.Create<byte>() & 0x1F;
            var values = Fixture.Create<byte[]>();

            WriteControlRegister(3, colorTableBaseAddressHighBits);

            Sut.SetVramContents(colorTableBaseAddress + offset, values);

            for(int i=0; i<values.Length; i++)
                Verify(m => m.WriteToColourTable(offset + i, values[i]));
        }

        #endregion

        #region Setting tabled

        [Test]
        [TestCase(16)]
        [TestCase(64)]
        [TestCase(128)]
        public void Sets_pattern_name_table_applying_proper_mask(int vramSizeInKb)
        {
            CreateSut(vramSizeInKb);
            var mask = (vramSizeInKb*1024) - 1;

            var address = 0xFFFF;

            WriteControlRegister(2, (byte)(address >> 10));

            var expected = (0xFC00) & mask;
            Assert.AreEqual(expected, Sut.PatternNameTableAddress);
        }

        [Test]
        [TestCase(16)]
        [TestCase(64)]
        [TestCase(128)]
        public void Sets_pattern_generator_table_applying_proper_mask(int vramSizeInKb)
        {
            CreateSut(vramSizeInKb);
            var mask = (vramSizeInKb * 1024) - 1;

            var address = 0xFFFF;

            WriteControlRegister(4, (byte)(address >> 11));

            var expected = (0xF800) & mask;
            Assert.AreEqual(expected, Sut.PatternGeneratorTableAddress);
        }

        [Test]
        [TestCase(16)]
        [TestCase(64)]
        [TestCase(128)]
        public void Sets_color_table_applying_proper_mask(int vramSizeInKb)
        {
            CreateSut(vramSizeInKb);
            var mask = (vramSizeInKb * 1024) - 1;

            var address = 0xBFFF;

            WriteControlRegister(3, (byte)((address >> 6) & 0xFF));
            WriteControlRegister(10, (byte)(address >> 14));

            var expected = (0xBFC0) & mask;
            Assert.AreEqual(expected, Sut.ColorTableAddress);
        }

        #endregion

        #region Expanded RAM

        [Test]
        public void Nothing_is_written_via_port_if_expansion_RAM_is_selected()
        {
            var value = Fixture.Create<byte>();
            Sut.WriteToPort(0, value);

            value = (byte)(~value);

            WriteControlRegister(45, 64);
            SetupVramWrite(0);
            Sut.WriteToPort(0, value);

            var vramContents = Sut.GetVramContents(0, 1).Single();
            Assert.AreNotEqual(value, vramContents);
        }

        [Test]
        public void Nothing_is_written_via_WriteRam_if_expansion_RAM_is_selected()
        {
            var value = Fixture.Create<byte>();
            Sut.WriteVram(0, value);

            value = (byte)(~value);

            WriteControlRegister(45, 64);
            Sut.WriteVram(0, value);

            var vramContents = Sut.GetVramContents(0, 1).Single();
            Assert.AreNotEqual(value, vramContents);
        }

        [Test]
        public void Nothing_is_read_via_port_if_expansion_RAM_is_selected()
        {
            var value = Fixture.Create<byte>();
            Sut.WriteToPort(0, value);

            WriteControlRegister(45, 64);
            SetupVramRead(0);
            var vramContents = Sut.ReadFromPort(0);

            Assert.AreEqual(0xFF, vramContents);
        }

        [Test]
        public void Nothing_is_read_via_ReadVram_if_expansion_RAM_is_selected()
        {
            var value = Fixture.Create<byte>();
            Sut.WriteToPort(0, value);

            WriteControlRegister(45, 64);
            var vramContents = Sut.ReadVram(0);

            Assert.AreEqual(0xFF, vramContents);
        }

        #endregion

        #region Interrupts and status register

        [Test]
        public void Reading_status_register_other_than_0_always_yields_0()
        {
            WriteControlRegister(15, 1);

            bool bit7Set = false;
            var sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 100)
            {
                if (Sut.ReadFromPort(1).GetBit(7) == 1)
                {
                    sw.Stop();
                    bit7Set = true;
                    break;
                }
                Thread.Sleep(1);
            }

            Assert.False(bit7Set);
        }

        [Test]
        [Ignore]
        //TODO: This test passes only when ran individually
        public void Sets_bit_7_of_status_Register_at_50Hz_and_clears_it_after_read()
        {
            WriteControlRegister(15, 0);

            bool bit7Set = false;
            Bit nextBitValue = 0;
            var sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 100) {
                if (Sut.ReadFromPort(1).GetBit(7) == 1) {
                    sw.Stop();
                    nextBitValue = Sut.ReadFromPort(1).GetBit(7);
                    bit7Set = true;
                    break;
                }
                Thread.Sleep(1);
            }

            Assert.True(bit7Set);
            Assert.LessOrEqual(sw.ElapsedMilliseconds, 30);
            Assert.AreEqual(0, nextBitValue);
        }

        [Test]
        public void Does_not_generate_interrupt_if_GINT_is_zero()
        {
            WriteControlRegister(1, 0);

            bool intLineActive = false;
            var sw = new Stopwatch();
            sw.Start();
            while(sw.ElapsedMilliseconds < 1000) {
                if(Sut.IntLineIsActive) {
                    intLineActive = true;
                    break;
                }
                Thread.Sleep(1);
            }
            sw.Stop();

            Assert.False(intLineActive);
        }

        [Test]
        [Ignore]
        //TODO: This test passes only when ran individually
        public void Generates_interrupt_if_GINT_is_one_and_clears_it_after_status_register_read()
        {
            WriteControlRegister(1, ((byte)0).WithBit(5,1));

            bool intLineActive = false;
            bool intLineActiveAfterStatusRegisterRead = false;
            var sw = new Stopwatch();
            sw.Start();
            while(sw.ElapsedMilliseconds < 1000) {
                if(Sut.IntLineIsActive) {
                    intLineActive = true;
                    Sut.ReadFromPort(1);
                    intLineActiveAfterStatusRegisterRead = Sut.IntLineIsActive;
                    break;
                }
                Thread.Sleep(1);
            }
            sw.Stop();

            Assert.True(intLineActive);
            Assert.False(intLineActiveAfterStatusRegisterRead);
        }

        #endregion

        #region Helper methods

        private void SetupVramRead(int address)
        {
            SetupVramAccess(address, 0);
        }

        private void SetupVramWrite(int address)
        {
            SetupVramAccess(address, 1);
        }

        private void SetupVramAccess(int address, Bit rwFlag)
        {
            WriteControlRegister(14, (byte)(address >> 14));
            Sut.WriteToPort(1, (byte)(address & 0xFF));
            Sut.WriteToPort(1, (byte)(((address >> 8) & 0x3F) | (rwFlag << 6)));
        }

        int RandomVramAddress()
        {
            return Fixture.Create<int>() & 0x3FFF;
        }

        void WriteControlRegister(int register, byte value)
        {
            Sut.WriteToPort(1, value);
            Sut.WriteToPort(1, (byte)(register | 0x80));
        }

        void Verify(Expression<Action<IV9938DisplayRenderer>> expression, bool resetCalls = false)
        {
            DisplayRenderer.Verify(expression, Times.Once);
            if(resetCalls)
                DisplayRenderer.ResetCalls();
        }

        #endregion
    }
}
