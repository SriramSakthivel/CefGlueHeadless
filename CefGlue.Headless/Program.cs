using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CefGlue.Headless;
using CefGlue.Headless.Extensions;
using Xilium.CefGlue;

namespace CefGlue.Headless;
class Program
{
    private static bool isHostProcess;
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Iam host process, copying binaries..");
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string platformDirectory = CefRuntime.Platform == CefRuntimePlatform.Linux ? "cef_linux" : "cef_windows";
            string sourcePath = Path.Combine(currentDir, platformDirectory);
            CopyFilesRecursively(sourcePath, currentDir);
            Console.WriteLine("Iam host process, copying binaries completed..");
        }
        CefGlue.Headless.Main.MainInternal();
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        try
        {
            // Create all directories in the target path
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            // Copy all files and replace any existing files
            foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                string destinationFile = filePath.Replace(sourcePath, targetPath);
                //if (!File.Exists(destinationFile))
                {
                    File.Copy(filePath, destinationFile, true);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }



}








public static class Main
{
    public static void MainInternal()
    {
        Console.WriteLine("Setting up..");
        Console.WriteLine("****");
        Headless.Initialize();
        using (HeadlessBrowser browser = new HeadlessBrowser(500, 300, "https://www.valueresearchonline.com/", 10))
        {
            WaitForBrowserInitialization(browser).GetAwaiter().GetResult();
            Console.WriteLine();
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("***** Initialized!!");
            Console.WriteLine("******************************************************************************");
            //Console.ReadKey();

            browser.OnLoadingStateChanged += (cefBrowser, loading, back, forward) =>
            {
                if (!loading && cefBrowser.GetMainFrame().Url == "https://www.valueresearchonline.com/")
                {
                    Thread.Sleep(1000);
                    //browser.NavigateAsync(
                    //        new Uri("https://search.valueresearchonline.com/api/autocomplete-securities-stocks/?ver=8&sfl=false&lang=en&term=STYLAM"))
                    //    .ContinueWith(task =>
                    //    {
                    //        string uri = task.Result.Uri;
                    //        int statusCode = task.Result.HttpStatusCode;
                    //        string source = browser.GetSourceAsync().Result;
                    //    });
                    browser.GetAsync(
                            "https://search.valueresearchonline.com/api/autocomplete-securities-stocks/?ver=8&sfl=false&lang=en&term=STYLAM")
                        .ContinueWith(task =>
                        {
                            var r = task.Result;
                            Console.WriteLine(Encoding.UTF8.GetString(r.ResponseBytes));
                        });
                }
            };
            Console.WriteLine("Enter to exit");
            Console.Read();
        }

        Console.WriteLine("Destroying..");
        Headless.Destroy();
        Environment.Exit(0);
    }        /// <summary>
             /// Wait for a browser initialization
             /// </summary>
             /// <param name="browser">Browser instance</param>
             /// <returns></returns>
    public static async Task<int> WaitForBrowserInitialization(HeadlessBrowser browser)
    {
        await browser.WaitForBrowserToInitialize().ConfigureAwait(false);
        return 0;
    }

    //public static async Task<int> ResizeAndScreenshot(HeadlessBrowser browser, int w, int h)
    //{
    //    await browser.ResizeAsync(w, h).ConfigureAwait(false);
    //    Bitmap result = await browser.RequestFrameAsync().ConfigureAwait(false);
    //    result.Save("b_" + w.ToString() + "x" + h.ToString() + ".png");
    //    return 0;
    //}   
}