using System;


namespace Compute.CL.Internals
{
    public enum CLError
    {
        Success = 0,
        DeviceNotFound = -1,
        DeviceNotAvailable = -2,
        DeviceCompilerNotAvailable = -3,
        MemObjectAllocationFailure = -4,
        OutOfResources = -5,
        OutOfHostMemory = -6,
        ProfilingInfoNotAvailable = -7,
        MemCopyOverlap = -8,
        ImageFormatMismatch = -9,
        ImageFormatNotSupported = -10,
        BuildProgramFailure = -11,
        MapFailure = -12,

        /* 1.1 */
        MisalignedSubBufferOffset = -13,
        ExecStatusErrorForEventsInWaitList = -14,

        /* 1.2 */
        CompileProgramFailure = -15,
        LinkerNotAvailable = -16,
        LinkProgramFailure = -17,
        DevicePartitionFailed = -18,
        KernelArgInfoNotAvailable = -19,

        InvalidValue = -30,
        InvalidDeviceType = -31,
        InvalidPlatform = -32,
        InvalidDevice = -33,
        InvalidContext = -34,
        InvalidQueueProperties = -35,
        InvalidCommandQueue = -36,
        InvalidHostPtr = -37,
        InvalidMemObject = -38,
        InvalidImageFormatDescriptor = -39,
        InvalidImageSize = -40,
        InvalidSampler = -41,
        InvalidBinary = -42,
        InvalidBuildOptions = -43,
        InvalidProgram = -44,
        InvalidProgramExecutable = -45,
        InvalidKernelName = -46,
        InvalidKernelDefinition = -47,
        InvalidKernel = -48,
        InvalidArgIndex = -49,
        InvalidArgValue = -50,
        InvalidArgSize = -51,
        InvalidKernelArgs = -52,
        InvalidWorkDimension = -53,
        InvalidWorkGroupSize = -54,
        InvalidWorkItemSize = -55,
        InvalidGlobalOffset = -56,
        InvalidEventWaitList = -57,
        InvalidEvent = -58,
        InvalidOperation = -59,
        InvalidGLObject = -60,
        InvalidBufferSize = -61,
        InvalidMipLevel = -62,
        InvalidGlobalWorkSize = -63,

        /* 1.1 */
        InvalidProperty = -64,

        /* 1.2 */
        InvalidImageDescriptor = -65,
        InvalidCompilerOptions = -66,
        InvalidLinkerOptions = -67,
        InvalidDevicePartitionCount = -68,

        /* 2.0 */
        InvalidPipeSize = -69,
        InvalidDeviceQueue = -70,

        /* 2.2 */
        InvalidSpecID = -71,
        MaxSizeRestrictionExceeded = -72,
    }

    public enum CLPlatformInfo : uint
    {
        Profile = 0x0900,
        Version = 0x0901,
        Name = 0x0902,
        Vendor = 0x0903,
        Extensions = 0x0904,

        /* 2.1 */
        HostTimerResolution = 0x0905
    }

    [Flags]
    public enum CLDeviceType : ulong
    {
        Default = (1 << 0),
        CPU = (1 << 1),
        GPU = (1 << 2),
        Accelerator = (1 << 3),

        /* 1.2 */
        Custom = 1 << 4,
        All = 0xFFFFFFFF
    }

