using CmsData;
using CmsData.Codes;
using CmsWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class BadRegistrationException : Exception
    {
        public BadRegistrationException(string msg) : base(msg) { }
    }

    public partial class OnlineRegModel
    {
        public OnlineRegModel()
        {
            HttpContextFactory.Current.Items["OnlineRegModel"] = this;
            CurrentDatabase = DbUtil.Db;
        }

        public OnlineRegModel(HttpRequestBase req, CMSDataContext db, int? id, bool? testing, string email, bool? login, string source)
            : this()
        {
            CurrentDatabase = db;
            Orgid = id;
            if (req?.Url != null)
            {
                URL = req.Url.OriginalString;
            }

            if (CurrentDatabase.Roles.Any(rr => rr.RoleName == "disabled"))
            {
                throw new Exception("Site is disabled for maintenance, check back later");
            }

            if (!id.HasValue)
            {
                throw new BadRegistrationException("no organization");
            }

            MobileAppMenuController.Source = source;

            if (org == null && masterorg == null)
            {
                throw new BadRegistrationException("invalid registration");
            }

            if (masterorg != null)
            {
                if (!UserSelectClasses(masterorg).Any())
                {
                    throw new Exception("no classes available on this org");
                }
            }
            else if (org != null)
            {
                if ((org.RegistrationTypeId ?? 0) == RegistrationTypeCode.None)
                {
                    throw new BadRegistrationException("no registration allowed on this org");
                }
            }
            this.testing = testing == true || DbUtil.Db.Setting("OnlineRegTesting", Util.IsDebug() ? "true" : "false").ToBool();

            // the email passed in is valid or they did not specify login
            if (AllowAnonymous && (Util.ValidEmail(email) || login != true))
            {
                nologin = true;
            }

            if (nologin)
            {
                CreateAnonymousList();
            }
            else
            {
                List = new List<OnlineRegPersonModel>();
            }

            // prepopulate their email address they passed in
            if (Util.ValidEmail(email))
            {
                List[0].EmailAddress = email;
            }

            HistoryAdd("index");
            UpdateDatum();
        }
        public void CreateAnonymousList()
        {
            List = new List<OnlineRegPersonModel>
                {
                    new OnlineRegPersonModel
                        {
                            orgid = Orgid,
                            masterorgid = masterorgid,
#if DEBUG2
                            FirstName = "David",
                            LastName = "Carroll" + DateTime.Now.Millisecond,
                            DateOfBirth = "5/30/52",
                            EmailAddress = "sombody@nowhere.com",
#endif
                        }
                };
        }
        public static string GetDescriptionForPayment(int? id)
        {
            try
            {
                var m = new OnlineRegModel(null, DbUtil.Db, id, false, null, null, null);
                return m.DescriptionForPayment;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
