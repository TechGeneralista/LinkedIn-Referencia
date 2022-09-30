using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;


namespace ImageCaptureDevice.UniversalVideoClass.DirectShow
{
    public class VideoCapability : IImageCaptureDeviceCapability
    {
        static internal VideoCapability[] FromStreamConfig(IAMStreamConfig videoStreamConfig)
        {
            if (videoStreamConfig == null)
                throw new ArgumentNullException("videoStreamConfig");

            int count, size;

            int hr = videoStreamConfig.GetNumberOfCapabilities(out count, out size);

            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);

            if (count <= 0)
                throw new NotSupportedException("This video device does not report capabilities.");

            if (size > Marshal.SizeOf(typeof(VideoStreamConfigCaps)))
                throw new NotSupportedException("Unable to retrieve video device capabilities. This video device requires a larger VideoStreamConfigCaps structure.");

            Dictionary<uint, VideoCapability> videocapsList = new Dictionary<uint, VideoCapability>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    VideoCapability vc = new VideoCapability(videoStreamConfig, i);

                    uint key = (((uint)vc.FrameSize.Height) << 32) |
                               (((uint)vc.FrameSize.Width) << 16);

                    if (!videocapsList.ContainsKey(key))
                        videocapsList.Add(key, vc);
                    else
                    {
                        if (vc.BitCount > videocapsList[key].BitCount)
                            videocapsList[key] = vc;
                    }
                }

                catch { }
            }

            VideoCapability[] videocaps = new VideoCapability[videocapsList.Count];
            videocapsList.Values.CopyTo(videocaps, 0);

            return videocaps;
        }

        public int Index { get; }
        public Size FrameSize { get; }
        public int AverageFrameRate { get; }
        public int MaximumFrameRate { get; }
        public int BitCount { get; }
        public int PixelsCount { get; }
        public string FrameSizeString { get; }

        internal VideoCapability(IAMStreamConfig videoStreamConfig, int index)
        {
            Index = index;
            AMMediaType mediaType = null;
            VideoStreamConfigCaps caps = new VideoStreamConfigCaps();

            try
            {
                int hr = videoStreamConfig.GetStreamCaps(index, out mediaType, caps);

                if(hr != 0)
                    Marshal.ThrowExceptionForHR(hr);

                if(mediaType.FormatType == FormatType.VideoInfo)
                {
                    VideoInfoHeader videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

                    FrameSize = new Size(videoInfo.BmiHeader.Width, videoInfo.BmiHeader.Height);
                    FrameSizeString = $"{FrameSize.Width}x{FrameSize.Height}";
                    BitCount = videoInfo.BmiHeader.BitCount;
                    AverageFrameRate = (int)(10000000 / videoInfo.AverageTimePerFrame);
                    MaximumFrameRate = (int)(10000000 / caps.MinFrameInterval);
                    PixelsCount = FrameSize.Width * FrameSize.Height;
                }

                else if(mediaType.FormatType == FormatType.VideoInfo2)
                {
                    VideoInfoHeader2 videoInfo = (VideoInfoHeader2)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader2));

                    FrameSize = new Size(videoInfo.BmiHeader.Width, videoInfo.BmiHeader.Height);
                    FrameSizeString = $"{FrameSize.Width}x{FrameSize.Height}";
                    BitCount = videoInfo.BmiHeader.BitCount;
                    AverageFrameRate = (int)(10000000 / videoInfo.AverageTimePerFrame);
                    MaximumFrameRate = (int)(10000000 / caps.MinFrameInterval);
                    PixelsCount = FrameSize.Width * FrameSize.Height;
                }
                else
                    throw new ApplicationException("Unsupported format found.");
            }

            finally
            {
                if(mediaType != null)
                    mediaType.Dispose();
            }
        }

        public override string ToString()
            => $"{FrameSize.Width}x{FrameSize.Height} {AverageFrameRate}FPS";

        public override bool Equals(object obj)
        {
            return Equals(obj as VideoCapability);
        }

        public bool Equals(VideoCapability vc2)
        {
            if((object)vc2 == null)
                return false;

            return ((FrameSize == vc2.FrameSize) && (BitCount == vc2.BitCount));
        }

        public override int GetHashCode()
        {
            return FrameSize.GetHashCode() ^ BitCount;
        }

        public static bool operator ==(VideoCapability a, VideoCapability b)
        {
            if(object.ReferenceEquals(a, b))
                return true;

            if(((object)a == null) || ((object)b == null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(VideoCapability a, VideoCapability b)
        {
            return !(a == b);
        }
    }
}
