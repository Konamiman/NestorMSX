using System;
using System.Text;
using System.Collections;
using System.Globalization;

namespace Evaluant.Calculator.Domain
{
    public class EvaluationVisitor : LogicalExpressionVisitor
    {
        private NumberFormatInfo numberFormatInfo;

        public EvaluationVisitor()
        {
            numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
        }

        protected object result;
        public object Result
        {
            get { return result; }
        }

        private object Evaluate(LogicalExpression expression)
        {
            expression.Accept(this);
            return result;
        }

        public override void Visit(LogicalExpression expression)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Visit(BinaryExpresssion expression)
        {
            // Evaluates the left expression and saves the value
            expression.LeftExpression.Accept(this);
            object left = result;

            // Evaluates the right expression and saves the value
            expression.RightExpression.Accept(this);
            object right = result;

            switch (expression.Type)
            {
                case BinaryExpressionType.And:
                    result = Convert.ToBoolean(left) && Convert.ToBoolean(right);
                    break;

                case BinaryExpressionType.Or:
                    result = Convert.ToBoolean(left) || Convert.ToBoolean(right);
                    break;

                case BinaryExpressionType.Div:
                    result = Convert.ToDecimal(left) / Convert.ToDecimal(right);
                    break;

                case BinaryExpressionType.Equal:
                    // Use the type of the left operand to make the comparison
                    result = Comparer.Default.Compare(left, 
                        Convert.ChangeType(right, left.GetType())) == 0;
                    break;

                case BinaryExpressionType.Greater:
                    // Use the type of the left operand to make the comparison
                    result = Comparer.Default.Compare(left, 
                        Convert.ChangeType(right, left.GetType())) > 0;
                    break;

                case BinaryExpressionType.GreaterOrEqual:
                    // Use the type of the left operand to make the comparison
                    result = Comparer.Default.Compare(left, 
                        Convert.ChangeType(right, left.GetType())) >= 0;
                    break;

                case BinaryExpressionType.Lesser:
                    // Use the type of the left operand to make the comparison
                    result = Comparer.Default.Compare(left, 
                        Convert.ChangeType(right, left.GetType())) < 0;
                    break;

                case BinaryExpressionType.LesserOrEqual:
                    // Use the type of the left operand to make the comparison
                    result = Comparer.Default.Compare(left, 
                        Convert.ChangeType(right, left.GetType())) <= 0;
                    break;

                case BinaryExpressionType.Minus:
                    result = Convert.ToDecimal(left) - Convert.ToDecimal(right);
                    break;

                case BinaryExpressionType.Modulo:
                    result = Convert.ToDecimal(left) % Convert.ToDecimal(right);
                    break;

                case BinaryExpressionType.NotEqual:
                    // Use the type of the left operand to make the comparison
                    result = Comparer.Default.Compare(left, 
                        Convert.ChangeType(right, left.GetType())) != 0;
                    break;

                case BinaryExpressionType.Plus:
                    result = Convert.ToDecimal(left) + Convert.ToDecimal(right);
                    break;

                case BinaryExpressionType.Times:
                    result = Convert.ToDecimal(left) * Convert.ToDecimal(right);
                    break;

                case BinaryExpressionType.Pow:
                    result = Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));
                    break;

            }
        }

        public override void Visit(UnaryExpression expression)
        {
            // Recursively evaluates the underlying expression
            expression.Expression.Accept(this);

            switch (expression.Type)
            {
                case UnaryExpressionType.Not:
                    result = !Convert.ToBoolean(result);
                    break;

                case UnaryExpressionType.Negate:
                    result = -Convert.ToDecimal(result);
                    break;
            }
        }

        public override void Visit(Value expression)
        {
            switch (expression.Type)
            {
                case ValueType.Boolean:
                    result = bool.Parse(expression.Text);
                    break;

                case ValueType.DateTime:
                    result = DateTime.Parse(expression.Text.Substring(1, expression.Text.Length-2));
                    break;

                case ValueType.Float:
                    result = decimal.Parse(expression.Text, numberFormatInfo);
                    break;

                case ValueType.Integer:
                    result = int.Parse(expression.Text);
                    break;

                case ValueType.String:
                    result = expression.Text;
                    break;
            }
        }

        public override void Visit(Function function)
        {
            // Evaluates all parameters
            object[] parameters = new object[function.Expressions.Length];
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = Evaluate(function.Expressions[i]);

            FunctionArgs args = new FunctionArgs();
            args.Parameters = parameters;

            // Calls external implementation
            OnEvaluateFunction(function.Identifier, args);

            // If an external implementation was found get the result back
            if (args.Result != null)
            {
                result = args.Result;
                return;
            }

            switch (function.Identifier)
            {
                #region Abs
                case "Abs":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Abs() takes exactly 1 argument");

                    result = Math.Abs(Convert.ToDecimal(
                        Evaluate(function.Expressions[0]))
                        );

                    break;

                #endregion

                #region Acos
                case "Acos":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Acos() takes exactly 1 argument");

                    result = Math.Acos(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Asin
                case "Asin":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Asin() takes exactly 1 argument");

                    result = Math.Asin(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Atan
                case "Atan":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Atan() takes exactly 1 argument");

                    result = Math.Atan(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Ceiling
                case "Ceiling":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Ceiling() takes exactly 1 argument");

                    result = Math.Ceiling(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Cos

                case "Cos":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Cos() takes exactly 1 argument");

                    result = Math.Cos(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Exp
                case "Exp":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Exp() takes exactly 1 argument");

                    result = Math.Exp(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Floor
                case "Floor":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Floor() takes exactly 1 argument");

                    result = Math.Floor(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region IEEERemainder
                case "IEEERemainder":

                    if (function.Expressions.Length != 2)
                        throw new ArgumentException("IEEERemainder() takes exactly 2 arguments");

                    result = Math.IEEERemainder(Convert.ToDouble(Evaluate(function.Expressions[0])), Convert.ToDouble(Evaluate(function.Expressions[1])));

                    break;

                #endregion

                #region Log
                case "Log":

                    if (function.Expressions.Length != 2)
                        throw new ArgumentException("Log() takes exactly 2 arguments");

                    result = Math.Log(Convert.ToDouble(Evaluate(function.Expressions[0])), Convert.ToDouble(Evaluate(function.Expressions[1])));

                    break;

                #endregion

                #region Log10
                case "Log10":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Log10() takes exactly 1 argument");

                    result = Math.Log10(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Pow
                case "Pow":

                    if (function.Expressions.Length != 2)
                        throw new ArgumentException("Pow() takes exactly 2 arguments");

                    result = Math.Pow(Convert.ToDouble(Evaluate(function.Expressions[0])), Convert.ToDouble(Evaluate(function.Expressions[1])));

                    break;

                #endregion

                #region Round
                case "Round":

                    if (function.Expressions.Length != 2)
                        throw new ArgumentException("Round() takes exactly 2 arguments");

                    result = Math.Round(Convert.ToDouble(Evaluate(function.Expressions[0])), Convert.ToInt16(Evaluate(function.Expressions[1])));

                    break;

                #endregion

                #region Sign
                case "Sign":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Sign() takes exactly 1 argument");

                    result = Math.Sign(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Sin
                case "Sin":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Sin() takes exactly 1 argument");

                    result = Math.Sin(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Sqrt
                case "Sqrt":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Sqrt() takes exactly 1 argument");

                    result = Math.Sqrt(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Tan
                case "Tan":

                    if (function.Expressions.Length != 1)
                        throw new ArgumentException("Tan() takes exactly 1 argument");

                    result = Math.Tan(Convert.ToDouble(Evaluate(function.Expressions[0])));

                    break;

                #endregion

                #region Max
                case "Max":

                    if (function.Expressions.Length != 2)
                        throw new ArgumentException("Max() takes exactly 2 arguments");

                    decimal maxleft = Convert.ToDecimal(Evaluate(function.Expressions[0]));
                    decimal maxright = Convert.ToDecimal(Evaluate(function.Expressions[1]));

                    result = Math.Max(maxleft, maxright);
                    break;

                #endregion

                #region Min
                case "Min":

                    if (function.Expressions.Length != 2)
                        throw new ArgumentException("Min() takes exactly 2 arguments");

                    decimal minleft = Convert.ToDecimal(Evaluate(function.Expressions[0]));
                    decimal minright = Convert.ToDecimal(Evaluate(function.Expressions[1]));

                    result = Math.Min(minleft, minright);
                    break;

                #endregion

                #region if
                case "if":

                    if (function.Expressions.Length != 3)
                        throw new ArgumentException("if() takes exactly 3 arguments");

                    bool cond = Convert.ToBoolean(Evaluate(function.Expressions[0]));
                    object then = Evaluate(function.Expressions[1]);
                    object els = Evaluate(function.Expressions[2]);

                    result = cond ? then : els;
                    break;

                #endregion

                default:
                    throw new ArgumentException("Function not found", 
                        function.Identifier);
            }
        }

        public event EvaluateFunctionHandler EvaluateFunction;

        private void OnEvaluateFunction(string name, FunctionArgs args)
        {
            if (EvaluateFunction != null)
                EvaluateFunction(name, args);
        }

        public override void Visit(Parameter parameter)
        {
            if (parameters.Contains(parameter.Name))
            {
                // The parameter is defined in the hashtable
                if (parameters[parameter.Name] is Expression)
                {
                    // The parameter is itself another Expression
                    Expression expression = (Expression)parameters[parameter.Name];
                    expression.Parameters = parameters;
                    expression.EvaluateFunction += EvaluateFunction;
                    expression.EvaluateParameter += EvaluateParameter;

                    result = ((Expression)parameters[parameter.Name]).Evaluate();
                }
                else
                    result = parameters[parameter.Name];
            }
            else
            {
                // The parameter should be defined in a call back method
                ParameterArgs args = new ParameterArgs();

                // Calls external implementation
                OnEvaluateParameter(parameter.Name, args);

                if (args.Result == null)
                    throw new ArgumentException("Parameter was not defined", parameter.Name);

                result = args.Result;
            }
        }

        public event EvaluateParameterHandler EvaluateParameter;

        private void OnEvaluateParameter(string name, ParameterArgs args)
        {
            if (EvaluateParameter != null)
                EvaluateParameter(name, args);
        }

        private Hashtable parameters = new Hashtable();

        public Hashtable Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

    }
}
