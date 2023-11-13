

using ShawnFramework.CommonModule;
using UnityEngine;

public abstract class SystemRoot : MonoBehaviour
{
    protected Launcher root;
    protected AssetsSvc assetsSvc;
    protected AudioSvc audioSvc;
    protected NetSvc netSvc;

    public virtual void InitSystem()
    {
        root = Launcher.Instance;
        assetsSvc = AssetsSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
    }
}

