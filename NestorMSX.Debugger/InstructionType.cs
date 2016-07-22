using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger
{
    public enum InstructionType
    {
        Standard,
        UndocumentedNew,
        DocumentedMirror,
        UndocumentedMirror,
        Undefined
    }
}
