using CmsData;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace CmsWeb.Areas.Main.Models.Avery
{
	public static class EmployerAddressRow
	{
		// Creates an TableRow instance and adds its children.
		public static TableRow AddPersonsRow()
		{
			TableRow tableRow1 = new TableRow() { RsidTableRowAddition = "005A43FA" };

			TablePropertyExceptions tablePropertyExceptions1 = new TablePropertyExceptions();

			TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();
			TopMargin topMargin1 = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
			BottomMargin bottomMargin1 = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };

			tableCellMarginDefault1.Append(topMargin1);
			tableCellMarginDefault1.Append(bottomMargin1);

			tablePropertyExceptions1.Append(tableCellMarginDefault1);

			TableRowProperties tableRowProperties1 = new TableRowProperties();
			CantSplit cantSplit1 = new CantSplit();
			TableRowHeight tableRowHeight1 = new TableRowHeight() { Val = (UInt32Value)1440U, HeightType = HeightRuleValues.Exact };

			tableRowProperties1.Append(cantSplit1);
			tableRowProperties1.Append(tableRowHeight1);


			tableRow1.Append(tablePropertyExceptions1);
			tableRow1.Append(tableRowProperties1);
			return tableRow1;
		}


	}
}
