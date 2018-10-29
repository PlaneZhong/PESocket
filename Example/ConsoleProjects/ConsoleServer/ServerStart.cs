using PENet;
using System;
using Protocol;

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
            }
        }
    }
}