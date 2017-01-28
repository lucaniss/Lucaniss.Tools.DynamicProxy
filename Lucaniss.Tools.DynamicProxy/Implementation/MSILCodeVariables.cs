using System.Reflection;
using System.Reflection.Emit;


namespace Lucaniss.Tools.DynamicProxy.Implementation
{
    internal class MSILCodeVariables
    {
        public FieldInfo OriginalInstanceFieldInfo;
        public FieldInfo InterceptorInstanceFieldInfo;
        public FieldInfo InterceptorHandlerInstanceFieldInfo;

        public LocalBuilder ArrayForArgumentTypesVariable { get; set; }
        public LocalBuilder ArrayForArgumentValuesVariable { get; set; }
    }
}