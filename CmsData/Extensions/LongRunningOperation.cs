﻿using System;
using UtilityExtensions;
using System.Linq;

namespace CmsData
{
    public partial class LongRunningOperation
    {
        partial void OnCreated()
        {
            try
            {
                Host = Host ?? Util.Host; //TODO: Don't fallback to Util.Host
            }
            catch { }
        }

        public void UpdateLongRunningOp(CMSDataContext db, string op)
        {
            var lop = FetchLongRunningOperation(db, op, QueryId);
            lop?.CopyProperties2(this);
        }
        public static LongRunningOperation FetchLongRunningOperation(CMSDataContext db, string op, Guid queryid)
        {
            var lop = db.LongRunningOperations.SingleOrDefault(m => m.Operation == op && m.QueryId == queryid);
            if (lop != null)
            {
                lop.Host = db.Host;
            }
            return lop;
        }
        public string Host { get; set; }

        public bool Finished => Completed.HasValue;

        public static void RemoveExisting(CMSDataContext db, Guid id)
        {
            var lop = db.LongRunningOperations.SingleOrDefault(m =>  m.QueryId == id);
            if (lop == null)
                return;
            db.LongRunningOperations.DeleteOnSubmit(lop);
            db.SubmitChanges();
        }

//        partial void OnQueryIdChanged()
//        {
//            QueryIdChanged();
//        }
//
//        public virtual void QueryIdChanged()
//        {
//            
//        }
    }
}
