using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "CLOCK",
                 StencilCategory = UIStencilCategory.Input,
                 Description = "Generates a clock signal with the specified properties",
                 FriendlyImageSource = "/Content/img/icons/Generic/clock.png")]
    public class Clock : EndpointIn, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Clock period",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Clock period in seconds")]
        public TimeSpan Period = TimeSpan.FromSeconds(6);
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Pulse width",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Pulse width in seconds")]
        public TimeSpan PulseWidth = TimeSpan.FromSeconds(3);
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Clock__
                
This block generates a clock signal with the specified properties.
-- - 
*Clock period:*
> Configuration of clock period in seconds.

*Pulse width:*
> Configuration of pulse width in seconds."
            };
        //------------------------------------------------------------------------------------------------------------------------
        private bool internalState;
        private Int64 currentSleepId;
        private object _locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        static readonly TimeSpan minPeriod = TimeSpan.FromSeconds(1);
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Clock()
            : base(0, 1)
        {
            Output.Name = "Clock signal";
            Output.Description = "";
            Output.Value = false;
            Output.IOType = typeof(bool);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            //base Validation
            base.Validate(ResultOutput);

            //check that clock is connected
            if (!Output.IsConnected)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Warning, "Did you really mean to have the clock unconnected?"));

            //validate period
            if (Period < minPeriod)
                ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Warning, "Period cannot be smaller that " + minPeriod.TotalSeconds + " sec (clamped to minimum)"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            if (!Output.IsConnected)
            {
                DebugEx.TraceLog("Avoiding starting up unconnected clock block, key: " + this.BlockKey);
                return;
            }
            //clamp period
            if (Period < minPeriod)
            {
                Period = minPeriod;
                PulseWidth = TimeSpan.FromMilliseconds(minPeriod.TotalMilliseconds / 2d);
            }

            lock (_locker)
            {
                DebugEx.Assert(currentSleepId == 0, "clock unexpected event");
                if (currentSleepId != 0)
                    SleepCancel(currentSleepId);
            }

            if (isFirstDeploy)
                YEventRouter.EventRouter.TriggerEvent(this, new BlockDeployAction()
                {
                    User = User,
                    BlockType = this.GetType(),
                    IsPersistent = true,
                    IsFirstDeploy = true,
                    IsDeploy = true,
                });

            //initial event
            if (GraphManager == null)
                DebugEx.Assert("No GraphManager found");
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnUndeploy(object User)
        {
            base.OnUndeploy(User);

            lock (_locker)
            {
                if (currentSleepId != 0)
                {
                    SleepCancel(currentSleepId);
                    currentSleepId = 0;
                }
            }
            YEventRouter.EventRouter.TriggerEvent(this, new BlockDeployAction() { User = User, BlockType = this.GetType(), IsPersistent = true, IsUndeploy = true });
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (Output.IsConnected)
            {
                DebugEx.Assert(currentSleepId == 0, "clock unexpected event");

                //set output state
                Output.Value = internalState;
                //external-event sleep
                var sleepTime = internalState ? PulseWidth : Period - PulseWidth;
                //set next sleep event
                lock (_locker)
                    currentSleepId = Sleep(sleepTime, 0, 0, true);
            }
            // Clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleExternalActionRequest(ExternalActionRequestInfo RequestInfo)
        {
            base.HandleExternalActionRequest(RequestInfo);
            // Toggle state
            internalState = !internalState;
            currentSleepId = 0;
        }
        #endregion
    }
}

