
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
    // ��Դ������
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
    /// ��Դ�ȸ��º���ģ�飺����AssetBundle��Դ����ϵͳ
    /// </summary>
    public class HotUpdateMgr 
    {
        private static HotUpdateMgr instance;
        public static HotUpdateMgr Instance => instance;

        // һЩ���ص�������Ϣ
        public static string RemoteFTPServerIP = "ftp://192.168.50.211";
        public static int MaxRedownloadCnt = 5; // �����ļ�������������ش���
        public static string DownloadPath = $"{Application.persistentDataPath}/";

        private MonoBehaviour mono;

        // Զ����Դ
        private Dictionary<string, AssetData> remoteAssets = new Dictionary<string, AssetData>();
        // ������Դ
        private Dictionary<string, AssetData> localAssets = new Dictionary<string, AssetData>();
        // �������б�
        private List<string> downloadList = new List<string>();

        public static void InitManager(MonoBehaviour mono)
        {
            instance = new HotUpdateMgr();
            instance.mono = mono;
        }

        /// <summary>
        /// ��ʼ�ȸ��£�
        /// </summary>
        /// <param name="overCallback">�����ص�</param>
        /// <param name="updateInfoCallBack">������Դ�ص�</param>
        public void HotUpdate(UnityAction<bool> overCallback, UnityAction<string, float> updateInfoCallBack)
        {
            remoteAssets.Clear();
            localAssets.Clear();
            downloadList.Clear();

            // ����Զ����Դ�Ա��ļ�
            updateInfoCallBack("��������Զ�˰汾�Ա��ļ�", 0.05f);
            DownloadRemoteVersionCompareFile((isOver) =>
            {
                if (isOver)
                {
                    string RemoteVCContent = File.ReadAllText(DownloadPath + "VersionCompare_Remote.txt");
                    updateInfoCallBack("���ڽ���Զ�˰汾�Ա��ļ�", 0.12f);
                    AnalyzeRemoteVCompare(RemoteVCContent, remoteAssets);

                    // ����������Դ�Ա��ļ�
                    AnalyzeLocalVersionCompare((isOver) =>
                    {
                        if (isOver)
                        {
                            updateInfoCallBack("���ڶԱȱ���/Զ����Դ�ļ�", 0.18f);
                            foreach (string assetName in  remoteAssets.Keys)
                            {
                                if (!localAssets.ContainsKey(assetName))
                                {
                                    // Զ���У������ޣ����������б�
                                    downloadList.Add(assetName);
                                }
                                else
                                {
                                    if (localAssets[assetName].md5 != remoteAssets[assetName].md5)
                                    {
                                        // ˫���У���������Դ����
                                        downloadList.Add(assetName);
                                    }
                                    // �������ʣ�µ���Զ��û�е����ݣ�ֱ��ɾ������
                                    localAssets.Remove(assetName); 
                                }
                            }
                            updateInfoCallBack("����ɾ�����õ���Դ�ļ�", 0.3f);
                            foreach (string assetName in localAssets.Keys)
                            {
                                if (File.Exists(DownloadPath + assetName))
                                {
                                    File.Delete(DownloadPath + assetName);
                                }
                            }
                            updateInfoCallBack("�������غ͸���AB���ļ�", 0.5f);
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


        // �첽����Զ����Դ�Ա��ļ� VersionCompare.txt
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

            //�����ⲿ�ɹ����
            overCallBack?.Invoke(isOver);
        }

        // ����Զ��VC�ļ���Ϣ
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

        // ����������Դ�Ա��ļ�
        private void AnalyzeLocalVersionCompare(UnityAction<bool> overCallBack)
        {
            if (File.Exists(DownloadPath + "VersionCompare.txt"))
            {
                // �ļ��Ѿ�����
                mono.StartCoroutine(IEAnalyzeLocalVersionCompare("file:///" + DownloadPath + "VersionCompare.txt", overCallBack));
            }
            else if (File.Exists(Application.streamingAssetsPath + "/VersionCompare.txt"))
            {
                // ����Ĭ����Դ
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
                // ��һ�ν�����Ϸ��û��Ĭ����Դ
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

        // �����б��д�������Դ
        private async void DownLoadMissingAsset(UnityAction<bool> overCallBack, UnityAction<string, float> updatePro)
        {
            string localPath = DownloadPath;
            //�Ƿ����سɹ�
            bool isOver = false;
            //���سɹ����б� ֮�������Ƴ����سɹ�������
            List<string> tempList = new List<string>();
            //�������ص�������
            int reDownLoadMaxNum = MaxRedownloadCnt;
            //���سɹ�����Դ��
            int downLoadOverNum = 0;
            //��һ��������Ҫ���ض��ٸ���Դ
            int downLoadMaxNum = downloadList.Count;
            //whileѭ����Ŀ�� �ǽ���n���������� ���������쳣ʱ ����ʧ��
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
                        tempList.Add(downloadList[i]);//���سɹ���¼����
                    }
                }
                //�����سɹ����ļ��� �Ӵ������б����Ƴ�
                for (int i = 0; i < tempList.Count; i++)
                    downloadList.Remove(tempList[i]);

                --reDownLoadMaxNum;
            }

            //�������ݶ��������� �����ⲿ�Ƿ��������
            overCallBack(downloadList.Count == 0);
        }

        // �����ʲ�
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
                    byte[] bytes = new byte[2048]; // һ������2KB
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
                Debug.Log(fileName + "����ʧ�ܣ�" + ex.Message);
                return false;
            }

        }
    }
}