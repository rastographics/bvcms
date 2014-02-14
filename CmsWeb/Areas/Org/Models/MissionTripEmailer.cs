using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class MissionTripEmailer
    {
        private int oid;
        private int pid;

        public MissionTripEmailer(int oid, int pid)
        {
            this.oid = oid;
            this.pid = pid;
            var org = DbUtil.Db.LoadOrganizationById(oid);
            var m = new Settings(org.RegSetting, DbUtil.Db, oid);
            Subject = m.SupportSubject;
            Body = m.SupportBody;
        }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
