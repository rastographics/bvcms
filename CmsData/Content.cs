using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;

namespace CmsData
{
    public partial class Content
    {
        public string ThumbUrl
        {
            get { return "/Image/{0}?v={1}".Fmt(ThumbID, DateCreated.HasValue ? DateCreated.Value.Ticks : 0); }
        }
    }
}
