using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints
{
    public abstract class EndpointIn : Endpoint
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public override bool IsEndpointIn { get { return true; } }
        public override bool IsEndpointOut { get { return false; } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public EndpointIn(int InputCount, int OutputCount)
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