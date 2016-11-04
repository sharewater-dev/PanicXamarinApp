using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.SQLite.SQLiteEntityLayer
{
    public class PriorityTypes : BaseEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        public Guid Id { get; set; }
        public string PriorityName { get; set; }

    }
}
