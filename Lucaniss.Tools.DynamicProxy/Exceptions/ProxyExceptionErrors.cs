namespace Lucaniss.Tools.DynamicProxy.Exceptions
{
    public enum ProxyExceptionErrors
    {
        InterceptorInstanceIsNull,
        OriginalInstanceAndInterceptorTypesAreNotCompatible,
        OriginalInstanceIsNull,
        OriginalInstancePublicMethodsMustBeVirtual,
        ProxyMethodNotFound,
        ProxyMethodNotInvoked,
        ProxyMethodWasInvoked
    }
}