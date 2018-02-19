using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.IoT
{
    public interface IGpio : IDisposable
    {
        IEnumerable<GpioPin> Pins { get; }
    }
}
