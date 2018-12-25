/****************************************************
	文件：PESession.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:20   	
	功能：网络会话管理
*****************************************************/

using System;
using System.Net.Sockets;

namespace PENet {
    public abstract class PESession<T> where T : PEMsg {
        private Socket skt;
        private Action closeCB;

        #region Recevie
        public void StartRcvData(Socket skt, Action closeCB) {
            try {
                this.skt = skt;
                this.closeCB = closeCB;

                OnConnected();

                PEPkg pack = new PEPkg();
                skt.BeginReceive(
                    pack.headBuff,
                    0,
                    pack.headLen,
                    SocketFlags.None,
                    new AsyncCallback(RcvHeadData),
                    pack);
            }
            catch (Exception e) {
                PETool.LogMsg("StartRcvData:" + e.Message, LogLevel.Error);
            }
        }

        private void RcvHeadData(IAsyncResult ar) {
            try {
                PEPkg pack = (PEPkg)ar.AsyncState;
                if (skt.Available == 0) {
                    OnDisConnected();
                    Clear();
                    return;
                }
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.headIndex += len;
                    if (pack.headIndex < pack.headLen) {
                        skt.BeginReceive(
                            pack.headBuff,
                            pack.headIndex,
                            pack.headLen - pack.headIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvHeadData),
                            pack);
                    }
                    else {
                        pack.InitBodyBuff();
                        skt.BeginReceive(pack.bodyBuff,
                            0,
                            pack.bodyLen,
                            SocketFlags.None,
                            new AsyncCallback(RcvBodyData),
                            pack);
                    }
                }
                else {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e) {
                PETool.LogMsg("RcvHeadError:" + e.Message, LogLevel.Error);
            }
        }

        private void RcvBodyData(IAsyncResult ar) {
            try {
                PEPkg pack = (PEPkg)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.bodyIndex += len;
                    if (pack.bodyIndex < pack.bodyLen) {
                        skt.BeginReceive(pack.bodyBuff,
                            pack.bodyIndex,
                            pack.bodyLen - pack.bodyIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvBodyData),
                            pack);
                    }
                    else {
                        T msg = PETool.DeSerialize<T>(pack.bodyBuff);
                        OnReciveMsg(msg);

                        //loop recive
                        pack.ResetData();
                        skt.BeginReceive(
                            pack.headBuff,
                            0,
                            pack.headLen,
                            SocketFlags.None,
                            new AsyncCallback(RcvHeadData),
                            pack);
                    }
                }
                else {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e) {
                PETool.LogMsg("RcvBodyError:" + e.Message, LogLevel.Error);
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// Send message data
        /// </summary>
        public void SendMsg(T msg) {
            byte[] data = PETool.PackLenInfo(PETool.Serialize<T>(msg));
            SendMsg(data);
        }

        /// <summary>
        /// Send binary data
        /// </summary>
        public void SendMsg(byte[] data) {
            NetworkStream ns = null;
            try {
                ns = new NetworkStream(skt);
                if (ns.CanWrite) {
                    ns.BeginWrite(
                        data,
                        0,
                        data.Length,
                        new AsyncCallback(SendCB),
                        ns);
                }
            }
            catch (Exception e) {
                PETool.LogMsg("SndMsgError:" + e.Message, LogLevel.Error);
            }
        }

        private void SendCB(IAsyncResult ar) {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            try {
                ns.EndWrite(ar);
                ns.Flush();
                ns.Close();
            }
            catch (Exception e) {
                PETool.LogMsg("SndMsgError:" + e.Message, LogLevel.Error);
            }
        }
        #endregion

        /// <summary>
        /// Release Resource
        /// </summary>
        private void Clear() {
            if (closeCB != null) {
                closeCB();
            }
            skt.Close();
        }

        /// <summary>
        /// Connect network
        /// </summary>
        protected virtual void OnConnected() {
            PETool.LogMsg("New Seesion Connected.", LogLevel.Info);
        }

        /// <summary>
        /// Receive network message
        /// </summary>
        protected virtual void OnReciveMsg(T msg) {
            PETool.LogMsg("Receive Network Message.", LogLevel.Info);
        }

        /// <summary>
        /// Disconnect network
        /// </summary>
        protected virtual void OnDisConnected() {
            PETool.LogMsg("Session Disconnected.", LogLevel.Info);
        }
    }
}