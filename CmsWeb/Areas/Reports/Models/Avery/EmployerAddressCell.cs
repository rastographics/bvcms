using CmsData;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Avery
{
	public static class EmployerAddressCell
	{
		// Creates an TableCell instance and adds its children.
		public static void AddPersonCell(this TableRow row, Person p, bool addEmployer)
		{
			TableCell tableCell1 = new TableCell();

			TableCellProperties tableCellProperties1 = new TableCellProperties();
			TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "3787", Type = TableWidthUnitValues.Dxa };

			tableCellProperties1.Append(tableCellWidth1);

			Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "005A43FA", RsidParagraphProperties = "005A43FA", RsidRunAdditionDefault = "005A43FA" };

			ParagraphProperties paragraphProperties1 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines() { Before = "111" };
			Indentation indentation1 = new Indentation() { Left = "95", Right = "95" };

			paragraphProperties1.Append(spacingBetweenLines1);
			paragraphProperties1.Append(indentation1);
			paragraph1.Append(paragraphProperties1);

			if (p != null)
			{
				Run run1 = new Run();
				Text text1 = new Text();
				var name = (p.TitleCode.HasValue() ? p.TitleCode + " " : "")
					+ (p.FirstName == "?" ? "" : p.FirstName + " ")
					+ (p.LastName == "?" ? "" : p.LastName);
				text1.Text = name;

				run1.Append(text1);

				paragraph1.Append(run1);

				Paragraph paragraph2 = null;
				if (addEmployer && p.EmployerOther.HasValue())
				{
					paragraph2 = new Paragraph()
					                       {
					                       	RsidParagraphAddition = "005A43FA",
					                       	RsidParagraphProperties = "005A43FA",
					                       	RsidRunAdditionDefault = "005A43FA"
					                       };

					ParagraphProperties paragraphProperties2 = new ParagraphProperties();
					Indentation indentation2 = new Indentation() { Left = "245", Right = "95", Hanging = "150" };

					paragraphProperties2.Append(indentation2);

					Run run2 = new Run();
					Text text2 = new Text();
					text2.Text = p.EmployerOther;

					run2.Append(text2);

					paragraph2.Append(paragraphProperties2);
					paragraph2.Append(run2);
				}

				Paragraph paragraph3 = new Paragraph()
				                       {
				                       	RsidParagraphAddition = "005A43FA",
				                       	RsidParagraphProperties = "005A43FA",
				                       	RsidRunAdditionDefault = "005A43FA"
				                       };

				ParagraphProperties paragraphProperties3 = new ParagraphProperties();
				Indentation indentation3 = new Indentation() {Left = "95", Right = "95"};

				paragraphProperties3.Append(indentation3);

				Run run3 = new Run();
				Text text3 = new Text();
				text3.Text = p.PrimaryAddress;

				run3.Append(text3);

				paragraph3.Append(paragraphProperties3);
				paragraph3.Append(run3);

				Paragraph paragraph3a = null;
				if (p.PrimaryAddress2.HasValue())
				{
					paragraph3a = new Paragraph()
					                        {
					                        	RsidParagraphAddition = "005A43FA",
					                        	RsidParagraphProperties = "005A43FA",
					                        	RsidRunAdditionDefault = "005A43FA"
					                        };

					ParagraphProperties paragraphProperties2 = new ParagraphProperties();
					Indentation indentation2 = new Indentation() {Left = "95", Right = "95"};

					paragraphProperties2.Append(indentation2);

					Run run2 = new Run();
					Text text2 = new Text();
					text2.Text = p.PrimaryAddress2;

					run2.Append(text2);

					paragraph3a.Append(paragraphProperties2);
					paragraph3a.Append(run2);
				}


				Paragraph paragraph4 = new Paragraph()
				                       {
				                       	RsidParagraphAddition = "005A43FA",
				                       	RsidParagraphProperties = "005A43FA",
				                       	RsidRunAdditionDefault = "005A43FA"
				                       };

				ParagraphProperties paragraphProperties4 = new ParagraphProperties();
				Indentation indentation4 = new Indentation() {Left = "95", Right = "95"};

				paragraphProperties4.Append(indentation4);

				Run run4 = new Run();
				Text text4 = new Text();
				text4.Text = p.CityStateZip;

				run4.Append(text4);

				paragraph4.Append(paragraphProperties4);
				paragraph4.Append(run4);

				tableCell1.Append(tableCellProperties1);
				tableCell1.Append(paragraph1);
				if (paragraph2 != null)
					tableCell1.Append(paragraph2);
				tableCell1.Append(paragraph3);
				if (paragraph3a != null)
					tableCell1.Append(paragraph3a);
				tableCell1.Append(paragraph4);
			}
			else
				tableCell1.Append(paragraph1);
			row.Append(tableCell1);
		}


	}
}
