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
    /// 对象池管理模块
    /// </summary>
    public class ObjectPoolManager
    {
        public static ObjectPoolManager Instance = new ObjectPoolManager();

        /// <summary>
        /// 所有在管理中的对象池的 Id
        /// </summary>
        private HashSet<int> m_objectPoolIds = new();

        /// <summary>
        /// 管理所有 Pool 的空闲对象，key 为对象类型的 HashCode，value 为空闲对象池
        /// </summary>
        private Dictionary<int, Stack<IRecyclable>> m_freeObjectPoolMap = new();

        /// <summary>
        /// 管理所有 Pool 的已用对象，key 为对象类型的 HashCode，value 为已用对象池
        /// </summary>
        private Dictionary<int, HashSet<IRecyclable>> m_usedObjectPoolMap = new();

        /// <summary>
        /// 存储所有对象的实例化方法，key 为对象类型的 HashCode，value 为实例化方法
        /// </summary>
        private Dictionary<int, Func<IRecyclable>> m_objectInstantiateFuncMap = new();

        /// <summary>
        /// 存储所有对象的取出方法，key 为对象类型的 HashCode，value 为取出方法
        /// </summary>
        private Dictionary<int, Action<IRecyclable>> m_objectOnSpawnActionMap = new();

        /// <summary>
        /// 存储所有对象的回收方法，key 为对象类型的 HashCode，value 为回收方法
        /// </summary>
        private Dictionary<int, Action<IRecyclable>> m_objectOnDespawnActionMap = new();

        /// <summary>
        /// 初始化一个对象池，并实例化指定个数的元素，如果已有该类型的对象池，则只会实例化指定个数的元素
        /// </summary>
        /// <param name="instantiateFunc">实例化方法</param>
        /// <param name="onSpawnAction">从对象池取出对象时调用的方法</param>
        /// <param name="onDespawnAction">对象被对象池回收时调用的方法</param>
        /// <param name="number">初始化时实例化并进池的元素个数，默认不实例化元素</param>
        public void CreatePool<T>(Func<IRecyclable> instantiateFunc, Action<T> onSpawnAction, Action<T> onDespawnAction, int number = 0) where T : IRecyclable
        {
            if (instantiateFunc == null || onSpawnAction == null || onDespawnAction == null)
            {
                throw new ObjectPoolException($"缺少参数，创建对象池失败！");
            }

            // 使用对象池必须实现IRecyclable接口
            if (typeof(IRecyclable).IsAssignableFrom(typeof(T)))
            {
                int hashCode = typeof(T).GetHashCode();
                if (m_objectPoolIds.Contains(hashCode))
                {
                    Debug.LogWarning($"类型为: {typeof(T).Name} 的对象池已存在，请勿重复创建！");
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
                throw new ObjectPoolException($"传入的类型: {typeof(T).Name} 没有实现 {typeof(IRecyclable).Name} 接口，创建对象池失败！");
            }
        }

        /// <summary>
        /// 从对象池获取指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>对应类型的对象实例</returns>
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
                    Debug.Log($"类型为 {obj.GetType()} 的对象被取出，该类型池内剩余 {m_freeObjectPoolMap[hashCode].Count} 个空闲对象");
                    return obj;
                }
                else
                {
                    // 池内已经没有空闲对象，实例化一个新的对象返回
                    var obj = (T)m_objectInstantiateFuncMap[hashCode].Invoke();
                    m_usedObjectPoolMap[hashCode].Add(obj);
                    Debug.Log($"池内已经没有类型为 {obj.GetType()} 的空闲对象，实例化一个新对象并取出，该类型已使用 {m_usedObjectPoolMap[hashCode].Count} 个对象");
                    return obj;
                }
            }

            throw new ObjectPoolException($"类型为 {typeof(T)} 的对象池不存在，请先创建对象池！");
        }

        /// <summary>
        /// 回收指定对象
        /// </summary>
        /// <param name="obj">要回收的对象</param>
        /// <typeparam name="T"></typeparam>
        public void Despawn<T>(T obj) where T : IRecyclable
        {
            var hashCode = obj.GetType().GetHashCode();
            if (m_objectPoolIds.Contains(hashCode))
            {
                if (!m_usedObjectPoolMap[hashCode].Contains(obj))
                {
                    Debug.LogWarning("回收的对象不在已用对象池中，回收失败！");
                    return;
                }
                m_objectOnDespawnActionMap[hashCode].Invoke(obj);
                m_freeObjectPoolMap[hashCode].Push(obj);
                m_usedObjectPoolMap[hashCode].Remove(obj);
                Debug.Log($"类型为 {obj.GetType()} 的对象被回收，该类型池内剩余 {m_freeObjectPoolMap[hashCode].Count} 个空闲对象");
            }
            else
            {
                throw new ObjectPoolException($"类型为 {typeof(T)} 的对象池不存在，请先创建对象池！");
            }
        }

        /// <summary>
        /// 回收指定类型的所有对象
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
                throw new ObjectPoolException($"类型为 {typeof(T)} 的对象池不存在，请先创建对象池！");
            }
        }

        /// <summary>
        /// 删除指定类型的对象池
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
        /// 通过对象池 Id 删除对象池
        /// </summary>
        /// <param name="hashCode">对象池的 Id</param>
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
        /// 删除所有对象池
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