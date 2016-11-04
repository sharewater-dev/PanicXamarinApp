using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.SQLite.SQLiteEntityLayer
{
    public class BaseEntity
    {
        public int rowid { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedBy { get; set; }
        public Guid? ModifiedOn { get; set; }

        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
