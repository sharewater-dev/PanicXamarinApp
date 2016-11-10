using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Round & Clamp",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Clamps and rounds input(s) to output(s) depending on settings",
                 FriendlyImageSource = "/Content/img/icons/Generic/float.png")]
    public class RoundDecimal : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Number of decimal places",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "set the number of decimal places that the input value should be rounded to")]
        public int NoOfDecimalPts = 2;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Minimum Value",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "set the minimum number the input value should be clamped to, if enabled")]
        public double MinValue = 0.0;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Clamp to Minimum Value",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Enable/disable clamping to specified minimum value")]
        public bool ClampToMin = false;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Maximum Value",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "set the maximum number the input value should be clamped to, if enabled")]
        public double MaxValue = 1000.0;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Clamp to Maximum Value",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Enable/disable clamping to specified maximum value")]
        public bool ClampToMax = false;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Round and clamp__
                
This block rounds input(s) to the number of decimal points specified and also clamps them to minimum and maximum values if enabled.

Lines (input-output) can be added or removed by right clicking on the block.
-- - 
*Number of decimal places:*
> The number of decimal points (0..28) to round to.

*Min value:*
> Minimum value to clamp input(s) to.

*Clamp to Minimum Value:*
> If set, minimum value set in *Min Value* setting will be applied if input value is less.

*Max value:*
> Maximum value to clamp input(s) to.

*Clamp to Maximum Value:*
> If set, maximum value set in *Max Value* setting will be applied if input value is more.

*Min value:*
> If set, delay starts counting from the respective input events. If not, delay starts counting after the last output event."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public RoundDecimal() : this(8) { }
        //------------------------------------------------------------------------------------------------------------------------
        public RoundDecimal(int IOCount = 1)
            : base(IOCount, IOCount)
        {
            // Create IO categories
            var inoutCat = new IOCategory() { Name = "Lines", MinVisibleIO = 1 };
            IOCategories.Add(inoutCat);

            //setup Input IO
            foreach (var inp in _Inputs)
            {
                inp.IOType = typeof(object);
                inp.Name = "Input Data (decimal)";
                inp.Description = "";

                inoutCat.Inputs.Add(inp);
            }

            //setup Output IO
            foreach (var outp in _Outputs)
            {
                outp.Name = "Output Data (rounded)";
                outp.Description = "";
                outp.IOType = typeof(double);

                inoutCat.Outputs.Add(outp);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            base.Validate(ResultOutput);

            if (!NoOfDecimalPts.isBetweenValues(0, 28))
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Number of decimal places must be up to 28"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            foreach (var inp in _Inputs)
            {
                if (inp.IsTouched && inp.Value != null)
                {
                    try
                    {
                        double dblOut;
                        if (ConvertEx.Convert<double>(inp.Value, out dblOut))
                        {
                            if (ClampToMin)
                                dblOut = dblOut.ClampFloor(MinValue);

                            if (ClampToMax)
                                dblOut = dblOut.ClampCeil(MaxValue);

                            var result = Decimal.Round(new Decimal(dblOut), NoOfDecimalPts);
                            _Outputs[inp.Index].Value = result;
                        }
                    }
                    catch { }
                }
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
