using Pedometer.Common;
using Pedometer.Models;
using Pedometer.Services;
using Pedometer.Styles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Pedometer.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        public ShellViewModel()
        {
            StartCommand = new Command( () => Start());
            StopCommand = new Command(() => Stop());
            ResetCommand = new Command(() => Reset());

            Initialize();
        }

        private void Initialize()
        {
            Goals = new List<string>();
            Goals.Add("5000");
            Goals.Add("7500");
            Goals.Add("10000");
            Goals.Add("15000");
            Goal = Preferences.Get("Goal", 5000);
            StepCount = Preferences.Get("StepCounter", 0);
            //MessagingCenter.Unsubscribe<string>(this, "counterValue");
            MessagingCenter.Subscribe<string>(this, "counterValue", (value) => 
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Counter = value;
                    GoalPercent = (StepCount / Goal) * 100;
                });
            });

            ThemeIsToggled = Preferences.Get("Theme", false);
        }

        private void SetTheme(bool status)
        {
            ThemeMode themeRequested;
            if (status)
            {
                themeRequested = ThemeMode.Dark;
            }
            else
            {
                themeRequested = ThemeMode.Light;
            }

            DependencyService.Get<IThemeService>().SetAppTheme(themeRequested);
        }

        private void Start()
        {
            DependencyService.Get<IAndroidService>().StartService();
            MessagingCenter.Send<string>("1", "myService");
        }

        private void Stop()
        {
            DependencyService.Get<IAndroidService>().StopService();
            MessagingCenter.Send<string>("0", "myService");
        }

        private void Reset()
        {
            DependencyService.Get<IAndroidService>().ResetService();
            //MessagingCenter.Send<string>("0", "myService");
        }

        private string m_Counter;
        public string Counter
        {
            get { return m_Counter; }
            set { Set(ref m_Counter, value); }
        }

        private int m_StepCount;
        public int StepCount
        {
            get { return m_StepCount; }
            set { Set(ref m_StepCount, value); }
        }

        private int m_Goal;
        public int Goal
        {
            get { return m_Goal; }
            set { Set(ref m_Goal, value); }
        }

        private string m_SelectedGoal;
        public string SelectedGoal
        {
            get { return m_SelectedGoal; }
            set 
            { 
                Set(ref m_SelectedGoal, value);
                try
                {
                    Goal = int.Parse(value);
                    Preferences.Set("Goal", Goal);
                }
                catch { }
            }
        }

        private int m_GoalPercent;
        public int GoalPercent
        {
            get { return m_GoalPercent; }
            set { Set(ref m_GoalPercent, value); }
        }

        private bool m_ThemeIsToggled;
        public bool ThemeIsToggled
        {
            get { return m_ThemeIsToggled; }
            set 
            { 
                Set(ref m_ThemeIsToggled, value); 
                try
                {
                    Preferences.Set("Theme", value);
                    SetTheme(value);
                }
                catch(Exception ex) { }
            }
        }

        public Command StartCommand { get; set; }
        public Command StopCommand { get; }
        public Command ResetCommand { get; set; }

        public List<string> Goals { get; private set; }
    }
}
