using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Misc;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public partial class EmulatorCommandsProvider
    {
        private enum DosTracingState
        {
            Off = 0,
            OnInclusive = 1,
            OnExclusive = 2
        }

        private bool dosTracingIsPaused;
        private DosTracingState dosTracingState;
        private byte[] dosTracingIncluded;
        private byte[] dosTracingExcluded;

        private static readonly char[] comma = {','};

        public string TraceDos([RawExpression]string @in=null, [RawExpression]string ex=null)
        {
            if(@in.IsEmpty() && ex.IsEmpty()) {
                StringBuilder sb = new StringBuilder();
                byte[] functionCodes;
                if(dosTracingState == DosTracingState.Off)
                    return "DOS tracing is OFF";
                else if(dosTracingState == DosTracingState.OnInclusive) {
                    sb.Append("DOS tracing is ON in INCLUSIVE mode. Traced function codes:\r\n\r\n");
                    functionCodes = dosTracingIncluded;
                }
                else {
                    sb.Append("DOS tracing is ON in EXCLUSIVE mode. Excluded function codes:\r\n\r\n");
                    functionCodes = dosTracingExcluded;
                }
                foreach (var code in functionCodes)
                    sb.AppendFormat("0x{0:X2} - {1}\r\n", code, DosFunctions.NameOf(code));

                if (dosTracingIsPaused)
                    sb.Append("\r\nDOS tracing is currently PAUSED");

                return sb.ToString();
            }

            if(@in.HasValue() && ex.HasValue())
                throw new CommandExecutionException("Can't specify both include and exclude lists");

            if(@in.IsCommandCI("off")) {
                if(dosTracingState != DosTracingState.Off) {
                    dosTracingState = DosTracingState.Off;
                    cpu.BeforeInstructionFetch -= BeforeInstructionFetchForDosCall;
                }
                return "DOS tracing disabled";
            }
            else if(@in.IsCommandCI("pause")) {
                if(dosTracingState == DosTracingState.Off)
                    return "DOS tracing is OFF, no action taken";
                else if(dosTracingIsPaused)
                    return "DOS tracing already paused, no action taken";
                dosTracingIsPaused = true;
                return "ok";
            }
            else if(@in.IsCommandCI("resume")) {
                if(dosTracingState == DosTracingState.Off)
                    return "DOS tracing is OFF, no action taken";
                else if(!dosTracingIsPaused)
                    return "DOS tracing not paused, no action taken";
                dosTracingIsPaused = false;
                return "ok";
            }

            var functionCodesAsStrings = (@in ?? ex).Split(comma, StringSplitOptions.RemoveEmptyEntries);
            var functionCodesAsBytes = new List<byte>();

            foreach(var codeAsString in functionCodesAsStrings) {
                if(DosFunctions.NameExists(codeAsString)) {
                    functionCodesAsBytes.Add(DosFunctions.CodeOf(codeAsString));
                    continue;
                }

                byte code;
                try {
                    code = (byte)EvaluateExpression(codeAsString).AsInt();
                }
                catch {
                    throw new CommandExecutionException($"{codeAsString} is not a valid DOS function code expression");
                }

                if(!DosFunctions.CodeExists(code))
                    throw new CommandExecutionException($"DOS function 0x{code:X2} does not exist");

                functionCodesAsBytes.Add(code);
            }

            functionCodesAsBytes = functionCodesAsBytes.Distinct().OrderBy(x => x).ToList();

            if(dosTracingState == DosTracingState.Off) {
                cpu.BeforeInstructionFetch += BeforeInstructionFetchForDosCall;
            }

            if(@in == null) {
                dosTracingExcluded = functionCodesAsBytes.ToArray();
                dosTracingState = DosTracingState.OnExclusive;
            }
            else {
                dosTracingIncluded = functionCodesAsBytes.ToArray();
                dosTracingState = DosTracingState.OnInclusive;
            }

            dosTracingIsPaused = false;
            return "ok";
        }

        private static readonly string[] AttributeLetters = {"*","*","A","D","V","S","H","R"};

        private void BeforeInstructionFetchForDosCall(object sender, BeforeInstructionFetchEventArgs beforeInstructionFetchEventArgs)
        {
            if (dosTracingIsPaused)
                return;

            var r = regs;
            if(r.PC == 5 && mappedRam.GetBlockInBank(0) == 3
                && ((dosTracingState == DosTracingState.OnInclusive && dosTracingIncluded.Contains(r.C))
                    || (dosTracingState == DosTracingState.OnExclusive && !dosTracingExcluded.Contains(r.C)))) {
                Print($"DOS call: {r.C:X2}h - {DosFunctions.NameOf(r.C)}");
                if(r.C == DosFunctions.CodeOf("WRITE")) { 
                    Print($"  FH={r.B}, buffer=0x{r.DE:X2}, count={r.HL}");
                }
                if (r.C == DosFunctions.CodeOf("EXPLAIN"))
                {
                    Print($"  Error={r.B:X2}");
                }
                if (r.C == DosFunctions.CodeOf("ALLOC"))
                {
                    Print($"  Drive={r.E}");
                }
                if (r.C == DosFunctions.CodeOf("IOCTL"))
                {
                    Print($"  B={r.B}, A={r.A}");
                }
                if (r.C == DosFunctions.CodeOf("GENV"))
                {
                    Print($"  HL = {ExtractString(r.HL)}");
                }
                if (r.C == DosFunctions.CodeOf("CHDIR"))
                {
                    Print($"  DE = {ExtractString(r.DE)}");
                }
                if (r.C == DosFunctions.CodeOf("FFIRST") || r.C == DosFunctions.CodeOf("FNEW"))
                {
                    var firstByte = cpu.Memory[r.DE];
                    if (firstByte == 0xFF)
                    {
                        Print($"  DE = FIB: {ExtractString(r.DE+1)}");
                        Print($"  HL = {ExtractString(r.HL)}");
                    }
                    else
                    {
                        Print($"  DE = {ExtractString(r.DE)}");
                    }

                    var attrsString = AttrsString(regs.B);
                    Print($"  B = {attrsString}");
                    Print($"  IX = {r.IX:X4}h ");
                    if(r.C == DosFunctions.CodeOf("FNEW"))
                        Print($"  IX.name = {ExtractString(r.IX+1)} ");
                    SetPostDosHook();
                }
                if (r.C == DosFunctions.CodeOf("FNEXT"))
                {
                    Print($"  IX = {r.IX:X4}h ");
                }
                if (r.C == DosFunctions.CodeOf("WPATH"))
                {
                    SetPostDosHook();
                }
                if (r.C == DosFunctions.CodeOf("GETCD"))
                {
                    SetPostDosHook();
                }
            }
        }

        private static string AttrsString(byte attrsByte)
        {
            var attrsString = "";
            for (int i = 7; i >= 0; i--)
            {
                attrsString = ((attrsByte & 1) == 1 ? AttributeLetters[i] : "-") + attrsString;
                attrsByte >>= 1;
            }
            return attrsString;
        }

        private void SetPostDosHook()
        {
            lastDosFunctionHooked = regs.C;
            postDosFunctionCallSp = regs.SP + 2;
            cpu.BeforeInstructionExecution += PostDosFunctionCall;
        }

        private int postDosFunctionCallSp;
        private byte lastDosFunctionHooked;
        private void PostDosFunctionCall(object sender, BeforeInstructionExecutionEventArgs beforeInstructionExecutionEventArgs)
        {
            if (regs.SP != postDosFunctionCallSp)
                return;

            cpu.BeforeInstructionExecution -= PostDosFunctionCall;

            if (lastDosFunctionHooked == DosFunctions.CodeOf("WPATH"))
            {
                Print($"  DE = {ExtractString(regs.DE)}");
                Print($"  HL = {ExtractString(regs.HL)}");
            }
            if (lastDosFunctionHooked == DosFunctions.CodeOf("GETCD"))
            {
                Print($"  DE = {ExtractString(regs.DE)}");
            }
            if (lastDosFunctionHooked == DosFunctions.CodeOf("FFIRST") ||
                lastDosFunctionHooked == DosFunctions.CodeOf("FNEXT"))
            {
                if (regs.A != 0)
                {
                    Print($"  Error = {DosErrors.NameOf(regs.A)}");
                }
                else
                {
                    Print($"  Found: {ExtractString(regs.IX + 1)}");
                    Print($"  Attrs: {AttrsString(cpu.Memory[regs.IX + 14])}");
                }
            }
        }
        
        private string ExtractString(int address)
        {
            var bytes = new List<byte>();
            byte theByte;
            while (((theByte = cpu.Memory[address]) != 0) && bytes.Count < 256)
            {
                bytes.Add(theByte);
                address++;
            }

            return Encoding.ASCII.GetString(bytes.ToArray());
        }
    }
}
