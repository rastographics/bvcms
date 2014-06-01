using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace CmsWeb.Areas.Main.Models.Avery
{
	public static class AverySpacerCell
	{
		// Creates an TableCell instance and adds its children.
		public static void AddSpacerCell(this TableRow row, string width = "173")
		{
			TableCell tableCell1 = new TableCell();

			TableCellProperties tableCellProperties1 = new TableCellProperties();
			TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = width, Type = TableWidthUnitValues.Dxa };

			tableCellProperties1.Append(tableCellWidth1);

			Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "005A43FA", RsidParagraphProperties = "005A43FA", RsidRunAdditionDefault = "005A43FA" };

			ParagraphProperties paragraphProperties1 = new ParagraphProperties();
			Indentation indentation1 = new Indentation() { Left = "95", Right = "95" };

			paragraphProperties1.Append(indentation1);

			paragraph1.Append(paragraphProperties1);

			tableCell1.Append(tableCellProperties1);
			tableCell1.Append(paragraph1);
			row.Append(tableCell1);
		}


	}
}
