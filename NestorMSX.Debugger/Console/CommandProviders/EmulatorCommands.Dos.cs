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

        public string TraceDos([RawExpression]string include=null, [RawExpression]string exclude=null)
        {
            if(include.IsEmpty() && exclude.IsEmpty()) {
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

            if(include.HasValue() && exclude.HasValue())
                throw new CommandExecutionException("Can't specify both include and exclude lists");

            if(include.IsCommandCI("off")) {
                if(dosTracingState != DosTracingState.Off) {
                    dosTracingState = DosTracingState.Off;
                    cpu.BeforeInstructionFetch -= BeforeInstructionFetchForDosCall;
                }
                return "DOS tracing disabled";
            }
            else if(include.IsCommandCI("pause")) {
                if(dosTracingState == DosTracingState.Off)
                    return "DOS tracing is OFF, no action taken";
                else if(dosTracingIsPaused)
                    return "DOS tracing already paused, no action taken";
                dosTracingIsPaused = true;
                return "ok";
            }
            else if(include.IsCommandCI("resume")) {
                if(dosTracingState == DosTracingState.Off)
                    return "DOS tracing is OFF, no action taken";
                else if(!dosTracingIsPaused)
                    return "DOS tracing not paused, no action taken";
                dosTracingIsPaused = false;
                return "ok";
            }

            var functionCodesAsStrings = (include ?? exclude).Split(comma, StringSplitOptions.RemoveEmptyEntries);
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

            if(include == null) {
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
                    //if (r.B == 5 && r.HL == 4062)
                    //    r.HL = 4091;
                }
                //if (r.C == DosFunctions.CodeOf("CLOSE"))
                //{
                //    Print($"  FH={r.B}");
                //}
            }
        }
    }
}
