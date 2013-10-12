using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using CmsData;
using CmsData.API;
using UtilityExtensions;
using System.IO;

namespace CmsWeb.Areas.Finance.Models.Report
{
	public class ContributionStatementsExtract
	{
		public DateTime fd { get; set; }
		public DateTime td { get; set; }
		public bool PDF { get; set; }
		public string OutputFile { get; set; }
		public string Host { get; set; }
		public int LastSet { get; set; }
		public string StartsWith { get; set; }
		public string Sort { get; set; }

        // For extended report
        public bool showCheckNo { get; set; }
        public bool showNotes { get; set; }

		public ContributionStatementsExtract(string Host, DateTime fd, DateTime td, bool PDF, string OutputFile, string startswith = null, string sort = null)
		{
			this.fd = fd;
			this.td = td;
			this.PDF = PDF;
			this.Host = Host;
			this.OutputFile = OutputFile;
		    StartsWith = startswith;
		    Sort = sort;
		}

		public CMSDataContext Db { get; set; }
		public void DoWork()
		{
			Db = new CMSDataContext(Util.GetConnectionString(Host));
			Db.Host = Host;
			Db.CommandTimeout = 1200;

			var noaddressok = Db.Setting("RequireAddressOnStatement", "true") == "false";
            showCheckNo = Db.Setting("RequireCheckNoOnStatement", "false").ToLower() == "true";
            showNotes = Db.Setting("RequireNotesOnStatement", "false").ToLower() == "true";

		    var qc = APIContribution.contributors(Db, fd, td, 0, 0, 0, noaddressok, useMinAmt: true, startswith: StartsWith, sort: Sort);
			var runningtotals = Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
			runningtotals.Count = qc.Count();
			Db.SubmitChanges();
			if (PDF)
			{
                if (showCheckNo || showNotes)
                {
                    var c = new ContributionStatementsExtra
                    {
                        FromDate = fd,
                        ToDate = td,
                        typ = 3,
                        ShowCheckNo = showCheckNo,
                        ShowNotes = showNotes
                    };
                    using (var stream = new FileStream(OutputFile, FileMode.Create))
                        c.Run(stream, Db, qc);
                    LastSet = c.LastSet();
                    var sets = c.Sets();
                    foreach (var set in sets)
                        using (var stream = new FileStream(Output(OutputFile, set), FileMode.Create))
                            c.Run(stream, Db, qc, set);
                    runningtotals = Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
                    runningtotals.LastSet = LastSet;
                    runningtotals.Sets = string.Join(",", sets);
                    runningtotals.Completed = DateTime.Now;
                    Db.SubmitChanges();
                }
                else
                {
                    var c = new ContributionStatements
                    {
                        FromDate = fd,
                        ToDate = td,
                        typ = 3
                    };
                    using (var stream = new FileStream(OutputFile, FileMode.Create))
                        c.Run(stream, Db, qc);
                    LastSet = c.LastSet();
                    var sets = c.Sets();
                    foreach (var set in sets)
                        using (var stream = new FileStream(Output(OutputFile, set), FileMode.Create))
                            c.Run(stream, Db, qc, set);
                    runningtotals = Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
                    runningtotals.LastSet = LastSet;
                    runningtotals.Sets = string.Join(",", sets);
                    runningtotals.Completed = DateTime.Now;
                    Db.SubmitChanges();
                }
			}
			else
			{
				textStream = new StreamWriter(OutputFile);
				foreach (var c in qc)
				{
					pageStatement = 1;
					writeHeader(c);
					writeContributions(c);
					string hdrGift = "   Date        Fund Name          Description of Gift-in-Kind Given as of {0:d}\n\n".Fmt(td);
					rWrite(hdrGift);
					writeSummary(c);
					runningtotals.Processed += 1;
					Db.SubmitChanges();
				}
				textStream.Close();
				runningtotals = Db.ContributionsRuns.OrderByDescending(mm => mm.Id).First();
				runningtotals.Completed = DateTime.Now;
				Db.SubmitChanges();
			}
		}
		public static string Output(string fn, int set)
		{
			var outf = fn.Replace(".pdf", "-{0}.pdf".Fmt(set));
			return outf;
		}

		private StreamWriter textStream;
		private void rWrite(string s)
		{
			s = s.Replace("\n", "\r\n");
			textStream.Write(s);
		}
		private void rWrite(char c)
		{
			if (c == '\n')
				textStream.Write("\r");
			textStream.Write(c);
		}
		private int pageStatement = 0;
		private string dateLine = "Date: " + Util.Now.Date.ToString("MMM dd yyyy") + "\n";
		private string totalLine = "                                               TOTAL";
		private string churchAddr = "\nBellevue Baptist Church\n" +
									"   2000 Appling Road\n" +
									"   P.O. Box 1210\n" +
									"   Cordova, TN  38016\n" +
									"(901) 347-2000\n\n";

