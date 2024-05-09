using Xilium.CefGlue;

namespace CefGlue.Headless.Handlers;

public class HeadlessResourceRequestHandler : CefResourceRequestHandler
{
    public HeadlessResourceRequestHandler(HeadlessBrowser browser)
    {
        Browser = browser ?? throw new ArgumentNullException(nameof(browser));
    }

    public HeadlessBrowser Browser { get; }

    protected override CefResponseFilter? GetResourceResponseFilter(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response)
    {
        return new HeadlessResponseFilter(new HeadlessResponse()
        {
            Identifier = request.Identifier,
            MimeType = response.MimeType,
            Url = response.Url,
            StatusCode = response.Status,
            StatusText = response.StatusText,
            CefErrorCode = (int)response.Error
        }, new TaskCompletionSource<HeadlessResponse>());

        //return base.GetResourceResponseFilter(browser, frame, request, response);
    }

    protected override CefCookieAccessFilter GetCookieAccessFilter(CefBrowser browser, CefFrame frame, CefRequest request)
    {
        return null;
    }
}