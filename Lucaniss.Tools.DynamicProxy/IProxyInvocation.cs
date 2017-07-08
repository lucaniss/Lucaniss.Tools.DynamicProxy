using System;


namespace Lucaniss.Tools.DynamicProxy
{
    public interface IProxyInvocation<out TProxy>
        where TProxy : class
    {
        TProxy OriginalInstance { get; }
        String MethodName { get; }
        String[] ArgumentTypes { get; }
        Object[] ArgumentValues { get; }

        void Invoke();
    }
}