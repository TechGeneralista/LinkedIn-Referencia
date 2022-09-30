using Common.Prop;
using Common.Tool;
using IOBoard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace UserProgram.Internals
{
    public class Parser
    {
        public INonSettableObservableProperty<int> CurrentRowIndex { get; } = new ObservableProperty<int>();
        public INonSettableObservableProperty<string> CurrentRowCode { get; } = new ObservableProperty<string>();
        public IReadOnlyCollection<string> CodeLines { get; private set; }


        readonly IOBoardClient ioBoardClient;
        readonly Varialables varialables;
        readonly ExpressionResolver expressionResolver;
        IReadOnlyCollection<IReadOnlyCollection<Token>> tokens;
        Stack<int> returnStack;


        public Parser(IOBoardClient ioBoardClient)
        {
            this.ioBoardClient = ioBoardClient;

            CurrentRowIndex.ValueChanged += CurrentRowIndex_ValueChanged;
            varialables = new Varialables();
            expressionResolver = new ExpressionResolver(varialables, ioBoardClient);
        }

        private void CurrentRowIndex_ValueChanged(int newValue)
        {
            if (CodeLines.IsNotNull() && CodeLines.Count() > newValue)
                CurrentRowCode.ForceSet(CodeLines.ElementAt(newValue));
        }

        internal void Initialize(IReadOnlyCollection<string> codeLines, IReadOnlyCollection<IReadOnlyCollection<Token>> tokens)
        {
            CodeLines = codeLines;
            this.tokens = tokens;

            varialables.Initialize();
            returnStack = new Stack<int>();

            CurrentRowIndex.ForceSet(GetCodeBlockStartIndex("Init"));
        }

        private int GetCodeBlockStartIndex(string labelName)
        {
            int labelRowIndex = -1;

            for(int i=0;i<tokens.Count();i++)
            {
                IReadOnlyCollection<Token> lineTokens = tokens.ElementAt(i);

                if (lineTokens.Count() != 1)
                    continue;

                Token firstToken = lineTokens.ElementAt(0);

                if (firstToken.Type == TokenTypes.Label && ((string)firstToken.Value) == labelName)
                {
                    labelRowIndex = i;
                    break;
                }
            }

            if (labelRowIndex == -1)
                Error(string.Format("Címke nem létezik: {0}", labelName));

            return labelRowIndex + 2;
        }

        internal void Loop()
        {
            IReadOnlyCollection<Token> lineTokens = tokens.ElementAt(CurrentRowIndex.Value);

            if (lineTokens.IsStartsWith(TokenTypes.EmptyLine))
                NextLine();

            else if (lineTokens.IsStartsWith(TokenTypes.BlockEnd))
            {
                if (returnStack.Count == 0)
                    CurrentRowIndex.ForceSet(GetCodeBlockStartIndex("Loop"));

                else
                    CurrentRowIndex.ForceSet(returnStack.Pop());
            }

            else if (lineTokens.IsStartsWith(TokenTypes.JumpFunction, TokenTypes.ParenthesesOpen) && lineTokens.IsEndsWith(TokenTypes.ParenthesesClose))
            {
                returnStack.Push(CurrentRowIndex.Value + 1);
                CurrentRowIndex.ForceSet(GetCodeBlockStartIndex((string)lineTokens.ElementAt(2).Value));
            }

            else if(lineTokens.IsStartsWith(TokenTypes.Symbol, TokenTypes.ValueAssignment))
            {
                try
                {
                    ResultValue resultValue = expressionResolver.Resolve(lineTokens.Copy(2));
                    varialables.SetSymbol((string)lineTokens.ElementAt(0).Value, resultValue);
                }
                catch(Exception ex)
                {
                    Error(ex.Message);
                }
                
                NextLine();
            }

            else if (lineTokens.IsStartsWith(TokenTypes.IOBoardArgs, TokenTypes.ValueAssignment))
            {
                try
                {
                    ResultValue resultValue = expressionResolver.Resolve(lineTokens.Copy(2));
                    IOBoardArgs ioBoardArgs = (IOBoardArgs)lineTokens.ElementAt(0).Value;
                    ioBoardArgs.LogicalValue = (bool)resultValue.Logical;
                    ioBoardClient.Write(ioBoardArgs);
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }

                NextLine();
            }

            else if (lineTokens.IsStartsWith(TokenTypes.WaitMsFunction, TokenTypes.ParenthesesOpen) && lineTokens.IsEndsWith(TokenTypes.ParenthesesClose))
            {
                try
                {
                    ResultValue resultValue = expressionResolver.Resolve(lineTokens.Copy(2, lineTokens.Count() - 3));
                    FunctionWaitMs(resultValue);
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }

                NextLine();
            }

            else if(lineTokens.IsStartsWith(TokenTypes.IfStatement, TokenTypes.ParenthesesOpen) && lineTokens.IsEndsWith(TokenTypes.ParenthesesClose))
            {
                ResultValue resultValue = null;

                try
                {
                    resultValue = expressionResolver.Resolve(lineTokens.Copy(2, lineTokens.Count() - 3));
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                }

                if (resultValue.Logical.IsNull())
                    Error("Az kifejezés eredménye nem lehet szám");

                IfStatement((bool)resultValue.Logical);
            }

            else
                Error("Szintaxis hiba");
        }

        private void IfStatement(bool result)
        {
            int trueBlockStartIndex = GetIfStatementTrueBlockStartIndex();
            int trueBlockEndIndex = GetIfStatementTrueBlockEndIndex();
            bool elseExist = tokens.ElementAt(trueBlockEndIndex).IsPatternMatch(TokenTypes.IfStatementElse);

            if(elseExist)
            {
                int falseBlockStartIndex = GetIfStatementFalseBlockStartIndex(trueBlockEndIndex);
                int falseBlockEndIndex = GetIfStatementFalseBlockEndIndex(trueBlockEndIndex);

                if (result)
                {
                    CurrentRowIndex.ForceSet(trueBlockStartIndex);
                    returnStack.Push(falseBlockEndIndex);
                }
                else
                {
                    CurrentRowIndex.ForceSet(falseBlockStartIndex);
                    returnStack.Push(falseBlockEndIndex);
                }
            }
            else
            {
                if(result)
                {
                    CurrentRowIndex.ForceSet(trueBlockStartIndex);
                    returnStack.Push(trueBlockEndIndex);
                }
                else
                    CurrentRowIndex.ForceSet(trueBlockEndIndex);
            }
        }

        private int GetIfStatementFalseBlockEndIndex(int fromIndex)
        {
            fromIndex += 1;
            int codeBlockLevel = 0;

            while (fromIndex < CodeLines.Count())
            {
                IEnumerable<Token> lineTokens = tokens.ElementAt(fromIndex);

                if (lineTokens.IsPatternMatch(TokenTypes.BlockStart))
                    codeBlockLevel += 1;

                else if (lineTokens.IsPatternMatch(TokenTypes.BlockEnd))
                {
                    codeBlockLevel -= 1;
                    
                    if(codeBlockLevel == 0)
                        return fromIndex + 1;
                }

                fromIndex += 1;
            }

            Error("Hiányzik a 'hamis' blokk záró karaktere");
            return -1;
        }

        private int GetIfStatementFalseBlockStartIndex(int fromIndex)
        {
            fromIndex += 1;

            if(!tokens.ElementAt(fromIndex).IsPatternMatch(TokenTypes.BlockStart))
                Error("Hiányzik a 'hamis' blokk nyitó karaktere");

            return fromIndex + 1;
        }

        private int GetIfStatementTrueBlockEndIndex()
        {
            int rowIndex = CurrentRowIndex.Value + 1;
            int codeBlockLevel = 0;

            while(rowIndex < CodeLines.Count())
            {
                IEnumerable<Token> lineTokens = tokens.ElementAt(rowIndex);

                if (lineTokens.IsPatternMatch(TokenTypes.BlockStart))
                    codeBlockLevel += 1;

                else if(lineTokens.IsPatternMatch(TokenTypes.BlockEnd))
                {
                    codeBlockLevel -= 1;
                    
                    if(codeBlockLevel == 0)
                        return rowIndex + 1;
                }

                rowIndex += 1;
            }

            Error("Hiányzik az 'igaz' blokk záró karaktere");
            return -1;
        }

        private int GetIfStatementTrueBlockStartIndex()
        {
            int rowIndex = CurrentRowIndex.Value + 1;

            if(!tokens.ElementAt(rowIndex).IsPatternMatch(TokenTypes.BlockStart))
                Error("Hiányzik az 'igaz' blokk nyitó karaktere");

            rowIndex += 1;

            return rowIndex;
        }

        private void FunctionWaitMs(ResultValue resultValue)
        {
            if (resultValue.Number.IsNull())
                Error("A WaitMs funkciónak csak olyan kifejezés adható meg melynek eredménye egy szám lesz");

            int waitMsValue = (int)((double)resultValue.Number);

            if (waitMsValue == 0)
                return;

            if (waitMsValue < 0)
                waitMsValue = Math.Abs(waitMsValue);

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < waitMsValue) ;
        }

        private void NextLine() => CurrentRowIndex.ForceSet(CurrentRowIndex.Value + 1);

        private void Error(string message) => throw new Exception(string.Format("{0} -> {1}: {2}", message, CurrentRowIndex.Value, CodeLines.ElementAt(CurrentRowIndex.Value)));
    }
}
