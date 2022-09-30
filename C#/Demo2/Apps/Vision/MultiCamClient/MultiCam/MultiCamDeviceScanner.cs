using MultiCamApp.DTOs;
using MultiCamApp.NetworkCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MultiCamApp.MultiCam
{
    public class MultiCamDeviceScanner : ObservableProperty
    {
        public MultiCamDevice[] AvailableDevices
        {
            get => availableDevices;
            private set => SetField(value, ref availableDevices);
        }


        const int udpBroadcastPort = 12000;
        const int udpReceivePort = 12010;
        const string deviceType = "A1E15B1EF0994088B93F9BAAA0285139";
        readonly byte[] version = { 1, 0 };
        MultiCamDevice[] availableDevices = new MultiCamDevice[0];
        List<MultiCamDevice> deviceList = new List<MultiCamDevice>();
        UDPCom udpCom;
        object deviceListLock = new object();


        public MultiCamDeviceScanner()
        {
            udpCom = new UDPCom(udpReceivePort);
            udpCom.DataReceived += UDPCom_DataReceived;
        }

        internal Task ScanAsync() => Task.Run(() => Scan());

        internal void Scan()
        {
            AvailableDevices = null;

            lock (deviceListLock)
            {
                deviceList = new List<MultiCamDevice>();
            }

            CommandDTO commandDTO = new CommandDTO();
            commandDTO.Command = "GetDeviceInfo0".StringToDTOMember(32);
            byte[] data = commandDTO.ToBytes();
            udpCom.SendBroadcastData(data, udpBroadcastPort);
            Thread.Sleep(1000);

            lock (deviceListLock)
            {
                AvailableDevices = deviceList.ToArray();
            }

            /*CommandDTO commandDTO = DTOUtils.ToStructure<CommandDTO>(receivedBytes);

            if (commandDTO.Command.DTOMemberToString() == "SetIPAddress")
            {
                IPAddressDTO ipAddressDTO = DTOUtils.ToStructure<IPAddressDTO>(receivedBytes);

                if (deviceInfoDTO.Type.DTOMemberToString() == deviceType)
                {
                    lock (deviceListLock)
                    {
                        deviceList.Add(new MultiCamDevice(deviceInfoDTO));
                    }
                }
            }*/

            /*foreach (MultiCamDevice multiCamDevice in availableDevices)
                multiCamDevice.GetSnapshoot();*/
        }

        private void UDPCom_DataReceived(IPEndPoint ipEndPoint, byte[] data)
        {
            CommandDTO commandDTO = data.ToStructure<CommandDTO>();

            if(commandDTO.Command.DTOMemberToString() == "SetDeviceInfo0")
            {
                DeviceInfoDTO0 deviceInfoDTO0 = data.ToStructure<DeviceInfoDTO0>();

                if( deviceInfoDTO0.Type.DTOMemberToString() == deviceType &&
                    deviceInfoDTO0.Version.SequenceEqual(version))
                {
                    lock (deviceListLock)
                    {
                        deviceList.Add(new MultiCamDevice(deviceInfoDTO0));
                    }
                }
            }
        }
    }
}
