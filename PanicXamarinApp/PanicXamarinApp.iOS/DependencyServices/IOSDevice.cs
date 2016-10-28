using Foundation;
using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.EntityModel;
using PanicXamarinApp.iOS.DependencyServices;
using Security;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(IOSDevice))]
namespace PanicXamarinApp.iOS.DependencyServices
{
    public class IOSDevice : IDevice
    {
        public UserDeviceModel GetIdentifier(int simSlot)
        {
            UserDeviceModel info = new UserDeviceModel();
            try
            {

                var UniqueID = UIDevice.CurrentDevice.IdentifierForVendor;
                if (UniqueID != null)
                    info.DeviceInformation.UniqueID = UniqueID.Description;
            }
            catch (Exception)
            {
                info.DeviceInformation.UniqueID = "000000000000";
            }


            //   string device_id = UIDevice.CurrentDevice.IdentifierForVendor?.

            //var query = new SecRecord(SecKind.GenericPassword);
            //query.Service = NSBundle.MainBundle.BundleIdentifier;
            //query.Account = "UniqueID";

            //NSData uniqueId = SecKeyChain.QueryAsData(query);               

            //if (uniqueId == null)
            //{
            //    query.ValueData = NSData.FromString(System.Guid.NewGuid().ToString());
            //    var err = SecKeyChain.Add(query);
            //    if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
            //        throw new Exception("Cannot store Unique ID");

            //    return query.ValueData.ToString();
            //}
            //else
            //{
            //    return uniqueId.ToString();
            //}

            //string serial = string.Empty;
            //uint platformExpert = IOServiceGetMatchingService(0, IOServiceMatching("IOPlatformExpertDevice"));

            //NSString key = (NSString)"IOPlatformSerialNumber";
            //IntPtr serialNumber = IORegistryEntryCreateCFProperty(platformExpert, key.Handle, IntPtr.Zero, 0);
            //if (serialNumber != IntPtr.Zero)
            //{
            //    serial = NSString.FromHandle(serialNumber);
            //}
            //IOObjectRelease(platformExpert);


            return info;
        }
    }
}
