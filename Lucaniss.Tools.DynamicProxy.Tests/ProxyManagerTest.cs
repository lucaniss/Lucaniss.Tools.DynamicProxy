using System;
using Lucaniss.Tools.DynamicMocks;
using Lucaniss.Tools.DynamicProxy.Consts;
using Lucaniss.Tools.DynamicProxy.Exceptions;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors;
using Lucaniss.Tools.DynamicProxy.Tests.Data.Classes;
using Lucaniss.Tools.DynamicProxy.Tests.Data.Classes.Inheritance;
using Lucaniss.Tools.DynamicProxy.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lucaniss.Tools.DynamicProxy.Tests
{
    [TestClass]
    public class ProxyManagerTest
    {
        [TestMethod]
        public void CreateProxy_WhenUsedGenericMethod_WithImplicitProxyType_ThenReturnProxy()
        {
            // Arrange
            var instance = new TestClassInherited();
            var interceptorHandler = CreateDefaultInterceptorHandler<TestClassInherited>();

            // Act
            var proxy = Proxy.Create(instance, interceptorHandler);

            // Assert
            Assert.IsNotNull(proxy);

            Assert.AreEqual(typeof (TestClassInherited).Name + "Proxy", proxy.GetType().Name);
            Assert.AreEqual(typeof (TestClassInherited), proxy.GetType().BaseType);
        }

        [TestMethod]
        public void CreateProxy_WhenUsedGenericMethod_WithExpliciteProxyType_ThenReturnProxy()
        {
            // Arrange
            var instance = new TestClassInherited();
            var interceptorHandler = CreateDefaultInterceptorHandler<TestClassBase>();

            // Act
            var proxy = Proxy.Create<TestClassInherited, TestClassBase, IProxyInterceptorHandler<TestClassBase>>(instance, interceptorHandler);

            // Assert
            Assert.IsNotNull(proxy);

            Assert.AreEqual(String.Format(ProxyConsts.TypeName, typeof (TestClassBase).Name), proxy.GetType().Name);
            Assert.AreEqual(typeof (TestClassBase), proxy.GetType().BaseType);
        }


        [TestMethod]
        public void CreateProxy_WhenUsedGeneralMethod_WithImplicitProxyType_ThenReturnProxy()
        {
            // Arrange
            Object instance = new TestClassInherited();
            Object interceptorHandler = CreateDefaultInterceptorHandler<TestClassInherited>();

            // Act
            var proxy = Proxy.Create(instance, interceptorHandler);

            // Assert
            Assert.AreEqual(String.Format(ProxyConsts.TypeName, typeof (TestClassInherited).Name), proxy.GetType().Name);
            Assert.AreEqual(typeof (TestClassInherited), proxy.GetType().BaseType);
        }

        [TestMethod]
        public void CreateProxy_WhenUsedGeneralMethod_WithExpliciteProxyType_ThenReturnProxy()
        {
            // Arrange
            Object instance = new TestClassInherited();
            Object interceptorHandler = CreateDefaultInterceptorHandler<TestClassInherited>();

            // Act
            var proxy = Proxy.Create(typeof (TestClassBase), instance, interceptorHandler);

            // Assert
            Assert.AreEqual(String.Format(ProxyConsts.TypeName, typeof (TestClassBase).Name), proxy.GetType().Name);
            Assert.AreEqual(typeof (TestClassBase), proxy.GetType().BaseType);
        }


        [TestMethod]
        public void CreateProxy_WhenOriginalInstanceHasNotEmptyConstructor_ThenReturnProxy()
        {
            // Arrange
            var instance = new TestClassWithConstructor("Lucaniss");
            var interceptorHandler = CreateDefaultInterceptorHandler<TestClassWithConstructor>();

            // Act
            var proxy = Proxy.Create(instance, interceptorHandler);

            // Assert
            Assert.IsNotNull(proxy);
            Assert.AreEqual("Lucaniss", proxy.Name);
        }


        [TestMethod]
        public void CreateProxy_WhenOriginalInstanceIsNull_ThenThrowException()
        {
            // Act
            Action action = () =>
            {
                Proxy.Create(null, CreateDefaultInterceptorHandler<TestClass>());
            };

            // Assert
            AssertException.Throws<ProxyException>(action, e => e.Error == ProxyExceptionErrors.OriginalInstanceIsNull);
        }

        [TestMethod]
        public void CreateProxy_WhenInterceptorInstanceIsNull_ThenThrowException()
        {
            // Act
            Action action = () =>
            {
                Proxy.Create(new TestClass(), null);
            };

            // Assert
            AssertException.Throws<ProxyException>(action, e => e.Error == ProxyExceptionErrors.InterceptorInstanceIsNull);
        }

        [TestMethod]
        public void CreateProxy_WhenOrignalInstancaAndInterceptorDoesNotHaveCompatibleTypes_ThenThrowException()
        {
            // Act
            Object instance = new TestClass();
            Object interceptorHandler = CreateDefaultInterceptorHandler<TestClassInherited>();

            Action action = () =>
            {
                Proxy.Create(instance, interceptorHandler);
            };

            // Assert
            AssertException.Throws<ProxyException>(action, e => e.Error == ProxyExceptionErrors.OriginalInstanceAndInterceptorTypesAreNotCompatible);
        }


        private static IProxyInterceptorHandler<T> CreateDefaultInterceptorHandler<T>() where T : class
        {
            var mock = Mock.Create<IProxyInterceptorHandler<T>>();

            mock.SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    invokation.Invoke();
                });

            return mock.Instance;
        }
    }
}