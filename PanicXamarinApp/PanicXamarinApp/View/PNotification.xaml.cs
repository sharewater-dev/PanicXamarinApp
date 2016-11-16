using PanicXamarinApp.DependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class PNotification : ContentPage
    {
        public PNotification()
        {
            InitializeComponent();
           DependencyService.Get<IPushNotification>().sendNotification();
        }
    }
}
