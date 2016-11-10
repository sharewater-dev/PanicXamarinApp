using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "Any Value",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "Accepts multiple inputs and forwards the first (or last) active one to the output; " +
                                                 "useful when needing to drive a next block with many -not simultaneously active- inputs",
                              FriendlyImageSource = "/Content/img/icons/Generic/combiner.svg")]
    public class AnyValue : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputA { get { return _Inputs[0]; } }
        public InputIO InputB { get { return _Inputs[1]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Evaluate All",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Choose mode when more than one input is active: Accept first or last active")]
        public Boolean EvaluateAll;

        IOCategory inpCat;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Any Value__
                
This block accepts multiple inputs and forwards the first (or last) active one to the output. It is useful when needing to drive a next block with many -not simultaneously active- inputs.
-- - 
*Evaluate All:*
> This mode can be used when more than one input is active. If it is ON the block forwards the last active input, else if it is OFF the block forwards the first active one."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public AnyValue()
            : this(10)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public AnyValue(int InputCount)
            : base(InputCount, 1)
        {
            inpCat = new IOCategory() { Name = "Inputs", MinVisibleIO = 2 };
            IOCategories.Add(inpCat);

            int i;

            //setup Input IO
            for (i = 0; i < _Inputs.Length; i++)
            {
                //input
                var id = (i + 1).ToString();
                _Inputs[i].IOType = typeof(Object);
                _Inputs[i].Name = "In" + id;
                _Inputs[i].Description = "Input #" + id;
                inpCat.Inputs.Add(_Inputs[i]);
            }

            //setup Output IO
            Output.Name = "Out";
            Output.Description = "First (or last) active input";
            Output.Value = null;
            Output.IOType = typeof(object);

            EvaluateAll = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //declares
            Object ret = null;

            //solve
            foreach (var inp in inpCat.Inputs)
                if (inp.IsConnected && inp.IsTouched)
                {
                    ret = inp.Value;
                    if (!EvaluateAll)
                        break;
                }

            //apply solution to output
            if (ret != null)
                Output.Value = ret;

            //switch block to clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
