using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.Logic.Blocks.Endpoints.Out
{
    [UIBasicInfo(IsVisible = false)]
    public class VirtualOutputCollector : EndpointOut
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public VirtualOutputCollector(int InputCount, int OutputCount) //OutputCount is not used.. it's there for Activator to find constructor
            : this(InputCount)
        { }
        //------------------------------------------------------------------------------------------------------------------------
        public VirtualOutputCollector(int InputCount)
            : base(InputCount, 0)
        {
            foreach (var inp in _Inputs)
            {
                inp.Name = "";
                inp.Description = "";
                inp.IOType = typeof(VirtualOutput.VirtualIOMsg);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //collect touched inputs
            var cnt = _Inputs.Count(io => io.State == IOState.Touched);
            //setup message
            var msges = new VirtualOutput.VirtualIOMsg[cnt];
            var ind = 0;
            for (int n = 0; n < _Inputs.Length; n++)
                if (_Inputs[n].State == IOState.Touched)
                {
                    msges[ind] = _Inputs[n].Value as VirtualOutput.VirtualIOMsg;
                    ind++;
                }
            //send message
            var handler = GraphManager.GetLibrarian<BlockLibrary.Basic.Librarian>().VirtualOutputBatchMsgHandler;
            if (handler != null)
                handler(msges);
            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}

