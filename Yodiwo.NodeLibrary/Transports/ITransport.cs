﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.NodeLibrary.Transports
{
    public interface ITransport
    {
        bool IsConnected { get; }
        void Disconnect();
        void SendMessage(API.ApiMsg msg);
        Trsp SendRequest<Trsp>(API.ApiMsg request, TimeSpan? timeout = null);
    }
}
