using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints
{
    public abstract class EndpointOut : Endpoint
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public override bool IsEndpointIn { get { return false; } }
        public override bool IsEndpointOut { get { return true; } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public EndpointOut(int InputCount, int OutputCount)
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
