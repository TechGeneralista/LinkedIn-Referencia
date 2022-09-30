using SmartVisionClientApp.Common;
using SmartVisionClientApp.Communication;
using SmartVisionClientApp.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace SmartVisionClientApp.CameraSelect
{
    public class Camera
    {
        public string Name { get; }
        public int Id { get; }
        public List<CameraProperty> Properties { get; private set; } = new List<CameraProperty>();


        TCPClient client;


        public Camera(TCPClient client, ConnectCameraResponse serverResponse)
        {
            this.client = client;

            Name = serverResponse.Response.ResponseObject.Name;
            Id = serverResponse.Response.ResponseObject.DevIdx;

            foreach(CamProperties cp in serverResponse.Response.ResponseObject.CamProperties)
            {
                if(cp.Supported && cp.Property != "AUTOFOCUS" && cp.Property != "FPS")
                    Properties.Add(new CameraProperty(serverResponse.Response.ResponseObject.DevIdx, client, cp));
            }
        }

        internal bool Disconnect()
        {
            QueryDTO queryDTO = new QueryDTO()
            {
                Query = "CAMERA_CLOSECAM",
                Params = new CameraIndexParams() { CameraIndex = Id }
            };

            QueryInfoResponse serverResponse = client.SendAndReceive<QueryDTO, QueryInfoResponse>(queryDTO);

            if (serverResponse.IsNotNull())
                return true;

            return false;
        }

        public BitmapSource Capture()
        {
            QueryDTO queryDTO = new QueryDTO()
            {
                Query = "CAMERA_CAMGRAB",
                Params = new CamGrabParams()
                {
                    CameraIndex = ObjectContainer.Get<Camera>().Id,
                    JPEGQuality = 75
                }
            };

            client.SendAndReceive<QueryDTO, CameraFrameResponse>(queryDTO);
            CameraFrameResponse serverResponse = client.SendAndReceive<QueryDTO, CameraFrameResponse>(queryDTO);

            byte[] jpgData = Convert.FromBase64String(serverResponse.Response.ResponseObject.Base64Content);
            BitmapSource img;

            using (Stream stream = new MemoryStream(jpgData))
            {
                JpegBitmapDecoder jpegBitmapDecoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                img = jpegBitmapDecoder.Frames[0];
            }

            return img;
        }
    }
}