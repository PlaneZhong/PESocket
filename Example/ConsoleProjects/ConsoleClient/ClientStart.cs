using PENet;
using System;
using Protocol;

namespace ConsoleClient {
    class ClientStart {
        static PESocket<ClientSession, NetMsg> client;

        static void Main(string[] args) {
            //TestCode
            //Random rd = new Random();
            //byte[] data = new byte[10];
            //rd.NextBytes(data);
            //byte[] compressBytes = PETool.Compress(data);
            //byte[] deCompress = PETool.DeCompress(compressBytes);

            //for(int i = 0; i < data.Length; i++) {
            //    if(data[i] != deCompress[i]) {
            //        Console.WriteLine("压缩数据出错。");
            //        return;
            //    }
            //}
            //Console.WriteLine("压缩数据成功。");


            client = new PESocket<ClientSession, NetMsg>();
            client.StartAsClient(IPCfg.srvIP, IPCfg.srvPort);

            Console.WriteLine("\nInput 'quit' to stop client!");
            while(true) {
                string ipt = Console.ReadLine();
                if(ipt == "quit") {
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