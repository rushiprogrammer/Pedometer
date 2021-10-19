using Pedometer.Services;
using Pedometer.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pedometer
{
    public partial class App : Application
    {
        public static ThemeMode AppTheme { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            Xamarin.Forms.DependencyService.Register<IThemeService,ThemeService>();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
