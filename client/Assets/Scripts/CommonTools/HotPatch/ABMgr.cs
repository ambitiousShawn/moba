using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShawnFramework.ShawHotPatch
{
    /// <summary>
    /// 基于 AssetBundle 的 资产管理器
    /// </summary>
    public class ABMgr 
    {
        private AssetBundle mainABPackage = null;
        private AssetBundleManifest manifest = null;
        private Dictionary<string, AssetBundle> nameToABDic = new Dictionary<string, AssetBundle>();
        private MonoBehaviour mono;
        
        // 获取AB包加载路径
        public string LoadPath = Application.streamingAssetsPath + "/";
        // 根目录名称
        public string RootFolderName
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
        /// 初始化AB包管理器
        /// </summary>
        public void InitAssetBundle(MonoBehaviour mono)
        {
            this.mono = mono;
            // 加载主包和配置
            if (mainABPackage == null)
            {
                mainABPackage = AssetBundle.LoadFromFile(LoadPath + RootFolderName);
                manifest = mainABPackage.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
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
                    AssetBundle target = AssetBundle.LoadFromFile (LoadPath + dependence);
                    nameToABDic.Add(dependence, target);
                }
            }
            // 加载目标包
            if (!nameToABDic.ContainsKey(packageName))
            {
                AssetBundle target = AssetBundle.LoadFromFile(LoadPath + packageName);
                nameToABDic.Add(packageName, target) ;
            }

            T obj = nameToABDic[packageName].LoadAsset<T>(resName);
            if (obj is GameObject)
                return GameObject.Instantiate(obj);
            else
                return obj;
        }

        /// <summary>
        /// Type同步加载指定资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadAsset(string packageName, string resName, System.Type type)
        {
            // 加载依赖包
            string[] dependencies = manifest.GetAllDependencies(packageName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependence = dependencies[i];
                if (!nameToABDic.ContainsKey(dependence))
                {
                    AssetBundle target = AssetBundle.LoadFromFile(LoadPath + dependence);
                    nameToABDic.Add(dependence, target);
                }
            }
            // 加载目标包
            if (!nameToABDic.ContainsKey(packageName))
            {
                AssetBundle target = AssetBundle.LoadFromFile(LoadPath + packageName);
                nameToABDic.Add(packageName, target) ;
            }
            Object obj = nameToABDic[packageName].LoadAsset(resName, type);
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
                    AssetBundle target = AssetBundle.LoadFromFile(LoadPath + dependence);
                    nameToABDic.Add(dependence, target);
                }
            }
            //加载目标包
            if (!nameToABDic.ContainsKey(packageName))
            {
                AssetBundle target = AssetBundle.LoadFromFile(LoadPath + packageName);
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

        /// <summary>
        /// Type异步加载资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        public void LoadAssetAsync(string packageName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            mono.StartCoroutine(ReallyLoadResAsync(packageName, resName, type, callBack));
        }
        private IEnumerator ReallyLoadResAsync(string packageName, string resName, System.Type type, UnityAction<Object> callBack)
        {
            // 加载依赖包
            string[] dependencies = manifest.GetAllDependencies(packageName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependence = dependencies[i];
                if (!nameToABDic.ContainsKey(dependence))
                {
                    AssetBundle target = AssetBundle.LoadFromFile(LoadPath + dependence);
                    nameToABDic.Add(dependence, target);
                }
            }
            //加载目标包
            if (!nameToABDic.ContainsKey(packageName))
            {
                AssetBundle target = AssetBundle.LoadFromFile(LoadPath + packageName);
                nameToABDic.Add(packageName, target);
            }
            //异步加载包中资源
            AssetBundleRequest abq = nameToABDic[packageName].LoadAssetAsync(resName, type);
            yield return abq;

            if (abq.asset is GameObject)
                callBack(GameObject.Instantiate(abq.asset));
            else
                callBack(abq.asset);
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