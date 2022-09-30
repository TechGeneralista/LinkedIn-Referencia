using Common.Tool;
using IOBoard;
using System.Text;


namespace UserProgram.Internals
{
    public enum TokenTypes 
    { 
        Label,
        EmptyLine,
        BlockStart,
        BlockEnd,
        Symbol,
        ValueAssignment,
        IOBoardArgs,
        Number,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        ParenthesesOpen,
        ParenthesesClose,
        IsEqual,
        IsNotEqual,
        LogicalAnd,
        LogicalOr,
        LogicalConstant,
        WaitMsFunction,
        JumpFunction,
        IfStatement,
        IfStatementElse
    }


    public class Token
    {
        public TokenTypes Type { get; }
        public object Value { get; }


        public Token(TokenTypes type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            if (Value.IsNull())
                return Type.ToString();

            if (Value.GetType() != typeof(IOBoardArgs))
                return string.Format("{0}: {1}", Type.ToString(), Value.ToString());

            return string.Format("{0}: {1}", Type.ToString(), ((IOBoardArgs)Value).ToString());
        }
    }
}
