
using ShawnFramework.Singleton;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using XLua;
using XLua.LuaDLL;

public class LuaManager : Singleton<LuaManager>
{
    private LuaEnv _luaEnv = null;

    public LuaEnv GlobalLuaEnv { get => _luaEnv; }

    // GC相关
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;

    protected override void Init()
    {
        _luaEnv = new LuaEnv();

        _luaEnv.AddLoader(LuaFileLoader);

        _luaEnv.DoString("require 'lua_enter'");
    }

    public void Tick()
    {
        if (Time.time - lastGCTime > GCInterval)
        {
            _luaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    private byte[] LuaFileLoader(ref string filepath)
    {
        byte[] fileContent;
// #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
//         string realPath = Application.persistentDataPath + "/Lua/" + filepath.Replace(".", "/");
//         if (File.Exists($"{realPath}.lua")) 
//         {
//             fileContent = File.ReadAllBytes($"{realPath}.lua");
//         }
//         else
//         {
//             return null;
//         }
// #endif
// #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        string realPath = Application.streamingAssetsPath + "/Lua/" + filepath.Replace(".", "/");
        if (File.Exists($"{realPath}.lua"))
        {
            fileContent = File.ReadAllBytes($"{realPath}.lua");
        }
        else
        {
            return Encoding.UTF8.GetBytes($"error('file path[{filepath}] not found!')\n");
        }
// #else
//         throw new NotImplementedException();
// #endif
        // 相对路径导入
        byte[] fileModuleNameBuffer = Encoding.UTF8.GetBytes($"local __file_module = '{filepath}';local __folder_module = '{ToFolderModuleName(filepath)}';");
        byte[] buffer = new byte[fileModuleNameBuffer.Length + fileContent.Length];
        System.Buffer.BlockCopy(fileModuleNameBuffer, 0, buffer, 0, fileModuleNameBuffer.Length);
        System.Buffer.BlockCopy(fileContent, 0, buffer, fileModuleNameBuffer.Length, fileContent.Length);

        return buffer;
    }

    private object ToFolderModuleName(string filepath)
    {
        int index = filepath.LastIndexOf(".");
        if (index > -1)
        {
            return filepath.Substring(0, index);
        }
        return string.Empty;
    }

    protected override void Dispose()
    {
        _luaEnv.Dispose();
    }
}

