using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "Watchdog",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Triggers output if input doesn't receive any event within specified time period",
                 FriendlyImageSource = "/Content/img/icons/Generic/watchdog.png")]
    public class Watchdog : Block, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Watchdog period (D.HH:MM:SS)",
                     InspectorCategory = UIInspectorCategory.Configuration)]
        public TimeSpan WatchdogPeriod = TimeSpan.FromMinutes(5);
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Active High",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If set, when watchdog timer expires output is set to True, and when a valid event is received it's set to False")]
        public bool ActiveHigh = true;
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Trigger on anything",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If set, responds to any input; else only on \"triggers\" (boolean True, or any non-boolean value)")]
        public bool TriggerOnAny = false;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Watchdog__
                
This block sets its output to True if no events are received on its input within the specified time period. Lines (input-output) can be added or removed using the right click on the block.

You can increase/decrease the number of watches by right clicking on the block. Each watch is independent from the rest, tracks its respective input and sets its output accordingly.
-- - 

*Watchdog period (D.HH:MM:SS):*
> Configuration of watchdog period (Days.Hours:Minutes:Seconds).

*Active High*
> If set to true: when the watchdog timer expires, the block's output is set to True, and when a valid event is received it's reset to False.
  Otherwise, when false the behavior is the opposite: when the watchdog timer expires, the block's output is set to False, and when a valid event is received it's reset to True.

*Trigger on anything*
> If set to true: the block treats any input as a valid event that resets the watchdog timer; else only valid ""triggers"" (boolean True, or any non-boolean value) will cause the timer to be reset (i.e. the timer will not reset on False).
"
            };
        //------------------------------------------------------------------------------------------------------------------------
        private Dictionary<InputIO, Int64> PendingReqs = new Dictionary<InputIO, Int64>();
        private HashSet<InputIO> ExpiredWatches = new HashSet<InputIO>();
        private object _locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Watchdog() : this(8) { }
        //------------------------------------------------------------------------------------------------------------------------
        public Watchdog(int IOCount = 1)
            : base(IOCount, IOCount)
        {
            // Create IO categories
            var inoutCat = new IOCategory() { Name = "Watches", MinVisibleIO = 1 };
            IOCategories.Add(inoutCat);

            //setup Input IO
            foreach (var inp in _Inputs)
            {
                inp.IOType = typeof(object);
                inp.Name = "In #" + inp.Index.ToString();
                inp.Description = "Input to watch for inactivity";

                inoutCat.Inputs.Add(inp);
            }

            //setup Output IO
            foreach (var outp in _Outputs)
            {
                outp.IOType = typeof(bool);
                outp.Name = "Out #" + outp.Index.ToString();
                outp.Description = "Watchdog trigger";

                inoutCat.Outputs.Add(outp);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            if (isFirstDeploy)
            {
                YEventRouter.EventRouter.TriggerEvent(this, new BlockDeployAction() { User = User, BlockType = this.GetType(), IsPersistent = true, IsFirstDeploy = true, IsDeploy = true });
            }

            lock (_locker)
            {
                foreach (var inp in _Inputs.Where(i => i.IsConnected))
                {
                    var newId = GraphManager.SleepManager.AddWatch(this, 0, inp, WatchdogPeriod);
                    PendingReqs.TryAdd(inp, newId, true);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnUndeploy(object User)
        {
            base.OnUndeploy(User);

            lock (_locker)
                foreach (var id in PendingReqs.Values)
                    GraphManager.SleepManager.CancelRequest(id);

            YEventRouter.EventRouter.TriggerEvent(this, new BlockDeployAction() { User = User, BlockType = this.GetType(), IsPersistent = true, IsUndeploy = true });
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            foreach (var inp in _Inputs.Where(i => i.IsConnected))
            {
                var addNewWatch = false;
                lock (_locker)
                {
                    if (ExpiredWatches.Contains(inp))
                    {
                        //watchdog expired! set relevant output
                        _Outputs[inp.Index].Value = ActiveHigh;
                        ExpiredWatches.Remove(inp);
                        addNewWatch = true;
                    }

                    if ((TriggerOnAny && inp.IsTouched) || inp.IsTriggered)
                    {
                        //find pending request
                        var pendingId = PendingReqs.TryGetOrDefault(inp);
                        //we received an event from corresponding input, so cancel its watch
                        GraphManager.SleepManager.CancelRequest(pendingId);
                        _Outputs[inp.Index].Value = !ActiveHigh;
                        addNewWatch = true;
                    }

                    if (addNewWatch)
                    {
                        //being here means a. expired watch, b. new event cancelling old watch
                        //in either case, set a new watch for this connected input
                        var newId = GraphManager.SleepManager.AddWatch(this, 0, inp, WatchdogPeriod);
                        PendingReqs.TryAdd(inp, newId, true);
                    }
                }
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleExternalActionRequest(ExternalActionRequestInfo RequestInfo)
        {
            base.HandleExternalActionRequest(RequestInfo);

            lock (_locker)
                ExpiredWatches.Add((InputIO)RequestInfo.ActionData);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void InternalResetState()
        {
            base.InternalResetState();
            ExpiredWatches.Clear();
            PendingReqs.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
