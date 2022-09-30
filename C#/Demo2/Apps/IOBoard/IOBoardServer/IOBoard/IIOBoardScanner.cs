using System.Collections.Generic;


namespace IOBoardServer.IOBoard
{
    public interface IIOBoardScanner
    {
        IEnumerable<IIOBoardDevice> Devices { get; }

        void Scan();
    }
}