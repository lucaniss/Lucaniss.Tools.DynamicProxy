using System;
using Lucaniss.Tools.DynamicMocks;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors;
using Lucaniss.Tools.DynamicProxy.Tests.Data.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lucaniss.Tools.DynamicProxy.Tests
{
    [TestClass]
    public class ProxyBehaviourForClassWithRefParameters
    {
        [TestMethod]
        public void InvokeProxyMethod_WhenMethodParameterIsRef_ThenChangeValue()
        {
            // Arrange
            var instance = new TestClassWithParameterRef();
            var interceptoHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClassWithParameterRef>>();

            const String valuBeforeCall = "Before";
            const String valuAfterCall = "After";

            interceptoHandlerMock
                .SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    invokation.Invoke();
                    invokation.ArgumentValues[0] = valuAfterCall;
                });

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptoHandlerMock.Instance);

            var text = valuBeforeCall;
            proxy.Echo(ref text);

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.AreNotEqual(valuBeforeCall, text);
            Assert.AreEqual(valuAfterCall, text);
        }

        [TestMethod]
        public void InvokeProxyMethod_WhenMethodParameterIsOut_ThenChangeValue()
        {
            // Arrange
            var instance = new TestClassWithParameterOut();
            var interceptoHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClassWithParameterOut>>();

            const String valuAfterCall = "After";

            interceptoHandlerMock
                .SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    invokation.Invoke();
                    invokation.ArgumentValues[0] = valuAfterCall;
                });

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptoHandlerMock.Instance);

            String text;
            proxy.Echo(out text);

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.AreNotEqual(String.Empty, text);
            Assert.AreEqual(valuAfterCall, text);
        }
    }
}