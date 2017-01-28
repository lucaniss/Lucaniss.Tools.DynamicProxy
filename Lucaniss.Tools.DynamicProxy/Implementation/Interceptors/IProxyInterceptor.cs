using System;


namespace Lucaniss.Tools.DynamicProxy.Implementation.Interceptors
{
    public interface IProxyInterceptor
    {
        Object Intercept(IProxyInterceptorHandler interceptorHandler, Object originalInstance, String methodName, String[] argumentTypeNames, Object[] argumentValues);
    }
}