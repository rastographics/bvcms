using CmsData;
using CmsWeb.Areas.Dialog.Models;
using System.Collections.Generic;
using CmsWeb.Areas.OnlineReg.Models;
using UtilityExtensions;
using System.Linq;

namespace CMSWebTests
{
    public class FakeOrganizationUtils
    {
        public static NewOrganizationModel FakeNewOrganizationModel { get; set; }
        public static Organization FakeOrganization { get; set; }
        public static int? OrgId { get; set; }

        public static NewOrganizationModel MakeFakeOrganization()
        {
            if (FakeNewOrganizationModel == null)
            {
                var controller = new CmsWeb.Areas.Dialog.Controllers.AddOrganizationController(FakeRequestManager.FakeRequest());
                var routeDataValues = new Dictionary<string, string> { { "controller", "AddOrganization" } };
                controller.ControllerContext = ControllerTestUtils.FakeContextController(controller, routeDataValues);

                var NewOrganizationIndex = controller.Index();
                OrgId = ((NewOrganizationModel)((System.Web.Mvc.ViewResultBase)NewOrganizationIndex).Model).OrganizationId;

                FakeNewOrganizationModel = new NewOrganizationModel();
                FakeOrganization = new Organization() { OrganizationName = "MockName", RegistrationTitle = "MockTitle", Location = "MockLocation", RegistrationTypeId = 8 };

                FakeNewOrganizationModel.org = FakeOrganization;

                controller.Submit((int)OrgId, FakeNewOrganizationModel);
            }
            else if (DbUtil.Db.Organizations.Where(x => x.OrganizationId == OrgId).IsNull())
            {
                FakeNewOrganizationModel = null;
                MakeFakeOrganization();
            }
            return FakeNewOrganizationModel;
        }

        public static OnlineRegModel GetFakeOnlineRegModel(int OrgId)
        {
            var m = new OnlineRegModel(ContextTestUtils.FakeHttpContext().Request, CMSDataContext.Create(Util.Host), OrgId, null, null, null, null);
            m.UserPeopleId = 1;
            return m;
        }

        public static void DeleteOrg(int OrgId)
        {
            var controller = new CmsWeb.Areas.Org.Controllers.OrgController(FakeRequestManager.FakeRequest());
            var routeDataValues = new Dictionary<string, string> { { "controller", "Org" } };
            controller.ControllerContext = ControllerTestUtils.FakeContextController(controller, routeDataValues);

            controller.Delete(OrgId);
        }
    }
}
