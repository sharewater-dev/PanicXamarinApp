using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yodiwo;
using Yodiwo.Logic.SubSystems;
using Yodiwo.API.Plegma;
using Yodiwo.Logic.Blocks.Things;
using System.Threading;

namespace Yodiwo.Logic.Blocks.Things
{
    public class ThingOut : BaseThings
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Trigger even on warmup",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "If enabled, an event will be sent to the device even during its initialization solving (which does not correspond to 'real' events)")]
        public bool TriggerOnWarmup = false;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public ThingOut(Thing thing)
            : base(thing,
                   thing.Ports
                        .Count(x => x.ioDirection == ioPortDirection.Input ||
                                    x.ioDirection == ioPortDirection.InputOutput),
                    0)
        {
            this.IsThingOut = true;

            //setup Input IO types
            int i = 0;
            var _portUid2IO = new Dictionary<string, IO>();
            var _io2PortUid = new Dictionary<IO, string>();
            foreach (var port in thing.Ports
                                     .Where(x => x.ioDirection == ioPortDirection.Input ||
                                                 x.ioDirection == ioPortDirection.InputOutput))
            {
                var pkey = (PortKey)port.PortKey;

                //identify type of input (int, boolean, string, color triplets, etc) based on channel.type string and assign here
                _Inputs[i].IOType = PortConfiguration.PortTypeDict.ContainsKey(port.Type) ?
                                       PortConfiguration.PortTypeDict[port.Type] : typeof(object);
                _Inputs[i].Name = port.Name;
                _Inputs[i].Description = port.Description;

                //add lookup entries
                _portUid2IO.Add(pkey.PortUID, _Inputs[i]);
                _io2PortUid.Add(_Inputs[i], pkey.PortUID);

                i++;
            }
            PortUid2IO_Lookup = _portUid2IO.AsReadOnly();
            IO2PortUid_Lookup = _io2PortUid.AsReadOnly();
        }

        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            var dirtyPorts = new List<PortUpdateMessage>();

            //stamp
            var timestamp = DateTime.UtcNow;

            try
            {
                //check input dirtiness
                int i = 0;
                foreach (var port in Thing.Ports
                                         .Where(x => x.ioDirection == ioPortDirection.Input ||
                                                     x.ioDirection == ioPortDirection.InputOutput))
                {
                    try
                    {
                        var io = PortUid2IO_Lookup.TryGetOrDefaultReadOnly(((PortKey)port.PortKey).PortUID);
                        if (io != null && io.IsConnected)
                        {
                            lock (port)
                            {
                                if (io.IsDirty ||
                                   (io.IsTouched && !port.ConfFlags.HasFlag(ePortConf.SupressIdenticalEvents)))
                                {
                                    dirtyPorts.Add(SolvePort(timestamp, port, io, Thing));
                                }
                            }
                        }
                    }
                    catch (Exception ex) { DebugEx.TraceErrorException(ex); }

                    i++;
                }

                //trigger event
                if (dirtyPorts.Count > 0)
                    YEventRouter.EventRouter.TriggerEvent(
                        this,
                        new EvThingSolved()
                        {
                            Residency = Residency,
                            PortsUpdated = dirtyPorts,
                            Timestamp = timestamp,
                            isWarmupSolve = Graph.IsWarmupSolve && !TriggerOnWarmup
                        }
                    );
            }
            catch (Exception ex) { DebugEx.TraceErrorException(ex); }

            //clean block
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
