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
        private Object _originalInstance;
        private Type _originalType;

        private Object _ThenInvokeInterceptororInstance;
        private Type _ThenInvokeInterceptororType;

        private Object _ThenInvokeInterceptororHandlerInstance;
        private Type _ThenInvokeInterceptororHandlerType;

        private TypeBuilder _typeBuilder;

        private Type _proxyType;
        private Object _proxyInstance;


        public Object Create(Type proxyType, Object originalInstance, Object interceptorHandlerIntance)
        {
            _originalInstance = originalInstance;
            _originalType = originalInstance.GetType();

            _ThenInvokeInterceptororInstance = new ProxyInterceptor();
            _ThenInvokeInterceptororType = typeof (IProxyInterceptor);

            _ThenInvokeInterceptororHandlerInstance = interceptorHandlerIntance;
            _ThenInvokeInterceptororHandlerType = interceptorHandlerIntance.GetType();

            _proxyType = proxyType;

            CreateProxy();

            return _proxyInstance;
        }


        private void CreateProxy()
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

            _proxyInstance = Activator.CreateInstance(_typeBuilder.CreateType(), _originalInstance, _ThenInvokeInterceptororInstance, _ThenInvokeInterceptororHandlerInstance);
        }


        private FieldInfo CreateOriginalInstanceField()
        {
            var fieldBuilder = _typeBuilder.DefineField(ProxyConsts.OriginalInstanceFieldName, _proxyType, FieldAttributes.Private);
            return fieldBuilder;
        }

        private FieldInfo CreateInterceptorInstanceField()
        {
            var fieldBuilder = _typeBuilder.DefineField(ProxyConsts.InterceptorInstanceFieldName, _ThenInvokeInterceptororType, FieldAttributes.Private);
            return fieldBuilder;
        }

        private FieldInfo CreateInterceptorHandlerInstanceField()
        {
            var fieldBuilder = _typeBuilder.DefineField(ProxyConsts.InterceptorHandlerInstanceFieldName, _ThenInvokeInterceptororType, FieldAttributes.Private);
            return fieldBuilder;
        }


        private void CreateClass()
        {
            var typeName = String.Format(ProxyConsts.TypeName, _proxyType.FullName);

            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(ProxyConsts.AssemblyName), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(ProxyConsts.ModuleName);

            _typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, _proxyType);
        }

        private void CreateConstructor(MSILCodeVariables variables)
        {
            var defaultConstructor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { _proxyType, _ThenInvokeInterceptororType, _ThenInvokeInterceptororHandlerType });
            var msil = defaultConstructor.GetILGenerator();

            MSILCodeGenerator.CreateConstructor(msil, variables);
        }

        private void CreateMethods(MSILCodeVariables variables)
        {
            foreach (var methodInfo in _proxyType.GetMethodInfosForProxy())
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