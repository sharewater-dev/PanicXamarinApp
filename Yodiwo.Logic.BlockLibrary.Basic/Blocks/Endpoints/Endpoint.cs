using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints
{
    public abstract class Endpoint : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public override bool IsEndpoint { get { return true; } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Endpoint(int InputCount, int OutputCount)
            : base(InputCount, OutputCount)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
