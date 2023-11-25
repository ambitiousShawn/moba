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
            //获取一个ABTools 编辑器窗口对象
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
                
            //保存默认资源到StreamingAssets 按钮
            if (GUI.Button(new Rect(115, 110, 225, 40), "Move AssetBundles To StreamingAssets"))
            {
                MoveAssetBundlesToStreamingAssets();
            }
 
            //上传AB包和对比文件 按钮
            if (GUI.Button(new Rect(10, 160, 330, 40), "上传AB包和对比文件"))
            {
                UploadAllAssetBundles();
            }
        }

        //上传AB包文件到服务器
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
        /// 开个新线程去处理文件上传操作
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
                    // 通信凭证
                    NetworkCredential credential = new NetworkCredential("ShawG", "ShawG");
                    req.Credentials = credential;
                    req.Proxy = null;
                    req.KeepAlive = false; //  请求完毕后 是否关闭控制连接
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    req.UseBinary = true;

                    Stream upLoadStream = req.GetRequestStream();

                    using (FileStream file = File.OpenRead(filePath))
                    {
                        byte[] bytes = new byte[2048]; // 一次上传2KB
                        
                        int contentLength = file.Read(bytes, 0, bytes.Length);

                        
                        while (contentLength != 0)
                        {
                            upLoadStream.Write(bytes, 0, contentLength);
                            contentLength = file.Read(bytes, 0, bytes.Length);
                        }

                        //循环完毕后 证明上传结束
                        file.Close();
                        upLoadStream.Close();
                    }

                    Debug.Log(fileName + "上传成功");
                }
                catch (Exception ex)
                {
                    Debug.LogError(fileName + "上传失败" + ex.Message);
                }
            });
        }

        /// <summary>
        /// 将 Assets 从 AB 移动到 StreamingAssets 路径
        /// </summary>
        private void MoveAssetBundlesToStreamingAssets()
        {
            UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            // 未选择不处理
            if (selectedAsset.Length == 0)
            {
                return;
            }

            string content = string.Empty;
            foreach (UnityEngine.Object asset in selectedAsset)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

                // 非 AssetBundles 不处理
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
            //刷新窗口
            AssetDatabase.Refresh();
        }

        // 生成版本对比文件
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
            content = content.Substring(0, content.Length - 1); // 去掉最后多余的 "|"

            File.WriteAllText(TargetPath + targetPlatform[currSelIndex] + "/" + VCFileName, content);
            AssetDatabase.Refresh();

            Debug.Log("<color=#FFA500>版本对比文件生成完成！</color>");
        }

        //获取文件MD5码
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
                    sb.Append(md5Info[i].ToString("x2")); // 16进制字符串
                }

                return sb.ToString();
            }
        }
    }
}