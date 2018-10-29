using System;
using PENet;

namespace Protocol {
    [Serializable]
    public class NetMsg : PEMsg {
        public string text;
    }


    public class IPCfg {
        public const string srvIP = "192.168.110";
        public const int srvPort = 17666;
    }
}