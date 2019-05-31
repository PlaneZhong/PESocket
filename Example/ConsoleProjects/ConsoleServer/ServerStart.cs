using PENet;
using System;
using Protocol;
using System.Collections.Generic;

namespace ConsoleServer {
    class ServerStart {
        static void Main(string[] args) {
            PESocket<ServerSession, NetMsg> server = new PESocket<ServerSession, NetMsg>();
            server.StartAsServer(IPCfg.srvIP, IPCfg.srvPort);

            Console.WriteLine("\nInput 'quit' to stop server!");
            while (true) {
                string ipt = Console.ReadLine();
                if (ipt == "quit") {
                    server.Close();
                    break;
                }
                if (ipt == "all") {
                    List<ServerSession> sessionLst = server.GetSesstionLst();
                    for (int i = 0; i < sessionLst.Count; i++) {
                        sessionLst[i].SendMsg(new NetMsg {
                            text = "broadcast from server."
                        });
                    }
                }
            }
        }
    }
}