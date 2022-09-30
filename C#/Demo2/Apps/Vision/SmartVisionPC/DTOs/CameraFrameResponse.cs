namespace SmartVisionClientApp.DTOs
{
    public class CameraFrameResponse : IHasQueryInfo
    {
        public Response2 Response { get; set; }
        public QueryInfo QueryInfo { get; set; }
    }
}
