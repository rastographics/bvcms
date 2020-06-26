using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTestFixtures
{
    public class MockCampus
    {
        public static Campu CreateCampus(CMSDataContext db, string code = null, string description = null, bool? hardwired = null)
        {
            int? campusId = null;
            campusId = db.Campus.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
            //try
            //{
            //    campusId = db.Campus.Max(c => c.Id) + 1;
            //}
            //catch
            //{
            //    campusId = 1;
            //}
            var campus = new Campu()
            {
                Id = (int)campusId,
                Code = code ?? DatabaseTestBase.RandomString(),
                Description = description ?? DatabaseTestBase.RandomString(),
                Hardwired = hardwired
            };

            db.Campus.InsertOnSubmit(campus);
            db.SubmitChanges();

            return campus;
        }
        public static void DeleteCampus(CMSDataContext db, Campu campus)
        {
            db.Campus.DeleteOnSubmit(campus);
            db.SubmitChanges();
        }
    }
}
