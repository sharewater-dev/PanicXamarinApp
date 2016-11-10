using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "FORWARD IF",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "input(s) passed to output(s) as long as enable port is true",
                 FriendlyImageSource = "/Content/img/icons/Generic/forwardif.svg")]
    public class ForwardIf : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Input { get { return _Inputs[0]; } } // not used
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { set { _Outputs[0] = value; } }

        private int enable_idx;
        //------------------------------------------------------------------------------------------------------------------------
        IOCategory ioCat;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Forward If__
                
This block passes its input(s) to output(s) as long as enable port is true. Otherwise, input(s) will not be forwarded. 

The enable port will be treated as boolean type.

Forward lines (input-output) can be added or removed by right clicking on the block."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ForwardIf()
            : this(8)
        {
        }

        public ForwardIf(int IOCount = 1)
            : base(IOCount + 1, IOCount)
        {
            int i = 0;

            ioCat = new IOCategory() { Name = "Inputs", MinVisibleIO = 1 };
            IOCategories.Add(ioCat);

            //setup I/Os
            for (i = 0; i < _Inputs.Length - 1; i++)
            {
                var id = (i + 1).ToString();
                //input
                _Inputs[i].IOType = typeof(Object);
                _Inputs[i].Name = "In" + id;
                _Inputs[i].Description = "Input #" + id;
                ioCat.Inputs.Add(_Inputs[i]);

                //output
                _Outputs[i].Name = "Out" + id;
                _Outputs[i].Description = "Output #" + id;
                _Outputs[i].Value = null;
                _Outputs[i].IOType = typeof(Object);
                ioCat.Outputs.Add(_Outputs[i]);
            }

            _Inputs[i].IOType = typeof(bool);
            _Inputs[i].Name = "Enable";
            _Inputs[i].Description = "Inputs(s) will pass to the Output(s) as long as this enable input stays true";
            _Inputs[i].UIHints.IsTrigger = true;
            enable_idx = i;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //---------------------------
            // apply solution to outputs and clean
            //---------------------------
            if (_Inputs[enable_idx].IsConnected)
            {
                bool enabled = (bool)_Inputs[enable_idx].Value;
                if (enabled)
                {
                    for (int n = 0; n < ioCat.Inputs.Count; n++)
                        if (_Inputs[n].IsConnected && _Inputs[n].IsTouched)
                            _Outputs[n].Value = _Inputs[n].Value;
                }
            }
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
