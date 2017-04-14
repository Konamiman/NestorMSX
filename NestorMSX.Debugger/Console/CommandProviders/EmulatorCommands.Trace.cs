using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public partial class EmulatorCommandsProvider
    {
        [Alias("print")]
        public string PrintEvaluated(string str)
        {
            return EvaluateInterpolatedString(str);
        }

        private int LastTraceNumberUsed = 0;

        private class TraceInfo
        {
            public int Number { get; set; }
            public TraceType Type { get; set; }
            public bool IsActive { get; set; }
            public string Condition { get; set; }
            public string ToPrint { get; set; }

            public string ActiveOrPausedName => IsActive ? "ACTIVE" : "PAUSED";
        }

        public enum TraceType
        {
            BeforeInstructionFetch,
            BeforeInstructionExecution
        }

        private ConcurrentDictionary<TraceType, string> TraceTypeNames;

        private Dictionary<int, TraceInfo> Traces = new Dictionary<int, TraceInfo>();
        private bool bifTraceHooked = false;
        private bool bieTraceHooked = false;

        private object syncObject = new object();

        [Alias("traces")]
        public string Trace([RawExpression]string command=null, [RawExpression]string parameter=null, string toPrint=null)
        {
            if (TraceTypeNames == null) {
                TraceTypeNames = new ConcurrentDictionary<TraceType, string>();
                TraceTypeNames[TraceType.BeforeInstructionFetch] = "Before instruction fetch";
                TraceTypeNames[TraceType.BeforeInstructionExecution] = "Before instruction execution";
            }

            if (command.IsEmpty())
                return $"{Traces.Count} traces defined";

            if (command.IsCommandCI("off")) {
                var trace = FindTraceInfo(parameter);
                trace.IsActive = false;
                AdjustTraceEventHandlers();
                return $"Trace {trace.Number} paused";
            }

            if (command.IsCommandCI("on"))
            {
                var trace = FindTraceInfo(parameter);
                trace.IsActive = false;
                AdjustTraceEventHandlers();
                return $"Trace {trace.Number} unpaused";
            }

            if (command.IsCommandCI("list")) {
                var sb = new StringBuilder($"{Traces.Count} traces defined");
                foreach (var key in Traces.Keys) {
                    var trace = Traces[key];
                    sb.Append($"\r\n\r\n{key} - {TraceTypeNames[trace.Type]} trace - {trace.ActiveOrPausedName}:\r\n");
                    sb.Append($"  when: {trace.Condition}\r\n");
                    sb.Append($"  show: {trace.ToPrint}");
                }
                return sb.ToString();
            }

            if (command.IsCommandCI("del")) {
                TraceInfo trace;
               // lock (syncObject) {
                    trace = FindTraceInfo(parameter);
                    Traces.Remove(trace.Number);
                //}
                AdjustTraceEventHandlers();
                return $"Trace {trace.Number} deleted";
            }

            if (command.IsCommandCI("delall"))
            {
               // lock (syncObject) {
                    Traces.Clear();
                    LastTraceNumberUsed = 0;
               // }
                AdjustTraceEventHandlers();
                return "All traces deleted";
            }

            if (command.IsCommandCI("bif"))
                return AddNewTrace(parameter, toPrint, TraceType.BeforeInstructionFetch);

            if (command.IsCommandCI("bie"))
                return AddNewTrace(parameter, toPrint, TraceType.BeforeInstructionExecution);

            throw new CommandExecutionException($"Unknown subcommand: {command}");
        }

        private string AddNewTrace(string parameter, string toPrint, TraceType type)
        {
            if (parameter.IsEmpty())
                throw new CommandExecutionException("condition can't be empty");

            if (toPrint.IsEmpty())
                throw new CommandExecutionException("format string can't be empty");

            try
            {
                EvaluateExpression(parameter);
            }
            catch (Exception ex)
            {
                throw new CommandExecutionException($"Can't evaluate supplied expression: {ex.Message}");
            }

            try
            {
                EvaluateInterpolatedString(toPrint);
            }
            catch (Exception ex)
            {
                throw new CommandExecutionException($"Can't evaluate supplied format string: {ex.Message}");
            }

            LastTraceNumberUsed++;
            var trace = new TraceInfo
            {
                Number = LastTraceNumberUsed,
                Condition = parameter,
                IsActive = true,
                ToPrint = toPrint,
                Type = type
            };

            //lock (syncObject) {
                Traces[trace.Number] = trace;
            //}
            AdjustTraceEventHandlers();
            return $"Added new trace with number: {trace.Number}";
        }

        private void AdjustTraceEventHandlers()
        {
            var activeBieTraces = Traces.Values.Count(v => v.IsActive);
            if (activeBieTraces > 0 && !bieTraceHooked) {
                cpu.BeforeInstructionExecution += Trace_BeforeInstructionExecution;
                bieTraceHooked = true;
            }
            else if (activeBieTraces == 0 && bieTraceHooked) {
                cpu.BeforeInstructionExecution -= Trace_BeforeInstructionExecution;
                bieTraceHooked = false;
            }

            var activeBifTraces = Traces.Values.Count(v => v.IsActive);
            if (activeBifTraces > 0 && !bifTraceHooked)
            {
                cpu.BeforeInstructionFetch += Trace_BeforeInstructionFetch;
                bifTraceHooked = true;
            }
            else if (activeBifTraces == 0 && bifTraceHooked)
            {
                cpu.BeforeInstructionFetch -= Trace_BeforeInstructionFetch;
                bifTraceHooked = false;
            }
        }

        private TraceInfo FindTraceInfo(string parameter)
        {
            int traceNumber;
            if(!int.TryParse(parameter, out traceNumber))
                throw new CommandExecutionException("Invalid command number");

            if(!Traces.ContainsKey(traceNumber))
                throw new CommandExecutionException("No trace defined with such number");

            return Traces[traceNumber];
        }

        private void Trace_BeforeInstructionExecution(object sender, BeforeInstructionExecutionEventArgs beforeInstructionExecutionEventArgs)
        {
            PrintForProperTraces(TraceType.BeforeInstructionExecution);
        }

        private void Trace_BeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs beforeInstructionFetchEventArgs)
        {
            PrintForProperTraces(TraceType.BeforeInstructionFetch);
        }

        private void PrintForProperTraces(TraceType type)
        {
            //lock (syncObject) {
                var traces = Traces.Values.ToArray();
                //traces = traces
                //    //.Where(t => t.Type == type && t.IsActive)
                //    .Where(t => Convert.ToBoolean(EvaluateExpression(t.Condition)))
                //    .ToArray();
            //var x = traces.Length;
            //if(traces.Length>0)
            //    PrintEvaluated(traces[0].ToPrint);
            Debug.WriteLine(regs.PC);
            for (var i = 0; i < traces.Length; i++)
            {
                if(Convert.ToBoolean(EvaluateExpression(traces[i].Condition)))
                    Print(PrintEvaluated(traces[i].ToPrint));
            }
            //}
        }

        private string EvaluateInterpolatedString(string str)
        {
            var sb = new StringBuilder();
            var paramValues = new List<object>();
            int prevStartIndex = 0;
            int paramIndex = 0;
            while (true)
            {
                var startIndex = str.IndexOf("{", prevStartIndex);
                if (startIndex == -1)
                    break;
                var endIndex = str.IndexOf("}", startIndex);
                if (endIndex == -1)
                    break;

                var rawParameter = str.Substring(startIndex + 1, endIndex - startIndex - 1);
                var parameterParts = rawParameter.Split(':');

                paramValues.Add(EvaluateExpression(parameterParts[0]));

                sb.Append(str.Substring(prevStartIndex, startIndex - prevStartIndex));
                if (parameterParts.Length == 1)
                    sb.Append($"{{{paramIndex}}}");
                else
                    sb.Append($"{{{paramIndex}:{parameterParts[1]}}}");

                prevStartIndex = endIndex + 1;
                paramIndex++;
            }

            return string.Format(sb.ToString(), paramValues.ToArray());
        }
    }
}
