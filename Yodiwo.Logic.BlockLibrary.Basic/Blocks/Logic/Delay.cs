using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [UIBasicInfo(IsVisible = true,
                 FriendlyName = "DELAY",
                 StencilCategory = UIStencilCategory.Logic,
                 Description = "Delays input to output relay",
                 FriendlyImageSource = "/Content/img/icons/Generic/delay.png")]
    public class Delay : Block, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Input { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO Output { set { _Outputs[0] = value; } get { return _Outputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Delay time (D.H:M:S)",
                     InspectorCategory = UIInspectorCategory.Configuration)]
        public TimeSpan DelayTime = TimeSpan.FromSeconds(1);
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Real Time",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "Real Time: Delay starts counting from the respective input events\n" +
                     "Non Real Time: Delay starts counting after the last output event, for close enough input events")]
        public bool RealTime = false;
        //------------------------------------------------------------------------------------------------------------------------
        class DelayedEvent
        {
            public int EventId;
            public List<TupleS<int, object>> Events = new List<TupleS<int, object>>();
        }
        private DelayedEvent _currentEvent = null;
        private int _pendingEventID = 0;

        private Queue<DelayedEvent> _eventQueue = new Queue<DelayedEvent>();
        //keeps track of ongoing a possible non RT delay. not used in RT mode
        private bool _nonRtInProgress;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Delay__
                
This block delays input to output relay. The delay time should be configured.

Lines (input-output) can be added or removed by right clicking on the block.
-- - 
*Delay time (D.H:M:S):*
> Configuration of delay time (Days.Hours:Minutes:Seconds).

*Real time:*
> If set, delay starts counting from the respective input events. If not, delay starts counting after the last output event."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Delay() : this(8) { }
        //------------------------------------------------------------------------------------------------------------------------
        public Delay(int IOCount = 1)
            : base(IOCount, IOCount)
        {
            // Create IO categories
            var inoutCat = new IOCategory() { Name = "Lines", MinVisibleIO = 1 };
            IOCategories.Add(inoutCat);
            Output.IOType = typeof(object);


            //setup Input IO
            foreach (var inp in _Inputs)
            {
                inp.IOType = typeof(object);
                inp.Name = "Input Data";
                inp.Description = "";

                inoutCat.Inputs.Add(inp);
            }

            //setup Output IO
            foreach (var outp in _Outputs)
            {
                outp.Name = "Output Data";
                outp.Description = "";

                inoutCat.Outputs.Add(outp);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (Graph.IsWarmupSolve)
            {
                //---------------------------
                // apply solution to outputs and clean
                //---------------------------
                for (int n = 0; n < _Inputs.Length; n++)
                    if (_Inputs[n].IsTouched)
                        _Outputs[n].Value = _Inputs[n].Value;
                //clean
                yield return BlockState.Clean;
            }
            else
            {
                if (_Outputs.Any(o => o.IsConnected))
                {
                    var modeSolver = RealTime ? SolveRt() : SolveNonRt();
                    foreach (var result in modeSolver)
                        yield return result;
                }
                else
                    yield return BlockState.Clean;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected IEnumerable<BlockState> SolveNonRt()
        {
            //---------------------------
            // Request sleep and hold
            //---------------------------
            Sleep(DelayTime);
            yield return BlockState.OnHold;

            //---------------------------
            // apply solution to outputs and clean
            //---------------------------
            for (int n = 0; n < _Inputs.Length; n++)
                if (_Inputs[n].IsTouched)
                    _Outputs[n].Value = _Inputs[n].Value;
            //clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected IEnumerable<BlockState> SolveRt()
        {
            if (_currentEvent == null || _Inputs.Any(io => io.IsTouched))
            {
                //generate id
                var id = ++_pendingEventID;
                //create vent
                var delayed = new DelayedEvent
                {
                    EventId = id,
                };
                for (var n = 0; n < _Inputs.Length; n++)
                {
                    if (_Inputs[n].IsTouched)
                        delayed.Events.Add(new TupleS<int, object>(n, _Inputs[n].Value));
                }

                //send request to subsystem (and return clean  since this is external)
                Sleep(DelayTime, ActionData: delayed, isExternal: true);
                yield return BlockState.Clean;
            }
            else if (_currentEvent != null)
            {
                //touch outputs with values
                //if (_currentEvent.EventId == _pendingEventID) <-- can be enabled if we want to keep latest only
                {
                    //touch outputs with values
                    foreach (var ev in _currentEvent.Events)
                        _Outputs[ev.Item1].Value = ev.Item2;
                }

                //clear event
                _currentEvent = null;
                yield return BlockState.Clean;

            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected IEnumerable<BlockState> SolveSemiRt()
        {
            var newEvent = CaptureEvent();
            if (newEvent != null)
                _eventQueue.Enqueue(newEvent);

            if (!_nonRtInProgress && _eventQueue.Count > 0)
            {
                _nonRtInProgress = true;
                //send request to subsystem (and return clean  since this is external)
                Sleep(DelayTime, ActionData: _eventQueue.Dequeue(), isExternal: true);
            }
            if (_currentEvent != null)
            {
                //touch outputs with values
                //if (_currentEvent.EventId == _pendingEventID) <-- can be enabled if we want to keep latest only
                {
                    //touch outputs with values
                    foreach (var ev in _currentEvent.Events)
                        _Outputs[ev.Item1].Value = ev.Item2;
                }
                //clear event
                _currentEvent = null;
            }
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private DelayedEvent CaptureEvent()
        {
            if (!_Inputs.Any(io => io.IsTouched))
                return null;
            //generate id
            var id = ++_pendingEventID;
            //create event
            var delayed = new DelayedEvent
            {
                EventId = id,
            };
            for (var n = 0; n < _Inputs.Length; n++)
            {
                if (_Inputs[n].IsTouched)
                    delayed.Events.Add(new TupleS<int, object>(n, _Inputs[n].Value));
            }
            return delayed;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleExternalActionRequest(ExternalActionRequestInfo RequestInfo)
        {
            base.HandleExternalActionRequest(RequestInfo);

            //ignored in RT mode, so not checking
            _nonRtInProgress = false;

            //keep event for processing by solve
            _currentEvent = (DelayedEvent)RequestInfo.ActionData;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void InternalResetState()
        {
            base.InternalResetState();
            _currentEvent = null;
            //increase, so any in-flight events will be discared
            _pendingEventID++;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
