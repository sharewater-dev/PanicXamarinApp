using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PanicXamarinApp.ViewModel
{
    public class ForgotPasswordViewModel : BaseNavigationViewModel
    {
        public string _Email = string.Empty;
        public ICommand _ForgotPasswordButton;
        public string _Message = string.Empty;


        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged("Email"); }
        }
        public string Message { get { return _Message; } set { _Message = value;OnPropertyChanged("Message"); } }
        public ICommand ForgotPasswordButton { get { return _ForgotPasswordButton; } set { _ForgotPasswordButton = value; OnPropertyChanged("ForgotPasswordButton"); } }

        public ForgotPasswordViewModel()
        {
            ForgotPasswordButton = new Command(ForgotPassword);
        }
        public void ForgotPassword()
        {
            if (string.IsNullOrEmpty(Email))
                Message = "Please entered registered mobile number";

            if (IsValidEmail())
                Message = "We are working on email sending functionality.";           
            
        }
        public bool IsValidEmail()
        {
            UserProfile user = new UserProfile();
            user.Email = Email;         

            ResponseModel<bool> _forgotPass = new UserDAL().IsValidEmail(user);
            if (!_forgotPass.Status)
            {
                Message = _forgotPass.Message;
            }
            return _forgotPass.Status;  
        }
    }
}
