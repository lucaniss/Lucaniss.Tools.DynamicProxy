using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lucaniss.Tools.DynamicProxy.Exceptions;
using Lucaniss.Tools.DynamicProxy.Extensions;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    internal class ProxyInvocation : IProxyInvocation
    {
        public Object OriginalInstance { get; }
        public String MethodName { get; }
        public String[] ArgumentTypes { get; }
        public Object[] ArgumentValues { get; }

        private Boolean _isInvoked;
        private Object _returnValue;


        public ProxyInvocation(Object originalInstance, String methodName, String[] argumentTypes, Object[] argumentValues)
        {
            OriginalInstance = originalInstance;
            MethodName = methodName;
            ArgumentTypes = argumentTypes;
            ArgumentValues = argumentValues;

            _isInvoked = false;
        }

        public void Invoke()
        {
            if (!_isInvoked)
            {
                var methodInfo = OriginalInstance.GetType().GetMethodInfosForProxy()
                    .SingleOrDefault(m => m.Name == MethodName && CheckIfParametersMatch(m, ArgumentTypes, ArgumentValues));

                if (methodInfo != null)
                {
                    _returnValue = methodInfo.Invoke(OriginalInstance, ArgumentValues);
                    _isInvoked = true;
                }
                else
                {
                    throw ProxyExceptionHelper.ProxyMethodNotFound(MethodName);
                }
            }
            else
            {
                throw ProxyExceptionHelper.ProxyMethodWasInvoked(MethodName);
            }
        }

        public Object GetReturnValue()
        {
            if (_isInvoked)
            {
                return _returnValue;
            }

            throw ProxyExceptionHelper.ProxyMethodNotInvoked(MethodName);
        }


        private static Boolean CheckIfParametersMatch(MethodInfo methodInfo, IReadOnlyList<String> argumentTypes, IReadOnlyList<Object> argumentValues)
        {
            var parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != argumentTypes.Count)
            {
                return false;
            }

            for (var i = 0; i < argumentTypes.Count; i++)
            {
                if (!parameterInfos[i].SafeGetType().IsAssignableFrom(argumentValues[i]?.SafeGetType()))
                {
                    if (parameterInfos[i].SafeGetTypeName() != argumentTypes[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}