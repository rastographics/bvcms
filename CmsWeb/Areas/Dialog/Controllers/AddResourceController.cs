using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Models;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [Authorize(Roles="Edit")]
    [RouteArea("Dialog", AreaPrefix = "AddResource")]
    public class AddResourceController : CmsStaffController
    {
        [Route("~/AddResource")]
        public ActionResult Index(bool displayCopySettings = false)
        {
            return View(new NewResourceModel());
        }

        [HttpPost, Route("Submit/{id:int}")]
        public ActionResult Submit(int id, NewResourceModel m, IEnumerable<HttpPostedFileBase> files)
        {
            var resource = new Resource
            {
                CreationDate = Util.Now,
                Description = m.Description,
                MemberTypeIds = m.MemberTypeIds,
                DivisionId = m.DivisionId,
                CampusId = m.CampusId,
                Name = m.Name,
                DisplayOrder = m.DisplayOrder,
                OrganizationId = m.OrganizationId
            };

            if (resource.CampusId.HasValue && resource.CampusId < 1) resource.CampusId = null;
            if (resource.DivisionId.HasValue && resource.DivisionId < 1) resource.DivisionId = null;
            if (resource.OrganizationId.HasValue && resource.OrganizationId < 1) resource.OrganizationId = null;

            DbUtil.Db.Resources.InsertOnSubmit(resource);
            DbUtil.Db.SubmitChanges();

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    if (file == null) continue;

                    var attachment = new ResourceAttachment
                    {
                        ResourceId = resource.ResourceId,
                        FilePath = UploadAttachment(file),
                        Name = file.FileName,
                        CreationDate = Util.Now
                    };

                    DbUtil.Db.ResourceAttachments.InsertOnSubmit(attachment);
                    DbUtil.Db.SubmitChanges();
                }
            }

            return Redirect("/Resources");
        }

        public string UploadAttachment(HttpPostedFileBase file)
        {
            var m = new AccountModel();
            string baseurl = null;

            var fn = $"{DbUtil.Db.Host}.{DateTime.Now:yyMMddHHmm}.{m.CleanFileName(Path.GetFileName(file.FileName))}";
            var error = string.Empty;
            var rackspacecdn = ConfigurationManager.AppSettings["RackspaceUrlCDN"];

            if (rackspacecdn.HasValue())
            {
                baseurl = rackspacecdn;
                var username = ConfigurationManager.AppSettings["RackspaceUser"];
                var key = ConfigurationManager.AppSettings["RackspaceKey"];
                var cloudIdentity = new CloudIdentity { APIKey = key, Username = username };
                var cloudFilesProvider = new CloudFilesProvider(cloudIdentity);
                cloudFilesProvider.CreateObject("AllFiles", file.InputStream, fn);
            }
            else // local server
            {
                baseurl = $"{Request.Url.Scheme}://{Request.Url.Authority}/Upload/";
                try
                {
                    var path = Server.MapPath("/Upload/");
                    path += fn;

                    path = m.GetNewFileName(path);
                    file.SaveAs(path);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    baseurl = string.Empty;
                }
            }

            return baseurl + fn;
        }
    }
}
