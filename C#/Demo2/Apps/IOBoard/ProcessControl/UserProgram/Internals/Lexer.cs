using IOBoard;
using System;
using System.Collections.Generic;
using System.Linq;


namespace UserProgram.Internals
{
    public class Lexer
    {
        public IReadOnlyCollection<string> CodeLines { get; private set; }
        public IReadOnlyCollection<IReadOnlyCollection<Token>> Tokens { get; private set; }


        internal void CreateTokens(string code)
        {
            GetCodeLines(code);
            CheckCodeBlocks();

            List<IReadOnlyCollection<Token>> tokens = new List<IReadOnlyCollection<Token>>();
            List<Token> lineTokens;
            int linesCount = CodeLines.Count();
            int currentLineIndex = 0;
            string currentLineStr;

            while (currentLineIndex < linesCount)
            {
                lineTokens = new List<Token>();
                currentLineStr = CodeLines.ElementAt(currentLineIndex);

                if (currentLineStr.Contains('#'))
                {
                    int commentMarkerIndex = currentLineStr.IndexOf('#');
                    currentLineStr = currentLineStr.Remove(commentMarkerIndex, currentLineStr.Length - commentMarkerIndex);
                }

                if (string.IsNullOrWhiteSpace(currentLineStr))
                    lineTokens.Add(new Token(TokenTypes.EmptyLine, null));
                else
                    ReadLine(lineTokens, currentLineStr.Replace(" ", string.Empty), currentLineIndex);

                currentLineIndex += 1;
                tokens.Add(lineTokens);
            }

            Tokens = tokens;
        }

        private void ReadLine(List<Token> lineTokens, string currentLineStr, int currentLineIndex)
        {
            CheckParentheses(currentLineStr, currentLineIndex);

            int currentCharIndex = 0;
            string temp = "";
            
            while(currentCharIndex < currentLineStr.Length)
            {
                temp += currentLineStr[currentCharIndex];

                if (temp.Length == 1 && 
                   (temp[0] == '{' || 
                    temp[0] == '}' || 
                    temp[0] == '+' ||
                    temp[0] == '-' ||
                    temp[0] == '*' ||
                    temp[0] == '/' ||
                    temp[0] == '(' ||
                    temp[0] == ')'))
                {
                    AddElement(lineTokens, temp, currentLineIndex);
                    currentCharIndex += 1;
                    temp = "";
                }

                else if (temp.Length > 1)
                {
                    if(char.IsLetterOrDigit(temp[temp.Length - 2]) && temp[temp.Length - 1] == ':')
                    {
                        AddElement(lineTokens, temp, currentLineIndex);
                        currentCharIndex += 1;
                        temp = "";
                    }
                    
                    else if (char.IsLetterOrDigit(temp[temp.Length-2]) && 
                            (temp[temp.Length - 1] == '=' ||
                             temp[temp.Length - 1] == '+' ||
                             temp[temp.Length - 1] == '-' ||
                             temp[temp.Length - 1] == '*' ||
                             temp[temp.Length - 1] == '/' ||
                             temp[temp.Length - 1] == '(' ||
                             temp[temp.Length - 1] == ')' ||
                             temp[temp.Length - 1] == '!' ||
                             temp[temp.Length - 1] == '&' ||
                             temp[temp.Length - 1] == '|'))
                    {
                        AddElement(lineTokens, temp.Replace(temp[temp.Length - 1].ToString(), string.Empty), currentLineIndex);
                        temp = "";
                    }

                    else if (temp.Length == 2)
                    {
                        if (temp[0] == '=' && char.IsLetterOrDigit(temp[1]))
                        {
                            AddElement(lineTokens, temp.Replace(temp[1].ToString(), string.Empty), currentLineIndex);
                            temp = "";
                        }

                        else
                            currentCharIndex += 1;
                    }

                    else if(temp.Length == 3)
                    {
                        if((temp.StartsWith("==") ||
                            temp.StartsWith("!=") ||
                            temp.StartsWith("&&") ||
                            temp.StartsWith("||")) 
                            && char.IsLetterOrDigit(temp[2]))
                        {
                            AddElement(lineTokens, temp.Replace(temp[2].ToString(), string.Empty), currentLineIndex);
                            temp = "";
                        }

                        else
                            currentCharIndex += 1;
                    }

                    else
                        currentCharIndex += 1;
                }

                else
                    currentCharIndex += 1;
            }

            AddElement(lineTokens, temp, currentLineIndex);
        }

