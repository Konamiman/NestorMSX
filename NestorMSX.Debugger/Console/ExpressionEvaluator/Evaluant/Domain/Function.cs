using System;

namespace Evaluant.Calculator.Domain
{
	public class Function : LogicalExpression
	{
		public Function(string identifier, LogicalExpression[] expressions)
		{
            this.identifier = identifier;
            this.expressions = expressions;
		}

        private string identifier;

        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        private LogicalExpression[] expressions;

        public LogicalExpression[] Expressions
        {
            get { return expressions; }
            set { expressions = value; }
        }

        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
