/****************************************************
	文件：PESocket.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:20   	
	功能：PESocekt核心类
*****************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace PENet {
    public class PESocket<T, K>
        where T : PESession<K>, new()
        where K : PEMsg {
        private Socket skt = null;
        public T session = null;
        public int backlog = 10;
        List<T> sessionLst = new List<T>();

        public PESocket() {
            skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #region Server
        /// <summary>
        /// Launch Server
        /// </summary>
        public void StartAsServer(string ip, int port) {
            try {
                skt.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                skt.Listen(backlog);
                skt.BeginAccept(new AsyncCallback(ClientConnectCB), skt);
                PETool.LogMsg("\nServer Start Success!\nWaiting for Connecting......", LogLevel.Info);
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
        }

        void ClientConnectCB(IAsyncResult ar) {
            try {
                Socket clientSkt = skt.EndAccept(ar);
                T session = new T();
                sessionLst.Add(session);
                session.StartRcvData(clientSkt, () => {
                    if (sessionLst.Contains(session)) {
                        sessionLst.Remove(session);
                    }
                });
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
            skt.BeginAccept(new AsyncCallback(ClientConnectCB), skt);
        }
        #endregion

        #region Client
        /// <summary>
        /// Launch Client
        /// </summary>
        public void StartAsClient(string ip, int port) {
            try {
                skt.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ServerConnectCB), skt);
                PETool.LogMsg("\nClient Start Success!\nConnecting To Server......", LogLevel.Info);
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
        }

        void ServerConnectCB(IAsyncResult ar) {
            try {
                skt.EndConnect(ar);
                session = new T();
                session.StartRcvData(skt, null);
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
        }
        #endregion

        public void Close() {
            if (skt != null) {
                skt.Close();
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="log">log switch</param>
        /// <param name="logCB">log function</param>
        public void SetLog(bool log = true, Action<string, int> logCB = null) {
            if (log == false) {
                PETool.log = false;
            }
            if (logCB != null) {
                PETool.logCB = logCB;
            }
        }
    }
}