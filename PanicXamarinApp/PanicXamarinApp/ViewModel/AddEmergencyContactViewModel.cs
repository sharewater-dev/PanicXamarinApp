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
    public class AddEmergencyContactViewModel :  BaseNavigationViewModel
    {
        AddEmergencyContact _viewModel;
        public string _First_EC = string.Empty;
        public string _Second_EC = string.Empty;
        public string _Third_EC = string.Empty;
        public string _Salutation = string.Empty;
        public string _FirstName = string.Empty;
        public string _SurName = string.Empty;
        public string _PhoneNumber = string.Empty;

        public string _RelationShip = string.Empty;      

        public string First_EC { get { return _First_EC; } set { _First_EC = value;OnPropertyChanged("First_EC"); } }
        public string Second_EC { get { return Second_EC; } set { Second_EC = value; OnPropertyChanged("Second_EC"); } }
        public string Third_EC { get { return Third_EC; } set { Third_EC = value; OnPropertyChanged("Third_EC"); } }

        public string Salutation { get { return _Salutation; } set { _Salutation = value; OnPropertyChanged("Title"); } }
        public string FirstName { get { return _FirstName; } set { _FirstName = value; OnPropertyChanged("FirstName"); } }
        public string SurName { get { return _SurName; } set { _SurName = value; OnPropertyChanged("SurName"); } }
        public string PhoneNumber { get { return _PhoneNumber; } set { _PhoneNumber = value; OnPropertyChanged("PhoneNumber"); } }
        public string RelationShip { get { return _RelationShip; } set { _RelationShip = value; OnPropertyChanged("RelationShip"); } }

        public ICommand SaveButton;
        public ICommand BackButton;

        public AddEmergencyContactViewModel(AddEmergencyContact _viewModel)
        {
            this._viewModel = _viewModel;
            SaveButton = new Command(SaveEvent);
            BackButton = new Command(BackEvent);
        }

        public void SaveEvent()
        {

        }

        public void BackEvent()
        {
            _viewModel.Navigation.PopAsync();
        }
    }
}
