namespace Lucaniss.Tools.DynamicProxy
{
    public interface IProxyInterceptorHandler
    {
        void Handle(IProxyInvocation invokation);
    }

    public interface IProxyInterceptorHandler<TClass> : IProxyInterceptorHandler
        where TClass : class
    {
    }
}