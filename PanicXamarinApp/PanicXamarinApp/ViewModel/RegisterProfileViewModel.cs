using PanicXamarinApp.View;
using PanicXamarinApp.EntityModel;
using PanicXamarinApp.SQLite.SQLiteDataAccessLayer;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using PanicXamarinApp.CommonLayer;

namespace PanicXamarinApp.ViewModel
{
    public class RegisterProfileViewModel : BaseNavigationViewModel
    {
        #region Local Instance 
        public RegisterProfile viewModel;
        public List<EmergencyContacts> _EmergencyContactsList = new List<EmergencyContacts>();
        private string _vehicleModel;
        private string _vehicleColor;
        private string _vehicleRegistation;
        private string _name;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _Val_vehicleModel;
        private string _V_vehicleColor;
        private string _V_vehicleRegistation;
        #endregion

        #region Properties
        public string VehicleModel
        {
            get { return _vehicleModel; }
            set
            {
                _vehicleModel = value;
                OnPropertyChanged("VehicleModel");
            }
        }
        public string VehicleColor
        {
            get { return _vehicleColor; }
            set
            {
                _vehicleColor = value;
                OnPropertyChanged("VehicleColor");
            }
        }
        public string VehicleRegistation
        {
            get { return _vehicleRegistation; }
            set
            {
                _vehicleRegistation = value;
                OnPropertyChanged("VehicleRegistation");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
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
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged("ConfirmPassword");
            }
        }
        public string Val_vehicleModel
        {
            get { return _Val_vehicleModel; }
            set
            {
                _Val_vehicleModel = value;
                OnPropertyChanged("Val_vehicleModel");
            }
        }
        public string V_vehicleColor
        {
            get { return _V_vehicleColor; }
            set
            {
                _V_vehicleColor = value;
                OnPropertyChanged("V_vehicleColor");
            }
        }
        public string V_vehicleRegistation
        {
            get { return _V_vehicleRegistation; }
            set
            {
                _V_vehicleRegistation = value;

                OnPropertyChanged("V_vehicleRegistation");
            }
        }
        #endregion

        #region Events
        public ICommand _submitCommand;
        public ICommand SubmitCommand
        {
            private set { _submitCommand = value; OnPropertyChanged("SubmitCommand"); }
            get { return _submitCommand; }
        }

        public ICommand _addICECommand;
        public ICommand AddICECommand
        {
            private set { _addICECommand = value; OnPropertyChanged("AddICECommand"); }
            get { return _addICECommand; }
        }

        public ICommand _backCommand;
        public ICommand BackCommand
        {
            private set { _backCommand = value; OnPropertyChanged("BackCommand"); }
            get { return _backCommand; }
        }
        #endregion

        #region RegisterProfileViewModel
        public RegisterProfileViewModel(RegisterProfile view)
        {
            viewModel = view;
            SubmitCommand = new Command(SubmitEvent);
            BackCommand = new Command(BackEvent);
            AddICECommand = new Command(AddICEEvent);
        }
        #endregion

        #region Functions 

        private void AddICEEvent()
        {
            viewModel.Navigation.PushAsync(new AddEmergencyContact(this));
        }

        private async void BackEvent()
        {
            await viewModel.Navigation.PopAsync();
        }

        private async void SubmitEvent()
        {
            Validation();
            //if (test.Result==true)
            //{
            //	SaveUserProfile();
            //}		
        }

        private async void Validation()
        {
            bool status = true;
            if (String.IsNullOrEmpty(VehicleModel))
            {
                Val_vehicleModel = "*";
                status = false;
            }
            else
                Val_vehicleModel = "";
            if (String.IsNullOrEmpty(VehicleColor))
            {
                V_vehicleColor = "*";
                status = false;
            }
            else
                V_vehicleColor = "";

            if (String.IsNullOrEmpty(VehicleRegistation))
            {
                V_vehicleRegistation = "*";
                status = false;
            }
            else
                V_vehicleColor = "";

            if (String.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                await viewModel.DisplayAlert("Warning", "Please enter password or Confirm Password", "OKay");
                status = false;
            }
            else if (!Password.Equals(ConfirmPassword))
            {
                await viewModel.DisplayAlert("Warning", "Password does not match", "OKay");
                status = false;
            }
            if (status)
                SaveUserProfile();

        }

        #endregion

        #region SQL-DB Operation
        public async void SaveUserProfile()
        {
            UserProfile profile = new UserProfile();
            profile.Name = Name;
            profile.Email = Email;
            profile.VehicleModel = VehicleModel;
            profile.VehicleColor = VehicleColor;
            profile.VehicleRegistation = VehicleRegistation;
            profile.Password = Password;
            profile.Id = Guid.NewGuid();

            ResponseModel<UserProfile> _user = new UserDAL().CheckUser(profile);

            if(_user.Status== false)
            {
                ResponseModel<UserProfile> _TUserProfile = new UserDAL().RegisterUserProfile(profile, _EmergencyContactsList);
                if (_TUserProfile.Status == true)
                {
                    CommonUtility.LoasUserDetails(profile);
                    await viewModel.DisplayAlert("Message", _TUserProfile.Message, "okay");
                    await viewModel.Navigation.PopAsync();
                }
            }
            else
            {
                await viewModel.DisplayAlert("Message", "user already exists", "okay");
            }
           
        }

        #endregion
    }
}
