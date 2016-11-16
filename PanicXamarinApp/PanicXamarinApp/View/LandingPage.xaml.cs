using PanicXamarinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class LandingPage : ContentPage
    {
        LandingPageViewModel _viewModel;
        public LandingPage()
        {
            InitializeComponent();
            _viewModel = new LandingPageViewModel(this);
            BindingContext = _viewModel;
            DisplayAlert("Success", "Successfully logged in.", "okay");         
           
        }
    }
}
