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
        CefGlue.Headless.Main.MainInternal(isHostProcess);

       
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
                if (!File.Exists(destinationFile))
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