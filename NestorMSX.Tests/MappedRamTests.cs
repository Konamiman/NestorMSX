using System;
using Konamiman.NestorMSX.BuiltInPlugins.MemoryTypes;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Konamiman.NestorMSX.Tests
{
    public class MappedRamTests
    {
        IFixture Fixture { get; set; }

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Test]
        public void Cannot_create_instances_of_wrong_size()
        {
            var wrongSizes = new[] {-1, 0, 1, 3, 200, 512};

            foreach(var size in wrongSizes)
            {
                Assert.Throws<ArgumentException>(() => new MappedRam(size));
            }
        }

        [Test]
        public void Can_create_instances_of_allowed_size()
        {
            var allowedSizes = new[] { 4, 8, 16, 32, 64, 256 };

            foreach (var size in allowedSizes)
            {
                new MappedRam(size);
            }
        }

        [Test]
        public void Same_segment_in_different_pages_mirror_memory()
        {
            var sut = new MappedRam(4);
            var value1 = Fixture.Create<byte>();
            var value2 = Fixture.Create<byte>();

            sut.WriteToSegmentSelectionRegister(0, 1);
            sut.WriteToSegmentSelectionRegister(1, 1);
            sut.WriteToSegmentSelectionRegister(2, 1);
            sut.WriteToSegmentSelectionRegister(3, 1);

            sut[0x0000] = value1;
            sut[0x3FFE] = value2;

            Assert.AreEqual(value1, sut[0x0000]);
            Assert.AreEqual(value2, sut[0x3FFE]);
            Assert.AreEqual(value1, sut[0x4000]);
            Assert.AreEqual(value2, sut[0x7FFE]);
            Assert.AreEqual(value1, sut[0x8000]);
            Assert.AreEqual(value2, sut[0xFFFE]);
        }

        [Test]
        public void Maximum_size_memory_has_256_segments()
        {
            var sut = new MappedRam(256);

            for(var segment = 0; segment < 256; segment++)
            {
                sut.WriteToSegmentSelectionRegister(0, (byte)segment);
                sut[0x0000] = (byte)segment;
                sut[0x3FFF] = (byte)~segment;
            }

            for(var segment = 0; segment < 256; segment++)
            {
                sut.WriteToSegmentSelectionRegister(0, (byte)segment);
                Assert.AreEqual(segment, sut[0x0000]);
                Assert.AreEqual((byte)~segment, sut[0x3FFF]);
            }
        }

        [Test]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        public void Memory_of_less_size_than_maximum_ignores_high_bits_of_segment_numbers(int sizeInSegments)
        {
            var sut = new MappedRam(sizeInSegments);

            for (var segment = 0; segment < 256; segment++)
            {
                sut.WriteToSegmentSelectionRegister(0, (byte)segment);
                sut[0x0000] = (byte)segment;
            }

            sut.WriteToSegmentSelectionRegister(0, 0);

            var expected = 256 - sizeInSegments;
            var actual = sut[0x0000];

            Assert.AreEqual(expected, actual);
        }
    }
}
