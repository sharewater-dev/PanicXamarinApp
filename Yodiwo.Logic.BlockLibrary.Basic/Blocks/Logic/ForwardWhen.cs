using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "FORWARD WHEN",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "input(s) passed to output(s) whenever trigger port is triggered",
                 FriendlyImageSource = "/Content/img/icons/Generic/forwardwhen.svg")]
    public class ForwardWhen : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Input { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { set { _Outputs[0] = value; } }

        private int trigger_idx;
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
                Value = @"### __Forward When__
                
This block passes its input(s) to its output(s) each time that the trigger port is triggered. Otherwise, the input(s) will not be forwarded to output(s) even if they change.

Forward lines (input-output) can be added or removed by right clicking on the block.
-- - 

#### __Important!__ ####
The trigger port is treated as all other trigger-type ports in Cyan, i.e. a trigger is registered on a Boolean True value or any non boolean value. 

This way any block output (text, numbers, etc) can be used as a trigger, but a boolean pulse (e.g. a button press) will produce a single trigger."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ForwardWhen()
            : this(8)
        {
        }

        public ForwardWhen(int IOCount = 1)
            : base(IOCount + 1, IOCount)
        {
            int i = 0;

            ioCat = new IOCategory() { Name = "Forwards", MinVisibleIO = 1 };
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
                _Outputs[i].IOType = typeof(object);
                ioCat.Outputs.Add(_Outputs[i]);
            }

            _Inputs[i].IOType = typeof(object);
            _Inputs[i].Name = "Trigger";
            _Inputs[i].Description = "Inputs(s) will be passed to the output(s) when this trigger switches from false to true";
            _Inputs[i].UIHints.IsTrigger = true;
            trigger_idx = i;
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
            if (_Inputs[trigger_idx].IsTriggered)
            {
                for (int n = 0; n < ioCat.Inputs.Count; n++)
                    if (_Inputs[n].IsConnected)
                        _Outputs[n].Value = _Inputs[n].Value;
            }

            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
