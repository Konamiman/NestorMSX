using System.Linq;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using NUnit.Framework;

namespace Konamiman.NestorMSX.Tests
{
    public class Ascii8RomTests
    {
        private const int bankSize = 8 * 1024;

        private byte[] contents;
        private Ascii8Rom Sut;

        [SetUp]
        public void Setup()
        {
            contents = Enumerable.Repeat((byte)0, bankSize)
                .Concat(Enumerable.Repeat((byte)1, bankSize))
                .Concat(Enumerable.Repeat((byte)2, bankSize))
                .ToArray();

            contents[1] = 9;
            contents[bankSize + 1] = 19;
            contents[2*bankSize + 1] = 29;

            Sut = new Ascii8Rom(contents);
        }

        [Test]
        public void Returns_FF_for_addresses_in_page_0_and_3()
        {
            Assert.AreEqual(0xFF, Sut[0]);
            Assert.AreEqual(0xFF, Sut[0x3FFF]);
            Assert.AreEqual(0xFF, Sut[0xC000]);
            Assert.AreEqual(0xFF, Sut[0xFFFF]);
        }

        [Test]
        public void Gets_data_from_first_blocks_by_default()
        {
            Assert.AreEqual(0, Sut[0x4000]);
            Assert.AreEqual(1, Sut[0x6000]);
            Assert.AreEqual(2, Sut[0x8000]);

            Assert.AreEqual(9, Sut[0x4001]);
            Assert.AreEqual(19, Sut[0x6001]);
            Assert.AreEqual(29, Sut[0x8001]);
        }

        [Test]
        public void Can_set_block_in_bank_1()
        {
            Sut[0x6000] = 1;
            Assert.AreEqual(1, Sut[0x4000]);
            Assert.AreEqual(19, Sut[0x4001]);

            Sut[0x6000] = 2;
            Assert.AreEqual(2, Sut[0x4000]);
            Assert.AreEqual(29, Sut[0x4001]);

            Sut[0x6000] = 0;
            Assert.AreEqual(0, Sut[0x4000]);
            Assert.AreEqual(9, Sut[0x4001]);
        }

        [Test]
        public void Can_set_block_in_bank_2()
        {
            Sut[0x6800] = 1;
            Assert.AreEqual(1, Sut[0x6000]);
            Assert.AreEqual(19, Sut[0x6001]);

            Sut[0x6800] = 2;
            Assert.AreEqual(2, Sut[0x6000]);
            Assert.AreEqual(29, Sut[0x6001]);

            Sut[0x6800] = 0;
            Assert.AreEqual(0, Sut[0x6000]);
            Assert.AreEqual(9, Sut[0x6001]);
        }

        [Test]
        public void Can_set_block_in_bank_3()
        {
            Sut[0x7000] = 1;
            Assert.AreEqual(1, Sut[0x8000]);
            Assert.AreEqual(19, Sut[0x8001]);

            Sut[0x7000] = 2;
            Assert.AreEqual(2, Sut[0x8000]);
            Assert.AreEqual(29, Sut[0x8001]);

            Sut[0x7000] = 0;
            Assert.AreEqual(0, Sut[0x8000]);
            Assert.AreEqual(9, Sut[0x8001]);
        }

        [Test]
        public void Can_set_block_in_bank_4()
        {
            Sut[0x7800] = 1;
            Assert.AreEqual(1, Sut[0xA000]);
            Assert.AreEqual(19, Sut[0xA001]);

            Sut[0x7800] = 2;
            Assert.AreEqual(2, Sut[0xA000]);
            Assert.AreEqual(29, Sut[0xA001]);

            Sut[0x7800] = 0;
            Assert.AreEqual(0, Sut[0xA000]);
            Assert.AreEqual(9, Sut[0xA001]);
        }

        [Test]
        public void Rounds_memory_size_to_8K()
        {
            var contents = Enumerable.Repeat((byte)0, 10000).ToArray();
            Sut = new Ascii8Rom(contents);

            Sut[0x6800] = 1;

            Assert.AreEqual(0, Sut[0x6000 + (9999 - bankSize)]);
            Assert.AreEqual(0xFF, Sut[0x6000 + (10000 - bankSize)]);
        }

        [Test]
        public void Ignores_non_existing_bank_numbers()
        {
            Sut[0x6000] = 1;
            Sut[0x6000] = 3;
            Assert.AreEqual(1, Sut[0x4000]);
            Assert.AreEqual(19, Sut[0x4001]);
        }
    }
}
