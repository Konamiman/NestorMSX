namespace Konamiman.NestorMSX.Z80Debugger
{
    public partial class InstructionExtractor
    {
        private readonly Z80Instruction[] _singleByteInstructionPrototypes =
        {
            new Z80Instruction
            {
                FormatString = "nop"
            },
            new Z80Instruction
            {
                FormatString = "ld bc,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ld (bc),a"
            },
            new Z80Instruction
            {
                FormatString = "inc bc"
            },
            new Z80Instruction
            {
                FormatString = "inc b"
            },
            new Z80Instruction
            {
                FormatString = "dec b"
            },
            new Z80Instruction
            {
                FormatString = "ld b,{0}"
            },
            new Z80Instruction
            {
                FormatString = "rlca"
            },
            new Z80Instruction
            {
                FormatString = "ex af,af'"
            },
            new Z80Instruction
            {
                FormatString = "add hl,bc"
            },
            new Z80Instruction
            {
                FormatString = "ld a,(bc)"
            },
            new Z80Instruction
            {
                FormatString = "dec bc"
            },
            new Z80Instruction
            {
                FormatString = "inc c"
            },
            new Z80Instruction
            {
                FormatString = "dec c"
            },
            new Z80Instruction
            {
                FormatString = "ld c,{0}"
            },
            new Z80Instruction
            {
                FormatString = "rrca"
            },
            new Z80Instruction
            {
                FormatString = "djnz {0}"
            },
            new Z80Instruction
            {
                FormatString = "ld de,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ld (de),a"
            },
            new Z80Instruction
            {
                FormatString = "inc de"
            },
            new Z80Instruction
            {
                FormatString = "inc d"
            },
            new Z80Instruction
            {
                FormatString = "dec d"
            },
            new Z80Instruction
            {
                FormatString = "ld d,{0}"
            },
            new Z80Instruction
            {
                FormatString = "rla"
            },
            new Z80Instruction
            {
                FormatString = "jr {0}"
            },
            new Z80Instruction
            {
                FormatString = "add hl,de"
            },
            new Z80Instruction
            {
                FormatString = "ld a,(de)"
            },
            new Z80Instruction
            {
                FormatString = "dec de"
            },
            new Z80Instruction
            {
                FormatString = "inc e"
            },
            new Z80Instruction
            {
                FormatString = "dec e"
            },
            new Z80Instruction
            {
                FormatString = "ld e,{0}"
            },
            new Z80Instruction
            {
                FormatString = "rra"
            },
            new Z80Instruction
            {
                FormatString = "jr nz,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ld hl,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ld ({0}),hl"
            },
            new Z80Instruction
            {
                FormatString = "inc hl"
            },
            new Z80Instruction
            {
                FormatString = "inc h"
            },
            new Z80Instruction
            {
                FormatString = "dec h"
            },
            new Z80Instruction
            {
                FormatString = "ld h,{0}"
            },
            new Z80Instruction
            {
                FormatString = "daa"
            },
            new Z80Instruction
            {
                FormatString = "jr z,{0}"
            },
            new Z80Instruction
            {
                FormatString = "sla hl"
            },
            new Z80Instruction
            {
                FormatString = "ld hl,({0})"
            },
            new Z80Instruction
            {
                FormatString = "dec hl"
            },
            new Z80Instruction
            {
                FormatString = "inc l"
            },
            new Z80Instruction
            {
                FormatString = "dec l"
            },
            new Z80Instruction
            {
                FormatString = "ld l,{0}"
            },
            new Z80Instruction
            {
                FormatString = "cpl"
            },
            new Z80Instruction
            {
                FormatString = "jr nc,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ld sp,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ld ({0}),a"
            },
            new Z80Instruction
            {
                FormatString = "inc sp"
            },
            new Z80Instruction
            {
                FormatString = "inc (hl)"
            },
            new Z80Instruction
            {
                FormatString = "dec (hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),{0}"
            },
            new Z80Instruction
            {
                FormatString = "scf"
            },
            new Z80Instruction
            {
                FormatString = "jr c,{0}"
            },
            new Z80Instruction
            {
                FormatString = "add hl,sp"
            },
            new Z80Instruction
            {
                FormatString = "ld a,({0})"
            },
            new Z80Instruction
            {
                FormatString = "dec sp"
            },
            new Z80Instruction
            {
                FormatString = "inc a"
            },
            new Z80Instruction
            {
                FormatString = "dec a"
            },
            new Z80Instruction
            {
                FormatString = "ld a,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ccf"
            },
            new Z80Instruction
            {
                FormatString = "ld b,b"
            },
            new Z80Instruction
            {
                FormatString = "ld b,c"
            },
            new Z80Instruction
            {
                FormatString = "ld b,d"
            },
            new Z80Instruction
            {
                FormatString = "ld b,e"
            },
            new Z80Instruction
            {
                FormatString = "ld b,h"
            },
            new Z80Instruction
            {
                FormatString = "ld b,l"
            },
            new Z80Instruction
            {
                FormatString = "ld b,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld b,a"
            },
            new Z80Instruction
            {
                FormatString = "ld c,b"
            },
            new Z80Instruction
            {
                FormatString = "ld c,c"
            },
            new Z80Instruction
            {
                FormatString = "ld c,d"
            },
            new Z80Instruction
            {
                FormatString = "ld c,e"
            },
            new Z80Instruction
            {
                FormatString = "ld c,h"
            },
            new Z80Instruction
            {
                FormatString = "ld c,l"
            },
            new Z80Instruction
            {
                FormatString = "ld c,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld c,a"
            },
            new Z80Instruction
            {
                FormatString = "ld d,b"
            },
            new Z80Instruction
            {
                FormatString = "ld d,c"
            },
            new Z80Instruction
            {
                FormatString = "ld d,d"
            },
            new Z80Instruction
            {
                FormatString = "ld d,e"
            },
            new Z80Instruction
            {
                FormatString = "ld d,h"
            },
            new Z80Instruction
            {
                FormatString = "ld d,l"
            },
            new Z80Instruction
            {
                FormatString = "ld d,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld d,a"
            },
            new Z80Instruction
            {
                FormatString = "ld e,b"
            },
            new Z80Instruction
            {
                FormatString = "ld e,c"
            },
            new Z80Instruction
            {
                FormatString = "ld e,d"
            },
            new Z80Instruction
            {
                FormatString = "ld e,e"
            },
            new Z80Instruction
            {
                FormatString = "ld e,h"
            },
            new Z80Instruction
            {
                FormatString = "ld e,l"
            },
            new Z80Instruction
            {
                FormatString = "ld e,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld e,a"
            },
            new Z80Instruction
            {
                FormatString = "ld h,b"
            },
            new Z80Instruction
            {
                FormatString = "ld h,c"
            },
            new Z80Instruction
            {
                FormatString = "ld h,d"
            },
            new Z80Instruction
            {
                FormatString = "ld h,e"
            },
            new Z80Instruction
            {
                FormatString = "ld h,h"
            },
            new Z80Instruction
            {
                FormatString = "ld h,l"
            },
            new Z80Instruction
            {
                FormatString = "ld h,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld h,a"
            },
            new Z80Instruction
            {
                FormatString = "ld l,b"
            },
            new Z80Instruction
            {
                FormatString = "ld l,c"
            },
            new Z80Instruction
            {
                FormatString = "ld l,d"
            },
            new Z80Instruction
            {
                FormatString = "ld l,e"
            },
            new Z80Instruction
            {
                FormatString = "ld l,h"
            },
            new Z80Instruction
            {
                FormatString = "ld l,l"
            },
            new Z80Instruction
            {
                FormatString = "ld l,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld l,a"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),b"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),c"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),d"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),e"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),h"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),l"
            },
            new Z80Instruction
            {
                FormatString = "halt"
            },
            new Z80Instruction
            {
                FormatString = "ld (hl),a"
            },
            new Z80Instruction
            {
                FormatString = "ld a,b"
            },
            new Z80Instruction
            {
                FormatString = "ld a,c"
            },
            new Z80Instruction
            {
                FormatString = "ld a,d"
            },
            new Z80Instruction
            {
                FormatString = "ld a,e"
            },
            new Z80Instruction
            {
                FormatString = "ld a,h"
            },
            new Z80Instruction
            {
                FormatString = "ld a,l"
            },
            new Z80Instruction
            {
                FormatString = "ld a,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "ld a,a"
            },
            new Z80Instruction
            {
                FormatString = "add a,b"
            },
            new Z80Instruction
            {
                FormatString = "add a,c"
            },
            new Z80Instruction
            {
                FormatString = "add a,d"
            },
            new Z80Instruction
            {
                FormatString = "add a,e"
            },
            new Z80Instruction
            {
                FormatString = "add a,h"
            },
            new Z80Instruction
            {
                FormatString = "add a,l"
            },
            new Z80Instruction
            {
                FormatString = "add a,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "add a,a"
            },
            new Z80Instruction
            {
                FormatString = "adc a,b"
            },
            new Z80Instruction
            {
                FormatString = "adc a,c"
            },
            new Z80Instruction
            {
                FormatString = "adc a,d"
            },
            new Z80Instruction
            {
                FormatString = "adc a,e"
            },
            new Z80Instruction
            {
                FormatString = "adc a,h"
            },
            new Z80Instruction
            {
                FormatString = "adc a,l"
            },
            new Z80Instruction
            {
                FormatString = "adc a,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "adc a,a"
            },
            new Z80Instruction
            {
                FormatString = "sub b"
            },
            new Z80Instruction
            {
                FormatString = "sub c"
            },
            new Z80Instruction
            {
                FormatString = "sub d"
            },
            new Z80Instruction
            {
                FormatString = "sub e"
            },
            new Z80Instruction
            {
                FormatString = "sub h"
            },
            new Z80Instruction
            {
                FormatString = "sub l"
            },
            new Z80Instruction
            {
                FormatString = "sub (hl)"
            },
            new Z80Instruction
            {
                FormatString = "sub a"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,b"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,c"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,d"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,e"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,h"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,l"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,(hl)"
            },
            new Z80Instruction
            {
                FormatString = "sbc a,a"
            },
            new Z80Instruction
            {
                FormatString = "and b"
            },
            new Z80Instruction
            {
                FormatString = "and c"
            },
            new Z80Instruction
            {
                FormatString = "and d"
            },
            new Z80Instruction
            {
                FormatString = "and e"
            },
            new Z80Instruction
            {
                FormatString = "and h"
            },
            new Z80Instruction
            {
                FormatString = "and l"
            },
            new Z80Instruction
            {
                FormatString = "and (hl)"
            },
            new Z80Instruction
            {
                FormatString = "and a"
            },
            new Z80Instruction
            {
                FormatString = "xor b"
            },
            new Z80Instruction
            {
                FormatString = "xor c"
            },
            new Z80Instruction
            {
                FormatString = "xor d"
            },
            new Z80Instruction
            {
                FormatString = "xor e"
            },
            new Z80Instruction
            {
                FormatString = "xor h"
            },
            new Z80Instruction
            {
                FormatString = "xor l"
            },
            new Z80Instruction
            {
                FormatString = "xor (hl)"
            },
            new Z80Instruction
            {
                FormatString = "xor a"
            },
            new Z80Instruction
            {
                FormatString = "or b"
            },
            new Z80Instruction
            {
                FormatString = "or c"
            },
            new Z80Instruction
            {
                FormatString = "or d"
            },
            new Z80Instruction
            {
                FormatString = "or e"
            },
            new Z80Instruction
            {
                FormatString = "or h"
            },
            new Z80Instruction
            {
                FormatString = "or l"
            },
            new Z80Instruction
            {
                FormatString = "or (hl)"
            },
            new Z80Instruction
            {
                FormatString = "or a"
            },
            new Z80Instruction
            {
                FormatString = "cp b"
            },
            new Z80Instruction
            {
                FormatString = "cp c"
            },
            new Z80Instruction
            {
                FormatString = "cp d"
            },
            new Z80Instruction
            {
                FormatString = "cp e"
            },
            new Z80Instruction
            {
                FormatString = "cp h"
            },
            new Z80Instruction
            {
                FormatString = "cp l"
            },
            new Z80Instruction
            {
                FormatString = "cp (hl)"
            },
            new Z80Instruction
            {
                FormatString = "cp a"
            },
            new Z80Instruction
            {
                FormatString = "ret nz"
            },
            new Z80Instruction
            {
                FormatString = "pop bc"
            },
            new Z80Instruction
            {
                FormatString = "jp nz,{0}"
            },
            new Z80Instruction
            {
                FormatString = "jp {0}"
            },
            new Z80Instruction
            {
                FormatString = "call nz,{0}"
            },
            new Z80Instruction
            {
                FormatString = "push bc"
            },
            new Z80Instruction
            {
                FormatString = "add a,{0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 0"
            },
            new Z80Instruction
            {
                FormatString = "ret z"
            },
            new Z80Instruction
            {
                FormatString = "ret"
            },
            new Z80Instruction
            {
                FormatString = "jp z,{0}"
            },
			null, //0xCB
            new Z80Instruction
            {
                FormatString = "call z,{0}"
            },
            new Z80Instruction
            {
                FormatString = "call {0}"
            },
            new Z80Instruction
            {
                FormatString = "adc a,{0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 8"
            },
            new Z80Instruction
            {
                FormatString = "ret nc"
            },
            new Z80Instruction
            {
                FormatString = "pop de"
            },
            new Z80Instruction
            {
                FormatString = "jp nc,{0}"
            },
            new Z80Instruction
            {
                FormatString = "out ({0}),a"
            },
            new Z80Instruction
            {
                FormatString = "call nc,{0}"
            },
            new Z80Instruction
            {
                FormatString = "push de"
            },
            new Z80Instruction
            {
                FormatString = "sub {0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 16"
            },
            new Z80Instruction
            {
                FormatString = "ret c"
            },
            new Z80Instruction
            {
                FormatString = "exx"
            },
            new Z80Instruction
            {
                FormatString = "jp c,{0}"
            },
            new Z80Instruction
            {
                FormatString = "in a,({0})"
            },
            new Z80Instruction
            {
                FormatString = "call c,{0}"
            },
			null, //0xDD
            new Z80Instruction
            {
                FormatString = "sbc a,{0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 24"
            },
            new Z80Instruction
            {
                FormatString = "ret po"
            },
            new Z80Instruction
            {
                FormatString = "pop hl"
            },
            new Z80Instruction
            {
                FormatString = "jp po,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ex (sp),hl"
            },
            new Z80Instruction
            {
                FormatString = "call po,{0}"
            },
            new Z80Instruction
            {
                FormatString = "push hl"
            },
            new Z80Instruction
            {
                FormatString = "and {0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 32"
            },
            new Z80Instruction
            {
                FormatString = "ret pe"
            },
            new Z80Instruction
            {
                FormatString = "jp (hl)"
            },
            new Z80Instruction
            {
                FormatString = "jp pe,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ex de,hl"
            },
            new Z80Instruction
            {
                FormatString = "call pe,{0}"
            },
			null, //0xED
            new Z80Instruction
            {
                FormatString = "xor {0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 40"
            },
            new Z80Instruction
            {
                FormatString = "ret p"
            },
            new Z80Instruction
            {
                FormatString = "pop af"
            },
            new Z80Instruction
            {
                FormatString = "jp p,{0}"
            },
            new Z80Instruction
            {
                FormatString = "di"
            },
            new Z80Instruction
            {
                FormatString = "call p,{0}"
            },
            new Z80Instruction
            {
                FormatString = "push af"
            },
            new Z80Instruction
            {
                FormatString = "or {0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 48"
            },
            new Z80Instruction
            {
                FormatString = "ret m"
            },
            new Z80Instruction
            {
                FormatString = "ld sp,hl"
            },
            new Z80Instruction
            {
                FormatString = "jp m,{0}"
            },
            new Z80Instruction
            {
                FormatString = "ei"
            },
            new Z80Instruction
            {
                FormatString = "call m,{0}"
            },
			null, //0xFD
            new Z80Instruction
            {
                FormatString = "cp {0}"
            },
            new Z80Instruction
            {
                FormatString = "rst 56"
            },
		};
    }
}