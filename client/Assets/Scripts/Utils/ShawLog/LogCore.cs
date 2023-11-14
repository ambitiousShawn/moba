using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ShawnFramework.ShawLog
{

    public class LogCore : MonoBehaviour
    {
        public class UnityLogger : ILogger
        {
            Type type = Type.GetType("UnityEngine.Debug, UnityEngine");
            public void Log(string msg, ELogColor color = ELogColor.None)
            {
                if (color != ELogColor.None)
                {
                    msg = WriteUnityLog(msg, color);
                }
                type.GetMethod("Log", new Type[] {typeof(object)}).Invoke(null, new object[] { msg });
            }
            public void Warn(string msg)
            {
                type.GetMethod("LogWarning", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }
            public void Error(string msg)
            {
                type.GetMethod("LogError", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }

            private string WriteUnityLog(string msg, ELogColor color)
            {
                switch (color)
                {
                    case ELogColor.None:
                        break;
                    case ELogColor.Red:
                        msg = string.Format("<color=#FF0000>{0}</color>", msg);
                        break;
                    case ELogColor.Green:
                        msg = string.Format("<color=#00FF00>{0}</color>", msg); 
                        break;
                    case ELogColor.Blue:
                        msg = string.Format("<color=#0000FF>{0}</color>", msg);
                        break;
                    case ELogColor.Cyan:
                        msg = string.Format("<color=#00FFFF>{0}</color>", msg);
                        break;
                    case ELogColor.Magenta:
                        msg = string.Format("<color=#FF00FF>{0}</color>", msg);
                        break;
                    case ELogColor.Yellow:
                        msg = string.Format("<color=#FFFF00>{0}</color>", msg);
                        break;
                }
                return msg;
            }
        }

        public class ConsoleLogger : ILogger
        {
            public void Log(string msg, ELogColor color = ELogColor.None)
            {
                WriteConsoleLog(msg, color);
            }
            public void Warn(string msg)
            {
                WriteConsoleLog(msg, ELogColor.Yellow);
            }
            public void Error(string msg)
            {
                WriteConsoleLog(msg, ELogColor.Red);
            }  
            private void WriteConsoleLog(string msg, ELogColor color)
            {
                switch (color)
                {
                    case ELogColor.None:
                        break;
                    case ELogColor.Red:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case ELogColor.Green:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case ELogColor.Blue:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case ELogColor.Cyan:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case ELogColor.Magenta:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case ELogColor.Yellow:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    default:
                        Console.WriteLine(msg);
                        break;
                }
            }
        }

        // ��־����
        public static LogConfig config;
        // ����ƽ̨
        public static ILogger platform;
        // ��־������
        private static StreamWriter streamWriter = null;

        /// <summary>
        /// ��־��ʼ���ӿ�
        /// </summary>
        /// <param name="config">��־�ĸ��Ի�������Ϣ</param>
        public static void InitSettings(LogConfig config = null)
        {
            if (config == null)
            {
                config = new LogConfig();
            }
            LogCore.config = config;

            if (config.Type == EShawLogType.Console)
            {
                platform = new ConsoleLogger();
            }
            else
            {
                platform = new UnityLogger();
            }

            if (config.EnableSaveToFile)
            {
                // �����ļ��洢ϵͳ
                if (!Directory.Exists(config.SavePath))
                {
                    Directory.CreateDirectory(config.SavePath);
                }
                string time = DateTime.Now.ToString("yyyyMMdd@HH-mm-ss");
                string path = config.SavePath + time;
                try
                {
                    streamWriter = File.AppendText(path);
                    streamWriter.AutoFlush = true;
                }
                catch
                {
                    streamWriter = null;
                }
                
            }
        }

        /// <summary>
        /// ��ӡ��־ͨ�ýӿ�
        /// </summary>
        /// <param name="msg">��־��Ϣ</param>
        /// <param name="args">ͨ�ò���</param>
        public static void Log(string msg, params object[] args)
        {
            if (!config.EnableLog)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args), config.EnableTrace);
            platform.Log(msg);
            
            if (config.EnableSaveToFile)
            {
                msg = string.Format("[Log]{0}", msg);
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.WriteLine(msg);
                    }
                    catch
                    {
                        streamWriter = null;
                    }
                }
            }
        }

        public static void Log(object obj)
        {
            if (!config.EnableLog)
            {
                return;
            }

            string msg = DecorateLog(string.Format(obj.ToString()));
            platform.Log(msg);

            if (config.EnableSaveToFile)
            {
                msg = string.Format("[Log]{0}", msg);
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.WriteLine(msg);
                    }
                    catch
                    {
                        streamWriter = null;
                    }
                }
            }
        }

        /// <summary>
        /// ��ӡ��ɫ��־ͨ�ýӿ�
        /// </summary>
        /// <param name="color">��ӡ��ɫ</param>
        /// <param name="msg">��־��Ϣ</param>
        /// <param name="args">ͨ�ò���</param>
        public static void ColorLog(string msg, ELogColor color, params object[] args)
        {
            if (!config.EnableLog)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args), config.EnableTrace);
            platform.Log(msg, color);

            if (config.EnableSaveToFile)
            {
                msg = string.Format("[Log]{0}", msg);
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.WriteLine(msg);
                    }
                    catch
                    {
                        streamWriter = null;
                    }
                }
            }
        }

        public static void ColorLog(object obj, ELogColor color)
        {
            if (!config.EnableLog)
            {
                return;
            }

            string msg = DecorateLog(string.Format(obj.ToString()));
            platform.Log(msg, color);

            if (config.EnableSaveToFile)
            {
                msg = string.Format("[Log]{0}", msg);
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.WriteLine(msg);
                    }
                    catch
                    {
                        streamWriter = null;
                    }
                }
            }
        }

        /// <summary>
        /// ��ӡ������Ϣ
        /// </summary>
        /// <param name="msg">��־��Ϣ</param>
        /// <param name="args">ͨ�ò���</param>
        public static void Warn(string msg, params object[] args)
        {
            if (!config.EnableLog)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args), true);
            platform.Warn(msg);

            if (config.EnableSaveToFile)
            {
                msg = string.Format("[Warn]{0}", msg);
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.WriteLine(msg);
                    }
                    catch
                    {
                        streamWriter = null;
                    }
                }
            }
        }

        /// <summary>
        /// ��ӡ������Ϣ
        /// </summary>
        /// <param name="msg">��־��Ϣ</param>
        /// <param name="args">ͨ�ò���</param>
        public static void Error(string msg, params object[] args)
        {
            if (!config.EnableLog)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg, args), config.EnableTrace);
            platform.Error(msg);
            if (config.EnableSaveToFile)
            {
                msg = string.Format("[Error]{0}", msg);
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.WriteLine(msg);
                    }
                    catch
                    {
                        streamWriter = null;
                    }
                }
            }
        }

        private static string DecorateLog(string msg, bool trace =  false)
        {
            StringBuilder sb = new StringBuilder("#", 100);
            // ʱ��
            sb.AppendFormat(" {0}", DateTime.Now.ToString("hh:mm:ss-fff"));
            // �߳�
            sb.AppendFormat(" {0}", string.Format(" ThreadID:{0}", Thread.CurrentThread.ManagedThreadId));
            // �ָ��� + ��־��Ϣ
            sb.AppendFormat(" {0} {1}", ">>>", msg);
            // ջ֡׷��
            if (trace)
            {
                sb.AppendFormat(" \nStackTrace:{0}", GetLogTrace());
            }
            
            return sb.ToString();
        }

        private static string GetLogTrace()
        {
            StackTrace st = new StackTrace(3, true);

            string traceInfo = "";
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                traceInfo += string.Format("\n  {0}::{1} line:{2}", sf.GetFileName(), sf.GetMethod(), sf.GetFileLineNumber());
            }
            return traceInfo;
        }
    }

}