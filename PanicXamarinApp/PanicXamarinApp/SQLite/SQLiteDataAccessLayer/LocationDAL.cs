using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PanicXamarinApp.SQLite.SQLiteDataAccessLayer
{
    public class LocationDAL
    {
        SQLiteConnection database;
        public LocationDAL()
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

        public ResponseModel<Location> Add(Location entity)
        {            
            ResponseModel<Location> _location = new ResponseModel<Location>();
            try
            {
                var record = database.Insert(entity);
                if (record > 0)
                {
                    _location.Message = "Record has been inserted";
                    _location.Status = true;                
                }
                else
                {
                    _location.Message = "Something went wrong";
                    _location.Status = false;
                }
            }
            catch (Exception ex)
            {
                _location.Message = ex.Message;
                _location.Status = false;
            }
            return _location;
        }
    }
}
