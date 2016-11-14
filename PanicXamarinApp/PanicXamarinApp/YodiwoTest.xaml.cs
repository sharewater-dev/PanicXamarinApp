using PanicXamarinApp.CommonLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Yodiwo;
using Yodiwo.NodeLibrary;

namespace PanicXamarinApp
{
    public partial class YodiwoTest : ContentPage
    {
        public static VirtNodeConfig ActiveCfg;
        Yodiwo.API.Plegma.NodeKey NodeKey { get { return ActiveCfg.NodeKey; } }

        Yodiwo.NodeLibrary.Node node;
        Yodiwo.NodeLibrary.Pairing.CommonDevicePairingPolling pairmodule;

        bool _IsPairing = true;
        public bool IsPairing { get { return _IsPairing; } set { _IsPairing = value; OnPropertyChanged(nameof(IsPairing)); } }

        string _OperationMsg = "Pairing";
        public string OperationMsg { get { return _OperationMsg; } set { _OperationMsg = value; OnPropertyChanged(nameof(OperationMsg)); } }

        bool supressEvents = false;
        public YodiwoTest()
        {
            InitializeComponent();
            Task.Run((Action)InitNodeLibrary);
        }       
        void InitNodeLibrary()
        {
            //load configs
            ActiveCfg = VirtNodeConfig.GetDefaultConfig();
            //add me as entry assembly
            TypeCache.AddEntryAssembly(typeof(YodiwoTest));

            //init nodelibrary confs
            NodeConfig conf = new NodeConfig()
            {
                //uuid = "XamarinSampleNode-" + MathTools.GenerateRandomAlphaNumericString(16),
                uuid = "XamarinSampleNode",
                Name = "Xamarin Sample Node",
                YpServer = ActiveCfg.ApiServer,
                YpchannelPort = ActiveCfg.YpchannelPort,
                SecureYpc = ActiveCfg.YpchannelSecure,
                CertificationServerName = ActiveCfg.CertificationServerName,
                FrontendServer = ActiveCfg.FrontendServer,
                CanSolveGraphs = true,// ActiveCfg.CanSolveGraphs,
                Pairing_CompletionInstructions = "Close the browser tab and switch back to the App",
                Pairing_NoUUIDAuthentication = true, //same machine authentication, no uuid authentiation required

                EnableNodeDiscovery = true,
#if true
                NodeDiscovery_YPCPort_Start = 5000,
                NodeDiscovery_YPCPort_End = 65000,
#else
                NodeDiscovery_YPCPort_Start = 0,
                NodeDiscovery_YPCPort_End = 0,
#endif
            };

            //prepare pairing module
            pairmodule = new Yodiwo.NodeLibrary.Pairing.CommonDevicePairingPolling();
            pairmodule.UriCustomLauncher = uri => Xamarin.Forms.Device.OpenUri(new Uri(uri));

            //prepare node graph manager module
            var nodeGraphManager = new Yodiwo.NodeLibrary.Graphs.NodeGraphManager(
                                                new Type[]
                                                    {
                                                        typeof(Yodiwo.Logic.BlockLibrary.Basic.Librarian),
                                                    });

            //create node
            node = new Yodiwo.NodeLibrary.Node(conf,
                                                pairmodule,
                                                NodeDataLoad, NodeDataSave,
                                                NodeGraphManager: nodeGraphManager
                                                );
            //   things: Helper.GatherThings()

            //register node  cbs
            node.OnTransportConnected += Node_OnTransportConnected; ;
            node.OnTransportDisconnected += Node_OnTransportDisconnected;
            node.OnTransportError += Node_OnTransportError;
            //   node.OnNodePaired += Node_OnNodePaired;
            node.OnThingActivated += Node_OnThingActivated; ;
            node.OnThingDeactivated += Node_OnThingDeactivated; ;

            //register port events
            // RegisterPortEventHandlers();

            //show status form
            SetStatus("Initializing Virtual Node");

            //Check Pairing
            if (NodeKey.IsInvalid)
            {
                SetStatus("Waiting for pairing completion");

                //start pairing
                var pair_res = node.StartPairing(ActiveCfg.FrontendServer, null, ActiveCfg.LocalWebServer).GetResults();

                if (!pair_res)
                {
                    SetStatus("Pairing Failed");
                    OperationMsg = "Pairing Failed";
                    return;
                }
                else
                    SetStatus("Pairing Completed");
            }
            else
            {
                //Setup node info from stored configurations
                //   node.SetupNodeKeys(ActiveCfg.NodeKey, ActiveCfg.NodeSecret.ToSecureString());
            }

            IsPairing = false;

            //Connect
            SetStatus("Connecting  to worker");
            node.Transport = Transport.YPCHANNEL;
            node.Connect();
            //close status form
            SetStatus("Virtual Gateway started");
        }

