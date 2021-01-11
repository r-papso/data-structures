using System;
using System.Collections.Generic;

namespace Structures.Helper
{
    /// <summary>
    /// Serves to store object prototypes
    /// </summary>
    public class PrototypeManager
    {
        private static object _lock = new object();
        private static volatile PrototypeManager _instance;

        private Dictionary<string, object> _prototypes = new Dictionary<string, object>();

        protected PrototypeManager() { }

        /// <summary>
        /// Instance of the manager
        /// </summary>
        public static PrototypeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PrototypeManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Registers new prototype into manager
        /// </summary>
        /// <typeparam name="T">Type of prototype</typeparam>
        /// <param name="key">Key used to object retrieval</param>
        /// <param name="prototype">Prototype</param>
        public void Register<T>(string key, T prototype)
        {
            if (_prototypes.ContainsKey(key))
                throw new ArgumentException($"Prototype container already contains prototype with key {key}");

            _prototypes.Add(key, prototype);
        }

        /// <summary>
        /// Tries to register new prototype into manager without throwing an exception
        /// when manager already contains prototype with specified key
        /// </summary>
        /// <typeparam name="T">Type of prototype</typeparam>
        /// <param name="key">Key used to object retrieval</param>
        /// <param name="prototype">Prototype</param>
        /// <returns>True, if registration was successful, false otherwise</returns>
        public bool TryRegister<T>(string key, T prototype)
        {
            try
            {
                Register(key, prototype);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets prototype by key used in registration
        /// </summary>
        /// <typeparam name="T">Type of prototype</typeparam>
        /// <param name="key">Key used in prototype registration</param>
        /// <returns>Prototype</returns>
        public T Get<T>(string key)
        {
            return (T)_prototypes[key];
        }

        /// <summary>
        /// Tries to get prototype by key without throwing an exception when manager does not contain prototype with specified key
        /// </summary>
        /// <typeparam name="T">Type of prototype</typeparam>
        /// <param name="key">Key used in prototype registration</param>
        /// <returns>Prototype, if <paramref name="key"/> is present in manager, default value of <typeparamref name="T"/> otherwise</returns>
        public T TryGet<T>(string key)
        {
            try
            {
                return Get<T>(key);
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// Unregister registered prototype
        /// </summary>
        /// <typeparam name="T">Type of prototype</typeparam>
        /// <param name="key">Key used in prototype registration</param>
        /// <returns>Unregistered prototype</returns>
        public T Unregister<T>(string key)
        {
            var result = (T)_prototypes[key];
            _prototypes.Remove(key);
            return result;
        }

        /// <summary>
        /// Tries to unregister registered prototype without throwing an exception when manager does not contain prototype with specified key
        /// </summary>
        /// <typeparam name="T">Type of prototype</typeparam>
        /// <param name="key">Key used in prototype registration</param>
        /// <returns>Unregistered prototype, if <paramref name="key"/> is present in manager, default value of <typeparamref name="T"/> otherwise</returns>
        public T TryUnregister<T>(string key)
        {
            try
            {
                return Unregister<T>(key);
            }
            catch (System.Exception)
            {
                return default;
            }
        }
    }
}
