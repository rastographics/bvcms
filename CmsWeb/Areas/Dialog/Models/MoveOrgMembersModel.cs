using CmsData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class MoveOrgMembersModel : LongRunningOperation
    {
        private IList<string> list = new List<string>();
        private string repairExe;
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

            repairExe = HttpContextFactory.Current.Server.MapPath("~/bin/RepairOrg.exe");
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoMoveWork(this));
        }

        public static void DoMoveWork(MoveOrgMembersModel model)
        {
            var statusContext = CMSDataContext.Create(model.Host);
            var workerContext = CMSDataContext.Create(model.Host);
            workerContext.CommandTimeout = 2200;
            var cul = workerContext.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            LongRunningOperation lop = FetchLongRunningOperation(statusContext, Op, model.QueryId);
            var orgsPeople = new Dictionary<int, List<int>>();
            foreach (var i in model.List) // {personId}.{organizationId}
            {
                var a = i.Split('.');
                if (a.Length != 2)
                {
                    continue;
                }

                var orgId = a[1].ToInt();

                if (orgId == model.TargetId)
                {
                    continue;
                }

                if (!orgsPeople.ContainsKey(orgId))
                {
                    orgsPeople[orgId] = new List<int>();
                }
                orgsPeople[orgId].Add(a[0].ToInt());
            }

            foreach (var oid in orgsPeople.Keys) 
            {
                var peopleIds = orgsPeople[oid];
                foreach (var pid in peopleIds)
                {
                    OrganizationMember.MoveToOrg(workerContext, pid, oid, model.TargetId, model.MoveRegistrationData, model.ChangeMemberType == true ? model.MoveToMemberTypeId : -1);
                    if (lop != null)
                    {
                        lop.Processed++;
                        lop.CustomMessage = $"Working from {pid},{oid} to {model.TargetId}";
                        statusContext.SubmitChanges();
                    }
                }
                model.BackgroundRepairTransactions(oid, workerContext);
            }
            model.BackgroundRepairTransactions(model.TargetId, workerContext);
            // finished
            if (lop != null)
            {
                lop.Completed = DateTime.Now;
                statusContext.SubmitChanges();
            }
            workerContext.UpdateMainFellowship(model.TargetId);
        }

        private void BackgroundRepairTransactions(int orgId, CMSDataContext context)
        {
            var connectionString = context.Connection.ConnectionString;
            var host = context.Host;
            Process.Start(new ProcessStartInfo
            {
                FileName = repairExe,
                Arguments = $"{orgId} --connection {connectionString} --host {host}",
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(repairExe)
            });
        }
    }
}
