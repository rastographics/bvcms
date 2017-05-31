using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
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

        public bool hasChildren => children.Any();

        public bool hasAddress => Address.HasValue();

        public bool hasPhones => HomePhone.HasValue() || head.Cell.HasValue() || spouse.Cell.HasValue();

        public bool hasEmail => head.Email.HasValue() || spouse.Email.HasValue();

        public Paragraph AddAlphaRow()
        {
            var paragraph1 = new Paragraph {RsidParagraphMarkRevision = "005205ED", RsidParagraphAddition = "00A01149", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C"};

            var paragraphProperties1 = new ParagraphProperties();
            var spacingBetweenLines1 = new SpacingBetweenLines {After = "60", Line = "240", LineRule = LineSpacingRuleValues.Auto};
            var justification1 = new Justification {Val = JustificationValues.Center};

            var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            var runFonts1 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var bold1 = new Bold();
            var fontSize1 = new FontSize {Val = "32"};
            var fontSizeComplexScript1 = new FontSizeComplexScript {Val = "32"};

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(bold1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(justification1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            var run1 = new Run {RsidRunProperties = "005205ED"};

            var runProperties1 = new RunProperties();
            var runFonts2 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var bold2 = new Bold();
            var fontSize2 = new FontSize {Val = "32"};
            var fontSizeComplexScript2 = new FontSizeComplexScript {Val = "32"};

            runProperties1.Append(runFonts2);
            runProperties1.Append(bold2);
            runProperties1.Append(fontSize2);
            runProperties1.Append(fontSizeComplexScript2);
            var text1 = new Text();
            text1.Text = FamilyName.Substring(0, 1);

            run1.Append(runProperties1);
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            return paragraph1;
        }

        public Paragraph AddPrimaryAdults(bool keepnext)
        {
            var paragraph1 = new Paragraph {RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "00B14EF8", RsidRunAdditionDefault = "00E7001C"};

            var paragraphProperties1 = new ParagraphProperties();
            var shading1 = new Shading {Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D9D9D9", ThemeFill = ThemeColorValues.Background1, ThemeFillShade = "D9"};
            var spacingBetweenLines1 = new SpacingBetweenLines {Before = "240", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

            var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            var runFonts1 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize1 = new FontSize {Val = "20"};
            var fontSizeComplexScript1 = new FontSizeComplexScript {Val = "20"};

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            var keeplines = new KeepLines();
            paragraphProperties1.Append(keeplines);
            if (keepnext)
                paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(shading1);
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            var run1 = new Run {RsidRunProperties = "00C74C6E"};

            var runProperties1 = new RunProperties();
            var runFonts2 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var bold1 = new Bold();

            runProperties1.Append(runFonts2);
            runProperties1.Append(bold1);
            var text1 = new Text();
            text1.Text = FamilyName;

            run1.Append(runProperties1);
            run1.Append(text1);

            var run2 = new Run {RsidRunProperties = "00E7001C"};

            var runProperties2 = new RunProperties();
            var runFonts3 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize2 = new FontSize {Val = "20"};

            runProperties2.Append(fontSize2);
            runProperties2.Append(runFonts3);
            var text2 = new Text();
            if (spouse != null && spouse.First.HasValue())
                text2.Text = $", {head.First} & {spouse.First}";
            else
                text2.Text = $", {head.First}";

            run2.Append(runProperties2);
            run2.Append(text2);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            paragraph1.Append(run2);
            return paragraph1;
        }

        public Paragraph AddChildren(bool keepnext)
        {
            var paragraph1 = new Paragraph {RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C"};

            var paragraphProperties1 = new ParagraphProperties();
            var spacingBetweenLines1 = new SpacingBetweenLines {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

            var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            var runFonts1 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize1 = new FontSize {Val = "20"};
            var fontSizeComplexScript1 = new FontSizeComplexScript {Val = "20"};

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            var keeplines = new KeepLines();
            paragraphProperties1.Append(keeplines);
            if (keepnext)
                paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraph1.Append(paragraphProperties1);

            var run1 = new Run {RsidRunProperties = "00E7001C"};

            var runProperties1 = new RunProperties();
            var runFonts2 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize2 = new FontSize {Val = "20"};

            runProperties1.Append(fontSize2);
            runProperties1.Append(runFonts2);
            run1.Append(runProperties1);

            var needbreak = false;
            foreach (var child in children)
            {
                if (needbreak)
                    run1.Append(new Break());
                needbreak = true;
                var text1 = new Text();
                var name = child.Last == FamilyName
                    ? child.First
                    : $"{child.First} {child.Last}";
                text1.Text = $"{name} {child.SafeAge}";
                run1.Append(text1);
            }
            paragraph1.Append(run1);

            return paragraph1;
        }

        public Paragraph AddAddress(bool keepnext)
        {
            var paragraph1 = new Paragraph {RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C"};

            var paragraphProperties1 = new ParagraphProperties();
            var spacingBetweenLines1 = new SpacingBetweenLines {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

            var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            var runFonts1 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize1 = new FontSize {Val = "20"};
            var fontSizeComplexScript1 = new FontSizeComplexScript {Val = "20"};

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            var keeplines = new KeepLines();
            paragraphProperties1.Append(keeplines);
            if (keepnext)
                paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraph1.Append(paragraphProperties1);

            var run1 = new Run {RsidRunProperties = "00E7001C"};

            var runProperties1 = new RunProperties();
            var runFonts2 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize2 = new FontSize {Val = "20"};

            runProperties1.Append(fontSize2);
            runProperties1.Append(runFonts2);
            var text1 = new Text();
            text1.Text = Address;

            run1.Append(runProperties1);
            run1.Append(text1);
            paragraph1.Append(run1);

            if (Address2.HasValue())
            {
                var run3 = new Run {RsidRunAddition = "00B14EF8"};

                var runProperties3 = new RunProperties();
                var runFonts4 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
                var fontSize3 = new FontSize {Val = "20"};

                runProperties1.Append(fontSize3);
                runProperties3.Append(runFonts4);
                var break1 = new Break();
                var text3 = new Text();
                text3.Text = Address2;

                run3.Append(runProperties3);
                run3.Append(break1);
                run3.Append(text3);
                paragraph1.Append(run3);
            }

            var run4 = new Run {RsidRunAddition = "00B14EF8"};

            var runProperties4 = new RunProperties();
            var runFonts5 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};

            runProperties4.Append(runFonts5);
            var break2 = new Break();

            run4.Append(runProperties4);
            run4.Append(break2);

            var run5 = new Run {RsidRunProperties = "00E7001C"};

            var runProperties5 = new RunProperties();
            var runFonts6 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize4 = new FontSize {Val = "20"};

            runProperties5.Append(fontSize4);
            runProperties5.Append(runFonts6);
            var text4 = new Text();
            text4.Text = CityStateZip;

            run5.Append(runProperties5);
            run5.Append(text4);

            paragraph1.Append(run4);
            paragraph1.Append(run5);

            return paragraph1;
        }

        public Paragraph AddPhones(bool keepnext)
        {
            var paragraph1 = new Paragraph {RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C"};

            var paragraphProperties1 = new ParagraphProperties();
            var spacingBetweenLines1 = new SpacingBetweenLines {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

            var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            var runFonts1 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize1 = new FontSize {Val = "20"};
            var fontSizeComplexScript1 = new FontSizeComplexScript {Val = "20"};

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            var keeplines = new KeepLines();
            paragraphProperties1.Append(keeplines);
            if (keepnext)
                paragraphProperties1.Append(new KeepNext());
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraph1.Append(paragraphProperties1);

            var needbreak = false;

            if (HomePhone.HasValue())
            {
                var run1 = new Run {RsidRunProperties = "00E7001C"};
                if (needbreak)
                    run1.Append(new Break());
                needbreak = true;

                var runProperties1 = new RunProperties();
                var runFonts2 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
                var fontSize3 = new FontSize {Val = "20"};

                runProperties1.Append(fontSize3);
                runProperties1.Append(runFonts2);
                var text1 = new Text();
                text1.Text = HomePhone.FmtFone("H");

                run1.Append(runProperties1);
                run1.Append(text1);
                paragraph1.Append(run1);
            }


            if (head.Cell.HasValue())
            {
                var run4 = new Run {RsidRunProperties = "00E7001C"};
                if (needbreak)
                    run4.Append(new Break());
                needbreak = true;

                var runProperties4 = new RunProperties();
                var runFonts5 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
                var fontSize3 = new FontSize {Val = "20"};

                runProperties4.Append(fontSize3);
                runProperties4.Append(runFonts5);
                var text3 = new Text();
                text3.Text = $"C {head.First}: {head.Cell.FmtFone()}";

                run4.Append(runProperties4);
                run4.Append(text3);
                paragraph1.Append(run4);
            }
            if (spouse != null && spouse.Cell.HasValue())
            {
                var run4 = new Run {RsidRunProperties = "00E7001C"};
                if (needbreak)
                    run4.Append(new Break());
                needbreak = true;

                var runProperties4 = new RunProperties();
                var runFonts5 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
                var fontSize3 = new FontSize {Val = "20"};

                runProperties4.Append(fontSize3);
                runProperties4.Append(runFonts5);
                var text3 = new Text();
                text3.Text = $"C {spouse.First}: {spouse.Cell.FmtFone()}";

                run4.Append(runProperties4);
                run4.Append(text3);
                paragraph1.Append(run4);
            }

            return paragraph1;
        }

        public Paragraph AddEmails()
        {
            var paragraph1 = new Paragraph {RsidParagraphMarkRevision = "00E7001C", RsidParagraphAddition = "00E7001C", RsidParagraphProperties = "005205ED", RsidRunAdditionDefault = "00E7001C"};

            var paragraphProperties1 = new ParagraphProperties();
            var spacingBetweenLines1 = new SpacingBetweenLines {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

            var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            var runFonts1 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
            var fontSize1 = new FontSize {Val = "20"};
            var fontSizeComplexScript1 = new FontSizeComplexScript {Val = "20"};

            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(fontSize1);
            paragraphMarkRunProperties1.Append(fontSizeComplexScript1);

            var keeplines = new KeepLines();
            paragraphProperties1.Append(keeplines);
            paragraphProperties1.Append(spacingBetweenLines1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);
            paragraph1.Append(paragraphProperties1);

            var needbreak = false;

            if (head.Email.HasValue())
            {
                var run4 = new Run {RsidRunProperties = "00E7001C"};
                if (needbreak)
                    run4.Append(new Break());
                needbreak = true;

                var runProperties4 = new RunProperties();
                var runFonts5 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
                var fontSize3 = new FontSize {Val = "20"};

                runProperties4.Append(fontSize3);
                runProperties4.Append(runFonts5);
                var text3 = new Text();
                text3.Text = $"{head.First}: {head.Email}";

                run4.Append(runProperties4);
                run4.Append(text3);
                paragraph1.Append(run4);
            }
            if (spouse != null && spouse.Email.HasValue())
            {
                var run4 = new Run {RsidRunProperties = "00E7001C"};
                if (needbreak)
                    run4.Append(new Break());
                needbreak = true;

                var runProperties4 = new RunProperties();
                var runFonts5 = new RunFonts {ComplexScriptTheme = ThemeFontValues.MinorHighAnsi};
                var fontSize3 = new FontSize {Val = "20"};

                runProperties4.Append(fontSize3);

                runProperties4.Append(runFonts5);
                var text3 = new Text();
                text3.Text = $"{spouse.First}: {spouse.Email}";

                run4.Append(runProperties4);
                run4.Append(text3);
                paragraph1.Append(run4);
            }

            return paragraph1;
        }
    }
}
