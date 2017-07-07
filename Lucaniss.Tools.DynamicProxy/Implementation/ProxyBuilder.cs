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
        private Type _originalType;

        private Object _proxyInterceptorInstance;
        private Type _proxyInterceptorType;

        private Object _proxyInterceptorHandlerInstance;
        private Type _proxyInterceptorHandlerType;

        private TypeBuilder _typeBuilder;

        private Type _proxyBaseType;
        private Object _proxyInstance;


        public ProxyBuilder(IProxyCache proxyCache)
        {
            _proxyCache = proxyCache;
        }


        public Object Create(Type proxyType, Object originalInstance, Object interceptorHandlerIntance)
        {
            _originalInstance = originalInstance;
            _originalType = originalInstance.GetType();

            _proxyInterceptorInstance = new ProxyInterceptor();
            _proxyInterceptorType = typeof (IProxyInterceptor);

            _proxyInterceptorHandlerInstance = interceptorHandlerIntance;
            _proxyInterceptorHandlerType = interceptorHandlerIntance.GetType();

            _proxyBaseType = proxyType;

            // INFO: Why critical section? Because this code may be executed in multi-thread environment.
            //       So we need to be sure that for the same base types (instance and interceptor handler) we get exactly the same referance to proxy type.
            lock (CriticalSection)
            {
                CreateProxy();
            }

            return _proxyInstance;
        }


        private void CreateProxy()
        {
            var proxyCacheKey = new ProxyCacheKey(_proxyBaseType, _proxyInterceptorType, _proxyInterceptorHandlerType);

            var proxyType = _proxyCache.GetProxyType(proxyCacheKey);
            if (proxyType == null)
            {
                CreateClass();

                var classVariables = new MSILCodeVariables
                {
                    OriginalInstanceFieldInfo = CreateOriginalInstanceField(),
                    InterceptorInstanceFieldInfo = CreateInterceptorInstanceField(),
                    InterceptorHandlerInstanceFieldInfo = CreateInterceptorHandlerInstanceField()
                };

                CreateConstructor(classVariables);
                CreateMethods(classVariables);

                proxyType = _typeBuilder.CreateType();
                _proxyCache.AddProxyType(proxyCacheKey, proxyType);
            }

            _proxyInstance = Activator.CreateInstance(proxyType, _originalInstance, _proxyInterceptorInstance, _proxyInterceptorHandlerInstance);
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

        private FieldInfo CreateInterceptorHandlerInstanceField()
        {
            var fieldBuilder = _typeBuilder.DefineField(ProxyConsts.InterceptorHandlerInstanceFieldName, _proxyInterceptorHandlerType, FieldAttributes.Private);
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
            var defaultConstructor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { _proxyBaseType, _proxyInterceptorType, _proxyInterceptorHandlerType });
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