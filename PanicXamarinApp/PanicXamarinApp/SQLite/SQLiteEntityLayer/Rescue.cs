using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.SQLite.SQLiteEntityLayer
{
    public class Rescue : BaseEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        public Guid Id { get; set; }
        public int AppID { get; set; }
        public Guid PriorityTypeId { get; set; }
        public Guid LocationId { get; set; }

        // MSISDN = Cell phone number 
        public String MSISDN { get; set; }       

        public string Profile { get; set; }

        public string IMEI { get; set; }

        public string UniqueId { get; set; }

        public string Model { get; set; }
        public string DeviceVersion { get; set; }
        public string VersionNumber { get; set; }

        public string Platform { get; set; }

    }
}
