using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.Logic.Blocks.Endpoints.Out
{
    [UIBasicInfo(IsVisible = false)]
    public class VirtualOutput : EndpointOut
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public class VirtualIOMsg
        {
            public BlockKey RemoteVirtualInputBlockKey;
            public byte[] Indices;
            public object[] Values;
            public UInt64 Revision;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public UInt64 Revision = 0;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfoAttribute(IsVisible = false)] //use attribute to allow property to be set by descriptor
        public int RemoteVirtualInputBlockID;
        [UIBasicInfoAttribute(IsVisible = false)] //use attribute to allow property to be set by descriptor
        public uint RemoteVirtualInputNodeID;
        [UIBasicInfoAttribute(IsVisible = false)] //use attribute to allow property to be set by descriptor
        public uint BrotherNodeID;
        //------------------------------------------------------------------------------------------------------------------------
        public BlockKey RemoteVirtualInputBlockKey
        {
            get
            {
                var bk = BlockKey;
                bk.GraphKey.NodeId = RemoteVirtualInputNodeID;
                bk.BlockId = RemoteVirtualInputBlockID;
                return bk;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public VirtualOutput(int InputCount, int OutputCount) //OutputCount is not used.. it's there for Activator to find constructor
            : this(InputCount)
        { }
        //------------------------------------------------------------------------------------------------------------------------
        public VirtualOutput(int InputCount)
            : base(InputCount, 0)
        {
            foreach (var inp in _Inputs)
            {
                inp.Name = "";
                inp.Description = "";
                inp.IOType = typeof(object);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            try
            {
                //collect touched inputs
                var cnt = _Inputs.Count(io => io.State == IOState.Touched);

                //setup message
                var msg = new VirtualIOMsg()
                {
                    Indices = new byte[cnt],
                    Values = new object[cnt],
                };
                var ind = 0;
                for (int n = 0; n < _Inputs.Length; n++)
                    if (_Inputs[n].State == IOState.Touched)
                    {
                        msg.Indices[ind] = (byte)n;
                        msg.Values[ind] = _Inputs[n].Value;
                        msg.RemoteVirtualInputBlockKey = RemoteVirtualInputBlockKey;
                        ind++;
                    }

                //send message
                GraphManager.GetLibrarian<BlockLibrary.Basic.Librarian>().VirtualOutputMsgHandler?.Invoke(msg);
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); }

            //increase revision
            Revision++;

            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}

