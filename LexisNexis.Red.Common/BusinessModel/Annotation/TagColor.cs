using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class TagColor
    {
        public TagColor(ColorName colorName, string colorValue)
        {
            this.ColorName = colorName;
            this.ColorValue = colorValue;
        }
        public ColorName ColorName
        {
            get;
            private set;
        }
        public string ColorValue
        {
            get;
            private set;
        }
    }


}
