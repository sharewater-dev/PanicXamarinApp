using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class LoginPage : ContentPage
    {
        LoginPageViewModel viewModel;
        public LoginPage()
        {
            InitializeComponent();
            viewModel = new LoginPageViewModel(this);
            BindingContext = viewModel;
            txtEmail.TextChanged += TxtEmail_TextChanged;
        }

        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            int _limit = 20;     //Enter text limit
            string _text = viewModel.Email;      //Get Current Text
            if (_text.Length > _limit)       //If it is more than your character restriction
            {
                _text = _text.Remove(_text.Length - 1);  // Remove Last character
                viewModel.Email = _text;        //Set the Old value
            }
        }
    }
}
