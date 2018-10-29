# PESocket
基于C#语言实现的高效便捷网络库。支持集成到Unity当中使用。

不用过多了解网络通信内部原理，只需几行简单的代码，便能简捷快速开发基于C#语言的服务器和客户端，实现网络通信。

### 技术支持QQ:1785275942

# 使用示意：

### 1.创建Socket服务器
``` c#
PESocket<ClientSession, NetMsg> client = new PESocket<ClientSession, NetMsg>();
client.StartAsServer("127.0.0.1", 17666);
```

### 2.创建Socket客户端
``` c#
PESocket<ClientSession, NetMsg> client = new PESocket<ClientSession, NetMsg>();
client.StartAsClient("127.0.0.1", 17666);
```

### 3.网络消息定义
网络消息需要继承自PEMsg类，并打上[Serializable]标签，便于使用C#语言的序列化功能。消息体支持多层嵌套。
``` c#
[Serializable]
public class NetMsg : PEMsg {
    public int id;
    public string name;
    public int coin;
}
```

### 4.发送网络消息
使用ClientSession/ServerSession类中的SendMsg(T msg)函数以及重载函数SendMsg(byte[] data)可以分别发送打包好的网络消息以及完成序列化二进制网络消息。
``` c#
NetMsg msg = new NetMsg {
    id = 10086,
    name = "Plane",
    coin = 99999
};

this.SendMsg(msg);
```

### 5.接收网络消息
在自定义的ClientSession/ServerSession类中重写OnReciveMsg(T msg)可以接收网络消息。
``` c#
protected override void OnReciveMsg(NetMsg msg) {
    base.OnReciveMsg(msg);

    //TODO 增加处理网络消息的业务逻辑
    PETool.LogMsg("Msg_id:" + msg.id);
    PETool.LogMsg("Msg_name:" + msg.name);
    PETool.LogMsg("Msg_coin:" + msg.coin);
}
```

### 6.第三方日志工具接口
通过SetLog(bool log = true, Action<string, int> logCB = null)接口，可以传入第三方的日志显示工具。（下面以Unity为例，实现在Unity编辑器控制台中输出日志信息）
``` c#
skt.SetLog(true, (string msg, int lv) => {
    switch (lv) {
        case 0:
            msg = "Log:" + msg;
            Debug.Log(msg);
            break;
        case 1:
            msg = "Warn:" + msg;
            Debug.LogWarning(msg);
            break;
        case 2:
            msg = "Error:" + msg;
            Debug.LogError(msg);
            break;
        case 3:
            msg = "Info:" + msg;
            Debug.Log(msg);
            break;
    }
});
```
