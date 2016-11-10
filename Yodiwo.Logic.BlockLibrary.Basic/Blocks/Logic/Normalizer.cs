using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Normalizer",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Normalizes its input",
                 FriendlyImageSource = "/Content/img/icons/Generic/normalizer.png")]
    public class Normalizer : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        // public InputIO Input { get { return _Inputs[0]; } } // not used
        //------------------------------------------------------------------------------------------------------------------------
        // public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } } // not used
        //------------------------------------------------------------------------------------------------------------------------
        IOCategory ioCat;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Input range min",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Minimum value of input")]
        public float LevelMIN = 0f;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Input range max",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Maximum value of input")]
        public float LevelMAX = 1f;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Invert result",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Invert output result")]
        public bool invert = false;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Include negative output range",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Include negative output range")]
        public bool negatives = false;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Normalizer__
                
This block normalizes its input(s) from a specified (min, max) range into the [0..1] range or the [-1..1] range.

Inputs can be added or removed by right clicking on the block.
-- - 
*Input range min:*
> Configuration of minimum input.

*Input range max:*
> Configuration of maximum input.

*Include negative output range:*
> If set, rescales to the [-1..1] range; otherwise to the [0..1] output range."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Normalizer() : this(5) { }
        //------------------------------------------------------------------------------------------------------------------------
        public Normalizer(int IOCount = 1)
            : base(IOCount, IOCount)
        {
            int i = 0;
            ioCat = new IOCategory() { Name = "Lines", MinVisibleIO = 1 };
            IOCategories.Add(ioCat);

            //setup I/Os
            for (i = 0; i < IOCount; i++)
            {
                var id = (i + 1).ToString();
                //input
                _Inputs[i].IOType = typeof(double);
                _Inputs[i].Name = "In" + id;
                _Inputs[i].Description = "Input #" + id;
                ioCat.Inputs.Add(_Inputs[i]);

                //output
                _Outputs[i].Name = "Out" + id;
                _Outputs[i].Description = "Output #" + id;
                _Outputs[i].IOType = typeof(double);
                ioCat.Outputs.Add(_Outputs[i]);
            }
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
            for (var i = 0; i < _Inputs.Length; i++)
            {
                if (_Inputs[i].IsConnected && _Inputs[i].IsTouched)
                {
                    //compute
                    var res = ((double)_Inputs[i].Value).Rescale(LevelMIN, LevelMAX, negatives ? -1 : 0, 1);
                    if (invert)
                    {
                        if (negatives)
                            res = -res;
                        else
                            res = 1.0 - res;
                    }
                    //apply
                    _Outputs[i].Value = res;
                }
            }
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


