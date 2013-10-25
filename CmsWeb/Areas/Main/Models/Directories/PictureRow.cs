using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Media.Imaging;
using CmsData;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;
using A14 = DocumentFormat.OpenXml.Office2010.Drawing;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Directories
{

	public class PictureRow
	{
//	    private static void ddd()
//	    {
//	        var img = new BitmapImage(new Uri("", UriKind.RelativeOrAbsolute));
//	        var widthPx = img.PixelWidth;
//	        var heightPx = img.PixelHeight;
//	        var horzRezDpi = img.DpiX;
//	        var vertRezDpi = img.DpiY;
//	        const int emusPerInch = 914400;
//	        const int emusPerCm = 360000;
//	        var widthEmus = (long) (widthPx/horzRezDpi*emusPerInch);
//	        var heightEmus = (long) (heightPx/vertRezDpi*emusPerInch);
//	        var maxWidthEmus = (long) (maxWidthCm*emusPerCm);
//	        if (widthEmus > maxWidthEmus)
//	        {
//	            var ratio = (heightEmus*1.0m)/widthEmus;
//	            widthEmus = maxWidthEmus;
//	            heightEmus = (long) (widthEmus*ratio);
//	        }
//	    }
//        public void ResizeImage(string OriginalFile, string NewFile, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
//{
//	System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFile);
//
//	// Prevent using images internal thumbnail
//	FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
//	FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
//
//	if (OnlyResizeIfWider)
//	{
//		if (FullsizeImage.Width <= NewWidth)
//		{
//			NewWidth = FullsizeImage.Width;
//		}
//	}
//
//	int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
//	if (NewHeight > MaxHeight)
//	{
//		// Resize with height instead
//		NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
//		NewHeight = MaxHeight;
//	}
//
//	System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);
//
//	// Clear handle to original file so that we can overwrite it if necessary
//	FullsizeImage.Dispose();
//
//	// Save resized picture
//	NewImage.Save(NewFile);
//}
//
		// Creates an Paragraph instance and adds its children.
		public TableRow GenerateTableRow(IndividualInfo ii, string iid)
		{
			TableRow tableRow1 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00442AD3" };

			TableRowProperties tableRowProperties1 = new TableRowProperties();
			CantSplit cantSplit1 = new CantSplit();

			tableRowProperties1.Append(cantSplit1);

			TableCell tableCell1 = new TableCell();

			TableCellProperties tableCellProperties1 = new TableCellProperties();
			TableCellWidth tableCellWidth1 = new TableCellWidth() { Width = "4788", Type = TableWidthUnitValues.Dxa };

			tableCellProperties1.Append(tableCellWidth1);

			Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run1 = new Run();
			Break break1 = new Break();

			run1.Append(break1);

			Run run2 = new Run();

			RunProperties runProperties1 = new RunProperties();
			NoProof noProof1 = new NoProof();

			runProperties1.Append(noProof1);
			Text text1 = new Text();
			text1.Text = ii.FirstName;

			run2.Append(runProperties1);
			run2.Append(text1);

			Run run3 = new Run();
			Text text2 = new Text() { Space = SpaceProcessingModeValues.Preserve };
			text2.Text = " ";

			run3.Append(text2);

			Run run4 = new Run();

			RunProperties runProperties2 = new RunProperties();
			NoProof noProof2 = new NoProof();

			runProperties2.Append(noProof2);
			Text text3 = new Text();
			text3.Text = ii.LastName;

			run4.Append(runProperties2);
			run4.Append(text3);

			paragraph1.Append(run1);
			paragraph1.Append(run2);
			paragraph1.Append(run3);
			paragraph1.Append(run4);

			Paragraph paragraph2 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00731A43", RsidRunAdditionDefault = "00FB1F22" };

			Run run5 = new Run();

			RunProperties runProperties3 = new RunProperties();
			NoProof noProof3 = new NoProof();

			runProperties3.Append(noProof3);
			Text text4 = new Text();
			text4.Text = ii.Address;

			run5.Append(runProperties3);
			run5.Append(text4);

			paragraph2.Append(run5);


			Paragraph paragraph2a = null;
			if (ii.Address2.HasValue())
			{
				paragraph2a = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00731A43", RsidRunAdditionDefault = "00FB1F22" };
				Run r = new Run();

				RunProperties rp = new RunProperties();
				NoProof np = new NoProof();

				rp.Append(np);
				Text tt = new Text();
				tt.Text = ii.Address;

				r.Append(rp);
				r.Append(tt);

				paragraph2a.Append(r);
			}


			Paragraph paragraph3 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00731A43", RsidRunAdditionDefault = "00FB1F22" };

			Run run6 = new Run();

			RunProperties runProperties4 = new RunProperties();
			NoProof noProof4 = new NoProof();

			runProperties4.Append(noProof4);
			Text text5 = new Text();
			text5.Text = ii.CityStateZip;
			run6.Append(runProperties4);
			run6.Append(text5);

			paragraph3.Append(run6);

			Paragraph paragraph4 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Table table1 = new Table();

			TableProperties tableProperties1 = new TableProperties();
			TableStyle tableStyle1 = new TableStyle() { Val = "TableGrid" };
			TableWidth tableWidth1 = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };

			TableBorders tableBorders1 = new TableBorders();
			TopBorder topBorder1 = new TopBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
			LeftBorder leftBorder1 = new LeftBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
			BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
			RightBorder rightBorder1 = new RightBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
			InsideHorizontalBorder insideHorizontalBorder1 = new InsideHorizontalBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
			InsideVerticalBorder insideVerticalBorder1 = new InsideVerticalBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };

			tableBorders1.Append(topBorder1);
			tableBorders1.Append(leftBorder1);
			tableBorders1.Append(bottomBorder1);
			tableBorders1.Append(rightBorder1);
			tableBorders1.Append(insideHorizontalBorder1);
			tableBorders1.Append(insideVerticalBorder1);
			TableLook tableLook1 = new TableLook() { Val = "04A0", FirstRow = true, LastRow = false, FirstColumn = true, LastColumn = false, NoHorizontalBand = false, NoVerticalBand = true };

			tableProperties1.Append(tableStyle1);
			tableProperties1.Append(tableWidth1);
			tableProperties1.Append(tableBorders1);
			tableProperties1.Append(tableLook1);

			TableGrid tableGrid1 = new TableGrid();
			GridColumn gridColumn1 = new GridColumn() { Width = "1435" };
			GridColumn gridColumn2 = new GridColumn() { Width = "3122" };

			tableGrid1.Append(gridColumn1);
			tableGrid1.Append(gridColumn2);

			TableRow tableRow2 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00731A43" };

			TableCell tableCell2 = new TableCell();

			TableCellProperties tableCellProperties2 = new TableCellProperties();
			TableCellWidth tableCellWidth2 = new TableCellWidth() { Width = "1435", Type = TableWidthUnitValues.Dxa };

			tableCellProperties2.Append(tableCellWidth2);

			Paragraph paragraph5 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run11 = new Run();
			Text text10 = new Text();
			text10.Text = "Email";

			run11.Append(text10);

			paragraph5.Append(run11);

			tableCell2.Append(tableCellProperties2);
			tableCell2.Append(paragraph5);

			TableCell tableCell3 = new TableCell();

			TableCellProperties tableCellProperties3 = new TableCellProperties();
			TableCellWidth tableCellWidth3 = new TableCellWidth() { Width = "3122", Type = TableWidthUnitValues.Dxa };

			tableCellProperties3.Append(tableCellWidth3);
			Paragraph paragraph6 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00485B24", RsidRunAdditionDefault = "00FB1F22" };

			paragraph6.Append(new Run(new Text(ii.Email)));

			tableCell3.Append(tableCellProperties3);
			tableCell3.Append(paragraph6);

			tableRow2.Append(tableCell2);
			tableRow2.Append(tableCell3);

			TableRow tableRow3 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00731A43" };

			TableCell tableCell4 = new TableCell();

			TableCellProperties tableCellProperties4 = new TableCellProperties();
			TableCellWidth tableCellWidth4 = new TableCellWidth() { Width = "1435", Type = TableWidthUnitValues.Dxa };

			tableCellProperties4.Append(tableCellWidth4);

			Paragraph paragraph7 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run12 = new Run();
			Text text11 = new Text();
			text11.Text = "Home Phone";

			run12.Append(text11);

			paragraph7.Append(run12);

			tableCell4.Append(tableCellProperties4);
			tableCell4.Append(paragraph7);

			TableCell tableCell5 = new TableCell();

			TableCellProperties tableCellProperties5 = new TableCellProperties();
			TableCellWidth tableCellWidth5 = new TableCellWidth() { Width = "3122", Type = TableWidthUnitValues.Dxa };

			tableCellProperties5.Append(tableCellWidth5);

			Paragraph paragraph8 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00485B24", RsidRunAdditionDefault = "00FB1F22" };

			Run run13 = new Run();

			RunProperties runProperties7 = new RunProperties();
			NoProof noProof7 = new NoProof();

			runProperties7.Append(noProof7);
			Text text12 = new Text();
			text12.Text = ii.HomePhone.FmtFone();

			run13.Append(runProperties7);
			run13.Append(text12);

			Run run14 = new Run();
			Text text13 = new Text() { Space = SpaceProcessingModeValues.Preserve };
			text13.Text = " ";

			run14.Append(text13);

			paragraph8.Append(run13);
			paragraph8.Append(run14);

			tableCell5.Append(tableCellProperties5);
			tableCell5.Append(paragraph8);

			tableRow3.Append(tableCell4);
			tableRow3.Append(tableCell5);

			TableRow tableRow4 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00731A43" };

			TableCell tableCell6 = new TableCell();

			TableCellProperties tableCellProperties6 = new TableCellProperties();
			TableCellWidth tableCellWidth6 = new TableCellWidth() { Width = "1435", Type = TableWidthUnitValues.Dxa };

			tableCellProperties6.Append(tableCellWidth6);

			Paragraph paragraph9 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run15 = new Run();
			Text text14 = new Text();
			text14.Text = "Cell Phone";

			run15.Append(text14);

			paragraph9.Append(run15);

			tableCell6.Append(tableCellProperties6);
			tableCell6.Append(paragraph9);

			TableCell tableCell7 = new TableCell();

			TableCellProperties tableCellProperties7 = new TableCellProperties();
			TableCellWidth tableCellWidth7 = new TableCellWidth() { Width = "3122", Type = TableWidthUnitValues.Dxa };

			tableCellProperties7.Append(tableCellWidth7);

			Paragraph paragraph10 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run16 = new Run();

			RunProperties runProperties8 = new RunProperties();
			NoProof noProof8 = new NoProof();

			runProperties8.Append(noProof8);
			Text text15 = new Text();
			text15.Text = ii.CellPhone.FmtFone();

			run16.Append(runProperties8);
			run16.Append(text15);

			paragraph10.Append(run16);

			tableCell7.Append(tableCellProperties7);
			tableCell7.Append(paragraph10);

			tableRow4.Append(tableCell6);
			tableRow4.Append(tableCell7);

			TableRow tableRow5 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00731A43" };

			TableCell tableCell8 = new TableCell();

			TableCellProperties tableCellProperties8 = new TableCellProperties();
			TableCellWidth tableCellWidth8 = new TableCellWidth() { Width = "1435", Type = TableWidthUnitValues.Dxa };

			tableCellProperties8.Append(tableCellWidth8);

			Paragraph paragraph11 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run17 = new Run();
			Text text16 = new Text();
			text16.Text = "Work Phone";

			run17.Append(text16);

			paragraph11.Append(run17);

			tableCell8.Append(tableCellProperties8);
			tableCell8.Append(paragraph11);

			TableCell tableCell9 = new TableCell();

			TableCellProperties tableCellProperties9 = new TableCellProperties();
			TableCellWidth tableCellWidth9 = new TableCellWidth() { Width = "3122", Type = TableWidthUnitValues.Dxa };

			tableCellProperties9.Append(tableCellWidth9);

			Paragraph paragraph12 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run18 = new Run();

			RunProperties runProperties9 = new RunProperties();
			NoProof noProof9 = new NoProof();

			runProperties9.Append(noProof9);
			Text text17 = new Text();
			text17.Text = ii.WorkPhone.FmtFone();

			run18.Append(runProperties9);
			run18.Append(text17);

			paragraph12.Append(run18);

			tableCell9.Append(tableCellProperties9);
			tableCell9.Append(paragraph12);

			tableRow5.Append(tableCell8);
			tableRow5.Append(tableCell9);

			TableRow tableRow6 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00731A43" };

			TableCell tableCell10 = new TableCell();

			TableCellProperties tableCellProperties10 = new TableCellProperties();
			TableCellWidth tableCellWidth10 = new TableCellWidth() { Width = "1435", Type = TableWidthUnitValues.Dxa };

			tableCellProperties10.Append(tableCellWidth10);

			Paragraph paragraph13 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run19 = new Run();
			Text text18 = new Text();
			text18.Text = "Birthday";

			run19.Append(text18);

			paragraph13.Append(run19);

			tableCell10.Append(tableCellProperties10);
			tableCell10.Append(paragraph13);

			TableCell tableCell11 = new TableCell();

			TableCellProperties tableCellProperties11 = new TableCellProperties();
			TableCellWidth tableCellWidth11 = new TableCellWidth() { Width = "3122", Type = TableWidthUnitValues.Dxa };

			tableCellProperties11.Append(tableCellWidth11);

			Paragraph paragraph14 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00BC57A3", RsidRunAdditionDefault = "00FB1F22" };

			Run run20 = new Run();

			RunProperties runProperties10 = new RunProperties();
			NoProof noProof10 = new NoProof();

			runProperties10.Append(noProof10);
			Text text19 = new Text();
			text19.Text = ii.BirthDay;

			run20.Append(runProperties10);
			run20.Append(text19);

			paragraph14.Append(run20);

			tableCell11.Append(tableCellProperties11);
			tableCell11.Append(paragraph14);

			tableRow6.Append(tableCell10);
			tableRow6.Append(tableCell11);

			TableRow tableRow7 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00731A43" };

			TableCell tableCell12 = new TableCell();

			TableCellProperties tableCellProperties12 = new TableCellProperties();
			TableCellWidth tableCellWidth12 = new TableCellWidth() { Width = "1435", Type = TableWidthUnitValues.Dxa };

			tableCellProperties12.Append(tableCellWidth12);

			Paragraph paragraph15 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "006D0F9D", RsidRunAdditionDefault = "00FB1F22" };

			Run run21 = new Run();
			Text text20 = new Text();
			text20.Text = "Anniversary";

			run21.Append(text20);

			paragraph15.Append(run21);

			tableCell12.Append(tableCellProperties12);
			tableCell12.Append(paragraph15);

			TableCell tableCell13 = new TableCell();

			TableCellProperties tableCellProperties13 = new TableCellProperties();
			TableCellWidth tableCellWidth13 = new TableCellWidth() { Width = "3122", Type = TableWidthUnitValues.Dxa };

			tableCellProperties13.Append(tableCellWidth13);
			Paragraph paragraph16 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			paragraph16.Append(new Run(new Text(ii.Anniversary)));

			tableCell13.Append(tableCellProperties13);
			tableCell13.Append(paragraph16);

			tableRow7.Append(tableCell12);
			tableRow7.Append(tableCell13);

			TableRow tableRow8 = new TableRow() { RsidTableRowAddition = "00FB1F22", RsidTableRowProperties = "00731A43" };

			TableCell tableCell14 = new TableCell();

			TableCellProperties tableCellProperties14 = new TableCellProperties();
			TableCellWidth tableCellWidth14 = new TableCellWidth() { Width = "1435", Type = TableWidthUnitValues.Dxa };

			tableCellProperties14.Append(tableCellWidth14);

			Paragraph paragraph17 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			Run run22 = new Run();
			Text text21 = new Text();
			text21.Text = "Spouse";

			run22.Append(text21);

			paragraph17.Append(run22);

			tableCell14.Append(tableCellProperties14);
			tableCell14.Append(paragraph17);

			TableCell tableCell15 = new TableCell();

			TableCellProperties tableCellProperties15 = new TableCellProperties();
			TableCellWidth tableCellWidth15 = new TableCellWidth() { Width = "3122", Type = TableWidthUnitValues.Dxa };

			tableCellProperties15.Append(tableCellWidth15);

			Paragraph paragraph18 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00F06304", RsidRunAdditionDefault = "00FB1F22" };

			ParagraphProperties paragraphProperties1 = new ParagraphProperties();

			ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
			NoProof noProof11 = new NoProof();

			paragraphMarkRunProperties1.Append(noProof11);

			paragraphProperties1.Append(paragraphMarkRunProperties1);

			Run run23 = new Run();

			RunProperties runProperties11 = new RunProperties();
			NoProof noProof12 = new NoProof();

			runProperties11.Append(noProof12);
			Text text22 = new Text();
			text22.Text = ii.Spouse;

			run23.Append(runProperties11);
			run23.Append(text22);

			paragraph18.Append(paragraphProperties1);
			paragraph18.Append(run23);

			tableCell15.Append(tableCellProperties15);
			tableCell15.Append(paragraph18);

			tableRow8.Append(tableCell14);
			tableRow8.Append(tableCell15);

			table1.Append(tableProperties1);
			table1.Append(tableGrid1);
			table1.Append(tableRow2);
			table1.Append(tableRow3);
			table1.Append(tableRow4);
			table1.Append(tableRow5);
			table1.Append(tableRow6);
			table1.Append(tableRow7);
			table1.Append(tableRow8);
			Paragraph paragraph19 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00485B24", RsidRunAdditionDefault = "00FB1F22" };

			tableCell1.Append(tableCellProperties1);
			tableCell1.Append(paragraph1);
			tableCell1.Append(paragraph2);
			if (paragraph2a != null)
				tableCell1.Append(paragraph2a);
			tableCell1.Append(paragraph3);
			tableCell1.Append(paragraph4);
			tableCell1.Append(table1);
			tableCell1.Append(paragraph19);

			TableCell tableCell16 = new TableCell();

			TableCellProperties tableCellProperties16 = new TableCellProperties();
			TableCellWidth tableCellWidth16 = new TableCellWidth() { Width = "4788", Type = TableWidthUnitValues.Dxa };
			TableCellVerticalAlignment tableCellVerticalAlignment1 = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };

			tableCellProperties16.Append(tableCellWidth16);
			tableCellProperties16.Append(tableCellVerticalAlignment1);
			tableCell16.Append(tableCellProperties16);

			Paragraph paragraph20 = new Paragraph() { RsidParagraphAddition = "00FB1F22", RsidParagraphProperties = "00960953", RsidRunAdditionDefault = "00FB1F22" };
			ParagraphProperties paragraphProperties2 = new ParagraphProperties();
			Justification justification1 = new Justification() { Val = JustificationValues.Center };

			paragraphProperties2.Append(justification1);
			paragraph20.Append(paragraphProperties2);

			if (iid.HasValue())
			{
				Run run24 = new Run();

				RunProperties runProperties12 = new RunProperties();
				NoProof noProof13 = new NoProof();

				runProperties12.Append(noProof13);

				Drawing drawing1 = new Drawing();

				Wp.Inline inline1 = new Wp.Inline()
									{
										DistanceFromTop = (UInt32Value)0U,
										DistanceFromBottom = (UInt32Value)0U,
										DistanceFromLeft = (UInt32Value)0U,
										DistanceFromRight = (UInt32Value)0U
									};
				Wp.Extent extent1 = new Wp.Extent() { Cx = 1428750L, Cy = 1905000L };
				Wp.EffectExtent effectExtent1 = new Wp.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L };
				Wp.DocProperties docProperties1 = new Wp.DocProperties() { Id = (UInt32Value)1U, Name = "Picture 1", Description = "D:\\Pictures\\ttt.jpg" };

				Wp.NonVisualGraphicFrameDrawingProperties nonVisualGraphicFrameDrawingProperties1 =
					new Wp.NonVisualGraphicFrameDrawingProperties();

				A.GraphicFrameLocks graphicFrameLocks1 = new A.GraphicFrameLocks() { NoChangeAspect = true };
				graphicFrameLocks1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

				nonVisualGraphicFrameDrawingProperties1.Append(graphicFrameLocks1);

				A.Graphic graphic1 = new A.Graphic();
				graphic1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

				A.GraphicData graphicData1 = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };

				Pic.Picture picture1 = new Pic.Picture();
				picture1.AddNamespaceDeclaration("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");

				Pic.NonVisualPictureProperties nonVisualPictureProperties1 = new Pic.NonVisualPictureProperties();
				Pic.NonVisualDrawingProperties nonVisualDrawingProperties1 = new Pic.NonVisualDrawingProperties()
																			 {
																				 Id = (UInt32Value)0U,
																				 Name = "Picture 459",
																				 Description = "D:\\Pictures\\ttt.jpg"
																			 };

				Pic.NonVisualPictureDrawingProperties nonVisualPictureDrawingProperties1 =
					new Pic.NonVisualPictureDrawingProperties();
				A.PictureLocks pictureLocks1 = new A.PictureLocks() { NoChangeAspect = false, NoChangeArrowheads = true };

				nonVisualPictureDrawingProperties1.Append(pictureLocks1);

				nonVisualPictureProperties1.Append(nonVisualDrawingProperties1);
				nonVisualPictureProperties1.Append(nonVisualPictureDrawingProperties1);

				Pic.BlipFill blipFill1 = new Pic.BlipFill();

				A.Blip blip1 = new A.Blip() { Embed = iid };

				A.BlipExtensionList blipExtensionList1 = new A.BlipExtensionList();

				A.BlipExtension blipExtension1 = new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

				A14.UseLocalDpi useLocalDpi1 = new A14.UseLocalDpi() { Val = false };
				useLocalDpi1.AddNamespaceDeclaration("a14", "http://schemas.microsoft.com/office/drawing/2010/main");

				blipExtension1.Append(useLocalDpi1);

				blipExtensionList1.Append(blipExtension1);

				blip1.Append(blipExtensionList1);
				A.SourceRectangle sourceRectangle1 = new A.SourceRectangle();

				A.Stretch stretch1 = new A.Stretch();
				A.FillRectangle fillRectangle1 = new A.FillRectangle();

				stretch1.Append(fillRectangle1);

				blipFill1.Append(blip1);
				blipFill1.Append(sourceRectangle1);
				blipFill1.Append(stretch1);

				Pic.ShapeProperties shapeProperties1 = new Pic.ShapeProperties() { BlackWhiteMode = A.BlackWhiteModeValues.Auto };

				A.Transform2D transform2D1 = new A.Transform2D();
				A.Offset offset1 = new A.Offset() { X = 0L, Y = 0L };
				A.Extents extents1 = new A.Extents() { Cx = 1428750L, Cy = 1905000L };

				transform2D1.Append(offset1);
				transform2D1.Append(extents1);

				A.PresetGeometry presetGeometry1 = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
				A.AdjustValueList adjustValueList1 = new A.AdjustValueList();

				presetGeometry1.Append(adjustValueList1);
				A.NoFill noFill1 = new A.NoFill();

				A.Outline outline1 = new A.Outline();
				A.NoFill noFill2 = new A.NoFill();

				outline1.Append(noFill2);

				shapeProperties1.Append(transform2D1);
				shapeProperties1.Append(presetGeometry1);
				shapeProperties1.Append(noFill1);
				shapeProperties1.Append(outline1);

				picture1.Append(nonVisualPictureProperties1);
				picture1.Append(blipFill1);
				picture1.Append(shapeProperties1);

				graphicData1.Append(picture1);

				graphic1.Append(graphicData1);

				inline1.Append(extent1);
				inline1.Append(effectExtent1);
				inline1.Append(docProperties1);
				inline1.Append(nonVisualGraphicFrameDrawingProperties1);
				inline1.Append(graphic1);

				drawing1.Append(inline1);

				run24.Append(runProperties12);
				run24.Append(drawing1);

				paragraph20.Append(run24);
			}
			tableCell16.Append(paragraph20);

			BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_GoBack", Id = "0" };
			BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "0" };

			tableRow1.Append(tableRowProperties1);
			tableRow1.Append(tableCell1);
			tableRow1.Append(tableCell16);
			tableRow1.Append(bookmarkStart1);
			tableRow1.Append(bookmarkEnd1);
			return tableRow1;
		}
	}
}
