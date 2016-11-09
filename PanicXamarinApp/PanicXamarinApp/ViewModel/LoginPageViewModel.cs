using System;
using System.Windows.Input;
using PanicXamarinApp.ViewModel;
using PanicXamarinApp.View;
using Xamarin.Forms;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;

namespace PanicXamarinApp
{
	public class LoginPageViewModel : BaseNavigationViewModel
	{
		private LoginPage viewModel;
		private string _email;
		private string _password;
		private bool _isFormValidated = false;

		public string Email
		{
			get { return _email; }
			set
			{
				_email = value;
				OnPropertyChanged("Email");
			}
		}
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged("Password");
			}
		}
		public ICommand _loginButton;
		public ICommand LoginButton
		{
			private set { _loginButton = value; OnPropertyChanged("LoginButton"); }
			get { return _loginButton; }
		}

		public ICommand _forgotPasswordButton;
		public ICommand ForgotPasswordButton
		{
			private set { _forgotPasswordButton = value; OnPropertyChanged("ForgotPasswordButton"); }
			get { return _forgotPasswordButton; }
		}

		public LoginPageViewModel(LoginPage viewModel)
		{
			this.viewModel = viewModel;
			LoginButton = new Command(LoginEvent);
			ForgotPasswordButton = new Command(ForgotPasswordEvent);
		}
		private void LoginEvent()
		{
			Validation();
			if (_isFormValidated)
			{
				CheckUser();
			}
		}

		private void Validation()
		{
			if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
			{
				viewModel.DisplayAlert("Error", "Please enter email or password", "okay");
				_isFormValidated = false;
			}
			_isFormValidated = true;
		}

		private void CheckUser()
		{
			UserProfile user = new UserProfile();
			user.Email = Email;
			user.Password = Password;

			ResponseModel<UserProfile> _TUserProfile = new UserDAL().CheckUser(user);
			if (_TUserProfile.Status == true)
			{
				viewModel.Navigation.PushAsync(new DashBoard());
			}
			else
				viewModel.DisplayAlert("Error", _TUserProfile.Message, "okay");
		}
		private void ForgotPasswordEvent()
		{
		}
	}
}
