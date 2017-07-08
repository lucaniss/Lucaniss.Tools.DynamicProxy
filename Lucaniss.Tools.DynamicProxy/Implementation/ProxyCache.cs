using System;
using System.Collections.Generic;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    internal class ProxyCache : IProxyCache
    {
        private static IDictionary<ProxyCacheKey, Type> _dictionary;


        public ProxyCache()
        {
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<ProxyCacheKey, Type>();
            }
        }

        public Type GetProxyType(ProxyCacheKey key)
        {
            return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
        }

        public void AddProxyType(ProxyCacheKey key, Type proxyType)
        {
            if (!_dictionary.ContainsKey(key))
            {
                _dictionary.Add(key, proxyType);
            }
        }
    }
}