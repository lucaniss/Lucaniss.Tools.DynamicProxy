using System;
using Lucaniss.Tools.DynamicMocks;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors;
using Lucaniss.Tools.DynamicProxy.Tests.Data;
using Lucaniss.Tools.DynamicProxy.Tests.Data.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lucaniss.Tools.DynamicProxy.Tests
{
    [TestClass]
    public class ProxyBehaviourForClassWithMethods
    {
        [TestMethod]
        public void InvokeProxyMethod_WhenInvokeMethod_ThenInvokeInterceptor()
        {
            // Arrange
            var instance = new TestClassAndMethodWithParameters();
            var interceptoHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClassAndMethodWithParameters>>();

            const String value = "TEST";
            var invocationHandler = new InvocationHandler();

            interceptoHandlerMock
                .SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    invokation.Invoke();

                    invocationHandler.IsInvoked = true;
                    invocationHandler.InvokationArgument = invokation.ArgumentValues[0];
                });

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptoHandlerMock.Instance);
            proxy.Echo(value);

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.IsTrue(invocationHandler.IsInvoked);
            Assert.AreEqual(value, invocationHandler.InvokationArgument);
        }
    }
}