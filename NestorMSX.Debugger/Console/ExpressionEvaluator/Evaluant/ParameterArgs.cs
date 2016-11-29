using System;

namespace Evaluant.Calculator
{
    public class ParameterArgs : EventArgs
    {
        public ParameterArgs()
        {
        }

        private object result;

        public object Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
