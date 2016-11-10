using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "PULSE",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Generates an active-high pulse with width equal to the Pulse duration property in the inspector",
                 FriendlyImageSource = "/Content/img/icons/Generic/pulse.svg")]
    public class Pulse : Block, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Pulse duration",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Pulse duration")]
        public TimeSpan DelayTime = TimeSpan.FromSeconds(1);
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Active high pulse",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If set, a low->high pulse is generated, otherwise a high->low one")]
        public bool ActiveHigh = true;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Activate only on trigger",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If set, only a valid trigger (any non boolean value, or boolean True) will generate a pulse")]
        public bool OnlyOnTrigger = true;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Pulse__
                
This block generates an active-high or active-low pulse of configurable width.
-- - 
*Pulse duration:*
> Specifies the pulse width (Days.Hours:Minutes:Seconds).

*Active high pulse:*
> If set, a low->high pulse is generated, otherwise a high->low one

*Only On Trigger:*
> If set, only a valid trigger (any non boolean value, or boolean True) will generate a pulse"
            };
        //------------------------------------------------------------------------------------------------------------------------
        private DictionaryTS<InputIO, Int64> PendingReqs = new DictionaryTS<InputIO, Int64>();
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Pulse() : this(8) { }

        public Pulse(int IOCount = 1)
            : base(IOCount, IOCount)
        {
            // Create IO categories
            var inoutCat = new IOCategory() { Name = "Pulses", MinVisibleIO = 1 };
            IOCategories.Add(inoutCat);

            //setup Input IO
            foreach (var inp in _Inputs)
            {
                inp.IOType = typeof(object);
                inp.Name = "In #" + inp.Index.ToString();
                inp.Description = "";

                inoutCat.Inputs.Add(inp);
            }

            //setup Output IO
            foreach (var outp in _Outputs)
            {
                outp.Name = "Pulse #" + outp.Index.ToString();
                outp.Description = "Output pulse";
                outp.IOType = typeof(bool);

                inoutCat.Outputs.Add(outp);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            base.Validate(ResultOutput);

            if (DelayTime < TimeSpan.FromSeconds(1))
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "DelayTime must be at least 1 second"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            foreach (var inp in _Inputs.Where(i => i.IsConnected))
            {
                var pulseOngoing = false;
                var pendingId = PendingReqs.TryGetOrDefault(inp, 0);
                if (pendingId != 0)
                {
                    pulseOngoing = true;
                    GraphManager.SleepManager.CancelRequest(pendingId);
                    PendingReqs.Remove(inp);
                }
                if (inp.IsTouched)
                {
                    if ((inp.IsTriggered || !OnlyOnTrigger))
                    {
                        if (!pulseOngoing) //if ongoing pulse, silently extend it, do not retrigger
                            _Outputs[inp.Index].Value = ActiveHigh;

                        pendingId = GraphManager.SleepManager.SleepExternal(this, 0, 0, DelayTime);
                        PendingReqs.Add(inp, pendingId);
                    }
                }
                else    //untouched input and we're here -> pulse ended   TODO: not necessarily if there are >1 lines!
                {
                    _Outputs[inp.Index].Value = !ActiveHigh;
                }
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
