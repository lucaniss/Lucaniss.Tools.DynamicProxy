using System;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    internal class ProxyCacheKey
    {
        private readonly Type _baseType;
        private readonly Type _interceptorType;
        private readonly Type _interceptorHandlerType;


        public ProxyCacheKey(Type baseType, Type interceptorType, Type interceptorHandlerType)
        {
            _baseType = baseType;
            _interceptorType = interceptorType;
            _interceptorHandlerType = interceptorHandlerType;
        }


        private Boolean Equals(ProxyCacheKey other)
        {
            return (_baseType == other._baseType) && (_interceptorType == other._interceptorType) && (_interceptorHandlerType == other._interceptorHandlerType);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ProxyCacheKey) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hashCode = _baseType?.GetHashCode() ?? 0;

                hashCode = (hashCode*397) ^ (_interceptorType?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (_interceptorHandlerType?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }
}