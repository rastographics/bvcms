using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Public.Models.CheckScanAPI;
using CmsWeb.CheckInAPI;
using ImageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;

namespace CmsWeb.Areas.Public.Controllers
{
    public class CheckScanAPIController : Controller
    {
        public ActionResult Exists()
        {
            return Content("1");
        }

        public ActionResult Authenticate()
        {
            CheckScanAuthentication authentication = new CheckScanAuthentication();
            authentication.authenticate();

            if (authentication.hasError())
            {
                return CheckScanMessage.createLoginErrorReturn(authentication);
            }

            User user = authentication.getUser();

            if (!user.InRole("Finance"))
            {
                return CheckScanMessage.createErrorReturn("Finance role is required for check scanning");
            }

            CheckScanMessage response = new CheckScanMessage();
            response.setSuccess();
            response.id = user.PeopleId ?? 0;
            response.data = user.Person.Name;

            return response;
        }

        public ActionResult Funds()
        {
            CheckScanAuthentication authentication = new CheckScanAuthentication();
            authentication.authenticate();

            if (authentication.hasError())
            {
                return CheckScanMessage.createLoginErrorReturn(authentication);
            }

            User user = authentication.getUser();

            if (!user.InRole("Finance"))
            {
                return CheckScanMessage.createErrorReturn("Finance role is required for check scanning");
            }

            var funds = (from f in DbUtil.Db.ContributionFunds
                         where f.FundStatusId == 1
                         orderby f.FundName
                         select new
                         {
                             id = f.FundId,
                             name = f.FundName,
                             description = f.FundDescription
                         }).ToList();

            CheckScanMessage response = new CheckScanMessage();
            response.setSuccess();
            response.data = SerializeJSON(funds);

            return response;
        }

        public ActionResult Bundles()
        {
            CheckScanAuthentication authentication = new CheckScanAuthentication();
            authentication.authenticate();

            if (authentication.hasError())
            {
                return CheckScanMessage.createLoginErrorReturn(authentication);
            }

            User user = authentication.getUser();

            if (!user.InRole("Finance"))
            {
                return CheckScanMessage.createErrorReturn("Finance role is required for check scanning");
            }

            var bundles = (from b in DbUtil.Db.BundleHeaders
                           where b.BundleStatusId == 1
                           orderby b.BundleHeaderId descending
                           select new
                           {
                               id = b.BundleHeaderId,
                               type = b.BundleHeaderType.Description
                           }).ToList();

            CheckScanMessage response = new CheckScanMessage();
            response.setSuccess();
            response.data = SerializeJSON(bundles);

            return response;
        }

        public ActionResult Upload(string data)
        {
            CheckScanAuthentication authentication = new CheckScanAuthentication();
            authentication.authenticate();

            if (authentication.hasError())
            {
                return CheckScanMessage.createLoginErrorReturn(authentication);
            }

            User user = authentication.getUser();

            if (!user.InRole("Finance"))
            {
                return CheckScanMessage.createErrorReturn("Finance role is required for check scanning");
            }

            CheckInMessage message = CheckInMessage.createFromString(data);

            List<CheckScanEntry> entries = JsonConvert.DeserializeObject<List<CheckScanEntry>>(message.data);

            BundleHeader header;

            if (message.id == 0)
            {
                header = new BundleHeader
                {
                    BundleHeaderTypeId = 1,
                    BundleStatusId = BundleStatusCode.Open,
                    CreatedBy = user.UserId,
                    ContributionDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    FundId = DbUtil.Db.Setting("DefaultFundId", "1").ToInt(),
                    RecordStatus = false,
                    TotalCash = 0,
                    TotalChecks = 0,
                    TotalEnvelopes = 0,
                    BundleTotal = 0
                };

                DbUtil.Db.BundleHeaders.InsertOnSubmit(header);
                DbUtil.Db.SubmitChanges();
            }
            else
            {
                header = (from h in DbUtil.Db.BundleHeaders
                          where h.BundleHeaderId == message.id
                          select h).FirstOrDefault();
            }

            CheckScanMessage response = new CheckScanMessage();

            if (header != null)
            {
                foreach (CheckScanEntry entry in entries)
                {
                    Other other = new Other();
                    other.Created = DateTime.Now;
                    other.UserID = user.UserId;

                    if (entry.front.Length > 0)
                    {
                        other.First = Convert.FromBase64String(entry.front);
                    }

                    if (entry.back.Length > 0)
                    {
                        other.Second = Convert.FromBase64String(entry.back);
                    }

                    ImageData.DbUtil.Db.Others.InsertOnSubmit(other);
                    ImageData.DbUtil.Db.SubmitChanges();

                    var detail = new BundleDetail
                    {
                        BundleHeaderId = header.BundleHeaderId,
                        CreatedBy = user.UserId,
                        CreatedDate = DateTime.Now
                    };

                    string bankAccount = entry.routing.Length > 0 && entry.account.Length > 0 ? Util.Encrypt(entry.routing + "|" + entry.account) : "";

                    detail.Contribution = new Contribution
                    {
                        CreatedBy = user.UserId,
                        CreatedDate = detail.CreatedDate,
                        FundId = header.FundId ?? 0,
                        PeopleId = FindPerson(entry.routing, entry.account),
                        ContributionDate = header.ContributionDate,
                        ContributionAmount = decimal.Parse(entry.amount),
                        ContributionStatusId = 0,
                        ContributionTypeId = 1,
                        ContributionDesc = entry.notes,
                        CheckNo = entry.number,
                        BankAccount = bankAccount,
                        ImageID = other.Id
                    };

                    header.BundleDetails.Add(detail);

                    DbUtil.Db.SubmitChanges();
                }

                response.setSuccess();
                response.id = header.BundleHeaderId;
            }
            else
            {
                response.setError(1, "Invalid Bundle ID");
            }

            return response;
        }

        private static int? FindPerson(string routing, string account)
        {
            return (from kc in DbUtil.Db.CardIdentifiers
                    where kc.Id == Util.Encrypt(routing + "|" + account)
                    select kc.PeopleId).SingleOrDefault();
        }

        // ReSharper disable once UnusedParameter.Local
        private static string SerializeJSON(object item)
        {
            return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss" });
        }
    }
}
