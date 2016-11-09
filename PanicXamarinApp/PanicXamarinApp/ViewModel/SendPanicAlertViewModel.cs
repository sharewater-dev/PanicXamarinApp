
using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.EntityModel;
using PanicXamarinApp.SQLite.SQLiteDataAccessLayer;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using PanicXamarinApp.View;
using Plugin.DeviceInfo;
using Plugin.Geolocator;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Connectivity;
using Android;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

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
        private dynamic _sendLocationCounter = 180;
        private string _message1 = "A PANIC ALERT WILL BE SENT IN";
        private string _message2 = "SECONDS WITH YOUR LOCATION AND DETAILS";
        private string _textCancelButton = "Cancel";
        private SendPanicAlert view;     
        public bool _isequestCancel = false;
        UserDeviceModel deviceIdentifier;
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
            set
            {
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
            DetectRescueRequest();
        }

        public async void StartCounter()
        {
            while (SendLocationCounter > 0)
            {
                await Task.Delay(1000);
                SendLocationCounter = SendLocationCounter <= 0 ? 0 : SendLocationCounter - 1;
            }
        }
        public async void DetectRescueRequest()
        {
            try
            {
                await Task.Delay(3000);
                // Get the Latitude and Longitude of the Current User
                if (CrossConnectivity.Current.IsConnected)
                {
                    var locationPermissionStatus = PermissionStatus.Unknown;
                    locationPermissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                    if (locationPermissionStatus == PermissionStatus.Granted) //This always return Granted even if i explicitly revoke from device
                    {
                        DetectLocationAndDeviceInfo();
                    }
                    else 
                    {                   
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                        locationPermissionStatus = results[Permission.Location];
                        if (locationPermissionStatus == PermissionStatus.Granted)
                        {
                            DetectLocationAndDeviceInfo();
                        }
                    }                 
                }
                else
                {
                    await view.DisplayAlert("Error!!", "Internet connectivity is not detected.", "okay");
                    await view.Navigation.PopAsync();
                }
            }

            catch (Exception ex)
            {
                await view.DisplayAlert("Alert!!", "GPS Location is disabled. Please enable and try again.", "okay");
            }

        }     

        public async void DetectLocationAndDeviceInfo()
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 10;
                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
                if (position != null)
                {
                    _location.CreatedOn = System.DateTime.Now;
                    _location.Latitude = position.Latitude;
                    _location.Longitude = position.Longitude;
                    var PhonePermissionStatus = PermissionStatus.Unknown;
                    PhonePermissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone);

                    if (PhonePermissionStatus == PermissionStatus.Granted)
                    {
                        IDevice device = DependencyService.Get<IDevice>();
                        deviceIdentifier = device.GetIdentifier(0);
                        SaveRescueRequest();
                    }
                    else
                    {
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Phone);
                        PhonePermissionStatus = results[Permission.Phone];
                        if (PhonePermissionStatus == PermissionStatus.Granted)
                        {
                            IDevice device = DependencyService.Get<IDevice>();
                            deviceIdentifier = device.GetIdentifier(0);
                            SaveRescueRequest();
                        }
                    }
                }
                else
                {
                    await view.DisplayAlert("Alert!!", "Location is not detected. Please try again", "okay");
                }
            }
            catch (Exception ex)
            {
                await view.DisplayAlert("Alert!!", "GPS Location is disabled. Please enable and try again", "okay");
            }
        }

        public async void SaveRescueRequest()
        {
            if (deviceIdentifier != null)
            {
                if (Device.OS == TargetPlatform.Android)
                {
                    _rescue.MSISDN = deviceIdentifier.DeviceInformation.PhoneNumber;
                    _rescue.IMEI = deviceIdentifier.DeviceInformation.IMEI;
                }
                else if (Device.OS == TargetPlatform.iOS)
                {
                    _rescue.MSISDN = "Apple can't shared phone number";
                    _rescue.UniqueId = deviceIdentifier.DeviceInformation.UniqueID;
                }          

                _location.Id = Guid.NewGuid();
                ResponseModel<Location> _TLocation = AddLocation();
                if (_TLocation != null && _TLocation.Status == true && !IsRequestCancel)
                {
                    _rescue.CreatedOn = System.DateTime.Now;
                    _rescue.LocationId = _location.Id;
                    _rescue.PriorityTypeId = Guid.NewGuid();
                    if (GetDeviceInfo())
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
                            await view.DisplayAlert("Location ", "Latitude : " + _location.Latitude + "\n  Longitude :" + _location.Longitude, "Okay");

                            if (Device.OS == TargetPlatform.Android)
                            {
                                await view.DisplayAlert("Phone Number ", "Phone Number : " + _rescue.MSISDN + "\n  IMEI :" + _rescue.IMEI, "Okay");
                            }
                            else if (Device.OS == TargetPlatform.iOS)
                            {
                                await view.DisplayAlert("Phone Number ", "Phone Number : " + _rescue.MSISDN + "\n  IMEI : Apple not sharing IMEI Number" + "\n  UniqueId :" + _rescue.UniqueId, "Okay");
                            }
                            _rescue.Model = _deviceInfo.Model;
                            _rescue.Platform = _deviceInfo.Platform;
                            _rescue.DeviceVersion = _deviceInfo.DVersion;
                            _rescue.VersionNumber = _deviceInfo.VersionNumber;
                            await view.DisplayAlert("Device Info", " Model : " + _rescue.Model + " \n Platform : " + _rescue.Platform + "\n DeviceVersion : " + _rescue.DeviceVersion + "\n VersionNumber : " + _rescue.VersionNumber, "Okay");
                        }
                        else
                        {
                            SendLocationCounter = 2;
                            await view.DisplayAlert("Alert!!", "Request is not completed. Please try again", "okay");
                        }
                    }
                    else
                    {
                        SendLocationCounter = 2;
                        await view.DisplayAlert("Alert!!", "Request is not completed. Please try again", "okay");
                    }
                }
                else
                {
                    SendLocationCounter = 2;
                    if (IsRequestCancel)
                        await view.DisplayAlert("Alert!!", "You have been cancelled the request.", "okay");
                    else
                        await view.DisplayAlert("Alert!!", "Location is not detected. Please try again", "okay");
                }
            }
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


                Label test1 = new Label { Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId() };
                Label test2 = new Label { Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId(true) };
                Label test3 = new Label { Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId(true, "hello") };
                Label test4 = new Label { Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId(true, "hello", "world") };
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
