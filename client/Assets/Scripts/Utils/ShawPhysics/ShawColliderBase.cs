using ShawnFramework.ShawMath;
using System.Numerics;
using UnityEngine;

namespace ShawnFramework.ShawnPhysics
{

    public abstract class ShawColliderBase
    {
        public string name;
        public ShawVector3 mPos;

        /// <summary>
        /// 碰撞检测核心逻辑
        /// </summary>
        /// <param name="collider">另一个碰撞体</param>
        /// <param name="normal">检测到的法线坐标</param>
        /// <param name="boarderAdjust">边界调整</param>
        /// <returns></returns>
        public virtual bool CollisionDetect(ShawColliderBase collider, ref ShawVector3 normal, ref ShawVector3 borderAdjust)
        {
            if (collider is ShawBoxCollider)
            {
                return BoxCollisionDetect(collider as ShawBoxCollider, ref normal, ref borderAdjust);
            }
            else if (collider is ShawCylinderCollider)
            {
                return SphereCollisionDetect(collider as ShawCylinderCollider, ref normal, ref borderAdjust);
            }
            else
            {
                return false;
            }
        }

        public abstract bool BoxCollisionDetect(ShawBoxCollider collider, ref ShawVector3 normal, ref ShawVector3 borderAdjust);

        public abstract bool SphereCollisionDetect(ShawCylinderCollider collider, ref ShawVector3 normal, ref ShawVector3 borderAdjust);
    }

    public class CollisionInfo
    {
        public ShawColliderBase collider;
        public ShawVector3 normal;
        public ShawVector3 borderAdjust;
    }
}