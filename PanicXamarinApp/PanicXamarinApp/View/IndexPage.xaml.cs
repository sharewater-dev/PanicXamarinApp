using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.EntityModel;
using PanicXamarinApp.ViewModel;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class IndexPage : ContentPage
    {
        IndexPageViewModel _pageModel = new IndexPageViewModel();
        public IndexPage()
        {          
            InitializeComponent();
            BindingContext = _pageModel;
            // Created  TapGestureRecognizer for panic button tapped event
            TapGestureRecognizer panicButtion = new TapGestureRecognizer();
            panicButtion.Tapped += PanicButtion_Tapped;
           // imgPanic.GestureRecognizers.Add(panicButtion);         
        }    

        #region Events 
        private async void PanicButtion_Tapped(object sender, EventArgs e)
        {
            try
            {
                _pageModel.IsBusy = true;
                // Get the Latitude and Longitude of the Current User
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                //  locator.AllowsBackgroundUpdates = true;
                var position = await locator.GetPositionAsync(10000);
                //  string status = "Position Status : " + position.Timestamp; ;
                string status = " Latitude : " + position.Latitude;
                status += ", Longitude : " + position.Longitude;
                if (position != null)
                {
                    var test= GetDeviceUniqueId();
                    if(Device.OS == TargetPlatform.Android)
                    {
                        status += ", IMEI : " + test.DeviceInformation.IMEI;
                        status += ", PhoneNumber : " + test.DeviceInformation.PhoneNumber;
                    }
                    else if(Device.OS == TargetPlatform.iOS)
                    {
                        status += ", UniqueID : " + test.DeviceInformation.UniqueID;
                        status += ", PhoneNumber : Apple can't shared phone number" ;
                    }
                  //  lblStatus.Text = status;
                }
                else
                {

                }          
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert!!", "GPS Location is disabled. Please enable and try again.", "okay");
            }
            finally
            {
                _pageModel.IsBusy = false;
            }
        }

        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "We are working on...", "Okay");
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "We are working on...", "Okay");
        }

        #endregion

        #region Functions

        public UserDeviceModel GetDeviceUniqueId()
        {
            IDevice device = DependencyService.Get<IDevice>();
            UserDeviceModel deviceIdentifier = device.GetIdentifier(0);
            return deviceIdentifier;
        }
        #endregion
    }
}
