/****************************************************
	文件：PETool.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:21   	
	功能：工具类
*****************************************************/

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PENet {
    public class PETool {

        public static byte[] PackNetMsg<T>(T msg) where T : PEMsg {
            return PackLenInfo(Serialize(msg));
        }

        /// <summary>
        /// Add length info to package
        /// </summary>
        public static byte[] PackLenInfo(byte[] data) {
            int len = data.Length;
            byte[] pkg = new byte[len + 4];
            byte[] head = BitConverter.GetBytes(len);
            head.CopyTo(pkg, 0);
            data.CopyTo(pkg, 4);
            return pkg;
        }

        public static byte[] Serialize<T>(T msg) where T : PEMsg {
            using(MemoryStream ms = new MemoryStream()) {
                try {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, msg);
                    ms.Seek(0, SeekOrigin.Begin);
                    return Compress(ms.ToArray());
                }
                catch(SerializationException e) {
                    LogMsg("Failed to serialize. Reason: " + e.Message, LogLevel.Error);
                    return null;
                }
            }
        }

        public static T DeSerialize<T>(byte[] bytes) where T : PEMsg {
            using(MemoryStream ms = new MemoryStream(DeCompress(bytes))) {
                try {
                    BinaryFormatter bf = new BinaryFormatter();
                    T msg = (T)bf.Deserialize(ms);
                    return msg;
                }
                catch(SerializationException e) {
                    LogMsg("Failed to deserialize. Reason: " + e.Message + " bytesLen:" + bytes.Length, LogLevel.Error);
                    return null;
                }
            }
        }

        public static byte[] Compress(byte[] input) {
            using(MemoryStream outMS = new MemoryStream()) {
                using(GZipStream gzs = new GZipStream(outMS, CompressionMode.Compress, true)) {
                    gzs.Write(input, 0, input.Length);
                    gzs.Close();
                    return outMS.ToArray();
                }
            }
        }

        public static byte[] DeCompress(byte[] input) {
            using(MemoryStream inputMS = new MemoryStream(input)) {
                using(MemoryStream outMS = new MemoryStream()) {
                    using(GZipStream gzs = new GZipStream(inputMS, CompressionMode.Decompress)) {
                        byte[] bytes = new byte[1024];
                        int len = 0;
                        while((len = gzs.Read(bytes, 0, bytes.Length)) > 0) {
                            outMS.Write(bytes, 0, len);
                        }
                        //高版本可用
                        //gzs.CopyTo(outMS);
                        gzs.Close();
                        return outMS.ToArray();
                    }
                }
            }
        }

        #region Log
        public static bool log = true;
        public static Action<string, int> logCB = null;
        public static void LogMsg(string msg, LogLevel lv = LogLevel.None) {
            if(log != true) {
                return;
            }
            //Add Time Stamp
            msg = DateTime.Now.ToLongTimeString() + " >> " + msg;
            if(logCB != null) {
                logCB(msg, (int)lv);
            }
            else {
                if(lv == LogLevel.None) {
                    Console.WriteLine(msg);
                }
                else if(lv == LogLevel.Warn) {
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("//--------------------Warn--------------------//");
                    Console.WriteLine(msg);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if(lv == LogLevel.Error) {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("//--------------------Error--------------------//");
                    Console.WriteLine(msg);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if(lv == LogLevel.Info) {
                    //Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("//--------------------Info--------------------//");
                    Console.WriteLine(msg);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
                else {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("//--------------------Error--------------------//");
                    Console.WriteLine(msg + " >> Unknow Log Type\n");
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Log Level
    /// </summary>
    public enum LogLevel {
        None = 0,// None
        Warn = 1,//Yellow
        Error = 2,//Red
        Info = 3//Green
    }
}