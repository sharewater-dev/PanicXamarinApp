using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Text matcher",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Outputs true or false at its output depending on whether its input matches the specified text",
                 FriendlyImageSource = "/Content/img/icons/Generic/equals.png")]
    public class TextMatcher : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Comparison text array",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "A text array specifying text strings that the block's input should be compared to. A match will set the output to the index [1..N] that was" +
                                   "matched. If there was no match, the output will be set to 0. Hence for single text blocks it acts as true-false matching")]
        public string[] TextToMatch;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Case Sensitive",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Set to true to make all comparisons case-sensitive")]
        public bool CaseSensitive;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Exact Match",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Set to true to match exact length. If false, we will try to match anywhere within the incoming text")]
        public bool ExactMatch;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Zero On No Match",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If true, output will be set to 0 if there is no match; if false, output will not be touched at all if there is no match")]
        public bool ZeroOnNoMatch;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Start from zero",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If true matched entries start from 0 instead of one and count upwards")]
        public bool StartFromZero;
        //------------------------------------------------------------------------------------------------------------------------
        private StringComparison StringComparisonOpts;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Text Matcher__
                
This block outputs the number corresponding to the matched entry of the compared-to text array.

Lines (input-output) can be added or removed by right clicking on the block.

-- - 
#### __Important Notes__ ####
 Contrary to common programming practice, counting seemingly starts from 1 in this block, i.e. a match on the 4th array entry will set the output to 4.

This is because if there's no match, the output may be set to 0 (see *Zero On No Match* configuration). Hence, for single-text-entry blocks, it acts as simple true-false matching.

To override this behavior and start counting from 0, see the 'Start from zero' switch

Obviously 'Start from zero' cannot be used in conjuction with the 'Zero on no Match' option.

Finally note that if this block's output is connected to a 'trigger-type' port, a value 0 will *not* trigger the next block!

-- - 
*Comparison text array:*
> A text array specifying text strings that the block's input should be compared to. A match will set the output to the index [1..N] that was matched. If there was no match, the output will be set to 0. Hence for single text blocks it acts as true-false matching.

*Case Sensitive:*
> If set, comparisons will be case-sensitive.

*Exact Match:*
> If set, the block must match exact length of input. Otherwise, the block will try to match anywhere within the incoming text.

*Zero On No Match:*
> If true, output will be set to 0 if there is no match; if false, output will not be touched at all if there is no match.

*Start from zero:*
> If true, output will be set to 0 for 1st match, 1 for 2nd and so on; If there's no match, there will be no output."

            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public TextMatcher() : this(8) { }
        //------------------------------------------------------------------------------------------------------------------------
        public TextMatcher(int IOCount = 1)
            : base(IOCount, IOCount)
        {
            // Create IO categories
            var inoutCat = new IOCategory() { Name = "Lines", MinVisibleIO = 1 };
            IOCategories.Add(inoutCat);

            //setup Input IO
            for (var i = 0; i < _Inputs.Length; i++)
            {
                var id = (i + 1).ToString();

                _Inputs[i].IOType = typeof(string);
                _Inputs[i].Name = "Text";
                _Inputs[i].Description = "Input #" + id;

                inoutCat.Inputs.Add(_Inputs[i]);
            }

            //setup Output IO
            for (var i = 0; i < _Outputs.Length; i++)
            {
                var id = (i + 1).ToString();

                _Outputs[i].Name = "Result";
                _Outputs[i].Description = "Output #" + id;
                _Outputs[i].IOType = typeof(int);

                inoutCat.Outputs.Add(_Outputs[i]);
            }
            TextToMatch = new string[1];
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            if (StartFromZero && ZeroOnNoMatch)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "When StartFromZero is active, ZeroOnNoMatch cannot be also active"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            for (var j = 0; j < _Inputs.Length; j++)
            {
                if (_Inputs[j].IsConnected && _Inputs[j].IsTouched && _Inputs[j].Value != null)
                {
                    var inputStr = (string)_Inputs[j].Value;


                    if (ZeroOnNoMatch)
                        //zero is the value not matching anything
                        _Outputs[j].Value = 0;

                    if (ExactMatch)
                    {
#if NETFX
                        StringComparisonOpts = CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
#else
                        StringComparisonOpts = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
#endif
                        for (int i = 0; i < TextToMatch.Length; i++)
                        {
                            if (0 == string.Compare(inputStr, TextToMatch[i], StringComparisonOpts))
                            {
                                _Outputs[j].Value = i + (StartFromZero ? 0 : 1);
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < TextToMatch.Length; i++)
                        {
                            if (inputStr.ContainsInvariant(TextToMatch[i], CaseSensitive) ||
                                TextToMatch[i].ContainsInvariant(inputStr, CaseSensitive))
                            {
                                _Outputs[j].Value = i + (StartFromZero ? 0 : 1);
                                break;
                            }
                        }
                    }
                }
            }
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
