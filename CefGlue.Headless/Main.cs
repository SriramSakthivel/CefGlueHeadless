using System.Text;
using CefGlue.Headless.Extensions;
using IPC.Pipeline;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CefGlue.Headless;

public static class Main
{
    public static void MainInternal(bool isHostProcess)
    {
        Console.WriteLine("Setting up..");
        Console.WriteLine("****");
        Headless.Initialize();
        using (HeadlessBrowser browser = new HeadlessBrowser(500, 300, "www.google.com", 10))
        {
            WaitForBrowserInitialization(browser).GetAwaiter().GetResult();
            Console.WriteLine();
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("***** Initialized!!");
            Console.WriteLine("******************************************************************************");
            //Console.ReadKey();

            if (isHostProcess)
            {
                ListenForRequests(browser);
            }

            
            Console.WriteLine("Enter to exit");
            Console.Read();
        }

        Console.WriteLine("Destroying..");
        Headless.Destroy();
        Environment.Exit(0);
    }

    private static void ListenForRequests(HeadlessBrowser browser)
    {
        IPCPipeline pipeWrite = new IPCPipeline("FinancialAnalyzer.Headless.ResponseQueue", PipeAccess.Write);
        IPCPipeline pipeRead = new IPCPipeline("FinancialAnalyzer.Headless.RequestQueue", PipeAccess.Read);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Listening for requests in {pipeRead.ChannelName}");
        Console.ResetColor();
        pipeRead.OnMessageReceived += async (arg1, o) =>
        {
            JObject jobj = (JObject) o;
            var requestBytes = Convert.FromBase64String(jobj["Value"]?.Value<string>());
            string requestText = Encoding.UTF8.GetString(requestBytes);
            var request = JsonConvert.DeserializeObject<JObject>(requestText);
            try
            {
                string navigateUri = request["navigate"]?.Value<string>();
                string getUri = request["get"]?.Value<string>();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Request Received Channel={pipeRead.ChannelName}, Request={requestText}");
                Console.ResetColor();

                if (!string.IsNullOrEmpty(navigateUri) && browser.GetBrowser().GetMainFrame().Url != navigateUri)
                {
                    Console.WriteLine($"Navigating to {navigateUri}");
                    var nav = await browser.NavigateAsync(new Uri(navigateUri));
                }

                Console.WriteLine($"Sending request to to {getUri}");
                var response = await browser.GetAsync(getUri);
                var responseText = Encoding.UTF8.GetString(response.ResponseBytes);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Sending response back to Channel={pipeRead.ChannelName}, Response={responseText}");
                Console.ResetColor();

                await pipeWrite.SendMessageAsync(new PipelineStringData("Response", responseText));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        };
    }

    /// <summary>
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