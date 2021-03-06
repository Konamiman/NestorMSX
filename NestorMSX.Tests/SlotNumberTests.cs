﻿using System;
using Konamiman.NestorMSX.Misc;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Konamiman.NestorMSX.Tests
{
    public class SlotNumberTests
    {
        Fixture Fixture { get; set; }

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture();
        }

        [Test]
        public void Can_create_instance_from_encoded_slot_number_for_non_expanded_slot()
        {
            var slotNumber = RandomSlotNumber();
            var sut = new SlotNumber(slotNumber);
            Assert.IsNotNull(sut);
        }

        [Test]
        public void Instance_for_non_expanded_slot_from_encoded_slot_number_has_proper_properties()
        {
            var slotNumber = RandomSlotNumber();
            var sut = new SlotNumber(slotNumber);
            Assert.AreEqual(slotNumber, sut.PrimarySlotNumber);
            Assert.AreEqual(0, sut.SubSlotNumber);
            Assert.False(sut.IsExpandedSlot);
            Assert.AreEqual(slotNumber, sut.EncodedByte);
        }

        [Test]
        public void Can_create_instance_from_encoded_slot_number_for_expanded_slot()
        {
            var slotNumber = (byte)(RandomSlotNumber() & 0x80);
            var sut = new SlotNumber(slotNumber);
            Assert.IsNotNull(sut);
        }

        [Test]
        public void Instance_for_expanded_slot_from_encoded_slot_number_has_proper_properties()
        {
            var primarySlotNumber = RandomSlotNumber();
            var subSlotNumber = RandomSlotNumber();
            var slotNumber =  EncodedByte(primarySlotNumber, subSlotNumber);;

            var sut = new SlotNumber(slotNumber);
            Assert.AreEqual(primarySlotNumber, sut.PrimarySlotNumber);
            Assert.AreEqual(subSlotNumber, sut.SubSlotNumber);
            Assert.True(sut.IsExpandedSlot);
            Assert.AreEqual(slotNumber, sut.EncodedByte);
        }

        [Test]
        public void Can_create_instance_from_valid_primary_and_expanded_slot_numbers()
        {
            var sut = new SlotNumber(RandomSlotNumber(), RandomSlotNumber());
            Assert.IsNotNull(sut);
        }

        [Test]
        public void Cannot_create_instance_from_invalid_primary_and_expanded_slot_numbers()
        {
            Assert.Throws<InvalidOperationException>(() => new SlotNumber(-1, RandomSlotNumber()));
            Assert.Throws<InvalidOperationException>(() => new SlotNumber(4, RandomSlotNumber()));
            Assert.Throws<InvalidOperationException>(() => new SlotNumber(RandomSlotNumber(), -1));
            Assert.Throws<InvalidOperationException>(() => new SlotNumber(RandomSlotNumber(), 4));
        }

        [Test]
        public void Instance_for_expanded_slot_from_primary_and_expanded_slot_number_has_proper_properties()
        {
            var primarySlotNumber = RandomSlotNumber();
            var subSlotNumber = RandomSlotNumber();

            var sut = new SlotNumber(primarySlotNumber, subSlotNumber);
            Assert.AreEqual(primarySlotNumber, sut.PrimarySlotNumber);
            Assert.AreEqual(subSlotNumber, sut.SubSlotNumber);
            Assert.True(sut.IsExpandedSlot);
            var expected = EncodedByte(primarySlotNumber, subSlotNumber);
            Assert.AreEqual(expected, sut.EncodedByte);
        }

        private static byte EncodedByte(byte primarySlotNumber, byte subSlotNumber)
        {
            return (byte)(0x80 | (subSlotNumber << 2) | primarySlotNumber);
        }

        [Test]
        public void Two_expanded_instances_are_equal_if_slot_and_subslot_are_equal()
        {
            var slotNumber = (byte)(Fixture.Create<byte>() | 0x80);
            var sut1 = new SlotNumber(slotNumber);
            var sut2 = new SlotNumber(slotNumber);
            Assert.True(sut1 == sut2);
            Assert.True(sut1.Equals(sut2));
        }

        [Test]
        public void Primary_and_expanded_instances_are_equal_if_primary_slot_is_equal_and_subslot_is_zero()
        {
            var slotNumber = (byte)(Fixture.Create<byte>() & 0x03);
            var sut1 = new SlotNumber(slotNumber);
            var sut2 = new SlotNumber((byte)(slotNumber | 0x80));
            Assert.True(sut1 == sut2);
            Assert.True(sut1.Equals(sut2));
        }

        [Test]
        public void Can_be_implicitly_cast_to_byte_and_unused_bits_are_zeroed()
        {
            var primarySlotNumber = RandomSlotNumber();
            var subSlotNumber = RandomSlotNumber();
            var slotNumber =  EncodedByte(primarySlotNumber, subSlotNumber);;

            var sut = (byte)new SlotNumber((byte)(slotNumber | 0x70));
            Assert.AreEqual(slotNumber, sut);
        }

        [Test]
        public void Can_be_implicitly_cast_from_byte()
        {
            var primarySlotNumber = RandomSlotNumber();
            var subSlotNumber = RandomSlotNumber();
            var slotNumber = EncodedByte(primarySlotNumber, subSlotNumber);

            var sut = (SlotNumber)(slotNumber);
            Assert.AreEqual(primarySlotNumber, sut.PrimarySlotNumber);
            Assert.AreEqual(subSlotNumber, sut.SubSlotNumber);
        }

        [Test]
        public void Can_be_compared_to_byte_and_unused_bits_are_ignored()
        {
            var slotNumber = (byte)(Fixture.Create<byte>() | 0x80);
            var slotNumberWithExtraBits = (byte)(slotNumber | 0x70);
            var sut = new SlotNumber(slotNumber);
            Assert.True(slotNumberWithExtraBits == sut);
            Assert.True(sut.Equals(slotNumberWithExtraBits));
        }

        private byte RandomSlotNumber()
        {
            return (byte)(Fixture.Create<byte>() & 3);
        }

        [Test]
        public void Can_parse_decimal_numbers()
        {
            var unparsedSlotNumber = (128 + 4*3 + 2).ToString();
            var parsedSlotNumber = SlotNumber.Parse(unparsedSlotNumber);
            var expected = new SlotNumber(2, 3);

            Assert.AreEqual(expected, parsedSlotNumber);
        }

        [Test]
        public void Can_parse_hexadecimal_numbers()
        {
            var unparsedSlotNumber = "#8E";
            var parsedSlotNumber = SlotNumber.Parse(unparsedSlotNumber);
            var expected = new SlotNumber(2, 3);

            Assert.AreEqual(expected, parsedSlotNumber);
        }

        [Test]
        public void Can_parse_slot_slash_subslot_numbers()
        {
            var unparsedSlotNumber = "2-3";
            var parsedSlotNumber = SlotNumber.Parse(unparsedSlotNumber);
            var expected = new SlotNumber(2, 3);

            Assert.AreEqual(expected, parsedSlotNumber);
        }

        [Test]
        public void Can_deal_with_whitespace()
        {
            var strings = new[] {" 142 ", " #8E ", " 2-3 "};
            var expected = new SlotNumber(2, 3);

            foreach(var s in strings)
                Assert.AreEqual(expected, SlotNumber.Parse(s));
        }

        [Test]
        public void Cannot_parse_invalid_strings()
        {
            Assert.Throws<ArgumentNullException>(() => SlotNumber.Parse(null));

            var invalidStrings = new[] {"", "999", "abc", "#FFF", "#xx", "3-4", "-2", "1-", "a-b"};

            foreach(var s in invalidStrings)
                Assert.Throws<FormatException>(() => SlotNumber.Parse(s));
        }
    }
}
