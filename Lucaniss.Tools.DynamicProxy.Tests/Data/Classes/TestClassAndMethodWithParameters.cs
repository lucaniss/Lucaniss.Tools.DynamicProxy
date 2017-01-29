using System;


// ReSharper disable All
// £F: used in proxy test only

namespace Lucaniss.Tools.DynamicProxy.Tests.Data.Classes
{
    public class TestClassAndMethodWithParameters
    {
        public virtual void Echo(String text)
        {
        }

        public static String GetMethodNameForEcho()
        {
            TestClassAndMethodWithParameters c;
            return nameof(c.Echo);
        }
    }
}