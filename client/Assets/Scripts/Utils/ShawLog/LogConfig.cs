using System;
using System.Collections;
using System.Collections.Generic;
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
    public class LogConfig : MonoBehaviour
    {
        public bool EnableLog = true;
        public bool EnableSaveToFile = false;
        public bool EnableTrace = false;
# if UNITY_EDITOR
        public string SavePath = string.Format("{0}Logs\\", Application.dataPath + "/");
#else
        public string SavePath = string.Format("{0}Logs\\", AppDomain.CurrentDomain.BaseDirectory);
#endif
        public EShawLogType Type = EShawLogType.Unity;
    }

    public interface ILogger
    {
        void Log(string msg, ELogColor color = ELogColor.None);
        void Warn(string msg);
        void Error(string msg);
    }
}