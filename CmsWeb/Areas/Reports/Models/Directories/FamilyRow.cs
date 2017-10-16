using System.Collections.Generic;
using CmsData;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Directories
{
    public class PersonInfo
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public int? Age { get; set; }
        public int position { get; set; }

        public string SafeAge => Age.HasValue && Age <= 20 ? "(" + Age + ")" : "";
    }

    public class FamilyInfo
    {
        public string FamilyName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string CityStateZip { get; set; }
        public string HomePhone { get; set; }
        public IEnumerable<PersonInfo> Members { get; set; }
    }

    public class FamilyRow
    {
        // Creates an Paragraph instance and adds its children.
        public Paragraph GenerateParagraph(FamilyInfo f)
        {
            var paragraph1 = new Paragraph {RsidParagraphAddition = "00185682", RsidParagraphProperties = "00EF4816", RsidRunAdditionDefault = "00185682"};

            var paragraphProperties1 = new ParagraphProperties();

            var tabs1 = new Tabs();
            var tabStop1 = new TabStop {Val = TabStopValues.Left, Position = 270};
            var tabStop2 = new TabStop {Val = TabStopValues.Left, Position = 1710};
            var tabStop3 = new TabStop {Val = TabStopValues.Left, Position = 4410};

            tabs1.Append(tabStop1);
            tabs1.Append(tabStop2);
            tabs1.Append(tabStop3);

            var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            var noProof1 = new NoProof();

            paragraphMarkRunProperties1.Append(noProof1);

            paragraphProperties1.Append(new KeepLines());
            paragraphProperties1.Append(tabs1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            var run1 = new Run {RsidRunProperties = "00E8319A"};

            var runProperties1 = new RunProperties();
            var bold1 = new Bold();
            var boldComplexScript1 = new BoldComplexScript();
            var italic1 = new Italic();
            var italicComplexScript1 = new ItalicComplexScript();
            var noProof2 = new NoProof();
            var color1 = new Color {Val = "4F81BD", ThemeColor = ThemeColorValues.Accent1};

            runProperties1.Append(bold1);
            runProperties1.Append(boldComplexScript1);
            runProperties1.Append(italic1);
            runProperties1.Append(italicComplexScript1);
            runProperties1.Append(noProof2);
            runProperties1.Append(color1);
            var text1 = new Text();
            text1.Text = f.FamilyName;

            run1.Append(runProperties1);
            run1.Append(text1);

            var run2 = new Run {RsidRunProperties = "006217A5"};

            var runProperties2 = new RunProperties();
            var runStyle1 = new RunStyle {Val = "IntenseEmphasis"};
            var noProof3 = new NoProof();

            runProperties2.Append(runStyle1);
            runProperties2.Append(noProof3);
            var text2 = new Text {Space = SpaceProcessingModeValues.Preserve};
            text2.Text = " Family ";

            run2.Append(runProperties2);
            run2.Append(text2);

            var run3 = new Run();

            var runProperties3 = new RunProperties();
            var noProof4 = new NoProof();

            runProperties3.Append(noProof4);
            var break1 = new Break();

            run3.Append(runProperties3);
            run3.Append(break1);

            var run4 = new Run();

            var runProperties4 = new RunProperties();
            var noProof5 = new NoProof();

            runProperties4.Append(noProof5);
            var tabChar1 = new TabChar();
            var text3 = new Text();
            text3.Text = f.Address;

            run4.Append(runProperties4);
            run4.Append(tabChar1);
            run4.Append(text3);

            var run5 = new Run();

            var runProperties5 = new RunProperties();
            var noProof6 = new NoProof();

            runProperties5.Append(noProof6);
            var break2 = new Break();

            run5.Append(runProperties5);
            run5.Append(break2);

            var run6 = new Run();

            var runProperties6 = new RunProperties();
            var noProof7 = new NoProof();

            runProperties6.Append(noProof7);
            var tabChar2 = new TabChar();
            var text4 = new Text();
            text4.Text = f.CityStateZip;

            run6.Append(runProperties6);
            run6.Append(tabChar2);
            run6.Append(text4);

            var run7 = new Run();

            var runProperties7 = new RunProperties();
            var noProof8 = new NoProof();

            runProperties7.Append(noProof8);
            var break3 = new Break();

            run7.Append(runProperties7);
            run7.Append(break3);

            var run8 = new Run();

            var runProperties8 = new RunProperties();
            var noProof9 = new NoProof();

            runProperties8.Append(noProof9);
            var tabChar3 = new TabChar();
            var text5 = new Text();
            text5.Text = "Home Phone: " + f.HomePhone.FmtFone();

            run8.Append(runProperties8);
            run8.Append(tabChar3);
            run8.Append(text5);


            var run20 = new Run();

            var runProperties20 = new RunProperties();
            var noProof21 = new NoProof();

            runProperties20.Append(noProof21);
            var tabChar11 = new TabChar();

            run20.Append(runProperties20);
            run20.Append(tabChar11);
            var bookmarkStart1 = new BookmarkStart {Name = "_GoBack", Id = "0"};
            var bookmarkEnd1 = new BookmarkEnd {Id = "0"};

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            paragraph1.Append(run2);
            paragraph1.Append(run3);
            paragraph1.Append(run4);
            paragraph1.Append(run5);
            paragraph1.Append(run6);
            paragraph1.Append(run7);
            paragraph1.Append(run8);

            foreach (var m in f.Members)
            {
                var first = new Run();
                first.Append(new RunProperties(new NoProof()));
                first.Append(new Break());
                var fname = m.Last == f.FamilyName
                    ? m.First
                    : m.First + " " + m.Last;
                fname = m.Age.HasValue && m.Age < 22
                    ? $"{fname} ({Person.AgeDisplay(m.Age, null)})"
                    : fname;

                first.Append(new Text(fname));
                paragraph1.Append(first);

                var email = new Run();
                email.Append(new RunProperties(new NoProof()));
                email.Append(new TabChar());
                email.Append(new Text(m.Email ?? ""));
                paragraph1.Append(email);

                var cell = new Run();
                cell.Append(new RunProperties(new NoProof()));
                cell.Append(new TabChar());
                cell.Append(new Text(m.Cell.FmtFone()));
                paragraph1.Append(cell);
            }

            paragraph1.Append(run20);
            paragraph1.Append(bookmarkStart1);
            paragraph1.Append(bookmarkEnd1);
            return paragraph1;
        }
    }
}
