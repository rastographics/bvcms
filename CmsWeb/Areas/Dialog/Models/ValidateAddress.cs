using CmsData;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class ValidateAddress : LongRunningOperation
    {
        public const string Op = "ValidateAddress";

        public ValidateAddress()
        {
            QueryId = Guid.NewGuid();
            Tag = new CodeInfo("0", "Tag");
        }

        [DisplayName("Choose A Tag")]
        public CodeInfo Tag { get; set; }

        public void Process(CMSDataContext db)
        {
            pids = FetchPeopleIds(DbUtil.Db, Tag.Value.ToInt()).ToList();

            var lop = new LongRunningOperation
            {
                Started = DateTime.Now,
                Count = pids.Count,
                Processed = 0,
                QueryId = QueryId,
                Operation = Op,
            };
            DbUtil.Db.LongRunningOperations.InsertOnSubmit(lop);
            DbUtil.Db.SubmitChanges();

            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        internal List<int> pids;

        public bool TagHasBeenSelected
        {
            get { return Count.HasValue; }
        }

        public static void DoWork(ValidateAddress model)
        {
            var db = DbUtil.Create(model.Host);
            var cul = DbUtil.Db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOperation lop = null;
            foreach (var pid in model.pids)
            {
                DbUtil.Db.Dispose();
                var fsb = new List<ChangeDetail>();
                db = DbUtil.Create(model.Host);
                var f = DbUtil.Db.LoadFamilyByPersonId(pid);
                var ret = AddressVerify.LookupAddress(f.AddressLineOne, f.AddressLineTwo, f.CityName, f.StateCode, f.ZipCode);
                if (ret.found != false && !ret.error.HasValue() && ret.Line1 != "error")
                {
                    f.UpdateValue(fsb, "AddressLineOne", ret.Line1);
                    f.UpdateValue(fsb, "AddressLineTwo", ret.Line2);
                    f.UpdateValue(fsb, "CityName", ret.City);
                    f.UpdateValue(fsb, "StateCode", ret.State);
                    f.UpdateValue(fsb, "ZipCode", ret.Zip.GetDigits());
                    var rc = DbUtil.Db.FindResCode(ret.Zip, null);
                    f.UpdateValue(fsb, "ResCodeId", rc.ToString());
                }
                else
                {
                    f.UpdateValue(fsb, "ZipCode", f.ZipCode.Zip5());
                }

                lop = FetchLongRunningOperation(DbUtil.Db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                f.LogChanges(DbUtil.Db, fsb, pid, Util.UserPeopleId ?? 0);
                DbUtil.Db.SubmitChanges();
                //Thread.Sleep(1000);
            }
            // finished
            lop = FetchLongRunningOperation(DbUtil.Db, Op, model.QueryId);
            lop.Completed = DateTime.Now;
            DbUtil.Db.SubmitChanges();
        }
        public static IQueryable<int> FetchPeopleIds(CMSDataContext db, int tagid)
        {
            return tagid == -1
                ? (from p in DbUtil.Db.PeopleQueryLast()
                   group p by p.FamilyId into ff
                   select ff.First().PeopleId)
                : (from t in DbUtil.Db.TagPeople
                   where t.Id == tagid
                   group t.Person by t.Person.FamilyId into ff
                   select ff.First().PeopleId);
        }
        public void Validate(ModelStateDictionary modelState)
        {
            if (Tag != null && Tag.Value == "0") // They did not choose a tag
            {
                modelState.AddModelError("Tag", "Must choose a tag");
            }
        }

        public bool ShowCount(CMSDataContext db)
        {
            if (Count == null && Tag != null)
            {
                var q = FetchPeopleIds(DbUtil.Db, Tag.Value.ToInt());
                Count = q.Count();
                DbUtil.Db.SubmitChanges();
                return true;
            }
            return false;
        }
    }
}
