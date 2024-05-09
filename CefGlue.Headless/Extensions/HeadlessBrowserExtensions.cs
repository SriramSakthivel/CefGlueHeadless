using Xilium.CefGlue;

namespace CefGlue.Headless.Extensions;

public static class HeadlessBrowserExtensions
{
    public static Task<string> GetSourceAsync(this HeadlessBrowser browser, CefFrame? frame = null)
    {
        if (browser == null) throw new ArgumentNullException(nameof(browser));

        frame ??= browser.GetBrowser().GetMainFrame();

        var visitor = new StringVisitor();
        frame.GetSource(visitor);
        return visitor.GetValueAsync();
    }

    public static Task<HeadlessResponse> GetAsync(this HeadlessBrowser browser, string url, CefFrame? frame = null)
    {
        if (url == null) throw new ArgumentNullException(nameof(url));
        frame ??= browser.GetBrowser().GetMainFrame();

        CefRequest request = CefRequest.Create();
        request.Url = url;
        request.Method = "GET";
        var requestClient = new HeaderRequestClient(request);
        CefUrlRequest cefUrlRequest = frame.CreateUrlRequest(request, requestClient);
        return requestClient.ResponseTask;
    }
}