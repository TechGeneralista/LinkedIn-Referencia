using Common;
using Common.Interface;
using Communication.TCP.Server.MultiClient;
using System;
using System.Collections.Generic;
using System.Net.Sockets;


namespace UniCamApp.Remote
{
    public class CommunicationParser
    {
        readonly TCPServerMultiClientDC tcpServerMultiClientDC;
        readonly ICanRemote remotable;


        public CommunicationParser(TCPServerMultiClientDC tcpServerMultiClientDC, ICanRemote remotable)
        {
            this.tcpServerMultiClientDC = tcpServerMultiClientDC;
            this.remotable = remotable;

            tcpServerMultiClientDC.DataReceived += TcpServer_DataReceived;
        }

        private void TcpServer_DataReceived(Socket clientSocket, string data)
        {
            string[] splitted = data.Split('/');
            Response response = Response.NotSet;

            if (splitted.Length > 1)
            {
                if(Enum.TryParse(splitted[0],out Command command))
                {
                    List<string> idsList = new List<string>(splitted);
                    idsList.RemoveAt(0);
                    remotable?.Remote(ref response, command, idsList.ToArray());
                }
            }

            if (response == Response.NotSet)
                response = Response.Error;

            tcpServerMultiClientDC.Send(clientSocket, response.ToString());
        }
    }
}
