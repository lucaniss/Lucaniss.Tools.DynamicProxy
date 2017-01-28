using System;


// ReSharper disable All
// £F: used in proxy test only

namespace Lucaniss.Tools.DynamicProxy.Tests.Data.Classes
{
    public class TestClassWithParameterOut
    {
        public virtual void Echo(out String text)
        {
            text = "Echo";
        }
    }
}