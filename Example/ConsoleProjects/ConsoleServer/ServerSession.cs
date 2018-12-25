using PENet;
using Protocol;

public class ServerSession : PESession<NetMsg> {
    protected override void OnConnected() {
        PETool.LogMsg("Client OnLine.");
        SendMsg(new NetMsg {
            text = "Welcome to connect!"
        });
    }

    protected override void OnReciveMsg(NetMsg msg) {
        PETool.LogMsg("Client Request:" + msg.text);
        SendMsg(new NetMsg {
            text = msg.text
        });
    }

    protected override void OnDisConnected() {
        PETool.LogMsg("Client OffLine.");
    }
}