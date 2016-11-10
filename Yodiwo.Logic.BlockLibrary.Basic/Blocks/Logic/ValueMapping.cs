using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Value Mapping",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Maps a specific key to a specific value",
                 FriendlyImageSource = "/Content/img/icons/Generic/valuemapping.png")]
    public class ValueMapping : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputValue { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputValue { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Keys",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Key list")]
        public string[] Keys = new string[] { "$default$", "key1", "key2" };
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Values",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Value list")]
        public string[] Values = new string[] { "default output", "value1", "value2" };
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Case Sensitive",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Case Sensitive key lookup")]
        public bool CaseSensitive = false;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Value Mapping__
                
Maps a specific key to a specific value (also known as dictionary).

The keys must be unique.. If no key is found for an input the block will try to find a default value with key ""$default$""
-- - 
"
            };
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [Newtonsoft.Json.JsonIgnore]
        Dictionary<string, string> mapping = new Dictionary<string, string>();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ValueMapping()
            : base(1, 1)
        {
            //setup Input IO
            InputValue.IOType = typeof(string);
            InputValue.Name = "Input";
            InputValue.Description = "Input value that will be used a key for the mapping";

            //setup Output IO
            OutputValue.Name = "Output";
            OutputValue.Description = "The value of the mapping";
            OutputValue.IOType = typeof(string);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            if (Keys == null || Values == null)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Null Keys/Values detected"));

            if (Keys.Length != Values.Length)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Keys/Values items do not match. (You must have as many keys as values)"));

            var keyset = CaseSensitive ? Keys.Select(k => k.ToLowerInvariant()).ToHashSet() : Keys.ToHashSet();
            if (keyset.Count != Keys.Length)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "Keys must be unique"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            //setup mappings
            for (int n = 0; n < Keys.Length; n++)
            {
                var key = Keys[n];
                if (CaseSensitive) key = key.ToLowerInvariant();
                mapping.Add(key, Values[n]);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (!InputValue.IsConnected)
                yield return BlockState.Clean;
            else
            {
                //get input
                var inpKey = InputValue.Value as string;
                if (inpKey == null)
                    yield return BlockState.Clean;
                else
                {
                    //case sensitive?
                    if (CaseSensitive) inpKey = inpKey.ToLowerInvariant();

                    //find mapping
                    bool ConsumeEvent = false;
                    bool isDefault = false;
                    string value;
                    if (!mapping.TryGetValue(inpKey, out value))
                    {
                        isDefault = true;
                        if (!mapping.TryGetValue("$default$", out value))
                            ConsumeEvent = true;
                    }

                    //set output
                    if (!isDefault || !ConsumeEvent)
                        OutputValue.Value = value;

                    //clean
                    yield return BlockState.Clean;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


