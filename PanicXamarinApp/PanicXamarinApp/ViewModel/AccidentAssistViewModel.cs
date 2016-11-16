using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PanicXamarinApp.ViewModel
{
    public class AccidentAssistViewModel : BaseNavigationViewModel
    {
        #region Local Instance 
        public ICommand _AccidentReportButton;
        public ICommand _WhatToDoButton;
        public ICommand _CallPoliceButton;
        public ICommand _CallAAButton;
        #endregion

        #region Properties
        public ICommand AccidentReportButton { get { return _AccidentReportButton; } set { _AccidentReportButton = value; OnPropertyChanged("AccidentReportButton"); } }
        public ICommand WhatToDoButton { get { return _WhatToDoButton; } set { _WhatToDoButton = value; OnPropertyChanged("WhatToDoButton"); } }
        public ICommand CallPoliceButton { get { return _CallPoliceButton; } set { _CallPoliceButton = value; OnPropertyChanged("CallPoliceButton"); } }
        public ICommand CallAAButton { get { return _CallAAButton; } set { _CallAAButton = value; OnPropertyChanged("CallAAButton"); } }
        #endregion

        #region AccidentAssistViewModel
        public AccidentAssistViewModel()
        {
            AccidentReportButton = new Command(AccidentReportEvent);
            WhatToDoButton = new Command(WhatToDoEvent);
            CallPoliceButton = new Command(CallPoliceEvent);
            CallAAButton = new Command(CallAAEvent);

        }
        #endregion AccidentAssistViewModel

        #region Function's
        public void AccidentReportEvent()        {        }
        public void WhatToDoEvent() { }
        public void CallPoliceEvent() { }
        public void CallAAEvent() { }
        #endregion

    }
}
