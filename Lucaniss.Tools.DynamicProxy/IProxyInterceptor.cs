using System;


// ReSharper disable UnusedMethodReturnValue.Global (£F: Use by MSIL)
// ReSharper disable UnusedMemberInSuper.Global (£F: Use by MSIL)

namespace Lucaniss.Tools.DynamicProxy
{
    public interface IProxyInterceptor
    {
        Object Intercept(IProxyInterceptorHandler interceptorHandler, Object originalInstance, String methodName, String[] argumentTypeNames, Object[] argumentValues);
    }
}