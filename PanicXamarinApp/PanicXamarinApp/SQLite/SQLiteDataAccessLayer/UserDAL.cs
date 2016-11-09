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
					_userProfile.Message = "User has been successfully registered !!";
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
	}
}
