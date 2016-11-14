using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PanicXamarinApp
{
	public class UserDAL
	{
		SQLiteConnection database;
		public UserDAL()
		{
			ConnectToDatabase();
			
		}
		public void ConnectToDatabase()
		{
			try
			{
				database = DependencyService.Get<ISQLite>().GetConnection();
			}
			catch (Exception ex)
			{
			}
		}
		public ResponseModel<UserProfile> RegisterUserProfile(UserProfile entity)
		{
			ResponseModel<UserProfile> _userProfile = new ResponseModel<UserProfile>();
			try
			{
				var record = database.Insert(entity);
				if (record > 0)
				{
					_userProfile.Message = "Profile Successfully created on this device !!";
					_userProfile.Status = true;
				}
				else
				{
					_userProfile.Message = "Something went wrong";
					_userProfile.Status = false;
				}
			}
			catch (Exception ex)
			{
				_userProfile.Message = ex.Message;
				_userProfile.Status = false;
			}
			return _userProfile;
		}


		public ResponseModel<UserProfile> CheckUser(UserProfile entity)
		{
			ResponseModel<UserProfile> _userProfile = new ResponseModel<UserProfile>();
			try
			{
				var record = database.Query<UserProfile>("Select * from UserProfile");
				var secondResult = (from i in database.Table<UserProfile>() where i.Email.ToLower().Equals(entity.Email.ToLower()) && i.Password.Equals(entity.Password) select i).ToList();

				if (secondResult.Count > 0)
				{
					_userProfile.Status = true;
				}
				else
				{
					_userProfile.Message = "Username or password does not match";
					_userProfile.Status = false;
				}
			}
			catch (Exception ex)
			{
				_userProfile.Message = ex.Message;
				_userProfile.Status = false;
			}
			return _userProfile;
		}

        public ResponseModel<bool> IsValidEmail(UserProfile entity)
        {
            ResponseModel<bool> _isValidEmail = new ResponseModel<bool>();
            try
            {
                var record = database.Query<UserProfile>("Select * from UserProfile");
                var secondResult = (from i in database.Table<UserProfile>() where i.Email.ToLower().Equals(entity.Email.ToLower()) select i).ToList();

                if (secondResult.Count > 0)
                {
                    _isValidEmail.Status = true;
                }
                else
                {
                    _isValidEmail.Message = "Email is not registed with is..Please enter valid email address";
                    _isValidEmail.Status = false;
                }
            }
            catch (Exception ex)
            {
                _isValidEmail.Message = ex.Message;
                _isValidEmail.Status = false;
            }
            return _isValidEmail;
        }

    }
}
