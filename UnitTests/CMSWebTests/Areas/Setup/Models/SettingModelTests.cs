using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Areas.Setup.Models;
using Shouldly;
using Xunit;

namespace CMSWebTests.Areas.Setup.Models
{
    public class SettingModelTests
    {
        [Fact]
        public void ShouldFindEnumTypeValue()
        {
            var boolean = (int) SettingDataType.Boolean;
            boolean.ShouldBe(1);

            var date = (int)SettingDataType.Date;
            date.ShouldBe(2);
        }

        [Fact]
        public void ShouldInstantiateListProperties()
        {
            var model = new SettingModel();

            model.Settings.ShouldBeOfType<List<SettingMetadatum>>();
            model.Settings.Count().ShouldBe(0);

            model.SettingTypes.ShouldBeOfType<List<SettingTypeModel>>();
            model.SettingTypes.Count().ShouldBe(0);
        }
    }
}
