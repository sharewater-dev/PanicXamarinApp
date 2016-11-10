using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "CONSTANT",
                 StencilCategory = UIStencilCategory.Input,
                 Description = "Specifies a constant input",
                 FriendlyImageSource = "/Content/img/icons/Generic/constant.svg")]
    public class Constant : EndpointIn
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Selector { get { return _Inputs[0]; } set { _Inputs[0] = value; } }
        public InputIO Trigger { get { return _Inputs[1]; } set { _Inputs[1] = value; } }
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Array of constant strings",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "A text array specifying text strings that the block can place on its output, depending on its selected index")]
        public string[] Constants;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Select Index",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Position (from 0) of string in array of constants to be placed on output. If Select block input is connected, this value is ignored")]
        public int Index;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Allow empty output",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If enabled, even empty output values will trigger events to the next connected block")]
        public bool AllowEmptyOutput;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = @"### __Constant__
This block specifies a constant input. Arrays of constant strings and the index of the array can be configured.
 -- -
*Array of constant strings:*
> A text array specifying text strings that the block can place on its output, depending on its selected index.

*Select Index:*
> Position (from 0) of strings in array of constants to be placed on output. If Select block input is connected, this value is ignored."
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Constant()
            : base(2, 1)
        {
            //init default falue
            Output.Name = "Constant";
            Output.Description = "";
            Output.Value = "";
            Output.IOType = typeof(string);

            Selector.Value = 0;
            Selector.Name = "Select";
            Selector.Description = "Index (from 0) of string to place on output. If connected, overrides internal block's value";
            Selector.IOType = typeof(int);

            Trigger.Name = "Trigger";
            Trigger.Description = "If triggered, the output will be rewritten according to which constant is selected from the Selector input or internal index";
            Trigger.IOType = typeof(bool);
            Trigger.UIHints.IsTrigger = true;

            Constants = new string[1];
            Constants[0] = "";
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            //base Validation
            base.Validate(ResultOutput);

            //validate index
            if (!Index.isBetweenValues(0, Constants.Length - 1))
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Index out of bounds. Should be within 0.." + Constants.Length));
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            string value = null;
            if (Trigger.IsTriggered || Selector.IsTouched || Graph.IsWarmupSolve)
            {
                if (Selector.IsConnected)
                {
                    int select = (int)Selector.Value;
                    if (select.isBetweenValues(0, Constants.Length - 1))
                        value = Constants[select];
                }
                else
                    value = Constants[Index];

                if (AllowEmptyOutput || !string.IsNullOrWhiteSpace(value))
                    Output.Value = value;
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}