		private string disclaimer = "\n\n" +
									"    NOTE: No goods or services were provided to you by the church in\n" +
									"    connection with any contribution; any value received consisted\n" +
									"    entirely of intangible religious benefits.\n";
		private string nineSpaces = "         ";
		private char newPage = (char)0; // start out as zero, then changes to 12

		private int countRec = 0;


		private void writeHeader(ContributorInfo c)
		{
			var isAddress2 = c.Address2 != null;
			if (newPage > 0)
				rWrite(newPage);
			newPage = (char)12;
			rWrite(dateLine);
			rWrite(churchAddr);
			rWrite(nineSpaces);
			rWrite(c.Name.PadRight(42) + "Envelope: # " + c.PeopleId.ToString() + "\n");
			rWrite(nineSpaces);
			if (pageStatement == 1)
			{
				rWrite(c.Address1 + "\n");
				if (isAddress2)
				{
					rWrite(nineSpaces);
					rWrite(c.Address2);
					rWrite("\n");
				}
				rWrite(nineSpaces);
				rWrite(c.City + ", " + c.State + " " + Util.FmtZip(c.Zip) + "\n");
				if (!isAddress2) rWrite("\n");
				rWrite(disclaimer);
			}
			else
				rWrite("\n" + nineSpaces + "Cont.....Page " + pageStatement.ToString() + "\n".PadRight(6, '\n'));
			rWrite("-------------------------------------------------------------------------------\n\n");
		}

		private void writeContributions(ContributorInfo c)
		{
			string hdrContributions = "   Date        Fund Name          Amount       Date        Fund Name          Amount\n\n";

			rWrite(hdrContributions);
			var q2 = APIContribution.contributions(Db, c, fd, td);
			var contrib = new List<string>();
			foreach (var ci in q2)
				contrib.Add("{0:MM/dd/yyyy}     {1,-16}{2,11:N2}"
						.Fmt(ci.ContributionDate, ci.Fund, ci.ContributionAmount));

			var n = contrib.Count;
			var nrows = n / 32 * 16;
			var remainder = n % 32;
			if (remainder < 16)
				nrows += remainder;
			else
				nrows += 16;

			int row, c1;
			for (row = 0, c1 = 0; row < nrows; row++, c1++)
			{
				if (row % 16 == 0)
					c1 = row * 2;
				var line = contrib[c1];
				var c2 = c1 + 16;
				if (c2 < n)
					line += "  " + contrib[c2];

				if (row > 0 && row % 16 == 0)
				{
					pageStatement += 1;
					writeHeader(c);
					rWrite(hdrContributions);
					rWrite("\n");
				}
				rWrite(line + "\n");
			}
			if (nrows > 0)
				rWrite("\n");
		}

		private void writeSummary(ContributorInfo c)
		{
			string hdr1Pledge = "            Pledge History                Current Year Summary ({0:MM/dd/yyyy})\n".Fmt(td);
			string hdr2Pledge = " Fund Name         Pledge        Given      Fund Name           Amount This Year\n\n";
			var recPledges = new List<string>();
			decimal Total = 0;
			rWrite(hdr1Pledge + hdr2Pledge);
			foreach (var p in APIContribution.pledges(Db, c, td))
				if (p.Fund != null)
					recPledges.Add("{0,-16}{1,12:N2}  {2,12:N2} ".Fmt(p.Fund, p.PledgeAmount, p.ContributionAmount));

			foreach (var ci in APIContribution.quarterlySummary(Db, c, fd, td))
			{
				Total += ci.ContributionAmount;
				if (recPledges != null && recPledges.Count > 0 && countRec < recPledges.Count)
					recPledges[countRec] += "{0,-20}{1,14:N2} ".Fmt(ci.Fund, ci.ContributionAmount);
				else
					recPledges.Add("{0,43}{1,-20}{2,14:N2} ".Fmt("", ci.Fund, ci.ContributionAmount));
				countRec++;
			}

			countRec = 0;
			foreach (var s in recPledges)
			{
				countRec++;
				if (countRec > 6 && (countRec % 6) == 1)
				{
					pageStatement++;
					writeHeader(c);
					rWrite(hdr1Pledge + hdr2Pledge);
				}
				rWrite(s + "\n");
			}
			if (countRec != 0)
				rWrite("\n");
			rWrite(totalLine + Total.ToString("C").PadLeft(25) + "\n");
			countRec = 0;
		}
	}
}