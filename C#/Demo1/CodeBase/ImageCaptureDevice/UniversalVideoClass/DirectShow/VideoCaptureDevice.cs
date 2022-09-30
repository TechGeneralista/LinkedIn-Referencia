using System;
using System.Runtime.InteropServices;
using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;


namespace ImageCaptureDevice.UniversalVideoClass.DirectShow
{
    public class VideoCaptureDevice
    {
        public event EventHandler<NewImageAvailableArgs> NewImageAvailable;

        public bool IsRun { get; private set; }

        public VideoCapability[] VideoCapabilities
        {
            get
            {
                if (videoCapabilities == null && !IsRun)
                        StartCapture(false);

                return (videoCapabilities != null) ? videoCapabilities : new VideoCapability[0];
            }
        }
        VideoCapability[] videoCapabilities;


        readonly string deviceMoniker;
        readonly Grabber grabber;
        readonly object sync = new object();
        object captureGraphObject, graphObject, sourceObject, videoGrabberObject, crossbarObject;
        IMediaControl mediaControl;


        public VideoCaptureDevice(string deviceMoniker)
        {
            this.deviceMoniker = deviceMoniker;
            grabber = new Grabber();
            grabber.NewImageAvailable += (s, e) => NewImageAvailable?.Invoke(this, e);
        }

        public void Start(IImageCaptureDeviceCapability videoCapability)
        {
            if (videoCapability == null)
                throw new ArgumentNullException(nameof(videoCapability));

            StartCapture(true, videoCapability);
        }

        private void StartCapture(bool runGraph, IImageCaptureDeviceCapability videoCapability = null)
        {
            try
            {
                Type type = Type.GetTypeFromCLSID(Clsid.CaptureGraphBuilder2);
                
                if(type == null)
                    throw new ApplicationException("Failed creating capture graph builder");

                captureGraphObject = Activator.CreateInstance(type);
                ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)captureGraphObject;

                type = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                
                if(type == null)
                    throw new ApplicationException("Failed creating filter graph");

                graphObject = Activator.CreateInstance(type);
                IFilterGraph2 graph = (IFilterGraph2)graphObject;

                captureGraph.SetFiltergraph((IGraphBuilder)graph);

                sourceObject = FilterInfo.CreateFilter(deviceMoniker);
                
                if(sourceObject == null)
                    throw new ApplicationException("Failed creating device object for moniker");

                IBaseFilter sourceBase = (IBaseFilter)sourceObject;

                type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);

                if(type == null)
                    throw new ApplicationException("Failed creating sample grabber");

                videoGrabberObject = Activator.CreateInstance(type);
                ISampleGrabber videoSampleGrabber = (ISampleGrabber)videoGrabberObject;
                IBaseFilter videoGrabberBase = (IBaseFilter)videoGrabberObject;

                graph.AddFilter(sourceBase, "source");
                graph.AddFilter(videoGrabberBase, "grabber_video");

                AMMediaType mediaType = new AMMediaType
                {
                    MajorType = MediaType.Video,
                    SubType = MediaSubType.RGB24
                };

                videoSampleGrabber.SetMediaType(mediaType);
                captureGraph.FindInterface(FindDirection.UpstreamOnly, Guid.Empty, sourceBase, typeof(IAMCrossbar).GUID, out crossbarObject);

                videoSampleGrabber.SetCallback(grabber, 1);

                GetPinCapabilitiesAndConfigureSizeAndRate(captureGraph, sourceBase, PinCategory.Capture, videoCapability, ref videoCapabilities);

                if (runGraph)
                {
                    captureGraph.RenderStream(PinCategory.Capture, MediaType.Video, sourceBase, null, videoGrabberBase);

                    if (videoSampleGrabber.GetConnectedMediaType(mediaType) == 0)
                    {
                        VideoInfoHeader vih = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));
                        grabber.SetFrameSize(vih.BmiHeader);
                        mediaType.Dispose();
                    }

                    mediaControl = (IMediaControl)graphObject;
                    mediaControl.Run();

