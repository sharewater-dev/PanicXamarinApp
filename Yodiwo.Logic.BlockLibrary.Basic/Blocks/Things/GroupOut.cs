using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Yodiwo;
using Yodiwo.API.Plegma;

namespace Yodiwo.Logic.Blocks.Things
{
    public class GroupOut : BaseGroups
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        InputIO TKeyIn;

        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
             FriendlyName = "Things To Trigger",
             Description = @"Thingkeys of Things within group to trigger. Can be comma or semicolon separated, or as a JSON-array. 
                             If the block's ThingKey port is connected, this value is ignored",
             InspectorCategory = UIInspectorCategory.Hardware_Configuration,
             IsReadonly = true)]
        public string ThingKeys = "";
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public GroupOut(ThingGroupDescriptor groupDescriptor)
            : base(groupDescriptor,
                   groupDescriptor.Things
                                  .First()
                                  .Ports
                                  .Where(x => x.ioDirection == ioPortDirection.Input ||
                                              x.ioDirection == ioPortDirection.InputOutput)
                                  .Count() + 1, //plus 1 for the optional thingKey port
                   0)
        {
            this.IsGroupOut = true;

            var _portUid2IO = new Dictionary<TupleS<string, ThingKey>, IO>();
            var _io2PortUid = new Dictionary<TupleS<IO, ThingKey>, string>();

            //setup Input IO types
            int i = 0;
            var firstThingPorts = GroupDescriptor.Things.First().Ports;

            for (int n = 0; n < firstThingPorts.Count; n++)
            {
                if (firstThingPorts[n].ioDirection == ioPortDirection.Input ||
                    firstThingPorts[n].ioDirection == ioPortDirection.InputOutput)
                {
                    var firstThingPort = firstThingPorts[n];

                    //setup io
                    //identify type of input (int, boolean, string, color triplets, etc) based on channel.type string and assign here
                    _Inputs[i].IOType = PortConfiguration.PortTypeDict.ContainsKey(firstThingPort.Type) ?
                                        PortConfiguration.PortTypeDict[firstThingPort.Type] : typeof(object);
                    _Inputs[i].Name = firstThingPort.Name;
                    _Inputs[i].Value = "";
                    _Inputs[i].Description = firstThingPort.Description;

                    //add lookup entries
                    foreach (var thing in GroupDescriptor.Things)
                    {
                        var pkey = (PortKey)thing.Ports[n].PortKey;
                        _portUid2IO.Add(TupleS.Create(pkey.PortUID, pkey.ThingKey), _Inputs[i]);
                        _io2PortUid.Add(TupleS.Create((IO)_Inputs[i], pkey.ThingKey), pkey.PortUID);
                    }

                    i++;
                }
            }

            PortUid2IO_Group_Lookup = _portUid2IO.AsReadOnly();
            IO2PortUid_Group_Lookup = _io2PortUid.AsReadOnly();

            // The last input port will be in one category 
            var inCa = new IOCategory() { Name = "ThingKey", MinVisibleIO = 0 };
            IOCategories.Add(inCa);

            //setup thingkey output
            TKeyIn = _Inputs[i];
            TKeyIn.IOType = typeof(string);
            TKeyIn.Name = "ThingKey";
            TKeyIn.Value = "";
            TKeyIn.Description = "Single or collection of Thingkeys within group to trigger. If multiple, separate with ',', ';' or as a JSON array";

            //update IO Category
            inCa.Inputs.Add(_Inputs[i]);
        }

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
            var dirtyPorts = new List<PortUpdateMessage>();

            //stamp
            var timestamp = DateTime.UtcNow;

            //find if this event refers to a single thing, multiple things, or all things within group
            var thingsToEvaluate = GroupDescriptor.Things;

            try
            {
                string tkeyValue = null;
                if (TKeyIn.IsConnected && !string.IsNullOrEmpty((string)TKeyIn.Value))
                    tkeyValue = (string)TKeyIn.Value;
                else if (!string.IsNullOrWhiteSpace(ThingKeys))
                    tkeyValue = ThingKeys;

                if (!string.IsNullOrWhiteSpace(tkeyValue))
                {
                    if (tkeyValue.First() == '[' && tkeyValue.Last() == ']')
                    {
                        //try to parse input as array, find valid thingKeys in it and save them
                        var thingKeysToEvaluate = tkeyValue.FromJSON<IEnumerable<string>>().Where(val => ((ThingKey)val).IsValid).ToHashSet();
                        //use identified keys to filter this group's things
                        thingsToEvaluate = GroupDescriptor.Things.Where(t => thingKeysToEvaluate.Contains(t.ThingKey)).ToArray();
                    }
                    else if (tkeyValue.Contains(',') || tkeyValue.Contains(';'))
                    {
                        //try to parse input as array, find valid thingKeys in it and save them
                        var thingKeysToEvaluate = tkeyValue.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        //use identified keys to filter this group's things
                        thingsToEvaluate = GroupDescriptor.Things.Where(t => thingKeysToEvaluate.Contains(t.ThingKey)).ToArray();
                    }
                    else
                    {
                        //try for single thing
                        ThingKey tkeySingle = tkeyValue;
                        if (tkeySingle.IsValid)
                        {
                            var thing = GroupDescriptor.Things.SingleOrDefault(t => t.ThingKey == tkeySingle);
                            if (thing != null)
                                thingsToEvaluate = new[] { thing };
                        }
                    }
                }
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex, reportIt: false); }

            //process things
            foreach (var thing in thingsToEvaluate)
            {
                foreach (var port in thing.Ports
                                          .Where(x => x.ioDirection == ioPortDirection.Input ||
                                                      x.ioDirection == ioPortDirection.InputOutput))
                {
                    var io = PortUid2IO_Group_Lookup.TryGetOrDefaultReadOnly(TupleS.Create(((PortKey)port.PortKey).PortUID, (ThingKey)thing.ThingKey)) as InputIO;
                    if (io != null && io.IsConnected)
                    {
                        lock (port)
                        {
                            //check input dirtiness
                            if (io.IsDirty ||
                               (io.IsTouched && !port.ConfFlags.HasFlag(ePortConf.SupressIdenticalEvents)))
                            {
                                dirtyPorts.Add(SolvePort(timestamp, port, io, thing));
                            }
                        }
                    }
                }
            }
            if (dirtyPorts.Count > 0)
                //trigger events for all things of group
                YEventRouter.EventRouter.TriggerEvent(this, new EvThingSolved() { Residency = Residency, PortsUpdated = dirtyPorts, MultiNode = true, Timestamp = timestamp, isWarmupSolve = Graph.IsWarmupSolve });

            //clean block
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
