using System;


namespace Lucaniss.Tools.DynamicProxy.Extensions
{
    internal static class ObjectExtensions
    {
        public static Type SafeGetType(this Object instance)
        {
            return instance.GetType().SafeGetType();
        }
    }
}