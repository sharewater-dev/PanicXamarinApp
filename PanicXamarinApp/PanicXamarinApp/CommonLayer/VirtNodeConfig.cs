using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.CommonLayer
{
    public class VirtNodeConfig
    {
        public string NodeKey;
        public string NodeSecret;
        public string FrontendServer;
        public string ApiServer;
        public int YpchannelPort;
        public bool YpchannelSecure;
        public string CertificationServerName;
        public string LocalWebServer;
        public bool CanSolveGraphs;


        public static VirtNodeConfig GetDefaultConfig()
        {
            //return GetLocalConfig();
            //return GetLANConfig();
            return GetTCyanConfig();
            //return GetCyanConfig();
        }

        public static VirtNodeConfig GetLocalConfig()
        {
            //create default conf
            VirtNodeConfig cfg = new VirtNodeConfig();
            cfg.FrontendServer = "http://localhost:3334";
            cfg.ApiServer = "localhost";
            cfg.YpchannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.YpchannelSecure = false;
            return cfg;
        }

        public static VirtNodeConfig GetLANConfig()
        {
            //create default conf
            VirtNodeConfig cfg = new VirtNodeConfig();
            cfg.FrontendServer = "http://10.30.254.33:3334";
            cfg.ApiServer = "10.30.254.33";
            cfg.YpchannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.YpchannelSecure = false;
            return cfg;
        }

        public static VirtNodeConfig GetTCyanConfig()
        {
            //create default conf
            VirtNodeConfig cfg = new VirtNodeConfig();
            cfg.FrontendServer = "https://tcyan.yodiwo.com";
            cfg.ApiServer = "tcyan.yodiwo.com";
            cfg.YpchannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.YpchannelSecure = true;
            return cfg;
        }

        public static VirtNodeConfig GetCyanConfig()
        {
            //create default conf
            VirtNodeConfig cfg = new VirtNodeConfig();
            cfg.FrontendServer = "https://cyan.yodiwo.com";
            cfg.ApiServer = "cyan.yodiwo.com";
            cfg.YpchannelPort = Yodiwo.API.Plegma.Constants.YPChannelPort;
            cfg.YpchannelSecure = true;
            return cfg;
        }

        public void Save()
        {
        }
    }
}
