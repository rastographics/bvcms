using CmsData;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models
{
    public class RemoteDepositCapture
    {
        public static ActionResult Export(CMSDataContext db, int bundleId, string accountNumber, string service, string token)
        {
            db.LogActivity($"Downloading RDC file for bundle {bundleId}", pid: db.CurrentUser.PeopleId, uid: db.CurrentUser.UserId);
            var client = new RestClient
            {
                BaseUrl = new Uri(service),
                Timeout = 120000,
            };
            var request = new RestRequest("api/export", Method.POST);
            request.AddJsonBody(new { Db = Util.Host, Id = bundleId, AccountNumber = accountNumber ?? "" });
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var fileNameFormat = db.Setting("RemoteDepositFilename", "RemoteDeposit-{host}-{bundleid}.icl");
                var fileDownloadName = CreateFileDownloadName(fileNameFormat, bundleId);
                var ext = fileDownloadName.Split('.').Last();
                return new FileContentResult(response.RawBytes, $"application/x-{ext}")
                {
                    FileDownloadName = fileDownloadName.ToSuitableId()
                };
            }
            else
            {
                var error = JsonConvert.DeserializeObject<RemoteDepositCaptureError>(response.Content);
                var exception = new Exception(error?.Message) { Source = service };
                exception.Data.Add("StackTrace", error?.StackTrace);
                if (error?.Data != null)
                {
                    foreach (var item in error.Data)
                    {
                        exception.Data.Add(item.Key, item.Value);
                    }
                }
                throw exception;
            }
        }

        private static string CreateFileDownloadName(string format, int bundleId)
        {
            var filename = format.Replace("{host}", Util.Host, true)
                                 .Replace("{bundleid}", $"{bundleId}", true)
                                 .Replace("{date", "{0", true);
            if (filename.Contains("{0"))
            {
                filename = string.Format(filename, DateTime.Now);
            }
            return filename;
        }
    }
}
