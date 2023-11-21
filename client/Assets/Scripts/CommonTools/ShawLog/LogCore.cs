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
        // 日志配置
        public static LogConfig config;
        // 适用平台
        public static ILogger platform;

        // 保存路径文件夹
        private static DirectoryInfo logDirectory = null;
        private static int maxLogFileCount = 50;
        private static object logsLocker = new object(); // 写入流锁
        private static string targetPath = ""; 
        private static List<string> logs = new List<string>(); // 待写入的信息流

        /// 日志初始化接口
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
                // 开启文件存储系统
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
                // 单独开个线程刷新IO流
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

        #region 对外调用的API
        /// <summary>
        /// 打印日志通用接口
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="args">通用参数</param>
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
        /// 打印彩色日志通用接口
        /// </summary>
        /// <param name="color">打印颜色</param>
        /// <param name="msg">日志信息</param>
        /// <param name="args">通用参数</param>
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
        /// 打印警告信息
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="args">通用参数</param>
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
        /// 打印报错信息
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="args">通用参数</param>
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
            // 线程
            if (config.EnableThread)
            {
                sb.AppendFormat(" {0}", string.Format(" ThreadID:{0}", Thread.CurrentThread.ManagedThreadId));
            }
            // 分隔符 + 日志信息
            sb.Append(msg);
            return sb.ToString();
        }
        #endregion

        #region 不同平台的日志打印
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

        #region 日志的生命周期管理
        // 自动清除过早的日志文件
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

            // 删除较早的日志文件
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

        // 刷新日志写入文件
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

        // 拿到所有日志文件(按时间顺序排列)
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