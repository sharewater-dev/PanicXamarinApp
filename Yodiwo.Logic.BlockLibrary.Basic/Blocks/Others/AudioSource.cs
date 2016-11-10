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
        FriendlyName = "MP3 Audio",
        StencilCategory = UIStencilCategory.Input,
        Description = "Internet MP3 Audio source",
        FriendlyImageSource = "/Content/img/icons/Generic/audiosource.png")]
    public class AudioSource : Yodiwo.Logic.Blocks.Endpoints.EndpointIn
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputVideo { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "URL",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Specifies the source URL")]
        public string Url = "http://pub8.rockradio.com:80/rr_classicrock";
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __MP3 Audio__
                
This block implements an internet MP3 Audio source.
-- - 
*URL:*
> Configures the source URL."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public AudioSource()
            : base(0, 1)
        {
            OutputVideo.Name = "Audio Url";
            OutputVideo.Description = "";
            OutputVideo.IOType = typeof(string);
            //provisional
        }

        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            //setup output
            AudioMediaDescriptor audio = new AudioMediaDescriptor()
            {
                Uri = Url,
                AudioDevice = AudioIn.Mp3Url,
            };
            OutputVideo.Value = audio.ToJSON(HtmlEncode: false);
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

