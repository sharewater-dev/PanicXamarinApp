using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
        FriendlyName = "JSON Single-level Serializer",
        StencilCategory = UIStencilCategory.Logic,
        Description = "Serializes inputs into a single level Json String",
        FriendlyImageSource = "/Content/img/icons/Generic/jsondeserializer.png")]
    public class Json1LevelSerializer : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public const int MaxInputs = 10;
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputJson { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        //public OutputIO OutputSuccess { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //public OutputIO OutputStatus { get { return _Outputs[1]; } set { _Outputs[1] = value; } }
        //public OutputIO OutputBody { get { return _Outputs[2]; } set { _Outputs[2] = value; } }
        //------------------------------------------------------------------------------------------------------------------------

        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Tokens to capture",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Tokens to capture")]
        public string[] Tokens2Capture;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Deserialize Array",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "The input object is an array, deserialize the first #inputs elements")]
        public bool IsArrayDeserializer;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __JSON Single-level Serializer__
                
This block Serializes its inputs into a one level Json String. In the non-array mode, input values are used as JSON values, while the respective JSON member names need filled in the block configuration.
In the array mode, JSON names do not have any meaning, and are ignored.
-- - 
*Tokens to capture:*
> Names .

*Serialize into array:*
> Create a JSON array instead of an object. JSON names need not be configured."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Json1LevelSerializer()
            : base(MaxInputs, 1)
        {
            var inCat = new IOCategory { Name = "Inputs", MinVisibleIO = 1 };
            foreach (var i in _Inputs)
            {
                i.Name = "Input";
                i.Description = "";
                i.IOType = typeof(string);
                inCat.Inputs.Add(i);

            }
            IOCategories.Add(inCat);

            //setup Input IO
            OutputJson.Name = "JSON";
            OutputJson.Description = "the JSON output string";
            OutputJson.IOType = typeof(string);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            //base Validation
            base.Validate(ResultOutput);

            try
            {
            }
            catch (Exception)
            {
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Warning, "fixed parameter deserialization error"));
            }
            if (!OutputJson.IsConnected)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Warning, "Input not connected"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void InternalResetState()
        {
            base.InternalResetState();
            //reset intermediate results 
        }

        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            try
            {
                if (!IsArrayDeserializer)
                {
                    var job = new JObject();
                    for (var i = 0; i < Math.Min(Tokens2Capture.Length, _Inputs.Length); i++)
                    {
                        if (_Inputs[i].IsConnected)
                        {
                            job.Add(Tokens2Capture[i], (string)_Inputs[i].Value);
                        }
                    }
                    OutputJson.Value = job.ToJSON();
                }
                else
                {
                    var jarr = new JArray();
                    foreach (var t in _Inputs.Where(i => i.IsConnected))
                    {
                        jarr.Add((string)t.Value);
                    }
                    OutputJson.Value = jarr.ToJSON();
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
