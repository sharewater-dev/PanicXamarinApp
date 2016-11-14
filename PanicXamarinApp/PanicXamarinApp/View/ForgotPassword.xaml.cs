using PanicXamarinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class ForgotPassword : ContentPage
    {
        ForgotPasswordViewModel _viewModel;
        public ForgotPassword()
        {
            InitializeComponent();
            _viewModel = new ForgotPasswordViewModel();
            BindingContext = _viewModel;
        }
    }
}
