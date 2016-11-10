using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "String Builder",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "Concatenate multiple input strings into one",
                              FriendlyImageSource = "/Content/img/icons/Generic/ampersand.png")]
    public class YStringBuilder : Block
    {
        #region Variables
        static readonly Regex AllowedCharsValidator = new Regex(@"^[A-Za-z0-9!@#%&='_ -/?|*+.\[\]\(\)\{\}]+$");
        static readonly Regex ForbiddenCharsValidator = new Regex(@"<>^");

        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Final string",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Specifies the text and format of the final string, each {0},{1}...{N} will be replaced by the equivalent input")]
        public string FinalString = "[enter your {0} fields here]";
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __String Builder__
                
This block concatenates multiple input strings into one. Inputs can be added or removed using the right click on the block.
-- - 
*Final string:*
> Configures the text and format of the final string. Each {0},{1}...{N} will be replaced by the equivalent input.

For example, you can right click on the block and add 2 more inputs.
Then connect a timestamp output on the 1st one, the output of a 
temperature sensor on the 2nd and the output of a humidity sensor on the 3rd. 

To have the output appear as:
> At 4/5/2016 04:14:23am the temperature was 24C and humidity 60%

you would write:
> At {0} the temperature was {1}C and humidity was {2}%
"
            };

        public const string IoCategoryInputs = "Inputs";
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public YStringBuilder()
            : this(10)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public YStringBuilder(int InputCount)
            : base(InputCount, 1)
        {
            // The inputs is going to be in one category that can be change the size of it
            // The minimum size of showing inputs of this category are 2
            var inpCat = new IOCategory() { Name = IoCategoryInputs, MinVisibleIO = 1 };
            IOCategories.Add(inpCat);

            for (int i = 0; i < _Inputs.Length; i++)
            {
                var id = (i + 1).ToString();
                //input
                _Inputs[i].IOType = typeof(string);
                _Inputs[i].Name = "In" + id;
                _Inputs[i].Description = "String #" + id;
                inpCat.Inputs.Add(_Inputs[i]);
            }

            //setup Output IO
            Output.Name = "Concatenared String";
            Output.Description = "Concatenated string";
            Output.IOType = typeof(string);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            //base Validation
            base.Validate(ResultOutput);

            if (ForbiddenCharsValidator.IsMatch(FinalString))
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Invalid characters in string to be output"));

            for (int i = 0; i < _Inputs.Length; i++)
                if (FinalString.Contains("{" + i + "}") && !_Inputs[i].IsConnected)
                    ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Warning, "Output string references unconnected input " + i));
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            string finalString = FinalString;
            for (int i = 0; i < _Inputs.Length; i++)
            {
                if (_Inputs[i].IsConnected)
                {
                    if (_Inputs[i].Value != null)
                    {
                        var val = _Inputs[i].Value.ToStringInvariant();
                        if (!ForbiddenCharsValidator.IsMatch(val))
                        {
                            finalString = finalString.Replace("{" + i + "}", val);
                            continue;
                        }
                    }
                }

                //if no replacement was done for any reason, just remove the {n} part
                if (finalString.Contains("{" + i + "}"))
                    finalString = finalString.Replace("{" + i + "}", "");
            }

            Output.Value = finalString;

            //clean block
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
