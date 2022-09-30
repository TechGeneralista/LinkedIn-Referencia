using Common.Tool;
using System;
using System.Collections.Generic;


namespace UserProgram.Internals
{
    public class Varialables
    {
        Dictionary<string, double> numberVarialables;
        Dictionary<string, bool> logicVarialables;


        internal void Initialize()
        {
            numberVarialables = new Dictionary<string, double>();
            logicVarialables = new Dictionary<string, bool>();
        }

        public ResultValue GetSymbol(string symbolName)
        {
            if (logicVarialables.ContainsKey(symbolName))
                return new ResultValue(logicVarialables[symbolName]);

            else if (numberVarialables.ContainsKey(symbolName))
                return new ResultValue(numberVarialables[symbolName]);

            Error(string.Format("A szimbólum nem létezik: {0}", symbolName));
            return null;
        }

        public void SetSymbol(string symbolName, ResultValue value)
        {
            if (value.Logical.IsNotNull())
            {
                if (numberVarialables.ContainsKey(symbolName))
                    Error(string.Format("Már létezik ilyen szimbólum: {0}", symbolName));

                logicVarialables[symbolName] = (bool)value.Logical;
            }

            else if (value.Number.IsNotNull())
            {
                if (logicVarialables.ContainsKey(symbolName))
                    Error(string.Format("Már létezik ilyen változó: {0}", symbolName));

                numberVarialables[symbolName] = (double)value.Number;
            }
        }

        private void Error(string message) => throw new Exception(message);
    }
}