        private void AddElement(List<Token> lineTokens, string element, int currentLineIndex)
        {
            if (element.Length == 0)
                return;

            else if (double.TryParse(element, out double d))
                lineTokens.Add(new Token(TokenTypes.Number, d));

            else if (element.Length == 1)
            {
                if (element[0] == '=')
                    lineTokens.Add(new Token(TokenTypes.ValueAssignment, '='));

                else if (element[0] == '+')
                    lineTokens.Add(new Token(TokenTypes.Addition, '+'));

                else if (element[0] == '-')
                    lineTokens.Add(new Token(TokenTypes.Subtraction, '-'));

                else if (element[0] == '*')
                    lineTokens.Add(new Token(TokenTypes.Multiplication, '*'));

                else if (element[0] == '/')
                    lineTokens.Add(new Token(TokenTypes.Division, '/'));

                else if (element[0] == '(')
                    lineTokens.Add(new Token(TokenTypes.ParenthesesOpen, '('));

                else if (element[0] == ')')
                    lineTokens.Add(new Token(TokenTypes.ParenthesesClose, ')'));

                else if (element[0] == '{')
                    lineTokens.Add(new Token(TokenTypes.BlockStart, '{'));

                else if (element[0] == '}')
                    lineTokens.Add(new Token(TokenTypes.BlockEnd, '}'));

                else if (char.IsLetter(element[0]))
                    lineTokens.Add(new Token(TokenTypes.Symbol, element));

                else
                    Error("Szintaxis hiba: A művelet ismeretlen", currentLineIndex);
            }

            else if (element.Length == 2)
            {
                if (element == "==")
                    lineTokens.Add(new Token(TokenTypes.IsEqual, "=="));

                else if (element == "!=")
                    lineTokens.Add(new Token(TokenTypes.IsNotEqual, "!="));

                else if (element == "&&")
                    lineTokens.Add(new Token(TokenTypes.LogicalAnd, "&&"));

                else if (element == "||")
                    lineTokens.Add(new Token(TokenTypes.LogicalOr, "||"));

                else if (element == "if")
                    lineTokens.Add(new Token(TokenTypes.IfStatement, element));

                else if (char.IsLetter(element[0]) && char.IsLetterOrDigit(element[1]))
                    lineTokens.Add(new Token(TokenTypes.Symbol, element));

                else
                    Error("Szintaxis hiba: A művelet ismeretlen", currentLineIndex);
            }

            else if (element.StartsWith("IOBoard"))
            {
                string[] temp = element.Split(new[] { "." }, StringSplitOptions.None);

                if (temp.Length != 4)
                    Error("Szintaxis hiba: IOBoard szimbólum", currentLineIndex);

                string serialNo = temp[1];

                if (temp[2] != "DO" && temp[2] != "DI" && temp[2] != "DIR" && temp[2] != "DIF" &&
                    temp[2] != "DigitalOutput" && temp[2] != "DigitalInput" && temp[2] != "DigitalInputRising" && temp[2] != "DigitalInputFalling")
                    Error("Szintaxis hiba: IOBoard szimbólum, nincs ilyen csatorna", currentLineIndex);

                string channelName = temp[2];

                if (!int.TryParse(temp[3], out int i))
                    Error("Szintaxis hiba: IOBoard szimbólum csatorna indexe nem szám", currentLineIndex);

                if (i < 0)
                    Error("Szintaxis hiba: IOBoard szimbólum csatorna indexe csak pozitív egész szám lehet", currentLineIndex);

                int channelIndex = i;

                lineTokens.Add(new Token(TokenTypes.IOBoardArgs, new IOBoardArgs(serialNo, channelName, channelIndex)));
            }

            else if (element.EndsWith(":"))
                lineTokens.Add(new Token(TokenTypes.Label, element.Replace(":", string.Empty)));

            else if (element == "High")
                lineTokens.Add(new Token(TokenTypes.LogicalConstant, true));

            else if (element == "Low")
                lineTokens.Add(new Token(TokenTypes.LogicalConstant, false));

            else if (element == "WaitMs")
                lineTokens.Add(new Token(TokenTypes.WaitMsFunction, element));

            else if (element == "Jump")
                lineTokens.Add(new Token(TokenTypes.JumpFunction, element));

            else if (element == "else")
                lineTokens.Add(new Token(TokenTypes.IfStatementElse, element));

            else
                lineTokens.Add(new Token(TokenTypes.Symbol, element));
        }

        private void GetCodeLines(string code)
        {
            string[] codeLines = code.Split(new[] { "\r\n" }, StringSplitOptions.None);

            for (int i = 0; i < codeLines.Length; i++)
                codeLines[i] = codeLines[i].Trim();

            CodeLines = codeLines;
        }

        private void CheckParentheses(string currentLineStr, int currentLineIndex)
        {
            int parenthesesLevel = 0;
            int charIndex = 0;

            while (charIndex < currentLineStr.Length)
            {
                char c = currentLineStr[charIndex];

                if (c == '(')
                    parenthesesLevel += 1;

                else if (c == ')')
                    parenthesesLevel -= 1;

                charIndex += 1;
            }

            if (parenthesesLevel != 0)
                Error("Szintaxis hiba: zárójelek");
        }

        private void CheckCodeBlocks()
        {
            int codeBlockLevel = 0;
            int lineIndex = 0;
            string line;

            while (lineIndex < CodeLines.Count())
            {
                line = CodeLines.ElementAt(lineIndex);

                if (line == "{")
                    codeBlockLevel += 1;

                else if (line == "}")
                    codeBlockLevel -= 1;

                lineIndex += 1;
            }

            if (codeBlockLevel != 0)
                Error("Szintaxis hiba: kód blokk");
        }

        private void Error(string message, int lineIndex) => throw new Exception(string.Format("{0} -> {1}: {2}", message, lineIndex, CodeLines.ElementAt(lineIndex)));
        private void Error(string message) => throw new Exception(message);
    }
}
