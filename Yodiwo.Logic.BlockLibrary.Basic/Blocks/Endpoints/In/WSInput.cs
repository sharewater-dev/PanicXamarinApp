using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yodiwo.API.Plegma;
using Yodiwo.Logic.SubSystems;

namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "WebSocket In",
                 StencilCategory = UIStencilCategory.Input,
                 Description = "Sends a message throught a websocket",
                 FriendlyImageSource = "https://www.pubnub.com/blog/wp-content/uploads/2013/09/WebSocketsLogo.png")]
    public class WSInput : EndpointIn, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputMsg { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = false,
            FriendlyName = "Message input field",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Message to send")]
        public string Message = "";
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = @"### __WebSocket In__
This block sends a message throught a websocket.
 -- -
*Message to send:*
> In this field the user can specify a message and send it via the websocket by pressing the 'send' button.
"
        };
        //------------------------------------------------------------------------------------------------------------------------
        string recv_msg = string.Empty;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public WSInput()
            : base(0, 1)
        {
            //setup Output IO
            OutputMsg.Name = "Message";
            OutputMsg.Description = "Message";
            OutputMsg.Value = string.Empty;
            OutputMsg.IOType = typeof(string);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void InternalResetState()
        {
            base.InternalResetState();
            recv_msg = string.Empty;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            OutputMsg.Value = recv_msg ?? string.Empty;

            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleExternalActionRequest(ExternalActionRequestInfo RequestInfo)
        {
            base.HandleExternalActionRequest(RequestInfo);

            recv_msg = RequestInfo.ActionData as string;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

    public class WsBlockMessage
    {
        public GraphDescriptorKey GraphDescriptorKey;
        public int BlockId;
        public string Message;
    }
}
