using CmsData;
using Elmah;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models
{
    public class RemoteDepositCapture
    {
        public static ActionResult Export (CMSDataContext db, int bundleId, string service, string token)
        {
            db.LogActivity($"Downloading RDC file for bundle {bundleId}", pid: db.CurrentUser.PeopleId, uid: db.CurrentUser.UserId);
            var client = new RestClient
            {
                BaseUrl = new Uri(service),
                Timeout = 120000,
            };
            var request = new RestRequest("api/export", Method.POST);
            request.AddJsonBody(new { Db = Util.Host, Id = bundleId });
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new FileContentResult(response.RawBytes, "application/x-icl")
                {
                    FileDownloadName = $"RemoteDeposit-{Util.Host}-{bundleId}.icl"
                };
            }
            else
            {
                var error = JsonConvert.DeserializeObject<RemoteDepositCaptureError>(response.Content);
                var exception = new Exception(error.Message) { Source = service };
                exception.Data.Add("StackTrace", error.StackTrace);
                foreach (var item in error.Data)
                {
                    exception.Data.Add(item.Key, item.Value);
                }
                ErrorSignal.FromCurrentContext().Raise(exception);
                return new RedirectResult(CmsStaffController.ErrorUrl(error.Message));
            }
        }
    }
}
