using System;


namespace Lucaniss.Tools.DynamicProxy.Implementation.Interceptors
{
    public interface IProxyInvokation
    {
        Object OriginalInstance { get; }
        String MethodName { get; }
        String[] ArgumentTypes { get; }
        Object[] ArgumentValues { get; }

        void Invoke();
    }
}