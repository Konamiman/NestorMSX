using System;
using System.Text.RegularExpressions;
using Evaluant.Calculator;

namespace Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator
{
    public class EvaluantExpressionEvaluatorWrapper : IExpressionEvaluator
    {
        public object Evaluate(string expression)
        {
            var exp = new Evaluant.Calculator.Expression(Preprocess(expression));
		    exp.EvaluateFunction += (name, args) =>
		    {
			    var ea = new EvaluateFunctionEventArgs
			    {
				    Name = name,
				    Parameters = args.Parameters
			    };
			    EvaluateFunction?.Invoke(exp, ea);
			    args.Result = ea.Result;
		    };
            exp.EvaluateParameter += (name, args) =>
		    {
			    var ea = new EvaluateNameEventArgs
			    {
				    Name = name
			    };
			    EvaluateName?.Invoke(exp, ea);
			    args.Result = ea.Result;
		    };

            try {
                return exp.Evaluate();
            }
            catch (EvaluationException ex) {
                throw new CommandExecutionException($"Error when evaluating expression: {ex.Message}", ex);
            }
        }

        private static string Preprocess(string expression)
        {
            try {
                //Swap apostrophes and quotes, adjusting escapes, e.g.:
                //"O'Brian said \"Hello!\"" --> 'O\'Brian said "Hello!"'
                expression = Regex.Replace(expression,
                    @"(?<escapquot>\\"")|(?<quot>"")|(?<apos>')",
                    e => e.Groups["escapquot"].Value != "" ? @"""" : e.Groups["quot"].Value != "" ? @"'" : @"\'");

                //Replace hex and bin literals with decimal literals
                //Hex can start with 0x, &H or #, or end with h
                //Bin can start with &B or %, or end with b
                expression = Regex.Replace(expression,
                    @"('[^'\\]*(?:\\.[^'\\]*)*')" + //anything in single quotes (string literals) is to be untouched
                    @"|" +
                    @"(((0[xX])|(&[Hh])|#)(?<hvalue>[0-9A-Fa-f]+))|((?<hvalue>[0-9][0-9A-Fa-f]*)(?![\(\]])(H|h))" +
                    //hex numbers
                    @"|" +
                    @"(((&[bB])|%)(?<bvalue>[0-9A-Fa-f]+))|((?<bvalue>[0-9][0-9A-Fa-f]*)(?![\(\]])(B|b))",
                    //bin numbers - [0-F] on purpose to catch FormatExceptions
                    e =>
                        e.Groups["hvalue"].Value != ""
                            ? int.Parse(e.Groups["hvalue"].Value, System.Globalization.NumberStyles.HexNumber)
                                .ToString()
                            : e.Groups["bvalue"].Value != ""
                                ? Convert.ToInt32(e.Groups["bvalue"].Value, 2).ToString()
                                : e.Value //must be a string literal if we reach this point
                    );

                //Enclose variable names in [ ] but skipping string literals
                expression = Regex.Replace(expression,
                    @"('[^'\\]*(?:\\.[^'\\]*)*')|((?>[A-Za-z_][A-Za-z0-9_\.]*)(?!\())",
                    e => e.Groups[1].Value != "" ? e.Value : "[" + e.Value + "]");

                return expression;
            }
            catch(FormatException ex) {
                throw new CommandExecutionException("Error parsing command, possible bad hexadecimal or binary number", ex);
            }
        }

        public event EventHandler<EvaluateFunctionEventArgs> EvaluateFunction;
        public event EventHandler<EvaluateNameEventArgs> EvaluateName;
    }
}
