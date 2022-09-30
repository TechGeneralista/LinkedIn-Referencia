using System;


namespace OpenCLWrapper.Internals
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

    public struct CLImageFormat
    {
        readonly public CLChannelOrder ChannelOrder;
        readonly public CLChannelType ChannelType;

        public CLImageFormat(CLChannelOrder channelOrder, CLChannelType channelType)
        {
            ChannelOrder = channelOrder;
            ChannelType = channelType;
        }
    }

    public struct CLImageDesc
    {
        readonly public CLMemObjectType Type;
        readonly public SizeT Width;
        readonly public SizeT Height;
        readonly public SizeT Depth;
        readonly public SizeT ArraySize;
        readonly public SizeT RowPitch;
        readonly public SizeT SlicePitch;
        readonly public uint NumMipLevels;
        readonly public uint NumSamples;
        readonly public IntPtr MemObject;

        public CLImageDesc(CLMemObjectType type, SizeT width, SizeT height, SizeT depth, SizeT arraySize, SizeT rowPitch, SizeT slicePitch, uint numMipLevels, uint numSamples, IntPtr memObject)
        {
            Type = type;
            Width = width;
            Height = height;
            Depth = depth;
            ArraySize = arraySize;
            RowPitch = rowPitch;
            SlicePitch = slicePitch;
            NumMipLevels = numMipLevels;
            NumSamples = numSamples;
            MemObject = memObject;
        }

        public CLImageDesc(CLMemObjectType type, SizeT width, SizeT height)
        {
            Type = type;
            Width = width;
            Height = height;
            Depth = 1;
            ArraySize = 0;
            RowPitch = 0;
            SlicePitch = 0;
            NumMipLevels = 0;
            NumSamples = 0;
            MemObject = IntPtr.Zero;
        }
    }

    public enum CLMemObjectType : uint
    {
        Buffer = 0x10F0,
        Image2D = 0x10F1,
        Image3D = 0x10F2,

        /* 1.2 */
        Image2DArray = 0x10F3,
        Image1D = 0x10F4,
        Image1DArray = 0x10F5,
        Image1DBuffer = 0x10F6,

        /* 2.0 */
        Pipe = 0x10F7
    }
}
