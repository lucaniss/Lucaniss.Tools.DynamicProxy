using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Lucaniss.Tools.DynamicProxy.Extensions
{
    internal static class TypeExtensions
    {
        public static IEnumerable<MethodInfo> GetMethodInfosForProxy(this Type type)
        {
            return type.GetMethods().Where(m => !m.IsStatic && m.IsPublic && m.IsVirtual);
        }

        public static Type SafeGetType(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        public static String SafeGetTypeName(this Type type)
        {
            return SafeGetType(type).IsGenericParameter ? type.Name : type.AssemblyQualifiedName;
        }

        public static Boolean IsValueOrPrimitiveType(this Type parameterInfo)
        {
            var type = parameterInfo.SafeGetType();
            return type.IsValueType || type.IsPrimitive;
        }
    }
}