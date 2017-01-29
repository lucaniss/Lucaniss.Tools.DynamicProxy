using System;
using Lucaniss.Tools.DynamicProxy.Exceptions;
using Lucaniss.Tools.DynamicProxy.Implementation;
using Lucaniss.Tools.DynamicProxy.Tests.Data.Classes;
using Lucaniss.Tools.DynamicProxy.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lucaniss.Tools.DynamicProxy.Tests
{
    [TestClass]
    public class ProxyInvocationTest
    {
        [TestMethod]
        public void Invoke_WhenMethodWasNotInvoked_ThenThrowException()
        {
            // Arrange
            var instance = new TestClassAndMethodWithParameters();
            var invokation = new ProxyInvocation(instance, TestClassAndMethodWithParameters.GetMethodNameForEcho(),
                new[] { typeof (String).AssemblyQualifiedName },
                new Object[] { "TEST" });

            // Act            
            Action action = () =>
            {
                invokation.GetReturnValue();
            };

            // Assert
            AssertException.Throws<ProxyException>(action, e => e.Error == ProxyExceptionErrors.ProxyMethodNotInvoked);
        }

        [TestMethod]
        public void Invoke_WhenMethodWasAlreadyInvoked_ThenThrowException()
        {
            // Arrange
            var instance = new TestClassAndMethodWithParameters();
            var invokation = new ProxyInvocation(instance, TestClassAndMethodWithParameters.GetMethodNameForEcho(),
                new[] { typeof (String).AssemblyQualifiedName },
                new Object[] { "TEST" });

            // Act
            invokation.Invoke();

            Action action = () =>
            {
                invokation.Invoke();
            };

            // Assert
            AssertException.Throws<ProxyException>(action, e => e.Error == ProxyExceptionErrors.ProxyMethodWasInvoked);
        }
    }
}