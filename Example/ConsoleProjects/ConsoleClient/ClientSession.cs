using PENet;
using Protocol;

public class ClientSession : PESession<NetMsg> {
    protected override void OnConnected() {
        PETool.LogMsg("Connect Server Succ.");
    }

    protected override void OnReciveMsg(NetMsg msg) {
        PETool.LogMsg("Server Response:" + msg.text);
    }

    protected override void OnDisConnected() {
        PETool.LogMsg("Server Shutdown.");
    }
}