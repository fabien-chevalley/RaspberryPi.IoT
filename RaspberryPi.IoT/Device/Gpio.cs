using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Woopsa;

namespace RaspberryPi.IoT
{
    public class Gpio : IGpio, IDisposable
    {
        private const string DevicePath = @"/sys/class/gpio";
        private readonly ILogger<RaspberryPi> _logger;
        private readonly List<GpioPin> _gpioPins;

        public Gpio(ILogger<RaspberryPi> logger)
        {
            _logger = logger;
            _gpioPins = new List<GpioPin>();

            InitializePin();
        }

        public IEnumerable<GpioPin> Pins => _gpioPins;

        #region Pinout

        public GpioPin P1Pin3 { get; private set; }

        public GpioPin P1Pin5 { get; private set; }

        public GpioPin P1Pin7 { get; private set; }

        public GpioPin P1Pin29 { get; private set; }

        public GpioPin P1Pin31 { get; private set; }

        public GpioPin P1Pin26 { get; private set; }

        public GpioPin P1Pin24 { get; private set; }

        public GpioPin P1Pin21 { get; private set; }

        public GpioPin P1Pin19 { get; private set; }

        public GpioPin P1Pin23 { get; private set; }

        public GpioPin P1Pin32 { get; private set; }

        public GpioPin P1Pin33 { get; private set; }

        public GpioPin P1Pin8 { get; private set; }

        public GpioPin P1Pin10 { get; private set; }

        public GpioPin P1Pin36 { get; private set; }

        public GpioPin P1Pin11 { get; private set; }

        public GpioPin P1Pin12 { get; private set; }

        public GpioPin P1Pin35 { get; private set; }

        public GpioPin P1Pin38 { get; private set; }

        public GpioPin P1Pin40 { get; private set; }

        public GpioPin P1Pin15 { get; private set; }

        public GpioPin P1Pin16 { get; private set; }

        public GpioPin P1Pin18 { get; private set; }

        public GpioPin P1Pin22 { get; private set; }

        public GpioPin P1Pin37 { get; private set; }

        public GpioPin P1Pin13 { get; private set; }

        #endregion

        private void InitializePin()
        {
            _gpioPins.Add(P1Pin3 = new GpioPin(2)); 
            _gpioPins.Add(P1Pin5 = new GpioPin(3)); 
            _gpioPins.Add(P1Pin7 = new GpioPin(4)); 
            _gpioPins.Add(P1Pin29 = new GpioPin(5)); 
            _gpioPins.Add(P1Pin31 = new GpioPin(6)); 
            _gpioPins.Add(P1Pin26 = new GpioPin(7)); 
            _gpioPins.Add(P1Pin24 = new GpioPin(8)); 
            _gpioPins.Add(P1Pin21 = new GpioPin(9)); 
            _gpioPins.Add(P1Pin19 = new GpioPin(10)); 
            _gpioPins.Add(P1Pin23 = new GpioPin(11)); 
            _gpioPins.Add(P1Pin32 = new GpioPin(12)); 
            _gpioPins.Add(P1Pin33 = new GpioPin(13)); 
            _gpioPins.Add(P1Pin36 = new GpioPin(16)); 
            _gpioPins.Add(P1Pin11 = new GpioPin(17)); 
            _gpioPins.Add(P1Pin12 = new GpioPin(18)); 
            _gpioPins.Add(P1Pin35 = new GpioPin(19)); 
            _gpioPins.Add(P1Pin38 = new GpioPin(20)); 
            _gpioPins.Add(P1Pin40 = new GpioPin(21)); 
            _gpioPins.Add(P1Pin15 = new GpioPin(22)); 
            _gpioPins.Add(P1Pin16 = new GpioPin(23)); 
            _gpioPins.Add(P1Pin18 = new GpioPin(24)); 
            _gpioPins.Add(P1Pin22 = new GpioPin(25)); 
            _gpioPins.Add(P1Pin37 = new GpioPin(26)); 
            _gpioPins.Add(P1Pin13 = new GpioPin(27)); 
        }

        #region IDisposable
        
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach(var pin in _gpioPins.ToArray())
                        pin.Dispose();
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
