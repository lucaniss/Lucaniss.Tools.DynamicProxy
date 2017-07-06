using System;
using Lucaniss.Tools.DynamicProxy.Implementation;


namespace Lucaniss.Tools.DynamicProxy
{
    internal interface IProxyCache
    {
        Type GetProxyType(ProxyCacheKey key);
        void AddProxyType(ProxyCacheKey key, Type proxyType);
    }
}