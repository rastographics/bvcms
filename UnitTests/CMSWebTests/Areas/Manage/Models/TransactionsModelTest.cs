using CmsData;
using CmsWeb.Models;
using SharedTestFixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CMSWebTests.Areas.Manage.Models
{
    [Collection(Collections.Database)]
    public class TransactionsModelTest : DatabaseTestBase
    {
        [Theory]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(14)]
        public void GetTransactionsByPeopleName(int count)
        {
            var currentPerson = MockPeople.CreateSavePerson(db);
            List<Transaction> transactionList = new List<Transaction>();
            for(var i = 0; i < count; i++)
            {
                var transaction = MockTransactions.CreateTransaction(db, null, currentPerson.PeopleId);
                transactionList.Add(transaction);
            }
            Assert.Equal(count, transactionList.Count());

            foreach (var item in transactionList)
            {
                MockTransactions.DeleteTransaction(db, (int)item.LoginPeopleId);
            }
        }
    }
}
