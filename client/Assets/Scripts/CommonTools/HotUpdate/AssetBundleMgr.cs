using ShawnFramework.ShawLog;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using XLua;
using static UnityEngine.GraphicsBuffer;

namespace ShawnFramework.ShawHotUpdate
{
    /// <summary>
    /// 基于 AssetBundle 的 资产管理器
    /// </summary>
    [LuaCallCSharp]
    public class AssetBundleMgr 
    {
        private static AssetBundleMgr instance;
        public static AssetBundleMgr Instance => instance;

        private AssetBundle mainABPackage = null;
        private AssetBundleManifest manifest = null;
        private Dictionary<string, AssetBundle> nameToABDic = new Dictionary<string, AssetBundle>();
        private MonoBehaviour mono;

        public static string SteamingAssetsPath = Application.streamingAssetsPath;
        public static string PersistentDataPath = Application.persistentDataPath;
        
        // 根目录名称
        public static string RootFolderName
        {
            get
            {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
                return "PC";
#endif
            }
        }

        /// <summary>
        /// AssetBundleMgr资源读取规则：
        ///     优先级1：从 persistantDataPath 中读取 asset(热更新资源路径)
        ///     优先级2：从 streamingAssetsPath 中读取 asset (默认资源路径)
        /// </summary>

        /// <summary>
        /// 初始化AB包管理器
        /// </summary>
        public static void InitManager(MonoBehaviour mono)
        {
            instance = new AssetBundleMgr();
            instance.mono = mono;
            // 加载主包和配置(主包在默认资源)
            if (instance.mainABPackage == null)
            {
                instance.mainABPackage = AssetBundle.LoadFromFile($"{SteamingAssetsPath}/{RootFolderName}");
                instance.manifest = instance.mainABPackage.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            LogCore.ColorLog("AssetBundle管理模块已初始化！", ELogColor.Orange);
        }

        /// <summary>
        /// 资源的同步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packageName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string packageName, string resName) where T : Object
        {
            // 加载依赖包
            string[] dependencies = manifest.GetAllDependencies(packageName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependence = dependencies[i];
                if (!nameToABDic.ContainsKey(dependence))
                {
                    AssetBundle target;
                    if (File.Exists($"{PersistentDataPath}/{dependence}"))
                    {
                        target = AssetBundle.LoadFromFile($"{PersistentDataPath}/{dependence}");
                    }
                    else
                    {
                        target = AssetBundle.LoadFromFile($"{SteamingAssetsPath}/{dependence}");
                    }
                    nameToABDic.Add(dependence, target);
                }
            }
            // 加载目标包
            if (!nameToABDic.ContainsKey(packageName))
            {
                AssetBundle target;
                if (File.Exists($"{PersistentDataPath}/{packageName}"))
                {
                    target = AssetBundle.LoadFromFile($"{PersistentDataPath}/{packageName}");
                }
                else
                {
                    target = AssetBundle.LoadFromFile($"{SteamingAssetsPath}/{packageName}");
                }
                nameToABDic.Add(packageName, target) ;
            }

            T obj = nameToABDic[packageName].LoadAsset<T>(resName);
            if (obj is GameObject)
                return GameObject.Instantiate(obj);
            else
                return obj;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packageName"></param>
        /// <param name="resName"></param>
        /// <param name="callBack"></param>
        public void LoadAssetAsync<T>(string packageName, string resName, UnityAction<T> callBack) where T : Object
        {
            mono.StartCoroutine(IE_ReallyLoadResAsync<T>(packageName, resName, callBack));
        }
        private IEnumerator IE_ReallyLoadResAsync<T>(string packageName, string resName, UnityAction<T> callBack) where T : Object
        {
            // 加载依赖包
            string[] dependencies = manifest.GetAllDependencies(packageName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependence = dependencies[i];
                if (!nameToABDic.ContainsKey(dependence))
                {
                    AssetBundle target;
                    if (File.Exists($"{PersistentDataPath}/{dependence}"))
                    {
                        target = AssetBundle.LoadFromFile($"{PersistentDataPath}/{dependence}");
                    }
                    else
                    {
                        target = AssetBundle.LoadFromFile($"{SteamingAssetsPath}/{dependence}");
                    }
                    nameToABDic.Add(dependence, target);
                }
            }
            //加载目标包
            if (!nameToABDic.ContainsKey(packageName))
            {
                AssetBundle target;
                if (File.Exists($"{PersistentDataPath}/{packageName}"))
                {
                    target = AssetBundle.LoadFromFile($"{PersistentDataPath}/{packageName}");
                }
                else
                {
                    target = AssetBundle.LoadFromFile($"{SteamingAssetsPath}/{packageName}");
                }
                nameToABDic.Add(packageName, target);
            }
            //异步加载包中资源
            AssetBundleRequest abq = nameToABDic[packageName].LoadAssetAsync<T>(resName);
            yield return abq;

            if (abq.asset is GameObject)
                callBack(GameObject.Instantiate(abq.asset) as T);
            else
                callBack(abq.asset as T);
        }

        //卸载AB包的方法
        public void UnLoadAB(string name)
        {
            if (nameToABDic.ContainsKey(name))
            {
                nameToABDic[name].Unload(false);
                nameToABDic.Remove(name);
            }
        }

        //清空AB包的方法
        public void ClearAB()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            nameToABDic.Clear();
            //卸载主包
            mainABPackage = null;
        }
    }

}