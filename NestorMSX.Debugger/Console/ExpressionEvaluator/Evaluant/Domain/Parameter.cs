using System;

namespace Evaluant.Calculator.Domain
{
	public class Parameter : LogicalExpression
	{
		public Parameter(string name)
		{
            this.name = name;
		}

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
