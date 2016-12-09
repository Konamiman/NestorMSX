using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konamiman.NestorMSX.Z80Debugger.Console.CommandInterpreter;
using Konamiman.NestorMSX.Z80Debugger.Console.ExpressionEvaluator;
using NUnit.Framework;

namespace NestorMSX.Debugger.Tests
{
    public partial class CommandInterpreterTests
    {
        [Name("SomeStaticCommands")]
        class ClassWithStaticCommands
        {
            public static readonly string SimpleCommandResult = "SimpleCommand result";

            public static readonly string SimpleCommandWithAliasesResult = "SimpleCommandWithAliases result";

            [Alias("sum")]
            public static int Addition(int num1, int num2, int num3, int num4 = 1, int num5 = 2, int num6 = 3)
            {
                return num1 + num2 + num3 + num4 + num5 + num6;
            }

            [Alias("rwprop")]
            public static int ReadAndWriteProperty { get; set; }

            public static string TryGetVariableValue_Name => "oktryget";
            public static string TryGetVariableValue_Value => "ok value TryGet";
            public static bool TryGetVariableValue(string name, out object value)
            {
                if(name == TryGetVariableValue_Name) {
                    value = TryGetVariableValue_Value;
                    return true;
                }
                else {
                    value = null;
                    return false;
                }
            }

            public static string TrySetVariableValue_Name => "oktryset";
            public static object TrySetVariableValue_Value { get; set; }
            public static bool TrySetVariableValue(string name, object value)
            {
                if(name == TrySetVariableValue_Name) {
                    TrySetVariableValue_Value = value;
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        [Test]
        public void CanExecuteStaticCommands()
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {typeof(ClassWithStaticCommands)});
            var result = sut.ExecuteCommand("sum 10 20 30");
            Assert.AreEqual(10+20+30+1+2+3, result);
        }

        [Test]
        public void CanGetStaticPropertyValue()
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {typeof(ClassWithStaticCommands)});
            sut.ExecuteCommand("rwprop=1234");
            var result = sut.ExecuteCommand("rwprop");
            Assert.AreEqual(1234, result);
        }

        [Test]
        public void CanGetStaticPropertyValueViaFallback()
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {typeof(ClassWithStaticCommands)});
            var result = sut.ExecuteCommand(ClassWithStaticCommands.TryGetVariableValue_Name);
            Assert.AreEqual(result, ClassWithStaticCommands.TryGetVariableValue_Value);
        }
        [Test]
        public void CanSetStaticPropertyValueViaFallback()
        {
            var sut = new CommandInterpreter(new EvaluantExpressionEvaluatorWrapper(), new object[] {typeof(ClassWithStaticCommands)});
            var result = sut.ExecuteCommand($"{ClassWithStaticCommands.TrySetVariableValue_Name}=1234");
            Assert.AreEqual(1234, ClassWithStaticCommands.TrySetVariableValue_Value);
        }
    }
}
