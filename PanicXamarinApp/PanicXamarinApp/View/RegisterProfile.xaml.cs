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
        #region Instance Variable
        RegisterProfileViewModel _registerProfileViewModel;
        #endregion

        #region RegisterProfile
        public RegisterProfile()
        {
            InitializeComponent();
            _registerProfileViewModel = new RegisterProfileViewModel(this);
            BindingContext = _registerProfileViewModel;
        }
        #endregion
    }
}
