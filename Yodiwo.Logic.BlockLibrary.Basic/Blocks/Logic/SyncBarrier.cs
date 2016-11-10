using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    /// <summary>
    /// This block will sync solving for diverging dependency paths
    /// </summary>
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Sync Barrier",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Synchronizes processing of events on its input",
                 FriendlyImageSource = "/Content/img/icons/Generic/solverbarrier.png")]
    public class SyncBarrier : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        bool[] touchedInputs;
        //------------------------------------------------------------------------------------------------------------------------
        IOCategory inoutCat;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
             FriendlyName = "Description",
             InspectorCategory = UIInspectorCategory.Description,
             Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
                new Yodiwo.Logic.Descriptors.Markdown()
                {
                    Value = @"### __Sync Barrier__

This block synchronizes inputs that may arrive asynchronously and from different input sources and/or events.

It guarantees that all inputs will simultaneously be passed through to the outputs, when **all** inputs reach *Touched* 
status (= new event, even if it assigns same value as before), however long this may take.

++*Note*++: Tracking of Inputs' Touched status is reset at:
 - every pass-through event
 - every graph redeploy"
                };
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public SyncBarrier()
            : this(8)
        {
        }

        public SyncBarrier(int IOCount)
            : base(IOCount, IOCount)
        {
            // Create IO categories
            inoutCat = new IOCategory() { Name = "Lines", MinVisibleIO = 2 };
            IOCategories.Add(inoutCat);

            //setup Input IO
            var id = 1;
            foreach (var inp in _Inputs)
            {
                inp.IOType = typeof(object);
                inp.Name = "Input Data #" + id.ToString();
                inp.Description = "";
                inoutCat.Inputs.Add(inp);
                id++;
            }

            //setup Output IO
            id = 1;
            foreach (var outp in _Outputs)
            {
                outp.Name = "Output Data #" + id.ToString();
                outp.IOType = typeof(object);
                outp.Description = "";
                inoutCat.Outputs.Add(outp);
                id++;
            }

            //bool array to track touched io
            touchedInputs = new bool[IOCount];
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //Pass through?
            bool passthrough = true;
            for (int n = 0; n < _Inputs.Length; n++)
                if (_Inputs[n].IsConnected)
                {
                    if (_Inputs[n].IsTouched)
                        touchedInputs[n] = true;
                    if (!touchedInputs[n])
                        passthrough = false;
                }

            //if all touched (passthrough), copy values to outputs
            if (passthrough)
            {
                for (int n = 0; n < _Inputs.Length; n++)
                {
                    _Outputs[n].Value = _Inputs[n].Value;
                    touchedInputs[n] = false;
                }
            }

            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
