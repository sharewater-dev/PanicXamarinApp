﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
#if NETFX
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
#elif UNIVERSAL
using Windows.Networking.Sockets;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if UNIVERSAL
using Socket = Windows.Networking.Sockets.StreamSocket;
#endif

namespace Yodiwo.YPChannel.Transport.Sockets
{
    public class Client : CommonSocketChannel
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public bool Secure { get; private set; } = false;
        public string CertificateServerName { get; private set; } = null;
#if NETFX
        public IEnumerable<X509Certificate2> CustomCertificates { get; private set; } = null;
#endif
        //------------------------------------------------------------------------------------------------------------------------
#if UNIVERSAL
        public static Action<Client, Socket> SocketSetup;
#endif
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Client(Protocol Protocol, ChannelSerializationMode SupportedChannelSerializationModes = ChannelSerializationMode.Json, ChannelSerializationMode PreferredChannelSerializationModes = ChannelSerializationMode.Json)
            : this(new[] { Protocol }, SupportedChannelSerializationModes: SupportedChannelSerializationModes, PreferredChannelSerializationModes: PreferredChannelSerializationModes)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Client(Protocol[] Protocols, ChannelSerializationMode SupportedChannelSerializationModes = ChannelSerializationMode.Json, ChannelSerializationMode PreferredChannelSerializationModes = ChannelSerializationMode.Json)
            : base(Protocols, ChannelRole.Client, SupportedChannelSerializationModes, PreferredChannelSerializationModes)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public Task<SimpleActionResult> ConnectAsync(string RemoteHost, int RemotePort, bool Secure, string CertificateServerName = null)
        {
            return Task.Run(() => ConnectAsync(RemoteHost, RemotePort, Secure, CertificateServerName: CertificateServerName));
        }
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public SimpleActionResult Connect(string RemoteHost, int RemotePort, bool Secure, string CertificateServerName = null, IEnumerable<X509Certificate2> CustomCertificates = null)
#elif UNIVERSAL
        public SimpleActionResult Connect(string RemoteHost, int RemotePort, bool Secure, string CertificateServerName = null)
#endif
        {
            if (SupportedChannelSerializationModes.HasFlag(ChannelSerializationMode.MessagePack))
                DebugEx.Assert(MsgPack != null, "MessagePack serializer not provided");

            //init results
            var result = new SimpleActionResult()
            {
                IsSuccessful = false,
                Message = "",
            };

            //check state
            if (_State != ChannelStates.Closed && _State != ChannelStates.Initializing)
            {
                result.Message = "Connected or connection already in progress";
                return result;
            }

            //reset state
            if (_State != ChannelStates.Initializing)
                Reset();

            //keep connection parameters
            this.Secure = Secure;
            this.CertificateServerName = CertificateServerName;
#if NETFX
            this.CustomCertificates = CustomCertificates;
#endif

            //connect socket
            var res = _sockConnection(RemoteHost, RemotePort);
            if (!res)
                return res;

            //start heartbeat
            Start();

            //wait for negotiation finish
            while (State == ChannelStates.Initializing || State == ChannelStates.Negotiating)
                Thread.Sleep(100);

            //set message
            if (State != ChannelStates.Open)
                result.Message = "Could not open channel (Negotiation Failed)";

            //return state
            result.IsSuccessful = State == ChannelStates.Open;
            if (result.IsSuccessful && result.Message == "")
                result.Message = "Connection Established (Channel Openned)";

            //hook for system close
            Channel.OnSystemShutDownRequest.Add(Yodiwo.WeakAction<object>.Create(_OnSystemShutDownRequestHandler));

            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------
        protected override void onRedirect(string Target)
        {
            base.onRedirect(Target);

            //destroy previous socket
            try
            {
#if NETFX
                if (_sock != null && _sock.Connected)
                    _sock.Disconnect(true);
#endif
                try { _sock?.Dispose(); } catch { }
            }
            catch { }

            //extract ip/port
            var splits = Target.Trim().Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length != 2)
            {
                DebugEx.TraceError("Could not redirect ypclient to " + Target);
                Close("Could not redirect ypclient to " + Target);
                return;
            }
            var ip = splits[0];
            int port;
            if (ip.Length == 0 || !splits[1].TryParse(out port))
            {
                DebugEx.TraceError("Could parse redirect target of ypclient (" + Target + ")");
                Close("Could parse redirect target of ypclient (" + Target + ")");
                return;
            }
            //connect
            var res = _sockConnection(ip, port);
            if (!res)
            {
                DebugEx.TraceError("Could connect to redirect target of ypclient (" + Target + ", Message:" + res.Message + ")");
                Close("Could connect to redirect target of ypclient (" + Target + ", Message:" + res.Message + ")");
                return;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        SimpleActionResult _sockConnection(string RemoteHost, int RemotePort)
        {
            //declares
            bool isSecured = false;
            string sslProtocol = "";
            var result = new SimpleActionResult()
            {
                IsSuccessful = false,
                Message = "",
            };

            lock (this)
            {
                //create socket
#if NETFX
                _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#elif UNIVERSAL
                _sock = new StreamSocket();
#endif

                //Try to connect
                try
                {
                    //attemp connection
#if NETFX
                    _sock.Connect(RemoteHost, RemotePort);
#elif UNIVERSAL
                    SocketSetup?.Invoke(this, _sock);
                    _sock.Control.KeepAlive = true;
                    _sock.ConnectAsync(new Windows.Networking.HostName(RemoteHost), RemotePort.ToStringInvariant(), SocketProtectionLevel.PlainSocket).AsTask().Wait();
#endif
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex, "Connection Error");
                    result.Message = ex.Message;
                    try { Close("Connection Error"); } catch { }
                    return result;
                }


                //create network stream
#if NETFX
                //Stream _netstream = new BufferedStream(new NetworkStream(_sock, true));
                Stream _netstream = new NetworkStream(_sock, true);
#endif

                //Wrap with a secure stream?
                if (Secure)
                {
#if NETFX
                    //create ssl stream
                    var sslstream = new SslStream(_netstream, false);
#endif
                    //decide on certifacte server name
                    var remCertHostName = !string.IsNullOrWhiteSpace(CertificateServerName) ? CertificateServerName : RemoteHost;

                    try
                    {
#if NETFX            
                        //collect certificates
                        var certs = Yodiwo.Tools.Certificates.CollectCertificates();
                        if (CustomCertificates != null)
                            foreach (var c in CustomCertificates)
                                certs.Add(c);

                        //try authenticate
                        sslstream.AuthenticateAsClient(remCertHostName,
                                                        certs,
                                                        System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12,
                                                        true
                                                        );


                        //checks
                        if (!sslstream.IsAuthenticated)
                        {
                            DebugEx.Assert("Not authenticated");
                            throw new Exception("Not authenticated");
                        }
                        if (!sslstream.IsEncrypted)
                        {
                            DebugEx.Assert("No encryption");
                            throw new Exception("Not encryption");
                        }

                        //get info
                        isSecured = true;
                        sslProtocol = sslstream.SslProtocol.ToStringInvariant();

                        //use this stream from now on
                        _netstream = sslstream;
#elif UNIVERSAL
                        _sock.UpgradeToSslAsync(SocketProtectionLevel.Tls12, new Windows.Networking.HostName(remCertHostName)).AsTask().Wait();
                        var _isSecured = _sock.Information.ProtectionLevel == SocketProtectionLevel.Tls10 ||
                                     _sock.Information.ProtectionLevel == SocketProtectionLevel.Tls11 ||
                                     _sock.Information.ProtectionLevel == SocketProtectionLevel.Tls12;
                        if (!_isSecured)
                            throw new Exception("Connection not secured (" + _sock.Information.ProtectionLevel + ")");
#endif
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceError(ex, "Certificate not accepted, " + ex.Message);
                        result.Message = "Certificate not accepted, " + ex.Message;
                        if (ex.InnerException != null)
                            result.Message += "  (inner msg=" + ex.InnerException.Message + ")";
                        try { Close("Certificate not accepted, " + ex.Message); } catch { }
#if NETFX
                        try { _netstream?.Close(); _netstream?.Dispose(); } catch { }
                        try { sslstream?.Close(); sslstream?.Dispose(); } catch { }
                        try { _sock?.Close(); } catch { }
#endif
                        try { _sock?.Dispose(); } catch { }
                        return result;
                    }
                }


                //write packers
#if NETFX
                var _nodelay = _sock.NoDelay;
                _sock.NoDelay = true; //Disable the Nagle Algorithm
                _netstream.WriteByte((byte)this.SupportedChannelSerializationModes);
                _netstream.WriteByte((byte)this.PreferredChannelSerializationModes);
                _sock.NoDelay = _nodelay; //Restore (default:enable) the Nagle Algorithm
#elif UNIVERSAL
                {
                    var wStream = _sock.OutputStream.AsStreamForWrite();
                    wStream.WriteByte((byte)this.SupportedChannelSerializationModes);
                    wStream.WriteByte((byte)this.PreferredChannelSerializationModes);
                    wStream.Flush();
                }
#endif

                //read final packer
                var packerType = ChannelSerializationMode.Unkown;
#if NETFX
                packerType = (ChannelSerializationMode)_netstream.ReadByte();
#elif UNIVERSAL
                packerType = (ChannelSerializationMode)_sock.InputStream.AsStreamForRead().ReadByte();
#endif
                if (!this.SupportedChannelSerializationModes.HasFlag(packerType))
                {
                    DebugEx.Assert("Invalid ChannelSerializationMode. Server uses  " + packerType);
                    result.Message = "Invalid ChannelSerializationMode. Server uses  " + packerType;
                    try { Close("Invalid ChannelSerializationMode. Server uses  " + packerType); } catch { }
#if NETFX
                    try { _netstream?.Close(); _netstream?.Dispose(); } catch { }
                    try { _sock?.Close(); } catch { }
#endif
                    try { _sock?.Dispose(); } catch { }
                    return result;
                }
                //select serialization mode
                _ChannelSerializationMode = packerType;

                //setup info
                try
                {
#if NETFX
                    this.LocalHost = _sock.LocalEndPoint.GetIPAddress().ToString();
                    this.RemotePort = _sock.LocalEndPoint.GetPort().ToStringInvariant();
#elif UNIVERSAL
                    this.LocalHost = _sock.Information.LocalAddress.ToString();
                    this.RemotePort = _sock.Information.LocalPort;
#endif
                }
                catch { }

                //setup info
                this.RemoteHost = RemoteHost;
                this.RemotePort = RemotePort.ToStringInvariant();

                //log
                DebugEx.TraceLog("YPClient (socks) new connection to " + RemoteHost + ":" + RemotePort + " (Secure=" + isSecured + ",SSL=" + sslProtocol + ")");

                //create stream
#if NETFX
                SetupStream(_netstream);
#elif UNIVERSAL
                SetupStream();
#endif
                result.IsSuccessful = true;
                return result;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void _OnSystemShutDownRequestHandler(object Sender) { Close("System shutdown event"); }
        //------------------------------------------------------------------------------------------------------------------------
#if DEBUG
        ~Client()
        {
            DebugEx.TraceLog("YPC Client destructed (name=" + Name + ")");
        }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
