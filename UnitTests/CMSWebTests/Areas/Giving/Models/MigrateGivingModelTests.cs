using Xunit;
using CmsWeb.Areas.Giving.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTestFixtures;
using Shouldly;

namespace CmsWeb.Areas.Giving.Models.Tests
{
    public class MigrateGivingModelTests : DatabaseTestBase
    {
        public MigrateGivingModelTests() : base()
        {
            MockAppSettings.Apply(
                ("PublicKey", "mytest"),
                ("PublicSalt", "66 82 79 78 66 82 79 78")
            );
        }

        [Fact]
        public void MigrateTest()
        {
            MigrateGivingModel.Migrate(db, out string error);

            error.ShouldBeEmpty();
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
