using System;
using Lucaniss.Tools.DynamicMocks;
using Lucaniss.Tools.DynamicProxy.Exceptions;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors;
using Lucaniss.Tools.DynamicProxy.Implementation.Interceptors.Implementations;
using Lucaniss.Tools.DynamicProxy.Tests.Data.Classes;
using Lucaniss.Tools.DynamicProxy.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lucaniss.Tools.DynamicProxy.Tests
{
    [TestClass]
    public class ProxyInterceptorTest
    {
        [TestMethod]
        public void Intercept_WhenMethodWasFound_WithoutParameters_ThenInvokeInterceptor()
        {
            // Arrange
            var interceptorBeforeInvokeFlag = false;
            var interceptorAfterInvokeFlag = false;

            var instance = new TestClassAndMethodWithoutParameters();
            var interceptor = new ProxyInterceptor();
            var interceptorHandlerMock = Mock.Create<IProxyInterceptorHandler>();

            interceptorHandlerMock.SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    interceptorBeforeInvokeFlag = true;
                    invokation.Invoke();
                    interceptorAfterInvokeFlag = true;
                });

            // Act
            instance.Echo();
            interceptor.Intercept(interceptorHandlerMock.Instance, instance, "Echo", new String[] { }, new Object[] { });

            // Assert
            Assert.AreEqual(true, interceptorBeforeInvokeFlag);
            Assert.AreEqual(true, interceptorAfterInvokeFlag);
        }

        [TestMethod]
        public void Intercept_WhenMethodWasFound_WithParameters_ThenInvokeInterceptor()
        {
            // Arrange
            var interceptorBeforeInvokeFlag = false;
            var interceptorAfterInvokeFlag = false;

            var instance = new TestClassAndMethodWithParameters();
            var interceptor = new ProxyInterceptor();
            var interceptorHandlerMock = Mock.Create<IProxyInterceptorHandler>();

            interceptorHandlerMock.SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    interceptorBeforeInvokeFlag = true;
                    invokation.Invoke();
                    interceptorAfterInvokeFlag = true;
                });

            // Act
            interceptor.Intercept(interceptorHandlerMock.Instance, instance, "Echo", new[] { typeof (String).FullName }, new Object[] { "Lucaniss" });

            // Assert
            Assert.AreEqual(true, interceptorBeforeInvokeFlag);
            Assert.AreEqual(true, interceptorAfterInvokeFlag);
        }

        [TestMethod]
        public void Intercept_WhenMethodIsMissing_ThenThrowException()
        {
            // Arrange
            var instance = new TestClass();
            var interceptor = new ProxyInterceptor();
            var interceptorHandlerMock = Mock.Create<IProxyInterceptorHandler>();

            interceptorHandlerMock.SetupMethod(e => e.Handle(Arg.Any<IProxyInvokation>()))
                .Callback<IProxyInvokation>(invokation =>
                {
                    invokation.Invoke();
                });

            Action action = () =>
            {
                interceptor.Intercept(interceptorHandlerMock.Instance, instance, "NotExistingMethod", new String[] { }, new Object[] { });
            };

            // Act
            AssertException.Throws<ProxyException>(action, e => e.Error == ProxyExceptionErrors.ProxyMethodNotFound);
        }
    }
}