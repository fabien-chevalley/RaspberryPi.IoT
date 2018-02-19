using System;

namespace RaspberryPi.IoT
{
    public interface ICamera
    {
        void GrabImage();
        void GrabVideo(TimeSpan duration);
        void StartStreaming(string url, string streamName, TimeSpan duration);
        string SharedFolder { get; }
    }
}