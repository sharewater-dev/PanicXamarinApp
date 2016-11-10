using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Dictation Parser",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Parses a phrase",
                 FriendlyImageSource = "/Content/img/icons/Generic/equals.png")]
    public class DictationParser : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Input { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputRecognised { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        public const int OutputValuesOffset = 1;
        public const int OutputValuesCount = 5;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Recognition templates",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "A text array specifying text strings that the block's input should be compared to.")]
        public string[] Templates;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "...",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "...")]
        public string[] Keywords;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Dictation Parser__"
            };
        //------------------------------------------------------------------------------------------------------------------------
        public string[] _RegExTemplates;
        public int[][] _RegExKeywordIndex;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public DictationParser()
            : base(1, 1 + OutputValuesCount)
        {
            // Create IO categories
            var outCat = new IOCategory() { Name = "Values", MinVisibleIO = 1 };
            IOCategories.Add(outCat);

            //setup input
            Input.IOType = typeof(string);
            Input.Name = "Text";
            Input.Description = "Input Text";

            //setup output
            OutputRecognised.Name = "Index";
            OutputRecognised.Description = "Recognised Index";
            OutputRecognised.IOType = typeof(int);

            //setup Output IO
            for (var i = 0; i < OutputValuesCount; i++)
            {
                var id = (i + 1).ToString();

                _Outputs[i + OutputValuesOffset].Name = "Value";
                _Outputs[i + OutputValuesOffset].Description = "Output #" + id;
                _Outputs[i + OutputValuesOffset].IOType = typeof(string);

                outCat.Outputs.Add(_Outputs[i + OutputValuesOffset]);
            }
            Templates = new string[1] { "set {the}{a} value {of} [intensity] to {the}{light} [lightname]" };
            Keywords = new string[2] { "intensity", "lightname" };
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            //preprocess
            for (int n = 0; n < Keywords.Length; n++)
                Keywords[n] = Keywords[n].ToLowerInvariant();

            //process templates into regex
            _RegExTemplates = new string[Templates.Length];
            _RegExKeywordIndex = new int[Templates.Length][];
            for (int ti = 0; ti < Templates.Length; ti++)
            {
                try
                {
                    //init pattern
                    var template = Templates[ti];
                    var pattern = "";
                    var indexList = new List<int>();

                    //fix spaces
                    colapseSpaces(template);

                    //construct regex pattern
                    int mode = 0;
                    var token = "";
                    var lastc = '.';
                    foreach (var c in template)
                    {
                        if (mode == 0)
                        {
                            if (c == '[')
                                mode = 1;
                            else if (c == '{')
                                mode = 2;
                            else
                            {
                                if (c == ' ' && (lastc == ' ' || lastc == '}'))
                                { } //ignore white space?
                                else
                                    pattern += escapeRegEx(c);
                            }
                        }
                        else if (mode == 1)
                        {
                            if (c == ']')
                            {
                                mode = 0;
                                pattern += @"([A-Za-z0-9\-\,\.\!\?]+)";
                                var ind = Array.IndexOf(Keywords, token.ToLowerInvariant());
                                indexList.Add(ind);
                                token = "";
                            }
                            else
                                token += c;
                        }
                        else if (mode == 2)
                        {
                            if (c == '}')
                            {
                                mode = 0;
                                if (token == "*")
                                    pattern += @"([A-Za-z0-9\-\,\.\!\?\s]+)??";
                                else
                                    pattern += @"(" + escapeRegEx(token) + @")?";
                                var ind = Array.IndexOf(Keywords, token.ToLowerInvariant());
                                indexList.Add(-1);
                                //spaces
                                pattern += @"(\s*?)";
                                indexList.Add(-1);
                                token = "";
                            }
                            else
                                token += c;
                        }
                        //keep
                        lastc = c;
                    }

                    //keep assigned indices
                    _RegExKeywordIndex[ti] = indexList.ToArray();

                    //keep regex pattern
                    _RegExTemplates[ti] = pattern;
                }
                catch (Exception ex) { DebugEx.Assert(ex); }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (Input.IsConnected && Input.IsTouched && Input.Value != null)
            {
                var inputStr = (string)Input.Value;

                //fix spaces
                colapseSpaces(inputStr);

                //reset output
                OutputRecognised.Value = 0;
                for (var i = 0; i < OutputValuesCount; i++)
                    _Outputs[i + OutputValuesOffset].Value = null;

                //process templates
                for (int ti = 0; ti < Templates.Length; ti++)
                {
                    try
                    {
                        var pattern = _RegExTemplates[ti];
                        var indices = _RegExKeywordIndex[ti];

                        //regex match
                        var match = Regex.Match(inputStr, pattern, RegexOptions.IgnoreCase);
                        if (match.Success == false)
                            continue;

                        //set template index
                        OutputRecognised.Value = ti + 1;

                        //process matches
                        var len = Math.Min(indices.Length, match.Groups.Count - 1);
                        for (int mi = 0; mi < len; mi++)
                            if (indices[mi] >= 0 && indices[mi] < OutputValuesCount)
                                Outputs[indices[mi] + OutputValuesOffset].Value = match.Groups[mi + 1].Value;

                        //stop trying to match templates
                        break;
                    }
                    catch (Exception ex) { DebugEx.Assert(ex); }
                }
            }

            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        string colapseSpaces(string input)
        {
            return Regex.Replace(input, @"\s+", " ");
        }
        //------------------------------------------------------------------------------------------------------------------------
        string escapeRegEx(string input)
        {
            return input.Replace(@"\", @"\\")
                        .Replace("[", @"\[").Replace("]", @"\]")
                        .Replace("(", @"\(").Replace(")", @"\)")
                        .Replace("+", @"\+").Replace("-", @"\-").Replace("*", @"\*").Replace("?", @"\?")
                        ;
        }
        //------------------------------------------------------------------------------------------------------------------------
        string escapeRegEx(char input)
        {
            if (input == '\\') return @"\\";
            if (input == '[') return @"\[";
            if (input == ']') return @"\]";
            if (input == '(') return @"\(";
            if (input == ')') return @"\)";
            if (input == '+') return @"\+";
            if (input == '-') return @"\-";
            if (input == '?') return @"\?";
            if (input == '*') return @"\*";

            return input.ToString();
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
