using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Multi Trigger",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Passes internal values to output(s) upon trigger",
                 FriendlyImageSource = "/Content/img/icons/Generic/icon-action-copy.png")]
    public class MultiTrigger : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Trigger { get { return _Inputs[0]; } set { _Inputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Array of constants",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "A text array specifying constant values to place on each output when triggered; right click to add more outputs")]
        public string[] Constants;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = @"### __Multi Trigger__
This block places value(s) from its internal array to corresponding output(s) each time it is triggered.
 -- -
*Array of constants:*
> A text array specifying text strings that the block can place on its output, depending on its selected index."
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public MultiTrigger()
            : base(1, 10)
        {
            // Create IO categories
            var outCat = new IOCategory() { Name = "Outputs", MinVisibleIO = 1 };
            IOCategories.Add(outCat);

            //setup Output IO
            for (var i = 0; i < _Outputs.Length; i++)
            {
                var id = (i + 1).ToString();

                _Outputs[i].Name = "Result";
                _Outputs[i].Description = "Output #" + id;
                _Outputs[i].IOType = typeof(string);
                _Outputs[i].Value = "";

                outCat.Outputs.Add(_Outputs[i]);
            }

            Trigger.Name = "Trigger";
            Trigger.Description = "If triggered, the outputs will be rewritten (and events produced) by the equivalent entry in the array of constants of the block's configuration";
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
            if (Constants.Length > _Outputs.Length)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Number of array elements must be at most " + _Outputs.Length));
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (Trigger.IsTriggered || Graph.IsWarmupSolve)
            {
                for (int i = 0; i < Constants.Length; i++)
                    if (_Outputs[i].IsConnected)
                        _Outputs[i].Value = Constants[i];
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}

