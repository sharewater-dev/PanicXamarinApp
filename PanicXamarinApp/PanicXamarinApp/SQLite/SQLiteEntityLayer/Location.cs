using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.SQLite.SQLiteEntityLayer
{
    public class Location : BaseEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
