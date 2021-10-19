using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using Pedometer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly:Xamarin.Forms.Dependency(typeof(Pedometer.Droid.BackgroundService))]
namespace Pedometer.Droid
{
    [Service(Label = "BackgroundService", ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
    public class BackgroundService : Service, IAndroidService
    {
        private AccelerometerService accelerometerService { get; set; }
        private PedometerService pedometerService { get; set; }
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if(intent.Action == "START_SERVICE")
            {
                RegisterNotification();
                System.Threading.Tasks.Task.Run(() =>
                {
                    accelerometerService = new AccelerometerService();
                    pedometerService = new PedometerService();
                    accelerometerService.StartAccelerometer();
                });
            }
            else if (intent.Action == "STOP_SERVICE")
            {
                accelerometerService.StopAccelerometer();
                pedometerService.StopPedometer();
                StopForeground(true);
                StopSelfResult(startId);
            }
            else if (intent.Action == "RESET_SERVICE")
            {
                pedometerService.ResetPedometer();
            }

            return base.OnStartCommand(intent, flags, startId);
        }

        public void StartService()
        {
            Intent intent = new Intent(MainActivity.ActivityCurrent ,typeof(BackgroundService));
            intent.SetAction("START_SERVICE");
            MainActivity.ActivityCurrent.StartService(intent);
        }

        public void StopService()
        {
            Intent intent = new Intent(MainActivity.ActivityCurrent, this.Class);
            intent.SetAction("STOP_SERVICE");
            MainActivity.ActivityCurrent.StartService(intent);
        }

        public void ResetService()
        {
            Intent intent = new Intent(MainActivity.ActivityCurrent, this.Class);
            intent.SetAction("RESET_SERVICE");
            MainActivity.ActivityCurrent.StartService(intent);
        }

        private void RegisterNotification()
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel("ServiceChannel", "BackgroundService", NotificationImportance.Max);
                NotificationManager notificationManager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
                Notification notification = new Notification.Builder(this, "ServiceChannel")
                    .SetContentTitle("Pedometer Service Started!")
                    .SetContentText("Count : ")
                    .SetSmallIcon(Resource.Drawable.icon_walk)
                    .SetOngoing(true).Build();

                StartForeground(100, notification);
            }
            else
            {
                NotificationManager notificationManager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(NotificationService);
                //notificationManager.CreateNotificationChannel(channel);
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "ServiceChannel")
                    .SetAutoCancel(true)
                    .SetContentTitle("Pedometer Service Started!")
                    .SetContentText("Count : ")
                    .SetSmallIcon(Resource.Drawable.icon_walk)
                    .SetOngoing(true);

                StartForeground(100, builder.Build());
            }
            
        }
    }
}