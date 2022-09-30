using Common.Tool;


namespace UserProgram.Internals
{
    public class ResultValue
    {
        public bool? Logical { get; }
        public double? Number { get; }
        public bool IsValid => Logical.IsNotNull() || Number.IsNotNull();


        public ResultValue() { }
        public ResultValue(bool logical) => Logical = logical;
        public ResultValue(double number) => Number = number;
    }
}
