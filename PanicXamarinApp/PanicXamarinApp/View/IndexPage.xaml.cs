using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.EntityModel;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;
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
            imgPanic.GestureRecognizers.Add(panicButtion);

            Utility _utility = new Utility();
            _utility.CreateDatabase();
        }    

        #region Events 
        private async void PanicButtion_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SendPanicAlert());           
        }

        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterProfile());
            //await DisplayAlert("Alert", "We are working on...", "Okay");
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            // await DisplayAlert("Alert", "We are working on...", "Okay");
            await Navigation.PushAsync(new LoginPage());
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
