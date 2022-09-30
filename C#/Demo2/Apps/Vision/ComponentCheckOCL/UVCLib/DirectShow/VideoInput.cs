namespace UVCLib.DirectShow
{
    public class VideoInput
    {
        //Properties
        public static VideoInput Default
        {
            get { return new VideoInput(-1, PhysicalConnectorType.Default); }
        }



        //Fields
        public readonly int _index;
        public readonly PhysicalConnectorType _type;



        //Constructor
        internal VideoInput( int index, PhysicalConnectorType type )
        {
            _index = index;
            _type = type;
        }
    }
}
