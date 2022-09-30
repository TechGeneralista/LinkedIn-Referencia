using System.Text;

namespace SQLBridgeApp
{
    public static class Constants
    {
        public const string CreateConnection = "cc";
        public const char MessageSeparator = '/';
        public const string TestConnection = "tc";
        public const string QueryExecute = "qe";
        public const string Ok = "ok";
        public const string Error = "er";


        public static string Concat(params string[] strings)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < strings.Length; i++)
            {
                stringBuilder.Append(strings[i]);

                if (i != (strings.Length - 1))
                    stringBuilder.Append(MessageSeparator);
            }

            return stringBuilder.ToString();
        }
    }
}
