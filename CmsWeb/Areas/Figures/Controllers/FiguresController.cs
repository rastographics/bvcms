using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Controllers;
using CmsWeb.Areas.Figures.Models;
using IronPython.Modules;

namespace CmsWeb.Areas.Figures.Controllers
{
    public class FiguresController : Controller
    { 
        // GET: Figures/Figures
        public ActionResult Index()
        {
            Progs();
            return View();
        }

        public ActionResult GetMapData(int? progId)
        {
            GoogleChartsData test = new GoogleChartsData();
            //List MapData = new List();
            //MapData=test.GetChartData();

            //make datatable
           // GoogleChartsDataTable MapData = new GoogleChartsDataTable();
            return Json(test.GetChartData(progId).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChartDisplayView(int[] fundIdsArr)
        {
            GoogleChartsData test = new GoogleChartsData();
            var temp = test.GetFundChartData(fundIdsArr).ToList();
            return View(temp);
        }
        

        public ActionResult AttendanceChartDisplayView(int[] orgIdsArr)
        {;
            GoogleChartsData test = new GoogleChartsData();
            var temp = test.GetAttendanceChartData(orgIdsArr).ToList();
            return View(temp);
        }

        public ActionResult RefineAttendanceView()
        {
            Progs();
            return View();
        }

        public ActionResult RefineFundView()
        {            
            return View();
        }

        public List<FundModel> Funds()
        {
            List<FundModel> fundList = (from f in DbUtil.Db.ContributionFunds
                select new FundModel
                {
                    Id = f.FundId,
                    FundName = f.FundName
                }).ToList();

            return fundList;
        }

        public static List<ProgModel> Programs;

        public static void Progs()
        {
            List<ProgModel> pList = (from f in DbUtil.Db.Programs
             select new ProgModel
             {
                 Id = f.Id,
                 ProgName = f.Name,
                 DivList = Divisions(f.Id)
             }).ToList();
            Programs = pList;
        }

        public static List<DivModel> Divisions(int progId)
        {
            List<DivModel> divisionList = (from f in DbUtil.Db.Divisions
                                           where f.ProgId == progId
                                            select new DivModel
                                            {
                                                Id = f.Id,
                                                DivName = f.Name,
                                                OrgList = Organizations(f.Id)
                                            }).ToList();

            return divisionList;
        }

        public static List<OrgModel> Organizations(int divId)
        {
            List<OrgModel>  orgList = (from f in DbUtil.Db.Organizations
                                        where f.DivisionId == divId
                                        select new OrgModel()
                                        {
                                            Id = f.OrganizationId,
                                            OrgName = f.OrganizationName
                                        }).ToList();

            return orgList;
        }

        public static ProgViewModel pvm = new ProgViewModel();

        public ActionResult ProgramView()
        {
            pvm.Proglist.Clear();

            foreach (ProgModel pd in Programs)
            {
                pvm.Proglist.Add(pd);
            }
            return View(pvm);
        }

        public static DivViewModel dvm = new DivViewModel();

        public ActionResult DivisionView(int? progId)
        {
            dvm.Divlist.Clear();
            if (progId != null)
            {
                ProgModel cd = Programs.Find(p => p.Id == progId);

                foreach (DivModel spd in cd.DivList)
                {
                    dvm.Divlist.Add(spd);
                }
            }
            return View(dvm);
        }

        public static OrgViewModel ovm = new OrgViewModel();
        public ActionResult OrganizationView(int? progId, int? divId)
        {
            ovm.Orglist.Clear();
            if (progId != null && divId != null)
            {
                ProgModel cd = Programs.Find(p => p.Id == progId);
                DivModel spd = cd.DivList.Find(p => p.Id == divId);

                foreach (OrgModel cpd in spd.OrgList)
                {
                    ovm.Orglist.Add(cpd);
                }
            }
            return View(ovm);
        }

        public static FundViewModel fvm = new FundViewModel();

        public ActionResult FundView()
        {
            fvm.Fundlist.Clear();

            fvm.Fundlist = (from f in DbUtil.Db.ContributionFunds
                select new FundModel
                {
                    Id = f.FundId,
                    FundName = f.FundName
                }).ToList();

            return View(fvm);
        }

    }
}
