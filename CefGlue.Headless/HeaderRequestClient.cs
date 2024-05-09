using Xilium.CefGlue;

namespace CefGlue.Headless;

public class HeaderRequestClient : CefUrlRequestClient
{
    private readonly TaskCompletionSource<HeadlessResponse> tcs = new TaskCompletionSource<HeadlessResponse>();
    private MemoryStream memoryStream;
    private readonly HeadlessResponse headlessResponse = new HeadlessResponse();
    public HeaderRequestClient(CefRequest request)
    {
        Request = request;
        headlessResponse.Url = request.Url;
        headlessResponse.Identifier = request.Identifier;
    }

    public CefRequest Request { get; }

    public Task<HeadlessResponse> ResponseTask => tcs.Task;

    protected override void OnDownloadData(CefUrlRequest request, Stream data)
    {
        // Handle downloaded data (e.g., read the response body)
        memoryStream = new MemoryStream();
        data.CopyTo(memoryStream);
        headlessResponse.ResponseBytes = memoryStream.ToArray();
        //string encoding = Encoding.UTF8.GetString(ms.ToArray());
    }

    protected override void OnDownloadProgress(CefUrlRequest request, long current, long total)
    {
        // Handle download progress (if needed)
    }

    protected override void OnRequestComplete(CefUrlRequest request)
    {
        var response = request.GetResponse();

        headlessResponse.StatusCode = response.Status;
        headlessResponse.StatusText = response.StatusText;
        headlessResponse.CefErrorCode = (int)response.Error;
        headlessResponse.MimeType = response.MimeType;

        tcs.TrySetResult(headlessResponse);
    }

    protected override void OnUploadProgress(CefUrlRequest request, long current, long total)
    {
        // Handle upload progress (if needed)
    }
}