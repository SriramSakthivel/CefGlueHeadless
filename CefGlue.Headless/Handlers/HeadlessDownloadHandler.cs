using Xilium.CefGlue;

namespace CefGlue.Headless.Handlers;

public class HeadlessDownloadHandler : CefDownloadHandler
{
    protected override void OnDownloadUpdated(CefBrowser browser, CefDownloadItem downloadItem, CefDownloadItemCallback callback)
    {
        base.OnDownloadUpdated(browser, downloadItem, callback);
    }
}