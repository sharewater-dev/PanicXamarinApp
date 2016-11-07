
using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.EntityModel;
using PanicXamarinApp.SQLite.SQLiteDataAccessLayer;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using PanicXamarinApp.View;
using Plugin.DeviceInfo;
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
        private DeviceInfo _deviceInfo = new DeviceInfo();
        
        #endregion

        #region Private Fields
        private dynamic _sendLocationCounter = 10;
        private string _message1 = "A PANIC ALERT WILL BE SENT IN";
        private string _message2 = "SECONDS WITH YOUR LOCATION AND DETAILS";
        private string _textCancelButton = "Cancel";
        private SendPanicAlert view;
        public bool _isequestCancel = false;
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
        public bool IsRequestCancel
        {
            get { return _isequestCancel; }
            set {
                _isequestCancel = value;
                OnPropertyChanged("IsequestCancel");
            }
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
                SendLocationCounter = SendLocationCounter <= 0 ? 0: SendLocationCounter - 1;
            }
        }
        public async void GetCurrentLocation()
        {
            try
            {
               await Task.Delay(3000);
                // Get the Latitude and Longitude of the Current User
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;                    
                var position = await locator.GetPositionAsync(100000);
                             
                if (position != null)
                {
                    var test = GetDeviceUniqueId();
                    if (Device.OS == TargetPlatform.Android)
                    {                    
                        _rescue.MSISDN = test.DeviceInformation.PhoneNumber;
                        _rescue.IMEI = test.DeviceInformation.IMEI;
                    }
                    else if (Device.OS == TargetPlatform.iOS)
                    {                      
                        _rescue.MSISDN = "Apple can't shared phone number";
                        _rescue.UniqueId = test.DeviceInformation.UniqueID;
                    }
                    _location.CreatedOn = System.DateTime.Now;
                    _location.Latitude = position.Latitude;
                    _location.Longitude = position.Longitude;

                    _location.Id = Guid.NewGuid();
                    ResponseModel<Location> _TLocation = AddLocation();
                    if (_TLocation != null && _TLocation.Status == true && !IsRequestCancel)
                    {
                        _rescue.CreatedOn = System.DateTime.Now;
                        _rescue.LocationId = _location.Id;
                        _rescue.PriorityTypeId = Guid.NewGuid();
                        if(GetDeviceInfo())
                        {
                            // _rescue.AppID = _deviceInfo.Id;
                            _rescue.Model = _deviceInfo.Model;
                            _rescue.Platform = _deviceInfo.Platform;
                            _rescue.DeviceVersion = _deviceInfo.DVersion;
                            _rescue.VersionNumber = _deviceInfo.VersionNumber;

                            ResponseModel<Rescue> _TRescue = SaveRescue();
                            if (_TRescue != null && _TRescue.Status == true)
                            {
                                Message1 = "YOUR PANIC ALERT HAS BEEN";
                                Message2 = "WITH YOUR LOCATION. AN OPERATOR WILL CONTACT YOU SHORTLY.";
                                TextCancelButton = "Back";
                                SendLocationCounter = 0;
                                await Task.Delay(1000);
                                SendLocationCounter = "SENT";
                                await view.DisplayAlert("Sucess!!", "Record has been successfully inserted.", "okay");
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
                        if(IsRequestCancel)                       
                            await view.DisplayAlert("Alert!!", "You have been cancelled the request.", "okay");
                        else
                            await view.DisplayAlert("Alert!!", "Location is not detected. Please try again", "okay");
                    }
                }
                else
                {
                    await view.DisplayAlert("Alert!!", "Location is not detected. Please try again", "okay");
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

        public bool GetDeviceInfo()
        {
            try
            {
                _deviceInfo.Id = CrossDeviceInfo.Current.Id;
                _deviceInfo.Model = CrossDeviceInfo.Current.Model;
                Plugin.DeviceInfo.Abstractions.Platform abc = CrossDeviceInfo.Current.Platform;
                _deviceInfo.Platform = CrossDeviceInfo.Current.Platform.ToString();
                _deviceInfo.DVersion = CrossDeviceInfo.Current.Version;
                _deviceInfo.VersionNumber = CrossDeviceInfo.Current.VersionNumber.ToString();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }       
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
            ResponseModel<Location> _TLocation = new LocationDAL().Add(_location);
            return _TLocation;
        }
        #endregion

    }
}
