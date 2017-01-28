using System;
using Lucaniss.Tools.DynamicMocks;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors;
using Lucaniss.Tools.DynamicProxy.Tests.Data;
using Lucaniss.Tools.DynamicProxy.Tests.Data.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lucaniss.Tools.DynamicProxy.Tests
{
    [TestClass]
    public class ProxyBehaviourForClassWithProperties
    {
        [TestMethod]
        public void InvokeProxyGetterMethod_WhenInvokeMethod_ThenInvokeInterceptor()
        {
            // Arrange
            var instance = new TestClassWithProperties();
            var interceptoHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClassWithProperties>>();

            var invocationHandler = new InvocationHandler();

            interceptoHandlerMock
                .SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    invokation.Invoke();
                    invocationHandler.IsInvoked = true;
                });

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptoHandlerMock.Instance);

            var value = proxy.TestProperty;

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.IsTrue(invocationHandler.IsInvoked);
        }

        [TestMethod]
        public void InvokeProxySetterMethod_WhenInvokeMethod_ThenInvokeInterceptor()
        {
            // Arrange
            var instance = new TestClassWithProperties();
            var interceptoHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClassWithProperties>>();

            var invocationHandler = new InvocationHandler();

            interceptoHandlerMock
                .SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    invokation.Invoke();
                    invocationHandler.IsInvoked = true;
                });

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptoHandlerMock.Instance);

            proxy.TestProperty = String.Empty;

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.IsTrue(invocationHandler.IsInvoked);
        }
    }
}