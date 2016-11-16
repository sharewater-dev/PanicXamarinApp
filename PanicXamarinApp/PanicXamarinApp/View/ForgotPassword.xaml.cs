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
        private void EmailChanged(object sender, TextChangedEventArgs e)
        {
            int _limit = 30;     //Enter text limit
            string _text = _viewModel.Email;      //Get Current Text
            if (_text.Length > _limit)       //If it is more than your character restriction
            {
                _text = _text.Remove(_text.Length - 1);  // Remove Last character
                _viewModel.Email = _text;        //Set the Old value
            }
        }
    }
}
