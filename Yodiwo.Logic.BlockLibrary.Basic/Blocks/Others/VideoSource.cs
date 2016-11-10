using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yodiwo.Logic;
using Yodiwo.API.Plegma;
using Yodiwo.Logic.SubSystems;
using Newtonsoft.Json;
using Yodiwo.API.MediaStreaming;

namespace Yodiwo.Logic.Blocks
{
    [UIBasicInfo(IsVisible = true,
        FriendlyName = "RTSP Video",
        StencilCategory = UIStencilCategory.Input,
        Description = "Internet RTSP Video source",
        FriendlyImageSource = "/Content/img/icons/Generic/videosource.svg")]
    public class VideoSource : Yodiwo.Logic.Blocks.Endpoints.EndpointIn
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputVideo { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "URL",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Specifies the source URL")]
        public string Url = "rtsp://streamer0.grnet.gr/parliament/parltv.sdp";
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __RTSP Video__
                
This block implements an internet RTSP Video source.
-- - 
*URL:*
> Configures the source URL."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public VideoSource()
            : base(0, 1)
        {
            //provisional
            OutputVideo.Name = "Video Url";
            OutputVideo.IOType = typeof(VideoMediaDescriptor);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            //setup output
            VideoMediaDescriptor video = new VideoMediaDescriptor()
            {
                Uri = Url,
                Protocol = Url.Split(':')[0],
                VideoDevice = VideoIn.WebUrl
            };
            OutputVideo.Value = video;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //clean block
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
