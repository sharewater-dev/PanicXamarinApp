using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "BOOL",
                 StencilCategory = UIStencilCategory.Input,
                 Description = "Specifies an input of type Boolean",
                 FriendlyImageSource = "/Content/img/icons/Generic/bool.png")]
    public class Boolean : EndpointIn
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Selector { get { return _Inputs[0]; } set { _Inputs[0] = value; } }
        public InputIO Trigger { get { return _Inputs[1]; } set { _Inputs[1] = value; } }
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Array of boolean numbers",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "A boolean array specifying true/false values that the block can place on its output, depending on its selected index")]
        public string[] Constants;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Select Index",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Position (from 0) of boolean value in array of constants to be placed on output. If Select block input is connected, this value is ignored")]
        public int Index;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = @"### __Boolean Input__
This block specifies an input of type bool. Boolean arrays and the index of the array can be configured.
 -- -
*Array of boolean numbers:*
> A boolean array specifying true/false values that the block can place on its output, depending on its selected index.

*Select Index:*
> Position (from 0) of boolean value in array of constants to be placed on output. If Select block input is connected, this value is ignored."
        };
        //------------------------------------------------------------------------------------------------------------------------
        private bool[] ConstantBooleans;
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Boolean()
            : base(2, 1)
        {
            Output.Name = "Output";
            Output.Description = "";
            Output.Value = false;
            Output.IOType = typeof(bool);

            Selector.Value = 0;
            Selector.Name = "Select";
            Selector.Description = "Index (from 0) of boolean value to place on output. If connected, overrides internal block's value";
            Selector.IOType = typeof(int);

            Trigger.Name = "Trigger";
            Trigger.Description = "If triggered, the output will be rewritten according to which constant is selected from the Selector input or internal index";
            Trigger.IOType = typeof(bool);
            Trigger.UIHints.IsTrigger = true;

            //init default falue
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

            //validate values
            for (int n = 0; n < Constants.Length; n++)
            {
                bool parsed;
                if (!Yodiwo.ConvertEx.Convert(Constants[n], out parsed))
                    ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Could not convert value " + Constants[n] + " at index " + n + " to boolean"));
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            //parse values
            bool parsed;
            ConstantBooleans = new bool[Constants.Length];
            for (int n = 0; n < Constants.Length; n++)
            {
                if (Yodiwo.ConvertEx.Convert(Constants[n], out parsed))
                    ConstantBooleans[n] = parsed;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            bool? localValue = null;
            if (Trigger.IsTriggered || Selector.IsTouched || Graph.IsWarmupSolve)
            {
                if (Selector.IsConnected)
                {
                    int select = (int)Selector.Value;
                    if (select.isBetweenValues(0, Constants.Length - 1))
                        localValue = ConstantBooleans[select];
                }
                else
                    localValue = ConstantBooleans[Index];

                if (localValue.HasValue)
                    Output.Value = localValue.Value;
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
