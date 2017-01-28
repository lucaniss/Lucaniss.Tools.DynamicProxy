using System;


// ReSharper disable All
// £F: used in proxy test only

namespace Lucaniss.Tools.DynamicProxy.Tests.Data.Classes
{
    public class TestClassWithConstructor
    {
        public virtual String Name { get; private set; }

        public TestClassWithConstructor(String name)
        {
            Name = name;
        }
    }
}