using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "Boolean AND",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "Performs a logical AND operation",
                              FriendlyImageSource = "/Content/img/icons/Generic/and.png")]
    public class AND : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputA { get { return _Inputs[0]; } }
        public InputIO InputB { get { return _Inputs[1]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = @"### __Boolean AND__
This block performs the operation of a logical AND gate.
 -- -
Boolean AND block provides a boolean output depending on its two boolean inputs."
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public AND()
            : this(8)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public AND(int InputCount)
            : base(InputCount, 1)
        {
            // The inputs is going to be in one category that can be change the size of it
            // The minimum size of showing inputs of this category are 2
            var inpCat = new IOCategory() { Name = "Inputs", MinVisibleIO = 2 };
            IOCategories.Add(inpCat);

            //setup Input IO
            foreach (var inp in _Inputs)
            {
                inp.IOType = typeof(bool);
                inp.Name = "Boolean In";
                inp.Description = "";
                inpCat.Inputs.Add(inp);
            }

            //setup Output IO
            Output.Name = "Boolean Out";
            Output.Description = "";
            Output.IOType = typeof(bool);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //declares
            bool ret = true;

            //solve
            foreach (var inp in _Inputs)
                if (inp.IsConnected)
                    ret &= (bool)inp.Value;

            //apply solution to outputs
            Output.Value = ret;

            //switch block to clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
