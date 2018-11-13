using System;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public void PrepareMissionTrip(int? gsid, int? goerid)
        {
            if (gsid.HasValue) // this means that the person is a suppoter who got a support email
            {
                var goerSupporter = CurrentDatabase.GoerSupporters.SingleOrDefault(gg => gg.Id == gsid); // used for mission trips
                if (goerSupporter != null)
                {
                    GoerId = goerSupporter.GoerId; // suppoert this particular goer
                    GoerSupporterId = gsid;
                }
                else
                    GoerId = 0; // allow this supporter to still select a goer
            }
            else if (goerid.HasValue)
            {
                GoerId = goerid;
            }
        }

        public int CheckRegisterLink(string regtag)
        {
            var pid = 0;
            if (regtag.HasValue())
            {
                var guid = regtag.ToGuid();
                if (guid == null)
                    throw new Exception("invalid link");
                var ot = CurrentDatabase.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                if (ot == null)
                    throw new Exception("invalid link");
#if DEBUG
#else
                if (ot.Used)
                    throw new Exception("link used");
                if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                    throw new Exception("link expired");
#endif

                registertag = regtag;
                if (registertag.HasValue() && !registerLinkType.HasValue())
                    registerLinkType = "registerlink";

                var a = ot.Querystring.Split(',');
                if(a.Length >= 4)
                    registerLinkType = a[3];
                pid = a[1].ToInt();
            }

            // handle if they are already logged in
            else if (HttpContext.Current.User.Identity.IsAuthenticated)
                pid = Util.UserPeopleId ?? 0;

            if (pid > 0)
            {
                UserPeopleId = pid;
//                Util.UserPeopleId = pid;
            }
            return pid;
        }

    }
}