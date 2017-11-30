using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RaspberryPi.IoT
{
    public enum GpioPinModes
    {
        Input,
        Output
    }

    public enum GpioPinValues
    {
        Low = 0,
        High = 1
    }

    public class GpioPin : IDisposable
    {
        #region Fields

        private IRaspberryPi _owner;

        #endregion

        public GpioPin(IRaspberryPi owner, int pinNumber, string gpioPath)
        {
            _owner = owner;
            PinNumber = pinNumber;
            GpioPath = gpioPath;
        }

        #region Public Members

        public int PinNumber { get; private set; }

        public string GpioPath { get; private set; }

        public void SetDriveMode(GpioPinModes driveMode)
        {
            if (driveMode == GpioPinModes.Output)
            {
                File.WriteAllText(Path.Combine(GpioPath, "direction"), "out");
                Directory.SetLastWriteTime(Path.Combine(GpioPath), DateTime.UtcNow);
            }
            else
            {
                File.WriteAllText(Path.Combine(GpioPath, "direction"), "in");
                Directory.SetLastWriteTime(Path.Combine(GpioPath), DateTime.UtcNow);
            }
        }

        public void Write(GpioPinValues pinValue)
        {
            File.WriteAllText(Path.Combine(GpioPath, "value"), ((int)pinValue).ToString());
            Directory.SetLastWriteTime(Path.Combine(GpioPath), DateTime.UtcNow);
        }

        public GpioPinValues Read()
        {
            if (File.Exists(Path.Combine(GpioPath, "value")))
            {
                var pinValue = File.ReadAllText(Path.Combine(GpioPath, "value"));
                return (GpioPinValues)Enum.Parse(typeof(GpioPinValues), pinValue);
            }
            else
            {
                return GpioPinValues.Low;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _owner.ClosePin(PinNumber);
        }

        #endregion
    }
}
