

using ShawnFramework.CommonModule;
using UnityEngine;
using UnityEngine.UI;

/*
    UGUI面板基类：
     - 1. 面板的激活与失活
     - 2. 面板的初始化
     - 3. 面板的反初始化
 */
namespace ShawnFramework.UGUI
{
    public abstract class PanelRoot : MonoBehaviour
    {
        protected Launcher root;
        protected AssetsSvc assetsSvc;
        protected AudioSvc audioSvc;
        protected NetSvc netSvc;

        public void SetActive(bool isActive = true)
        {
            gameObject.SetActive(isActive);
            if (isActive)
            {
                InitPanel();
            }
            else
            {
                UninitPanel();
            }
        }
        public void SetActive(Image img, bool isActive = true)
        {
            img.gameObject.SetActive(isActive);
        }
        protected virtual void InitPanel()
        {
            root = Launcher.Instance;
            assetsSvc = AssetsSvc.Instance;
            audioSvc = AudioSvc.Instance;
            netSvc = NetSvc.Instance;
        }

        protected virtual void UninitPanel()
        {
            root = null;
            assetsSvc = null;
            audioSvc = null;
            netSvc = null;
        }
    }

}