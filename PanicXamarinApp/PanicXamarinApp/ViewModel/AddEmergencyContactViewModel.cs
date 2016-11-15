using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using PanicXamarinApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PanicXamarinApp.ViewModel
{
    public class AddEmergencyContactViewModel : BaseNavigationViewModel
    {
        #region Local Instance 
        AddEmergencyContact _viewModel;
        public List<EmergencyContacts> _emergencyContactsList;
        public string _First_EC = string.Empty;
        public string _Second_EC = string.Empty;
        public string _Third_EC = string.Empty;
        public string _Salutation = string.Empty;
        public string _FirstName = string.Empty;
        public string _SurName = string.Empty;
        public string _PhoneNumber = string.Empty;
        public string _RelationShip = string.Empty;
        public int _SaturationSelectedIndex = 0;
        public int _RelationShipSelectedIndex = 0;
        #endregion

        #region Properties    
        public string First_EC { get { return _First_EC; } set { _First_EC = value; OnPropertyChanged("First_EC"); } }
        public string Second_EC { get { return _Second_EC; } set { _Second_EC = value; OnPropertyChanged("Second_EC"); } }
        public string Third_EC { get { return _Third_EC; } set { _Third_EC = value; OnPropertyChanged("Third_EC"); } }
        public string Salutation { get { return _Salutation; } set { _Salutation = value; OnPropertyChanged("Title"); } }
        public string FirstName { get { return _FirstName; } set { _FirstName = value; OnPropertyChanged("FirstName"); } }
        public string SurName { get { return _SurName; } set { _SurName = value; OnPropertyChanged("SurName"); } }
        public string PhoneNumber { get { return _PhoneNumber; } set { _PhoneNumber = value; OnPropertyChanged("PhoneNumber"); } }
        public string RelationShip { get { return _RelationShip; } set { _RelationShip = value; OnPropertyChanged("RelationShip"); } }

        public int SaturationSelectedIndex { get { return _SaturationSelectedIndex; } set { _SaturationSelectedIndex = value; OnPropertyChanged("SaturationSelectedIndex"); } }

        public int RelationShipSelectedIndex { get { return _RelationShipSelectedIndex; } set { _RelationShipSelectedIndex = value;OnPropertyChanged("RelationShipSelectedIndex"); } }
        #endregion

        #region Events
        public ICommand _saveButton;
        public ICommand SaveButton { get { return _saveButton; } private set { _saveButton = value; OnPropertyChanged("SaveButton"); } }
        public ICommand _backButton;
        public ICommand BackButton { get { return _backButton; } private set { _backButton = value; OnPropertyChanged("BackButton"); } }
        #endregion

        #region AddEmergencyContactViewModel
        public AddEmergencyContactViewModel(AddEmergencyContact viewModel)
        {
            this._viewModel = viewModel;
            SaveButton = new Command(SaveEvent);
            BackButton = new Command(BackEvent);
            _emergencyContactsList = new List<EmergencyContacts>();
        }
        #endregion

        #region Functions
        public void SaveEvent()
        {
            if (!Validation())
                return;
            if (_emergencyContactsList.Count != 3)
            {
                EmergencyContacts conatct = new EmergencyContacts();
                conatct.Salutation = Salutation;
                conatct.FirstName = FirstName;
                conatct.LastName = SurName;
                conatct.PhoneNumber = PhoneNumber;
                conatct.RelationShip = RelationShip;
                _emergencyContactsList.Add(conatct);
                LoadData();
            }
            else
                _viewModel.DisplayAlert("Error", "You can't add more than 3 emergency contact", "okay");

        }

        public void LoadData()
        {
            Salutation = "";
            FirstName = "";
            SurName = "";
            PhoneNumber = "";
            RelationShip = "";
            SaturationSelectedIndex = 0;
            RelationShipSelectedIndex = 0;
            if (_emergencyContactsList.Count == 1)
            {
                First_EC = string.Format("{0} {1}", _emergencyContactsList[0].FirstName, _emergencyContactsList[0].LastName);
            }
            else if (_emergencyContactsList.Count == 2)
            {
                First_EC = string.Format("{0} {1}", _emergencyContactsList[0].FirstName, _emergencyContactsList[0].LastName);
                Second_EC = string.Format("{0} {1}", _emergencyContactsList[1].FirstName, _emergencyContactsList[1].LastName);
            }
            else if (_emergencyContactsList.Count == 3)
            {
                First_EC = string.Format("{0} {1}", _emergencyContactsList[0].FirstName, _emergencyContactsList[0].LastName);
                Second_EC = string.Format("{0} {1}", _emergencyContactsList[1].FirstName, _emergencyContactsList[1].LastName);
                Third_EC = string.Format("{0} {1}", _emergencyContactsList[2].FirstName, _emergencyContactsList[2].LastName);
            }
        }
        public bool Validation()
        {
            bool IsValid = false;
            if (string.IsNullOrEmpty(Salutation) || Salutation.Contains("SELECT"))
                _viewModel.DisplayAlert("Validaion ", "Please select Salutation", "Okay");
            else if (string.IsNullOrEmpty(FirstName))
                _viewModel.DisplayAlert("Validaion ", "Please enter firstname", "Okay");
            else if (string.IsNullOrEmpty(SurName))
                _viewModel.DisplayAlert("Validaion ", "Please enter surname", "Okay");
            else if (string.IsNullOrEmpty(PhoneNumber))
                _viewModel.DisplayAlert("Validaion ", "Please enter phonenumber", "Okay");
            else if (string.IsNullOrEmpty(RelationShip) || RelationShip.Contains("SELECT"))
                _viewModel.DisplayAlert("Validaion ", "Please select relationship", "Okay");
            else
                IsValid = true;
            return IsValid;
        }

        public void BackEvent()
        {
            _viewModel.Navigation.PopAsync();
        }
        #endregion
    }
}
