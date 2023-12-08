// 导出给 Lua
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using XLua;

[LuaCallCSharp]
public class LuaHelper
{
    
    public static bool IsGameObjectNull(GameObject go)
    {
        return go == null;
    }

    public static bool IsTransformNull(Transform tr)
    {
        if (tr == null)
            return true;
        return tr.gameObject == null;
    }

    #region UI 组件监听
    /// <summary>
    /// 绑定按钮事件监听（TODO：加入点击音效）
    /// </summary>
    /// <param name="buttonGameObj"></param>
    /// <param name="listener"></param>
    /// <param name="audio"></param>
    /// <returns></returns>
    public static bool BindClick(GameObject buttonGameObj, UnityAction listener, string audioclip = null)
    {
        if (buttonGameObj == null || listener == null)
            return false;
        Button button = buttonGameObj.GetComponent<Button>();
        if (button == null)
            return false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            listener?.Invoke();
        });
        return true;
    }
    #endregion

    #region 文件操作
    public static void WriteToFile(string path, string content)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is empty or null.");
            return;
        }

        try
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(content);
            }
            // Debug.Log("Successfully write data to " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Fail to write data to " + path + ". Error: " + e.Message);
        }
    }

    public static string ReadFromFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is empty or null.");
            return null;
        }

        try
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File doesn't exist at " + path);
                return null;
            }

            using (StreamReader sr = new StreamReader(path))
            {
                string content = sr.ReadToEnd();
                return content;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Fail to read data from " + path + ". Error: " + e.Message);
            return null;
        }
    }

    public static void AppendToFile(string path, string content)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is empty or null.");
            return;
        }

        try
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter sw = new StreamWriter(path, true)) // Changed the second parameter to true for appending
            {
                sw.Write(content);
            }
            // Debug.Log("Successfully append data to " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Fail to append data to " + path + ". Error: " + e.Message);
        }
    }

    public static bool FileExists(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        else
        {
            return File.Exists(path);
        }
    }

    public static void MakeSureFolderExists(string folder)
    {
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
    }

    public static void MakeSureParentFolderExists(string path)
    {
        string folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
    }

    public static void DeleteFolder(string folder)
    {
        if (System.IO.Directory.Exists(folder))
        {
            try
            {
                System.IO.Directory.Delete(folder, true);
            }
            catch (System.IO.IOException e)
            {
                Debug.LogError(e.Message);
                return;
            }
        }
    }

    #endregion
}