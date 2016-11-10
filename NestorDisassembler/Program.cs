using System;
using System.IO;
using System.Linq;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Z80Debugger;
using Konamiman.NestorMSX.Z80Debugger.InstructionExtraction;

namespace Konamiman.NestorDisassembler
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileContents = File.ReadAllBytes(args[0]);
            var extractor = new InstructionExtractor(fileContents);

            while(extractor.NextInstructionAddress < fileContents.Length)
            {
                var add = extractor.NextInstructionAddress;
                var instr = extractor.ExtractInstruction();

                var formStrParts = instr.FormatString.Split(' ');
                var formString = formStrParts.Length == 1 ? formStrParts[0] : $"{formStrParts[0],-4} {formStrParts[1]}";

                var instrBytes = string.Join(" ", instr.RawBytes.Select(b => $"{b:X2}").ToArray());
                
                var text = string.Format(formString, instr.Operands.Select(
                    o => o.Type == OperandType.ImmediateByte || o.Type == OperandType.IndexRegisterOffset
                        ? $"{o.Value:X2}h"
                        : $"{o.Value:X4}h").Cast<object>().ToArray());
                Console.WriteLine($"{add:X4}  {instrBytes,-11}  {text}");
            }
        }
    }
}
