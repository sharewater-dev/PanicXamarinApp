using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "FLOAT",
                 StencilCategory = UIStencilCategory.Input,
                 Description = "Specifies an input of floating-point type",
                 FriendlyImageSource = "/Content/img/icons/Generic/float.png")]
    public class Float : EndpointIn
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Selector { get { return _Inputs[0]; } set { _Inputs[0] = value; } }
        public InputIO Trigger { get { return _Inputs[1]; } set { _Inputs[1] = value; } }
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Array of constant numbers",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "A floating-point number array specifying numbers that the block can place on its output, depending on its selected index")]
        public string[] Constants;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Select Index",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Position (from 0) of number in array of constants to be placed on output. If Select block input is connected, this value is ignored")]
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
            Value = @"### __Float__
This block specifies an input of floating-point type. Arrays of constant floating-point numbers and the index of the array can be configured.
 -- -
*Array of constant numbers:*
> A floating-point number array specifying numbers that the block can place on its output, depending on its selected index.

*Select Index:*
> Position (from 0) of number in array of constants to be placed on output. If Select block input is connected, this value is ignored"
        };
        //------------------------------------------------------------------------------------------------------------------------
        private float[] ConstantFloats;
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Float()
            : base(2, 1)
        {
            Output.Name = "Constant";
            Output.Description = "";
            Output.Value = 0f;
            Output.IOType = typeof(float);

            Selector.Value = 0;
            Selector.Name = "Select";
            Selector.Description = "Index (from 0) of number to place on output. If connected, overrides internal block's value";
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

            if (!Index.isBetweenValues(0, Constants.Length - 1))
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Index out of bounds. Should be within 0.." + (Constants.Length - 1)));

            //validate valid floats
            for (int n = 0; n < Constants.Length; n++)
            {
                float parsed;
                if (!Yodiwo.ConvertEx.Convert(Constants[n], out parsed))
                    ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Could not convert value " + Constants[n] + " at index " + n + " to floating point"));
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            //init floats
            float parsed;
            ConstantFloats = new float[Constants.Length];
            for (int n = 0; n < Constants.Length; n++)
            {
                if (Yodiwo.ConvertEx.Convert(Constants[n], out parsed))
                    ConstantFloats[n] = parsed;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            float? localValue = null;
            if (Trigger.IsTriggered || Selector.IsTouched || Graph.IsWarmupSolve)
            {
                if (Selector.IsConnected)
                {
                    int select = (int)Selector.Value;
                    if (select.isBetweenValues(0, Constants.Length - 1))
                        localValue = ConstantFloats[select];
                }
                else
                    localValue = ConstantFloats[Index];

                if (localValue.HasValue)
                    Output.Value = localValue.Value;
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
