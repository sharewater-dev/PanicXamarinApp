using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Level scaler",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Scales its input to the specified limits",
                 FriendlyImageSource = "/Content/img/icons/Generic/levelscaler.png")]
    public class LevelScaler : Block
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
                     FriendlyName = "Output range min",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Minimum value of scaled output")]
        public float LevelMIN = 0f;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Output range max",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Maximum value of scaled output")]
        public float LevelMAX = 1f;

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Input can have negative values",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Input can have negative values",
            IsAdvanced = true)]
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
                Value = @"### __Level Scaler__
                
This block scales its input(s) to the specified limits. These limits (min, max) represent the range of the scaled output and should be configured. 

The input value is expected to be in the [0..1] range, or [-1..1] if Negative range is selected. 

Inputs can be added or removed by right clicking on the block.
-- - 
*Output range min:*
> Configuration of bottom scale limit.

*Output range max:*
> Configuration of upper scale limit.

*Input can have negative values:*
> Set to True, to scale to range [-1..1] range."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        public LevelScaler() : this(5) { }
        //------------------------------------------------------------------------------------------------------------------------
        public LevelScaler(int IOCount = 1)
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
                if (_Inputs[i].IsConnected && _Inputs[i].IsTouched)
                    _Outputs[i].Value = ((double)_Inputs[i].Value).Rescale(negatives ? -1 : 0, 1, LevelMIN, LevelMAX);
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


