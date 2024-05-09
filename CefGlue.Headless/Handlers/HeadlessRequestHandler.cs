using Xilium.CefGlue;

namespace CefGlue.Headless.Handlers;

public class HeadlessRequestHandler : CefRequestHandler
{
    public HeadlessRequestHandler(HeadlessBrowser browser)
    {
        Browser = browser ?? throw new ArgumentNullException(nameof(browser));
    }

    public HeadlessBrowser Browser { get; }

    protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request,
        bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
    {
        if (Browser.ShouldTrackRequest(request))
        {
            return new HeadlessResourceRequestHandler(Browser);
        }

        return null;
    }
}