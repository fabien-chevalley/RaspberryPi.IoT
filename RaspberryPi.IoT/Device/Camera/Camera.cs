using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MMALSharp;
using Woopsa;
using MMALSharp.Handlers;
using MMALSharp.Components;
using MMALSharp.Native;
using Nito.AsyncEx;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RaspberryPi.IoT
{
    public class Camera : ICamera
    {
        public const string Folder = "/home/pi/share/camera";
        public const string Image = "image";
        public const string Video = "video";

        private readonly ILogger<RaspberryPi> _logger;

        public Camera(ILogger<RaspberryPi> logger)
        {
            _logger = logger;   
        }

        public string LastImageFilename { get; private set; }

        public string LastVideoFilename { get; private set; }

        public string SharedFolder => Folder;

        public void GrabImage()
        {
            var task = Task.Run(async () =>
            {
                await GrabImageAsync();
            });

            //TODO: Improve Woopsa error to display AggregateException
            task.Wait();
        }  

        public async Task GrabImageAsync()
        {
            var camera = MMALCamera.Instance;
            using (var imgCaptureHandler = new ImageStreamCaptureHandler(Path.Combine(Folder, Image), "jpg"))        
            using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
            using (var renderer = new MMALNullSinkComponent())
            {
                imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                camera.Camera.StillPort.ConnectTo(imgEncoder);
                camera.Camera.PreviewPort.ConnectTo(renderer);
                camera.ConfigureCameraSettings();

                await camera.BeginProcessing(camera.Camera.StillPort);
            }
            camera.Cleanup();
            LastImageFilename = Path.Combine(Image, GetLastFile(Path.Combine(Folder, Image)).Name);
        }

        public void Timelapse(TimeSpan interval, TimeSpan timeout)
        { 
            var camera = MMALCamera.Instance;
            var task = Task.Run(async () =>
            {
                var endTime = DateTime.Now.Add(timeout);
                _logger.LogTrace($"Begin timelapse, end time : {endTime}, interval = {interval}");
                while (DateTime.Now.CompareTo(endTime) < 0)
                {
                    await Task.Delay(interval);
                    await GrabImageAsync();
                }
            });

            //TODO: Improve Woopsa error to display AggregateException
            task.Wait();
            camera.Cleanup();
        }  

        public void GrabVideo(TimeSpan duration)
        {
            var camera = MMALCamera.Instance;
            var task = Task.Run(async () =>
            {
                using (var vidCaptureHandler  = new VideoStreamCaptureHandler(Path.Combine(Folder, Video), "avi"))        
                using (var vidEncoder  = new MMALVideoEncoder(vidCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.Add(duration)))
                using (var renderer = new MMALVideoRenderer())
                { 
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0, 25000000);

                    camera.Camera.VideoPort.ConnectTo(vidEncoder);
                    camera.Camera.PreviewPort.ConnectTo(renderer);
                    camera.ConfigureCameraSettings();
                    
                    await camera.BeginProcessing(camera.Camera.VideoPort);
                }
            });

            //TODO: Improve Woopsa error to display AggregateException
            task.Wait();
            camera.Cleanup();
            LastVideoFilename = Path.Combine(Image, GetLastFile(Path.Combine(Folder, Video)).Name);
        }  

        public void StartStreaming(string url, string streamName, TimeSpan duration)
        {
            var camera = MMALCamera.Instance;
            var task = Task.Run(async () =>
            {
                using (var ffCaptureHandler = new FFmpegCaptureHandler($"-i - -vcodec copy -an -f flv {url}{streamName}"))
                using (var vidEncoder = new MMALVideoEncoder(ffCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.Add(duration)))
                using (var renderer = new MMALVideoRenderer())
                {
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0, 25000000);

                    camera.Camera.VideoPort.ConnectTo(vidEncoder);
                    camera.Camera.PreviewPort.ConnectTo(renderer);
                    camera.ConfigureCameraSettings(); 
 
                    await camera.BeginProcessing(camera.Camera.VideoPort);
                }
            });

            //TODO: Improve Woopsa error to display AggregateException
            task.Wait();
            camera.Cleanup();
        }

        public void StartStreamingAsync(string url, string streamName, TimeSpan duration)
        {
            var camera = MMALCamera.Instance;

            AsyncContext.Run(async () =>
            {            
                using (var ffCaptureHandler = new FFmpegCaptureHandler($"-i - -vcodec copy -an -f flv {url}{streamName}"))
                using (var vidEncoder = new MMALVideoEncoder(ffCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.Add(duration)))
                using (var renderer = new MMALVideoRenderer())
                {
                    camera.ConfigureCameraSettings(); 

                    //Create our component pipeline. Here we are using the H.264 standard with a YUV420 pixel format. The video will be taken at 25mb/s at the highest quality setting (10).
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0, 25000000);

                    camera.Camera.VideoPort.ConnectTo(vidEncoder);
                    camera.Camera.PreviewPort.ConnectTo(renderer);
                                            
                    //Camera warm up time
                    await Task.Delay(2000);
                    
                    //Take video for 3 minutes.
                    await camera.BeginProcessing(camera.Camera.VideoPort);
                }

            });

            camera.Cleanup();
        }

        private FileInfo GetLastFile(string folder)
        {
            var directory = new DirectoryInfo(folder);
            return directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First();
        }
    }
}
