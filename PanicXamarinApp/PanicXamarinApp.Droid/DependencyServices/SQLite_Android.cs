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
using PanicXamarinApp.DependencyServices;
using SQLite;
using PanicXamarinApp.Droid.DependencyServices;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]
namespace PanicXamarinApp.Droid.DependencyServices
{
    public class SQLite_Android : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            try
            {
                var sqliteFilename = "SQLite3.db3";                //string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
                //string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
                //var path = Path.Combine(libraryPath, sqliteFilename);


                //var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                //path = Path.Combine(path, sqliteFilename);
                //// Create the connection
                //var conn = new SQLiteConnection(path);
                // Return the database connection


             //   var dbName = "CustomersDb.db3";
                var path = Path.Combine(System.Environment.
                  GetFolderPath(System.Environment.
                  SpecialFolder.Personal), sqliteFilename);
             //   return new SQLiteConnection(path);
                var conn = new SQLiteConnection(path);
               
                return conn;

            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}