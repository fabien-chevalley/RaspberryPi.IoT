using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.IoT
{
    public interface IGpio
    {
        void Open(int pinNumber);
        void Close(int pinNumber);
        void SetMode(int pinNumber, GpioModes mode);
        void Write(int pinNumber, GpioValues value);
        GpioValues Read(int pinNumber);
        void Pulse(int pinNumber, int milliseconds);
        string Status { get; }
    }
}
