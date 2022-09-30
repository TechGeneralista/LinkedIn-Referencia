using Common.Types;
using System;


namespace OpenCLWrapper.Internals
{
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