    public enum CLDeviceInfo : uint
    {
        Type = 0x1000,
        VendorID = 0x1001,
        MaxComputeUnits = 0x1002,
        MaxWorkItemDimensions = 0x1003,
        MaxWorkGroupSize = 0x1004,
        MaxWorkItemSizes = 0x1005,
        PreferredVectorWidthChar = 0x1006,
        PreferredVectorWidthShort = 0x1007,
        PreferredVectorWidthInt = 0x1008,
        PreferredVectorWidthLong = 0x1009,
        PreferredVectorWidthFloat = 0x100A,
        PreferredVectorWidthDouble = 0x100B,
        MaxClockFrequency = 0x100C,
        AddressBits = 0x100D,
        MaxReadImageArgs = 0x100E,
        MaxWriteImageArgs = 0x100F,
        MaxMemAllocSize = 0x1010,
        Image2DMaxWidth = 0x1011,
        Image2DMaxHeight = 0x1012,
        Image3DMaxWidth = 0x1013,
        Image3DMaxHeight = 0x1014,
        Image3DMaxDepth = 0x1015,
        ImageSupport = 0x1016,
        MaxParameterSize = 0x1017,
        MaxSamplers = 0x1018,
        MemBaseAddrAlign = 0x1019,
        MinDataTypeAlignSize = 0x101A,
        SingleFPConfig = 0x101B,
        GlobalMemCacheType = 0x101C,
        GlobalMemCacheLineSize = 0x101D,
        GlobalMemCacheSize = 0x101E,
        GlobalMemSize = 0x101F,
        MaxConstantBufferSize = 0x1020,
        MaxConstantArgs = 0x1021,
        LocalMemType = 0x1022,
        LocalMemSize = 0x1023,
        ErrorCorrectionSupport = 0x1024,
        ProfilingTimerResolution = 0x1025,
        EndianLittle = 0x1026,
        Available = 0x1027,
        CompilerAvailable = 0x1028,
        ExecutionCapabilities = 0x1029,
        QueueProperties = 0x102A,

        /* 2.0 */
        QueueOnHostProperties = 0x102A,

        Name = 0x102B,
        Vendor = 0x102C,
        DriverVersion = 0x102D,
        Profile = 0x102E,
        Version = 0x102F,
        Extensions = 0x1030,
        Platform = 0x1031,

        /* 1.2 */
        DoubleFPConfig = 0x1032,

        /* 1.1 */
        /* 0x1032 reserved for CL_DEVICE_DOUBLE_FP_CONFIG */
        /* 0x1033 reserved for CL_DEVICE_HALF_FP_CONFIG */
        PreferredVectorWidthHalf = 0x1034,
        HostUnifiedMemory = 0x1035,
        NativeVectorWidthChar = 0x1036,
        NativeVectorWidthShort = 0x1037,
        NativeVectorWidthInt = 0x1038,
        NativeVectorWidthLong = 0x1039,
        NativeVectorWidthFloat = 0x103A,
        NativeVectorWidthDouble = 0x103B,
        NativeVectorWidthHalf = 0x103C,
        OpenCLCVersion = 0x103D,

        /* 1.2 */
        LinkerAvailable = 0x103E,
        BuiltInKernels = 0x103F,
        ImageMaxBufferSize = 0x1040,
        ImageMaxArraySize = 0x1041,
        ParentDevice = 0x1042,
        PartitionMaxSubDevices = 0x1043,
        PartitionProperties = 0x1044,
        PartitionAffinityDomain = 0x1045,
        PartitionType = 0x1046,
        ReferenceCount = 0x1047,
        PreferredInteropUserSync = 0x1048,
        PrintfBufferSize = 0x1049,
        ImagePitchAlignment = 0x104A,
        ImageBaseAddressAlignment = 0x104B,

        /* 2.0 */
        MaxReadWriteImageArgs = 0x104C,
        MaxGlobalVaribleSize = 0x104D,
        QueueOnDeviceProperties = 0x104E,
        QueueOnDevicePreferredSize = 0x104F,
        QueueOnDeviceMaxSize = 0x1050,
        MaxOnDeviceQueues = 0x1051,
        MaxOnDeviceEvents = 0x1052,
        SVMCapabilities = 0x1053,
        GlobalVariablePreferredTotalSize = 0x1054,
        MaxPipeArgs = 0x1055,
        PipeMaxActiveReservations = 0x1056,
        PipeMaxPacketSize = 0x1057,
        PreferredPlatformAtomicAlignment = 0x1058,
        PreferredGlobalAtomicAlignment = 0x1059,
        PreferredLocalAtomicAlignment = 0x105A,

        /* 2.1 */
        ILVersion = 0x105B,
        MaxNumSubGroups = 0x105C,
        SubGroupIndependentForwardProgress = 0x105D
    }

    public enum CLBool : uint
    {
        False = 0,
        True = 1
    }

    [Flags]
    public enum CLDeviceFPConfig : ulong
    {
        Denorm = (1 << 0),
        InfNan = (1 << 1),
        RoundToNearest = (1 << 2),
        RoundToZero = (1 << 3),
        RoundToInf = (1 << 4),
        FMA = (1 << 5),

