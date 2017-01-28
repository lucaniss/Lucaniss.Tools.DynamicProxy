using System;


// ReSharper disable All
// ŁF: used in proxy test only

namespace Lucaniss.Tools.DynamicProxy.Tests.Data.Classes
{
    public class TestClassWithParameterRef
    {
        public virtual void Echo(ref String text)
        {
            text = "Echo (altered)";
        }
    }
}