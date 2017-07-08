using System;


// ReSharper disable UnusedMemberInSuper.Global (£F: Use by MSIL)

namespace Lucaniss.Tools.DynamicProxy
{
    internal interface IProxyInterceptor<in TProxy>
        where TProxy : class
    {
        Object InterceptInternal(TProxy originalInstance, String methodName, String[] argumentTypeNames, Object[] argumentValues);
    }
}