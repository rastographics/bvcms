using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockTransactions
    {
        public static Transaction CreateTransaction(CMSDataContext db, string name = null, int? peopleId = null)
        {
            if(name == null)
            {
                var transaction = new Transaction
                {
                    Id = DatabaseTestBase.RandomNumber(),
                    LoginPeopleId = peopleId
                };
                db.Transactions.InsertOnSubmit(transaction);
                db.SubmitChanges();
                return transaction;
            }
            else
            {
                var transaction = new Transaction
                {
                    Id = DatabaseTestBase.RandomNumber(),
                    LoginPeopleId = peopleId
                };
                db.Transactions.InsertOnSubmit(transaction);
                db.SubmitChanges();
                return transaction;
            }
        }

        public static void DeleteTransaction(CMSDataContext db, int peopleId)
        {
            var transaction = db.Transactions.Where(b => b.LoginPeopleId == peopleId).FirstOrDefault();
            db.Transactions.DeleteOnSubmit(transaction);
            db.SubmitChanges();
        }
    }
}
