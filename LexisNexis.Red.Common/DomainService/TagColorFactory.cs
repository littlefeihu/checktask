using LexisNexis.Red.Common.BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService
{
    public class TagColorFactory
    {
        public static List<TagColor> InitTagColors()
        {
            return new List<TagColor> {
		        new TagColor(ColorName.Magenta,"#ec008c"),
				new TagColor(ColorName.Purple,"#92278f"),
				new TagColor(ColorName.Indigo,"#4c009a"),
				new TagColor(ColorName.Blue,"#0076fe"),
				new TagColor(ColorName.Orange,"#ff9600"),
				new TagColor(ColorName.Green,"#45db5e"),
				new TagColor(ColorName.Teal,"#13d6c6"),
				new TagColor(ColorName.Cyan,"#54c7fd"),
				new TagColor(ColorName.Red,"#fe2951"),
				new TagColor(ColorName.Grey,"#cccccc"),
				new TagColor(ColorName.DarkGrey,"#898989"),
                new TagColor(ColorName.Black,"#363636")
             };
        }
    }
}
