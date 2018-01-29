using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class ResouceHelper
    {
        public static Stream GetStreamFromAssembly(string assemblyName, string resourceName)
        {
            AssemblyName an = new AssemblyName(assemblyName);
            var assembly = Assembly.Load(an);
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
