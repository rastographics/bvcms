/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

namespace CmsData
{
    public partial class ChangeDetail
    {
        public ChangeDetail(string field, object before, object after)
        {
            Field = field;
            Before = before == null 
                ? "(null)" 
                : before.ToString();
            After = after == null 
                ? "(null)" 
                : after.ToString();
        }
    }
}
