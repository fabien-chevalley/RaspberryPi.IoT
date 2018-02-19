using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaspberryPi.IoT
{
    public class SimulationCamera : ICamera
    {
        public const string Folder = "/home/pi/share/camera";
        public const string Image = "image";
        public const string Video = "video";

        private readonly ILogger<RaspberryPi> _logger;

        public SimulationCamera(ILogger<RaspberryPi> logger)
        {
            _logger = logger;    
        }
        
        public string LastImageFilename => Path.Combine(Image, GetLastFile(Path.Combine(Folder, Image)).Name);

        public string LastVideoFilename => Path.Combine(Image, GetLastFile(Path.Combine(Folder, Video)).Name);

        public string SharedFolder => Folder;
        
        public void GrabImage()
        {
        }  

        public void GrabVideo(TimeSpan duration)
        {
        }  

        public void StartStreaming(string url, string streamName, TimeSpan duration)
        {
        }

        private FileInfo GetLastFile(string folder)
        {
            var directory = new DirectoryInfo(folder);
            return directory.GetFiles()
                .Where(f => f.Length > 0)
                .OrderByDescending(f => f.LastWriteTime)
                .First();
        }
    }
}
