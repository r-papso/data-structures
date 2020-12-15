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
        public void RegisterPrototype<T>(string key, T prototype)
        {
            if (_prototypes.ContainsKey(key))
                throw new ArgumentException($"Prototype container already contains prototype with key {key}");

            _prototypes.Add(key, prototype);
        }

        /// <summary>
        /// Gets prototype by key used in registration
        /// </summary>
        /// <typeparam name="T">Type of prototype</typeparam>
        /// <param name="key">Key used in prototype registration</param>
        /// <returns>Prototype</returns>
        public T GetPrototype<T>(string key)
        {
            return (T)_prototypes[key];
        }
    }
}
