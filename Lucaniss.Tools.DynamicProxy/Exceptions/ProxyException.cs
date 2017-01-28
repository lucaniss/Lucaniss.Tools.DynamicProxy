using System;


namespace Lucaniss.Tools.DynamicProxy.Exceptions
{
    public class ProxyException : Exception
    {
        public ProxyExceptionErrors Error { get; private set; }


        public ProxyException(ProxyExceptionErrors error, String message)
            : base(message)
        {
            Error = error;
        }
    }
}