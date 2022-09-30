using Common.Tool;
using IOBoard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserProgram.Internals
{
    public class ExpressionResolver
    {
        Varialables varialables;
        IOBoardClient ioBoardClient;


        public ExpressionResolver(Varialables varialables, IOBoardClient ioBoardClient)
        {
            this.varialables = varialables;
            this.ioBoardClient = ioBoardClient;
        }

        internal ResultValue Resolve(List<Token> expression)
        {
            while (expression.IsContains(TokenTypes.ParenthesesOpen, out _) && expression.IsContains(TokenTypes.ParenthesesClose, out _))
            {
                GetInternalParenthesesPair(expression, out int openIndex, out int closeIndex);

                int insertIndex = openIndex;
                List<Token> expressionBetweenParentheses = expression.Copy(openIndex, (closeIndex - openIndex) + 1);

                for (int i = 0; i < expressionBetweenParentheses.Count; i++)
                    expression.RemoveAt(insertIndex);

                expressionBetweenParentheses.Remove(expressionBetweenParentheses.First());
                expressionBetweenParentheses.Remove(expressionBetweenParentheses.Last());

                expressionBetweenParentheses = ResolveWithoutParentheses(expressionBetweenParentheses);
                expression.Insert(insertIndex, expressionBetweenParentheses[0]);
            }

            expression = ResolveWithoutParentheses(expression);

            if (expression.IsPatternMatch(TokenTypes.Number))
                return new ResultValue((double)expression[0].Value);

            else if (expression.IsPatternMatch(TokenTypes.LogicalConstant))
                return new ResultValue((bool)expression[0].Value);

            throw new Exception("Nem sikerült megoldani a kifejezést");
        }

        private void GetInternalParenthesesPair(List<Token> expression, out int openIndex, out int closeIndex)
        {
            openIndex = closeIndex = -1;

            foreach(Token token in expression)
            {
                if (token.Type == TokenTypes.ParenthesesOpen)
                    openIndex = expression.IndexOf(token);

                else if(token.Type == TokenTypes.ParenthesesClose && openIndex != -1)
                {
                    closeIndex = expression.IndexOf(token);
                    return;
                }
            }
        }

        private List<Token> ResolveWithoutParentheses(List<Token> expression)
        {
            expression = ResolveSymbols(expression);
            expression = ResolveMathOperations(expression);
            expression = ResolveComparsions(expression);
            expression = ResolveLogicalOperations(expression);
            return expression;
        }

        private List<Token> ResolveLogicalOperations(List<Token> expression)
        {
            while (expression.IsContains(TokenTypes.LogicalAnd, out int index))
            {
                expression = ResolveLogicalAnd(expression, index);
            }

            while (expression.IsContains(TokenTypes.LogicalOr, out int index))
            {
                expression = ResolveLogicalOr(expression, index);
            }

            return expression;
        }

        private List<Token> ResolveComparsions(List<Token> expression)
        {
            while (expression.IsContains(TokenTypes.IsEqual, out int index))
            {
                expression = ResolveIsEqual(expression, index);
            }

            while (expression.IsContains(TokenTypes.IsNotEqual, out int index))
            {
                expression = ResolveIsNotEqual(expression, index);
            }

            return expression;
        }

        private List<Token> ResolveMathOperations(List<Token> expression)
        {
            while (expression.IsContains(TokenTypes.Multiplication, out int index))
            {
                expression = ResolveMultiplication(expression, index);
            }

            while (expression.IsContains(TokenTypes.Division, out int index))
            {
                expression = ResolveDivision(expression, index);
            }

            while (expression.IsContains(TokenTypes.Addition, out int index))
            {
                expression = ResolveAddition(expression, index);
            }

            while (expression.IsContains(TokenTypes.Subtraction, out int index))
            {
                expression = ResolveSubtraction(expression, index);
            }

            return expression;
        }

        private List<Token> ResolveSymbols(List<Token> expression)
        {
            while (expression.IsContains(TokenTypes.Symbol, out int index))
            {
                expression = ResolveSymbol(expression, index);
            }

            while (expression.IsContains(TokenTypes.IOBoardArgs, out int index))
            {
                expression = ResolveIOBoardArgs(expression, index);
            }

            return expression;
        }

        private List<Token> ResolveIOBoardArgs(List<Token> expression, int index)
        {
            IOBoardArgs ioBoardArgs = (IOBoardArgs)expression[index].Value;
            ioBoardClient.Read(ioBoardArgs);

            expression.RemoveAt(index);

            if (ioBoardArgs.LogicalValue.IsNotNull())
                expression.Insert(index, new Token(TokenTypes.LogicalConstant, (bool)ioBoardArgs.LogicalValue));

            return expression;
        }

        private List<Token> ResolveMultiplication(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token number0 = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token number1 = expression[index + 1];

            if (number0.Type != TokenTypes.Number || operation.Type != TokenTypes.Multiplication || number1.Type != TokenTypes.Number)
                throw new Exception("Sikertelen szorzás");

            double result = (double)number0.Value * (double)number1.Value;
            expression.Remove(number0);
            expression.Remove(operation);
            expression.Remove(number1);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.Number, result));

            return expression;
        }

        private List<Token> ResolveDivision(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token number0 = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token number1 = expression[index + 1];

            if (number0.Type != TokenTypes.Number || operation.Type != TokenTypes.Division || number1.Type != TokenTypes.Number)
                throw new Exception("Sikertelen osztás");

            double result = (double)number0.Value / (double)number1.Value;
            expression.Remove(number0);
            expression.Remove(operation);
            expression.Remove(number1);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.Number, result));

            return expression;
        }

        private List<Token> ResolveAddition(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token number0 = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token number1 = expression[index + 1];

            if (number0.Type != TokenTypes.Number || operation.Type != TokenTypes.Addition || number1.Type != TokenTypes.Number)
                throw new Exception("Sikertelen összeadás");

            double result = (double)number0.Value + (double)number1.Value;
            expression.Remove(number0);
            expression.Remove(operation);
            expression.Remove(number1);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.Number, result));

            return expression;
        }

        private List<Token> ResolveSubtraction(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token number0 = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token number1 = expression[index + 1];

            if (number0.Type != TokenTypes.Number || operation.Type != TokenTypes.Subtraction || number1.Type != TokenTypes.Number)
                throw new Exception("Sikertelen kivonás");

            double result = (double)number0.Value - (double)number1.Value;
            expression.Remove(number0);
            expression.Remove(operation);
            expression.Remove(number1);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.Number, result));

            return expression;
        }

        private List<Token> ResolveIsEqual(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token leftToken = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token rightToken = expression[index + 1];

            bool result;

            if (leftToken.Type == TokenTypes.LogicalConstant && operation.Type == TokenTypes.IsEqual && rightToken.Type == TokenTypes.LogicalConstant)
            {
                result = (bool)leftToken.Value == (bool)rightToken.Value;
            }

            else if (leftToken.Type == TokenTypes.Number && operation.Type == TokenTypes.IsEqual && rightToken.Type == TokenTypes.Number)
            {
                result = (double)leftToken.Value == (double)rightToken.Value;
            }

            else
                throw new Exception("Sikertelen összehasonlítás");

            expression.Remove(leftToken);
            expression.Remove(operation);
            expression.Remove(rightToken);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.LogicalConstant, result));

            return expression;
        }

        private List<Token> ResolveIsNotEqual(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token logical0 = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token logical1 = expression[index + 1];

            if (logical0.Type != TokenTypes.LogicalConstant || operation.Type != TokenTypes.IsNotEqual || logical1.Type != TokenTypes.LogicalConstant)
                throw new Exception("Sikertelen összehasonlítás");

            bool result = (bool)logical0.Value != (bool)logical1.Value;
            expression.Remove(logical0);
            expression.Remove(operation);
            expression.Remove(logical1);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.LogicalConstant, result));

            return expression;
        }

        private List<Token> ResolveSymbol(List<Token> expression, int index)
        {
            Token symbolToken = expression[index];
            expression.RemoveAt(index);
            ResultValue resultValue = varialables.GetSymbol((string)symbolToken.Value);

            if(resultValue.Number.IsNotNull())
                expression.Insert(index, new Token(TokenTypes.Number, (double)resultValue.Number));

            else if (resultValue.Logical.IsNotNull())
                expression.Insert(index, new Token(TokenTypes.LogicalConstant, (bool)resultValue.Logical));

            return expression;
        }

        private List<Token> ResolveLogicalAnd(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token logical0 = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token logical1 = expression[index + 1];

            if (logical0.Type != TokenTypes.LogicalConstant || operation.Type != TokenTypes.LogicalAnd || logical1.Type != TokenTypes.LogicalConstant)
                throw new Exception("Sikertelen 'ÉS' kapcsolat");

            bool result = (bool)logical0.Value && (bool)logical1.Value;
            expression.Remove(logical0);
            expression.Remove(operation);
            expression.Remove(logical1);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.LogicalConstant, result));

            return expression;
        }

        private List<Token> ResolveLogicalOr(List<Token> expression, int index)
        {
            int resultInsertionIndex = index - 1;
            Token logical0 = expression[resultInsertionIndex];
            Token operation = expression[index];
            Token logical1 = expression[index + 1];

            if (logical0.Type != TokenTypes.LogicalConstant || operation.Type != TokenTypes.LogicalOr || logical1.Type != TokenTypes.LogicalConstant)
                throw new Exception("Sikertelen 'VAGY' kapcsolat");

            bool result = (bool)logical0.Value || (bool)logical1.Value;
            expression.Remove(logical0);
            expression.Remove(operation);
            expression.Remove(logical1);
            expression.Insert(resultInsertionIndex, new Token(TokenTypes.LogicalConstant, result));

            return expression;
        }
    }
}
