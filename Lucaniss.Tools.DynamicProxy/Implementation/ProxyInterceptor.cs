using System;
using System.Reflection;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    public class ProxyInterceptor : IProxyInterceptor
    {
        public static MethodInfo GetInterceptorMethodInfo()
        {
            ProxyInterceptor instance;
            return typeof (ProxyInterceptor).GetMethod(nameof(instance.Intercept));
        }

        public Object Intercept(IProxyInterceptorHandler interceptorHandler, Object originalInstance, String methodName, String[] argumentTypeNames, Object[] argumentValues)
        {
            var proxyInvokation = new ProxyInvocation(originalInstance, methodName, argumentTypeNames, argumentValues);
            interceptorHandler.Handle(proxyInvokation);

            return proxyInvokation.GetReturnValue();
        }
    }
}