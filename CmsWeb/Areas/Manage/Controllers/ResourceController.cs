using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Models;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Resources"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [Authorize(Roles = "ManageResources")]
    public class ResourceController : CmsStaffController
    {
        [HttpGet]
        [Route("~/Resources")]
        public ActionResult Index()
        {
            var resourceTypes = DbUtil.Db.ResourceTypes.OrderBy(x => x.DisplayOrder).Select(x => new ResourceTypeModel(x)).ToList();

            return View(resourceTypes);
        }

        [HttpGet]
        [Route("~/Resources/{id}")]
        public ActionResult Display(int id)
        {
            var resource = new ResourceModel(DbUtil.Db.Resources.FirstOrDefault(x => x.ResourceId == id));

            return View(resource);
        }

        [Route("~/Resources/Edit/{id}")]
        public ActionResult Edit(int id)
        {
            var resource = new EditResourceModel(DbUtil.Db.Resources.FirstOrDefault(x => x.ResourceId == id));

            return View(resource);
        }

        [HttpPost]
        [Route("~/Resources/Save/{id}"), ValidateInput(false)]
        public ActionResult Save(int id, EditResourceModel model)
        {
            var resource = DbUtil.Db.Resources.FirstOrDefault(x => x.ResourceId == id);

            if (model.OrganizationId.HasValue && model.OrganizationId < 1)
                model.OrganizationId = null;
            if (model.OrganizationTypeId.HasValue && model.OrganizationTypeId < 1)
                model.OrganizationTypeId = null;
            if (model.DivisionId.HasValue && model.DivisionId < 1)
                model.DivisionId = null;
            if (model.CampusId.HasValue && model.CampusId < 1)
                model.CampusId = null;

            resource.Name = model.Name;
            resource.DisplayOrder = model.DisplayOrder;
            resource.OrganizationId = model.OrganizationId;
            resource.OrganizationTypeId = model.OrganizationTypeId;
            resource.DivisionId = model.DivisionId;
            resource.CampusId = model.CampusId;
            resource.MemberTypeIds = model.MemberTypeIds != null ? string.Join(",", model.MemberTypeIds) : string.Empty;
            resource.Description = model.Description;
            resource.ResourceTypeId = model.ResourceTypeId;
            resource.ResourceCategoryId = model.ResourceCategoryId;

            DbUtil.Db.SubmitChanges();

            return Redirect("/Resources/" + resource.ResourceId + "/");
        }

        [Route("~/Resources/Attachments/Edit/{id}")]
        public ActionResult EditAttachment(int id)
        {
            var attachment = DbUtil.Db.ResourceAttachments.FirstOrDefault(x => x.ResourceAttachmentId == id);

            return View(attachment);
        }

        [Route("~/Resources/Attachments/Save/{id}")]
        public ActionResult SaveAttachment(int id, ResourceAttachment ra)
        {
            var attachment = DbUtil.Db.ResourceAttachments.FirstOrDefault(x => x.ResourceAttachmentId == id);

            attachment.Name = ra.Name;
            attachment.DisplayOrder = ra.DisplayOrder;

            DbUtil.Db.SubmitChanges();

            return Redirect("/Resources/" + attachment.ResourceId);
        }

        [Route("~/Resources/{id}/Attachments/New")]
        public ActionResult NewAttachment(int id)
        {
            var attachment = new ResourceAttachment
            {
                ResourceId = id
            };

            return View(attachment);
        }

        [Route("~/Resources/{id}/Attachments/Upload")]
        public ActionResult UploadAttachment(int id, ResourceAttachment attachment, HttpPostedFileBase file)
        {
            attachment.CreationDate = Util.Now;
            attachment.UpdateDate = Util.Now;
            attachment.FilePath = UploadAttachment(file);

            DbUtil.Db.ResourceAttachments.InsertOnSubmit(attachment);
            DbUtil.Db.SubmitChanges();

            return Redirect("/Resources/" + attachment.ResourceId);
        }

        [Route("~/Resources/Delete/{id}")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var resource = DbUtil.Db.Resources.FirstOrDefault(x => x.ResourceId == id);

            if (resource != null)
            {
                DbUtil.Db.ResourceAttachments.DeleteAllOnSubmit(resource.ResourceAttachments);
                DbUtil.Db.Resources.DeleteOnSubmit(resource);
                DbUtil.Db.SubmitChanges();
            }

            return Content("/Resources/");
        }

        [Route("~/Resources/Attachments/Delete/{id}")]
        [HttpPost]
        public ActionResult DeleteAttachment(int id)
        {
            var attachment = DbUtil.Db.ResourceAttachments.FirstOrDefault(x => x.ResourceAttachmentId == id);

            if (attachment != null)
            {
                DbUtil.Db.ResourceAttachments.DeleteOnSubmit(attachment);
                DbUtil.Db.SubmitChanges();
            }

            return Content("/Resources/"+id);
        }

        public string UploadAttachment(HttpPostedFileBase file)
        {
            var m = new AccountModel();
            string baseurl = null;

            var fn = $"{DbUtil.Db.Host}.{DateTime.Now:yyMMddHHmm}.{m.CleanFileName(Path.GetFileName(file.FileName))}";
            var error = string.Empty;

            var rackspacecdn = DbUtil.Db.Setting("RackspaceUrlCDN", null);
            string username;
            string key;
            if (string.IsNullOrEmpty(rackspacecdn))
            {
                rackspacecdn = ConfigurationManager.AppSettings["RackspaceUrlCDN"];
                username = ConfigurationManager.AppSettings["RackspaceUser"];
                key = ConfigurationManager.AppSettings["RackspaceKey"];
            }
            else
            {
                username = DbUtil.Db.Setting("RackspaceUser", null);
                key = DbUtil.Db.Setting("RackspaceKey", null);
            }

            if (rackspacecdn.HasValue())
            {
                baseurl = rackspacecdn;
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
