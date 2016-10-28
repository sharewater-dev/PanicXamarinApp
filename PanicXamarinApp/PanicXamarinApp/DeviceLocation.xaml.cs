using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp
{
    public partial class DeviceLocation : ContentPage
    {
        public DeviceLocation()
        {
            InitializeComponent();
            btnGetDeviceLocation.Clicked += BtnGetDeviceLocation_Clicked;
        }

        private async void BtnGetDeviceLocation_Clicked(object sender, EventArgs e)
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
              //  locator.AllowsBackgroundUpdates = true;

               var position = await locator.GetPositionAsync(10000);

                string status = "Position Status : " + position.Timestamp; ;
                status += " Position Latitude : " + position.Latitude;
                status += "Position Longitude : " + position.Longitude;
                lblStatus.Text = status;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Warning", "Gps location are disabled", "okay");

            }
        }
    }
}
