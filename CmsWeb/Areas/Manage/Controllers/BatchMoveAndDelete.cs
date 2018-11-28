using CmsData;
using LumenWorks.Framework.IO.Csv;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [ValidateInput(false)]
    [RouteArea("Manage", AreaPrefix = "BatchMoveAndDelete"), Route("{action}/{id?}")]
    public class BatchMoveAndDelete : CmsStaffAsyncController
    {
        [Authorize(Roles = "Admin")]
        public ActionResult MoveAndDelete()
        {
            return View();
        }

        [HttpPost]
        [AsyncTimeout(600000)]
        public void MoveAndDeleteAsync(string text)
        {
            AsyncManager.OutstandingOperations.Increment();
            var host = Util.Host;
            ThreadPool.QueueUserWorkItem(e =>
            {
                var sb = new StringBuilder();
                sb.Append("<h2>done</h2>\n<p><a href='/'>home</a></p>\n");
                using (var csv = new CsvReader(new StringReader(text), false, '\t'))
                {
                    while (csv.ReadNextRecord())
                    {
                        if (csv.FieldCount != 2)
                        {
                            sb.AppendFormat("expected two ids, row {0}<br/>\n", csv[0]);
                            continue;
                        }

                        var fromid = csv[0].ToInt();
                        var toid = csv[1].ToInt();
                        var Db = DbUtil.Create(host);
                        var p = DbUtil.Db.LoadPersonById(fromid);

                        if (p == null)
                        {
                            sb.AppendFormat("fromid {0} not found<br/>\n", fromid);
                            DbUtil.Db.Dispose();
                            continue;
                        }
                        var tp = DbUtil.Db.LoadPersonById(toid);
                        if (tp == null)
                        {
                            sb.AppendFormat("toid {0} not found<br/>\n", toid);
                            DbUtil.Db.Dispose();
                            continue;
                        }
                        try
                        {
                            p.MovePersonStuff(DbUtil.Db, toid);
                            DbUtil.Db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            sb.AppendFormat("error on move ({0}, {1}): {2}<br/>\n", fromid, toid, ex.Message);
                            DbUtil.Db.Dispose();
                            continue;
                        }
                        try
                        {
                            DbUtil.Db.PurgePerson(fromid);
                            sb.AppendFormat("moved ({0}, {1}) successful<br/>\n", fromid, toid);
                        }
                        catch (Exception ex)
                        {
                            sb.AppendFormat("error on delete ({0}): {1}<br/>\n", fromid, ex.Message);
                        }
                        finally
                        {
                            DbUtil.Db.Dispose();
                        }
                    }
                }
                AsyncManager.Parameters["results"] = sb.ToString();
                AsyncManager.OutstandingOperations.Decrement();
            });
        }

        public ActionResult MoveAndDeleteCompleted(string results)
        {
            return Content(results);
        }

    }
}
