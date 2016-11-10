using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Selector",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Selects an input path to forward to its output based on the value in its select input",
                 FriendlyImageSource = "/Content/img/icons/Generic/selector.png")]
    public class Selector : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO SelectorIO { get { return _Inputs[selector_idx]; } set { _Inputs[selector_idx] = value; } }
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        private int selector_idx;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Selector__
                
This block selects an input path to forward to its output based on the value in its *select* input.

Inputs can be added or removed by right clicking on the block.
-- - 
#### __Important Note__ ####
If the select value is larger than the number of inputs, it automatically wraps around and selects the proper input (i.e. final selection = input_value % number_of_inputs).
-- - 

#### __Also Important__ ####
Counting starts from zero, so a value of 0 on select will choose the 1st input to place on the output."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Selector()
            : this(16)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Selector(int IOCount)
            : base(IOCount + 1, 1)
        {
            int i = 0;
            // The non-trigger inputs are going to be in one IOCategory 
            // The minimum size of showing inputs of this category is 2
            var inpCat = new IOCategory() { Name = "Inputs", MinVisibleIO = 2 };
            IOCategories.Add(inpCat);

            //setup input IO
            for (i = 0; i < _Inputs.Length - 1; i++)
            {
                _Inputs[i].IOType = typeof(object);
                _Inputs[i].Name = "In" + (i + 1).ToString();
                _Inputs[i].Description = "";
                inpCat.Inputs.Add(_Inputs[i]);
            }

            _Inputs[i].IOType = typeof(int);
            _Inputs[i].Name = "Select";
            _Inputs[i].Description = "";
            _Inputs[i].UIHints.IsTrigger = true;
            selector_idx = i;

            //setup Output IO
            Output.Name = "Out";
            Output.Description = "";
            Output.IOType = typeof(object);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (SelectorIO.IsConnected)
            {
                var selector_value = (int)SelectorIO.Value;

                //only continue if selector index is within allowed range
                if (selector_value.isBetweenValues(0, _Inputs.Length - 2))
                {
                    int selected = selector_value % (_Inputs.Length - 1);

                    if (_Inputs[selected].IsConnected && (_Inputs[selected].IsTouched || SelectorIO.IsTouched))
                    {
                        Output.Value = _Inputs[selected].Value;
                    }
                }
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}

