using System;


namespace SmartVisionClientApp
{
    public class CommandLineParser
    {

        string[] args;

        public CommandLineParser(string[] args)
        {
            this.args = args;
        }

        internal string GetParameter(string paramName)
        {
            string paramId = "--" + paramName + "=";
            string foundedParam = null;

            foreach(string str in args)
            {
                if(str.Contains(paramId))
                {
                    foundedParam = str.Replace(paramId, string.Empty);
                    break;
                }
            }

            return foundedParam;
        }
    }
}