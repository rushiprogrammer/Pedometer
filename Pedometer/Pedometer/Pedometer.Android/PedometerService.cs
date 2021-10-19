using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Pedometer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Pedometer.Droid
{
    public class PedometerService : IDisposable
    {
        public int Counter { get; set; }
        private bool _running = false;
        public PedometerService()
        {
            Initialize();
        }

        private void Initialize()
        {
            Counter = Preferences.Get("StepCounter", 0);
            _running = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), StartPedometer);
        }

        private bool StartPedometer()
        {
            try
            {
                var datas = Services.SqliteService.GetNullData();
                foreach (var data in datas)
                {
                    var steps = GetStepCount(data, 0.006);
                    Services.SqliteService.RemoveData(steps.Id);
                    Counter = Counter + (int)steps.StepsCount;
                }

                MessagingCenter.Send<string>(Counter.ToString(), "counterValue");

                Preferences.Set("StepCounter", Counter);

                UpdateNotification();
            }
            catch (Exception ex) { }

            return _running;
        }

        private void UpdateNotification()
        {
            PendingIntent pIntent = PendingIntent.GetActivity(MainActivity.ActivityCurrent, 0, new Intent(MainActivity.ActivityCurrent, typeof(MainActivity)), 0);
            
            if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.O)
            {
                var builder = new Notification.Builder(MainActivity.ActivityCurrent, "ServiceChannel")
                .SetContentTitle("Pedometer Service Started!")
                .SetContentText("Count : " + Counter).SetNumber(Counter)
                .SetSmallIcon(Resource.Drawable.icon_walk)
                .SetContentIntent(pIntent)
                .SetOnlyAlertOnce(true)
                .SetOngoing(false);

                var notificationManager = AndroidX.Core.App.NotificationManagerCompat.From(MainActivity.ActivityCurrent);
                notificationManager.Notify(100, builder.Build());
            }
            else
            {
                NotificationCompat.Builder builder = new NotificationCompat.Builder(MainActivity.ActivityCurrent, "ServiceChannel")
                    .SetAutoCancel(true)
                    .SetContentIntent(pIntent)
                    .SetContentTitle("Pedometer Service Started!")
                    .SetContentText("Count : " + Counter).SetNumber(Counter)
                    .SetSmallIcon(Resource.Drawable.icon_walk)
                    .SetOnlyAlertOnce(true)
                    .SetOngoing(true);

                var notificationManager = AndroidX.Core.App.NotificationManagerCompat.From(MainActivity.ActivityCurrent);
                notificationManager.Notify(100, builder.Build());
            }
        }

        public void ResetPedometer()
        {
            Counter = 0;
        }

        public void StopPedometer()
        {
            _running = false;
            var notificationManager = AndroidX.Core.App.NotificationManagerCompat.From(MainActivity.ActivityCurrent);
            notificationManager.Cancel(100);
        }

        public AccelerationData GetStepCount(AccelerationData data, double correctionFactor)
        {
            try
            {
                List<double> FinalMagnitude = null;
                double AverageMagnitude = 0.0;

                var xArray = data.Ax.Split(',');
                var yArray = data.Ay.Split(',');
                var zArray = data.Az.Split(',');
                var magArray = data.Aall.Split(',');

                var resultX = xArray.Select(x => double.Parse(x)).ToArray();
                var resultY = yArray.Select(y => double.Parse(y)).ToArray();
                var resultZ = zArray.Select(z => double.Parse(z)).ToArray();
                List<double> result = magArray.Select(x => double.Parse(x)).ToList();

                AverageMagnitude = result.Average();
                var finalMag = result.Select(x => x - AverageMagnitude).ToArray();
                FinalMagnitude = Services.MaximaService.Maxima(finalMag);
                var timeArray = data.Atime.Split(',');
                List<TimeSpan> TimeStampArray = new List<TimeSpan>();
                var count = timeArray.Length - 1;
                for (int i = 0; i < count; i++)
                {
                    DateTime tn = DateTime.Parse(timeArray[i]);
                    DateTime tnext = DateTime.Parse(timeArray[i + 1]);
                    var t = tnext - tn;
                    TimeStampArray.Add(t);
                }

                var samplingPeriod = TimeStampArray.Max();
                var samplingRate = 1 / samplingPeriod.TotalSeconds;
                var np = Math.Round(samplingRate / 5);

                data.Checked = true;

                var envelopeAx = Services.MaximaService.Maxima(resultX);
                var envelopeAy = Services.MaximaService.Maxima(resultY);
                var envelopeAz = Services.MaximaService.Maxima(resultZ);

                data.StepsCount = 0;

                if (envelopeAx.Count > 0 && envelopeAy.Count > 0 && envelopeAz.Count > 0)
                {
                    var AvgAx = envelopeAx.Average();
                    var AvgAy = envelopeAy.Average();
                    var AvgAz = envelopeAz.Average();

                    //correctionFactor can be 3 to 5
                    correctionFactor = 0;

                    var avg = ((AvgAx + AvgAy + AvgAz) / np);
                    var threshold = avg - correctionFactor;

                    //var avgMag = FinalMagnitude.Average();
                    //if (avgMag > threshold)
                    //    data.StepsCount++;
                    var isStep = false;
                    var peaks = Services.MaximaService.Maxima(FinalMagnitude.ToArray());
                    foreach (var final in peaks)
                    {
                        if (final > threshold)
                        {
                            isStep = true;
                        }
                    }
                    if (isStep)
                    {
                        data.StepsCount++;
                    }
                }
            }
            catch { }

            return data;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
