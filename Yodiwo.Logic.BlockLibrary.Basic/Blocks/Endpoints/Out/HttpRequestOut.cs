using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace Yodiwo.Logic.Blocks.Endpoints.Out
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "HTTP Out",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Makes an HTTP Request and Outputs the result",
                 FriendlyImageSource = "/Content/img/icons/Generic/http.png")]
    public class HttpRequestOut : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputBody { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputSuccess { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        public OutputIO OutputStatus { get { return _Outputs[1]; } set { _Outputs[1] = value; } }
        public OutputIO OutputBody { get { return _Outputs[2]; } set { _Outputs[2] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "URL",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "The URL to make the request to")]
        public string Url = "http://httpbin.org/post";
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Method",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Http Request Method")]
        public HttpMethods Method = HttpMethods.Post;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Headers",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "HTTP Request Headers")]
        public string Headers = "";
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Data Format",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "The format of the request's body")]
        public HttpRequestDataFormat DataFormat = HttpRequestDataFormat.Json;
        //------------------------------------------------------------------------------------------------------------------------
        //intermediate results
        private Yodiwo.Tools.Http.RequestResult _result;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __HTTP Out__
                
This block makes an HTTP Request and Outputs the result. The HTTP Request body is configured by the input of the block
-- - 
*URL:*
> Configuration of the URL to make the request to.

*Method:*
> Specifies the Http Request Method.
> - POST
> - GET
> - PUT

*Headers:*
> Specifies the HTTP Request Headers.

*Data Format:*
> Specifies the format of the request's body."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public HttpRequestOut()
            : base(1, 3)
        {
            //setup Input IO
            InputBody.IOType = typeof(string);
            InputBody.Name = "Body";
            InputBody.Description = "The Body of the request ...";

            //setup Output IO
            OutputSuccess.Name = "ResponseIsSuccess";
            OutputSuccess.Description = "Response is success";
            OutputSuccess.IOType = typeof(string);
            OutputStatus.Name = "ResponseStatusCode";
            OutputStatus.Description = "Response status code";
            OutputStatus.IOType = typeof(string);
            OutputBody.Name = "ResponseBody";
            OutputBody.Description = "Response body";
            OutputStatus.IOType = typeof(string);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleInternalActionRequest(InternalActionRequestInfo requestInfo)
        {
            base.HandleInternalActionRequest(requestInfo);
            _result = (Yodiwo.Tools.Http.RequestResult)requestInfo.ActionData;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void InternalResetState()
        {
            base.InternalResetState();
            //clear intermediate state
            _result = default(Yodiwo.Tools.Http.RequestResult);
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            DebugEx.TraceLog(Graph.IsWarmupSolve.ToString());
            if (InputBody.IsConnected && InputBody.IsTouched)
            {
                //get headers
                var headerDict = new Dictionary<string, string>();
                try
                {
                    headerDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Headers);
                }
                catch { /*deserialization failed, no headers*/ }

                if (!Graph.IsWarmupSolve)
                {
                    //do request and wait for it
                    Yodiwo.Tools.Http.Request(Method, Url, (string)InputBody.Value, DataFormat, headerDict, null, null, HandleHttpResponse, null);
                    yield return BlockState.OnHold;
                }

                //process results
                if (_result.IsValid)
                {
                    OutputSuccess.Value = _result.IsSuccessStatusCode;
                    OutputStatus.Value = (int)_result.StatusCode;
                    OutputBody.Value = _result.ResponseBodyText ?? "";
                    _result = default(Yodiwo.Tools.Http.RequestResult);
                }
                else
                {
                    OutputSuccess.Value = false;
                    OutputStatus.Value = 0;
                    OutputBody.Value = "";
                }
            }
            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void HandleHttpResponse(Yodiwo.Tools.Http.RequestResult Result)
        {
            GraphManager.RequestInternalGraphAction(this.BlockKey,
                                                    GetInternalActionToken(),
                                                    Result
                                                    );
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
