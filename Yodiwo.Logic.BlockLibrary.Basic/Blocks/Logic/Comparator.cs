using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Comparator",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Outputs true when the configured relation between the two inputs is true",
                 FriendlyImageSource = "/Content/img/icons/Generic/comparator.svg")]
    public class Comparator : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputA { get { return _Inputs[0]; } }
        public InputIO InputB { get { return _Inputs[1]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        public OutputIO OutputBar { get { return _Outputs[1]; } set { _Outputs[1] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Condition",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Relation to evaluate")]
        public Condition condition;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Compared Value",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Value that Input A will be compared with if Input B is left unconnected; can be decimal")]
        public string ComparedValue = "0";

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Perform string comparison",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Treat input as text instead of numbers")]
        public bool StringCompare = false;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Error Margin",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Allowed margin of error (double precision fl. point) on fl. point equality operations")]
        public double _allowedError = 0.001;
        //------------------------------------------------------------------------------------------------------------------------
        public enum Condition
        {
            LessThan,
            LessOrEqual,
            Equal,
            MoreOrEqual,
            MoreThan,
            NotEqual,
        }
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Comparator__
                
This block evaluates its outputs based on the selected relation between two inputs. 

Inputs can either both come from the graph itself, or a single input from the graph can be evaluated against a static (at deploy time) value.

Two outputs are provided, the result of the evaluation, and also its inversion, so that your graph can easily handle 'do this if true, do that if false' scenarios.
-- - 
*Condition:*
> Select the oppropriate condition.
 - A less than B
 - A less than or equal to B
 - A more than B
 - A more than or equal to B
 - A equal to B
 - A not equal to B

*Compared Value:*
> It is the value that Input A will be compared to, if Input B is left unconnected.

*Perform string comparison:*
> If set, input will be treated as text; otherwise floating point comparison will be performed

*Error Margin:*
> The allowed margin of error (double precision floating point) on equality operations"
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Comparator()
            : base(2, 2)
        {
            //setup Input IO
            InputA.IOType = typeof(CommonIOTypes);
            InputA.Name = "Input A";
            InputA.Description = "";

            InputB.IOType = typeof(CommonIOTypes);
            InputB.Name = "Input B";
            InputB.Description = "";

            //setup Output IO
            Output.Name = "Result";
            Output.Description = "Comparison result";
            Output.IOType = typeof(bool);

            OutputBar.Name = "Inversed Result";
            OutputBar.Description = "Inversed comparison result";
            OutputBar.IOType = typeof(bool);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (InputA.IsConnected && (InputA.IsTouched || InputB.IsTouched))
            {
                //---------------------------
                // apply solution to outputs and clean
                // if no match is found, no value is written to output and the blocks following this one are not marked dirty
                //---------------------------
                int cmp = 0;
                bool canCompare = false;
                var inputBval = (InputB.IsConnected) ? InputB.Value : ComparedValue;

                if (!StringCompare)
                {
                    double dbla, dblb;
                    var convertedAtoDbl = ConvertEx.Convert(InputA.Value, out dbla);
                    var convertedBtoDbl = ConvertEx.Convert(inputBval, out dblb);

                    if (convertedAtoDbl && convertedBtoDbl)
                    {
                        cmp = dbla.AlmostEqual(dblb, _allowedError) ? 0 : (dbla < dblb) ? -1 : 1;
                        canCompare = true;
                    }
                }

                //if text comparison was selected or fl.point comparison failed:
                if (!canCompare)
                {
                    string stra;
                    string strb;
                    var convertedAtoStr = ConvertEx.Convert(InputA.Value, out stra);
                    var convertedBtoStr = ConvertEx.Convert(inputBval, out strb);

                    if (convertedAtoStr && convertedBtoStr)
                    {
                        cmp = String.Compare(stra, strb);
                        canCompare = true;
                    }
                }

                //if either text or floating point comparison succeeded:
                if (canCompare)
                {
                    bool res = false;
                    switch (condition)
                    {
                        case Condition.LessThan:
                            res = (cmp < 0);
                            break;
                        case Condition.LessOrEqual:
                            res = (cmp <= 0);
                            break;
                        case Condition.Equal:
                            res = (cmp == 0);
                            break;
                        case Condition.MoreOrEqual:
                            res = (cmp >= 0);
                            break;
                        case Condition.MoreThan:
                            res = (cmp > 0);
                            break;
                        case Condition.NotEqual:
                            res = (cmp != 0);
                            break;
                    }
                    Output.Value = res;
                    OutputBar.Value = !res;
                }
            }
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


