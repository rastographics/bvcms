using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Areas.Dialog.Models;
using System.Collections.Generic;
using CmsWeb.Areas.OnlineReg.Models;
using UtilityExtensions;
using System.Linq;
using Xunit;
using SharedTestFixtures;
using CmsWeb.Lifecycle;
using System;

namespace CMSWebTests
{
    [Collection(Collections.Database)]
    public class FakeOrganizationUtils
    {
        public static NewOrganizationModel MakeFakeOrganization(IRequestManager requestManager, Organization Orgconfig = null)
        {
            var controller = new CmsWeb.Areas.Dialog.Controllers.AddOrganizationController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "AddOrganization" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var NewOrganizationIndex = controller.Index();
            var OrgId = ((NewOrganizationModel)((System.Web.Mvc.ViewResultBase)NewOrganizationIndex).Model).OrganizationId;

            var FakeNewOrganizationModel = new NewOrganizationModel();
            Organization FakeOrganization = null;
            if (Orgconfig.IsNull())
            {
                FakeOrganization = new Organization() { OrganizationName = "MockName", RegistrationTitle = "MockTitle", Location = "MockLocation", RegistrationTypeId = RegistrationTypeCode.OnlineGiving };
            }
            else
            {
                FakeOrganization = Orgconfig;
            }

            //Add fake registration settings
            SetFakeSettings(FakeOrganization.RegistrationTypeId, FakeOrganization.OrganizationId);

            FakeNewOrganizationModel.org = FakeOrganization;

            controller.Submit((int)OrgId, FakeNewOrganizationModel);
            return FakeNewOrganizationModel;
        }
        public static Settings SetFakeSettings(int? regType, int orgId)
        {
            if (regType == RegistrationTypeCode.ChooseVolunteerTimes)
            {
                var m = new Settings();
                var timeSlots = new TimeSlots();
                var ts1 = new TimeSlots.TimeSlot() { DayOfWeek = 0, Name = "MockTimeSlot", Time = System.DateTime.Now };
                timeSlots.list.Add(ts1);
                m.TimeSlots = timeSlots;
                m.OrgId = orgId;
                return m;
            }
            return null;
        }
        public static OnlineRegModel GetFakeOnlineRegModel(int OrgId)
        {
            var m = new OnlineRegModel(HttpContextFactory.Current.Request, CMSDataContext.Create(DatabaseFixture.Host), OrgId, null, null, null, null);
            m.UserPeopleId = 1;
            return m;
        }

        public static void DeleteOrg(int OrgId)
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            if (db.Organizations.Any(x => x.OrganizationId == OrgId))
            {
                db.Organizations.First(o=>o.OrganizationId == OrgId)
                    .PurgeOrg(db);
            }
        }
    }
}
