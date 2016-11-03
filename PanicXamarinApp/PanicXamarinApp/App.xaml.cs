using PanicXamarinApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PanicXamarinApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //  MainPage = new PanicXamarinApp.MainPage();
           //   MainPage = new DeviceLocation();
            //  MainPage = new _MasterDetailPage();
            // MainPage = new MasterDetailPage1();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
