﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
#elif UNIVERSAL
using Windows.Networking.Sockets;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yodiwo;

#if UNIVERSAL
using Socket = Windows.Networking.Sockets.StreamSocket;
#endif

namespace Yodiwo.YPChannel.Transport.Sockets
{
    public class Server
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        Socket sock;
#elif UNIVERSAL
        Windows.Networking.Sockets.StreamSocketListener sock;
#endif
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        Thread PortListener;
#elif UNIVERSAL
        Task PortListener;
#endif
        //------------------------------------------------------------------------------------------------------------------------
        HashSetTS<ServerChannel> _Channels = new HashSetTS<ServerChannel>();
        public IReadOnlySet<ServerChannel> Channels => _Channels;
        //------------------------------------------------------------------------------------------------------------------------
        public ISerializer MsgPackSerializer;
        //------------------------------------------------------------------------------------------------------------------------
        public Func<Protocol[], Socket, ServerChannel> ChannelConstructor = null;
        int Port;
        ChannelSerializationMode SupportedChannelSerializationModes;
        ChannelSerializationMode PreferredChannelSerializationModes;
        //------------------------------------------------------------------------------------------------------------------------
        DictionaryTS<string, ServerChannel> IssuedKeys = new DictionaryTS<string, ServerChannel>();
        //------------------------------------------------------------------------------------------------------------------------
        public delegate bool OnNewSocketConnectionFilterDelegate(Server Server, string RemoteEndpointIP);
        public OnNewSocketConnectionFilterDelegate OnNewSocketConnectionFilter = null;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnNewChannelHandler(Server Server, Channel Channel);
        public event OnNewChannelHandler OnNewChannel = null;
        //------------------------------------------------------------------------------------------------------------------------
        Protocol[] Protocols;
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        X509Certificate2 _certificate = null;
        public X509Certificate2 Certificate { get { return _certificate; } }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        class ReconnectionBookkeepEntry
        {
            public DateTime ConnectionTimestamp;
            public int Connections;
        }
        DictionaryTS<string, ReconnectionBookkeepEntry> reconnectThrottleBookKeeper = new DictionaryTS<string, ReconnectionBookkeepEntry>();
        public TimeSpan ReconnectionThrottleTimeout = TimeSpan.FromSeconds(2);
        public int ReconnectionThrottleAfterConnectionCount = 10; //after xx connections reconnection throttle will kick-in
        public bool IsReconnectionThrottleEnabled = true;
        //------------------------------------------------------------------------------------------------------------------------
        bool _IsRunning = false;
        public bool IsRunning { get { return _IsRunning; } }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Server(Protocol Protocol, ChannelSerializationMode SupportedChannelSerializationModes = ChannelSerializationMode.Json, ChannelSerializationMode PreferredChannelSerializationModes = ChannelSerializationMode.Json)
            : this(new[] { Protocol }, SupportedChannelSerializationModes: SupportedChannelSerializationModes, PreferredChannelSerializationModes: PreferredChannelSerializationModes)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Server(Protocol[] Protocols, ChannelSerializationMode SupportedChannelSerializationModes = ChannelSerializationMode.Json, ChannelSerializationMode PreferredChannelSerializationModes = ChannelSerializationMode.Json)
        {
            this.Protocols = Protocols == null ? (Protocol[])null : (Protocol[])Protocols.Clone(); //copy here to avoid user changing stuff later on
            this.SupportedChannelSerializationModes = SupportedChannelSerializationModes;
            this.PreferredChannelSerializationModes = PreferredChannelSerializationModes;

            //hook event for shutdown
            Channel.OnSystemShutDownRequest.Add(Yodiwo.WeakAction<object>.Create(ShutdownRedirect));
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        void ShutdownRedirect(object Sender) { Stop(CloseAllChannels: true); }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public bool Start(int Port, X509Certificate2 certificate = null)
#elif UNIVERSAL
        public bool Start(int Port)
#endif
        {
            lock (this)
            {
                //keep
                this.Port = Port;

                //get certificate
#if NETFX
                this._certificate = certificate;
                if (Certificate != null && !Certificate.HasPrivateKey)
                {
                    DebugEx.Assert("Not private key found in servers certificate");
                    return false;
                }
#endif

                //create socket and bind
                try
                {
#if NETFX
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Bind(new IPEndPoint(IPAddress.Any, Port));
#elif UNIVERSAL
                    sock = new StreamSocketListener();
                    sock.ConnectionReceived += Sock_ConnectionReceived;
                    sock.BindServiceNameAsync(Port.ToStringInvariant()).AsTask().Wait();
#endif
                }
                catch (Exception ex)
                {
                    DebugEx.Assert(ex, "YPServer failed to bind port " + Port);
                    return false;
                }

                //mark running
                _IsRunning = true;

                //log
#if NETFX
                DebugEx.TraceLog("YPServer (socks) started on port " + Port + "  (Secure=" + (certificate != null).ToStringInvariant() + ")");
#elif UNIVERSAL
                DebugEx.TraceLog("YPServer (socks) started on port " + Port + "  (Secure= False)");
#endif
                //start port listener
#if NETFX
                PortListener = new Thread(PortListenerEntryPoint);
                PortListener.Name = "YPC Server PortListener thread";
                PortListener.IsBackground = true;
                PortListener.Start();
#endif
                //done
                return true;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Stop(bool CloseAllChannels = false)
        {
            try
            {
                lock (this)
                {
                    //close all channels
                    if (CloseAllChannels)
                    {
                        _Channels.ForEach(c => { try { c.Close("Server stopped"); } catch (Exception ex) { DebugEx.Assert(ex, "Error while closing channel"); } });
                        _Channels.Clear();
                    }

                    //update flag
                    if (!_IsRunning)
                        return;
                    else
                        _IsRunning = false;

                    //close my socket
                    try
                    {
                        if (sock != null)
                        {
#if NETFX
                            try { sock.Close(); } catch { }
#endif
                            try { sock.Dispose(); } catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceErrorException(ex);
                    }
                    sock = null;

                    //wait for task finish
#if NETFX
                    PortListener?.Join(1000);
#elif UNIVERSAL
                    PortListener?.Wait(1000);
#endif
                    PortListener = null;
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "YPChannel Server Stop() failed");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        void PortListenerEntryPoint()
        {
            try
            {
                //start listening
                sock?.Listen(10);

                //heartbeat
                while (_IsRunning)
                {
                    //listen
                    Socket newsocket = null;

                    //we need to stop?
                    var __sock = sock;  //keep a local copy
                    if (__sock == null)
                        break;

                    //accept and get new socket
                    try { newsocket = __sock.Accept(); }
                    catch (Exception ex) { DebugEx.TraceError(ex, "YPServer sock.Accept() exception"); continue; }

                    //handlew new connection
                    HandleNewConnection(newsocket);
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "YPChannel server heartbeat error");
            }

            //inform for heartbeat end
            DebugEx.TraceLog("YPChannel server heartbeat finished");
        }
#elif UNIVERSAL
        private void Sock_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            HandleNewConnection(args.Socket);
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        void HandleNewConnection(Socket newsocket)
        {
            try
            {
                //check reconnection throttle
                try
                {
#if NETFX
                    var re = newsocket.RemoteEndPoint.GetIPAddress().ToStringInvariant();
#elif UNIVERSAL
                    var re = newsocket.Information.RemoteAddress.ToStringInvariant();
#endif
                    //filtering
                    if (OnNewSocketConnectionFilter != null && OnNewSocketConnectionFilter(this, re) == false)
                    {
#if NETFX
                        try { newsocket.Close(); } catch { }
#endif
                        try { newsocket.Dispose(); } catch { }
                        DebugEx.TraceWarning("Connection from " + re + " closed from filter");
                        return;
                    }

                    if (IsReconnectionThrottleEnabled && re != "127.0.0.1") //no reconnection throttle for localhost connections
                    {
                        var rbe = reconnectThrottleBookKeeper.TryGetOrDefault(re);
                        if (rbe == null)
                        {
                            rbe = new ReconnectionBookkeepEntry()
                            {
                                ConnectionTimestamp = DateTime.Now + TimeSpan.FromMilliseconds(100),
                                Connections = 1,
                            };
                            reconnectThrottleBookKeeper.ForceAdd(re, rbe);
                        }
                        else
                        {
                            rbe.Connections++;
                            if (rbe.Connections > ReconnectionThrottleAfterConnectionCount)
                            {
                                var lastTimestamp = rbe.ConnectionTimestamp;
                                var elapsed = DateTime.Now - lastTimestamp;
                                if (elapsed < ReconnectionThrottleTimeout)
                                {
#if NETFX
                                    try { newsocket.Close(); } catch { }
#endif
                                    try { newsocket.Dispose(); } catch { }
                                    DebugEx.TraceWarning("Connection from " + re + " closed due to reconnection throttle (" + elapsed.Seconds + " sec)");
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { DebugEx.TraceWarning(ex, "YPServer Reconnection throttle exception"); }

                //cleanup old entries
                reconnectThrottleBookKeeper.RemoveWhere(e => DateTime.Now - e.Value.ConnectionTimestamp > ReconnectionThrottleTimeout);

                //start task new connection
                Task.Run(() =>
                    {
                        try
                        {
                            //check
#if NETFX
                            if (!newsocket.Connected)
                            {
                                DebugEx.TraceWarning("YPServer newsocket not connected?");
                                try { newsocket.Close(); } catch { }
                                return;
                            }
#endif
                            //create channel
                            var con = ChannelConstructor;
                            var channel = con == null ? new ServerChannel(this, Protocols, SupportedChannelSerializationModes, PreferredChannelSerializationModes, newsocket) : con(Protocols, newsocket);
                            if (channel == null)
                            {
                                DebugEx.Assert("Could not create channel");
#if NETFX
                                try { newsocket.Close(); } catch { }
#endif
                                try { newsocket.Dispose(); } catch { }
                                return;
                            }

                            //add to set
                            string channelKey = null;
                            lock (_Channels)
                            {
                                //generate unique key
                                while (IssuedKeys.ContainsKey(channelKey = MathTools.GenerateRandomAlphaNumericString(64))) ;
                                //set on channel
                                channel._ChannelKey = channelKey;
                                //add to lookups
                                _Channels.Add(channel);
                                IssuedKeys.Add(channelKey, channel);
                            }

                            //start task timeout monitor
                            bool setupFinished = false;
                            Task.Run(() =>
                            {
                                try
                                {
                                    //wait
                                    Thread.Sleep(30000);
                                    //check
                                    if (!setupFinished)
                                    {
                                        DebugEx.TraceLog("ServerChannel setup timeout");
                                        try { channel.Close("ServerChannel setup timeout"); } catch { }
#if NETFX
                                        try { newsocket?.Close(); } catch { }
#endif
                                        try { newsocket?.Dispose(); } catch { }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DebugEx.Assert(ex, "Unhandled exception");
#if NETFX
                                    try { newsocket?.Close(); } catch { }
#endif
                                    try { newsocket?.Dispose(); } catch { }
                                }
                            });

                            //set serializer
                            channel.MsgPack = MsgPackSerializer;

                            //setup channel socket
                            if (channel.SetupServerSocket() == false)
                            {
#if NETFX
                                try { newsocket?.Close(); } catch { }
#endif
                                try { newsocket?.Dispose(); } catch { }
                                //add to lookups
                                _Channels.Remove(channel);
                                IssuedKeys.Remove(channelKey);
                                return;
                            }

                            //mark setup finish
                            setupFinished = true;

                            //call event
                            OnNewChannel?.Invoke(this, channel);

                            //start heartbeat
                            channel.Start();
                        }
                        catch (Exception ex)
                        {
                            DebugEx.Assert(ex, "YPServer: Failed setting up new connection");
#if NETFX
                            try { newsocket.Close(); } catch { }
#endif
                            try { newsocket.Dispose(); } catch { }
                            return;
                        }
                    });
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "YPChannel server setup new connection error");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~Server()
        {
            try
            {
                //stop server
                Stop(CloseAllChannels: true);
                //disconnect/close/dispose sockets
                if (sock != null)
                {
#if NETFX
                    try { sock.Close(); } catch { }
#endif
                    try { sock.Dispose(); } catch { }
                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "YPChannel Server destructor caught unhandled exception");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        internal void onChannelClose(ServerChannel channel)
        {
            try
            {
                //remove from channel set
                _Channels.Remove(channel);
                IssuedKeys.Remove(channel.ChannelKey);
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Server onChannelClose() unhandled exception"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}