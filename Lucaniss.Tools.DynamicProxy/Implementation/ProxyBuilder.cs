using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Lucaniss.Tools.DynamicProxy.Consts;
using Lucaniss.Tools.DynamicProxy.Extensions;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    internal class ProxyBuilder
    {
        private static readonly Object CriticalSection = new Object();

        private readonly IProxyCache _proxyCache;

        private Object _originalInstance;

        private Object _proxyInterceptorInstance;
        private Type _proxyInterceptorType;

        private Type _proxyInterceptorHandlerType;

        private TypeBuilder _typeBuilder;

        private Type _proxyBaseType;


        public ProxyBuilder(IProxyCache proxyCache)
        {
            _proxyCache = proxyCache;
        }


        public Object Create(Object originalInstance, Object interceptorHandlerIntance, Type proxyType)
        {
            _originalInstance = originalInstance;

            _proxyInterceptorType = typeof (ProxyInterceptor<>).MakeGenericType(proxyType);
            _proxyInterceptorInstance = Activator.CreateInstance(_proxyInterceptorType, interceptorHandlerIntance);

            _proxyInterceptorHandlerType = interceptorHandlerIntance.GetType();

            _proxyBaseType = proxyType;

            // INFO: Why critical section? Because this code may be executed in multi-thread environment.
            //       So we need to be sure that for the same base types (instance and interceptor handler) we get exactly the same referance to proxy type.
            lock (CriticalSection)
            {
                var type = CreateProxyType();
                return Activator.CreateInstance(type, _originalInstance, _proxyInterceptorInstance);
            }
        }


        private Type CreateProxyType()
        {
            var proxyCacheKey = new ProxyCacheKey(_proxyBaseType, _proxyInterceptorType, _proxyInterceptorHandlerType);

            var proxyType = _proxyCache.GetProxyType(proxyCacheKey);
            if (proxyType == null)
            {
                CreateClass();

                var classVariables = new MSILCodeVariables
                {
                    OriginalInstanceFieldInfo = CreateOriginalInstanceField(),
                    InterceptorInstanceFieldInfo = CreateInterceptorInstanceField()
                };

                CreateConstructor(classVariables);
                CreateMethods(classVariables);

                proxyType = _typeBuilder.CreateType();
                _proxyCache.AddProxyType(proxyCacheKey, proxyType);
            }

            return proxyType;
        }


        private FieldInfo CreateOriginalInstanceField()
        {
            var fieldBuilder = _typeBuilder.DefineField(ProxyConsts.OriginalInstanceFieldName, _proxyBaseType, FieldAttributes.Private);
            return fieldBuilder;
        }

        private FieldInfo CreateInterceptorInstanceField()
        {
            var fieldBuilder = _typeBuilder.DefineField(ProxyConsts.InterceptorInstanceFieldName, _proxyInterceptorType, FieldAttributes.Private);
            return fieldBuilder;
        }


        private void CreateClass()
        {
            var typeName = String.Format(ProxyConsts.TypeName, _proxyBaseType.FullName);

            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(ProxyConsts.AssemblyName), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(ProxyConsts.ModuleName);

            _typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, _proxyBaseType);
        }

        private void CreateConstructor(MSILCodeVariables variables)
        {
            var defaultConstructor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { _proxyBaseType, _proxyInterceptorType });
            var msil = defaultConstructor.GetILGenerator();

            MSILCodeGenerator.CreateConstructor(msil, variables);
        }

        private void CreateMethods(MSILCodeVariables variables)
        {
            foreach (var methodInfo in _proxyBaseType.GetMethodInfosForProxy())
            {
                CreateMethod(methodInfo, variables);
            }
        }

        private void CreateMethod(MethodInfo methodInfo, MSILCodeVariables variables)
        {
            var methodBuilder = _typeBuilder.DefineMethod(
                methodInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                methodInfo.CallingConvention,
                methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            CreateGenericParameters(methodInfo, methodBuilder);
            CreateMethodBody(methodInfo, methodBuilder, variables);
        }

        private static void CreateGenericParameters(MethodInfo methodInfo, MethodBuilder methodBuilder)
        {
            var genericParameters = methodInfo.GetGenericArguments().Select(p => p.Name).ToArray();
            if (genericParameters.Any())
            {
                // INFO: Definicja parametrów generycznych metody (na podstawie sygnatury metody interfejsu).
                methodBuilder.DefineGenericParameters(genericParameters);
            }
        }

        private static void CreateMethodBody(MethodInfo methodInfo, MethodBuilder methodBuilder, MSILCodeVariables variables)
        {
            // INFO: Pobranie generatora kodu IL dla tworzonej metody.
            var msil = methodBuilder.GetILGenerator();

            // INFO: Zadeklarowanie zmiennych lokalnych tablicowych, które będą przechowywały nazwy typów oraz wartości argumentów metody.
            variables.ArrayForArgumentTypesVariable = MSILCodeGenerator.DeclareLocalVariable(msil, typeof (String[]));
            variables.ArrayForArgumentValuesVariable = MSILCodeGenerator.DeclareLocalVariable(msil, typeof (Object[]));

            // INFO: Zainicjalizowanie zmiennych lokalnych tablicowych.
            MSILCodeGenerator.CreateArrayForArgumentTypes(msil, methodInfo, variables.ArrayForArgumentTypesVariable);
            MSILCodeGenerator.CraeteArrayForArgumentValues(msil, methodInfo, variables.ArrayForArgumentValuesVariable);

            // INFO: Pobranie parametrów metody.
            var parameters = methodInfo.GetParameters();

            // INFO: Skopiowanie nazw typów oraz wartości argumentów metody do zmiennych lokalnych tablicowych.
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];

                MSILCodeGenerator.StoreArgumentTypeNameInArray(msil, parameter, index, variables.ArrayForArgumentTypesVariable);
                MSILCodeGenerator.StoreArgumentValueInArray(msil, parameter, index, variables.ArrayForArgumentValuesVariable);
            }

            // INFO: Wywołanie metody interceptora (do której przekazujemy stworzone wcześniej zmienne lokalne tablicowe).
            MSILCodeGenerator.InvokeProxyInterceptorMethod(msil, methodInfo, variables);

            // INFO: Obsłużenie zwracanej przez interceptor wartości.
            MSILCodeGenerator.HandleProxyInterceptorMethodReturnValue(msil, methodInfo);

            // INFO: Przypisanie wartości argumentów przekazywanych przez referencję (ref, out).
            MSILCodeGenerator.AssignReferenceArgumentValues(msil, parameters, variables.ArrayForArgumentValuesVariable);

            // INFO: Zakończenie metody.
            MSILCodeGenerator.ReturnFromMethod(msil, methodInfo);
        }
    }
}