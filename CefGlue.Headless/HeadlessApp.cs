using Xilium.CefGlue;

namespace CefGlue.Headless;

public class HeadlessApp : CefApp
{
    protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
    {
    }
}