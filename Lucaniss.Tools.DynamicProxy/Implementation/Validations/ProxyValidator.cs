using System;
using System.Linq;
using Lucaniss.Tools.DynamicProxy.Exceptions;
using Lucaniss.Tools.DynamicProxy.Extensions;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors;


namespace Lucaniss.Tools.DynamicProxy.Implementation.Validations
{
    internal static class ProxyValidator
    {
        public static void Validate(Object originalInstance, Object interceptorHandlerInstance)
        {
            CheckIfOriginalInstanceAndInterceptorAreNotNulls(originalInstance, interceptorHandlerInstance);
            CheckIfOriginalInstanceAndInterceptorTypesAreCompatible(originalInstance, interceptorHandlerInstance);
            CheckIfOriginalInstanceMethodsAreVirtual(originalInstance);
        }


        // ReSharper disable once UnusedParameter.Local (assertion method)
        private static void CheckIfOriginalInstanceAndInterceptorAreNotNulls(Object originalInstance, Object interceptorInstance)
        {
            if (originalInstance == null)
            {
                throw ProxyExceptionHelper.OriginalInstanceIsNull();
            }

            if (interceptorInstance == null)
            {
                throw ProxyExceptionHelper.InterceptorInstanceIsNull();
            }
        }

        // ReSharper disable once UnusedParameter.Local (assertion method)
        private static void CheckIfOriginalInstanceAndInterceptorTypesAreCompatible(Object originalInstance, Object interceptorInstance)
        {
            var originalInstanceType = originalInstance.GetType();
            var interceptorInstanceType = interceptorInstance.GetType();

            if (!CheckIfOriginalInstanceTypeAndInterceptorTypeAreValid(originalInstanceType, interceptorInstanceType))
            {
                throw ProxyExceptionHelper.OriginalInstanceAndInterceptorTypesAreNotCompatible();
            }
        }

        // ReSharper disable once UnusedParameter.Local (assertion method)
        private static void CheckIfOriginalInstanceMethodsAreVirtual(Object originalInstance)
        {
            if (originalInstance.GetType().GetMethods().Any(e => !e.IsSpecialCrlMethod() && e.IsPublic && !e.IsVirtual))
            {
                throw ProxyExceptionHelper.OriginalInstancePublicMethodsMustBeVirtual();
            }
        }

        // ReSharper disable once UnusedParameter.Local (assertion method)
        private static Boolean CheckIfOriginalInstanceTypeAndInterceptorTypeAreValid(Type originalInstanceType, Type interceptorHandlerInstanceType)
        {
            var interceptorHandlerInterfaceType = interceptorHandlerInstanceType.GetInterfaces()
                .Single(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof (IProxyInterceptorHandler<>));

            if (interceptorHandlerInterfaceType != null)
            {
                var proxyType = interceptorHandlerInterfaceType.GetGenericArguments()[0];
                if (proxyType.IsAssignableFrom(originalInstanceType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}