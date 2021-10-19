using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Essentials;

namespace Pedometer.Services
{
    public class AccelerometerService : IDisposable
    {
        private CancellationTokenSource cancellationToken;
        public List<double> XArray { get; set; }
        public List<double> YArray { get; set; }
        public List<double> ZArray { get; set; }
        public List<string> TimeStampArray { get; set; }
        public List<double> MagnitudeArray { get; set; }

        public const double Gravity = 9.81;
        public AccelerometerService()
        {
            Initialize();
        }

        private void Initialize()
        {
            XArray = new List<double>();
            YArray = new List<double>();
            ZArray = new List<double>();
            TimeStampArray = new List<string>();
            MagnitudeArray = new List<double>();
            cancellationToken = new CancellationTokenSource();

            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Services.SqliteService.Init();
            
        }

        public void StartAccelerometer()
        {
            Accelerometer.Start(SensorSpeed.Game);
        }

        public void StopAccelerometer()
        {
            Accelerometer.Stop();
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            try
            {
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                var x = e.Reading.Acceleration.X; //* Gravity;
                var y = e.Reading.Acceleration.Y; //* Gravity;
                var z = e.Reading.Acceleration.Z; //* Gravity;

                var Magnitude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
                XArray.Add(x);
                YArray.Add(y);
                ZArray.Add(z);
                TimeStampArray.Add(timestamp);
                MagnitudeArray.Add(Magnitude);

                if (XArray.Count == 160 && YArray.Count == 160 && ZArray.Count == 160)
                {
                    var xString = string.Join(",", XArray);
                    var yString = string.Join(",", YArray);
                    var zString = string.Join(",", ZArray);
                    var timeString = string.Join(",", TimeStampArray);
                    var magString = string.Join(",", MagnitudeArray);

                    Services.SqliteService.AddData(xString, yString, zString, timeString, magString, 0);

                    XArray.Clear();
                    YArray.Clear();
                    ZArray.Clear();
                    TimeStampArray.Clear();
                    MagnitudeArray.Clear();
                }
            }
            catch { }
        }

        public void Dispose()
        {
            Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
        }
    }
}
