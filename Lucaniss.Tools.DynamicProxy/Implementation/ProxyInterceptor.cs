using System;
using System.Reflection;


// ReSharper disable UnusedMethodReturnValue.Global (ŁF: Use by MSIL)

namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    public abstract class ProxyInterceptor
    {
        public static MethodInfo GetInterceptorMethodInfo()
        {
            ProxyInterceptor instance;
            return typeof (ProxyInterceptor).GetMethod(nameof(instance.Intercept));
        }

        public abstract Object Intercept(Object originalInstance, String methodName, String[] argumentTypeNames, Object[] argumentValues);
    }

    public class ProxyInterceptor<TProxy> : ProxyInterceptor, IProxyInterceptor<TProxy>
        where TProxy : class
    {
        private readonly IProxyInterceptorHandler<TProxy> _interceptorHandler;


        public ProxyInterceptor(IProxyInterceptorHandler<TProxy> interceptorHandler)
        {
            _interceptorHandler = interceptorHandler;
        }


        public override Object Intercept(Object originalInstance, String methodName, String[] argumentTypeNames, Object[] argumentValues)
        {
            return InterceptInternal((TProxy) originalInstance, methodName, argumentTypeNames, argumentValues);
        }

        public Object InterceptInternal(TProxy originalInstance, String methodName, String[] argumentTypeNames, Object[] argumentValues)
        {
            var proxyInvokation = new ProxyInvocation<TProxy>(originalInstance, methodName, argumentTypeNames, argumentValues);
            _interceptorHandler.Handle(proxyInvokation);

            return proxyInvokation.GetReturnValue();
        }
    }
}