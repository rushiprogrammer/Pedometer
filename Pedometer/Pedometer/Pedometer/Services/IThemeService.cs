using Pedometer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(Pedometer.Services.IThemeService))]
namespace Pedometer.Services
{
    public interface IThemeService
    {
        void SetAppTheme(ThemeMode themeMode);
    }
}
