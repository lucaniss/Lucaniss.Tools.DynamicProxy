using System;
using Lucaniss.Tools.DynamicProxy.Implementation;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors;
using Lucaniss.Tools.DynamicProxy.Implementation.Validations;


namespace Lucaniss.Tools.DynamicProxy
{
    public static class ProxyManager
    {
        public static TProxy CreateProxy<TClass, TProxy, TInterceptorHandler>(TClass originalInstance, TInterceptorHandler interceptorHandlerInstance)
            where TProxy : class
            where TClass : TProxy
            where TInterceptorHandler : IProxyInterceptorHandler<TProxy>
        {
            return (TProxy) CreateProxyInternal(typeof (TProxy), originalInstance, interceptorHandlerInstance);
        }

        public static TClass CreateProxy<TClass, TInterceptor>(TClass originalInstance, TInterceptor interceptorHandlerInstance)
            where TClass : class
            where TInterceptor : IProxyInterceptorHandler<TClass>
        {
            return (TClass) CreateProxyInternal(typeof (TClass), originalInstance, interceptorHandlerInstance);
        }

        public static Object CreateProxy(Type proxyType, Object originalInstance, Object interceptorHandlerInstance)
        {
            return CreateProxyInternal(proxyType, originalInstance, interceptorHandlerInstance);
        }

        public static Object CreateProxy(Object originalInstance, Object interceptorHandlerInstance)
        {
            return CreateProxyInternal(originalInstance?.GetType(), originalInstance, interceptorHandlerInstance);
        }


        private static Object CreateProxyInternal(Type proxyType, Object originalInstance, Object interceptorHandlerInstance)
        {
            ProxyValidator.Validate(originalInstance, interceptorHandlerInstance);

            var proxyBuilder = new ProxyBuilder();
            var proxy = proxyBuilder.Create(proxyType, originalInstance, interceptorHandlerInstance);

            return proxy;
        }
    }
}