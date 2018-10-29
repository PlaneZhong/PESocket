using PENet;
using System;
using Protocol;

namespace ConsoleClient {
    class ClientStart {
        static PESocket<ClientSession, NetMsg> client;

        static void Main(string[] args) {
            client = new PESocket<ClientSession, NetMsg>();
            client.StartAsClient(IPCfg.srvIP, IPCfg.srvPort);

            Console.WriteLine("\nInput 'quit' to stop client!");
            while (true) {
                string ipt = Console.ReadLine();
                if (ipt == "quit") {
                    client.Close();
                    break;
                }
                else {
                    client.session.SendMsg(new NetMsg {
                        text = ipt
                    });
                }
            }
        }
    }
}