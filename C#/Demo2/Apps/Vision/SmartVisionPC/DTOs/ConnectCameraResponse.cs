namespace SmartVisionClientApp.DTOs
{
    public class ConnectCameraResponse : IHasQueryInfo
    {
        public Response1 Response { get; set; }
        public QueryInfo QueryInfo { get; set; }
    }
}
