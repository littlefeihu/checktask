﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public abstract class Hyperlink
    {
        public HyperLinkType LinkType { get; protected set; }
    }
}
