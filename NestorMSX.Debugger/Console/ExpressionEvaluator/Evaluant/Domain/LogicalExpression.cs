using System;
using Antlr.Runtime.Tree;
using System.Text;

namespace Evaluant.Calculator.Domain
{
    public abstract class LogicalExpression
    {
        const char BS = '\\';

        private static string extractString(string text)
        {

            StringBuilder sb = new StringBuilder(text);
            int startIndex = 1; // Skip initial quote
            int slashIndex = -1;

            while ((slashIndex = sb.ToString().IndexOf(BS, startIndex)) != -1)
            {
                char escapeType = sb[slashIndex + 1];
                switch (escapeType)
                {
                    case 'u':
                        string hcode = String.Concat(sb[slashIndex + 4], sb[slashIndex + 5]);
                        string lcode = String.Concat(sb[slashIndex + 2], sb[slashIndex + 3]);
                        char unicodeChar = Encoding.Unicode.GetChars(new byte[] { System.Convert.ToByte(hcode, 16), System.Convert.ToByte(lcode, 16) })[0];
                        sb.Remove(slashIndex, 6).Insert(slashIndex, unicodeChar);
                        break;
                    case 'n': sb.Remove(slashIndex, 2).Insert(slashIndex, '\n'); break;
                    case 'r': sb.Remove(slashIndex, 2).Insert(slashIndex, '\r'); break;
                    case 't': sb.Remove(slashIndex, 2).Insert(slashIndex, '\t'); break;
                    case '\'': sb.Remove(slashIndex, 2).Insert(slashIndex, '\''); break;
                    case '\\': sb.Remove(slashIndex, 2).Insert(slashIndex, '\\'); break;
                    default: throw new ApplicationException("Unvalid escape sequence: \\" + escapeType);
                }

                startIndex = slashIndex + 1;

            }

            sb.Remove(0, 1);
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static LogicalExpression Create(CommonTree ast)
        {
            if (ast == null)
                throw new ArgumentNullException("tree");

            switch (ast.Type)
            {
                case ECalcParser.STRING:
                    return new Value(extractString(ast.Text), ValueType.String);

                case ECalcParser.INTEGER:
                    return new Value(ast.Text, ValueType.Integer);

                case ECalcParser.BOOLEAN:
                    return new Value(ast.Text, ValueType.Boolean);

                case ECalcParser.DATETIME:
                    return new Value(ast.Text, ValueType.DateTime);

                case ECalcParser.FLOAT:
                    return new Value(ast.Text, ValueType.Float);

                case ECalcParser.NOT:
                    return new UnaryExpression(UnaryExpressionType.Not,
                        Create((CommonTree)ast.GetChild(0)));

                case ECalcParser.NEGATE:
                    return new UnaryExpression(UnaryExpressionType.Negate,
                        Create((CommonTree)ast.GetChild(0)));

                case ECalcParser.MULT:
                    return new BinaryExpresssion(BinaryExpressionType.Times,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.POW:
                    return new BinaryExpresssion(BinaryExpressionType.Pow,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.DIV:
                    return new BinaryExpresssion(BinaryExpressionType.Div,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.MOD:
                    return new BinaryExpresssion(BinaryExpressionType.Modulo,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.PLUS:
                    return new BinaryExpresssion(BinaryExpressionType.Plus,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.MINUS:
                    return new BinaryExpresssion(BinaryExpressionType.Minus,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.LT:
                    return new BinaryExpresssion(BinaryExpressionType.Lesser,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.LTEQ:
                    return new BinaryExpresssion(BinaryExpressionType.LesserOrEqual,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.GT:
                    return new BinaryExpresssion(BinaryExpressionType.Greater,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.GTEQ:
                    return new BinaryExpresssion(BinaryExpressionType.GreaterOrEqual,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.EQUALS:
                    return new BinaryExpresssion(BinaryExpressionType.Equal,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.NOTEQUALS:
                    return new BinaryExpresssion(BinaryExpressionType.NotEqual,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.AND:
                    return new BinaryExpresssion(BinaryExpressionType.And,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcParser.OR:
                    return new BinaryExpresssion(BinaryExpressionType.Or,
                        Create((CommonTree)ast.GetChild(0)),
                        Create((CommonTree)ast.GetChild(1)));

                case ECalcLexer.IDENT:
                    LogicalExpression[] expressions = new LogicalExpression[ast.ChildCount];

                    for (int i = 0; i < ast.ChildCount; i++)
                        expressions[i] = LogicalExpression.Create((CommonTree)ast.GetChild(i));

                    return new Function(ast.Text, expressions);

                case ECalcLexer.PARAM:
                    return new Parameter(((CommonTree)ast.GetChild(0)).Text);

                default:
                    return null;
            }

        }

        public virtual void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
