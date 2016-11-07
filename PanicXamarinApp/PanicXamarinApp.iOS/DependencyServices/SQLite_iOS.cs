using PanicXamarinApp.DependencyServices;
using PanicXamarinApp.iOS.DependencyServices;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_iOS))]
namespace PanicXamarinApp.iOS.DependencyServices
{
    public class SQLite_iOS : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            try
            {
                var sqliteFilename = "SQLite1.db3";
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
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
