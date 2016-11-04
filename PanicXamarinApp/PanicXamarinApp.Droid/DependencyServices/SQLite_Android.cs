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
                var sqliteFilename = "TodoSQLite.db3";
                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
                var path = Path.Combine(libraryPath, sqliteFilename);
                // Create the connection
                var conn = new SQLiteConnection(path);
                // Return the database connection
                return conn;

            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}