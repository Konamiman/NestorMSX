﻿using System;

namespace Konamiman.NestorMSX.Z80Debugger
{
    public interface IInstructionExtractor
    {
        Func<int, byte> GetByteAtAddress { get; }
        ushort NextInstructionAddress { get; set; }
        Z80Instruction ExtractInstruction();
    }
}
