using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Updater
{
    static class Program
    {
        const string ApiPath = "https://api.github.com/repos/dioram/Elektronik-Tools-2.0/releases";

        static int Main(string[] args)
        {
            try
            {
                var assetURL = GetReleaseURL(args[0]);
                if (assetURL == null) return 1;
                
                DownloadRelease(assetURL);
                
                if (Directory.Exists(args[1])) Directory.Delete(args[1], true);
                ZipFile.ExtractToDirectory("Release.zip", 
                                           Path.Combine(args[1]));

                Process.Start(Path.Combine(args[1], "Elektronik.exe"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return 2;
            }

            Console.WriteLine("Update finished successfully");
            Console.ReadKey();
            return 0;
        }

        private static void DownloadRelease(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = "request";
            var response = request.GetResponse();
            using var file = File.OpenWrite("Release.zip");
            response.GetResponseStream().CopyTo(file);
        }

        private static string? GetReleaseURL(string version)
        {
            var request = WebRequest.CreateHttp(ApiPath);
            request.UserAgent = "request";
            var response = request.GetResponse();
            var serializer = new JsonSerializer();

            using var sr = new StreamReader(response.GetResponseStream());
            using var jsonTextReader = new JsonTextReader(sr);
            return (string?) serializer.Deserialize<JArray>(jsonTextReader)?
                    .FirstOrDefault(t => (string) t["tag_name"]! == version)?["assets"]?[0]?["browser_download_url"];
        }
    }
}