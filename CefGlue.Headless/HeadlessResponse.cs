namespace CefGlue.Headless;

public class HeadlessResponse
{
    public ulong Identifier { get; set; }
    public string Url { get; set; }
    public string StatusText { get; set; }
    public int StatusCode { get; set; }
    public string MimeType { get; set; }
    public byte[] ResponseBytes { get; set; }
    public int CefErrorCode { get; set; }
}