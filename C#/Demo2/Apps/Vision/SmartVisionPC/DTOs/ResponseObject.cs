namespace SmartVisionClientApp.DTOs
{
    public class ResponseObject
    {
        public string Name { get; set; }
        public int DevIdx { get; set; }
        //public object SupportedProps { get; set; }
        public CamProperties[] CamProperties { get; set; }
    }
}
