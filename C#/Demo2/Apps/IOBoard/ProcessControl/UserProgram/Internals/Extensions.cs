using System.Collections.Generic;
using System.Linq;


namespace UserProgram.Internals
{
    public static class Extensions
    {
        public static bool IsStartsWith(this IEnumerable<Token> tokens, params TokenTypes[] tokenTypes)
        {
            if (tokenTypes.Length > tokens.Count())
                return false;

            for (int index = 0; index < tokenTypes.Length; index++)
            {
                if (tokenTypes[index] != tokens.ElementAt(index).Type)
                    return false;
            }

            return true;
        }

        public static bool IsEndsWith(this IEnumerable<Token> tokens, params TokenTypes[] tokenTypes)
        {
            if (tokenTypes.Length > tokens.Count())
                return false;

            int offset = tokens.Count() - tokenTypes.Length;

            for (int index = 0; index < tokenTypes.Length; index++)
            {
                if (tokenTypes[index] != tokens.ElementAt(offset + index).Type)
                    return false;
            }

            return true;
        }

        public static List<Token> Copy(this IEnumerable<Token> source, int startIndex)
        {
            List<Token> dest = new List<Token>();

            for (int i = startIndex; i < source.Count(); i++)
            {
                Token originalToken = source.ElementAt(i);
                dest.Add(new Token(originalToken.Type, originalToken.Value));
            }

            return dest;
        }

        public static List<Token> Copy(this IEnumerable<Token> source, int startIndex, int length)
        {
            List<Token> dest = new List<Token>();
            int copyTo = startIndex + length;

            for (int i = startIndex; i < copyTo; i++)
            {
                Token originalToken = source.ElementAt(i);
                dest.Add(new Token(originalToken.Type, originalToken.Value));
            }

            return dest;
        }

        public static bool IsPatternMatch(this IEnumerable<Token> tokens, params TokenTypes[] tokenTypes)
        {
            if (tokenTypes.Length != tokens.Count())
                return false;

            for (int index = 0; index < tokenTypes.Length; index++)
            {
                if (tokenTypes[index] != tokens.ElementAt(index).Type)
                    return false;
            }

            return true;
        }

        public static bool IsContains(this List<Token> tokens, TokenTypes tokenType, out int index)
        {
            foreach (Token token in tokens)
            {
                if (tokenType == token.Type)
                {
                    index = tokens.IndexOf(token);
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public static int GetFirstElementIndex(this List<Token> tokens, TokenTypes tokenType)
        {
            foreach(Token token in tokens)
            {
                if (token.Type == tokenType)
                    return tokens.IndexOf(token);
            }

            return -1;
        }
    }
}
