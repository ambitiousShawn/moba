using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShawnFramework.ShawUtil
{
    public interface IRecyclable
    {
        void OnDispose();
    }

    public class ObjectPoolException : SystemException
    {
        public ObjectPoolException(string message)
        {
        }
    }

    /// <summary>
    /// ����ع���ģ��
    /// </summary>
    public class ObjectPoolManager
    {
        public static ObjectPoolManager Instance = new ObjectPoolManager();

        /// <summary>
        /// �����ڹ����еĶ���ص� Id
        /// </summary>
        private HashSet<int> m_objectPoolIds = new();

        /// <summary>
        /// �������� Pool �Ŀ��ж���key Ϊ�������͵� HashCode��value Ϊ���ж����
        /// </summary>
        private Dictionary<int, Stack<IRecyclable>> m_freeObjectPoolMap = new();

        /// <summary>
        /// �������� Pool �����ö���key Ϊ�������͵� HashCode��value Ϊ���ö����
        /// </summary>
        private Dictionary<int, HashSet<IRecyclable>> m_usedObjectPoolMap = new();

        /// <summary>
        /// �洢���ж����ʵ����������key Ϊ�������͵� HashCode��value Ϊʵ��������
        /// </summary>
        private Dictionary<int, Func<IRecyclable>> m_objectInstantiateFuncMap = new();

        /// <summary>
        /// �洢���ж����ȡ��������key Ϊ�������͵� HashCode��value Ϊȡ������
        /// </summary>
        private Dictionary<int, Action<IRecyclable>> m_objectOnSpawnActionMap = new();

        /// <summary>
        /// �洢���ж���Ļ��շ�����key Ϊ�������͵� HashCode��value Ϊ���շ���
        /// </summary>
        private Dictionary<int, Action<IRecyclable>> m_objectOnDespawnActionMap = new();

        /// <summary>
        /// ��ʼ��һ������أ���ʵ����ָ��������Ԫ�أ�������и����͵Ķ���أ���ֻ��ʵ����ָ��������Ԫ��
        /// </summary>
        /// <param name="instantiateFunc">ʵ��������</param>
        /// <param name="onSpawnAction">�Ӷ����ȡ������ʱ���õķ���</param>
        /// <param name="onDespawnAction">���󱻶���ػ���ʱ���õķ���</param>
        /// <param name="number">��ʼ��ʱʵ���������ص�Ԫ�ظ�����Ĭ�ϲ�ʵ����Ԫ��</param>
        public void CreatePool<T>(Func<IRecyclable> instantiateFunc, Action<T> onSpawnAction, Action<T> onDespawnAction, int number = 0) where T : IRecyclable
        {
            if (instantiateFunc == null || onSpawnAction == null || onDespawnAction == null)
            {
                throw new ObjectPoolException($"ȱ�ٲ��������������ʧ�ܣ�");
            }

            // ʹ�ö���ر���ʵ��IRecyclable�ӿ�
            if (typeof(IRecyclable).IsAssignableFrom(typeof(T)))
            {
                int hashCode = typeof(T).GetHashCode();
                if (m_objectPoolIds.Contains(hashCode))
                {
                    Debug.LogWarning($"����Ϊ: {typeof(T).Name} �Ķ�����Ѵ��ڣ������ظ�������");
                    return;
                }
                m_objectPoolIds.Add(hashCode);
                m_freeObjectPoolMap.Add(hashCode, new Stack<IRecyclable>());
                m_usedObjectPoolMap.Add(hashCode, new HashSet<IRecyclable>());
                m_objectInstantiateFuncMap.Add(hashCode, instantiateFunc);
                m_objectOnSpawnActionMap.Add(hashCode, obj => onSpawnAction((T)obj));
                m_objectOnDespawnActionMap.Add(hashCode, obj => onDespawnAction((T)obj));
                for (int i = 0; i < number; i++)
                {
                    var obj = instantiateFunc.Invoke();
                    m_objectOnDespawnActionMap[hashCode].Invoke(obj);
                    Despawn(obj);
                }
            }
            else
            {
                throw new ObjectPoolException($"���������: {typeof(T).Name} û��ʵ�� {typeof(IRecyclable).Name} �ӿڣ����������ʧ�ܣ�");
            }
        }

        /// <summary>
        /// �Ӷ���ػ�ȡָ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>��Ӧ���͵Ķ���ʵ��</returns>
        public T Spawn<T>() where T : IRecyclable
        {
            var hashCode = typeof(T).GetHashCode();
            if (m_objectPoolIds.Contains(hashCode))
            {
                if (m_freeObjectPoolMap[hashCode].Count > 0)
                {
                    var obj = (T)m_freeObjectPoolMap[hashCode].Pop();
                    m_objectOnSpawnActionMap[hashCode].Invoke(obj);
                    m_usedObjectPoolMap[hashCode].Add(obj);
                    Debug.Log($"����Ϊ {obj.GetType()} �Ķ���ȡ���������ͳ���ʣ�� {m_freeObjectPoolMap[hashCode].Count} �����ж���");
                    return obj;
                }
                else
                {
                    // �����Ѿ�û�п��ж���ʵ����һ���µĶ��󷵻�
                    var obj = (T)m_objectInstantiateFuncMap[hashCode].Invoke();
                    m_usedObjectPoolMap[hashCode].Add(obj);
                    Debug.Log($"�����Ѿ�û������Ϊ {obj.GetType()} �Ŀ��ж���ʵ����һ���¶���ȡ������������ʹ�� {m_usedObjectPoolMap[hashCode].Count} ������");
                    return obj;
                }
            }

            throw new ObjectPoolException($"����Ϊ {typeof(T)} �Ķ���ز����ڣ����ȴ�������أ�");
        }

        /// <summary>
        /// ����ָ������
        /// </summary>
        /// <param name="obj">Ҫ���յĶ���</param>
        /// <typeparam name="T"></typeparam>
        public void Despawn<T>(T obj) where T : IRecyclable
        {
            var hashCode = obj.GetType().GetHashCode();
            if (m_objectPoolIds.Contains(hashCode))
            {
                if (!m_usedObjectPoolMap[hashCode].Contains(obj))
                {
                    Debug.LogWarning("���յĶ��������ö�����У�����ʧ�ܣ�");
                    return;
                }
                m_objectOnDespawnActionMap[hashCode].Invoke(obj);
                m_freeObjectPoolMap[hashCode].Push(obj);
                m_usedObjectPoolMap[hashCode].Remove(obj);
                Debug.Log($"����Ϊ {obj.GetType()} �Ķ��󱻻��գ������ͳ���ʣ�� {m_freeObjectPoolMap[hashCode].Count} �����ж���");
            }
            else
            {
                throw new ObjectPoolException($"����Ϊ {typeof(T)} �Ķ���ز����ڣ����ȴ�������أ�");
            }
        }

        /// <summary>
        /// ����ָ�����͵����ж���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DespawnAll<T>() where T : IRecyclable
        {
            var hashCode = typeof(T).GetHashCode();
            if (m_objectPoolIds.Contains(hashCode))
            {
                var itemList = m_usedObjectPoolMap[hashCode].ToList();
                var action = m_objectOnDespawnActionMap[hashCode];
                foreach (var item in itemList)
                {
                    action.Invoke(item);
                }
            }
            else
            {
                throw new ObjectPoolException($"����Ϊ {typeof(T)} �Ķ���ز����ڣ����ȴ�������أ�");
            }
        }

        /// <summary>
        /// ɾ��ָ�����͵Ķ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool DisposePool<T>() where T : IRecyclable
        {
            var hashCode = typeof(T).GetHashCode();
            if (m_objectPoolIds.Contains(hashCode))
            {
                var list = m_usedObjectPoolMap[hashCode].ToList();
                list.AddRange(m_freeObjectPoolMap[hashCode].ToList());
                foreach (var obj in list)
                {
                    obj.OnDispose();
                }
                m_objectPoolIds.Remove(hashCode);
                m_freeObjectPoolMap.Remove(hashCode);
                m_usedObjectPoolMap.Remove(hashCode);
                m_objectOnSpawnActionMap.Remove(hashCode);
                m_objectOnDespawnActionMap.Remove(hashCode);
                return true;
            }

            return false;
        }

        /// <summary>
        /// ͨ������� Id ɾ�������
        /// </summary>
        /// <param name="hashCode">����ص� Id</param>
        /// <returns></returns>
        public bool DisposePool(int hashCode)
        {
            if (m_objectPoolIds.Contains(hashCode))
            {
                var list = m_usedObjectPoolMap[hashCode].ToList();
                list.AddRange(m_freeObjectPoolMap[hashCode].ToList());
                foreach (var obj in list)
                {
                    obj.OnDispose();
                }
                m_objectPoolIds.Remove(hashCode);
                m_freeObjectPoolMap.Remove(hashCode);
                m_usedObjectPoolMap.Remove(hashCode);
                m_objectOnSpawnActionMap.Remove(hashCode);
                m_objectOnDespawnActionMap.Remove(hashCode);
                return true;
            }

            return false;
        }

        /// <summary>
        /// ɾ�����ж����
        /// </summary>
        /// <returns></returns>
        public void DisposeAll()
        {
            foreach (var hashCode in m_objectPoolIds)
            {
                DisposePool(hashCode);
            }
        }
    }

}