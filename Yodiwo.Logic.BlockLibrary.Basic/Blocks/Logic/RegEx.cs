using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "Regular Expression Matcher",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "Matches the input strings to the desired Regex and outputs a bool flag and some of the matched strings from all input strings. The 'HasMatches' output returns true if there was any match, elsewhere returns false.",
                              FriendlyImageSource = "/Content/img/icons/Generic/regex.png")]
    public class RegEx : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO BoolOutput { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        #region UIBasicInfos
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Regular Expression",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Regular expression to evaluate")]
        public string regexString = "[enter your regular expression here]";
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = false,
            FriendlyName = "Output Type",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Specify whether captures or all text will be passed in the output string(s).",
            IsAdvanced = true)]
        public OutputType OutputFormat = OutputType.OnlyCaptures;
        //------------------------------------------------------------------------------------------------------------------------
        public enum OutputType
        {
            All,
            OnlyCaptures,
        }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Text Direction",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Specify the search direction.",
            IsAdvanced = true)]
        public TextDirection SearchDirection;
        //------------------------------------------------------------------------------------------------------------------------
        public enum TextDirection
        {
            LeftToRight,
            RightToLeft,
        }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Ignore Case",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Specify if the matcher should or should not ignore the case.")]
        public bool IgnoreCase;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Line mode",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = " Use single-line mode, where the period (.) matches every character (instead of every character except \\n). "
                        + "Or use multiline mode, where ^ and $ match the beginning and end of each line (instead of the beginning and the end of the input string).",
            IsAdvanced = true)]
        public LineOptions SingleOrMultiLine;
        //------------------------------------------------------------------------------------------------------------------------
        public enum LineOptions
        {
            Singleline,
            Multiline,
        }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Culture Invariant",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Specifies that cultural differences in language are ignored",
            IsAdvanced = true)]
        public bool IsCultureInvariant;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Regular Expression Matcher__
                
This block matches the input strings to the desired Regex and outputs a bool flag and some of the matched strings from all input strings. The 'HasMatches' output returns true if any match, elsewhere returns false. Inputs/Outputs can be added or removed using the right click on the block.
-- - 
*Regular Expression:*
> Specifies the regular expression to be evalueated. To test expected outcome, take a look at https://regex101.com.

*Output Type:*
> Specify whether captures or all text will passed in the output string(s).

*Text Direction:*
> Configuration of the search direction.

*Ignore Case:*
> Configures if the matcher should or should not ignore the case.

*Line mode:*
> Use single-line mode, where the period (.) matches every character. Otherwise, use multiline mode where ^ and $ match the beginning and the end of each line (instead of the beginning and the end of the input string).

*Culture Invariant:*
> Configures if cultural differences in language are ignored."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public RegEx()
            : base(1, 10)
        {
            // The minimum size of showing output strings of this category are 0
            var outCat = new IOCategory() { Name = "String Outputs", MinVisibleIO = 0 };
            IOCategories.Add(outCat);

            // setup Input
            _Inputs[0].IOType = typeof(string);
            _Inputs[0].Name = "In";
            _Inputs[0].Description = "Input string";

            // first output will be true or false
            _Outputs[0].Name = "HasMatches";
            _Outputs[0].Description = "If there are matched strings returns true, else returns false";
            _Outputs[0].IOType = typeof(bool);


            //setup Output
            for (int i = 1; i < _Outputs.Length; i++)
            {
                var id = i.ToStringInvariant();
                // output
                _Outputs[i].Name = "String #" + id;
                _Outputs[i].Description = "Captured String #" + id;
                _Outputs[i].IOType = typeof(string);
                outCat.Outputs.Add(_Outputs[i]);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            //base Validation
            base.Validate(ResultOutput);

#if false   //hide too many info messages
            // check output format
            if (OutputFormat == OutputType.All)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Info, "All matched text will be passed in the output strings."));
            else
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Info, "Only captures will be passed in the output strings."));
            // check search direction
            if (SearchDirection == TextDirection.RightToLeft)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Info, "Right to Left search direction is enabled."));
            // check case-sensivity 
            if (IgnoreCase)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Info, "Case will be ignored."));
            // check multi line of single line mode
            if (SingleOrMultiLine == LineOptions.Multiline)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Info, "Multiline mode is enabled."));
            // check if is CultureInvariant
            if (IsCultureInvariant)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Info, "Cultural differences in language are ignored."));
#endif
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            // create Regex object 
            RegexOptions options = getOptions();
            Regex rx = new Regex(regexString, options);
            List<String> OutputStrings = new List<String>();
            bool hasMatch = false;

            try
            {
                for (var i = 0; i < _Inputs.Length; i++)
                {
                    // try to match non-null connected Input
                    if (_Inputs[i].IsConnected && _Inputs[i].Value != null)
                    {
                        // check whether the output should be all text or captures only
                        switch (OutputFormat)
                        {
                            case OutputType.All:
                                MatchCollection matches = rx.Matches((string)_Inputs[i].Value);
                                if (matches.Count != 0)
                                {
                                    hasMatch = true;
                                    foreach (var m in matches)
                                        OutputStrings.Add(m.ToString());
                                }
                                break;
                            case OutputType.OnlyCaptures:
                                Match match = Regex.Match((string)_Inputs[i].Value, regexString);
                                if (match.Success)
                                {
                                    hasMatch = true;
                                    foreach (Group group in match.Groups)
                                    {
                                        foreach (Capture capture in group.Captures)
                                        {
                                            OutputStrings.Add(capture.Value);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                // assign boolean output
                if (BoolOutput.IsConnected && hasMatch)
                    BoolOutput.Value = true;

                // assign string output
                if (OutputStrings.Count > 0)
                {
                    // Documentation in https://msdn.microsoft.com/en-us/library/system.text.regularexpressions.group.captures(v=vs.110).aspx
                    // skips 1rst capture of the 1rst group because it contains the entire matched string
                    // so we skip the first Element at OutputStrings if it contains more that one element
                    if (OutputStrings.Count > 1)
                        OutputStrings.RemoveAt(0);

                    // for every string output assing the next available match 
                    // skip the boolean output _Outputs[0]
                    int count = 0;
                    for (var i = 1; i < _Outputs.Length; i++)
                    {
                        if (_Outputs[i].IsConnected)
                        {
                            // assign next available match
                            if (count < OutputStrings.Count)
                            {
                                _Outputs[i].Value = OutputStrings.ElementAt(count);
                                count++;
                            }
                        }
                    }
                }
                else
                    BoolOutput.Value = false;
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }

            //clean block
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private RegexOptions getOptions()
        {
            RegexOptions opt = RegexOptions.None;
            // check search direction
            if (SearchDirection == TextDirection.RightToLeft)
                opt |= RegexOptions.RightToLeft;
            // check case-sensivity 
            if (IgnoreCase)
                opt |= RegexOptions.IgnoreCase;
            // check multi line of single line mode
            if (SingleOrMultiLine == LineOptions.Multiline)
                opt |= RegexOptions.Multiline;
            else
                opt |= RegexOptions.Singleline;
            // check if is CultureInvariant
            if (IsCultureInvariant)
                opt |= RegexOptions.CultureInvariant;

            return opt;
        }
        #endregion
    }
}
