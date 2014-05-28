using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;

namespace CmsData
{
    public partial class Division
    {
        public bool ToggleTag(CMSDataContext Db, int progid)
        {
            var pd = ProgDivs.SingleOrDefault(d => d.ProgId == progid);
            if (pd == null)
            {
                ProgDivs.Add(new ProgDiv { ProgId = progid });
                return true;
            }
            ProgDivs.Remove(pd);
            if (ProgId == progid)
                ProgId = null;
            Db.ProgDivs.DeleteOnSubmit(pd);
            return false;
        }
    }
}