        /* 1.1 */
        SoftFloat = (1 << 6),

        /* 1.2 */
        CorrectlyRoundedDivideSqrt = (1 << 7)
    }

    public enum CLDeviceMemCacheType : uint
    {
        None = 0x0,
        ReadOnlyCache = 0x1,
        ReadWriteCache = 0x2
    }

    public enum CLDeviceLocalMemType : uint
    {
        Local = 0x1,
        Global = 0x2
    }

    [Flags]
    public enum CLDeviceExecCapabilities : ulong
    {
        Kernel = (1 << 0),
        NativeKernel = (1 << 1)
    }

    [Flags]
    public enum CLCommandQueueProperties : ulong
    {
        OutOfOrderExecModeEnable = (1 << 0),
        ProfilingEnable = (1 << 1),

        /* 2.0 */
        OnDevice = (1 << 2),
        OnDeviceDefault = (1 << 3)
    }

    public enum CLContextProperties : uint
    {
        Platform = 0x1084,

        /* 1.2 */
        InteropUserSync = 0x1085
    }

    public enum CLProgramBuildInfo : uint
    {
        Status = 0x1181,
        Options = 0x1182,
        Log = 0x1183,

        /* 1.2 */
        BinaryType = 0x1184,

        /* 2.0 */
        BuildGlobalVariableTotalSize = 0x1185
    }

    public enum CLBuildStatus
    {
        Success = 0,
        None = -1,
        Error = -2,
        InProgress = -3
    }

    [Flags]
    public enum CLMemFlags : ulong
    {
        ReadWrite = (1 << 0),
        WriteOnly = (1 << 1),
        ReadOnly = (1 << 2),
        UseHostPtr = (1 << 3),
        AllocHostPtr = (1 << 4),
        CopyHostPtr = (1 << 5),

        /* 1.2 */
        HostWriteOnly = (1 << 7),
        HostReadOnly = (1 << 8),
        HostNoAccess = (1 << 9),

        /* 2.0 */
        KernelReadAndWrite = (1 << 12)
    }

    public enum CLChannelOrder : int
    {
        R = 0x10B0,
        A = 0x10B1,
        RG = 0x10B2,
        RA = 0x10B3,
        RGB = 0x10B4,
        RGBA = 0x10B5,
        BGRA = 0x10B6,
        ARGB = 0x10B7,
        Intensity = 0x10B8,
        Luminance = 0x10B9,

        /* 1.1 */
        Rx = 0x10BA,
        RGx = 0x10BB,
        RGBx = 0x10BC,

        /* 1.2 */
        Depth = 0x10BD,
        DepthStencil = 0x10BE,

        /* 2.0 */
        sRGB = 0x10BF,
        sRGBx = 0x10C0,
        sRGBA = 0x10C1,
        sBGRA = 0x10C2,
        ABGR = 0x10C3
    }

    public enum CLChannelType : int
    {
        SNormInt8 = 0x10D0,
        SNormInt16 = 0x10D1,
        UNormInt8 = 0x10D2,
        UNormInt16 = 0x10D3,
        UNormShort565 = 0x10D4,
        UNormShort555 = 0x10D5,
        UNormInt101010 = 0x10D6,
        SignedInt8 = 0x10D7,
        SignedInt16 = 0x10D8,
        SignedInt32 = 0x10D9,
        UnSignedInt8 = 0x10DA,
        UnSignedInt16 = 0x10DB,
        UnSignedInt32 = 0x10DC,
        HalfFloat = 0x10DD,
        Float = 0x10DE,

        /* 1.2 */
        UnormInt24 = 0x10DF,

        /* 2.1 */
        UnormInt101010_2 = 0x10E0
    }

    public enum CLKernelWorkGroupInfo : uint
    {
        WorkGroupSize = 0x11B0,
        CompileWithWorkGroupSize = 0x11B1,
        LocalMemSize = 0x11B2,

        /* 1.1 */
        PreferredWorkGroupSizeMultiple = 0x11B3,
        PrivateMemSize = 0x11B4,

        /* 1.2 */
        GlobalWorkSize = 0x11B5
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
