using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Counter",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Counts up/down to the configured value and resets",
                 FriendlyImageSource = "/Content/img/icons/Generic/counter.png")]
    public class Counter : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO InPulse { get { return _Inputs[0]; } }
        public InputIO InNumber { get { return _Inputs[1]; } }
        public InputIO Reset { get { return _Inputs[2]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutCurValue { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        public OutputIO OutWrapEv { get { return _Outputs[1]; } set { _Outputs[1] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Starting value",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Set value from which to start counting")]
        public int StartingValue = 0;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Counter wrap value",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "value that's being counted (reaching this this will wrap the counter), overriden by input port if connected")]
        public int WrapValue = 0;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Count down",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "if true counter counts downwards (starting from the wrap value)")]
        public bool CountDown = false;
        //------------------------------------------------------------------------------------------------------------------------
        private int CurrentCount;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Counter__
                
This block counts to the configured wrap value and then restarts from the configured starting value. The counter will ***never*** have its output set to the wrap value, at that point it is reset to its starting value.

The counter counts integer values; it can handle any range, negative, positive or mixed; upwards or downwards.
-- - 
*Counter starting value:*
> This is the value that the counter starts counting from graph deployment, wrap, or reset.

*Counter wrap value:*
> This is the value that is being counted. When reaching the value, the counter is wrapped and set to its starting value.

*Count down:*
> If it is true the counter counts downwards (starting from the wrap value). Otherwise it counts upwards."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Counter()
            : base(3, 2)
        {
            //setup Input IO
            InPulse.IOType = typeof(object);
            InPulse.Name = "Trigger";
            InPulse.Description = "counts events";

            InNumber.IOType = typeof(int);
            InNumber.Name = "Add_Num";
            InNumber.Description = "adds input (number) to current counter value";

            Reset.IOType = typeof(bool);
            Reset.Name = "Reset";
            Reset.Description = "If triggered, the counter will reset to its starting point";

            //setup Output IO
            OutCurValue.Name = "CurrentValue";
            OutCurValue.Description = "";
            OutCurValue.IOType = typeof(int);

            OutWrapEv.Name = "WrapEvent";
            OutWrapEv.Description = "";
            OutWrapEv.IOType = typeof(bool);

        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            if (StartingValue == WrapValue)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "starting value cannot be equal to wrap value"));

            if (CountDown && StartingValue < WrapValue)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "when counting down, starting value must be larger than wrap value"));

            if (!CountDown && StartingValue > WrapValue)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "when counting up, starting value must be smaller than wrap value"));

            if (!InNumber.IsConnected && !InPulse.IsConnected)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Warning, "both inputs unconnected in counter"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            CurrentCount = StartingValue;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            var triggered = false;
            var cntWrapped = false;
            var stepSize = (int)InNumber.Value;

            //check reset
            if (Reset.IsTriggered)
            {
                triggered = true;
                CurrentCount = StartingValue;
            }

            //check pulsed input
            if (InPulse.IsTriggered)
            {
                triggered = true;
                if (CountDown)
                {
                    if (--CurrentCount <= StartingValue)
                    {
                        CurrentCount = WrapValue;
                        cntWrapped = true;
                    }
                }
                else
                {
                    if (++CurrentCount >= WrapValue)
                    {
                        CurrentCount = StartingValue;
                        cntWrapped = true;
                    }
                }
            }

            //check numbered input
            if (InNumber.IsConnected && InNumber.IsTouched)
            {
                triggered = true;

                if (CountDown)
                {
                    CurrentCount -= stepSize;
                    if (CurrentCount <= StartingValue)
                    {
                        CurrentCount %= StartingValue - WrapValue;
                        cntWrapped = true;
                    }
                }
                else
                {
                    CurrentCount += stepSize;
                    if (CurrentCount >= WrapValue)
                    {
                        CurrentCount %= WrapValue - StartingValue;
                        cntWrapped = true;
                    }
                }
            }

            if (triggered)
            {
                //apply output
                OutCurValue.Value = CurrentCount;
                OutWrapEv.Value = cntWrapped ? true : false;
            }
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}


