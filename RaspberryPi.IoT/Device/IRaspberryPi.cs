using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.IoT
{
    public interface IRaspberryPi : IDisposable
    {
        GpioPin OpenPin(int pinNumber);
        void ClosePin(int pinNumber);
        IDictionary<string, string> Pins { get; }
    }
}
