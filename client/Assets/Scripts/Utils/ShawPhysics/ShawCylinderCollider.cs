using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System.Collections.Generic;

namespace ShawnFramework.ShawnPhysics
{
    public class ShawCylinderCollider : ShawColliderBase
    {
        public ShawInt mRadius;

        public ShawCylinderCollider(ColliderConfig config)
        {
            name = config.mName;
            mPos = config.mPos;
            mRadius = config.mRadius;
        }

        public override bool BoxCollisionDetect(ShawBoxCollider collider, ref ShawVector3 normal, ref ShawVector3 borderAdjust)
        {
            ShawVector3 disOffset = mPos - collider.mPos;
            // 向量在长方体轴向上的投影
            ShawInt dot_disX = ShawVector3.Dot(disOffset, collider.mDir[0]);
            ShawInt dot_disZ = ShawVector3.Dot(disOffset, collider.mDir[2]);
            // 限制长度
            ShawInt clamp_X = ShawMathLibrary.Clamp(dot_disX, -collider.mSize.x, collider.mSize.x);
            ShawInt clamp_Z = ShawMathLibrary.Clamp(dot_disZ, -collider.mSize.z, collider.mSize.z);

            ShawVector3 s_x = clamp_X * collider.mDir[0];
            ShawVector3 s_z = clamp_Z * collider.mDir[2];

            ShawVector3 point = collider.mPos;
            point += s_x;
            point += s_z;

            ShawVector3 po = mPos - point;
            po.y = 0;

            if (ShawVector3.SqrMagnitude(po) > mRadius * mRadius)
            {
                return false;
            }
            normal = po.normalized;
            ShawInt len = po.magnitude;
            borderAdjust = normal * (mRadius - len);
            return true;
        }

        public override bool SphereCollisionDetect(ShawCylinderCollider collider, ref ShawVector3 normal, ref ShawVector3 borderAdjust)
        {
            ShawVector3 disOffset = mPos - collider.mPos;
            if (ShawVector3.SqrMagnitude(disOffset) > (mRadius + collider.mRadius) * (mRadius + collider.mRadius))
            {
                return false;
            }
            normal = disOffset.normalized;
            borderAdjust = (mRadius + collider.mRadius - disOffset.magnitude) * disOffset;
            return true;
        }

        /// <summary>
        /// 计算碰撞交互的核心算法(一般在FixedUpdate中进行调用)
        /// </summary>
        /// <param name="colliders">环境中的所有碰撞信息(配合GetAllEnvCollider()使用)</param>
        /// <param name="velocity">当前自身的移动速度</param>
        /// <param name="borderAdjust">偏移距离</param>
        public void CalcColliderInteraction(List<ShawColliderBase> colliders, ref ShawVector3 velocity, ref ShawVector3 borderAdjust)
        {
            if (velocity == ShawVector3.zero)
            {
                return;
            }
            List<CollisionInfo> collisionInfoLst = new List<CollisionInfo>();
            ShawVector3 normal = ShawVector3.zero;
            ShawVector3 adj = ShawVector3.zero;
            // 遍历场景中所有碰撞体
            for (int i = 0; i < colliders.Count; i++)
            {
                ShawColliderBase collider = colliders[i];
                if (CollisionDetect(collider, ref normal, ref adj))
                {
                    // 发生碰撞，存储碰撞信息
                    CollisionInfo info = new CollisionInfo
                    {
                        collider = collider,
                        normal = normal,
                        borderAdjust = adj,
                    };
                    collisionInfoLst.Add(info);
                }
            }

            if (collisionInfoLst.Count == 1)
            {
                // 仅有一个碰撞体，修正速度
                CollisionInfo info = collisionInfoLst[0];
                velocity = CorrectVelocity(velocity, info.normal);
                borderAdjust = info.borderAdjust;
                LogCore.ColorLog("发生碰撞，速度矫正:" + velocity.ToString(), ELogColor.Orange);
            }
            else
            {
                // 多种地形共同作用
                ShawVector3 centerNormal = ShawVector3.zero;
                CollisionInfo info = null;
                ShawArgs borderNormalAngle = CalcMaxNormalAngle(collisionInfoLst, velocity, ref centerNormal, ref info);
                ShawArgs angle = ShawVector3.Angle(-velocity, centerNormal);
                if (angle > borderNormalAngle)
                {
                    velocity = CorrectVelocity(velocity, info.normal);
                    LogCore.ColorLog("多个碰撞体，校正速度：" + velocity, ELogColor.Orange);
                    ShawVector3 adjSum = ShawVector3.zero;
                    for (int i = 0; i < collisionInfoLst.Count; i++)
                    {
                        adjSum += collisionInfoLst[i].borderAdjust;
                    }
                    borderAdjust = adjSum;
                }
                else
                {
                    velocity = ShawVector3.zero;
                    LogCore.ColorLog("速度方向反向量在校正法线夹角内，无法移动：" + angle, ELogColor.Orange);
                }

            }

        }

        // 根据法向信息修正传入的速度
        ShawVector3 CorrectVelocity(ShawVector3 velocity, ShawVector3 normal)
        {
            if (normal == ShawVector3.zero)
            {
                return velocity;
            }
            // 角度大于90°，确保速度方向与法线方向钝角
            if (ShawVector3.Angle(normal, velocity) > ShawArgs.HALFPI)
            {
                ShawInt projectLen = ShawVector3.Dot(velocity, normal); // 投影长度
                if (projectLen != 0)
                {
                    velocity -= projectLen * normal;
                }
            }
            return velocity;
        }

        // 多碰撞环境下最大的法向角
        private ShawArgs CalcMaxNormalAngle(List<CollisionInfo> infoList, ShawVector3 velocity, ref ShawVector3 centerNormal, ref CollisionInfo info)
        {
            for (int i = 0; i < infoList.Count; i++)
            {
                centerNormal += infoList[i].normal;
            }
            centerNormal /= infoList.Count;

            ShawArgs normalAngle = ShawArgs.Zero;
            ShawArgs velocityAngle = ShawArgs.Zero;
            for (int i = 0; i < infoList.Count; i++)
            {
                ShawArgs tmpNorAngle = ShawVector3.Angle(centerNormal, infoList[i].normal);
                if (normalAngle < tmpNorAngle)
                {
                    normalAngle = tmpNorAngle;
                }

                //找出速度方向与法线方向夹角最大的碰撞法线，速度校正由这个法线来决定
                ShawArgs tmpVelAngle = ShawVector3.Angle(velocity, infoList[i].normal);
                if (velocityAngle < tmpVelAngle)
                {
                    velocityAngle = tmpVelAngle;
                    info = infoList[i];
                }
            }

            return normalAngle;
        }
    }
}