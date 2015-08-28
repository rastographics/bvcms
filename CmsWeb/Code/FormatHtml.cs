using System;
using System.IO;
using System.Text;
using TidyManaged;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class TidyLib
    {
        public static string FormatHtml(string body)
        {
            try
            {
                if (!body.HasValue())
                    return body;

                using (var doc = Document.FromString(body))
                {
                    doc.NewBlockLevelTags = "registertag, article";

                    doc.ShowWarnings = false;
                    doc.Quiet = true;
                    doc.DocType = DocTypeMode.Strict;
                    doc.DropFontTags = true;
                    doc.UseLogicalEmphasis = true;
                    doc.OutputXhtml = true;
                    doc.OutputXml = false;
                    doc.MakeClean = true;
                    doc.DropEmptyParagraphs = true;
                    doc.CleanWord2000 = true;
                    doc.QuoteAmpersands = true;
                    doc.JoinStyles = false;
                    doc.JoinClasses = false;
                    doc.Markup = true;
                    doc.IndentSpaces = 4;
                    doc.IndentBlockElements = AutoBool.Yes;
                    doc.CharacterEncoding = EncodingType.Utf8;
                    doc.OutputBodyOnly = AutoBool.Auto;
                    doc.CleanAndRepair();

                    var content = doc.Save();
                    return content.Length < body.Length ? body : content;
                }
            }
            catch (DllNotFoundException)
            {
                return body;
            }
        }
    }
}
