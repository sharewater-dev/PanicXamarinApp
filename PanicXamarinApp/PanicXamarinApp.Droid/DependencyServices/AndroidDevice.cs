using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using PanicXamarinApp.Droid.DependencyService;
using PanicXamarinApp.DependencyServices;
using Android.Telephony;
using PanicXamarinApp.EntityModel;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]


namespace PanicXamarinApp.Droid.DependencyService
{
    public class AndroidDevice : IDevice
    {
        public UserDeviceModel GetIdentifier(int simSlot)
        {
            UserDeviceModel info = new UserDeviceModel();
            try
            {
             
                TelephonyManager IMEI_telManager = (TelephonyManager)Forms.Context.GetSystemService(Context.TelephonyService);
                if (IMEI_telManager != null)
                {
                    info.DeviceInformation.IMEI = IMEI_telManager.GetDeviceId(simSlot);
                    info.DeviceInformation.PhoneNumber = IMEI_telManager.Line1Number;
                }
            }
            catch (Exception ex)
            {
             
            }
          
            return info;
        }
    }
}