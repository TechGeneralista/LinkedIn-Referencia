namespace MultiCamApp.Main
{
    public class WindowTitle
    {
        public string Title { get; }


        public const string ApplicationName = "MultiCam";
        public readonly byte[] Version = { 1, 0 };


        public WindowTitle()
        {
            Title = ApplicationName + " V" + Version[0] + "." + Version[1];
        }
    }
}
