using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.BlockLibrary.Basic
{
    public class Librarian : Yodiwo.Logic.Librarian
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        public Action<Blocks.Endpoints.Out.VirtualOutput.VirtualIOMsg> VirtualOutputMsgHandler = null;
        public Action<Blocks.Endpoints.Out.VirtualOutput.VirtualIOMsg[]> VirtualOutputBatchMsgHandler = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Librarian(GraphManager GraphManager)
            : base(GraphManager)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override void _Initialize()
        {
            base._Initialize();

        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void _Deinitialize()
        {
            base._Deinitialize();

        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
