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
    [UIBasicInfo(IsVisible = false,
                 FriendlyName = "Dependency Barrier",
                 StencilCategory = UIStencilCategory.Logic,
                 IsAdvanced = true,
                 Description = "Synchronizes processing of events on its input",
                 FriendlyImageSource = "/Content/img/icons/Generic/solverbarrier.png")]
    public class SolverBarrier : Block
    {
        #region Variables

        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Dependency Barrier__"
            };

        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public SolverBarrier()
            : this(5)
        {
        }

        public SolverBarrier(int IOCount)
            : base(IOCount, IOCount)
        {
            // Create IO categories
            var inoutCat = new IOCategory() { Name = "Lines", MinVisibleIO = 2 };
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
                outp.Description = "";
                inoutCat.Outputs.Add(outp);
                outp.IOType = typeof(object);
                id++;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //Pass through
            for (int n = 0; n < _Inputs.Length; n++)
                if (_Inputs[n].IsTouched)
                    _Outputs[n].Value = _Inputs[n].Value;

            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
