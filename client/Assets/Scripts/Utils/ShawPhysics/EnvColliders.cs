using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShawnFramework.ShawnPhysics
{
    /// <summary>
    /// 使用方式：
    ///     使用前自行整理好[环境]中的碰撞信息存入该容器后
    ///     初始化时调用Init()方法
    ///     随后在FixedUpdate下调用 CalcColliderInteraction() 即可做 碰撞检测 + 速度矫正
    /// </summary>
    public class EnvColliders : MonoBehaviour
    {
        
        public List<ColliderConfig> colliderConfigLst;

        List<ShawColliderBase> envColliderLst;
        
        public void Init()
        {
            envColliderLst = new List<ShawColliderBase>();
            for (int i = 0;  i < colliderConfigLst.Count; i++)
            {
                ColliderConfig config = colliderConfigLst[i];
                if (config.mType == ColliderType.Box)
                {
                    envColliderLst.Add(new ShawBoxCollider(config));
                }
                else if (config.mType == ColliderType.Cylinder)
                {
                    envColliderLst.Add(new ShawCylinderCollider(config));
                }
                else
                {
                    // TODO
                }
            }
        }

        /// <summary>
        /// 拿到场景中所有的碰撞体信息
        /// </summary>
        /// <returns></returns>
        public List<ShawColliderBase> GetAllEnvColliders() 
        {
            return envColliderLst;
        }
    }

}