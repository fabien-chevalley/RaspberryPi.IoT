using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.IoT
{
    public interface IRaspberryPi : IDisposable
    {
        IGpio Gpio { get; }
        ICamera Camera { get; }
    }
}
