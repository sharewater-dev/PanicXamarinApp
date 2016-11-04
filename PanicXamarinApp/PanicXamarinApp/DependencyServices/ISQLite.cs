using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.DependencyServices
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
