using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShawnFramework.Singleton
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateSingletonInstance();
                }
                return _instance;
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <returns></returns>
        public static T CreateSingletonInstance()
        {
            if (_instance == null)
            {
                _instance = Activator.CreateInstance<T>();
                if (_instance != null )
                {
                    (_instance as Singleton<T>).Init();
                }
            }
            return _instance;
        }

        /// <summary>
        /// ���õ�������
        /// </summary>
        public static void Reset()
        {
            Release();
            CreateSingletonInstance();
        }
        /// <summary>
        /// �ͷŵ�������
        /// </summary>
        public static void Release()
        {
            if ( _instance != null )
            {
                _instance.Dispose();
                _instance = (T)((object)null);
            }
        }

        protected abstract void Init();

        protected abstract void Dispose();
    }

}