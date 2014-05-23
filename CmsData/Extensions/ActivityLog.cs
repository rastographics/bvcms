/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData
{
    public partial class ActivityLog
    {
        public string HowLongAgo
        {
            get 
            {
                TimeSpan delta = Util.Now.Subtract(ActivityDate.Value);
                if (delta.Days > 7)
                    return ActivityDate.Value.ToShortDateString();
                else if (delta.Days > 1)
                    return string.Format("{0} days ago", delta.Days);
                else if (delta.Days == 1)
                    return string.Format("Yesterday", delta.Days);
                else if (delta.Hours > 1)
                    return string.Format("{0} hours ago", delta.Hours);
                else if (delta.Hours == 1)
                    return string.Format("1 hour ago", delta.Hours);
                else if (delta.Minutes > 1)
                    return string.Format("{0} minutes ago", delta.Minutes);
                else if (delta.Minutes == 1)
                    return string.Format("1 minute ago", delta.Minutes);
                else
                    return string.Format("{0} seconds ago", delta.Seconds);
            }
        }
    }
}
