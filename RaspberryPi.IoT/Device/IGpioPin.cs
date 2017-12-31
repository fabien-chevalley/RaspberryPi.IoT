namespace RaspberryPi.IoT
{
    public interface IGpioPin
    {
        void Open();
        void Close();
        void SetMode(GpioModes mode);
        void Write(GpioValues value);
        GpioValues Read();
        void Pulse(int milliseconds);
        string Status { get; }
    }
}