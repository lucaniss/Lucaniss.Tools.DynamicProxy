using System;
using Lucaniss.Tools.DynamicMocks;
using Lucaniss.Tools.DynamicProxy.Exceptions;
using Lucaniss.Tools.DynamicProxy.Implementation;
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

            var interceptorHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClassAndMethodWithoutParameters>>();

            interceptorHandlerMock.SetupMethod(e => e.Handle(Arg.Any<IProxyInvocation<TestClassAndMethodWithoutParameters>>()))
                .Callback<IProxyInvocation<TestClassAndMethodWithoutParameters>>(invokation =>
                {
                    interceptorBeforeInvokeFlag = true;
                    invokation.Invoke();
                    interceptorAfterInvokeFlag = true;
                });

            var instance = new TestClassAndMethodWithoutParameters();
            var interceptor = new ProxyInterceptor<TestClassAndMethodWithoutParameters>(interceptorHandlerMock.Instance);

            // Act
            instance.Echo();
            interceptor.Intercept(instance, "Echo", new String[] { }, new Object[] { });

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

            var interceptorHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClassAndMethodWithParameters>>();

            interceptorHandlerMock.SetupMethod(e => e.Handle(Arg.Any<IProxyInvocation<TestClassAndMethodWithParameters>>()))
                .Callback<IProxyInvocation<TestClassAndMethodWithParameters>>(invokation =>
                {
                    interceptorBeforeInvokeFlag = true;
                    invokation.Invoke();
                    interceptorAfterInvokeFlag = true;
                });

            var instance = new TestClassAndMethodWithParameters();
            var interceptor = new ProxyInterceptor<TestClassAndMethodWithParameters>(interceptorHandlerMock.Instance);

            // Act
            interceptor.Intercept(instance, "Echo", new[] { typeof (String).AssemblyQualifiedName }, new Object[] { "Lucaniss" });

            // Assert
            Assert.AreEqual(true, interceptorBeforeInvokeFlag);
            Assert.AreEqual(true, interceptorAfterInvokeFlag);
        }

        [TestMethod]
        public void Intercept_WhenMethodIsMissing_ThenThrowException()
        {
            // Arrange          
            var interceptorHandlerMock = Mock.Create<IProxyInterceptorHandler<TestClass>>();

            interceptorHandlerMock.SetupMethod(e => e.Handle(Arg.Any<IProxyInvocation<TestClass>>()))
                .Callback<IProxyInvocation<TestClass>>(invokation =>
                {
                    invokation.Invoke();                    
                });

            var instance = new TestClass();
            var interceptor = new ProxyInterceptor<TestClass>(interceptorHandlerMock.Instance);

            Action action = () =>
            {
                interceptor.Intercept(instance, "NotExistingMethod", new String[] { }, new Object[] { });
            };

            // Act
            AssertException.Throws<ProxyException>(action, e => e.Error == ProxyExceptionErrors.ProxyMethodNotFound);
        }
    }
}