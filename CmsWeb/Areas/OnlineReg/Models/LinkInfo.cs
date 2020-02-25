using CmsData;
using System;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class LinkInfo
    {
        private readonly string from;
        private readonly string link;
        internal string[] a;
        internal string error;
        internal int? oid;
        internal OneTimeLink ot;
        internal int? pid;

        public LinkInfo(CMSDataContext db, string link, string from, string id, bool hasorg = true)
        {
            this.link = link;
            this.from = from;
            try
            {
                if (!id.HasValue())
                {
                    throw LinkException("missing id");
                }

                var guid = id.ToGuid();
                if (guid == null)
                {
                    throw LinkException("invalid id");
                }

                ot = db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                if (ot == null)
                {
                    throw LinkException("missing link");
                }

                a = ot.Querystring.SplitStr(",", 5);
                if (hasorg)
                {
                    oid = a[0].ToInt();
                }

                pid = a[1].ToInt();
#if DEBUG
#else
                    if (ot.Used)
                        throw LinkException("link used");
                    if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                        throw LinkException("link expired");
#endif
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        internal Exception LinkException(string msg)
        {
            DbUtil.LogActivity($"{link}{@from}Error: {msg}", oid, pid);
            return new Exception(msg);
        }
    }
}
