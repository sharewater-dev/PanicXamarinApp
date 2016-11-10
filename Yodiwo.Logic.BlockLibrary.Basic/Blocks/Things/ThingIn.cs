using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yodiwo.Logic;
using Yodiwo.API.Plegma;

namespace Yodiwo.Logic.Blocks.Things
{
    public class ThingIn : BaseThings, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        List<ThingUpdateAction> actionData = new List<ThingUpdateAction>();
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO TkeyOut { private set; get; }
        public OutputIO Timestamp { private set; get; }

        public const string IoCategoryShowThingKey = "Show ThingKey";
        public const string IoCategoryShowTimestamp = "Show Timestamp";
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ThingIn(Thing thing)
            : base(thing, 0, thing.Ports
                                   .Count(x => x.ioDirection == ioPortDirection.Output ||
                                               x.ioDirection == ioPortDirection.InputOutput) + 2)
        {
            this.IsThingIn = true;

            //setup Input IO types
            int i = 0;
            var _portUid2IO = new Dictionary<string, IO>();
            var _io2PortUid = new Dictionary<IO, string>();
            foreach (var port in thing.Ports
                                     .Where(x => x.ioDirection == ioPortDirection.Output ||
                                                 x.ioDirection == ioPortDirection.InputOutput))
            {
                var pkey = (PortKey)port.PortKey;

                //setup io
                _Outputs[i].Name = port.Name;
                _Outputs[i].Value = port.GetStateObject();

                //add lookup entries
                _portUid2IO.Add(pkey.PortUID, _Outputs[i]);
                _io2PortUid.Add(_Outputs[i], pkey.PortUID);

                i++;
            }

            PortUid2IO_Lookup = _portUid2IO.AsReadOnly();
            IO2PortUid_Lookup = _io2PortUid.AsReadOnly();

            // Next IO: optional thingkey
            var tkeyCa = new IOCategory() { Name = IoCategoryShowThingKey, MinVisibleIO = 0 };
            IOCategories.Add(tkeyCa);

            //setup output
            TkeyOut = _Outputs[i];
            TkeyOut.Name = "ThingKey";
            TkeyOut.Value = "";
            TkeyOut.Description = "Thingkey of Thing that generated the event";

            //update IO Category
            tkeyCa.Outputs.Add(_Outputs[i]);

            // Next IO: optional timestamp
            var tsCa = new IOCategory() { Name = IoCategoryShowTimestamp, MinVisibleIO = 0 };
            IOCategories.Add(tsCa);

            //setup output
            Timestamp = _Outputs[i + 1];
            Timestamp.Name = "Timestamp";
            Timestamp.Value = "";
            Timestamp.Description = "Timestamp of event *processing* in UTC";

            //update IO Category
            tsCa.Outputs.Add(_Outputs[i + 1]);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnDeploy(bool isFirstDeploy, object User)
        {
            base.OnDeploy(isFirstDeploy, User);

            //apply latest value from ports
            foreach (var p in Thing.Ports)
            {
                if (p.ioDirection == ioPortDirection.Output ||
                    p.ioDirection == ioPortDirection.InputOutput)
                {
                    var pkey = (PortKey)p.PortKey;
                    var outp = PortUid2IO_Lookup[pkey.PortUID];
                    outp.Value = p.GetStateObject();
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            var changed = false;
            var ts = DateTime.UtcNow;

            //handle action
            ThingUpdateAction[] toProcess;
            lock (actionData)
            {
                toProcess = actionData.ToArray();
                actionData.Clear();
            }
            foreach (var action in toProcess)
            {
                changed |= SolveThingUpdateAction(action, this.Thing);
            }
            if (changed)
            {
                //update ThingKey port with thingkey of Thing that triggered this event
                TkeyOut.Value = this.ThingKey;
                Timestamp.Value = ts;
            }
            else
                DebugEx.TraceLog(this.BlockKey + "/" + Thing.ThingKey + ": exit with no change in output(s)");

            //done
            yield return BlockState.Clean;
        }

        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleExternalActionRequest(ExternalActionRequestInfo RequestInfo)
        {
            base.HandleExternalActionRequest(RequestInfo);

            //handle request
            DebugEx.Assert(RequestInfo.ActionData is ThingUpdateAction, "Invalid action data passed to ThingIn");
            var data = RequestInfo.ActionData as ThingUpdateAction;
            if (data != null)
            {
                lock (actionData)
                    actionData.Add(data);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void InternalResetState()
        {
            base.InternalResetState();
            lock (actionData)
                actionData.Clear();
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
