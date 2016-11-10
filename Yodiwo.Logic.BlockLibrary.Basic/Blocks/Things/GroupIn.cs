using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yodiwo.API.Plegma;

namespace Yodiwo.Logic.Blocks.Things
{
    public class GroupIn : BaseGroups, IHandleExternalActionRequest
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------ 
        List<ThingUpdateAction> actionData = new List<ThingUpdateAction>();
        //------------------------------------------------------------------------------------------------------------------------
        OutputIO TkeyOut;
        OutputIO Timestamp;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public GroupIn(ThingGroupDescriptor groupDescriptor)
            : base(groupDescriptor,
                   0,
                   groupDescriptor.Things
                                  .First()
                                  .Ports
                                  .Where(x => x.ioDirection == ioPortDirection.Output ||
                                              x.ioDirection == ioPortDirection.InputOutput)
                                  .Count() + 2)
        {
            this.IsGroupIn = true;

            var _portUid2IO = new Dictionary<TupleS<string, ThingKey>, IO>();
            var _io2PortUid = new Dictionary<TupleS<IO, ThingKey>, string>();

            //setup IO types
            //NOTE: just initializing based on first thing, since they're guarranteed to be of same type
            int i = 0;
            var firstThingPorts = GroupDescriptor.Things.First().Ports;

            for (int n = 0; n < firstThingPorts.Count; n++)
                if (firstThingPorts[n].ioDirection == ioPortDirection.Output ||
                    firstThingPorts[n].ioDirection == ioPortDirection.InputOutput)
                {
                    var firstThingPort = firstThingPorts[n];

                    //setup io
                    _Outputs[i].Name = firstThingPort.Name;
                    _Outputs[i].Value = "";
                    _Outputs[i].Description = firstThingPort.Description;

                    //add lookup entries
                    foreach (var thing in GroupDescriptor.Things)
                    {
                        var pkey = (PortKey)thing.Ports[n].PortKey;
                        _portUid2IO.Add(TupleS.Create(pkey.PortUID, pkey.ThingKey), _Outputs[i]);
                        _io2PortUid.Add(TupleS.Create((IO)_Outputs[i], pkey.ThingKey), pkey.PortUID);
                    }

                    i++;
                }
            PortUid2IO_Group_Lookup = _portUid2IO.AsReadOnly();
            IO2PortUid_Group_Lookup = _io2PortUid.AsReadOnly();

            //add ThingKey port
            TkeyOut = _Outputs[i];
            TkeyOut.Name = "Thingkey";
            TkeyOut.Value = "";
            TkeyOut.Description = "Thingkey of the Group's Thing that generated the event";

            // Next IO: optional timestamp
            var tsCa = new IOCategory() { Name = "Show timestamp", MinVisibleIO = 0 };
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
        public override void Validate(List<BuildValidationResult> ResultOutput)
        {
            //base Validation
            base.Validate(ResultOutput);

            foreach (var t in GroupDescriptor.Things)
            {
                if (t.BlockType != null)
                {
                    var specialType = Helper.NonGenericTypesToClass.TryGetOrDefault(t.BlockType);
                    if (specialType != null && specialType.Item1 != null)
                    {
                        ResultOutput.Add(new BuildValidationResult(this, BuildValidationResult.ResultType.Error, "groups of non default thing models not supported yet"));
                        break;
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            var changed = false;

            //handle action
            ThingUpdateAction[] toProcess;
            lock (actionData)
            {
                toProcess = actionData.ToArray();
                actionData.Clear();
            }
            foreach (var action in toProcess)
            {
                var evThing = GroupDescriptor.Things.Single(yt => yt.ThingKey == action.PortKey.ThingKey); //will throw up if nothing found

                changed |= SolveThingUpdateAction(action, evThing);

                if (changed)
                {
                    //update ThingKey port with key of Thing that triggered this event
                    TkeyOut.Value = evThing.ThingKey;
                    //update timestamp port
                    Timestamp.Value = DateTime.UtcNow;
                }
            }
            if (changed)
                DebugEx.TraceLog(this.BlockKey + "/" + ": exit with no change in output(s)");
            //done
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void HandleExternalActionRequest(ExternalActionRequestInfo RequestInfo)
        {
            base.HandleExternalActionRequest(RequestInfo);

            //handle request
            DebugEx.Assert(RequestInfo.ActionData is ThingUpdateAction, "Invalid action data passed to GroupIn");
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
