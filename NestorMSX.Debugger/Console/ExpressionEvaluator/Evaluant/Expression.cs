using System;
using System.Collections;
using System.Text;
using Antlr.Runtime.Tree;
using Evaluant.Calculator.Domain;
using Antlr.Runtime;

namespace Evaluant.Calculator
{
    public class Expression
    {
        protected string expression;

        public Expression(string expression)
        {
            if (expression == null || expression == String.Empty)
                throw new 
                    ArgumentException("Expression can't be empty", "expression");

            this.expression = expression;
        }

        protected CommonTree Parse(string expression)
        {
            ECalcLexer lexer = new ECalcLexer(new ANTLRStringStream(expression));
            ECalcParser parser = new ECalcParser(new CommonTokenStream(lexer));

            try
            {
                RuleReturnScope rule = parser.expression();
                if (parser.HasError)
                {
                    throw new EvaluationException(parser.ErrorMessage + " " + parser.ErrorPosition);
                }

                return rule.Tree as CommonTree;
            }
            catch (EvaluationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new EvaluationException(e.Message, e);
            }
        }

        public object Evaluate()
        {
            EvaluationVisitor visitor = new EvaluationVisitor();
            visitor.EvaluateFunction += EvaluateFunction;
            visitor.EvaluateParameter += EvaluateParameter;
            visitor.Parameters = parameters;

            LogicalExpression.Create(Parse(expression)).Accept(visitor);
            return visitor.Result;
        }

        public event EvaluateFunctionHandler EvaluateFunction;
        public event EvaluateParameterHandler EvaluateParameter;

        private Hashtable parameters = new Hashtable();

        public Hashtable Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

    }
}
