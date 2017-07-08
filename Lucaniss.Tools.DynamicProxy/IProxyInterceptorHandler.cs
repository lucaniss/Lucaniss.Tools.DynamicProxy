namespace Lucaniss.Tools.DynamicProxy
{
    public interface IProxyInterceptorHandler<in TProxy>
        where TProxy : class
    {
        void Handle(IProxyInvocation<TProxy> invokation);
    }
}