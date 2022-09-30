using Common.Tool;

namespace IOBoard
{
    public class IOBoardArgs
    {
        public string SerialNo { get; }
        public string ChannelName { get; }
        public int ChannelIndex { get; }
        public bool? LogicalValue { get; set; }


        public IOBoardArgs(string serialNo, string channelName, int channelIndex)
        {
            SerialNo = serialNo;
            ChannelName = channelName;
            ChannelIndex = channelIndex;
        }

        public IOBoardArgs(string serialNo, string channelName, int channelIndex, bool? logicalValue)
        {
            SerialNo = serialNo;
            ChannelName = channelName;
            ChannelIndex = channelIndex;
            LogicalValue = logicalValue;
        }

        public override string ToString() => SerialNo + '.' + ChannelName + '.' + ChannelIndex + '.' + (LogicalValue.IsNull() ? "null" : LogicalValue.ToString());
    }
}
