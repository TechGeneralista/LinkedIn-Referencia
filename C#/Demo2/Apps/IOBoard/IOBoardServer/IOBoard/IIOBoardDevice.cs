using System.Collections.Generic;


namespace IOBoardServer.IOBoard
{
    public enum IOBoardStates { Disconnected, Connecting, Connected }

    public interface IIOBoardDevice
    {
        string SerialNo { get; }

        void Connect();
        string SendAndReceive(string data);
        void Disconnect();
    }
}