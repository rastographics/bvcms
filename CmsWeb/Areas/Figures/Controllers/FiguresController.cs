using CmsData;
using CmsWeb.Areas.Figures.Models;
using CmsWeb.Lifecycle;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Figures.Controllers
{
    public class FiguresController : CMSBaseController
    {
        public List<ProgModel> Programs;
        public ProgViewModel pvm = new ProgViewModel();
        public DivViewModel dvm = new DivViewModel();
        public OrgViewModel ovm = new OrgViewModel();
        public FundViewModel fvm = new FundViewModel();

        public FiguresController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            Progs();
            return View();
        }

        public ActionResult GetMapData(int? progId)
        {
            var test = new GoogleChartsData();
            return Json(test.GetChartData(progId).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChartDisplayView(int[] fundIdsArr)
        {
            var test = new GoogleChartsData();
            var temp = test.GetFundChartData(fundIdsArr).ToList();
            return View(temp);
        }

        public ActionResult AttendanceChartDisplayView(int[] orgIdsArr)
        {
            var test = new GoogleChartsData();
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

        public ActionResult ProgramView()
        {
            if(Programs == null)
            {
                Progs();
            }
            pvm.Proglist.Clear();

            foreach (var pd in Programs)
            {
                pvm.Proglist.Add(pd);
            }

            return View(pvm);
        }

        public ActionResult DivisionView(int? progId)
        {
            if(Programs == null)
            {
                Progs();
            }

            dvm.Divlist.Clear();
            if (progId != null)
            {
                var cd = Programs.Find(p => p.Id == progId);

                if(cd != null)
                {
                    foreach (var spd in cd.DivList)
                    {
                        dvm.Divlist.Add(spd);
                    }
                }
            }

            return View(dvm);
        }

        public ActionResult OrganizationView(int? progId, int? divId)
        {
            ovm.Orglist.Clear();
            if (progId == null || divId == null)
            {
                return View(ovm);
            }

            if (Programs == null)
            {
                Progs();
            }

            var cd = Programs.Find(p => p.Id == progId);
            var spd = cd.DivList.Find(p => p.Id == divId);

            if(spd?.OrgList != null)
            {
                foreach (var cpd in spd.OrgList)
                {
                    ovm.Orglist.Add(cpd);
                }
            }

            return View(ovm);
        }

        public ActionResult FundView()
        {
            fvm.Fundlist.Clear();

            fvm.Fundlist = (from f in CurrentDatabase.ContributionFunds
                            select new FundModel
                            {
                                Id = f.FundId,
                                FundName = f.FundName
                            }).ToList();

            return View(fvm);
        }

        private List<FundModel> Funds()
        {
            var fundList = (from f in CurrentDatabase.ContributionFunds
                            select new FundModel
                            {
                                Id = f.FundId,
                                FundName = f.FundName
                            }).ToList();

            return fundList;
        }

        private void Progs()
        {
            var pList = (from f in CurrentDatabase.Programs
                         select new ProgModel
                         {
                             Id = f.Id,
                             ProgName = f.Name,
                             DivList = Divisions(f.Id)
                         }).ToList();
            Programs = pList;
        }

        private List<DivModel> Divisions(int progId)
        {
            var divisionList = (from f in CurrentDatabase.Divisions
                                where f.ProgId == progId
                                select new DivModel
                                {
                                    Id = f.Id,
                                    DivName = f.Name,
                                    OrgList = Organizations(f.Id)
                                }).ToList();

            return divisionList;
        }

        private List<OrgModel> Organizations(int divId)
        {
            var orgList = (from f in CurrentDatabase.Organizations
                           where f.DivisionId == divId
                           select new OrgModel
                           {
                               Id = f.OrganizationId,
                               OrgName = f.OrganizationName
                           }).ToList();

            return orgList;
        }
    }
}
