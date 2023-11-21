using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ShawnFramework.ShawLog
{

    public class LogCore 
    {
        // ��־����
        public static LogConfig config;
        // ����ƽ̨
        public static ILogger platform;

        // ����·���ļ���
        private static DirectoryInfo logDirectory = null;
        private static int maxLogFileCount = 50;
        private static object logsLocker = new object(); // д������
        private static string targetPath = ""; 
        private static List<string> logs = new List<string>(); // ��д�����Ϣ��

        /// ��־��ʼ���ӿ�
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
                maxLogFileCount = config.MaxLogFileCount;
                // �����ļ��洢ϵͳ
                if (!Directory.Exists(config.SavePathFolder))
                {
                    logDirectory = Directory.CreateDirectory(config.SavePathFolder);
                }
                else
                {
                    logDirectory = new DirectoryInfo(config.SavePathFolder);
                }

                string time = DateTime.Now.ToString("yyyyMMdd@HH-mm-ss");
                targetPath = config.SavePathFolder + time + ".log";
                AutoClearEarlyLogFile();
                Application.logMessageReceivedThreaded += (content, stacktrace, type) =>
                {
                    lock (logsLocker)
                    {
                        string msg = $"[{time:s}][{type}] {content}";
                        if (type != LogType.Log)
                        {
                            msg += $"[Stack:{stacktrace}]";
                        }
                        logs.Add(msg);
                    }
                };
                // ���������߳�ˢ��IO��
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        FlushLog();
                    }
                });
            }
        }

        #region ������õ�API
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

            msg = DecorateLog(string.Format(msg, args));
            platform.Log(msg);
        }

        public static void Log(object obj)
        {
            if (!config.EnableLog)
            {
                return;
            }

            string msg = DecorateLog(string.Format(obj.ToString()));
            platform.Log(msg);
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

            msg = DecorateLog(string.Format(msg, args));
            platform.Log(msg, color);
        }

        /// <summary>
        /// ��ӡ������Ϣ
        /// </summary>
        /// <param name="msg">��־��Ϣ</param>
        /// <param name="args">ͨ�ò���</param>
        public static void Warn(string msg)
        {
            if (!config.EnableLog)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg));
            platform.Warn(msg);
        }

        /// <summary>
        /// ��ӡ������Ϣ
        /// </summary>
        /// <param name="msg">��־��Ϣ</param>
        /// <param name="args">ͨ�ò���</param>
        public static void Error(string msg)
        {
            if (!config.EnableLog)
            {
                return;
            }

            msg = DecorateLog(string.Format(msg));
            platform.Error(msg);
        }

        private static string DecorateLog(string msg)
        {
            StringBuilder sb = new StringBuilder(config.StartChar, 100);
            // �߳�
            if (config.EnableThread)
            {
                sb.AppendFormat(" {0}", string.Format(" ThreadID:{0}", Thread.CurrentThread.ManagedThreadId));
            }
            // �ָ��� + ��־��Ϣ
            sb.Append(msg);
            return sb.ToString();
        }
        #endregion

        #region ��ͬƽ̨����־��ӡ
        public class UnityLogger : ILogger
        {
            Type type = Type.GetType("UnityEngine.Debug, UnityEngine");
            public void Log(string msg, ELogColor color = ELogColor.None)
            {
                if (color != ELogColor.None)
                {
                    msg = WriteUnityLog(msg, color);
                }
                type.GetMethod("Log", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
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
                    case ELogColor.Orange:
                        msg = string.Format("<color=#FFA500>{0}</color>", msg);
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

        #endregion

        #region ��־���������ڹ���
        // �Զ�����������־�ļ�
        static void AutoClearEarlyLogFile()
        {
            List<DateTime> createTimeLst = new List<DateTime>();
            foreach (FileInfo file in logDirectory.GetFiles())
            {
                if (file.Extension == ".log")
                {
                    createTimeLst.Add(file.CreationTime);
                }
            }

            // ɾ���������־�ļ�
            if (createTimeLst.Count >= maxLogFileCount)
            {
                createTimeLst.Sort();
                DateTime oldestTime = createTimeLst[createTimeLst.Count - maxLogFileCount];
                foreach (FileInfo file in logDirectory.GetFiles())
                {
                    if (file.Extension == ".log" && file.CreationTime <= oldestTime)
                    {
                        file.Delete();
                    }
                }
            }
        }

        // ˢ����־д���ļ�
        static void FlushLog()
        {
            lock (logsLocker)
            {
                StreamWriter stream = new StreamWriter(targetPath, true);
                List<string> temp = logs;
                logs = new List<string>();
                foreach (string line in temp)
                {
                    stream.WriteLine(line);
                    stream.Flush();
                }
                stream.Close();
            }
        }

        // �õ�������־�ļ�(��ʱ��˳������)
        public List<FileInfo> GetAllLogFilesOrderByTime()
        {
            return logDirectory.GetFiles()
                .Where(file => file.Extension == ".log")
                .OrderBy(f => f.CreationTime.Ticks)
                .ToList();
        }
        #endregion
    }
}