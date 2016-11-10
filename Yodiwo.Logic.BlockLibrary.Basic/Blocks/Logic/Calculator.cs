using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Calculator",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Performs an arithmetic operation on the inputs",
                 FriendlyImageSource = "/Content/img/icons/Generic/calculator.png")]
    public class Calculator : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputA { get { return _Inputs[0]; } }
        public InputIO InputB { get { return _Inputs[1]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Operation",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Output true on the following relation")]
        public Operation operation;
        //------------------------------------------------------------------------------------------------------------------------
        public enum Operation
        {
            Add,
            SubtractBfromA,
            Multiply,
            DivideAbyB,
            BpercentOfA,
            AmoduloB,
        }
        //------------------------------------------------------------------------------------------------------------------------

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Double Precision",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "number of digits of precision of result")]
        public bool DoublePrecision;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Calculator__
                
This block performs a specific arithmetic operation on the inputs. The arithmetic operation should be configured as well as the type of precision.
-- - 
*Operation:*
> Select the oppropriate operation.
> - Addition
> - Subtraction
> - Multiplication
> - Division
> - Percentage of
> - Modulo

*Double Precision:*
> Turn to ON if double precision is needed."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Calculator()
            : base(2, 1)
        {
            //setup Input IO
            InputA.IOType = typeof(double);
            InputA.Name = "Input A";
            InputA.Description = "";

            InputB.IOType = typeof(double);
            InputB.Name = "Input B";
            InputB.Description = "";

            //setup Output IO
            Output.Name = "Result";
            Output.Description = "";
            Output.IOType = typeof(double);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (InputA.IsConnected && InputB.IsConnected)
            {
                double res = 0.0;
                bool allOk = true;
                string op = "";
                try
                {
                    switch (operation)
                    {
                        case Operation.Add:
                            op = "+";
                            res = (double)InputA.Value + (double)InputB.Value;
                            break;
                        case Operation.SubtractBfromA:
                            op = "-";
                            res = (double)InputA.Value - (double)InputB.Value;
                            break;
                        case Operation.Multiply:
                            op = "*";
                            res = (double)InputA.Value * (double)InputB.Value;
                            break;
                        case Operation.DivideAbyB:
                            op = "/";
                            if ((double)InputB.Value != 0)
                                res = (double)InputA.Value / (double)InputB.Value;
                            else
                                allOk = false;
                            break;
                        case Operation.BpercentOfA:
                            op = "percentof";
                            res = (double)InputA.Value * (double)InputB.Value / 100;
                            break;
                        case Operation.AmoduloB:
                            op = "modulo";
                            if ((double)InputB.Value != 0)
                                res = (double)InputA.Value % (double)InputB.Value;
                            else
                                allOk = false;
                            break;
                        default:
                            break;
                    }
                    if (allOk)
                    {
                        Output.Value = DoublePrecision ? res : (int)res;
                        DebugEx.TraceLog("Calc: InA: " + InputA.Value.ToString() + " " + op + " InB:" + InputB.Value.ToString() + " => " + Output.Value.ToString());
                    }
                }
                catch { }
            }
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


