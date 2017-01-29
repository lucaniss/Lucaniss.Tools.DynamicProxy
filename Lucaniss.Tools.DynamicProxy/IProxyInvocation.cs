using System;


namespace Lucaniss.Tools.DynamicProxy
{
    public interface IProxyInvocation
    {
        Object OriginalInstance { get; }
        String MethodName { get; }
        String[] ArgumentTypes { get; }
        Object[] ArgumentValues { get; }

        void Invoke();
    }
}