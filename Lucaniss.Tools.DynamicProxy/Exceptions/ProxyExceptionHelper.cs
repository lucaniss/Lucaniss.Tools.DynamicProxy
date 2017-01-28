using System;
using Lucaniss.Tools.DynamicProxy.Resources;


namespace Lucaniss.Tools.DynamicProxy.Exceptions
{
    internal static class ProxyExceptionHelper
    {
        public static Exception InterceptorInstanceIsNull()
        {
            var message = String.Format(ExceptionResources.InterceptorInstanceIsNull);
            return new ProxyException(ProxyExceptionErrors.InterceptorInstanceIsNull, message);
        }

        public static Exception OriginalInstanceAndInterceptorTypesAreNotCompatible()
        {
            var message = String.Format(ExceptionResources.OriginalInstanceAndInterceptorTypesAreNotCompatible);
            return new ProxyException(ProxyExceptionErrors.OriginalInstanceAndInterceptorTypesAreNotCompatible, message);
        }

        public static Exception OriginalInstanceIsNull()
        {
            var message = String.Format(ExceptionResources.OriginalInstanceIsNull);
            return new ProxyException(ProxyExceptionErrors.OriginalInstanceIsNull, message);
        }

        public static Exception OriginalInstancePublicMethodsMustBeVirtual()
        {
            var message = String.Format(ExceptionResources.OriginalInstancePublicMethodsMustBeVirtual);
            return new ProxyException(ProxyExceptionErrors.OriginalInstancePublicMethodsMustBeVirtual, message);
        }

        public static Exception ProxyMethodNotFound(String methodName)
        {
            var message = String.Format(ExceptionResources.ProxyMethodNotFound, methodName);
            return new ProxyException(ProxyExceptionErrors.ProxyMethodNotFound, message);
        }

        public static Exception ProxyMethodNotInvoked(String methodName)
        {
            var message = String.Format(ExceptionResources.ProxyMethodNotInvoked, methodName);
            return new ProxyException(ProxyExceptionErrors.ProxyMethodNotInvoked, message);
        }

        public static Exception ProxyMethodWasInvoked(String methodName)
        {
            var message = String.Format(ExceptionResources.ProxyMethodWasInvoked, methodName);
            return new ProxyException(ProxyExceptionErrors.ProxyMethodWasInvoked, message);
        }
    }
}