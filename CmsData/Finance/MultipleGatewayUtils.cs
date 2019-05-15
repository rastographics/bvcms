using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsData
{
    public class MultipleGatewayUtils
    {
        CMSDataContext db { get; set; }
        public MultipleGatewayUtils(CMSDataContext db)
        {
            this.db = db;
        }

        public static SelectList GatewayTypesList()
        {
            List<string> list = CMSDataContext.Create(HttpContextFactory.Current).Gateways
                .Select(g => g.GatewayName).ToList();
            list.Insert(0, "");
            return new SelectList(list);
        }

        public string Setting(string name, string defaultvalue, int ProcessId)
        {
            int? GatewayAccountId = db.PaymentProcess.Where(x => x.ProcessId == ProcessId).Select(x => x.GatewayAccountId).FirstOrDefault();
            if (name == null)
            {
                return defaultvalue;
            }

            var list = db.GatewayDetails.Where(x => x.GatewayAccountId == GatewayAccountId).ToDictionary(x => x.GatewayDetailName.Trim(), x => x.GatewayDetailValue);

            if (list == null)
            {
                try
                {
                    list = db.GatewayDetails.ToList().ToDictionary(x => x.GatewayDetailName.Trim(), x => x.GatewayDetailValue,
                        StringComparer.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    DbUtil.LogActivity($"Gateway: Could not get gateway-{ProcessId} details\nErr:\n{ex.Message}");
                    return string.Empty;
                }
            }
            if (list.ContainsKey(name) && list[name].HasValue())
                return list[name];
            if (defaultvalue.HasValue())
                return defaultvalue;
            return string.Empty;
        }

        public bool Setting(string name, int ProcessId, bool defaultValue = false)
        {
            var setting = Setting(name, null, ProcessId);
            if (!setting.HasValue())
            {
                return defaultValue;
            }

            return setting.ToLower() == "true";
        }

        public int? GatewayId (PaymentProcessTypes processType)
        {
            return (from e in db.PaymentProcess
                    join d in db.GatewayAccount on e.GatewayAccountId equals d.GatewayAccountId into gj
                    from sub in gj.DefaultIfEmpty()
                    where e.ProcessId == (int)processType
                    select new
                    {
                        sub.GatewayId
                    }).FirstOrDefault().GatewayId;
        }

        public bool GatewayTesting(PaymentProcessTypes processType)
        {
            var User = db.Users.SingleOrDefault(us => us.UserId == Util.UserId);
            return (User != null && User.InRole("Developer")) ? Setting("GatewayTesting", (int)processType) : false;
        }

        public PaymentProcessTypes ProcessByTransactionDescription(string Description)
        {
            Regex _rx = new Regex("\\s+[(][0-9]+[)]");
            Match _mt = _rx.Match(Description);
            if (_mt.Success)
            {
                Description = Description.Replace(_mt.Value, "");
            }

            int? RegistrationTypeId = db.Organizations.Where(x => x.OrganizationName == Description).Select(x => x.RegistrationTypeId).FirstOrDefault();

            switch (RegistrationTypeId)
            {
                case null:
                case 8:
                    return PaymentProcessTypes.OneTimeGiving;
                default:
                    return PaymentProcessTypes.OnlineRegistration;
            }
        }
    }
}
