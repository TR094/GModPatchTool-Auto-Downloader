using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Principal;
using System.Threading;

namespace GModPatchToolAutoDownloader
{
    internal class Program
    {
        public static string websitePath = "https://github.com/solsticegamestudios/GModPatchTool/releases/latest/download/GModPatchTool-Windows.zip";
        public static string tempFolder = Path.Combine(Path.GetTempPath(), "GModPatchToolAutoDownloader");
        public static string zipPath = Path.Combine(tempFolder, "tempGPTAD.zip");
        public static string executablePath = Path.Combine(tempFolder, "gmodpatchtool.exe");

        public void DownloadURL()
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(websitePath, zipPath);
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Failed to download the file from the URL.");
                Console.ResetColor();
            }
        }

        public void ExtractZIP()
        {
            try
            {
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Name.EndsWith(".exe"))
                        {
                            entry.ExtractToFile(Path.Combine(tempFolder, entry.Name));
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Failed to extract the ZIP file.");
                Console.ResetColor();
            }
        }

        static void Main(string[] args)
        {
            if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: This program must NOT be run as an administrator. Please restart it without administrator privileges. \n" +
                    "If possible, launch it through your Downloads folder, rather than straight out of your browser.");
                Console.ReadLine();
                System.Environment.Exit(0);
            }
            else
            {
                Console.ResetColor();
            }

            var down = new Program();
            Directory.CreateDirectory(tempFolder);

            Console.WriteLine("INFO: Downloading zip file...");
            down.DownloadURL();

            Console.WriteLine("INFO: Extracting zip file...");
            down.ExtractZIP();

            Console.WriteLine("INFO: Executing patcher...");
            var process = Process.Start(executablePath);
            process.WaitForExit();

            Console.WriteLine("INFO: Deleting temporary files...");
            Directory.Delete(tempFolder, true);
            Console.WriteLine("INFO: Patch complete. Closing...");
            Thread.Sleep(1000);
            System.Environment.Exit(0);

        }
    }
}
