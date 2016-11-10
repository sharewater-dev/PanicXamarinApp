using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints.Out
{
    [UIBasicInfo(IsVisible = false,
                 FriendlyName = "Console Out",
                 StencilCategory = UIStencilCategory.Output,
                 Description = "Writes its input to the console",
                 FriendlyImageSource = "/Content/img/icons/Generic/consoleout.png")]
    public class ConsoleOut : EndpointOut
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Input { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Console Out__ 

This block write its input to the console. Its is frequently used for checking of other blocks' output."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ConsoleOut()
            : base(1, 0)
        {
            //setup Input IO types
            Input.IOType = typeof(string);

            Input.Name = "Input Data";
            Input.Description = "";
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //check input dirtiness
            if (Input.IsConnected && Input.State == IOState.Touched)
                DebugEx.TraceLog("ConsoleOut Block " + this.BlockKey + ": " + Input.Value);

            //clean block
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
