using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "De-Multiplexer",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Performs the operation of a de-multiplexer",
                 FriendlyImageSource = "/Content/img/icons/Generic/demux.png")]
    public class DeMux : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputSelector { get { return _Inputs[0]; } }
        public InputIO InputSet { get { return _Inputs[1]; } }
        public InputIO InputClear { get { return _Inputs[2]; } }
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __De-Multiplexer__
                
This block demultiplexes a single input into multiple possible outputs. 

The block will pass the value of the SET input to the currently selected output, and the value of the CLEAR input to all other outputs.

It is ok to leave CLEAR unconnected. In that case all non selected outputs will not receive events regardless of changes in the input. 

While an output is selected it receives all of the SET input's events.

Outputs can be added or removed by right clicking on the block. If you need more outputs, you can use multiple Demux blocks and alter each block's Selector input accordingly (e.g. using the calculator block)"
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public DeMux() : this(16) { }
        //------------------------------------------------------------------------------------------------------------------------
        public DeMux(int outputCount)
            : base(3, outputCount)
        {
            //setup Selector Input
            InputSelector.IOType = typeof(int?);
            InputSelector.Name = "Selector";
            InputSelector.Description = "";
            InputSelector.UIHints.IsTrigger = true;

            //setup Input - True Port
            InputSet.IOType = typeof(object);
            InputSet.Name = "Set Value";
            InputSet.Description = "";

            //setup Input - False Port
            InputClear.IOType = typeof(object);
            InputClear.Name = "Clear Value";
            InputClear.Description = "";

            // setup IOCategories - updateable out ports
            IOCategory ioCat = new IOCategory() { Name = "Outputs", MinVisibleIO = 2 };
            IOCategories.Add(ioCat);

            //setup Output IO
            foreach (var outp in _Outputs)
            {
                outp.Name = "Result #" + outp.Index.ToString();
                outp.Description = "";
                outp.IOType = typeof(object);

                ioCat.Outputs.Add(outp);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (InputSelector.IsConnected)
            {
                var sel = (int?)InputSelector.Value;
                if (sel.HasValue)
                {
                    //"clear" all outputs
                    if (InputClear.IsConnected)
                        for (int i = 0; i < _Outputs.Length; i++)
                            _Outputs[i].Value = InputClear.Value;

                    //now set the selected value to proper output
                    if (sel.Value.isBetweenValues(0, _Outputs.Length - 1))
                        _Outputs[sel.Value].Value = InputSet.Value;
                }
                else
                {
                    for (int i = 0; i < _Outputs.Length; i++)
                        _Outputs[i].Value = null;
                }
            }
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


