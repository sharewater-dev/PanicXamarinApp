using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [UIBasicInfo(IsVisible = false)]
    public class VirtualInput : EndpointIn, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        Endpoints.Out.VirtualOutput.VirtualIOMsg Data = null;
        //------------------------------------------------------------------------------------------------------------------------
        public UInt64 LatestRevision;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfoAttribute(IsVisible = false)] //use attribute to allow property to be set by descriptor
        public NodeKey AuthorizedNodeKey;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public VirtualInput(int InputCount, int OutputCount) //InputCount is not used.. it's there for Activator to find constructor
                : this(OutputCount)
        { }
        //------------------------------------------------------------------------------------------------------------------------
        public VirtualInput(int OutputCount)
                : base(0, OutputCount)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void InternalResetState()
        {
            base.InternalResetState();
            Data = null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            //Mark as uninitialized
            //(this block will receive the first initialization data when the graph on the other end finished warmup solve)
            MarkUninitialized();
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //process data
            if (Data != null)
            {
                try
                {
                    //keep revision
                    LatestRevision = Data.Revision;

                    //apply data to outputs
                    for (int n = 0; n < Data.Indices.Length; n++)
                        _Outputs[Data.Indices[n]].Value = Data.Values[n];
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "Unhandled exception");
                }
            }

            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleExternalActionRequest(ExternalActionRequestInfo RequestInfo)
        {
            base.HandleExternalActionRequest(RequestInfo);

            //get action data
            Data = RequestInfo.ActionData as Endpoints.Out.VirtualOutput.VirtualIOMsg;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
