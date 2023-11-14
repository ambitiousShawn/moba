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
            // �����ڳ����������ϵ�ͶӰ
            ShawInt dot_disX = ShawVector3.Dot(disOffset, collider.mDir[0]);
            ShawInt dot_disZ = ShawVector3.Dot(disOffset, collider.mDir[2]);
            // ���Ƴ���
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
        /// ������ײ�����ĺ����㷨(һ����FixedUpdate�н��е���)
        /// </summary>
        /// <param name="colliders">�����е�������ײ��Ϣ(���GetAllEnvCollider()ʹ��)</param>
        /// <param name="velocity">��ǰ������ƶ��ٶ�</param>
        /// <param name="borderAdjust">ƫ�ƾ���</param>
        public void CalcColliderInteraction(List<ShawColliderBase> colliders, ref ShawVector3 velocity, ref ShawVector3 borderAdjust)
        {
            if (velocity == ShawVector3.zero)
            {
                return;
            }
            List<CollisionInfo> collisionInfoLst = new List<CollisionInfo>();
            ShawVector3 normal = ShawVector3.zero;
            ShawVector3 adj = ShawVector3.zero;
            // ����������������ײ��
            for (int i = 0; i < colliders.Count; i++)
            {
                ShawColliderBase collider = colliders[i];
                if (CollisionDetect(collider, ref normal, ref adj))
                {
                    // ������ײ���洢��ײ��Ϣ
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
                // ����һ����ײ�壬�����ٶ�
                CollisionInfo info = collisionInfoLst[0];
                velocity = CorrectVelocity(velocity, info.normal);
                borderAdjust = info.borderAdjust;
                LogCore.ColorLog("������ײ���ٶȽ���:" + velocity.ToString(), ELogColor.Orange);
            }
            else
            {
                // ���ֵ��ι�ͬ����
                ShawVector3 centerNormal = ShawVector3.zero;
                CollisionInfo info = null;
                ShawArgs borderNormalAngle = CalcMaxNormalAngle(collisionInfoLst, velocity, ref centerNormal, ref info);
                ShawArgs angle = ShawVector3.Angle(-velocity, centerNormal);
                if (angle > borderNormalAngle)
                {
                    velocity = CorrectVelocity(velocity, info.normal);
                    LogCore.ColorLog("�����ײ�壬У���ٶȣ�" + velocity, ELogColor.Orange);
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
                    LogCore.ColorLog("�ٶȷ���������У�����߼н��ڣ��޷��ƶ���" + angle, ELogColor.Orange);
                }

            }

        }

        // ���ݷ�����Ϣ����������ٶ�
        ShawVector3 CorrectVelocity(ShawVector3 velocity, ShawVector3 normal)
        {
            if (normal == ShawVector3.zero)
            {
                return velocity;
            }
            // �Ƕȴ���90�㣬ȷ���ٶȷ����뷨�߷���۽�
            if (ShawVector3.Angle(normal, velocity) > ShawArgs.HALFPI)
            {
                ShawInt projectLen = ShawVector3.Dot(velocity, normal); // ͶӰ����
                if (projectLen != 0)
                {
                    velocity -= projectLen * normal;
                }
            }
            return velocity;
        }

        // ����ײ���������ķ����
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

                //�ҳ��ٶȷ����뷨�߷���н�������ײ���ߣ��ٶ�У�����������������
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