namespace UserProgram.Internals
{
    public class SubString
    {
        public int StartIndex { get; }
        public int Length { get; }
        public string Str { get; }

        public SubString(int startIndex, int length, string str)
        {
            StartIndex = startIndex;
            Length = length;
            Str = str;
        }
    }
}