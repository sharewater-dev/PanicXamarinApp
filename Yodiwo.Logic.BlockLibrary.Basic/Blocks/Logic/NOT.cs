using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "Boolean NOT",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "logic Inversion of input (will try to do it for booleans, strings, numbers)",
                              FriendlyImageSource = "/Content/img/icons/Generic/not.png")]
    public class NOT : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputA { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputA { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = @"### __Boolean NOT__
This block performs the operation of a logical NOT gate. This operation is known as logic inversion.
 -- -
Boolean NOT block provides a boolean output depending on its boolean input."
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public NOT()
            : base(1, 1)
        {
            //setup Input IO types
            InputA.IOType = typeof(bool);
            InputA.Name = "In";
            InputA.Description = "True value causes block's output state to toggle";

            //init output value
            OutputA.Value = false;
            OutputA.Name = "Out";
            OutputA.IOType = typeof(bool);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions

        protected override IEnumerable<BlockState> Solve()
        {
            //apply solution to output
            if (InputA.IsConnected && InputA.IsTouched)
                OutputA.Value = !(bool)InputA.Value;

            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
