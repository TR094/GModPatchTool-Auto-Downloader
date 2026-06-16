using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Security.Principal;

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
                Console.WriteLine("Error. Failed to download the file from the URL");
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
                Console.WriteLine("Error. Failed to extract the ZIP");
            }
        }

        static void Main(string[] args)
        {
            if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This program must NOT be run as an administrator. Please restart it without administrator privileges. \n" +
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

            Console.WriteLine("Downloading zip file...");
            down.DownloadURL();

            Console.WriteLine("Extracting zip file...");
            down.ExtractZIP();

            Console.WriteLine("Executing EXE...");
            var process = Process.Start(executablePath);
            process.WaitForExit();

            Console.WriteLine("Deleting temporary folder...");
            Directory.Delete(tempFolder, true);
            Console.WriteLine("Done!");
            Thread.Sleep(1000);
            System.Environment.Exit(0);

        }
    }
}
