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
    public class RescueDAL
    {
        SQLiteConnection database;
        public RescueDAL()
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
        public ResponseModel<Rescue> Add(Rescue entity)
        {
            ResponseModel<Rescue> _rescue = new ResponseModel<Rescue>();
            try
            {
                var record = database.Insert(entity);
                if (record > 0)
                {
                    _rescue.Message = "Record has been inserted";
                    _rescue.Status = true;
                }
                else
                {
                    _rescue.Message = "Something went wrong";
                    _rescue.Status = false;
                }
            }
            catch (Exception ex)
            {
                _rescue.Message = ex.Message;
                _rescue.Status = false;
            }
            return _rescue;
        }
    }
}
