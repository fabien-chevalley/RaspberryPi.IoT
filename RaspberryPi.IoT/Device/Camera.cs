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

namespace RaspberryPi.IoT
{
    public class Camera : IDisposable
    {
        private readonly ILogger<RaspberryPi> _logger;
        private readonly MMALCamera _camera;

        public Camera(ILogger<RaspberryPi> logger)
        {
            _logger = logger;   
            _camera = MMALCamera.Instance;   
            _logger.LogDebug(_camera.ToString());
        }
        
        public void GrabImage(string path)
        {
            var task = Task.Run(async () =>
            {
                using (var imgCaptureHandler = new ImageStreamCaptureHandler(path, "jpg"))        
                using (var imgEncoder = new MMALImageEncoder(imgCaptureHandler))
                using (var renderer = new MMALNullSinkComponent())
                {
                    imgEncoder.ConfigureOutputPort(0, MMALEncoding.JPEG, MMALEncoding.I420, 90);

                    _camera.Camera.StillPort.ConnectTo(imgEncoder);
                    _camera.Camera.PreviewPort.ConnectTo(renderer);
                    _camera.ConfigureCameraSettings();

                    await _camera.BeginProcessing(_camera.Camera.StillPort, imgEncoder);
                }
            });

            //TODO: Improve Woopsa error to display AggregateException
            task.Wait();
        }  

        public void GrabVideo(string path, TimeSpan duration)
        {
            var task = Task.Run(async () =>
            {
                using (var vidCaptureHandler  = new VideoStreamCaptureHandler(path, "avi"))        
                using (var vidEncoder  = new MMALVideoEncoder(vidCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.Add(duration)))
                using (var renderer = new MMALVideoRenderer())
                { 
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0, 25000000);

                    _camera.Camera.VideoPort.ConnectTo(vidEncoder);
                    _camera.Camera.PreviewPort.ConnectTo(renderer);
                    _camera.ConfigureCameraSettings();
                    
                    await _camera.BeginProcessing(_camera.Camera.VideoPort);
                }
            });

            //TODO: Improve Woopsa error to display AggregateException
            task.Wait();
        }  

        public void StartStreaming(string url, TimeSpan duration)
        {
            var task = Task.Run(async () =>
            {
                //TODO: Stream directly with rtmp-sharp library
                using (var ffCaptureHandler = FFmpegCaptureHandler.RTMPStreamer("mystream", "rtmp://localhost:6767/live"))
                using (var vidEncoder = new MMALVideoEncoder(ffCaptureHandler, new MMAL_RATIONAL_T(25, 1), DateTime.Now.Add(duration)))
                using (var renderer = new MMALVideoRenderer())
                {
                    vidEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0, 25000000);

                    _camera.Camera.VideoPort.ConnectTo(vidEncoder);
                    _camera.Camera.PreviewPort.ConnectTo(renderer);
                    _camera.ConfigureCameraSettings(); 

                    await _camera.BeginProcessing(_camera.Camera.VideoPort);
                }
            });

            //TODO: Improve Woopsa error to display AggregateException
            task.Wait();
        }

        #region IDisposable
        
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {            
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _camera.Cleanup();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
