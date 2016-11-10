using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Things
{
    public static class Helper
    {
        // Dictionary for YThing class instantiation selection 
        public static Dictionary<string, TupleS<Type, Type>> NonGenericTypesToClass = new Dictionary<string, TupleS<Type, Type>>();
    }
}
