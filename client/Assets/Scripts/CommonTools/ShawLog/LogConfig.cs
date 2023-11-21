using UnityEngine;

namespace ShawnFramework.ShawLog
{
    public enum ELogColor
    {
        None,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow,
        Orange,
    }
    public enum EShawLogType
    {
        Unity,
        Console,
    }
    public class LogConfig 
    {
        public bool EnableLog = true;
        public bool EnableSaveToFile = false;
        public bool EnableThread = false;
        public string StartChar = "# ";
        public string SplitChar = ">>>";
        public int MaxLogFileCount = 50;

        // 日志保存路径
        public string SavePathFolder = string.Format("{0}\\", Application.persistentDataPath);
        
        public EShawLogType Type = EShawLogType.Unity;
    }

    public interface ILogger
    {
        void Log(string msg, ELogColor color = ELogColor.None);
        void Warn(string msg);
        void Error(string msg);
    }
}