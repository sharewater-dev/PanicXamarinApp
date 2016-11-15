using PanicXamarinApp.DependencyServices;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PanicXamarinApp.SQLite.SQLiteEntityLayer
{
    public class Utility
    {
        SQLiteConnection database;
        public Utility()
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

        public void CreateDatabase()
        {
            try
            {
                database.CreateTable<Location>();
                database.CreateTable<PriorityTypes>();
                database.CreateTable<Rescue>();
                database.CreateTable<UserProfile>();
                database.CreateTable<EmergencyContacts>();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
