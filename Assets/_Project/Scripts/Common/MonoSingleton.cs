using System;
using UnityEngine;

namespace Sponsord
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        #region Singleton
        private static T _instance;
        private static object _lock = new object();
        private static bool _isShuttingDown;
        private bool _initializedFromAwake;

        public static T Instance
        {
            get
            {
                if (_isShuttingDown)
                {
                    //return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        InitInstance();
                    }

                    return _instance;
                }
            }
        }
        #endregion

        private void Awake()
        {
            if (_initializedFromAwake)
            {
                Debug.LogError($"There is more than one objects of singletone type '{typeof(T).Name} on the scene'");
                enabled = false;
                return;
            }

            _initializedFromAwake = true;
            InitInstance(this as T);
        }

        private void OnDestroy()
        {
            _instance = null;
            _initializedFromAwake = false;
            _isShuttingDown = true;

            OnDestroySingleton();
        }

        private void OnApplicationQuit()
        {
            _isShuttingDown = true;
        }

        private static void InitInstance(T singleton = null)
        {
            if (singleton != null)
            {
                _instance = singleton;
            }
            else
            {
                Type thisType = typeof(T);
                _instance = FindObjectOfType<T>() ?? new GameObject(thisType.Name).AddComponent<T>();
            }

            _instance.InitSingleton();
        }

        protected virtual void InitSingleton() { }

        protected virtual void OnDestroySingleton() { }
    }
}

////Usage example

////    public class GameUIManager : AutoSingletonManager<GameUIManager>
////{
////    //Initialization code (called from Awake) goes in here
////    public override void InitManager() { }
////}