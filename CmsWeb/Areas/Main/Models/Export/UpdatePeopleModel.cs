using System;
using System.Collections.Generic;
using System.Xml;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsWeb.Code;
using NPOI.HSSF.UserModel;
using UtilityExtensions;
using System.Linq;
using CmsData;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.Data.Common;
using System.IO;
using System.Text;

namespace CmsWeb.Models
{
    public class UpdatePeopleModel : ActionResult
    {
        int queryid;
        public UpdatePeopleModel(int QueryId)
        {
            this.queryid = QueryId;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var fs = new FileStream(context.HttpContext.Server.MapPath(
                @"\Content\UpdatePeople.xls"), FileMode.Open, FileAccess.Read);
            var wb = new HSSFWorkbook(fs, true);
            var sheet = wb.GetSheet("Sheet1");
            var r = 1;
            foreach (var p in UpdatePeopleRows())
            {
                var row = sheet.CreateRow(r++);
                var c = 0;
                row.CreateCell(c++).SetCellValue(p.PeopleId);
                row.CreateCell(c++).SetCellValue(p.Title);
                row.CreateCell(c++).SetCellValue(p.First);
                row.CreateCell(c++).SetCellValue(p.GoesBy);
                row.CreateCell(c++).SetCellValue(p.Last);
                row.CreateCell(c++).SetCellValue(p.Suffix);
                row.CreateCell(c++).SetCellValue(p.Email1);
                row.CreateCell(c++).SetCellValue(p.Email2);
                row.CreateCell(c++).SetCellValue(p.Gender);
                if (p.BirthDate.HasValue)
                    row.CreateCell(c++).SetCellValue(p.BirthDate.Value);
                else
                    row.CreateCell(c++, NPOI.SS.UserModel.CellType.BLANK);
                if (p.Anniversary.HasValue)
                    row.CreateCell(c++).SetCellValue(p.Anniversary.Value);
                else
                    row.CreateCell(c++, NPOI.SS.UserModel.CellType.BLANK);
                if (p.Joined.HasValue)
                    row.CreateCell(c++).SetCellValue(p.Joined.Value);
                else
                    row.CreateCell(c++, NPOI.SS.UserModel.CellType.BLANK);
                row.CreateCell(c++).SetCellValue(p.Cell);
                row.CreateCell(c++).SetCellValue(p.Work);
                row.CreateCell(c++).SetCellValue(p.Member);
                if (p.Grade.HasValue)
                    row.CreateCell(c++).SetCellValue(p.Grade.Value);
                else
                    row.CreateCell(c++, NPOI.SS.UserModel.CellType.BLANK);
                row.CreateCell(c++).SetCellValue(p.Marital);
                row.CreateCell(c++).SetCellValue(p.FamilyPos);
                row.CreateCell(c++).SetCellValue(p.AltName);
                row.CreateCell(c++).SetCellValue(p.Campus);
                row.CreateCell(c++).SetCellValue(p.School);
                row.CreateCell(c++).SetCellValue(p.Occupation);
                row.CreateCell(c++).SetCellValue(p.Employer);
                if (p.Deceased.HasValue)
                    row.CreateCell(c++).SetCellValue(p.Deceased.Value);
                else
                    row.CreateCell(c++, NPOI.SS.UserModel.CellType.BLANK);
            }
            var Response = context.HttpContext.Response;
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=UpdatePeople.xls");
            Response.Charset = "";
            wb.Write(Response.OutputStream);
        }
        public IEnumerable<UpdatePeopleItem> UpdatePeopleRows()
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);
            var q = from p in query
                    select new UpdatePeopleItem
                    {
                        PeopleId = p.PeopleId,
                        Title = p.TitleCode,
                        First = p.FirstName,
                        GoesBy = p.NickName,
                        Last = p.LastName,
                        Suffix = p.SuffixCode,
                        Email1 = p.EmailAddress,
                        Email2 = p.EmailAddress2,
                        Gender = p.Gender.Description,
                        BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay).ToDate(),
                        Anniversary = p.WeddingDate,
                        Joined = p.JoinDate,
                        Cell = p.CellPhone.FmtFone(),
                        Work = p.WorkPhone.FmtFone(),
                        Member = p.MemberStatus.Description,
                        Grade = p.Grade,
                        Marital = p.MaritalStatus.Description,
                        FamilyPos = p.FamilyPosition.Description,
                        AltName = p.AltName,
                        Campus = p.Campu.Description,
                        School = p.SchoolOther,
                        Occupation = p.OccupationOther,
                        Employer = p.EmployerOther,
                        Deceased = p.DeceasedDate
                    };
            return q;
        }
        public static void UpdatePeople(string path, string host, int userPeopleId)
        {
            var factory = DbProviderFactories.GetFactory("System.Data.OleDb");
            using (var cn = factory.CreateConnection())
            {
                cn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                                      + path + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
                cn.Open();
                var Db = new CMSDataContext(Util.GetConnectionString(host));
                UpdatePeople(cn, Db, userPeopleId);
            }
        }
        public class UpdatePeopleItem
        {
            public int PeopleId { get; set; }
            public string Title { get; set; }
            public string First { get; set; }
            public string GoesBy { get; set; }
            public string Last { get; set; }
            public string Suffix { get; set; }
            public string Email1 { get; set; }
            public string Email2 { get; set; }
            public string Gender { get; set; }
            public DateTime? BirthDate { get; set; }
            public DateTime? Anniversary { get; set; }
            public DateTime? Joined { get; set; }
            public string Cell { get; set; }
            public string Work { get; set; }
            public string Member { get; set; }
            public int? Grade { get; set; }
            public string Marital { get; set; }
            public string FamilyPos { get; set; }
            public string AltName { get; set; }
            public string Campus { get; set; }
            public string School { get; set; }
            public string Occupation { get; set; }
            public string Employer { get; set; }
            public DateTime? Deceased { get; set; }
        };
        private static void UpdatePeople(DbConnection cn, CMSDataContext Db, int userPeopleId)
        {
            var cv = new CodeValueModel();
            var pcmd = cn.CreateCommand();
            pcmd.CommandText = "select * from [Sheet1$]";
            var rd = pcmd.ExecuteReader();
            while (rd.Read())
            {
                var i = Util.GetAs(rd, typeof(UpdatePeopleItem)) as UpdatePeopleItem;
                var p = Db.LoadPersonById(i.PeopleId);

                var psb = new StringBuilder();

                p.UpdateValue(psb, "TitleCode", i.Title);
                p.UpdateValue(psb, "FirstName", i.First);
                p.UpdateValue(psb, "NickName", i.GoesBy);
                p.UpdateValue(psb, "LastName", i.Last);
                p.UpdateValue(psb, "SuffixCode", i.Suffix);
                p.UpdateValue(psb, "EmailAddress", i.Email1);
                p.UpdateValue(psb, "EmailAddress2", i.Email2);
                p.UpdateValue(psb, "DOB", i.BirthDate.FormatDate());
                p.UpdateValue(psb, "WeddingDate", i.Anniversary);
                p.UpdateValue(psb, "JoinDate", i.Joined);
                p.UpdateValue(psb, "CellPhone", i.Cell.GetDigits());
                p.UpdateValue(psb, "WorkPhone", i.Work.GetDigits());
                p.UpdateValue(psb, "AltName", i.AltName);
                p.UpdateValue(psb, "SchoolOther", i.School);
                p.UpdateValue(psb, "OccupationOther", i.Occupation);
                p.UpdateValue(psb, "EmployerOther", i.Employer);
                p.UpdateValue(psb, "Grade", i.Grade);
                p.UpdateValue(psb, "DeceasedDate", i.Deceased);

                p.UpdateValue(psb, "MemberStatusId", CviOrNull(cv.MemberStatusCodes().SingleOrDefault(c => c.Value == i.Member)) ?? 20);
                p.UpdateValue(psb, "GenderId", CviOrNull(cv.GenderCodes().SingleOrDefault(c => c.Value == i.Gender)) ?? 0);
                p.UpdateValue(psb, "MaritalStatusId", CviOrNull(cv.MaritalStatusCodes().SingleOrDefault(c => c.Value == i.Marital)) ?? 0);
                p.UpdateValue(psb, "PositionInFamilyId", CviOrNull(cv.FamilyPositionCodes().SingleOrDefault(c => c.Value == i.FamilyPos)) ?? 0);
                p.UpdateValue(psb, "CampusId", CviOrNull(cv.AllCampuses().SingleOrDefault(c => c.Value == i.Campus)));
               
                p.LogChanges(Db, psb.ToString());
                Db.SubmitChanges();
            }
        }
        private static int? CviOrNull(CodeValueItem cvi)
        {
            if (cvi == null)
                return null;
            return cvi.Id;
        }
    }
}