        void SetStatus(string status)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                txtStatus.Text = status;
            });
        }

        //void RegisterPortEventHandlers()
        //{


        //    node.PortEventHandlers[Helper.TextThing.Ports[0]] = data =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            txtbox1.Text = data
        //        });
        //    };
        //    node.PortEventHandlers[Helper.Light1Thing.Ports[0]] = data =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            try { light1.Opacity = data.ParseToFloat(); } catch { }
        //        });
        //    };
        //    node.PortEventHandlers[Helper.Text2SpeechThing.Ports[0]] = data =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            //VFSharp.Tools.Speech.SpeakText(data);
        //        });
        //    };
        //    node.PortEventHandlers[Helper.CheckBox1Thing.Ports[0]] = data =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            supressEvents = true;
        //            try { chk_CheckBox1.IsToggled = data.ParseToBool(); } catch { } finally { supressEvents = false; }
        //        });
        //    };
        //    node.PortEventHandlers[Helper.Slider1Thing.Ports[0]] = data =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            supressEvents = true;
        //            try { slider1.Value = data.ParseToFloat(); } catch { } finally { supressEvents = false; }
        //        });
        //    };
        //}

        private void Node_OnThingDeactivated(Yodiwo.API.Plegma.Thing thing)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                chk_CheckBox1_Ind.Text = "Deactivated";
            });
        }

        private void Node_OnThingActivated(Yodiwo.API.Plegma.Thing thing)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                chk_CheckBox1_Ind.Text = "Activated";
            });
        }

        //private void Node_OnNodePaired(Yodiwo.API.Plegma.NodeKey nodekey, System.Security.SecureString nodesecret)
        //{

        //    throw new NotImplementedException();
        //}

        private void Node_OnTransportError(Transport Transport, TransportErrors Error, string msgStatus)
        {
            //  DebugEx.TraceLog("OnTransportError transport=" + Transport + " msg=" + msgStatus);
            SetStatus("TransportError transport=" + Transport + " msg=" + msgStatus);
        }

        private void Node_OnTransportDisconnected(Transport Transport, string msgStatus)
        {
            //   DebugEx.TraceLog("OnDisconnected transport=" + Transport + " msg=" + msgStatus);
            SetStatus("Disconnected transport=" + Transport + " msg=" + msgStatus);
        }

        private void Node_OnTransportConnected(Transport Transport, string msgStatus)
        {
            //   DebugEx.TraceLog("OnConnected transport=" + Transport + " msg=" + msgStatus);
            SetStatus("Connected transport=" + Transport + " msg=" + msgStatus);
        }

        public byte[] NodeDataLoad(string Identifier, bool Secure)
        {
            //FileStream 
            //if (File.Exists(Identifier) == false)
            //    return null;
            //else
            //    return File.ReadAllBytes(Identifier);
            return null;
        }

        public bool NodeDataSave(string Identifier, byte[] Data, bool Secure)
        {
            try
            {
                // File.WriteAllBytes(Identifier, Data);
                return true;
            }
            catch (Exception ex)
            {
                //  DebugEx.Assert(ex, "Data save failed");
                return false;
            }
        }

        private void chk_CheckBox1_Changed(object sender, ToggledEventArgs e)
        {
            if (!supressEvents)
                node.SetState(Helper.CheckBox1Thing.Ports[0], e.Value.ToString());
        }
        private void slider1_Changed(object sender, ValueChangedEventArgs e)
        {
            if (!supressEvents)
                node.SetState(Helper.Slider1Thing.Ports[0], e.NewValue.ToString());
        }      
    }
}
