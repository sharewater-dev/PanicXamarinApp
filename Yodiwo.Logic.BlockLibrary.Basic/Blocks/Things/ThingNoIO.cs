using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yodiwo.Logic;
using Yodiwo.API.Plegma;

namespace Yodiwo.Logic.Blocks.Things
{
    public class ThingNoIO : BaseThings
    {
        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ThingNoIO(Thing thing)
            : base(thing, 0, 0)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
