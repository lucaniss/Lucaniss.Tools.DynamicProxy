using System;


namespace Lucaniss.Tools.DynamicProxy.Tests.Data
{
    internal class InvocationHandler
    {
        public Boolean IsInvoked { get; set; }
        public Object InvokationArgument { get; set; }
    }
}