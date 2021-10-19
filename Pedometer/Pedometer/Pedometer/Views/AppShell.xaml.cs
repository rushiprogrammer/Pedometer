using Pedometer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pedometer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();
        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = ViewModel;
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            //Routing.RegisterRoute(nameof(DealWebViewPage), typeof(DealWebViewPage));
            //Routing.RegisterRoute(nameof(BlogDetailPage), typeof(BlogDetailPage));
        }
    }
}