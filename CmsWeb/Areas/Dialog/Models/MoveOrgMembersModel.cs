using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web.Hosting;
using CmsData;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class MoveOrgMembersModel : LongRunningOperation
    {
        private IList<string> list = new List<string>();
        public const string Op = "Move Members To Org";

        public MoveOrgMembersModel()
        {
            QueryId = Guid.NewGuid();
            List = new List<string>();
        }

        public int TargetId { get; set; }
        public bool MoveRegistrationData { get; set; }

        public IList<string> List
        {
            get { return list; }
            set
            {
                list = value;
                SerializedList = JsonConvert.SerializeObject(list);
            }
        }

        public string SerializedList { get; set; }

        public int ListCount => List.Count;
        public bool ChangeMemberType { get; set; }
        public int MoveToMemberTypeId { get; set; }

        public void ProcessMove(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            list = JsonConvert.DeserializeObject<IList<string>>(SerializedList);
            var lop = new LongRunningOperation()
            {
                Started = DateTime.Now,
                Count = List.Count,
                Processed = 0,
                QueryId = QueryId,
                Operation = Op,
            };
            db.LongRunningOperations.InsertOnSubmit(lop);
            db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoMoveWork(this));
		}
        public static void DoMoveWork(MoveOrgMembersModel model)
        {
			var db = DbUtil.Create(model.Host);
            db.CommandTimeout = 2200;
		    var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            LongRunningOperation lop = null;

            foreach (var i in model.List)
            {
                var a = i.Split('.');
                if (a.Length != 2)
                    continue;
                var pid = a[0].ToInt();
                var oid = a[1].ToInt();

                if (oid == model.TargetId)
                    continue;
                OrganizationMember.MoveToOrg(db, pid, oid, model.TargetId, model.MoveRegistrationData, model.ChangeMemberType == true ? model.MoveToMemberTypeId : -1);
                //Once member has been inserted into the new Organization then update member  in Previous Organization as not enrolled.
                db.RepairTransactions(oid);
                lop = FetchLongRunningOperation(db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                lop.CustomMessage = $"Working from {pid},{oid} to {model.TargetId}";
                db.SubmitChanges();
            }
            // finished
            lop = FetchLongRunningOperation(db, Op, model.QueryId);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
            db.UpdateMainFellowship(model.TargetId);
		}
    }
}
