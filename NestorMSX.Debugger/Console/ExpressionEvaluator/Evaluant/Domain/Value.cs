using System;

namespace Evaluant.Calculator.Domain
{
	public class Value : LogicalExpression
	{
        public Value(string text, ValueType type)
        {
            this.text = text;
            this.type = type;
        }

		private string text;
		
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

        private ValueType type;

        public ValueType Type
		{
			get { return type; }
			set { type = value; }
		}

        public override void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

	public enum ValueType
	{
		Integer,
		String,
		DateTime,
		Float,
		Boolean
	}
}
