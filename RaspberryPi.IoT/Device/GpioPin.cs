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

    public class GpioPin
    {
        public GpioPin(int pinNumber, string gpioPath)
        {
            PinNumber = pinNumber;
            GpioPath = gpioPath;
        }

        #region Public members

        public int PinNumber { get; private set; }

        public string GpioPath { get; private set; }

        public void SetMode(GpioModes mode)
        {
            switch (mode)
            {
                case GpioModes.Input:
                    File.WriteAllText(Path.Combine(GpioPath, "direction"), "in");
                    Directory.SetLastWriteTime(Path.Combine(GpioPath), DateTime.UtcNow);
                break;
                case GpioModes.Output:
                    File.WriteAllText(Path.Combine(GpioPath, "direction"), "out");
                    Directory.SetLastWriteTime(Path.Combine(GpioPath), DateTime.UtcNow);
                break;
            }
        }

        public void Write(GpioValues pinValue)
        {
            File.WriteAllText(Path.Combine(GpioPath, "value"), ((int)pinValue).ToString());
            Directory.SetLastWriteTime(Path.Combine(GpioPath), DateTime.UtcNow);
        }

        public GpioValues Read()
        {
            if (File.Exists(Path.Combine(GpioPath, "value")))
            {
                var pinValue = File.ReadAllText(Path.Combine(GpioPath, "value"));
                return (GpioValues)Enum.Parse(typeof(GpioValues), pinValue);
            }
            else
            {
                return GpioValues.Low;
            }
        }

        #endregion
    }
}
