using System;
using System.Collections.Generic;


namespace Lucaniss.Tools.DynamicProxy.Helpers
{
    internal static class FrameworkHelper
    {
        public static IEnumerable<String> GetSpecialClrMethodNames()
        {
            yield return "GetType";
        }
    }
}