using System;

namespace Evaluant.Calculator
{
    public class FunctionArgs : EventArgs
    {
        public FunctionArgs()
        {
        }

        private object result;

        public object Result
        {
            get { return result; }
            set { result = value; }
        }

        private object[] parameters = new object[0];

        public object[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

    }
}
