using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Logic;
using Yodiwo;
using Yodiwo.Logic.SubSystems;
using Yodiwo.API.MediaStreaming;


namespace Yodiwo.Logic.Blocks
{
    [UIBasicInfo(IsVisible = false,
        FriendlyName = "Web Video",
        StencilCategory = UIStencilCategory.Output,
        Description = "Web Link to video stream",
        FriendlyImageSource = "/Content/img/icons/Generic/webvideolink.jpg")]
    public class WebVideoLink : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputVideo { get { return _Inputs[0]; } set { _Inputs[0] = value; } }
        //        public InputIO InputTrigger { get { return _Inputs[1]; } set { _Inputs[1] = value; } }

        public OutputIO OutputLink { get { return _Outputs[0]; } set { _Outputs[0] = value; } }

        //------------------------------------------------------------------------------------------------------------------------
        //        [UIBasicInfo(IsVisible = true,
        //            FriendlyName = "to",
        //            Category = "Configuration",
        //            Description = "Specifies the destination user",
        //            ShortDescription = "Send to")]
        //        public string User = "-705302022@chat.facebook.com";
        //
        //        [UIBasicInfo(IsVisible = true,
        //            FriendlyName = "Message template",
        //            Category = "Configuration",
        //            Description = "Specifies the message ",
        //            ShortDescription = "message body")]
        //        public string Message = "You have... 1 new message... *beep*: {0}";

        //------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __WebVideoLink__"
            };

        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public WebVideoLink()
            : base(1, 1)
        {
            // Setup IO
            InputVideo.Name = "Video";
            InputVideo.Description = "";
            InputVideo.IOType = typeof(VideoDescriptor);
            //            InputTrigger.Name = "Trigger";
            //            InputTrigger.Description = "";
            //            InputTrigger.InputType = typeof(object);
            OutputLink.Name = "Link";
            OutputLink.Description = "";
            OutputLink.Value = "";
            OutputLink.IOType = typeof(string);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //check input dirtiness
            //            if (InputTrigger.IsConnected && InputTrigger.State == IOState.Dirty)
            //            {
            //                DebugEx.Assert (InputVideo.IsConnected, "video input not connected");
            //                if (InputVideo.IsConnected && InputVideo.Value != null) {
            //                    SubSystems.SocialMediaSubSystem.makeVideoConnection (InputVideo.ConnectedOutput.OwnerBlock, this, InputVideo.Value as VideoDescriptor);
            //
            //                }
            //            }
            if (InputVideo.IsConnected && InputVideo.State == IOState.Touched)
            {
                if ((InputVideo.Value as VideoDescriptor).HttpUrl != null)
                {
                    OutputLink.Value = (InputVideo.Value as VideoDescriptor).HttpUrl;
                }
                else
                {
                    OutputLink.Value = "";
                }
            }
            else
            {
                OutputLink.Value = "";
            }

            //clean block
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