                    IsRun = true;
                }
                else
                    ReleaseComObjects();
            }

            catch 
            { 
                Stop();
            }
        }

        public void Stop()
        {
            IsRun = false;
            
            if(mediaControl != null)
            {
                mediaControl.Stop();
                mediaControl = null;
            }

            ReleaseComObjects();
        }

        private void ReleaseComObjects()
        {
            if (graphObject != null)
            {
                Marshal.ReleaseComObject(graphObject);
                graphObject = null;
            }

            if (sourceObject != null)
            {
                Marshal.ReleaseComObject(sourceObject);
                sourceObject = null;
            }

            if (videoGrabberObject != null)
            {
                Marshal.ReleaseComObject(videoGrabberObject);
                videoGrabberObject = null;
            }

            if (captureGraphObject != null)
            {
                Marshal.ReleaseComObject(captureGraphObject);
                captureGraphObject = null;
            }

            if (crossbarObject != null)
            {
                Marshal.ReleaseComObject(crossbarObject);
                crossbarObject = null;
            }
        }

        private void SetResolution(IAMStreamConfig streamConfig, IImageCaptureDeviceCapability resolution)
        {
            if(resolution == null)
                return;

            int capabilitiesCount = 0, capabilitySize = 0;
            AMMediaType newMediaType = null;
            VideoStreamConfigCaps caps = new VideoStreamConfigCaps();

            streamConfig.GetNumberOfCapabilities(out capabilitiesCount, out capabilitySize);

            for(int i = 0; i < capabilitiesCount; i++)
            {
                try
                {
                    VideoCapability vc = new VideoCapability(streamConfig, i);

                    if((VideoCapability)resolution == vc)
                    {
                        if(streamConfig.GetStreamCaps(i, out newMediaType, caps) == 0)
                        {
                            break;
                        }
                    }
                }

                catch { }
            }

            if(newMediaType != null)
            {
                streamConfig.SetFormat(newMediaType);
                newMediaType.Dispose();
            }
        }



        private void GetPinCapabilitiesAndConfigureSizeAndRate(ICaptureGraphBuilder2 graphBuilder, IBaseFilter baseFilter,
            Guid pinCategory, IImageCaptureDeviceCapability resolutionToSet, ref VideoCapability[] capabilities)
        {
            object streamConfigObject;
            graphBuilder.FindInterface(pinCategory, MediaType.Video, baseFilter, typeof(IAMStreamConfig).GUID, out streamConfigObject);

            if(streamConfigObject != null)
            {
                IAMStreamConfig streamConfig = null;

                try
                {
                    streamConfig = (IAMStreamConfig)streamConfigObject;
                }

                catch { }

                if(streamConfig != null)
                {
                    if(capabilities == null)
                    {
                        try
                        {
                            capabilities = VideoCapability.FromStreamConfig(streamConfig);
                        }

                        catch { }
                    }

                    if(resolutionToSet != null)
                    {
                        SetResolution(streamConfig, resolutionToSet);
                    }
                }

                Marshal.ReleaseComObject(streamConfigObject);
            }

            if(capabilities == null)
                capabilities = new VideoCapability[0];
        }

        public bool SetCameraProperty(CameraControlProperties property, int value, ControlFlags controlFlags)
        {
            bool ret = true;

            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
                throw new ArgumentException("Video source is not specified.");

            lock (sync)
            {
                object tempSourceObject = null;

                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }

                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMCameraControl))
                    throw new NotSupportedException("The video source does not support camera control.");

                IAMCameraControl pCamControl = (IAMCameraControl)tempSourceObject;
                int hr = pCamControl.Set(property, value, controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        public bool GetCameraProperty(CameraControlProperties property, out int value, out ControlFlags controlFlags)
        {
            bool ret = true;

            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
                throw new ArgumentException("Video source is not specified.");

            lock (sync)
            {
                object tempSourceObject = null;

                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }

                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMCameraControl))
                    throw new NotSupportedException("The video source does not support camera control.");

                IAMCameraControl pCamControl = (IAMCameraControl)tempSourceObject;
                int hr = pCamControl.Get(property, out value, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        public bool GetCameraPropertyRange(CameraControlProperties property, out int minValue, out int maxValue, out int stepSize, out int defaultValue, out ControlFlags controlFlags)
        {
            bool ret = true;

            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
                throw new ArgumentException("Video source is not specified.");

            lock (sync)
            {
                object tempSourceObject = null;

                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }

                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMCameraControl))
                    throw new NotSupportedException("The video source does not support camera control.");

                IAMCameraControl pCamControl = (IAMCameraControl)tempSourceObject;
                int hr = pCamControl.GetRange(property, out minValue, out maxValue, out stepSize, out defaultValue, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        public bool SetVideoProperty(VideoProcAmpProperties property, int value, ControlFlags controlFlags)
        {
            bool ret = true;

            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
            {
                throw new ArgumentException("Video source is not specified.");
            }

            lock (sync)
            {
                object tempSourceObject = null;

                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }

                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                    throw new NotSupportedException("The video source does not support camera control.");

                IAMVideoProcAmp pCamControl = (IAMVideoProcAmp)tempSourceObject;
                int hr = pCamControl.Set(property, value, controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        public bool GetVideoProperty(VideoProcAmpProperties property, out int value, out ControlFlags controlFlags)
        {
            bool ret = true;

            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
            {
                throw new ArgumentException("Video source is not specified.");
            }

            lock (sync)
            {
                object tempSourceObject = null;

                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                    throw new NotSupportedException("The video source does not support camera control.");

                IAMVideoProcAmp pCamControl = (IAMVideoProcAmp)tempSourceObject;
                int hr = pCamControl.Get(property, out value, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }

        public bool GetVideoPropertyRange(VideoProcAmpProperties property, out int minValue, out int maxValue, out int stepSize, out int defaultValue, out ControlFlags controlFlags)
        {
            bool ret = true;

            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
            {
                throw new ArgumentException("Video source is not specified.");
            }

            lock (sync)
            {
                object tempSourceObject = null;

                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }

                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                    throw new NotSupportedException("The video source does not support camera control.");

                IAMVideoProcAmp pCamControl = (IAMVideoProcAmp)tempSourceObject;
                int hr = pCamControl.GetRange(property, out minValue, out maxValue, out stepSize, out defaultValue, out controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }
    }
}
