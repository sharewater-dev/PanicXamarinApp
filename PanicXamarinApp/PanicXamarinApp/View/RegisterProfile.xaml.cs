using PanicXamarinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class RegisterProfile : ContentPage
    {
        RegisterProfileViewModel _registerProfileViewModel;
        public RegisterProfile()
        {
            InitializeComponent();
            _registerProfileViewModel = new RegisterProfileViewModel();    
        }

        #region Events
        private void BtnAddICE_Clicked(object sender, EventArgs e)
        {
          
        }

        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void BtnSubmit_Clicked(object sender, EventArgs e)
        {
           
        }
        #endregion
    }
}
