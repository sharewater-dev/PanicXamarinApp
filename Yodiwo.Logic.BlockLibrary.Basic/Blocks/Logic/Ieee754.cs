using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Logic.Blocks.Endpoints.In;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "IEEE Floating point builder",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "Performs a logical AND operation",
                              FriendlyImageSource = "/Content/img/icons/Generic/float.png")]
    public class Ieee754 : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputA => _Inputs[0];
        public InputIO InputB => _Inputs[1];
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Flip bytes within inputs",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Flip bytes within inputs")]
        public bool FlipBytes = false;
        //------------------------------------------------------------------------------------------------------------------------

        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Description",
                 InspectorCategory = UIInspectorCategory.Description,
                 Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
        new Yodiwo.Logic.Descriptors.Markdown()
        {
            Value = ""
            //            Value = @"### __Boolean AND__
            //This block performs the operation of a logical AND gate.
            // -- -
            //Boolean AND block provides a boolean output depending on its two boolean inputs."
        };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Ieee754()
                : base(2, 1)
        {

            InputA.IOType = typeof(int);
            InputA.Name = "InputA";
            InputA.Description = "";
            InputB.IOType = typeof(int);
            InputB.Name = "InputB";
            InputB.Description = "";

            //setup Output IO
            Output.Name = "Out";
            Output.Description = "";
            Output.IOType = typeof(double);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (InputA.IsConnected && InputB.IsConnected)
            {

                ushort a = (ushort)(int)InputA.Value;
                ushort b = (ushort)(int)InputB.Value;

                byte[] bytes = new byte[4];
                var abytes = BitConverter.GetBytes(a);
                var bbytes = BitConverter.GetBytes(b);
                if (FlipBytes)
                {
                    abytes = abytes.Reverse().ToArray();
                    bbytes = bbytes.Reverse().ToArray();
                }
                abytes.CopyTo(bytes, 0);
                bbytes.CopyTo(bytes, 2);
                var result = BitConverter.ToSingle(bytes, 0);
                Output.Value = (double)result;
            }
            //switch block to clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
