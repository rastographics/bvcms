﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityExtensions;

namespace CmsData
{
    public partial class MeetingExtra
    {
        public string GetFirstValue() => Util.PickFirst(Data, StrValue, $"{IntValue}", $"{DateValue}");
    }
}
