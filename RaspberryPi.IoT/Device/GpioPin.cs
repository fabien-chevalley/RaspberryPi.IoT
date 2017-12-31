using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RaspberryPi.IoT
{
    public enum GpioModes
    {
        Input,
        Output
    }

    public enum GpioValues
    {
        Low = 0,
        High = 1
    }

    public class GpioPin : IGpioPin, IDisposable
    {
        private const string DevicePath = @"/sys/class/gpio";
        private readonly string _gpioPath;

        public GpioPin(int pinNumber)
        {
            if (pinNumber < 1 || pinNumber > 27)
                throw new ArgumentOutOfRangeException("Valid pins are between 1 and 27.");

            PinNumber = pinNumber;
            _gpioPath = Path.Combine(DevicePath, string.Concat("gpio", PinNumber.ToString()));
        }

        #region Public members

        public int PinNumber { get; private set; }

        // //TODO : Improve with pin status, not only pin number 
        public string Status => $"Status of pin {PinNumber}: ";

        public void Open()
        {
            var gpioExportPath = Path.Combine(DevicePath, "export");
            if (!Directory.Exists(_gpioPath))
            {
                File.WriteAllText(gpioExportPath, PinNumber.ToString());
                Directory.CreateDirectory(_gpioPath);
            }
        }

        public void Close()
        {
            var gpioExportPath = Path.Combine(DevicePath, "unexport");
            if (Directory.Exists(_gpioPath))
                File.WriteAllText(gpioExportPath, PinNumber.ToString());
        }

        public void SetMode(GpioModes mode)
        {
            switch (mode)
            {
                case GpioModes.Input:
                    File.WriteAllText(Path.Combine(_gpioPath, "direction"), "in");
                    Directory.SetLastWriteTime(Path.Combine(_gpioPath), DateTime.UtcNow);
                break;
                case GpioModes.Output:
                    File.WriteAllText(Path.Combine(_gpioPath, "direction"), "out");
                    Directory.SetLastWriteTime(Path.Combine(_gpioPath), DateTime.UtcNow);
                break;
            }
        }

        public void Write(GpioValues value)
        {
            File.WriteAllText(Path.Combine(_gpioPath, "value"), ((int)value).ToString());
            Directory.SetLastWriteTime(Path.Combine(_gpioPath), DateTime.UtcNow);
        }

        public GpioValues Read()
        {
            if (File.Exists(Path.Combine(_gpioPath, "value")))
            {
                var value = File.ReadAllText(Path.Combine(_gpioPath, "value"));
                return (GpioValues)Enum.Parse(typeof(GpioValues), value);
            }
            else
            {
                return GpioValues.Low;
            }
        }

        public void Pulse(int milliseconds)
        {
            Write(GpioValues.High);
            System.Threading.Thread.Sleep(milliseconds);
            Write(GpioValues.Low);
        }

        #endregion

        #region IDisposable
        
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    Close();

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
