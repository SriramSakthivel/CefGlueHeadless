﻿using CefGlue.Headless.Handlers;
using Xilium.CefGlue;

namespace CefGlue.Headless;

public class HeadlessCefClient : CefClient
{
    private readonly HeadlessLifeSpanHandler lifeSpanHandler;
    private readonly HeadlessRenderHandler renderHandler;
    private readonly HeadlessLoadHandler loadHandler;
    private readonly HeadlessDisplayHandler displayHandler;
    private readonly HeadlessDownloadHandler downloadHandler;
    private readonly HeadlessRequestHandler requestHandler;

    /// <summary>
    /// Fires when the loading state changes
    /// </summary>
    public event BrowserLoadingStateChangeDelegate LoadingStateChanged;

    /// <summary>
    /// Fires when the underlying browser size has changed
    /// </summary>
    public event BrowserSizeChangedDelegate OnSizeChanged;

    /// <summary>
    /// Fires when a frame has been received
    /// </summary>
    public event BrowserFrameReceivedDelegate OnFrameReceived;

    /// <summary>
    /// Fires when a console message is received
    /// </summary>
    public event BrowserConsoleMessageReceivedDelegate OnConsoleMessageReceived;

    /// <summary>
    /// Loading ended
    /// </summary>
    public event BrowserLoadEndedDelegate LoadingEnded;

    /// <summary>
    /// Set up a new headless CefCLient
    /// </summary>
    /// <param name="offscreenBrowser">The browser instance</param>
    public HeadlessCefClient(HeadlessBrowser offscreenBrowser)
    {
        downloadHandler = new HeadlessDownloadHandler();
        requestHandler = new HeadlessRequestHandler(offscreenBrowser);
        lifeSpanHandler = new HeadlessLifeSpanHandler(offscreenBrowser);
        renderHandler = new HeadlessRenderHandler(offscreenBrowser);
        loadHandler = new HeadlessLoadHandler(offscreenBrowser);
        loadHandler.LoadingEnded += (browser, httpStatus) => LoadingEnded?.Invoke(browser, httpStatus);
        displayHandler = new HeadlessDisplayHandler(offscreenBrowser);
        displayHandler.OnConsoleMessageReceived += (browser, message, source, line) => OnConsoleMessageReceived?.Invoke(browser, message, source, line);
        renderHandler.OnFrameReceived += (bmImg) => OnFrameReceived?.Invoke(bmImg);
        renderHandler.OnSizeChanged += (browser, width, height) => OnSizeChanged?.Invoke(browser, width, height);
        loadHandler.LoadingStateChanged += (browser, loading, back, forward) => LoadingStateChanged?.Invoke(browser, loading, back, forward);
    }

    /// <summary>
    /// Request a frame
    /// </summary>
    public void RequestFrame()
    {
        renderHandler.RequestFrame();
    }

    protected override CefLifeSpanHandler GetLifeSpanHandler()
    {
        return lifeSpanHandler;
    }

    protected override CefRenderHandler GetRenderHandler()
    {
        return renderHandler;
    }

    protected override CefLoadHandler GetLoadHandler()
    {
        return loadHandler;
    }

    protected override CefDisplayHandler GetDisplayHandler()
    {
        return displayHandler;
    }
    protected override CefDownloadHandler? GetDownloadHandler()
    {
        return downloadHandler;
    }

    protected override CefRequestHandler? GetRequestHandler()
    {
        return requestHandler;
    }
}