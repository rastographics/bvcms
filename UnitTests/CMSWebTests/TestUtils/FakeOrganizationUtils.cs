using CmsData;
using ImageData;
using System;
using System.Security.Principal;
using System.Web;
using CmsWeb.Lifecycle;
using CmsWeb.Areas.Dialog.Models;
using System.Collections.Generic;
using CmsWeb.Areas.OnlineReg.Models;

namespace CMSWebTests
{
    public class FakeOrganizationUtils
    {
        public static NewOrganizationModel FakeNewOrganizationModel { get; set; }
        public static Organization FakeOrganization { get; set; }

        public static NewOrganizationModel MakeFakeOrganization()
        {
            if(FakeNewOrganizationModel == null)
            {
                var controller = new CmsWeb.Areas.Dialog.Controllers.AddOrganizationController(FakeRequestManager.FakeRequest());
                var routeDataValues = new Dictionary<string, string> { { "controller", "AddOrganization" } };
                controller.ControllerContext = ControllerTestUtils.FakeContextController(controller, routeDataValues);

                var NewOrganizationIndex = controller.Index();
                var OrgId = ((CmsWeb.Areas.Dialog.Models.NewOrganizationModel)((System.Web.Mvc.ViewResultBase)NewOrganizationIndex).Model).OrganizationId;

                FakeNewOrganizationModel = new NewOrganizationModel();
                FakeOrganization = new Organization() { OrganizationName = "MockName", RegistrationTitle = "MockTitle", Location = "MockLocation", RegistrationTypeId = 8 };

                FakeNewOrganizationModel.org = FakeOrganization;

                controller.Submit((int)OrgId, FakeNewOrganizationModel);
            }
            return FakeNewOrganizationModel;
        }

        public static OnlineRegModel GetFakeOnlineRegModel(int OrgId)
        {
            var m = new OnlineRegModel(ContexTestUtils.FakeHttpContext().Request, ContexTestUtils.CurrentDatabase(), OrgId, null, null, null, null);
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
