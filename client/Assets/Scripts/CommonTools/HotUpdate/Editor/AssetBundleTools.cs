using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ShawnFramework.ShawHotUpdate
{

    public class AssetBundleTools : EditorWindow
    {
        [MenuItem("AssetBundle Tools/Open Upload Window")]
        private static void OpenWindow()
        {
            //��ȡһ��ABTools �༭�����ڶ���
            AssetBundleTools windown = GetWindowWithRect(typeof(AssetBundleTools), new Rect(0, 0, 350, 220)) as AssetBundleTools;
            windown.Show();
        }

        private int currSelIndex = 0;
        private string[] targetPlatform = new string[] { "PC", "IOS", "Android" };
        private string serverIP = "ftp://192.168.50.211";

        private readonly string TargetPath = Application.dataPath + "/AssetBundles/";
        private readonly string VCFileName = "VersionCompare.txt";

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 150, 15), "Platform");
            currSelIndex = GUI.Toolbar(new Rect(10, 30, 250, 20), currSelIndex, targetPlatform);
            GUI.Label(new Rect(10, 60, 150, 15), "IP");
            serverIP = GUI.TextField(new Rect(10, 80, 150, 20), serverIP);
            
            if (GUI.Button(new Rect(10, 110, 100, 40), "Generator VersionCompare.txt"))
            {
                GeneratorVCFile();
            }
                
            //����Ĭ����Դ��StreamingAssets ��ť
            if (GUI.Button(new Rect(115, 110, 225, 40), "Move AssetBundles To StreamingAssets"))
            {
                MoveAssetBundlesToStreamingAssets();
            }
 
            //�ϴ�AB���ͶԱ��ļ� ��ť
            if (GUI.Button(new Rect(10, 160, 330, 40), "�ϴ�AB���ͶԱ��ļ�"))
            {
                UploadAllAssetBundles();
            }
        }

        //�ϴ�AB���ļ���������
        private void UploadAllAssetBundles()
        {
            DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/Assets/AssetBundles/" + targetPlatform[currSelIndex]);
            FileInfo[] fileInfos = directory.GetFiles();

            int total = fileInfos.Length;
            for (int i = 0; i < total; i++)
            {
                FileInfo info = fileInfos[i];
                if (info.Extension == "" ||
                    info.Extension == ".txt")
                {
                    FtpUploadFile(info.FullName, info.Name);
                }
            }
        }

        /// <summary>
        /// �������߳�ȥ�����ļ��ϴ�����
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="name"></param>
        private async void FtpUploadFile(string filePath, string fileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AssetBundles/" + targetPlatform[currSelIndex] + "/" + fileName)) as FtpWebRequest;
                    // ͨ��ƾ֤
                    NetworkCredential credential = new NetworkCredential("ShawG", "ShawG");
                    req.Credentials = credential;
                    req.Proxy = null;
                    req.KeepAlive = false; //  ������Ϻ� �Ƿ�رտ�������
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    req.UseBinary = true;

                    Stream upLoadStream = req.GetRequestStream();

                    using (FileStream file = File.OpenRead(filePath))
                    {
                        byte[] bytes = new byte[2048]; // һ���ϴ�2KB
                        
                        int contentLength = file.Read(bytes, 0, bytes.Length);

                        
                        while (contentLength != 0)
                        {
                            upLoadStream.Write(bytes, 0, contentLength);
                            contentLength = file.Read(bytes, 0, bytes.Length);
                        }

                        //ѭ����Ϻ� ֤���ϴ�����
                        file.Close();
                        upLoadStream.Close();
                    }

                    Debug.Log(fileName + "�ϴ��ɹ�");
                }
                catch (Exception ex)
                {
                    Debug.LogError(fileName + "�ϴ�ʧ��" + ex.Message);
                }
            });
        }

        /// <summary>
        /// �� Assets �� AB �ƶ��� StreamingAssets ·��
        /// </summary>
        private void MoveAssetBundlesToStreamingAssets()
        {
            UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            // δѡ�񲻴���
            if (selectedAsset.Length == 0)
            {
                return;
            }

            string content = string.Empty;
            foreach (UnityEngine.Object asset in selectedAsset)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

                // �� AssetBundles ������
                if (fileName.IndexOf('.') != -1)
                {
                    continue;
                }
                AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets" + fileName);

                FileInfo info = new FileInfo(Application.streamingAssetsPath + fileName);
                content += $"{info.Name} {info.Length} {GetMD5(info.FullName)}|";
            }
            if (content.Length <= 0)
            {
                Debug.LogWarning($"AssetBundles/{targetPlatform[currSelIndex]} is null!");
                return;
            }
            content = content.Substring(0, content.Length - 1);
            File.WriteAllText(Application.streamingAssetsPath + "/" + VCFileName, content);
            //ˢ�´���
            AssetDatabase.Refresh();
        }

        // ���ɰ汾�Ա��ļ�
        private void GeneratorVCFile()
        {
            DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/AssetBundles/" + targetPlatform[currSelIndex]);
            FileInfo[] fileInfos = directory.GetFiles();

            string content = string.Empty;

            foreach (FileInfo info in fileInfos)
            {
                if (info.Extension == string.Empty)
                {
                    content += $"{info.Name} {info.Length} {GetMD5(info.FullName)}|";
                }
            }
            if (content.Length <= 0)
            {
                Debug.LogWarning($"AssetBundles/{targetPlatform[currSelIndex]} is null!");
                return;
            }
            content = content.Substring(0, content.Length - 1); // ȥ��������� "|"

            File.WriteAllText(TargetPath + targetPlatform[currSelIndex] + "/" + VCFileName, content);
            AssetDatabase.Refresh();

            Debug.Log("<color=#FFA500>�汾�Ա��ļ�������ɣ�</color>");
        }

        //��ȡ�ļ�MD5��
        private string GetMD5(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] md5Info = md5.ComputeHash(file);

                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < md5Info.Length; i++)
                {
                    sb.Append(md5Info[i].ToString("x2")); // 16�����ַ���
                }

                return sb.ToString();
            }
        }
    }
}