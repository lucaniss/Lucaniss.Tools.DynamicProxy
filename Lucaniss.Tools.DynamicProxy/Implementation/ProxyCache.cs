using System;
using System.Collections.Concurrent;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    internal class ProxyCache : IProxyCache
    {
        private static ConcurrentDictionary<ProxyCacheKey, Type> _dictionary;


        public ProxyCache()
        {
            if (_dictionary == null)
            {
                _dictionary = new ConcurrentDictionary<ProxyCacheKey, Type>();
            }
        }

        public Type GetProxyType(ProxyCacheKey key)
        {
            Type proxyType;

            _dictionary.TryGetValue(key, out proxyType);
            return proxyType;
        }

        public void AddProxyType(ProxyCacheKey key, Type proxyType)
        {
            _dictionary.TryAdd(key, proxyType);
        }
    }
}