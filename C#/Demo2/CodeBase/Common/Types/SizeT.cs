using System;


namespace Common.Types
{
    public struct SizeT
    {
        public SizeT(int value) => this.value = new IntPtr(value);
        public SizeT(uint value) => this.value = new IntPtr((int)value);
        public SizeT(long value) => this.value = new IntPtr(value);
        public SizeT(ulong value) => this.value = new IntPtr((long)value);


        public static implicit operator int(SizeT t) => t.value.ToInt32();
        public static implicit operator uint(SizeT t) => (uint)t.value;
        public static implicit operator long(SizeT t) => t.value.ToInt64();
        public static implicit operator ulong(SizeT t) => (ulong)t.value;


        public static implicit operator SizeT(int value) => new SizeT(value);
        public static implicit operator SizeT(uint value) => new SizeT(value);
        public static implicit operator SizeT(long value) => new SizeT(value);
        public static implicit operator SizeT(ulong value) => new SizeT(value);


        public static bool operator !=(SizeT val1, SizeT val2) => val1.value != val2.value;
        public static bool operator ==(SizeT val1, SizeT val2) => val1.value == val2.value;


        public override bool Equals(object obj) => value.Equals(obj);
        public override string ToString() => value.ToString();
        public override int GetHashCode() => value.GetHashCode();


        private IntPtr value;
    }
}
