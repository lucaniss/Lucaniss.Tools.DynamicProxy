using System;
using Lucaniss.Tools.DynamicProxy.Implementation;


namespace Lucaniss.Tools.DynamicProxy
{
    public static class Proxy
    {
        public static TProxy Create<TClass, TProxy, TInterceptorHandler>(TClass originalInstance, TInterceptorHandler interceptorHandlerInstance)
            where TClass : TProxy
            where TProxy : class
            where TInterceptorHandler : IProxyInterceptorHandler<TProxy>
        {
            return (TProxy) CreateProxy(originalInstance, interceptorHandlerInstance, typeof (TProxy));
        }

        public static TClass Create<TClass, TInterceptorHandler>(TClass originalInstance, TInterceptorHandler interceptorHandlerInstance)
            where TClass : class
            where TInterceptorHandler : IProxyInterceptorHandler<TClass>
        {
            return (TClass) CreateProxy(originalInstance, interceptorHandlerInstance, typeof (TClass));
        }

        public static Object Create(Object originalInstance, Object interceptorHandlerInstance, Type proxyType)
        {
            return CreateProxy(originalInstance, interceptorHandlerInstance, proxyType);
        }

        public static Object Create(Object originalInstance, Object interceptorHandlerInstance)
        {
            return CreateProxy(originalInstance, interceptorHandlerInstance, originalInstance?.GetType());
        }


        private static Object CreateProxy(Object originalInstance, Object interceptorHandlerInstance, Type proxyType)
        {
            ProxyValidator.Validate(originalInstance, interceptorHandlerInstance);

            var proxyBuilder = new ProxyBuilder(new ProxyCache());
            var proxy = proxyBuilder.Create(originalInstance, interceptorHandlerInstance, proxyType);

            return proxy;
        }
    }
}