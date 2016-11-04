
using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.EntityModel;
using PanicXamarinApp.SQLite.SQLiteDataAccessLayer;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using PanicXamarinApp.View;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PanicXamarinApp.ViewModel
{
    public class SendPanicAlertViewModel : BaseNavigationViewModel
    {
        #region SQL-DB Entity private fields
        private Rescue _rescue = new Rescue();
        private Location _location = new Location();
        #endregion

        #region Private Fields
        private dynamic _sendLocationCounter = 10;
        private string _message1 = "A PANIC ALERT WILL BE SENT IN";
        private string _message2 = "SECONDS WITH YOUR LOCATION AND DETAILS";
        private string _textCancelButton = "Cancel";
        private SendPanicAlert view;
        #endregion

        #region Properties 
        public dynamic SendLocationCounter
        {
            get { return _sendLocationCounter; }
            set
            {
                _sendLocationCounter = value;
                OnPropertyChanged("SendLocationCounter");
            }
        }
        public string Message1
        {
            get { return _message1; }
            set
            {
                _message1 = value;
                OnPropertyChanged("Message1");
            }
        }
        public string Message2
        {
            get { return _message2; }
            set
            {
                _message2 = value;
                OnPropertyChanged("Message2");
            }
        }
        public string TextCancelButton
        {
            get { return _textCancelButton; }
            set { _textCancelButton = value; OnPropertyChanged("TextCancelButton"); }
        }
        #endregion

        #region Functions
        public SendPanicAlertViewModel(SendPanicAlert view)
        {
            this.view = view;
            StartCounter();
            GetCurrentLocation();
        }

        public async void StartCounter()
        {
            while (SendLocationCounter > 0)
            {
                await Task.Delay(1000);
                SendLocationCounter = SendLocationCounter - 1;
            }
        }
        public async void GetCurrentLocation()
        {
            try
            {
                await Task.Delay(10000);
                // Get the Latitude and Longitude of the Current User
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                //  locator.AllowsBackgroundUpdates = true;
                var position = await locator.GetPositionAsync(10000);
                //  string status = "Position Status : " + position.Timestamp; ;
                string status = " Latitude : " + position.Latitude;
                status += ", Longitude : " + position.Longitude;
                if (position != null)
                {
                    var test = GetDeviceUniqueId();
                    if (Device.OS == TargetPlatform.Android)
                    {
                        status += ", IMEI : " + test.DeviceInformation.IMEI;
                        status += ", PhoneNumber : " + test.DeviceInformation.PhoneNumber;
                        _rescue.MSISDN = test.DeviceInformation.PhoneNumber;
                    }
                    else if (Device.OS == TargetPlatform.iOS)
                    {
                        status += ", UniqueID : " + test.DeviceInformation.UniqueID;
                        status += ", PhoneNumber : Apple can't shared phone number";
                        _rescue.MSISDN = "Apple can't shared phone number";
                    }

                    _location.CreatedOn = System.DateTime.Now;
                    _location.Latitude = position.Latitude;
                    _location.Longitude = position.Longitude;
                    _location.Id = Guid.NewGuid();
                     ResponseModel<Location> _TLocation=AddLocation();
                    if(_TLocation != null && _TLocation.Status == true)
                    {
                        _rescue.CreatedOn = System.DateTime.Now;
                        _rescue.LocationId = _location.Id;
                        _rescue.PriorityTypeId = Guid.NewGuid();
                        ResponseModel<Rescue> _TRescue =SaveRescue();
                        if(_TRescue != null && _TRescue.Status == true)
                        {
                            Message1 = "YOUR PANIC ALERT HAS BEEN";
                            Message2 = "WITH YOUR LOCATION. AN OPERATOR WILL CONTACT YOU SHORTLY.";
                            TextCancelButton = "Back";
                            //SendLocationCounter = "SENT";
                        }
                        else
                        {
                            await view.DisplayAlert("Alert!!", "Location is not detected. Please try again", "okay");
                        }
                    }                      
                    else
                    {
                        await view.DisplayAlert("Alert!!", "Location is not detected. Please try again", "okay");
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                await view.DisplayAlert("Alert!!", "GPS Location is disabled. Please enable and try again.", "okay");
            }           
        }

        public UserDeviceModel GetDeviceUniqueId()
        {
            IDevice device = DependencyService.Get<IDevice>();
            UserDeviceModel deviceIdentifier = device.GetIdentifier(0);
            return deviceIdentifier;
        }

        #endregion

        #region SQL-DB Operation
        public ResponseModel<Rescue> SaveRescue()
        {
            ResponseModel<Rescue> _TRescue = new RescueDAL().Add(_rescue);
            return _TRescue;
        }

        public ResponseModel<Location> AddLocation()
        {
            ResponseModel<Location> _TLocation= new LocationDAL().Add(_location);
            return _TLocation;
        }
        #endregion

    }
}
