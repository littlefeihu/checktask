using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class IndexMenuItem
    {
        public string Title { get; set; }
        public string FileName { get; set; }
        public bool IsHeader { get; set; }
        public string Index { get; set; }
    }
}
