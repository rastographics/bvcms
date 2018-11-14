using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Resources"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [Authorize(Roles = "ManageResources")]
    public class ResourceController : CmsStaffController
    {
        public ResourceController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        [Route("~/Resources")]
        public ActionResult Index()
        {
            var resourceTypes = CurrentDatabase.ResourceTypes.OrderBy(x => x.DisplayOrder).Select(x => new ResourceTypeModel(x)).ToList();

            return View(resourceTypes);
        }

        [HttpGet]
        [Route("~/Resources/{id}")]
        public ActionResult Display(int id)
        {
            var resource = new ResourceModel(CurrentDatabase.Resources.FirstOrDefault(x => x.ResourceId == id));

            return View(resource);
        }

        [Route("~/Resources/Edit/{id}")]
        public ActionResult Edit(int id)
        {
            var resource = new EditResourceModel(CurrentDatabase.Resources.FirstOrDefault(x => x.ResourceId == id));

            return View(resource);
        }

        [HttpPost]
        [Route("~/Resources/Save/{id}"), ValidateInput(false)]
        public ActionResult Save(int id, EditResourceModel model)
        {
            var resource = CurrentDatabase.Resources.FirstOrDefault(x => x.ResourceId == id);

            if (model.DivisionId.HasValue && model.DivisionId < 1)
            {
                model.DivisionId = null;
            }

            if (model.CampusId.HasValue && model.CampusId < 1)
            {
                model.CampusId = null;
            }

            resource.Name = model.Name;
            resource.DisplayOrder = model.DisplayOrder;
            resource.DivisionId = model.DivisionId;
            resource.CampusId = model.CampusId;
            resource.MemberTypeIds = model.MemberTypeIds != null ? string.Join(",", model.MemberTypeIds) : string.Empty;
            resource.StatusFlagIds = model.StatusFlagIds != null ? string.Join(",", model.StatusFlagIds) : string.Empty;
            resource.Description = model.Description;
            resource.ResourceTypeId = model.ResourceTypeId;
            resource.ResourceCategoryId = model.ResourceCategoryId;

            CurrentDatabase.ResourceOrganizations.DeleteAllOnSubmit(
                CurrentDatabase.ResourceOrganizations.Where(ro => ro.ResourceId == resource.ResourceId)
                );
            foreach (var orgId in model.OrganizationIds)
            {
                resource.ResourceOrganizations.Add(new ResourceOrganization
                {
                    Resource = resource,
                    OrganizationId = orgId
                });
            }

            CurrentDatabase.ResourceOrganizationTypes.DeleteAllOnSubmit(
                CurrentDatabase.ResourceOrganizationTypes.Where(ro => ro.ResourceId == resource.ResourceId)
                );
            foreach (var orgTypeId in model.OrganizationTypeIds)
            {
                resource.ResourceOrganizationTypes.Add(new ResourceOrganizationType
                {
                    Resource = resource,
                    OrganizationTypeId = orgTypeId
                });
            }

            CurrentDatabase.SubmitChanges();

            return Redirect("/Resources/" + resource.ResourceId + "/");
        }

        [Route("~/Resources/Attachments/Edit/{id}")]
        public ActionResult EditAttachment(int id)
        {
            var attachment = CurrentDatabase.ResourceAttachments.FirstOrDefault(x => x.ResourceAttachmentId == id);

            return View(attachment);
        }

        [Route("~/Resources/Attachments/Save/{id}")]
        public ActionResult SaveAttachment(int id, ResourceAttachment ra)
        {
            var attachment = CurrentDatabase.ResourceAttachments.FirstOrDefault(x => x.ResourceAttachmentId == id);

            attachment.Name = ra.Name;
            attachment.DisplayOrder = ra.DisplayOrder;

            CurrentDatabase.SubmitChanges();

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

            CurrentDatabase.ResourceAttachments.InsertOnSubmit(attachment);
            CurrentDatabase.SubmitChanges();

            return Redirect("/Resources/" + attachment.ResourceId);
        }

        [Route("~/Resources/Delete/{id}")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var resource = CurrentDatabase.Resources.FirstOrDefault(x => x.ResourceId == id);

            if (resource != null)
            {
                CurrentDatabase.ResourceAttachments.DeleteAllOnSubmit(resource.ResourceAttachments);
                CurrentDatabase.ResourceOrganizationTypes.DeleteAllOnSubmit(resource.ResourceOrganizationTypes);
                CurrentDatabase.ResourceOrganizations.DeleteAllOnSubmit(resource.ResourceOrganizations);
                CurrentDatabase.Resources.DeleteOnSubmit(resource);
                CurrentDatabase.SubmitChanges();
            }

            return Content("/Resources/");
        }

        [Route("~/Resources/Attachments/Delete/{id}")]
        [HttpPost]
        public ActionResult DeleteAttachment(int id)
        {
            var attachment = CurrentDatabase.ResourceAttachments.FirstOrDefault(x => x.ResourceAttachmentId == id);
            var resourceId = attachment.ResourceId;

            if (attachment != null)
            {
                CurrentDatabase.ResourceAttachments.DeleteOnSubmit(attachment);
                CurrentDatabase.SubmitChanges();
            }

            return Content("/Resources/" + resourceId);
        }

        public string UploadAttachment(HttpPostedFileBase file)
        {
            var m = new AccountModel();
            string baseurl = null;

            var fn = $"{CurrentDatabase.Host}.{DateTime.Now:yyMMddHHmm}.{m.CleanFileName(Path.GetFileName(file.FileName))}";
            var error = string.Empty;

            var rackspacecdn = CurrentDatabase.Setting("RackspaceUrlCDN", null);
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
                username = CurrentDatabase.Setting("RackspaceUser", null);
                key = CurrentDatabase.Setting("RackspaceKey", null);
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

            return Util.URLCombine(baseurl, fn);
        }
    }
}
