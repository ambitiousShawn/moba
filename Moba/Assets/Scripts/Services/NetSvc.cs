using UnityEngine;

namespace ShawnFramework.CommonModule
{
    public class NetSvc : MonoBehaviour
    {
        public static NetSvc Instance;
        public void InitService()
        {
            Instance = this;
        }
    }
}