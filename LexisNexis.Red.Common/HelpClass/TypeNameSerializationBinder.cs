using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class TypeNameSerializationBinder : SerializationBinder
    {
        public static readonly TypeNameSerializationBinder Instance = new TypeNameSerializationBinder();

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {

            assemblyName = null;
            typeName = serializedType.AssemblyQualifiedName;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName, true);
        }
    }
}
