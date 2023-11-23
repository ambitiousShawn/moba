
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ShawnFramework.ShawHotUpdate
{
    // 资源数据类
    public class AssetData
    {
        public string name;
        public long size;
        public string md5;

        public AssetData(string name, string size, string md5)
        {
            this.name = name;
            this.size = long.Parse(size);
            this.md5 = md5;
        }
    }

    /// <summary>
    /// 资源热更新核心模块：基于AssetBundle资源管理系统
    /// </summary>
    public class HotUpdateMgr 
    {
        private static HotUpdateMgr instance;
        public static HotUpdateMgr Instance => instance;

        // 一些下载的配置信息
        public static string RemoteFTPServerIP = "ftp://192.168.50.211";
        public static int MaxRedownloadCnt = 5; // 单个文件的最大重新下载次数
        public static string DownloadPath = $"{Application.persistentDataPath}/";

        private MonoBehaviour mono;

        // 远端资源
        private Dictionary<string, AssetData> remoteAssets = new Dictionary<string, AssetData>();
        // 本地资源
        private Dictionary<string, AssetData> localAssets = new Dictionary<string, AssetData>();
        // 待下载列表
        private List<string> downloadList = new List<string>();

        public static void InitManager(MonoBehaviour mono)
        {
            instance = new HotUpdateMgr();
            instance.mono = mono;
        }

        /// <summary>
        /// 开始热更新！
        /// </summary>
        /// <param name="overCallback">结束回调</param>
        /// <param name="updateInfoCallBack">更新资源回调</param>
        public void HotUpdate(UnityAction<bool> overCallback, UnityAction<string, float> updateInfoCallBack)
        {
            remoteAssets.Clear();
            localAssets.Clear();
            downloadList.Clear();

            // 下载远端资源对比文件
            updateInfoCallBack("正在下载远端版本对比文件", 0.05f);
            DownloadRemoteVersionCompareFile((isOver) =>
            {
                if (isOver)
                {
                    string RemoteVCContent = File.ReadAllText(DownloadPath + "VersionCompare_Remote.txt");
                    updateInfoCallBack("正在解析远端版本对比文件", 0.12f);
                    AnalyzeRemoteVCompare(RemoteVCContent, remoteAssets);

                    // 解析本地资源对比文件
                    AnalyzeLocalVersionCompare((isOver) =>
                    {
                        if (isOver)
                        {
                            updateInfoCallBack("正在对比本地/远端资源文件", 0.18f);
                            foreach (string assetName in  remoteAssets.Keys)
                            {
                                if (!localAssets.ContainsKey(assetName))
                                {
                                    // 远端有，本地无，加入下载列表
                                    downloadList.Add(assetName);
                                }
                                else
                                {
                                    if (localAssets[assetName].md5 != remoteAssets[assetName].md5)
                                    {
                                        // 双端有，但存在资源更新
                                        downloadList.Add(assetName);
                                    }
                                    // 本地最后剩下的是远端没有的内容，直接删除即可
                                    localAssets.Remove(assetName); 
                                }
                            }
                            updateInfoCallBack("正在删除无用的资源文件", 0.3f);
                            foreach (string assetName in localAssets.Keys)
                            {
                                if (File.Exists(DownloadPath + assetName))
                                {
                                    File.Delete(DownloadPath + assetName);
                                }
                            }
                            updateInfoCallBack("正在下载和更新AB包文件", 0.5f);
                            DownLoadMissingAsset((isOver) =>
                            {
                                if (isOver)
                                {
                                    File.WriteAllText(DownloadPath + "VersionCompare.txt", RemoteVCContent);
                                }
                                overCallback(isOver);
                            }, updateInfoCallBack);
                        }
                        else
                        {
                            overCallback(false);
                        }
                    });
                }
                else
                {
                    overCallback(false);
                }
            });
        }


        // 异步下载远端资源对比文件 VersionCompare.txt
        private async void DownloadRemoteVersionCompareFile(UnityAction<bool> overCallBack)
        {
            bool isOver = false;
            int reDownLoadMaxNum = 5;

            while (!isOver && reDownLoadMaxNum > 0)
            {
                await Task.Run(() => 
                {
                    isOver = DownLoadAsset("VersionCompare.txt", DownloadPath + "VersionCompare_Remote.txt");
                });
                --reDownLoadMaxNum;
            }

            //告诉外部成功与否
            overCallBack?.Invoke(isOver);
        }

        // 解析远端VC文件信息
        private void AnalyzeRemoteVCompare(string content, Dictionary<string, AssetData> remoteAssets)
        {
            string[] infos = content.Split('|');
            string[] info = null;
            for (int i = 0; i < infos.Length; i++)
            {
                info = infos[i].Split(' '); // name / length / md5
                remoteAssets.Add(info[0], new AssetData(info[0], info[1], info[2]));
            }
        }

        // 解析本地资源对比文件
        private void AnalyzeLocalVersionCompare(UnityAction<bool> overCallBack)
        {
            if (File.Exists(DownloadPath + "VersionCompare.txt"))
            {
                // 文件已经存在
                mono.StartCoroutine(IEAnalyzeLocalVersionCompare("file:///" + DownloadPath + "VersionCompare.txt", overCallBack));
            }
            else if (File.Exists(Application.streamingAssetsPath + "/VersionCompare.txt"))
            {
                // 加载默认资源
                string path =
#if UNITY_ANDROID
                Application.streamingAssetsPath;
#else
                    "file:///" + Application.streamingAssetsPath;
#endif
                mono.StartCoroutine(IEAnalyzeLocalVersionCompare(path + "/VersionCompare.txt", overCallBack));
            }
            else
            {
                // 第一次进入游戏，没有默认资源
                overCallBack(true);
            }
        }

        private IEnumerator IEAnalyzeLocalVersionCompare(string filePath, UnityAction<bool> overCallBack)
        {
            UnityWebRequest req = UnityWebRequest.Get(filePath);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                AnalyzeRemoteVCompare(req.downloadHandler.text, localAssets);
                overCallBack(true);
            }
            else
                overCallBack(false);
        }

        // 下载列表中待下载资源
        private async void DownLoadMissingAsset(UnityAction<bool> overCallBack, UnityAction<string, float> updatePro)
        {
            string localPath = DownloadPath;
            //是否下载成功
            bool isOver = false;
            //下载成功的列表 之后用于移除下载成功的内容
            List<string> tempList = new List<string>();
            //重新下载的最大次数
            int reDownLoadMaxNum = MaxRedownloadCnt;
            //下载成功的资源数
            int downLoadOverNum = 0;
            //这一次下载需要下载多少个资源
            int downLoadMaxNum = downloadList.Count;
            //while循环的目的 是进行n次重新下载 避免网络异常时 下载失败
            while (downloadList.Count > 0 && reDownLoadMaxNum > 0)
            {
                for (int i = 0; i < downloadList.Count; i++)
                {
                    isOver = false;
                    await Task.Run(() => 
                    {
                        isOver = DownLoadAsset(downloadList[i], localPath + downloadList[i]);
                    });
                    if (isOver)
                    {
                        updatePro(++downLoadOverNum + "/" + downLoadMaxNum, 0.5f + (downLoadOverNum / (float)downLoadMaxNum) / 2);
                        tempList.Add(downloadList[i]);//下载成功记录下来
                    }
                }
                //把下载成功的文件名 从待下载列表中移除
                for (int i = 0; i < tempList.Count; i++)
                    downloadList.Remove(tempList[i]);

                --reDownLoadMaxNum;
            }

            //所有内容都下载完了 告诉外部是否下载完成
            overCallBack(downloadList.Count == 0);
        }

        // 下载资产
        private bool DownLoadAsset(string fileName, string localPath)
        {
            try
            {
                string platform =
#if UNITY_IOS
            "IOS";
#elif UNITY_ANDROID
            "Android";
#else
                "PC";
#endif
                
                FtpWebRequest req = FtpWebRequest.Create(new Uri($"{RemoteFTPServerIP}/AssetBundles/{platform}/{fileName}")) as FtpWebRequest;
                NetworkCredential n = new NetworkCredential("ShawG", "ShawG");
                req.Credentials = n;
                req.Proxy = null;
                req.KeepAlive = false;
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                req.UseBinary = true;
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                Stream downLoadStream = res.GetResponseStream();
                using (FileStream file = File.Create(localPath))
                {
                    byte[] bytes = new byte[2048]; // 一次下载2KB
                    int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);

                    while (contentLength != 0)
                    {
                        file.Write(bytes, 0, contentLength);
                        contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    }

                    file.Close();
                    downLoadStream.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(fileName + "下载失败！" + ex.Message);
                return false;
            }

        }
    }
}