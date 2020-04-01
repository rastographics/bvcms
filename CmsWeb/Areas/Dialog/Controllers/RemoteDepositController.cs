using CmsData.Codes;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Finance.Models;
using CmsWeb.Common;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "RemoteDeposit"), Route("{action}/{id?}")]
    public class RemoteDepositController : CmsStaffController
    {
        public RemoteDepositController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/RemoteDeposit/{id:int}")]
        public ActionResult Index(int id)
        {
            if (GetServiceConfiguration(out string service, out string token))
            {
                return View(CurrentDatabase.BundleHeaders.First(h => h.BundleHeaderId == id));
            }
            return View("NotConfigured");
        }

        [HttpPost]
        public ActionResult Process(RemoteDeposit model)
        {
            if (GetServiceConfiguration(out string service, out string token))
            {
                var id = model.BundleHeaderId;
                var depositDate = model.DepositDate;
                var accountNumber = model.AccountNumber;
                if (depositDate.HasValue) {
                    var bundle = CurrentDatabase.BundleHeaders.First(h => h.BundleHeaderId == id);
                    bundle.DepositDate = depositDate;
                    CurrentDatabase.SubmitChanges();
                }
                return RemoteDepositCapture.Export(CurrentDatabase, id, accountNumber, service, token);
            }
            return RedirectShowError("The remote deposit capture service is not configured");
        }

        private bool GetServiceConfiguration(out string service, out string token)
        {
            service = Configuration.Current.RemoteDepositCaptureService;
            token = Configuration.Current.RemoteDepositCaptureServiceToken;
            var spec = CurrentDatabase.Contents.Any(x => x.Name == "X9Specification" && x.TypeID == ContentTypeCode.TypeText);
            return spec && service.HasValue() && token.HasValue();
        }
    }
}
