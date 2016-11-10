using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PanicXamarinApp.CommonLayer
{
    public static class CommonUtility
    {
        public static void LoasUserDetails(UserProfile profile)
        {
            Application.Current.Properties["UserId"] = profile.Id;
            Application.Current.Properties["Email"] = profile.Email;
            Application.Current.Properties["Name"] = profile.Name;
            Application.Current.Properties["VehicleModel"] = profile.VehicleModel;
            Application.Current.Properties["VehicleColor"] = profile.VehicleColor;
            Application.Current.Properties["VehicleRegistation"] = profile.VehicleRegistation;
        }
        public static UserProfile GetUserProfile()
        {
            if (Application.Current.Properties.ContainsKey("UserId"))
            {
                UserProfile profile = new UserProfile();
                profile.Id = string.IsNullOrEmpty(Application.Current.Properties["UserId"].ToString()) ? Guid.Empty : (Guid)Application.Current.Properties["UserId"];
                profile.Email = string.IsNullOrEmpty(Application.Current.Properties["Email"].ToString()) ? string.Empty : Application.Current.Properties["Email"].ToString();
                profile.Name = string.IsNullOrEmpty(Application.Current.Properties["Name"].ToString()) ? string.Empty : (string)Application.Current.Properties["Name"];
                profile.VehicleModel = string.IsNullOrEmpty(Application.Current.Properties["VehicleModel"].ToString()) ? string.Empty : (string)Application.Current.Properties["VehicleModel"];
                profile.VehicleColor = string.IsNullOrEmpty(Application.Current.Properties["VehicleColor"].ToString()) ? string.Empty : (string)Application.Current.Properties["VehicleColor"];
                profile.VehicleRegistation = string.IsNullOrEmpty(Application.Current.Properties["VehicleRegistation"].ToString()) ? string.Empty : (string)Application.Current.Properties["VehicleRegistation"];
                return profile;
            }
            return null;        
        }

    }
}
