using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "In Range",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "Detects if an input is in a specific range",
                              FriendlyImageSource = "/Content/img/icons/Generic/range.png")]
    public class InRange : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputValue { get { return _Inputs[0]; } }
        public InputIO InputMin { get { return _Inputs[1]; } }
        public InputIO InputMax { get { return _Inputs[2]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Input range min",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Minimum value of input (decimal)")]
        public float LevelMIN = 0.0f;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Input range max",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Maximum value of input (decimal)")]
        public float LevelMAX = 1.0f;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Invert result",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Invert output result (True on value outside of range)")]
        public bool invert = false;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = @"### __In Range__
Detects if an input is in a specific range.
 -- -
Detects if an input is in a specific range."
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public InRange()
            : base(3, 1)
        {
            //setup Input IO
            InputValue.IOType = typeof(double);
            InputValue.Name = "Input";
            InputValue.Description = "";

            //setup Input IO (optional)
            InputMin.IOType = typeof(double);
            InputMin.Name = "Min";
            InputMin.Description = "";
            var inpMinCat = new IOCategory() { Name = "Min", MinVisibleIO = 0 };
            inpMinCat.Inputs.Add(InputMin);
            IOCategories.Add(inpMinCat);

            //setup Input IO (optional)
            InputMax.IOType = typeof(double);
            InputMax.Name = "Max";
            InputMax.Description = "";
            var inpMaxCat = new IOCategory() { Name = "Max", MinVisibleIO = 0 };
            inpMaxCat.Inputs.Add(InputMax);
            IOCategories.Add(inpMaxCat);

            //setup Output IO
            Output.Name = "Output";
            Output.Description = "";
            Output.IOType = typeof(bool);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //find value and min/max
            var value = (double)InputValue.Value;
            var min = InputMin.IsConnected ? (double)InputMin.Value : LevelMIN;
            var max = InputMax.IsConnected ? (double)InputMax.Value : LevelMAX;

            //apply solution to outputs
            Output.Value = value >= min && value <= max;

            //switch block to clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
