using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "Random Number Generator",
                              StencilCategory = UIStencilCategory.Input,
                              Description = "Generate random number",
                              FriendlyImageSource = "/Content/img/icons/Generic/dice.png")]
    public class RandomNumberGenerator : EndpointIn
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputI { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        public OutputIO OutputD { get { return _Outputs[1]; } set { _Outputs[1] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Minimum (inclusive)",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Minimum number to generate (inclusive)")]
        public float Minimum = 0;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Maximum (inclusive)",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Maximum number to generate (inclusive)")]
        public float Maximum = 1;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Random Number Generator__ 

This block generates a random number.
-- -
This block has two outputs providing different information.
- Integer : Gives a rounded integer of the generated number
- Decimal : Gives the generated number in its decimal form
"
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public RandomNumberGenerator()
            : base(0, 2)
        {
            //mark as a volatile block
            IsVolatile = true;

            //setup Output IO
            _Outputs[0].Name = "Integer";
            _Outputs[0].IOType = typeof(int);

            _Outputs[1].Name = "Decimal";
            _Outputs[1].IOType = typeof(float);

            _Outputs[0].Value = 0L;
            _Outputs[1].Value = 0D;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            //generate random number
            var min = Math.Min(Minimum, Maximum);
            var max = Math.Max(Minimum, Maximum);
            var rnd = MathTools.GetRandomNumber(min, max);

            //apply solution to Outputs
            _Outputs[0].Value = (int)Math.Round(rnd);
            _Outputs[1].Value = rnd;

            //switch block to clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
