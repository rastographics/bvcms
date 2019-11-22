using CmsData;
using CmsData.Codes;
using CmsWeb.Controllers;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;
using CmsWeb.Constants;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class BadRegistrationException : Exception
    {
        public BadRegistrationException(string msg) : base(msg) { }
    }

    public partial class OnlineRegModel : IDbBinder
    {        
        public CMSDataContext CurrentDatabase
        {
            get => _currentDatabase ?? DbUtil.Db;
            set
            {
                _currentDatabase = value;
                Init();
            }
        }

        private void Init()
        {
            foreach(var person in List)
            {
                person.CurrentDatabase = CurrentDatabase;
            }
            HttpContextFactory.Current.Items["OnlineRegModel"] = this;
        }
        private CMSDataContext _currentDatabase;

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public OnlineRegModel() { Init(); }

        public OnlineRegModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }

        public OnlineRegModel(HttpRequestBase req, CMSDataContext db, int? id, bool? testing, string email, bool? login, string source)
            : this(db)
        {
            CurrentDatabase = db;
            Orgid = id;
            if (req?.Url != null)
            {
                URL = req.Url.ToString();
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
            this.testing = testing == true || CurrentDatabase.Setting("OnlineRegTesting", Util.IsDebug() ? "true" : "false").ToBool();

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

            // if logged in and trying a non anonymous online reg, use email for logged in user
            if (!AllowAnonymous && !email.HasValue() && Util.UserEmail.HasValue())
            {
                email = Util.UserEmail;
                ProcessType = PaymentProcessTypes.OnlineRegistration;
            }

            // prepopulate their email address they passed in
            if (Util.ValidEmail(email) && ProcessType == PaymentProcessTypes.OnlineRegistration)
            {
                var person =
                    new OnlineRegPersonModel(CurrentDatabase)
                    {
                        orgid = Orgid,
                        masterorgid = masterorgid,
                        EmailAddress = email
                    };
                List.Add(person);
            }

            HistoryAdd("index");
            UpdateDatum();
        }
        public void CreateAnonymousList()
        {
            List = new List<OnlineRegPersonModel>
                {
                    new OnlineRegPersonModel(CurrentDatabase)
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
        public static string GetDescriptionForPayment(int? id, CMSDataContext db)
        {
            try
            {
                var m = new OnlineRegModel(null, db, id, false, null, null, null);
                return m.DescriptionForPayment;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
