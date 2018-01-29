using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Segment
{
    public interface ISegment
    {
        Task Init();

        Task Init(string fileName);

        List<String> Analyse(string text);
    }
}
