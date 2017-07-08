using System;


namespace Lucaniss.Tools.DynamicProxy.Consts
{
    public static class ProxyConsts
    {
        public const String AssemblyName = "Lucaniss.Tools.Proxy.Assembly";
        public const String ModuleName = "Lucaniss.Tools.Proxy.Module";
        public const String TypeName = "{0}Proxy";

        public const String OriginalInstanceFieldName = "_proxyOriginalInstance";
        public const String InterceptorInstanceFieldName = "_proxyInterceptorInstance";
    }
}