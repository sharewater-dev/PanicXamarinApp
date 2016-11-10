using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
        FriendlyName = "JSON Single-level Deserializer",
        StencilCategory = UIStencilCategory.Logic,
        Description = "Deserializes one level of a Json String",
        FriendlyImageSource = "/Content/img/icons/Generic/jsondeserializer.png")]
    public class Json1LevelDeserializer : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public const int MaxOutputs = 10;
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InputJson { get { return _Inputs[0]; } set { _Inputs[0] = value; } }
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
                Value = @"### __JSON Single-level Deserializer__
                
This block deserializes one level of a Json String. The tokens to be captured should be configured as well as if the input is an array. Outputs can be added or removed using the right click on the block.
-- - 
*Tokens to capture:*
> Configuration of tokens to be captured.

*Deserialize Array:*
> This option should be turned to ON if the input object is an array. In this case the first input elements will be deserialized."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Json1LevelDeserializer()
            : base(1, MaxOutputs)
        {
            var outCat = new IOCategory { Name = "Outputs", MinVisibleIO = 1 };
            foreach (var o in _Outputs)
            {
                o.Name = "Deserialized Output";
                o.Description = "";
                o.IOType = typeof(string);
                outCat.Outputs.Add(o);

            }
            IOCategories.Add(outCat);


            //setup Input IO
            InputJson.Name = "JSON";
            InputJson.Description = "the JSON input string";
            InputJson.IOType = typeof(string);

            //setup Output IO
            //OutputSuccess.Name = "ResponseIsSuccess";
            //OutputSuccess.Description = "Response is success";
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
                //_initVariables = JsonInitVariables.FromJSON<Dictionary<string, string>>() ??
                //                 new Dictionary<string, string>();
            }
            catch (Exception)
            {
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Warning, "fixed parameter deserialization error"));
            }
            if (!InputJson.IsConnected)
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
            if (InputJson.State == IOState.Touched)
            {
                try
                {
                    int i;
                    if (!IsArrayDeserializer)
                    {
                        var job = (JObject)JsonConvert.DeserializeObject((string)InputJson.Value);
                        foreach (var elem in job)
                        {
                            i = Array.IndexOf(Tokens2Capture, elem.Key);
                            if (i >= 0)
                            {
                                _Outputs[i].Value = elem.Value.ToString(); //TODO
                            }
                        }
                    }
                    else
                    {
                        var jarr = (JArray)JsonConvert.DeserializeObject((string)InputJson.Value);
                        for (i = 0; i < Math.Min(jarr.Count, _Outputs.Count()); i++)
                        {
                            _Outputs[i].Value = jarr[i].ToString();
                        }
                    }
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }

            }
            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
