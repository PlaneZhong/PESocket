using PENet;
using Protocol;

public class ClientSession : PESession<NetMsg> {
    protected override void OnConnected() {
    }

    protected override void OnReciveMsg(NetMsg msg) {
        PETool.LogMsg("Server Response:" + msg.text);
    }

    protected override void OnDisConnected() {
    }
}