using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.SQLite.SQLiteEntityLayer
{
    public class ResponseModel<T>
    {
        public T ObjModel { get; set; }
        public List<T> ObjListModel { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
