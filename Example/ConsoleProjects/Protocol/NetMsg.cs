using System;
using PENet;

namespace Protocol {
    [Serializable]
    public class NetMsg : PEMsg {
        public string text;
    }


    public class IPCfg {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }
}