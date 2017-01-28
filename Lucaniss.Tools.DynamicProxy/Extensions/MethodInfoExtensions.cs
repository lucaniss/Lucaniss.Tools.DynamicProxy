using System;
using System.Linq;
using System.Reflection;
using Lucaniss.Tools.DynamicProxy.Helpers;


namespace Lucaniss.Tools.DynamicProxy.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static Boolean IsSpecialCrlMethod(this MethodBase methodInfo)
        {
            return FrameworkHelper.GetSpecialClrMethodNames().Contains(methodInfo.Name);
        }
    }
}