using System;
using Lucaniss.Tools.DynamicProxy.Implementation;


namespace Lucaniss.Tools.DynamicProxy
{
    public static class Proxy
    {
        public static TProxy Create<TClass, TProxy, TInterceptorHandler>(TClass originalInstance, TInterceptorHandler interceptorHandlerInstance)
            where TProxy : class
            where TClass : TProxy
            where TInterceptorHandler : IProxyInterceptorHandler<TProxy>
        {
            return (TProxy) CreateProxy(typeof (TProxy), originalInstance, interceptorHandlerInstance);
        }

        public static TClass Create<TClass, TInterceptorHandler>(TClass originalInstance, TInterceptorHandler interceptorHandlerInstance)
            where TClass : class
            where TInterceptorHandler : IProxyInterceptorHandler<TClass>
        {
            return (TClass) CreateProxy(typeof (TClass), originalInstance, interceptorHandlerInstance);
        }

        public static Object Create(Type proxyType, Object originalInstance, Object interceptorHandlerInstance)
        {
            return CreateProxy(proxyType, originalInstance, interceptorHandlerInstance);
        }

        public static Object Create(Object originalInstance, Object interceptorHandlerInstance)
        {
            return CreateProxy(originalInstance?.GetType(), originalInstance, interceptorHandlerInstance);
        }


        private static Object CreateProxy(Type proxyType, Object originalInstance, Object interceptorHandlerInstance)
        {
            ProxyValidator.Validate(originalInstance, interceptorHandlerInstance);

            var proxyBuilder = new ProxyBuilder(new ProxyCache());
            var proxy = proxyBuilder.Create(proxyType, originalInstance, interceptorHandlerInstance);

            return proxy;
        }
    }
}