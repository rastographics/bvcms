/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using UtilityExtensions;

namespace CmsData
{
    public partial class ActivityLog
    {
        public string HowLongAgo
        {
            get
            {
                var delta = Util.Now.Subtract(ActivityDate.Value);
                if (delta.Days > 7)
                    return ActivityDate.Value.ToShortDateString();
                if (delta.Days > 1)
                    return $"{delta.Days} days ago";
                if (delta.Days == 1)
                    return "Yesterday";
                if (delta.Hours > 1)
                    return $"{delta.Hours} hours ago";
                if (delta.Hours == 1)
                    return "1 hour ago";
                if (delta.Minutes > 1)
                    return $"{delta.Minutes} minutes ago";
                if (delta.Minutes == 1)
                    return "1 minute ago";
                return $"{delta.Seconds} seconds ago";
            }
        }
    }
}
