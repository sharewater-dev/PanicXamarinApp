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
    public class LandingPageViewModel : BaseNavigationViewModel
    {
        #region Local Instance 
        public LandingPage viewModel;
        public ICommand _AccidentAssistButton;
        public ICommand _PointsOfInterestButton;
        public ICommand _AAeCARDButton;
        public ICommand _ReminderServicesButton;
        public ICommand _BookingsButton;
        public ICommand _TrackMyJourneyButton;
        public ICommand _UpdateUserDetail;
        #endregion

        #region Properties
        public ICommand AccidentAssistButton { get { return _AccidentAssistButton; } set { _AccidentAssistButton = value;OnPropertyChanged("AccidentAssistButton"); } }
        public ICommand PointsOfInterestButton { get { return _PointsOfInterestButton; } set { _PointsOfInterestButton = value; OnPropertyChanged("PointsOfInterestButton"); } }
        public ICommand AAeCARDButton { get { return _AAeCARDButton; } set { _AAeCARDButton = value; OnPropertyChanged("AAeCARDButton"); } }
        public ICommand ReminderServicesButton { get { return _ReminderServicesButton; } set { _ReminderServicesButton = value; OnPropertyChanged("ReminderServicesButton"); } }
        public ICommand BookingsButton { get { return _BookingsButton; } set { _BookingsButton = value; OnPropertyChanged("BookingsButton"); } }
        public ICommand TrackMyJourneyButton { get { return _TrackMyJourneyButton; } set { _TrackMyJourneyButton = value; OnPropertyChanged("TrackMyJourneyButton"); } }

        public ICommand UpdateUserDetail { get { return _UpdateUserDetail; } set { _UpdateUserDetail = value; OnPropertyChanged("UpdateUserDetail"); } }

        #endregion

        #region LandingPageViewModel
        public LandingPageViewModel(LandingPage viewModel)
        {
            this.viewModel = viewModel;
            AccidentAssistButton = new Command(AccidentAssistEvent);
            PointsOfInterestButton = new Command(PointsOfInterestEvent);
            AAeCARDButton = new Command(AAeCARDButtonEvent);
            ReminderServicesButton = new Command(ReminderServicesEvent);
            BookingsButton = new Command(BookingsEvent);
            TrackMyJourneyButton = new Command(TrackMyJourneyEvent);
            UpdateUserDetail = new Command(UpdateUserDetailEvent);

        }
        #endregion

        #region Function's

        public void UpdateUserDetailEvent()
        {

        }
        public void AccidentAssistEvent() {
            viewModel.Navigation.PushAsync(new AccidentDetail());
        }
        public void PointsOfInterestEvent() { }
        public void AAeCARDButtonEvent() { }
        public void ReminderServicesEvent() { }
        public void BookingsEvent() { }
        public void TrackMyJourneyEvent() { }
        #endregion
    }
}
