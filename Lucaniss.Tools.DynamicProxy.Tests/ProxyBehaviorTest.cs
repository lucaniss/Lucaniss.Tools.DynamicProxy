using Lucaniss.Tools.Proxy.Tests.Data.Classes;
using Lucaniss.Tools.Proxy.Tests.Data.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lucaniss.Tools.Proxy.Tests
{
    [TestClass]
    public class ProxyBehaviorTest
    {
        [TestMethod]
        public void InvokeProxyMethod_HappyPath_WhenCallMethodWithoutParameters_Intercept()
        {
            // Arrange
            var instance = new TestClassAndMethodWithoutParameters();
            var interceptor = new TestClassAndMethodWithoutParametersInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);

            proxy.Echo();

            // Assert
            Assert.IsNotNull(proxy);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }

        [TestMethod]
        public void InvokeProxyMethod_HappyPath_WhenCallMethodWithParameters_Itercept()
        {
            // Arrange
            var instance = new TestClassAndMethodWithParameters();
            var interceptor = new TestClassAndMethodWithParametersInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);
            proxy.Echo("Lucaniss");

            // Assert
            Assert.IsNotNull(proxy);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }


        [TestMethod]
        public void InvokeProxyProperty_HappyPath_WhenCallPropertyGetMethod_Intercept()
        {
            // Arrange
            var instance = new TestClassWithProperties
            {
                TestProperty = "Lucaniss"
            };

            var interceptor = new TestClassWithPropertiesInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);

            var propertyValue = proxy.TestProperty;

            // Assert
            Assert.IsNotNull(proxy);
            Assert.AreEqual("Lucaniss", propertyValue);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }

        [TestMethod]
        public void InvokeProxyProperty_HappyPath_WhenCallPropertySetMethod_Intercept()
        {
            // Arrange
            var instance = new TestClassWithProperties();
            var interceptor = new TestClassWithPropertiesInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);

            proxy.TestProperty = "Lucaniss";

            // Assert
            Assert.IsNotNull(proxy);

            Assert.AreEqual("Lucaniss", instance.TestProperty);
            Assert.AreEqual("Lucaniss", proxy.TestProperty);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }


        [TestMethod]
        public void InvokeProxyProperty_HappyPath_WhenCallPropertyGetMethodByReflection_Intercept()
        {
            // Arrange
            var instance = new TestClassWithProperties();
            var interceptor = new TestClassWithPropertiesInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);

            var property = typeof (TestClassWithProperties).GetProperty("TestProperty");
            var propertyValue = property.GetValue(proxy);

            // Assert
            Assert.IsNotNull(proxy);
            Assert.IsNull(propertyValue);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }

        [TestMethod]
        public void InvokeProxyProperty_HappyPath_WhenCallPropertySetMethodByReflection_Intercept()
        {
            // Arrange
            var instance = new TestClassWithProperties();
            var interceptor = new TestClassWithPropertiesInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);

            var property = typeof (TestClassWithProperties).GetProperty("TestProperty");
            property.SetValue(proxy, "Lucaniss");

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.IsNotNull("Lucaniss", instance.TestProperty);
            Assert.IsNotNull("Lucaniss", proxy.TestProperty);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }


        [TestMethod]
        public void InvokeProxyProperty_HappyPath_WhenCallPropertyGetMethodWithDirtyInterceptor_ChangedValue()
        {
            // Arrange
            var instance = new TestClassWithProperties();
            var interceptor = new DirtyChangingGetInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);

            var proxyValue = proxy.TestProperty;

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.AreNotEqual("Lucaniss", proxyValue);
            Assert.AreEqual("Dirty Value", proxyValue);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }

        [TestMethod]
        public void InvokeProxyProperty_HappyPath_WhenCallPropertySetMethodWithDirtyInterceptor_ChangedValue()
        {
            // Arrange
            var instance = new TestClassWithProperties();
            var interceptor = new DirtyChangingSetInterceptor();

            // Act
            var proxy = ProxyManager.CreateProxy(instance, interceptor);

            proxy.TestProperty = "Lucaniss";

            // Assert           
            Assert.IsNotNull(proxy);

            Assert.AreNotEqual("Lucaniss", instance.TestProperty);
            Assert.AreNotEqual("Lucaniss", proxy.TestProperty);

            Assert.AreEqual("Dirty Value", instance.TestProperty);
            Assert.AreEqual("Dirty Value", proxy.TestProperty);

            Assert.AreEqual(true, interceptor.BeforeMethodCallWasInvoked);
            Assert.AreEqual(true, interceptor.AfterMethodCallWasInvoked);
        }       
    }
}