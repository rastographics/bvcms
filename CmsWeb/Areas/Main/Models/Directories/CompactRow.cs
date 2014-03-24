using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Directories
{
	public class CompactFamilyInfo
	{
		public string FamilyName { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public string CityStateZip { get; set; }
		public string HomePhone { get; set; }
		public PersonInfo head { get; set; }
		public PersonInfo spouse { get; set; }
		public IEnumerable<PersonInfo> children { get; set; }
		public bool hasChildren { get { return children.Count() > 0; }}
		public bool hasAddress { get { return Address.HasValue(); }}
		public bool hasPhones { get { return HomePhone.HasValue() || head.Cell.HasValue() || spouse.Cell.HasValue(); }}
		public bool hasEmail { get { return head.Email.HasValue() || spouse.Email.HasValue(); }}

		public Paragraph AddAlphaRow()
		{
			Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "005205ED", RsidParagraphAddition = "00A01149", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C" };

			ParagraphProperties paragraphProperties1 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines() { After = "60", Line = "240", LineRule = LineSpacingRuleValues.Auto };
			Justification justification1 = new Justification() { Val = JustificationValues.Center };

			ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
			RunFonts runFonts1 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			Bold bold1 = new Bold();
			FontSize fontSize1 = new FontSize() { Val = "32" };
			FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "32" };

			paragraphMarkRunProperties1.Append(runFonts1);
			paragraphMarkRunProperties1.Append(bold1);
			paragraphMarkRunProperties1.Append(fontSize1);
			paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

			paragraphProperties1.Append(new KeepNext());
			paragraphProperties1.Append(spacingBetweenLines1);
			paragraphProperties1.Append(justification1);
			paragraphProperties1.Append(paragraphMarkRunProperties1);

			Run run1 = new Run() { RsidRunProperties = "005205ED" };

			RunProperties runProperties1 = new RunProperties();
			RunFonts runFonts2 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			Bold bold2 = new Bold();
			FontSize fontSize2 = new FontSize() { Val = "32" };
			FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = "32" };

			runProperties1.Append(runFonts2);
			runProperties1.Append(bold2);
			runProperties1.Append(fontSize2);
			runProperties1.Append(fontSizeComplexScript2);
			Text text1 = new Text();
			text1.Text = this.FamilyName.Substring(0, 1);

			run1.Append(runProperties1);
			run1.Append(text1);

			paragraph1.Append(paragraphProperties1);
			paragraph1.Append(run1);
			return paragraph1;
		}
		public Paragraph AddPrimaryAdults(bool keepnext)
		{
            Paragraph paragraph1 = new Paragraph(){ RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "00B14EF8", RsidRunAdditionDefault = "00E7001C" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
			Shading shading1 = new Shading(){ Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D9D9D9", ThemeFill = ThemeColorValues.Background1, ThemeFillShade = "D9" };
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines(){ Before = "240", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize1 = new FontSize() { Val = "20" };
			FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "20" };

			paragraphMarkRunProperties1.Append(runFonts1);
			paragraphMarkRunProperties1.Append(fontSize1);
			paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

			KeepLines keeplines = new KeepLines();
			paragraphProperties1.Append(keeplines);
			if (keepnext)
				paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(shading1);
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            Run run1 = new Run(){ RsidRunProperties = "00C74C6E" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts2 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
            Bold bold1 = new Bold();

            runProperties1.Append(runFonts2);
            runProperties1.Append(bold1);
            Text text1 = new Text();
            text1.Text = FamilyName;

            run1.Append(runProperties1);
            run1.Append(text1);

            Run run2 = new Run(){ RsidRunProperties = "00E7001C" };

            RunProperties runProperties2 = new RunProperties();
            RunFonts runFonts3 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize2 = new FontSize(){ Val = "20" };

			runProperties2.Append(fontSize2);
            runProperties2.Append(runFonts3);
            Text text2 = new Text();
			if (spouse != null && spouse.First.HasValue())
				text2.Text = ", {0} & {1}".Fmt(head.First, spouse.First);
			else
				text2.Text = ", {0}".Fmt(head.First);

            run2.Append(runProperties2);
            run2.Append(text2);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            paragraph1.Append(run2);
            return paragraph1;
		}
        public Paragraph AddChildren(bool keepnext)
        {
            Paragraph paragraph1 = new Paragraph(){ RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines(){ After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize1 = new FontSize() { Val = "20" };
			FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "20" };

			paragraphMarkRunProperties1.Append(runFonts1);
			paragraphMarkRunProperties1.Append(fontSize1);
			paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

			KeepLines keeplines = new KeepLines();
			paragraphProperties1.Append(keeplines);
			if (keepnext)
				paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraph1.Append(paragraphProperties1);

            Run run1 = new Run(){ RsidRunProperties = "00E7001C" };

            RunProperties runProperties1 = new RunProperties();
			RunFonts runFonts2 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize2 = new FontSize(){ Val = "20" };

			runProperties1.Append(fontSize2);
            runProperties1.Append(runFonts2);
            run1.Append(runProperties1);

			var needbreak = false;
			foreach (var child in children)
			{
				if (needbreak)
					run1.Append(new Break());
				needbreak = true;
				Text text1 = new Text();
			    var name = child.Last == FamilyName
			        ? child.First
			        : "{0} {1}".Fmt(child.First, child.Last);
				text1.Text = "{0} {1}".Fmt(name, child.SafeAge);
	            run1.Append(text1);
			}
            paragraph1.Append(run1);

            return paragraph1;
        }
		public Paragraph AddAddress(bool keepnext)
        {
            Paragraph paragraph1 = new Paragraph(){ RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines(){ After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto };

            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize1 = new FontSize() { Val = "20" };
			FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "20" };

			paragraphMarkRunProperties1.Append(runFonts1);
			paragraphMarkRunProperties1.Append(fontSize1);
			paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

			KeepLines keeplines = new KeepLines();
			paragraphProperties1.Append(keeplines);
			if (keepnext)
				paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraph1.Append(paragraphProperties1);

            Run run1 = new Run(){ RsidRunProperties = "00E7001C" };

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts2 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize2 = new FontSize(){ Val = "20" };

			runProperties1.Append(fontSize2);
            runProperties1.Append(runFonts2);
            Text text1 = new Text();
			text1.Text = Address;

            run1.Append(runProperties1);
            run1.Append(text1);
            paragraph1.Append(run1);

			if (Address2.HasValue())
			{
				Run run3 = new Run() { RsidRunAddition = "00B14EF8" };

				RunProperties runProperties3 = new RunProperties();
				RunFonts runFonts4 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
				FontSize fontSize3 = new FontSize(){ Val = "20" };

				runProperties1.Append(fontSize3);
				runProperties3.Append(runFonts4);
				Break break1 = new Break();
				Text text3 = new Text();
				text3.Text = Address2;

				run3.Append(runProperties3);
				run3.Append(break1);
				run3.Append(text3);
				paragraph1.Append(run3);
			}

            Run run4 = new Run(){ RsidRunAddition = "00B14EF8" };

            RunProperties runProperties4 = new RunProperties();
            RunFonts runFonts5 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };

            runProperties4.Append(runFonts5);
            Break break2 = new Break();

            run4.Append(runProperties4);
            run4.Append(break2);

            Run run5 = new Run(){ RsidRunProperties = "00E7001C" };

            RunProperties runProperties5 = new RunProperties();
            RunFonts runFonts6 = new RunFonts(){ ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize4 = new FontSize(){ Val = "20" };

			runProperties5.Append(fontSize4);
            runProperties5.Append(runFonts6);
            Text text4 = new Text();
			text4.Text = CityStateZip;

            run5.Append(runProperties5);
            run5.Append(text4);

            paragraph1.Append(run4);
            paragraph1.Append(run5);

            return paragraph1;
        }
		public Paragraph AddPhones(bool keepnext)
		{
			Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C" };

			ParagraphProperties paragraphProperties1 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto };

			ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
			RunFonts runFonts1 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize1 = new FontSize() { Val = "20" };
			FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "20" };

			paragraphMarkRunProperties1.Append(runFonts1);
			paragraphMarkRunProperties1.Append(fontSize1);
			paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

			KeepLines keeplines = new KeepLines();
			paragraphProperties1.Append(keeplines);
			if (keepnext)
				paragraphProperties1.Append(new KeepNext());
			paragraphProperties1.Append(spacingBetweenLines1);
			paragraphProperties1.Append(paragraphMarkRunProperties1);
			paragraph1.Append(paragraphProperties1);

			var needbreak = false;
			
			if (HomePhone.HasValue())
			{
				Run run1 = new Run() { RsidRunProperties = "00E7001C" };
				if (needbreak)
					run1.Append(new Break());
				needbreak = true;

				RunProperties runProperties1 = new RunProperties();
				RunFonts runFonts2 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
				FontSize fontSize3 = new FontSize(){ Val = "20" };

				runProperties1.Append(fontSize3);
				runProperties1.Append(runFonts2);
				Text text1 = new Text();
				text1.Text = HomePhone.FmtFone("H");

				run1.Append(runProperties1);
				run1.Append(text1);
				paragraph1.Append(run1);
			}


			if (head.Cell.HasValue())
			{
				Run run4 = new Run() { RsidRunProperties = "00E7001C" };
				if (needbreak)
					run4.Append(new Break());
				needbreak = true;

				RunProperties runProperties4 = new RunProperties();
				RunFonts runFonts5 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
				FontSize fontSize3 = new FontSize(){ Val = "20" };

				runProperties4.Append(fontSize3);
				runProperties4.Append(runFonts5);
				Text text3 = new Text();
				text3.Text = "C {0}: {1}".Fmt(head.First, head.Cell.FmtFone());

				run4.Append(runProperties4);
				run4.Append(text3);
				paragraph1.Append(run4);
			}
			if (spouse != null && spouse.Cell.HasValue())
			{
				Run run4 = new Run() { RsidRunProperties = "00E7001C" };
				if (needbreak)
					run4.Append(new Break());
				needbreak = true;

				RunProperties runProperties4 = new RunProperties();
				RunFonts runFonts5 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
				FontSize fontSize3 = new FontSize(){ Val = "20" };

				runProperties4.Append(fontSize3);
				runProperties4.Append(runFonts5);
				Text text3 = new Text();
				text3.Text = "C {0}: {1}".Fmt(spouse.First, spouse.Cell.FmtFone());

				run4.Append(runProperties4);
				run4.Append(text3);
				paragraph1.Append(run4);
			}

			return paragraph1;
		}
		public Paragraph AddEmails()
		{
			Paragraph paragraph1 = new Paragraph() { RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C" };

			ParagraphProperties paragraphProperties1 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto };

			ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
			RunFonts runFonts1 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
			FontSize fontSize1 = new FontSize() { Val = "20" };
			FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = "20" };

			paragraphMarkRunProperties1.Append(runFonts1);
			paragraphMarkRunProperties1.Append(fontSize1);
			paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

			KeepLines keeplines = new KeepLines();
			paragraphProperties1.Append(keeplines);
			paragraphProperties1.Append(spacingBetweenLines1);
			paragraphProperties1.Append(paragraphMarkRunProperties1);
			paragraph1.Append(paragraphProperties1);

			var needbreak = false;

			if (head.Email.HasValue())
			{
				Run run4 = new Run() { RsidRunProperties = "00E7001C" };
				if (needbreak)
					run4.Append(new Break());
				needbreak = true;

				RunProperties runProperties4 = new RunProperties();
				RunFonts runFonts5 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
				FontSize fontSize3 = new FontSize(){ Val = "20" };

				runProperties4.Append(fontSize3);
				runProperties4.Append(runFonts5);            
				Text text3 = new Text();
				text3.Text = "{0}: {1}".Fmt(head.First, head.Email);

				run4.Append(runProperties4);
				run4.Append(text3);
				paragraph1.Append(run4);

			}
			if (spouse != null && spouse.Email.HasValue())
			{
				Run run4 = new Run() { RsidRunProperties = "00E7001C" };
				if (needbreak)
					run4.Append(new Break());
				needbreak = true;

				RunProperties runProperties4 = new RunProperties();
				RunFonts runFonts5 = new RunFonts() { ComplexScriptTheme = ThemeFontValues.MinorHighAnsi };
				FontSize fontSize3 = new FontSize(){ Val = "20" };

				runProperties4.Append(fontSize3);

				runProperties4.Append(runFonts5);
				Text text3 = new Text();
				text3.Text = "{0}: {1}".Fmt(spouse.First, spouse.Email);

				run4.Append(runProperties4);
				run4.Append(text3);
				paragraph1.Append(run4);
			}

			return paragraph1;
		}
	}
}
