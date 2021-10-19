using Pedometer.Models;
using Pedometer.Styles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedometer.Services
{
    public class ThemeService : IThemeService
    {
        public void SetAppTheme(ThemeMode themeMode)
        {
            SetTheme(themeMode);
        }
        private void SetTheme(ThemeMode mode)
        {
            if (mode == ThemeMode.Dark)
            {
                if (App.AppTheme == ThemeMode.Dark)
                    return;
                Xamarin.Forms.Application.Current.Resources = new DarkTheme();
            }
            else
            {
                if (App.AppTheme != ThemeMode.Dark)
                    return;
                Xamarin.Forms.Application.Current.Resources = new LightTheme();
            }
            App.AppTheme = mode;
        }
    }
}
