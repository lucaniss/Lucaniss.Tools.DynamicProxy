namespace Lucaniss.Tools.DynamicProxy.Implementation.Interceptors
{
    public interface IProxyInterceptorHandler
    {
        void Handle(IProxyInvokation invokation);
    }

    public interface IProxyInterceptorHandler<TClass> : IProxyInterceptorHandler
        where TClass : class
    {
    }
}