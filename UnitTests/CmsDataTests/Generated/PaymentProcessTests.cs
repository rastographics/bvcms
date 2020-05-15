using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;

namespace CmsDataTests.Generated
{
    [Collection(Collections.Database)]
    public class PaymentProcessTests : DatabaseTestBase
    {
        [Fact]
        public void PaymentProcess_UpdateTest()
        {
            db.PaymentProcess.Count().ShouldBe(4);
            var process = db.PaymentProcess.First(p => p.ProcessId == 1);
            process.ProcessName.ShouldBe("One-Time Giving");
            process.AcceptACH = true;
            process.AcceptCredit = false;
            db.SubmitChanges();

            var newdb = db.Copy();
            var actual = newdb.PaymentProcess.First(p => p.ProcessId == 1);
            actual.ShouldBeEquivalentTo(process);
        }
    }
}
