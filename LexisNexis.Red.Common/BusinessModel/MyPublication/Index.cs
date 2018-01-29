using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class Index
    {

        public string Title { get; set; }

        public string FileName { get; set; }

        internal void TolowerCase()
        {
            if (Title != null && Title.Length > 1)
                Title = Title[0] + Title.Substring(1).ToLower();
        }
    }
}
