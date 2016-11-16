using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PanicXamarinApp.ViewModel
{
    public class AccidentDetailViewModel : BaseNavigationViewModel
    {
        #region Local Instance 
        public string _DpDate;
        public string _TpTime;
        public string _Location;
        public string _Description;
        public string _Injuries;
        public string _CharLeft;
        public ICommand _NextButton;
        #endregion

        #region Properties
        public string DpDate { get { return _DpDate; } set { _DpDate = value;OnPropertyChanged("DpDate"); } }
        public string TpTime { get { return _TpTime; } set { _TpTime = value; OnPropertyChanged("TpTime"); } }
        public string Location { get { return _Location; } set { _Location = value; OnPropertyChanged("Location"); } }
        public string Description { get { return _Description; } set { _Description = value; OnPropertyChanged("Description"); } }
        public string Injuries { get { return _Injuries; } set { _Injuries = value; OnPropertyChanged("Injuries"); } }
        public string CharLeft { get { return _CharLeft; } set { _CharLeft = value; OnPropertyChanged("CharLeft"); } }
        public ICommand NextButton { get { return _NextButton; } private set { _NextButton = value;OnPropertyChanged("NextButton"); } }
        #endregion

        #region AccidentDetailViewModel
        public AccidentDetailViewModel()
        {

        }
        #endregion

    }
}
