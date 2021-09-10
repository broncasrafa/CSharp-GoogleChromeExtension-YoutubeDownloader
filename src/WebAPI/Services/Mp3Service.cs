using System;
using System.IO;
using System.Net;
using MediaToolkit;
using MediaToolkit.Model;

namespace WebAPI.Services
{
    public class Mp3Service
    {
        private static string _PathToSave = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}/outputConversions";

        public static string ConvertMp4VideoToMp3(string videoUrl, string mp3Filename)
        {
            byte[] videoBytes = { };
            string filename = Path.Combine(_PathToSave, mp3Filename + ".mp4");
            string outputMp3filename = $"{_PathToSave}/{mp3Filename}.mp3";

            using (var webClient = new WebClient())
            {
                videoBytes = webClient.DownloadData(videoUrl);
            }

            File.WriteAllBytes(filename, videoBytes);

            var inputFile = new MediaFile { Filename = filename };
            var outputFile = new MediaFile { Filename = outputMp3filename };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }

            byte[] mp3File = File.ReadAllBytes(outputMp3filename);
            File.Delete(filename);
            File.Delete(outputMp3filename);

            return Convert.ToBase64String(mp3File);
        }
    }
}
