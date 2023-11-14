using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShawnFramework.ShawnPhysics
{
    /// <summary>
    /// ʹ�÷�ʽ��
    ///     ʹ��ǰ���������[����]�е���ײ��Ϣ�����������
    ///     ��ʼ��ʱ����Init()����
    ///     �����FixedUpdate�µ��� CalcColliderInteraction() ������ ��ײ��� + �ٶȽ���
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
        /// �õ����������е���ײ����Ϣ
        /// </summary>
        /// <returns></returns>
        public List<ShawColliderBase> GetAllEnvColliders() 
        {
            return envColliderLst;
        }
    }

}