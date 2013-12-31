using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using Vt = DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;
using A14 = DocumentFormat.OpenXml.Office2010.Drawing;
using Ds = DocumentFormat.OpenXml.CustomXmlDataProperties;
using Ovml = DocumentFormat.OpenXml.Vml.Office;
using V = DocumentFormat.OpenXml.Vml;
using M = DocumentFormat.OpenXml.Math;
using System.IO;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Directories
{
	public class IndividualInfo
	{
		public string FamilyName { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public int? PictureId { get; set; }

		public int PeopleId { get; set; }
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CityStateZip { get; set; }
		public string Email { get; set; }
		public string BirthDate { get; set; }
		public string BirthDay { get; set; }
		public string Anniversary { get; set; }
		public string JoinDate { get; set; }
		public string JoinType { get; set; }
		public string HomePhone { get; set; }
		public string CellPhone { get; set; }
		public string WorkPhone { get; set; }
		public string MemberStatus { get; set; }
		public string FellowshipLeader { get; set; }
		public string Age { get; set; }
		public string Spouse { get; set; }
		public string School { get; set; }
		public string Grade { get; set; }
		public string Married { get; set; }
	}
	public class PictureDir : ActionResult
	{
		private IEnumerable<IndividualInfo> q;

		public PictureDir(Guid qid)
		{
			var Db = DbUtil.Db;
			var q0 = Db.PeopleQuery(qid);
			q = from p in q0
					let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
					let spouse = Db.People.Where(pp => pp.PeopleId == p.SpouseId).Select(pp => pp.PreferredName).SingleOrDefault()
					let famname = p.Family.People.Single(hh => hh.PeopleId == hh.Family.HeadOfHouseholdId).LastName
					orderby famname, p.FamilyId, p.PositionInFamilyId, p.Age descending, p.GenderId
					select new IndividualInfo()
					{
						PeopleId = p.PeopleId,
						Title = p.TitleCode,
						FirstName = p.PreferredName,
						LastName = p.LastName,
						Address = p.PrimaryAddress,
						Address2 = p.PrimaryAddress2,
						CityStateZip = p.CityStateZip,
						Email = p.EmailAddress,
						BirthDate = p.BirthMonth + "/" + p.BirthDay + "/" + p.BirthYear,
						BirthDay = " " + p.BirthMonth + "/" + p.BirthDay,
						Anniversary = " " + p.WeddingDate.Value.Month + "/" + p.WeddingDate.Value.Day,
						JoinDate = p.JoinDate.FormatDate(),
						JoinType = p.JoinType.Description,
						HomePhone = p.DoNotPublishPhones == true ? "" : p.HomePhone,
						CellPhone = p.DoNotPublishPhones == true ? "" : p.CellPhone,
						WorkPhone = p.DoNotPublishPhones == true ? "" : p.WorkPhone,
						MemberStatus = p.MemberStatus.Description,
						FellowshipLeader = p.BFClass.LeaderName,
						Spouse = spouse,
						Age = p.Age.ToString(),
						School = p.SchoolOther,
						Grade = p.Grade.ToString(),
						Married = p.MaritalStatus.Description,
						PictureId = p.Picture.LargeId
					};

		}

		// Creates a WordprocessingDocument.
		public override void ExecuteResult(ControllerContext context)
		{
			var Response = context.HttpContext.Response;
			Response.ContentType = "application/vnd.ms-word";
			Response.AddHeader("Content-Disposition", "attachment;filename=CMSPictureDir.docx");
			var ms = new MemoryStream();
			using (var package = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
			{
				CreateParts(package);
			}
			ms.Position = 0;
			ms.CopyTo(Response.OutputStream);
		}
		private void AddRows(MainDocumentPart mainpart, Table t)
		{
			var pr = new PictureRow();
			foreach (var i in q)
			{
				var b = ImageDb.FetchBytes(DbUtil.Db, i.PictureId, 160, 200);
				string iid = "";
				if (b != null)
				{
					ImagePart imagePart = mainpart.AddImagePart(ImagePartType.Jpeg);
					using (var writer = new BinaryWriter(imagePart.GetStream()))
					{
						writer.Write(b);
					}
					iid = mainpart.GetIdOfPart(imagePart);
				}
				var r = pr.GenerateTableRow(i, iid);
				t.Append(r);
			}
		}
		// Adds child parts and generates content of the specified part.
		private void CreateParts(WordprocessingDocument document)
		{
			ExtendedFilePropertiesPart extendedFilePropertiesPart1 = document.AddNewPart<ExtendedFilePropertiesPart>("rId3");
			GenerateExtendedFilePropertiesPart1Content(extendedFilePropertiesPart1);

			MainDocumentPart mainDocumentPart1 = document.AddMainDocumentPart();
			GenerateMainDocumentPart1Content(mainDocumentPart1);

			FontTablePart fontTablePart1 = mainDocumentPart1.AddNewPart<FontTablePart>("rId13");
			GenerateFontTablePart1Content(fontTablePart1);

			StylesWithEffectsPart stylesWithEffectsPart1 = mainDocumentPart1.AddNewPart<StylesWithEffectsPart>("rId3");
			GenerateStylesWithEffectsPart1Content(stylesWithEffectsPart1);

			EndnotesPart endnotesPart1 = mainDocumentPart1.AddNewPart<EndnotesPart>("rId7");
			GenerateEndnotesPart1Content(endnotesPart1);

			FooterPart footerPart1 = mainDocumentPart1.AddNewPart<FooterPart>("rId12");
			GenerateFooterPart1Content(footerPart1);

			StyleDefinitionsPart styleDefinitionsPart1 = mainDocumentPart1.AddNewPart<StyleDefinitionsPart>("rId2");
			GenerateStyleDefinitionsPart1Content(styleDefinitionsPart1);

			CustomXmlPart customXmlPart1 = mainDocumentPart1.AddNewPart<CustomXmlPart>("application/xml", "rId1");
			GenerateCustomXmlPart1Content(customXmlPart1);

			CustomXmlPropertiesPart customXmlPropertiesPart1 = customXmlPart1.AddNewPart<CustomXmlPropertiesPart>("rId1");
			GenerateCustomXmlPropertiesPart1Content(customXmlPropertiesPart1);

			FootnotesPart footnotesPart1 = mainDocumentPart1.AddNewPart<FootnotesPart>("rId6");
			GenerateFootnotesPart1Content(footnotesPart1);

			HeaderPart headerPart1 = mainDocumentPart1.AddNewPart<HeaderPart>("rId11");
			GenerateHeaderPart1Content(headerPart1);

			WebSettingsPart webSettingsPart1 = mainDocumentPart1.AddNewPart<WebSettingsPart>("rId5");
			GenerateWebSettingsPart1Content(webSettingsPart1);

			FooterPart footerPart2 = mainDocumentPart1.AddNewPart<FooterPart>("rId10");
			GenerateFooterPart2Content(footerPart2);

			DocumentSettingsPart documentSettingsPart1 = mainDocumentPart1.AddNewPart<DocumentSettingsPart>("rId4");
			GenerateDocumentSettingsPart1Content(documentSettingsPart1);

			HeaderPart headerPart2 = mainDocumentPart1.AddNewPart<HeaderPart>("rId9");
			GenerateHeaderPart2Content(headerPart2);

			ThemePart themePart1 = mainDocumentPart1.AddNewPart<ThemePart>("rId14");
			GenerateThemePart1Content(themePart1);

			SetPackageProperties(document);
		}

		// Generates content of extendedFilePropertiesPart1.
		private void GenerateExtendedFilePropertiesPart1Content(ExtendedFilePropertiesPart extendedFilePropertiesPart1)
		{
			Ap.Properties properties1 = new Ap.Properties();
			properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
			Ap.Template template1 = new Ap.Template();
			template1.Text = "Normal.dotm";
			Ap.TotalTime totalTime1 = new Ap.TotalTime();
			totalTime1.Text = "4";
			Ap.Pages pages1 = new Ap.Pages();
			pages1.Text = "1";
			Ap.Words words1 = new Ap.Words();
			words1.Text = "30";
			Ap.Characters characters1 = new Ap.Characters();
			characters1.Text = "172";
			Ap.Application application1 = new Ap.Application();
			application1.Text = "Microsoft Office Word";
			Ap.DocumentSecurity documentSecurity1 = new Ap.DocumentSecurity();
			documentSecurity1.Text = "0";
			Ap.Lines lines1 = new Ap.Lines();
			lines1.Text = "1";
			Ap.Paragraphs paragraphs1 = new Ap.Paragraphs();
			paragraphs1.Text = "1";
			Ap.ScaleCrop scaleCrop1 = new Ap.ScaleCrop();
			scaleCrop1.Text = "false";

			Ap.HeadingPairs headingPairs1 = new Ap.HeadingPairs();

			Vt.VTVector vTVector1 = new Vt.VTVector() {BaseType = Vt.VectorBaseValues.Variant, Size = (UInt32Value) 2U};

			Vt.Variant variant1 = new Vt.Variant();
			Vt.VTLPSTR vTLPSTR1 = new Vt.VTLPSTR();
			vTLPSTR1.Text = "Title";

			variant1.Append(vTLPSTR1);

			Vt.Variant variant2 = new Vt.Variant();
			Vt.VTInt32 vTInt321 = new Vt.VTInt32();
			vTInt321.Text = "1";

			variant2.Append(vTInt321);

			vTVector1.Append(variant1);
			vTVector1.Append(variant2);

			headingPairs1.Append(vTVector1);

			Ap.TitlesOfParts titlesOfParts1 = new Ap.TitlesOfParts();

			Vt.VTVector vTVector2 = new Vt.VTVector() {BaseType = Vt.VectorBaseValues.Lpstr, Size = (UInt32Value) 1U};
			Vt.VTLPSTR vTLPSTR2 = new Vt.VTLPSTR();
			vTLPSTR2.Text = "";

			vTVector2.Append(vTLPSTR2);

			titlesOfParts1.Append(vTVector2);
			Ap.Company company1 = new Ap.Company();
			company1.Text = "Microsoft";
			Ap.LinksUpToDate linksUpToDate1 = new Ap.LinksUpToDate();
			linksUpToDate1.Text = "false";
			Ap.CharactersWithSpaces charactersWithSpaces1 = new Ap.CharactersWithSpaces();
			charactersWithSpaces1.Text = "201";
			Ap.SharedDocument sharedDocument1 = new Ap.SharedDocument();
			sharedDocument1.Text = "false";
			Ap.HyperlinksChanged hyperlinksChanged1 = new Ap.HyperlinksChanged();
			hyperlinksChanged1.Text = "false";
			Ap.ApplicationVersion applicationVersion1 = new Ap.ApplicationVersion();
			applicationVersion1.Text = "14.0000";

			properties1.Append(template1);
			properties1.Append(totalTime1);
			properties1.Append(pages1);
			properties1.Append(words1);
			properties1.Append(characters1);
			properties1.Append(application1);
			properties1.Append(documentSecurity1);
			properties1.Append(lines1);
			properties1.Append(paragraphs1);
			properties1.Append(scaleCrop1);
			properties1.Append(headingPairs1);
			properties1.Append(titlesOfParts1);
			properties1.Append(company1);
			properties1.Append(linksUpToDate1);
			properties1.Append(charactersWithSpaces1);
			properties1.Append(sharedDocument1);
			properties1.Append(hyperlinksChanged1);
			properties1.Append(applicationVersion1);

			extendedFilePropertiesPart1.Properties = properties1;
		}

		// Generates content of mainDocumentPart1.
		private void GenerateMainDocumentPart1Content(MainDocumentPart mainDocumentPart1)
		{
			Document document1 = new Document() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			document1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			document1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			document1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			document1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			document1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			document1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			document1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			document1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			document1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			document1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			document1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			document1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			document1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			document1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			document1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			Body body1 = new Body();

			Table table1 = new Table();

			TableProperties tableProperties1 = new TableProperties();
			TableStyle tableStyle1 = new TableStyle() {Val = "TableGrid"};
			TableWidth tableWidth1 = new TableWidth() {Width = "0", Type = TableWidthUnitValues.Auto};

			TableBorders tableBorders1 = new TableBorders();
			InsideVerticalBorder insideVerticalBorder1 = new InsideVerticalBorder()
			                                             {
			                                             	Val = BorderValues.None,
			                                             	Color = "auto",
			                                             	Size = (UInt32Value) 0U,
			                                             	Space = (UInt32Value) 0U
			                                             };

			tableBorders1.Append(insideVerticalBorder1);
			TableLook tableLook1 = new TableLook()
			                       {
			                       	Val = "04A0",
			                       	FirstRow = true,
			                       	LastRow = false,
			                       	FirstColumn = true,
			                       	LastColumn = false,
			                       	NoHorizontalBand = false,
			                       	NoVerticalBand = true
			                       };

			tableProperties1.Append(tableStyle1);
			tableProperties1.Append(tableWidth1);
			tableProperties1.Append(tableBorders1);
			tableProperties1.Append(tableLook1);

			TableGrid tableGrid1 = new TableGrid();
			GridColumn gridColumn1 = new GridColumn() {Width = "4788"};
			GridColumn gridColumn2 = new GridColumn() {Width = "4788"};

			tableGrid1.Append(gridColumn1);
			tableGrid1.Append(gridColumn2);

			table1.Append(tableProperties1);
			table1.Append(tableGrid1);

			

			Paragraph paragraph21 = new Paragraph()
			                        {
			                        	RsidParagraphAddition = "00FB1F22",
			                        	RsidParagraphProperties = "00367B71",
			                        	RsidRunAdditionDefault = "00FB1F22"
			                        };

			SectionProperties sectionProperties1 = new SectionProperties() {RsidR = "00FB1F22", RsidSect = "00367B71"};
			HeaderReference headerReference1 = new HeaderReference() {Type = HeaderFooterValues.Default, Id = "rId9"};
			FooterReference footerReference1 = new FooterReference() {Type = HeaderFooterValues.Default, Id = "rId10"};
			HeaderReference headerReference2 = new HeaderReference() {Type = HeaderFooterValues.First, Id = "rId11"};
			FooterReference footerReference2 = new FooterReference() {Type = HeaderFooterValues.First, Id = "rId12"};
			PageSize pageSize1 = new PageSize() {Width = (UInt32Value) 12240U, Height = (UInt32Value) 15840U};
			PageMargin pageMargin1 = new PageMargin()
			                         {
			                         	Top = 432,
			                         	Right = (UInt32Value) 1440U,
			                         	Bottom = 990,
			                         	Left = (UInt32Value) 1440U,
			                         	Header = (UInt32Value) 540U,
			                         	Footer = (UInt32Value) 270U,
			                         	Gutter = (UInt32Value) 0U
			                         };
			Columns columns1 = new Columns() {Space = "720"};
			TitlePage titlePage1 = new TitlePage();
			DocGrid docGrid1 = new DocGrid() {LinePitch = 360};

			sectionProperties1.Append(headerReference1);
			sectionProperties1.Append(footerReference1);
			sectionProperties1.Append(headerReference2);
			sectionProperties1.Append(footerReference2);
			sectionProperties1.Append(pageSize1);
			sectionProperties1.Append(pageMargin1);
			sectionProperties1.Append(columns1);
			sectionProperties1.Append(titlePage1);
			sectionProperties1.Append(docGrid1);

			AddRows(mainDocumentPart1, table1); // #############################################################

			body1.Append(table1);
			body1.Append(paragraph21);
			body1.Append(sectionProperties1);

			document1.Append(body1);

			mainDocumentPart1.Document = document1;
		}

		// Generates content of fontTablePart1.
		private void GenerateFontTablePart1Content(FontTablePart fontTablePart1)
		{
			Fonts fonts1 = new Fonts() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14"}};
			fonts1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			fonts1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			fonts1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			fonts1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");

			Font font1 = new Font() {Name = "Calibri"};
			Panose1Number panose1Number1 = new Panose1Number() {Val = "020F0502020204030204"};
			FontCharSet fontCharSet1 = new FontCharSet() {Val = "00"};
			FontFamily fontFamily1 = new FontFamily() {Val = FontFamilyValues.Swiss};
			Pitch pitch1 = new Pitch() {Val = FontPitchValues.Variable};
			FontSignature fontSignature1 = new FontSignature()
			                               {
			                               	UnicodeSignature0 = "E10002FF",
			                               	UnicodeSignature1 = "4000ACFF",
			                               	UnicodeSignature2 = "00000009",
			                               	UnicodeSignature3 = "00000000",
			                               	CodePageSignature0 = "0000019F",
			                               	CodePageSignature1 = "00000000"
			                               };

			font1.Append(panose1Number1);
			font1.Append(fontCharSet1);
			font1.Append(fontFamily1);
			font1.Append(pitch1);
			font1.Append(fontSignature1);

			Font font2 = new Font() {Name = "Times New Roman"};
			Panose1Number panose1Number2 = new Panose1Number() {Val = "02020603050405020304"};
			FontCharSet fontCharSet2 = new FontCharSet() {Val = "00"};
			FontFamily fontFamily2 = new FontFamily() {Val = FontFamilyValues.Roman};
			Pitch pitch2 = new Pitch() {Val = FontPitchValues.Variable};
			FontSignature fontSignature2 = new FontSignature()
			                               {
			                               	UnicodeSignature0 = "E0002AFF",
			                               	UnicodeSignature1 = "C0007841",
			                               	UnicodeSignature2 = "00000009",
			                               	UnicodeSignature3 = "00000000",
			                               	CodePageSignature0 = "000001FF",
			                               	CodePageSignature1 = "00000000"
			                               };

			font2.Append(panose1Number2);
			font2.Append(fontCharSet2);
			font2.Append(fontFamily2);
			font2.Append(pitch2);
			font2.Append(fontSignature2);

			Font font3 = new Font() {Name = "Cambria"};
			Panose1Number panose1Number3 = new Panose1Number() {Val = "02040503050406030204"};
			FontCharSet fontCharSet3 = new FontCharSet() {Val = "00"};
			FontFamily fontFamily3 = new FontFamily() {Val = FontFamilyValues.Roman};
			Pitch pitch3 = new Pitch() {Val = FontPitchValues.Variable};
			FontSignature fontSignature3 = new FontSignature()
			                               {
			                               	UnicodeSignature0 = "E00002FF",
			                               	UnicodeSignature1 = "400004FF",
			                               	UnicodeSignature2 = "00000000",
			                               	UnicodeSignature3 = "00000000",
			                               	CodePageSignature0 = "0000019F",
			                               	CodePageSignature1 = "00000000"
			                               };

			font3.Append(panose1Number3);
			font3.Append(fontCharSet3);
			font3.Append(fontFamily3);
			font3.Append(pitch3);
			font3.Append(fontSignature3);

			Font font4 = new Font() {Name = "Tahoma"};
			Panose1Number panose1Number4 = new Panose1Number() {Val = "020B0604030504040204"};
			FontCharSet fontCharSet4 = new FontCharSet() {Val = "00"};
			FontFamily fontFamily4 = new FontFamily() {Val = FontFamilyValues.Swiss};
			NotTrueType notTrueType1 = new NotTrueType();
			Pitch pitch4 = new Pitch() {Val = FontPitchValues.Variable};
			FontSignature fontSignature4 = new FontSignature()
			                               {
			                               	UnicodeSignature0 = "00000003",
			                               	UnicodeSignature1 = "00000000",
			                               	UnicodeSignature2 = "00000000",
			                               	UnicodeSignature3 = "00000000",
			                               	CodePageSignature0 = "00000001",
			                               	CodePageSignature1 = "00000000"
			                               };

			font4.Append(panose1Number4);
			font4.Append(fontCharSet4);
			font4.Append(fontFamily4);
			font4.Append(notTrueType1);
			font4.Append(pitch4);
			font4.Append(fontSignature4);

			fonts1.Append(font1);
			fonts1.Append(font2);
			fonts1.Append(font3);
			fonts1.Append(font4);

			fontTablePart1.Fonts = fonts1;
		}

		// Generates content of stylesWithEffectsPart1.
		private void GenerateStylesWithEffectsPart1Content(StylesWithEffectsPart stylesWithEffectsPart1)
		{
			Styles styles1 = new Styles() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			styles1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			styles1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			styles1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			styles1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			styles1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			styles1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			styles1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			styles1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			styles1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			styles1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			styles1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			styles1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			styles1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			styles1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			styles1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			DocDefaults docDefaults1 = new DocDefaults();

			RunPropertiesDefault runPropertiesDefault1 = new RunPropertiesDefault();

			RunPropertiesBaseStyle runPropertiesBaseStyle1 = new RunPropertiesBaseStyle();
			RunFonts runFonts1 = new RunFonts()
			                     {
			                     	AsciiTheme = ThemeFontValues.MinorHighAnsi,
			                     	HighAnsiTheme = ThemeFontValues.MinorHighAnsi,
			                     	EastAsiaTheme = ThemeFontValues.MinorHighAnsi,
			                     	ComplexScriptTheme = ThemeFontValues.MinorBidi
			                     };
			FontSize fontSize1 = new FontSize() {Val = "22"};
			FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() {Val = "22"};
			Languages languages1 = new Languages() {Val = "en-US", EastAsia = "en-US", Bidi = "ar-SA"};

			runPropertiesBaseStyle1.Append(runFonts1);
			runPropertiesBaseStyle1.Append(fontSize1);
			runPropertiesBaseStyle1.Append(fontSizeComplexScript1);
			runPropertiesBaseStyle1.Append(languages1);

			runPropertiesDefault1.Append(runPropertiesBaseStyle1);

			ParagraphPropertiesDefault paragraphPropertiesDefault1 = new ParagraphPropertiesDefault();

			ParagraphPropertiesBaseStyle paragraphPropertiesBaseStyle1 = new ParagraphPropertiesBaseStyle();
			SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines()
			                                           {After = "200", Line = "276", LineRule = LineSpacingRuleValues.Auto};

			paragraphPropertiesBaseStyle1.Append(spacingBetweenLines1);

			paragraphPropertiesDefault1.Append(paragraphPropertiesBaseStyle1);

			docDefaults1.Append(runPropertiesDefault1);
			docDefaults1.Append(paragraphPropertiesDefault1);

			LatentStyles latentStyles1 = new LatentStyles()
			                             {
			                             	DefaultLockedState = false,
			                             	DefaultUiPriority = 99,
			                             	DefaultSemiHidden = true,
			                             	DefaultUnhideWhenUsed = true,
			                             	DefaultPrimaryStyle = false,
			                             	Count = 267
			                             };
			LatentStyleExceptionInfo latentStyleExceptionInfo1 = new LatentStyleExceptionInfo()
			                                                     {
			                                                     	Name = "Normal",
			                                                     	UiPriority = 0,
			                                                     	SemiHidden = false,
			                                                     	UnhideWhenUsed = false,
			                                                     	PrimaryStyle = true
			                                                     };
			LatentStyleExceptionInfo latentStyleExceptionInfo2 = new LatentStyleExceptionInfo()
			                                                     {
			                                                     	Name = "heading 1",
			                                                     	UiPriority = 9,
			                                                     	SemiHidden = false,
			                                                     	UnhideWhenUsed = false,
			                                                     	PrimaryStyle = true
			                                                     };
			LatentStyleExceptionInfo latentStyleExceptionInfo3 = new LatentStyleExceptionInfo()
			                                                     {Name = "heading 2", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo4 = new LatentStyleExceptionInfo()
			                                                     {Name = "heading 3", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo5 = new LatentStyleExceptionInfo()
			                                                     {Name = "heading 4", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo6 = new LatentStyleExceptionInfo()
			                                                     {Name = "heading 5", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo7 = new LatentStyleExceptionInfo()
			                                                     {Name = "heading 6", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo8 = new LatentStyleExceptionInfo()
			                                                     {Name = "heading 7", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo9 = new LatentStyleExceptionInfo()
			                                                     {Name = "heading 8", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo10 = new LatentStyleExceptionInfo()
			                                                      {Name = "heading 9", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo11 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 1", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo12 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 2", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo13 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 3", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo14 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 4", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo15 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 5", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo16 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 6", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo17 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 7", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo18 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 8", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo19 = new LatentStyleExceptionInfo()
			                                                      {Name = "toc 9", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo20 = new LatentStyleExceptionInfo()
			                                                      {Name = "caption", UiPriority = 35, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo21 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Title",
			                                                      	UiPriority = 10,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo22 = new LatentStyleExceptionInfo()
			                                                      {Name = "Default Paragraph Font", UiPriority = 1};
			LatentStyleExceptionInfo latentStyleExceptionInfo23 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Subtitle",
			                                                      	UiPriority = 11,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo24 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Strong",
			                                                      	UiPriority = 22,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo25 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Emphasis",
			                                                      	UiPriority = 20,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo26 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Table Grid",
			                                                      	UiPriority = 59,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo27 = new LatentStyleExceptionInfo()
			                                                      {Name = "Placeholder Text", UnhideWhenUsed = false};
			LatentStyleExceptionInfo latentStyleExceptionInfo28 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "No Spacing",
			                                                      	UiPriority = 1,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo29 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Shading",
			                                                      	UiPriority = 60,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo30 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light List",
			                                                      	UiPriority = 61,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo31 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Grid",
			                                                      	UiPriority = 62,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo32 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 1",
			                                                      	UiPriority = 63,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo33 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 2",
			                                                      	UiPriority = 64,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo34 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 1",
			                                                      	UiPriority = 65,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo35 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 2",
			                                                      	UiPriority = 66,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo36 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 1",
			                                                      	UiPriority = 67,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo37 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 2",
			                                                      	UiPriority = 68,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo38 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 3",
			                                                      	UiPriority = 69,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo39 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Dark List",
			                                                      	UiPriority = 70,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo40 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Shading",
			                                                      	UiPriority = 71,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo41 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful List",
			                                                      	UiPriority = 72,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo42 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Grid",
			                                                      	UiPriority = 73,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo43 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Shading Accent 1",
			                                                      	UiPriority = 60,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo44 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light List Accent 1",
			                                                      	UiPriority = 61,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo45 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Grid Accent 1",
			                                                      	UiPriority = 62,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo46 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 1 Accent 1",
			                                                      	UiPriority = 63,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo47 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 2 Accent 1",
			                                                      	UiPriority = 64,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo48 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 1 Accent 1",
			                                                      	UiPriority = 65,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo49 = new LatentStyleExceptionInfo()
			                                                      {Name = "Revision", UnhideWhenUsed = false};
			LatentStyleExceptionInfo latentStyleExceptionInfo50 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "List Paragraph",
			                                                      	UiPriority = 34,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo51 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Quote",
			                                                      	UiPriority = 29,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo52 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Intense Quote",
			                                                      	UiPriority = 30,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false,
			                                                      	PrimaryStyle = true
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo53 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 2 Accent 1",
			                                                      	UiPriority = 66,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo54 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 1 Accent 1",
			                                                      	UiPriority = 67,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo55 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 2 Accent 1",
			                                                      	UiPriority = 68,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo56 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 3 Accent 1",
			                                                      	UiPriority = 69,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo57 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Dark List Accent 1",
			                                                      	UiPriority = 70,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo58 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Shading Accent 1",
			                                                      	UiPriority = 71,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo59 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful List Accent 1",
			                                                      	UiPriority = 72,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo60 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Grid Accent 1",
			                                                      	UiPriority = 73,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo61 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Shading Accent 2",
			                                                      	UiPriority = 60,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo62 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light List Accent 2",
			                                                      	UiPriority = 61,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo63 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Grid Accent 2",
			                                                      	UiPriority = 62,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo64 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 1 Accent 2",
			                                                      	UiPriority = 63,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo65 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 2 Accent 2",
			                                                      	UiPriority = 64,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo66 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 1 Accent 2",
			                                                      	UiPriority = 65,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo67 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 2 Accent 2",
			                                                      	UiPriority = 66,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo68 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 1 Accent 2",
			                                                      	UiPriority = 67,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo69 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 2 Accent 2",
			                                                      	UiPriority = 68,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo70 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 3 Accent 2",
			                                                      	UiPriority = 69,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo71 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Dark List Accent 2",
			                                                      	UiPriority = 70,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo72 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Shading Accent 2",
			                                                      	UiPriority = 71,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo73 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful List Accent 2",
			                                                      	UiPriority = 72,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo74 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Grid Accent 2",
			                                                      	UiPriority = 73,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo75 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Shading Accent 3",
			                                                      	UiPriority = 60,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo76 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light List Accent 3",
			                                                      	UiPriority = 61,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo77 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Grid Accent 3",
			                                                      	UiPriority = 62,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo78 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 1 Accent 3",
			                                                      	UiPriority = 63,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo79 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 2 Accent 3",
			                                                      	UiPriority = 64,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo80 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 1 Accent 3",
			                                                      	UiPriority = 65,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo81 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 2 Accent 3",
			                                                      	UiPriority = 66,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo82 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 1 Accent 3",
			                                                      	UiPriority = 67,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo83 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 2 Accent 3",
			                                                      	UiPriority = 68,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo84 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 3 Accent 3",
			                                                      	UiPriority = 69,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo85 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Dark List Accent 3",
			                                                      	UiPriority = 70,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo86 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Shading Accent 3",
			                                                      	UiPriority = 71,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo87 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful List Accent 3",
			                                                      	UiPriority = 72,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo88 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Colorful Grid Accent 3",
			                                                      	UiPriority = 73,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo89 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Shading Accent 4",
			                                                      	UiPriority = 60,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo90 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light List Accent 4",
			                                                      	UiPriority = 61,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo91 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Light Grid Accent 4",
			                                                      	UiPriority = 62,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo92 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 1 Accent 4",
			                                                      	UiPriority = 63,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo93 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Shading 2 Accent 4",
			                                                      	UiPriority = 64,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo94 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 1 Accent 4",
			                                                      	UiPriority = 65,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo95 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium List 2 Accent 4",
			                                                      	UiPriority = 66,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo96 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 1 Accent 4",
			                                                      	UiPriority = 67,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo97 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 2 Accent 4",
			                                                      	UiPriority = 68,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo98 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Medium Grid 3 Accent 4",
			                                                      	UiPriority = 69,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo99 = new LatentStyleExceptionInfo()
			                                                      {
			                                                      	Name = "Dark List Accent 4",
			                                                      	UiPriority = 70,
			                                                      	SemiHidden = false,
			                                                      	UnhideWhenUsed = false
			                                                      };
			LatentStyleExceptionInfo latentStyleExceptionInfo100 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 4",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo101 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 4",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo102 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 4",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo103 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 5",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo104 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 5",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo105 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 5",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo106 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 5",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo107 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 5",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo108 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 5",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo109 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 5",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo110 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 5",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo111 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 5",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo112 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 5",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo113 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 5",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo114 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 5",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo115 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 5",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo116 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 5",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo117 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 6",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo118 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 6",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo119 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 6",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo120 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 6",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo121 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 6",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo122 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 6",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo123 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 6",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo124 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 6",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo125 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 6",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo126 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 6",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo127 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 6",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo128 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 6",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo129 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 6",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo130 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 6",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo131 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Subtle Emphasis",
			                                                       	UiPriority = 19,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo132 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Intense Emphasis",
			                                                       	UiPriority = 21,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo133 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Subtle Reference",
			                                                       	UiPriority = 31,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo134 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Intense Reference",
			                                                       	UiPriority = 32,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo135 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Book Title",
			                                                       	UiPriority = 33,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo136 = new LatentStyleExceptionInfo()
			                                                       {Name = "Bibliography", UiPriority = 37};
			LatentStyleExceptionInfo latentStyleExceptionInfo137 = new LatentStyleExceptionInfo()
			                                                       {Name = "TOC Heading", UiPriority = 39, PrimaryStyle = true};

			latentStyles1.Append(latentStyleExceptionInfo1);
			latentStyles1.Append(latentStyleExceptionInfo2);
			latentStyles1.Append(latentStyleExceptionInfo3);
			latentStyles1.Append(latentStyleExceptionInfo4);
			latentStyles1.Append(latentStyleExceptionInfo5);
			latentStyles1.Append(latentStyleExceptionInfo6);
			latentStyles1.Append(latentStyleExceptionInfo7);
			latentStyles1.Append(latentStyleExceptionInfo8);
			latentStyles1.Append(latentStyleExceptionInfo9);
			latentStyles1.Append(latentStyleExceptionInfo10);
			latentStyles1.Append(latentStyleExceptionInfo11);
			latentStyles1.Append(latentStyleExceptionInfo12);
			latentStyles1.Append(latentStyleExceptionInfo13);
			latentStyles1.Append(latentStyleExceptionInfo14);
			latentStyles1.Append(latentStyleExceptionInfo15);
			latentStyles1.Append(latentStyleExceptionInfo16);
			latentStyles1.Append(latentStyleExceptionInfo17);
			latentStyles1.Append(latentStyleExceptionInfo18);
			latentStyles1.Append(latentStyleExceptionInfo19);
			latentStyles1.Append(latentStyleExceptionInfo20);
			latentStyles1.Append(latentStyleExceptionInfo21);
			latentStyles1.Append(latentStyleExceptionInfo22);
			latentStyles1.Append(latentStyleExceptionInfo23);
			latentStyles1.Append(latentStyleExceptionInfo24);
			latentStyles1.Append(latentStyleExceptionInfo25);
			latentStyles1.Append(latentStyleExceptionInfo26);
			latentStyles1.Append(latentStyleExceptionInfo27);
			latentStyles1.Append(latentStyleExceptionInfo28);
			latentStyles1.Append(latentStyleExceptionInfo29);
			latentStyles1.Append(latentStyleExceptionInfo30);
			latentStyles1.Append(latentStyleExceptionInfo31);
			latentStyles1.Append(latentStyleExceptionInfo32);
			latentStyles1.Append(latentStyleExceptionInfo33);
			latentStyles1.Append(latentStyleExceptionInfo34);
			latentStyles1.Append(latentStyleExceptionInfo35);
			latentStyles1.Append(latentStyleExceptionInfo36);
			latentStyles1.Append(latentStyleExceptionInfo37);
			latentStyles1.Append(latentStyleExceptionInfo38);
			latentStyles1.Append(latentStyleExceptionInfo39);
			latentStyles1.Append(latentStyleExceptionInfo40);
			latentStyles1.Append(latentStyleExceptionInfo41);
			latentStyles1.Append(latentStyleExceptionInfo42);
			latentStyles1.Append(latentStyleExceptionInfo43);
			latentStyles1.Append(latentStyleExceptionInfo44);
			latentStyles1.Append(latentStyleExceptionInfo45);
			latentStyles1.Append(latentStyleExceptionInfo46);
			latentStyles1.Append(latentStyleExceptionInfo47);
			latentStyles1.Append(latentStyleExceptionInfo48);
			latentStyles1.Append(latentStyleExceptionInfo49);
			latentStyles1.Append(latentStyleExceptionInfo50);
			latentStyles1.Append(latentStyleExceptionInfo51);
			latentStyles1.Append(latentStyleExceptionInfo52);
			latentStyles1.Append(latentStyleExceptionInfo53);
			latentStyles1.Append(latentStyleExceptionInfo54);
			latentStyles1.Append(latentStyleExceptionInfo55);
			latentStyles1.Append(latentStyleExceptionInfo56);
			latentStyles1.Append(latentStyleExceptionInfo57);
			latentStyles1.Append(latentStyleExceptionInfo58);
			latentStyles1.Append(latentStyleExceptionInfo59);
			latentStyles1.Append(latentStyleExceptionInfo60);
			latentStyles1.Append(latentStyleExceptionInfo61);
			latentStyles1.Append(latentStyleExceptionInfo62);
			latentStyles1.Append(latentStyleExceptionInfo63);
			latentStyles1.Append(latentStyleExceptionInfo64);
			latentStyles1.Append(latentStyleExceptionInfo65);
			latentStyles1.Append(latentStyleExceptionInfo66);
			latentStyles1.Append(latentStyleExceptionInfo67);
			latentStyles1.Append(latentStyleExceptionInfo68);
			latentStyles1.Append(latentStyleExceptionInfo69);
			latentStyles1.Append(latentStyleExceptionInfo70);
			latentStyles1.Append(latentStyleExceptionInfo71);
			latentStyles1.Append(latentStyleExceptionInfo72);
			latentStyles1.Append(latentStyleExceptionInfo73);
			latentStyles1.Append(latentStyleExceptionInfo74);
			latentStyles1.Append(latentStyleExceptionInfo75);
			latentStyles1.Append(latentStyleExceptionInfo76);
			latentStyles1.Append(latentStyleExceptionInfo77);
			latentStyles1.Append(latentStyleExceptionInfo78);
			latentStyles1.Append(latentStyleExceptionInfo79);
			latentStyles1.Append(latentStyleExceptionInfo80);
			latentStyles1.Append(latentStyleExceptionInfo81);
			latentStyles1.Append(latentStyleExceptionInfo82);
			latentStyles1.Append(latentStyleExceptionInfo83);
			latentStyles1.Append(latentStyleExceptionInfo84);
			latentStyles1.Append(latentStyleExceptionInfo85);
			latentStyles1.Append(latentStyleExceptionInfo86);
			latentStyles1.Append(latentStyleExceptionInfo87);
			latentStyles1.Append(latentStyleExceptionInfo88);
			latentStyles1.Append(latentStyleExceptionInfo89);
			latentStyles1.Append(latentStyleExceptionInfo90);
			latentStyles1.Append(latentStyleExceptionInfo91);
			latentStyles1.Append(latentStyleExceptionInfo92);
			latentStyles1.Append(latentStyleExceptionInfo93);
			latentStyles1.Append(latentStyleExceptionInfo94);
			latentStyles1.Append(latentStyleExceptionInfo95);
			latentStyles1.Append(latentStyleExceptionInfo96);
			latentStyles1.Append(latentStyleExceptionInfo97);
			latentStyles1.Append(latentStyleExceptionInfo98);
			latentStyles1.Append(latentStyleExceptionInfo99);
			latentStyles1.Append(latentStyleExceptionInfo100);
			latentStyles1.Append(latentStyleExceptionInfo101);
			latentStyles1.Append(latentStyleExceptionInfo102);
			latentStyles1.Append(latentStyleExceptionInfo103);
			latentStyles1.Append(latentStyleExceptionInfo104);
			latentStyles1.Append(latentStyleExceptionInfo105);
			latentStyles1.Append(latentStyleExceptionInfo106);
			latentStyles1.Append(latentStyleExceptionInfo107);
			latentStyles1.Append(latentStyleExceptionInfo108);
			latentStyles1.Append(latentStyleExceptionInfo109);
			latentStyles1.Append(latentStyleExceptionInfo110);
			latentStyles1.Append(latentStyleExceptionInfo111);
			latentStyles1.Append(latentStyleExceptionInfo112);
			latentStyles1.Append(latentStyleExceptionInfo113);
			latentStyles1.Append(latentStyleExceptionInfo114);
			latentStyles1.Append(latentStyleExceptionInfo115);
			latentStyles1.Append(latentStyleExceptionInfo116);
			latentStyles1.Append(latentStyleExceptionInfo117);
			latentStyles1.Append(latentStyleExceptionInfo118);
			latentStyles1.Append(latentStyleExceptionInfo119);
			latentStyles1.Append(latentStyleExceptionInfo120);
			latentStyles1.Append(latentStyleExceptionInfo121);
			latentStyles1.Append(latentStyleExceptionInfo122);
			latentStyles1.Append(latentStyleExceptionInfo123);
			latentStyles1.Append(latentStyleExceptionInfo124);
			latentStyles1.Append(latentStyleExceptionInfo125);
			latentStyles1.Append(latentStyleExceptionInfo126);
			latentStyles1.Append(latentStyleExceptionInfo127);
			latentStyles1.Append(latentStyleExceptionInfo128);
			latentStyles1.Append(latentStyleExceptionInfo129);
			latentStyles1.Append(latentStyleExceptionInfo130);
			latentStyles1.Append(latentStyleExceptionInfo131);
			latentStyles1.Append(latentStyleExceptionInfo132);
			latentStyles1.Append(latentStyleExceptionInfo133);
			latentStyles1.Append(latentStyleExceptionInfo134);
			latentStyles1.Append(latentStyleExceptionInfo135);
			latentStyles1.Append(latentStyleExceptionInfo136);
			latentStyles1.Append(latentStyleExceptionInfo137);

			Style style1 = new Style() {Type = StyleValues.Paragraph, StyleId = "Normal", Default = true};
			StyleName styleName1 = new StyleName() {Val = "Normal"};
			PrimaryStyle primaryStyle1 = new PrimaryStyle();

			style1.Append(styleName1);
			style1.Append(primaryStyle1);

			Style style2 = new Style() {Type = StyleValues.Paragraph, StyleId = "Heading2"};
			StyleName styleName2 = new StyleName() {Val = "heading 2"};
			BasedOn basedOn1 = new BasedOn() {Val = "Normal"};
			NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() {Val = "Normal"};
			LinkedStyle linkedStyle1 = new LinkedStyle() {Val = "Heading2Char"};
			UIPriority uIPriority1 = new UIPriority() {Val = 9};
			UnhideWhenUsed unhideWhenUsed1 = new UnhideWhenUsed();
			PrimaryStyle primaryStyle2 = new PrimaryStyle();
			Rsid rsid1 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties1 = new StyleParagraphProperties();
			KeepNext keepNext1 = new KeepNext();
			KeepLines keepLines1 = new KeepLines();
			SpacingBetweenLines spacingBetweenLines2 = new SpacingBetweenLines() {Before = "200", After = "0"};
			OutlineLevel outlineLevel1 = new OutlineLevel() {Val = 1};

			styleParagraphProperties1.Append(keepNext1);
			styleParagraphProperties1.Append(keepLines1);
			styleParagraphProperties1.Append(spacingBetweenLines2);
			styleParagraphProperties1.Append(outlineLevel1);

			StyleRunProperties styleRunProperties1 = new StyleRunProperties();
			RunFonts runFonts2 = new RunFonts()
			                     {
			                     	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                     	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                     	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                     	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                     };
			Bold bold1 = new Bold();
			BoldComplexScript boldComplexScript1 = new BoldComplexScript();
			Color color1 = new Color() {Val = "4F81BD", ThemeColor = ThemeColorValues.Accent1};
			FontSize fontSize2 = new FontSize() {Val = "26"};
			FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() {Val = "26"};

			styleRunProperties1.Append(runFonts2);
			styleRunProperties1.Append(bold1);
			styleRunProperties1.Append(boldComplexScript1);
			styleRunProperties1.Append(color1);
			styleRunProperties1.Append(fontSize2);
			styleRunProperties1.Append(fontSizeComplexScript2);

			style2.Append(styleName2);
			style2.Append(basedOn1);
			style2.Append(nextParagraphStyle1);
			style2.Append(linkedStyle1);
			style2.Append(uIPriority1);
			style2.Append(unhideWhenUsed1);
			style2.Append(primaryStyle2);
			style2.Append(rsid1);
			style2.Append(styleParagraphProperties1);
			style2.Append(styleRunProperties1);

			Style style3 = new Style() {Type = StyleValues.Character, StyleId = "DefaultParagraphFont", Default = true};
			StyleName styleName3 = new StyleName() {Val = "Default Paragraph Font"};
			UIPriority uIPriority2 = new UIPriority() {Val = 1};
			SemiHidden semiHidden1 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed2 = new UnhideWhenUsed();

			style3.Append(styleName3);
			style3.Append(uIPriority2);
			style3.Append(semiHidden1);
			style3.Append(unhideWhenUsed2);

			Style style4 = new Style() {Type = StyleValues.Table, StyleId = "TableNormal", Default = true};
			StyleName styleName4 = new StyleName() {Val = "Normal Table"};
			UIPriority uIPriority3 = new UIPriority() {Val = 99};
			SemiHidden semiHidden2 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed3 = new UnhideWhenUsed();

			StyleTableProperties styleTableProperties1 = new StyleTableProperties();
			TableIndentation tableIndentation1 = new TableIndentation() {Width = 0, Type = TableWidthUnitValues.Dxa};

			TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();
			TopMargin topMargin1 = new TopMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellLeftMargin tableCellLeftMargin1 = new TableCellLeftMargin() {Width = 108, Type = TableWidthValues.Dxa};
			BottomMargin bottomMargin1 = new BottomMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellRightMargin tableCellRightMargin1 = new TableCellRightMargin() {Width = 108, Type = TableWidthValues.Dxa};

			tableCellMarginDefault1.Append(topMargin1);
			tableCellMarginDefault1.Append(tableCellLeftMargin1);
			tableCellMarginDefault1.Append(bottomMargin1);
			tableCellMarginDefault1.Append(tableCellRightMargin1);

			styleTableProperties1.Append(tableIndentation1);
			styleTableProperties1.Append(tableCellMarginDefault1);

			style4.Append(styleName4);
			style4.Append(uIPriority3);
			style4.Append(semiHidden2);
			style4.Append(unhideWhenUsed3);
			style4.Append(styleTableProperties1);

			Style style5 = new Style() {Type = StyleValues.Numbering, StyleId = "NoList", Default = true};
			StyleName styleName5 = new StyleName() {Val = "No List"};
			UIPriority uIPriority4 = new UIPriority() {Val = 99};
			SemiHidden semiHidden3 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed4 = new UnhideWhenUsed();

			style5.Append(styleName5);
			style5.Append(uIPriority4);
			style5.Append(semiHidden3);
			style5.Append(unhideWhenUsed4);

			Style style6 = new Style() {Type = StyleValues.Table, StyleId = "TableGrid"};
			StyleName styleName6 = new StyleName() {Val = "Table Grid"};
			BasedOn basedOn2 = new BasedOn() {Val = "TableNormal"};
			UIPriority uIPriority5 = new UIPriority() {Val = 59};
			Rsid rsid2 = new Rsid() {Val = "00F06304"};

			StyleParagraphProperties styleParagraphProperties2 = new StyleParagraphProperties();
			SpacingBetweenLines spacingBetweenLines3 = new SpacingBetweenLines()
			                                           {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties2.Append(spacingBetweenLines3);

			StyleTableProperties styleTableProperties2 = new StyleTableProperties();
			TableIndentation tableIndentation2 = new TableIndentation() {Width = 0, Type = TableWidthUnitValues.Dxa};

			TableBorders tableBorders3 = new TableBorders();
			TopBorder topBorder2 = new TopBorder()
			                       {Val = BorderValues.Single, Color = "auto", Size = (UInt32Value) 4U, Space = (UInt32Value) 0U};
			LeftBorder leftBorder2 = new LeftBorder()
			                         {
			                         	Val = BorderValues.Single,
			                         	Color = "auto",
			                         	Size = (UInt32Value) 4U,
			                         	Space = (UInt32Value) 0U
			                         };
			BottomBorder bottomBorder2 = new BottomBorder()
			                             {
			                             	Val = BorderValues.Single,
			                             	Color = "auto",
			                             	Size = (UInt32Value) 4U,
			                             	Space = (UInt32Value) 0U
			                             };
			RightBorder rightBorder2 = new RightBorder()
			                           {
			                           	Val = BorderValues.Single,
			                           	Color = "auto",
			                           	Size = (UInt32Value) 4U,
			                           	Space = (UInt32Value) 0U
			                           };
			InsideHorizontalBorder insideHorizontalBorder2 = new InsideHorizontalBorder()
			                                                 {
			                                                 	Val = BorderValues.Single,
			                                                 	Color = "auto",
			                                                 	Size = (UInt32Value) 4U,
			                                                 	Space = (UInt32Value) 0U
			                                                 };
			InsideVerticalBorder insideVerticalBorder3 = new InsideVerticalBorder()
			                                             {
			                                             	Val = BorderValues.Single,
			                                             	Color = "auto",
			                                             	Size = (UInt32Value) 4U,
			                                             	Space = (UInt32Value) 0U
			                                             };

			tableBorders3.Append(topBorder2);
			tableBorders3.Append(leftBorder2);
			tableBorders3.Append(bottomBorder2);
			tableBorders3.Append(rightBorder2);
			tableBorders3.Append(insideHorizontalBorder2);
			tableBorders3.Append(insideVerticalBorder3);

			TableCellMarginDefault tableCellMarginDefault2 = new TableCellMarginDefault();
			TopMargin topMargin2 = new TopMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellLeftMargin tableCellLeftMargin2 = new TableCellLeftMargin() {Width = 108, Type = TableWidthValues.Dxa};
			BottomMargin bottomMargin2 = new BottomMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellRightMargin tableCellRightMargin2 = new TableCellRightMargin() {Width = 108, Type = TableWidthValues.Dxa};

			tableCellMarginDefault2.Append(topMargin2);
			tableCellMarginDefault2.Append(tableCellLeftMargin2);
			tableCellMarginDefault2.Append(bottomMargin2);
			tableCellMarginDefault2.Append(tableCellRightMargin2);

			styleTableProperties2.Append(tableIndentation2);
			styleTableProperties2.Append(tableBorders3);
			styleTableProperties2.Append(tableCellMarginDefault2);

			style6.Append(styleName6);
			style6.Append(basedOn2);
			style6.Append(uIPriority5);
			style6.Append(rsid2);
			style6.Append(styleParagraphProperties2);
			style6.Append(styleTableProperties2);

			Style style7 = new Style() {Type = StyleValues.Paragraph, StyleId = "Header"};
			StyleName styleName7 = new StyleName() {Val = "header"};
			BasedOn basedOn3 = new BasedOn() {Val = "Normal"};
			LinkedStyle linkedStyle2 = new LinkedStyle() {Val = "HeaderChar"};
			UIPriority uIPriority6 = new UIPriority() {Val = 99};
			UnhideWhenUsed unhideWhenUsed5 = new UnhideWhenUsed();
			Rsid rsid3 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties3 = new StyleParagraphProperties();

			Tabs tabs1 = new Tabs();
			TabStop tabStop1 = new TabStop() {Val = TabStopValues.Center, Position = 4680};
			TabStop tabStop2 = new TabStop() {Val = TabStopValues.Right, Position = 9360};

			tabs1.Append(tabStop1);
			tabs1.Append(tabStop2);
			SpacingBetweenLines spacingBetweenLines4 = new SpacingBetweenLines()
			                                           {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties3.Append(tabs1);
			styleParagraphProperties3.Append(spacingBetweenLines4);

			style7.Append(styleName7);
			style7.Append(basedOn3);
			style7.Append(linkedStyle2);
			style7.Append(uIPriority6);
			style7.Append(unhideWhenUsed5);
			style7.Append(rsid3);
			style7.Append(styleParagraphProperties3);

			Style style8 = new Style() {Type = StyleValues.Character, StyleId = "HeaderChar", CustomStyle = true};
			StyleName styleName8 = new StyleName() {Val = "Header Char"};
			BasedOn basedOn4 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle3 = new LinkedStyle() {Val = "Header"};
			UIPriority uIPriority7 = new UIPriority() {Val = 99};
			Rsid rsid4 = new Rsid() {Val = "00442AD3"};

			style8.Append(styleName8);
			style8.Append(basedOn4);
			style8.Append(linkedStyle3);
			style8.Append(uIPriority7);
			style8.Append(rsid4);

			Style style9 = new Style() {Type = StyleValues.Paragraph, StyleId = "Footer"};
			StyleName styleName9 = new StyleName() {Val = "footer"};
			BasedOn basedOn5 = new BasedOn() {Val = "Normal"};
			LinkedStyle linkedStyle4 = new LinkedStyle() {Val = "FooterChar"};
			UIPriority uIPriority8 = new UIPriority() {Val = 99};
			UnhideWhenUsed unhideWhenUsed6 = new UnhideWhenUsed();
			Rsid rsid5 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties4 = new StyleParagraphProperties();

			Tabs tabs2 = new Tabs();
			TabStop tabStop3 = new TabStop() {Val = TabStopValues.Center, Position = 4680};
			TabStop tabStop4 = new TabStop() {Val = TabStopValues.Right, Position = 9360};

			tabs2.Append(tabStop3);
			tabs2.Append(tabStop4);
			SpacingBetweenLines spacingBetweenLines5 = new SpacingBetweenLines()
			                                           {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties4.Append(tabs2);
			styleParagraphProperties4.Append(spacingBetweenLines5);

			style9.Append(styleName9);
			style9.Append(basedOn5);
			style9.Append(linkedStyle4);
			style9.Append(uIPriority8);
			style9.Append(unhideWhenUsed6);
			style9.Append(rsid5);
			style9.Append(styleParagraphProperties4);

			Style style10 = new Style() {Type = StyleValues.Character, StyleId = "FooterChar", CustomStyle = true};
			StyleName styleName10 = new StyleName() {Val = "Footer Char"};
			BasedOn basedOn6 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle5 = new LinkedStyle() {Val = "Footer"};
			UIPriority uIPriority9 = new UIPriority() {Val = 99};
			Rsid rsid6 = new Rsid() {Val = "00442AD3"};

			style10.Append(styleName10);
			style10.Append(basedOn6);
			style10.Append(linkedStyle5);
			style10.Append(uIPriority9);
			style10.Append(rsid6);

			Style style11 = new Style() {Type = StyleValues.Paragraph, StyleId = "BalloonText"};
			StyleName styleName11 = new StyleName() {Val = "Balloon Text"};
			BasedOn basedOn7 = new BasedOn() {Val = "Normal"};
			LinkedStyle linkedStyle6 = new LinkedStyle() {Val = "BalloonTextChar"};
			UIPriority uIPriority10 = new UIPriority() {Val = 99};
			SemiHidden semiHidden4 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed7 = new UnhideWhenUsed();
			Rsid rsid7 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties5 = new StyleParagraphProperties();
			SpacingBetweenLines spacingBetweenLines6 = new SpacingBetweenLines()
			                                           {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties5.Append(spacingBetweenLines6);

			StyleRunProperties styleRunProperties2 = new StyleRunProperties();
			RunFonts runFonts3 = new RunFonts() {Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma"};
			FontSize fontSize3 = new FontSize() {Val = "16"};
			FontSizeComplexScript fontSizeComplexScript3 = new FontSizeComplexScript() {Val = "16"};

			styleRunProperties2.Append(runFonts3);
			styleRunProperties2.Append(fontSize3);
			styleRunProperties2.Append(fontSizeComplexScript3);

			style11.Append(styleName11);
			style11.Append(basedOn7);
			style11.Append(linkedStyle6);
			style11.Append(uIPriority10);
			style11.Append(semiHidden4);
			style11.Append(unhideWhenUsed7);
			style11.Append(rsid7);
			style11.Append(styleParagraphProperties5);
			style11.Append(styleRunProperties2);

			Style style12 = new Style() {Type = StyleValues.Character, StyleId = "BalloonTextChar", CustomStyle = true};
			StyleName styleName12 = new StyleName() {Val = "Balloon Text Char"};
			BasedOn basedOn8 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle7 = new LinkedStyle() {Val = "BalloonText"};
			UIPriority uIPriority11 = new UIPriority() {Val = 99};
			SemiHidden semiHidden5 = new SemiHidden();
			Rsid rsid8 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties3 = new StyleRunProperties();
			RunFonts runFonts4 = new RunFonts() {Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma"};
			FontSize fontSize4 = new FontSize() {Val = "16"};
			FontSizeComplexScript fontSizeComplexScript4 = new FontSizeComplexScript() {Val = "16"};

			styleRunProperties3.Append(runFonts4);
			styleRunProperties3.Append(fontSize4);
			styleRunProperties3.Append(fontSizeComplexScript4);

			style12.Append(styleName12);
			style12.Append(basedOn8);
			style12.Append(linkedStyle7);
			style12.Append(uIPriority11);
			style12.Append(semiHidden5);
			style12.Append(rsid8);
			style12.Append(styleRunProperties3);

			Style style13 = new Style() {Type = StyleValues.Paragraph, StyleId = "Title"};
			StyleName styleName13 = new StyleName() {Val = "Title"};
			BasedOn basedOn9 = new BasedOn() {Val = "Normal"};
			NextParagraphStyle nextParagraphStyle2 = new NextParagraphStyle() {Val = "Normal"};
			LinkedStyle linkedStyle8 = new LinkedStyle() {Val = "TitleChar"};
			UIPriority uIPriority12 = new UIPriority() {Val = 10};
			PrimaryStyle primaryStyle3 = new PrimaryStyle();
			Rsid rsid9 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties6 = new StyleParagraphProperties();

			ParagraphBorders paragraphBorders1 = new ParagraphBorders();
			BottomBorder bottomBorder3 = new BottomBorder()
			                             {
			                             	Val = BorderValues.Single,
			                             	Color = "4F81BD",
			                             	ThemeColor = ThemeColorValues.Accent1,
			                             	Size = (UInt32Value) 8U,
			                             	Space = (UInt32Value) 4U
			                             };

			paragraphBorders1.Append(bottomBorder3);
			SpacingBetweenLines spacingBetweenLines7 = new SpacingBetweenLines()
			                                           {After = "300", Line = "240", LineRule = LineSpacingRuleValues.Auto};
			ContextualSpacing contextualSpacing1 = new ContextualSpacing();

			styleParagraphProperties6.Append(paragraphBorders1);
			styleParagraphProperties6.Append(spacingBetweenLines7);
			styleParagraphProperties6.Append(contextualSpacing1);

			StyleRunProperties styleRunProperties4 = new StyleRunProperties();
			RunFonts runFonts5 = new RunFonts()
			                     {
			                     	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                     	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                     	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                     	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                     };
			Color color2 = new Color() {Val = "17365D", ThemeColor = ThemeColorValues.Text2, ThemeShade = "BF"};
			Spacing spacing1 = new Spacing() {Val = 5};
			Kern kern1 = new Kern() {Val = (UInt32Value) 28U};
			FontSize fontSize5 = new FontSize() {Val = "52"};
			FontSizeComplexScript fontSizeComplexScript5 = new FontSizeComplexScript() {Val = "52"};

			styleRunProperties4.Append(runFonts5);
			styleRunProperties4.Append(color2);
			styleRunProperties4.Append(spacing1);
			styleRunProperties4.Append(kern1);
			styleRunProperties4.Append(fontSize5);
			styleRunProperties4.Append(fontSizeComplexScript5);

			style13.Append(styleName13);
			style13.Append(basedOn9);
			style13.Append(nextParagraphStyle2);
			style13.Append(linkedStyle8);
			style13.Append(uIPriority12);
			style13.Append(primaryStyle3);
			style13.Append(rsid9);
			style13.Append(styleParagraphProperties6);
			style13.Append(styleRunProperties4);

			Style style14 = new Style() {Type = StyleValues.Character, StyleId = "TitleChar", CustomStyle = true};
			StyleName styleName14 = new StyleName() {Val = "Title Char"};
			BasedOn basedOn10 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle9 = new LinkedStyle() {Val = "Title"};
			UIPriority uIPriority13 = new UIPriority() {Val = 10};
			Rsid rsid10 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties5 = new StyleRunProperties();
			RunFonts runFonts6 = new RunFonts()
			                     {
			                     	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                     	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                     	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                     	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                     };
			Color color3 = new Color() {Val = "17365D", ThemeColor = ThemeColorValues.Text2, ThemeShade = "BF"};
			Spacing spacing2 = new Spacing() {Val = 5};
			Kern kern2 = new Kern() {Val = (UInt32Value) 28U};
			FontSize fontSize6 = new FontSize() {Val = "52"};
			FontSizeComplexScript fontSizeComplexScript6 = new FontSizeComplexScript() {Val = "52"};

			styleRunProperties5.Append(runFonts6);
			styleRunProperties5.Append(color3);
			styleRunProperties5.Append(spacing2);
			styleRunProperties5.Append(kern2);
			styleRunProperties5.Append(fontSize6);
			styleRunProperties5.Append(fontSizeComplexScript6);

			style14.Append(styleName14);
			style14.Append(basedOn10);
			style14.Append(linkedStyle9);
			style14.Append(uIPriority13);
			style14.Append(rsid10);
			style14.Append(styleRunProperties5);

			Style style15 = new Style() {Type = StyleValues.Character, StyleId = "Strong"};
			StyleName styleName15 = new StyleName() {Val = "Strong"};
			BasedOn basedOn11 = new BasedOn() {Val = "DefaultParagraphFont"};
			UIPriority uIPriority14 = new UIPriority() {Val = 22};
			PrimaryStyle primaryStyle4 = new PrimaryStyle();
			Rsid rsid11 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties6 = new StyleRunProperties();
			Bold bold2 = new Bold();
			BoldComplexScript boldComplexScript2 = new BoldComplexScript();

			styleRunProperties6.Append(bold2);
			styleRunProperties6.Append(boldComplexScript2);

			style15.Append(styleName15);
			style15.Append(basedOn11);
			style15.Append(uIPriority14);
			style15.Append(primaryStyle4);
			style15.Append(rsid11);
			style15.Append(styleRunProperties6);

			Style style16 = new Style() {Type = StyleValues.Character, StyleId = "Heading2Char", CustomStyle = true};
			StyleName styleName16 = new StyleName() {Val = "Heading 2 Char"};
			BasedOn basedOn12 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle10 = new LinkedStyle() {Val = "Heading2"};
			UIPriority uIPriority15 = new UIPriority() {Val = 9};
			Rsid rsid12 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties7 = new StyleRunProperties();
			RunFonts runFonts7 = new RunFonts()
			                     {
			                     	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                     	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                     	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                     	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                     };
			Bold bold3 = new Bold();
			BoldComplexScript boldComplexScript3 = new BoldComplexScript();
			Color color4 = new Color() {Val = "4F81BD", ThemeColor = ThemeColorValues.Accent1};
			FontSize fontSize7 = new FontSize() {Val = "26"};
			FontSizeComplexScript fontSizeComplexScript7 = new FontSizeComplexScript() {Val = "26"};

			styleRunProperties7.Append(runFonts7);
			styleRunProperties7.Append(bold3);
			styleRunProperties7.Append(boldComplexScript3);
			styleRunProperties7.Append(color4);
			styleRunProperties7.Append(fontSize7);
			styleRunProperties7.Append(fontSizeComplexScript7);

			style16.Append(styleName16);
			style16.Append(basedOn12);
			style16.Append(linkedStyle10);
			style16.Append(uIPriority15);
			style16.Append(rsid12);
			style16.Append(styleRunProperties7);

			styles1.Append(docDefaults1);
			styles1.Append(latentStyles1);
			styles1.Append(style1);
			styles1.Append(style2);
			styles1.Append(style3);
			styles1.Append(style4);
			styles1.Append(style5);
			styles1.Append(style6);
			styles1.Append(style7);
			styles1.Append(style8);
			styles1.Append(style9);
			styles1.Append(style10);
			styles1.Append(style11);
			styles1.Append(style12);
			styles1.Append(style13);
			styles1.Append(style14);
			styles1.Append(style15);
			styles1.Append(style16);

			stylesWithEffectsPart1.Styles = styles1;
		}

		// Generates content of endnotesPart1.
		private void GenerateEndnotesPart1Content(EndnotesPart endnotesPart1)
		{
			Endnotes endnotes1 = new Endnotes() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			endnotes1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			endnotes1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			endnotes1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			endnotes1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			endnotes1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			endnotes1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			endnotes1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			endnotes1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			endnotes1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			endnotes1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			endnotes1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			endnotes1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			endnotes1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			endnotes1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			endnotes1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			Endnote endnote1 = new Endnote() {Type = FootnoteEndnoteValues.Separator, Id = -1};

			Paragraph paragraph22 = new Paragraph()
			                        {
			                        	RsidParagraphAddition = "00B32099",
			                        	RsidParagraphProperties = "00442AD3",
			                        	RsidRunAdditionDefault = "00B32099"
			                        };

			ParagraphProperties paragraphProperties3 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines8 = new SpacingBetweenLines()
			                                           {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			paragraphProperties3.Append(spacingBetweenLines8);

			Run run25 = new Run();
			SeparatorMark separatorMark1 = new SeparatorMark();

			run25.Append(separatorMark1);

			paragraph22.Append(paragraphProperties3);
			paragraph22.Append(run25);

			endnote1.Append(paragraph22);

			Endnote endnote2 = new Endnote() {Type = FootnoteEndnoteValues.ContinuationSeparator, Id = 0};

			Paragraph paragraph23 = new Paragraph()
			                        {
			                        	RsidParagraphAddition = "00B32099",
			                        	RsidParagraphProperties = "00442AD3",
			                        	RsidRunAdditionDefault = "00B32099"
			                        };

			ParagraphProperties paragraphProperties4 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines9 = new SpacingBetweenLines()
			                                           {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			paragraphProperties4.Append(spacingBetweenLines9);

			Run run26 = new Run();
			ContinuationSeparatorMark continuationSeparatorMark1 = new ContinuationSeparatorMark();

			run26.Append(continuationSeparatorMark1);

			paragraph23.Append(paragraphProperties4);
			paragraph23.Append(run26);

			endnote2.Append(paragraph23);

			endnotes1.Append(endnote1);
			endnotes1.Append(endnote2);

			endnotesPart1.Endnotes = endnotes1;
		}

		// Generates content of footerPart1.
		private void GenerateFooterPart1Content(FooterPart footerPart1)
		{
			Footer footer1 = new Footer() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			footer1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			footer1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			footer1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			footer1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			footer1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			footer1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			footer1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			footer1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			footer1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			footer1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			footer1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			footer1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			footer1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			footer1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			footer1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			Paragraph paragraph24 = new Paragraph()
			                        {
			                        	RsidParagraphAddition = "0005059E",
			                        	RsidParagraphProperties = "00367B71",
			                        	RsidRunAdditionDefault = "0005059E"
			                        };

			ParagraphProperties paragraphProperties5 = new ParagraphProperties();
			ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() {Val = "Footer"};

			Tabs tabs3 = new Tabs();
			TabStop tabStop5 = new TabStop() {Val = TabStopValues.Clear, Position = 4680};
			TabStop tabStop6 = new TabStop() {Val = TabStopValues.Clear, Position = 9360};
			TabStop tabStop7 = new TabStop() {Val = TabStopValues.Left, Position = 1896};

			tabs3.Append(tabStop5);
			tabs3.Append(tabStop6);
			tabs3.Append(tabStop7);

			paragraphProperties5.Append(paragraphStyleId1);
			paragraphProperties5.Append(tabs3);

			Run run27 = new Run();

			RunProperties runProperties13 = new RunProperties();
			Color color5 = new Color() {Val = "808080", ThemeColor = ThemeColorValues.Background1, ThemeShade = "80"};
			Spacing spacing3 = new Spacing() {Val = 60};

			runProperties13.Append(color5);
			runProperties13.Append(spacing3);
			Text text23 = new Text();
			text23.Text = "Page";

			run27.Append(runProperties13);
			run27.Append(text23);

			Run run28 = new Run();
			Text text24 = new Text() {Space = SpaceProcessingModeValues.Preserve};
			text24.Text = " | ";

			run28.Append(text24);

			Run run29 = new Run();
			FieldChar fieldChar1 = new FieldChar() {FieldCharType = FieldCharValues.Begin};

			run29.Append(fieldChar1);

			Run run30 = new Run();
			FieldCode fieldCode1 = new FieldCode() {Space = SpaceProcessingModeValues.Preserve};
			fieldCode1.Text = " PAGE   \\* MERGEFORMAT ";

			run30.Append(fieldCode1);

			Run run31 = new Run();
			FieldChar fieldChar2 = new FieldChar() {FieldCharType = FieldCharValues.Separate};

			run31.Append(fieldChar2);

			Run run32 = new Run() {RsidRunProperties = "007977E5", RsidRunAddition = "007977E5"};

			RunProperties runProperties14 = new RunProperties();
			Bold bold4 = new Bold();
			BoldComplexScript boldComplexScript4 = new BoldComplexScript();
			NoProof noProof14 = new NoProof();

			runProperties14.Append(bold4);
			runProperties14.Append(boldComplexScript4);
			runProperties14.Append(noProof14);
			Text text25 = new Text();
			text25.Text = "1";

			run32.Append(runProperties14);
			run32.Append(text25);

			Run run33 = new Run();

			RunProperties runProperties15 = new RunProperties();
			Bold bold5 = new Bold();
			BoldComplexScript boldComplexScript5 = new BoldComplexScript();
			NoProof noProof15 = new NoProof();

			runProperties15.Append(bold5);
			runProperties15.Append(boldComplexScript5);
			runProperties15.Append(noProof15);
			FieldChar fieldChar3 = new FieldChar() {FieldCharType = FieldCharValues.End};

			run33.Append(runProperties15);
			run33.Append(fieldChar3);

			Run run34 = new Run();

			RunProperties runProperties16 = new RunProperties();
			Bold bold6 = new Bold();
			BoldComplexScript boldComplexScript6 = new BoldComplexScript();
			NoProof noProof16 = new NoProof();

			runProperties16.Append(bold6);
			runProperties16.Append(boldComplexScript6);
			runProperties16.Append(noProof16);
			Text text26 = new Text() {Space = SpaceProcessingModeValues.Preserve};
			text26.Text = " of ";

			run34.Append(runProperties16);
			run34.Append(text26);

			Run run35 = new Run();

			RunProperties runProperties17 = new RunProperties();
			Bold bold7 = new Bold();
			BoldComplexScript boldComplexScript7 = new BoldComplexScript();
			NoProof noProof17 = new NoProof();

			runProperties17.Append(bold7);
			runProperties17.Append(boldComplexScript7);
			runProperties17.Append(noProof17);
			FieldChar fieldChar4 = new FieldChar() {FieldCharType = FieldCharValues.Begin};

			run35.Append(runProperties17);
			run35.Append(fieldChar4);

			Run run36 = new Run();

			RunProperties runProperties18 = new RunProperties();
			Bold bold8 = new Bold();
			BoldComplexScript boldComplexScript8 = new BoldComplexScript();
			NoProof noProof18 = new NoProof();

			runProperties18.Append(bold8);
			runProperties18.Append(boldComplexScript8);
			runProperties18.Append(noProof18);
			FieldCode fieldCode2 = new FieldCode() {Space = SpaceProcessingModeValues.Preserve};
			fieldCode2.Text = " NUMPAGES  \\* Arabic  \\* MERGEFORMAT ";

			run36.Append(runProperties18);
			run36.Append(fieldCode2);

			Run run37 = new Run();

			RunProperties runProperties19 = new RunProperties();
			Bold bold9 = new Bold();
			BoldComplexScript boldComplexScript9 = new BoldComplexScript();
			NoProof noProof19 = new NoProof();

			runProperties19.Append(bold9);
			runProperties19.Append(boldComplexScript9);
			runProperties19.Append(noProof19);
			FieldChar fieldChar5 = new FieldChar() {FieldCharType = FieldCharValues.Separate};

			run37.Append(runProperties19);
			run37.Append(fieldChar5);

			Run run38 = new Run() {RsidRunAddition = "007977E5"};

			RunProperties runProperties20 = new RunProperties();
			Bold bold10 = new Bold();
			BoldComplexScript boldComplexScript10 = new BoldComplexScript();
			NoProof noProof20 = new NoProof();

			runProperties20.Append(bold10);
			runProperties20.Append(boldComplexScript10);
			runProperties20.Append(noProof20);
			Text text27 = new Text();
			text27.Text = "1";

			run38.Append(runProperties20);
			run38.Append(text27);

			Run run39 = new Run();

			RunProperties runProperties21 = new RunProperties();
			Bold bold11 = new Bold();
			BoldComplexScript boldComplexScript11 = new BoldComplexScript();
			NoProof noProof21 = new NoProof();

			runProperties21.Append(bold11);
			runProperties21.Append(boldComplexScript11);
			runProperties21.Append(noProof21);
			FieldChar fieldChar6 = new FieldChar() {FieldCharType = FieldCharValues.End};

			run39.Append(runProperties21);
			run39.Append(fieldChar6);

			Run run40 = new Run();

			RunProperties runProperties22 = new RunProperties();
			Bold bold12 = new Bold();
			BoldComplexScript boldComplexScript12 = new BoldComplexScript();
			NoProof noProof22 = new NoProof();

			runProperties22.Append(bold12);
			runProperties22.Append(boldComplexScript12);
			runProperties22.Append(noProof22);
			TabChar tabChar1 = new TabChar();

			run40.Append(runProperties22);
			run40.Append(tabChar1);

			paragraph24.Append(paragraphProperties5);
			paragraph24.Append(run27);
			paragraph24.Append(run28);
			paragraph24.Append(run29);
			paragraph24.Append(run30);
			paragraph24.Append(run31);
			paragraph24.Append(run32);
			paragraph24.Append(run33);
			paragraph24.Append(run34);
			paragraph24.Append(run35);
			paragraph24.Append(run36);
			paragraph24.Append(run37);
			paragraph24.Append(run38);
			paragraph24.Append(run39);
			paragraph24.Append(run40);

			footer1.Append(paragraph24);

			footerPart1.Footer = footer1;
		}

		// Generates content of styleDefinitionsPart1.
		private void GenerateStyleDefinitionsPart1Content(StyleDefinitionsPart styleDefinitionsPart1)
		{
			Styles styles2 = new Styles() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14"}};
			styles2.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			styles2.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			styles2.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			styles2.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");

			DocDefaults docDefaults2 = new DocDefaults();

			RunPropertiesDefault runPropertiesDefault2 = new RunPropertiesDefault();

			RunPropertiesBaseStyle runPropertiesBaseStyle2 = new RunPropertiesBaseStyle();
			RunFonts runFonts8 = new RunFonts()
			                     {
			                     	AsciiTheme = ThemeFontValues.MinorHighAnsi,
			                     	HighAnsiTheme = ThemeFontValues.MinorHighAnsi,
			                     	EastAsiaTheme = ThemeFontValues.MinorHighAnsi,
			                     	ComplexScriptTheme = ThemeFontValues.MinorBidi
			                     };
			FontSize fontSize8 = new FontSize() {Val = "22"};
			FontSizeComplexScript fontSizeComplexScript8 = new FontSizeComplexScript() {Val = "22"};
			Languages languages2 = new Languages() {Val = "en-US", EastAsia = "en-US", Bidi = "ar-SA"};

			runPropertiesBaseStyle2.Append(runFonts8);
			runPropertiesBaseStyle2.Append(fontSize8);
			runPropertiesBaseStyle2.Append(fontSizeComplexScript8);
			runPropertiesBaseStyle2.Append(languages2);

			runPropertiesDefault2.Append(runPropertiesBaseStyle2);

			ParagraphPropertiesDefault paragraphPropertiesDefault2 = new ParagraphPropertiesDefault();

			ParagraphPropertiesBaseStyle paragraphPropertiesBaseStyle2 = new ParagraphPropertiesBaseStyle();
			SpacingBetweenLines spacingBetweenLines10 = new SpacingBetweenLines()
			                                            {After = "200", Line = "276", LineRule = LineSpacingRuleValues.Auto};

			paragraphPropertiesBaseStyle2.Append(spacingBetweenLines10);

			paragraphPropertiesDefault2.Append(paragraphPropertiesBaseStyle2);

			docDefaults2.Append(runPropertiesDefault2);
			docDefaults2.Append(paragraphPropertiesDefault2);

			LatentStyles latentStyles2 = new LatentStyles()
			                             {
			                             	DefaultLockedState = false,
			                             	DefaultUiPriority = 99,
			                             	DefaultSemiHidden = true,
			                             	DefaultUnhideWhenUsed = true,
			                             	DefaultPrimaryStyle = false,
			                             	Count = 267
			                             };
			LatentStyleExceptionInfo latentStyleExceptionInfo138 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Normal",
			                                                       	UiPriority = 0,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo139 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "heading 1",
			                                                       	UiPriority = 9,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo140 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 2", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo141 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 3", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo142 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 4", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo143 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 5", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo144 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 6", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo145 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 7", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo146 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 8", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo147 = new LatentStyleExceptionInfo()
			                                                       {Name = "heading 9", UiPriority = 9, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo148 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 1", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo149 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 2", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo150 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 3", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo151 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 4", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo152 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 5", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo153 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 6", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo154 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 7", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo155 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 8", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo156 = new LatentStyleExceptionInfo()
			                                                       {Name = "toc 9", UiPriority = 39};
			LatentStyleExceptionInfo latentStyleExceptionInfo157 = new LatentStyleExceptionInfo()
			                                                       {Name = "caption", UiPriority = 35, PrimaryStyle = true};
			LatentStyleExceptionInfo latentStyleExceptionInfo158 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Title",
			                                                       	UiPriority = 10,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo159 = new LatentStyleExceptionInfo()
			                                                       {Name = "Default Paragraph Font", UiPriority = 1};
			LatentStyleExceptionInfo latentStyleExceptionInfo160 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Subtitle",
			                                                       	UiPriority = 11,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo161 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Strong",
			                                                       	UiPriority = 22,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo162 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Emphasis",
			                                                       	UiPriority = 20,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo163 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Table Grid",
			                                                       	UiPriority = 59,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo164 = new LatentStyleExceptionInfo()
			                                                       {Name = "Placeholder Text", UnhideWhenUsed = false};
			LatentStyleExceptionInfo latentStyleExceptionInfo165 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "No Spacing",
			                                                       	UiPriority = 1,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo166 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo167 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo168 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo169 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo170 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo171 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo172 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo173 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo174 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo175 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo176 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo177 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo178 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo179 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo180 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 1",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo181 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 1",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo182 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 1",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo183 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 1",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo184 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 1",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo185 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 1",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo186 = new LatentStyleExceptionInfo()
			                                                       {Name = "Revision", UnhideWhenUsed = false};
			LatentStyleExceptionInfo latentStyleExceptionInfo187 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "List Paragraph",
			                                                       	UiPriority = 34,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo188 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Quote",
			                                                       	UiPriority = 29,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo189 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Intense Quote",
			                                                       	UiPriority = 30,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo190 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 1",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo191 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 1",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo192 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 1",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo193 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 1",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo194 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 1",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo195 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 1",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo196 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 1",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo197 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 1",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo198 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 2",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo199 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 2",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo200 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 2",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo201 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 2",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo202 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 2",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo203 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 2",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo204 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 2",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo205 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 2",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo206 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 2",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo207 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 2",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo208 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 2",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo209 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 2",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo210 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 2",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo211 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 2",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo212 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 3",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo213 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 3",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo214 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 3",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo215 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 3",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo216 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 3",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo217 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 3",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo218 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 3",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo219 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 3",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo220 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 3",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo221 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 3",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo222 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 3",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo223 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 3",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo224 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 3",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo225 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 3",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo226 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 4",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo227 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 4",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo228 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 4",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo229 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 4",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo230 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 4",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo231 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 4",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo232 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 4",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo233 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 4",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo234 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 4",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo235 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 4",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo236 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 4",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo237 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 4",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo238 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 4",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo239 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 4",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo240 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 5",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo241 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 5",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo242 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 5",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo243 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 5",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo244 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 5",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo245 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 5",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo246 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 5",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo247 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 5",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo248 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 5",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo249 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 5",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo250 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 5",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo251 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 5",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo252 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 5",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo253 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 5",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo254 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Shading Accent 6",
			                                                       	UiPriority = 60,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo255 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light List Accent 6",
			                                                       	UiPriority = 61,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo256 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Light Grid Accent 6",
			                                                       	UiPriority = 62,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo257 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 1 Accent 6",
			                                                       	UiPriority = 63,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo258 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Shading 2 Accent 6",
			                                                       	UiPriority = 64,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo259 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 1 Accent 6",
			                                                       	UiPriority = 65,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo260 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium List 2 Accent 6",
			                                                       	UiPriority = 66,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo261 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 1 Accent 6",
			                                                       	UiPriority = 67,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo262 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 2 Accent 6",
			                                                       	UiPriority = 68,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo263 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Medium Grid 3 Accent 6",
			                                                       	UiPriority = 69,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo264 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Dark List Accent 6",
			                                                       	UiPriority = 70,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo265 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Shading Accent 6",
			                                                       	UiPriority = 71,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo266 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful List Accent 6",
			                                                       	UiPriority = 72,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo267 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Colorful Grid Accent 6",
			                                                       	UiPriority = 73,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo268 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Subtle Emphasis",
			                                                       	UiPriority = 19,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo269 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Intense Emphasis",
			                                                       	UiPriority = 21,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo270 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Subtle Reference",
			                                                       	UiPriority = 31,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo271 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Intense Reference",
			                                                       	UiPriority = 32,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo272 = new LatentStyleExceptionInfo()
			                                                       {
			                                                       	Name = "Book Title",
			                                                       	UiPriority = 33,
			                                                       	SemiHidden = false,
			                                                       	UnhideWhenUsed = false,
			                                                       	PrimaryStyle = true
			                                                       };
			LatentStyleExceptionInfo latentStyleExceptionInfo273 = new LatentStyleExceptionInfo()
			                                                       {Name = "Bibliography", UiPriority = 37};
			LatentStyleExceptionInfo latentStyleExceptionInfo274 = new LatentStyleExceptionInfo()
			                                                       {Name = "TOC Heading", UiPriority = 39, PrimaryStyle = true};

			latentStyles2.Append(latentStyleExceptionInfo138);
			latentStyles2.Append(latentStyleExceptionInfo139);
			latentStyles2.Append(latentStyleExceptionInfo140);
			latentStyles2.Append(latentStyleExceptionInfo141);
			latentStyles2.Append(latentStyleExceptionInfo142);
			latentStyles2.Append(latentStyleExceptionInfo143);
			latentStyles2.Append(latentStyleExceptionInfo144);
			latentStyles2.Append(latentStyleExceptionInfo145);
			latentStyles2.Append(latentStyleExceptionInfo146);
			latentStyles2.Append(latentStyleExceptionInfo147);
			latentStyles2.Append(latentStyleExceptionInfo148);
			latentStyles2.Append(latentStyleExceptionInfo149);
			latentStyles2.Append(latentStyleExceptionInfo150);
			latentStyles2.Append(latentStyleExceptionInfo151);
			latentStyles2.Append(latentStyleExceptionInfo152);
			latentStyles2.Append(latentStyleExceptionInfo153);
			latentStyles2.Append(latentStyleExceptionInfo154);
			latentStyles2.Append(latentStyleExceptionInfo155);
			latentStyles2.Append(latentStyleExceptionInfo156);
			latentStyles2.Append(latentStyleExceptionInfo157);
			latentStyles2.Append(latentStyleExceptionInfo158);
			latentStyles2.Append(latentStyleExceptionInfo159);
			latentStyles2.Append(latentStyleExceptionInfo160);
			latentStyles2.Append(latentStyleExceptionInfo161);
			latentStyles2.Append(latentStyleExceptionInfo162);
			latentStyles2.Append(latentStyleExceptionInfo163);
			latentStyles2.Append(latentStyleExceptionInfo164);
			latentStyles2.Append(latentStyleExceptionInfo165);
			latentStyles2.Append(latentStyleExceptionInfo166);
			latentStyles2.Append(latentStyleExceptionInfo167);
			latentStyles2.Append(latentStyleExceptionInfo168);
			latentStyles2.Append(latentStyleExceptionInfo169);
			latentStyles2.Append(latentStyleExceptionInfo170);
			latentStyles2.Append(latentStyleExceptionInfo171);
			latentStyles2.Append(latentStyleExceptionInfo172);
			latentStyles2.Append(latentStyleExceptionInfo173);
			latentStyles2.Append(latentStyleExceptionInfo174);
			latentStyles2.Append(latentStyleExceptionInfo175);
			latentStyles2.Append(latentStyleExceptionInfo176);
			latentStyles2.Append(latentStyleExceptionInfo177);
			latentStyles2.Append(latentStyleExceptionInfo178);
			latentStyles2.Append(latentStyleExceptionInfo179);
			latentStyles2.Append(latentStyleExceptionInfo180);
			latentStyles2.Append(latentStyleExceptionInfo181);
			latentStyles2.Append(latentStyleExceptionInfo182);
			latentStyles2.Append(latentStyleExceptionInfo183);
			latentStyles2.Append(latentStyleExceptionInfo184);
			latentStyles2.Append(latentStyleExceptionInfo185);
			latentStyles2.Append(latentStyleExceptionInfo186);
			latentStyles2.Append(latentStyleExceptionInfo187);
			latentStyles2.Append(latentStyleExceptionInfo188);
			latentStyles2.Append(latentStyleExceptionInfo189);
			latentStyles2.Append(latentStyleExceptionInfo190);
			latentStyles2.Append(latentStyleExceptionInfo191);
			latentStyles2.Append(latentStyleExceptionInfo192);
			latentStyles2.Append(latentStyleExceptionInfo193);
			latentStyles2.Append(latentStyleExceptionInfo194);
			latentStyles2.Append(latentStyleExceptionInfo195);
			latentStyles2.Append(latentStyleExceptionInfo196);
			latentStyles2.Append(latentStyleExceptionInfo197);
			latentStyles2.Append(latentStyleExceptionInfo198);
			latentStyles2.Append(latentStyleExceptionInfo199);
			latentStyles2.Append(latentStyleExceptionInfo200);
			latentStyles2.Append(latentStyleExceptionInfo201);
			latentStyles2.Append(latentStyleExceptionInfo202);
			latentStyles2.Append(latentStyleExceptionInfo203);
			latentStyles2.Append(latentStyleExceptionInfo204);
			latentStyles2.Append(latentStyleExceptionInfo205);
			latentStyles2.Append(latentStyleExceptionInfo206);
			latentStyles2.Append(latentStyleExceptionInfo207);
			latentStyles2.Append(latentStyleExceptionInfo208);
			latentStyles2.Append(latentStyleExceptionInfo209);
			latentStyles2.Append(latentStyleExceptionInfo210);
			latentStyles2.Append(latentStyleExceptionInfo211);
			latentStyles2.Append(latentStyleExceptionInfo212);
			latentStyles2.Append(latentStyleExceptionInfo213);
			latentStyles2.Append(latentStyleExceptionInfo214);
			latentStyles2.Append(latentStyleExceptionInfo215);
			latentStyles2.Append(latentStyleExceptionInfo216);
			latentStyles2.Append(latentStyleExceptionInfo217);
			latentStyles2.Append(latentStyleExceptionInfo218);
			latentStyles2.Append(latentStyleExceptionInfo219);
			latentStyles2.Append(latentStyleExceptionInfo220);
			latentStyles2.Append(latentStyleExceptionInfo221);
			latentStyles2.Append(latentStyleExceptionInfo222);
			latentStyles2.Append(latentStyleExceptionInfo223);
			latentStyles2.Append(latentStyleExceptionInfo224);
			latentStyles2.Append(latentStyleExceptionInfo225);
			latentStyles2.Append(latentStyleExceptionInfo226);
			latentStyles2.Append(latentStyleExceptionInfo227);
			latentStyles2.Append(latentStyleExceptionInfo228);
			latentStyles2.Append(latentStyleExceptionInfo229);
			latentStyles2.Append(latentStyleExceptionInfo230);
			latentStyles2.Append(latentStyleExceptionInfo231);
			latentStyles2.Append(latentStyleExceptionInfo232);
			latentStyles2.Append(latentStyleExceptionInfo233);
			latentStyles2.Append(latentStyleExceptionInfo234);
			latentStyles2.Append(latentStyleExceptionInfo235);
			latentStyles2.Append(latentStyleExceptionInfo236);
			latentStyles2.Append(latentStyleExceptionInfo237);
			latentStyles2.Append(latentStyleExceptionInfo238);
			latentStyles2.Append(latentStyleExceptionInfo239);
			latentStyles2.Append(latentStyleExceptionInfo240);
			latentStyles2.Append(latentStyleExceptionInfo241);
			latentStyles2.Append(latentStyleExceptionInfo242);
			latentStyles2.Append(latentStyleExceptionInfo243);
			latentStyles2.Append(latentStyleExceptionInfo244);
			latentStyles2.Append(latentStyleExceptionInfo245);
			latentStyles2.Append(latentStyleExceptionInfo246);
			latentStyles2.Append(latentStyleExceptionInfo247);
			latentStyles2.Append(latentStyleExceptionInfo248);
			latentStyles2.Append(latentStyleExceptionInfo249);
			latentStyles2.Append(latentStyleExceptionInfo250);
			latentStyles2.Append(latentStyleExceptionInfo251);
			latentStyles2.Append(latentStyleExceptionInfo252);
			latentStyles2.Append(latentStyleExceptionInfo253);
			latentStyles2.Append(latentStyleExceptionInfo254);
			latentStyles2.Append(latentStyleExceptionInfo255);
			latentStyles2.Append(latentStyleExceptionInfo256);
			latentStyles2.Append(latentStyleExceptionInfo257);
			latentStyles2.Append(latentStyleExceptionInfo258);
			latentStyles2.Append(latentStyleExceptionInfo259);
			latentStyles2.Append(latentStyleExceptionInfo260);
			latentStyles2.Append(latentStyleExceptionInfo261);
			latentStyles2.Append(latentStyleExceptionInfo262);
			latentStyles2.Append(latentStyleExceptionInfo263);
			latentStyles2.Append(latentStyleExceptionInfo264);
			latentStyles2.Append(latentStyleExceptionInfo265);
			latentStyles2.Append(latentStyleExceptionInfo266);
			latentStyles2.Append(latentStyleExceptionInfo267);
			latentStyles2.Append(latentStyleExceptionInfo268);
			latentStyles2.Append(latentStyleExceptionInfo269);
			latentStyles2.Append(latentStyleExceptionInfo270);
			latentStyles2.Append(latentStyleExceptionInfo271);
			latentStyles2.Append(latentStyleExceptionInfo272);
			latentStyles2.Append(latentStyleExceptionInfo273);
			latentStyles2.Append(latentStyleExceptionInfo274);

			Style style17 = new Style() {Type = StyleValues.Paragraph, StyleId = "Normal", Default = true};
			StyleName styleName17 = new StyleName() {Val = "Normal"};
			PrimaryStyle primaryStyle5 = new PrimaryStyle();

			style17.Append(styleName17);
			style17.Append(primaryStyle5);

			Style style18 = new Style() {Type = StyleValues.Paragraph, StyleId = "Heading2"};
			StyleName styleName18 = new StyleName() {Val = "heading 2"};
			BasedOn basedOn13 = new BasedOn() {Val = "Normal"};
			NextParagraphStyle nextParagraphStyle3 = new NextParagraphStyle() {Val = "Normal"};
			LinkedStyle linkedStyle11 = new LinkedStyle() {Val = "Heading2Char"};
			UIPriority uIPriority16 = new UIPriority() {Val = 9};
			UnhideWhenUsed unhideWhenUsed8 = new UnhideWhenUsed();
			PrimaryStyle primaryStyle6 = new PrimaryStyle();
			Rsid rsid13 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties7 = new StyleParagraphProperties();
			KeepNext keepNext2 = new KeepNext();
			KeepLines keepLines2 = new KeepLines();
			SpacingBetweenLines spacingBetweenLines11 = new SpacingBetweenLines() {Before = "200", After = "0"};
			OutlineLevel outlineLevel2 = new OutlineLevel() {Val = 1};

			styleParagraphProperties7.Append(keepNext2);
			styleParagraphProperties7.Append(keepLines2);
			styleParagraphProperties7.Append(spacingBetweenLines11);
			styleParagraphProperties7.Append(outlineLevel2);

			StyleRunProperties styleRunProperties8 = new StyleRunProperties();
			RunFonts runFonts9 = new RunFonts()
			                     {
			                     	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                     	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                     	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                     	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                     };
			Bold bold13 = new Bold();
			BoldComplexScript boldComplexScript13 = new BoldComplexScript();
			Color color6 = new Color() {Val = "4F81BD", ThemeColor = ThemeColorValues.Accent1};
			FontSize fontSize9 = new FontSize() {Val = "26"};
			FontSizeComplexScript fontSizeComplexScript9 = new FontSizeComplexScript() {Val = "26"};

			styleRunProperties8.Append(runFonts9);
			styleRunProperties8.Append(bold13);
			styleRunProperties8.Append(boldComplexScript13);
			styleRunProperties8.Append(color6);
			styleRunProperties8.Append(fontSize9);
			styleRunProperties8.Append(fontSizeComplexScript9);

			style18.Append(styleName18);
			style18.Append(basedOn13);
			style18.Append(nextParagraphStyle3);
			style18.Append(linkedStyle11);
			style18.Append(uIPriority16);
			style18.Append(unhideWhenUsed8);
			style18.Append(primaryStyle6);
			style18.Append(rsid13);
			style18.Append(styleParagraphProperties7);
			style18.Append(styleRunProperties8);

			Style style19 = new Style() {Type = StyleValues.Character, StyleId = "DefaultParagraphFont", Default = true};
			StyleName styleName19 = new StyleName() {Val = "Default Paragraph Font"};
			UIPriority uIPriority17 = new UIPriority() {Val = 1};
			SemiHidden semiHidden6 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed9 = new UnhideWhenUsed();

			style19.Append(styleName19);
			style19.Append(uIPriority17);
			style19.Append(semiHidden6);
			style19.Append(unhideWhenUsed9);

			Style style20 = new Style() {Type = StyleValues.Table, StyleId = "TableNormal", Default = true};
			StyleName styleName20 = new StyleName() {Val = "Normal Table"};
			UIPriority uIPriority18 = new UIPriority() {Val = 99};
			SemiHidden semiHidden7 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed10 = new UnhideWhenUsed();

			StyleTableProperties styleTableProperties3 = new StyleTableProperties();
			TableIndentation tableIndentation3 = new TableIndentation() {Width = 0, Type = TableWidthUnitValues.Dxa};

			TableCellMarginDefault tableCellMarginDefault3 = new TableCellMarginDefault();
			TopMargin topMargin3 = new TopMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellLeftMargin tableCellLeftMargin3 = new TableCellLeftMargin() {Width = 108, Type = TableWidthValues.Dxa};
			BottomMargin bottomMargin3 = new BottomMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellRightMargin tableCellRightMargin3 = new TableCellRightMargin() {Width = 108, Type = TableWidthValues.Dxa};

			tableCellMarginDefault3.Append(topMargin3);
			tableCellMarginDefault3.Append(tableCellLeftMargin3);
			tableCellMarginDefault3.Append(bottomMargin3);
			tableCellMarginDefault3.Append(tableCellRightMargin3);

			styleTableProperties3.Append(tableIndentation3);
			styleTableProperties3.Append(tableCellMarginDefault3);

			style20.Append(styleName20);
			style20.Append(uIPriority18);
			style20.Append(semiHidden7);
			style20.Append(unhideWhenUsed10);
			style20.Append(styleTableProperties3);

			Style style21 = new Style() {Type = StyleValues.Numbering, StyleId = "NoList", Default = true};
			StyleName styleName21 = new StyleName() {Val = "No List"};
			UIPriority uIPriority19 = new UIPriority() {Val = 99};
			SemiHidden semiHidden8 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed11 = new UnhideWhenUsed();

			style21.Append(styleName21);
			style21.Append(uIPriority19);
			style21.Append(semiHidden8);
			style21.Append(unhideWhenUsed11);

			Style style22 = new Style() {Type = StyleValues.Table, StyleId = "TableGrid"};
			StyleName styleName22 = new StyleName() {Val = "Table Grid"};
			BasedOn basedOn14 = new BasedOn() {Val = "TableNormal"};
			UIPriority uIPriority20 = new UIPriority() {Val = 59};
			Rsid rsid14 = new Rsid() {Val = "00F06304"};

			StyleParagraphProperties styleParagraphProperties8 = new StyleParagraphProperties();
			SpacingBetweenLines spacingBetweenLines12 = new SpacingBetweenLines()
			                                            {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties8.Append(spacingBetweenLines12);

			StyleTableProperties styleTableProperties4 = new StyleTableProperties();
			TableIndentation tableIndentation4 = new TableIndentation() {Width = 0, Type = TableWidthUnitValues.Dxa};

			TableBorders tableBorders4 = new TableBorders();
			TopBorder topBorder3 = new TopBorder()
			                       {Val = BorderValues.Single, Color = "auto", Size = (UInt32Value) 4U, Space = (UInt32Value) 0U};
			LeftBorder leftBorder3 = new LeftBorder()
			                         {
			                         	Val = BorderValues.Single,
			                         	Color = "auto",
			                         	Size = (UInt32Value) 4U,
			                         	Space = (UInt32Value) 0U
			                         };
			BottomBorder bottomBorder4 = new BottomBorder()
			                             {
			                             	Val = BorderValues.Single,
			                             	Color = "auto",
			                             	Size = (UInt32Value) 4U,
			                             	Space = (UInt32Value) 0U
			                             };
			RightBorder rightBorder3 = new RightBorder()
			                           {
			                           	Val = BorderValues.Single,
			                           	Color = "auto",
			                           	Size = (UInt32Value) 4U,
			                           	Space = (UInt32Value) 0U
			                           };
			InsideHorizontalBorder insideHorizontalBorder3 = new InsideHorizontalBorder()
			                                                 {
			                                                 	Val = BorderValues.Single,
			                                                 	Color = "auto",
			                                                 	Size = (UInt32Value) 4U,
			                                                 	Space = (UInt32Value) 0U
			                                                 };
			InsideVerticalBorder insideVerticalBorder4 = new InsideVerticalBorder()
			                                             {
			                                             	Val = BorderValues.Single,
			                                             	Color = "auto",
			                                             	Size = (UInt32Value) 4U,
			                                             	Space = (UInt32Value) 0U
			                                             };

			tableBorders4.Append(topBorder3);
			tableBorders4.Append(leftBorder3);
			tableBorders4.Append(bottomBorder4);
			tableBorders4.Append(rightBorder3);
			tableBorders4.Append(insideHorizontalBorder3);
			tableBorders4.Append(insideVerticalBorder4);

			TableCellMarginDefault tableCellMarginDefault4 = new TableCellMarginDefault();
			TopMargin topMargin4 = new TopMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellLeftMargin tableCellLeftMargin4 = new TableCellLeftMargin() {Width = 108, Type = TableWidthValues.Dxa};
			BottomMargin bottomMargin4 = new BottomMargin() {Width = "0", Type = TableWidthUnitValues.Dxa};
			TableCellRightMargin tableCellRightMargin4 = new TableCellRightMargin() {Width = 108, Type = TableWidthValues.Dxa};

			tableCellMarginDefault4.Append(topMargin4);
			tableCellMarginDefault4.Append(tableCellLeftMargin4);
			tableCellMarginDefault4.Append(bottomMargin4);
			tableCellMarginDefault4.Append(tableCellRightMargin4);

			styleTableProperties4.Append(tableIndentation4);
			styleTableProperties4.Append(tableBorders4);
			styleTableProperties4.Append(tableCellMarginDefault4);

			style22.Append(styleName22);
			style22.Append(basedOn14);
			style22.Append(uIPriority20);
			style22.Append(rsid14);
			style22.Append(styleParagraphProperties8);
			style22.Append(styleTableProperties4);

			Style style23 = new Style() {Type = StyleValues.Paragraph, StyleId = "Header"};
			StyleName styleName23 = new StyleName() {Val = "header"};
			BasedOn basedOn15 = new BasedOn() {Val = "Normal"};
			LinkedStyle linkedStyle12 = new LinkedStyle() {Val = "HeaderChar"};
			UIPriority uIPriority21 = new UIPriority() {Val = 99};
			UnhideWhenUsed unhideWhenUsed12 = new UnhideWhenUsed();
			Rsid rsid15 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties9 = new StyleParagraphProperties();

			Tabs tabs4 = new Tabs();
			TabStop tabStop8 = new TabStop() {Val = TabStopValues.Center, Position = 4680};
			TabStop tabStop9 = new TabStop() {Val = TabStopValues.Right, Position = 9360};

			tabs4.Append(tabStop8);
			tabs4.Append(tabStop9);
			SpacingBetweenLines spacingBetweenLines13 = new SpacingBetweenLines()
			                                            {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties9.Append(tabs4);
			styleParagraphProperties9.Append(spacingBetweenLines13);

			style23.Append(styleName23);
			style23.Append(basedOn15);
			style23.Append(linkedStyle12);
			style23.Append(uIPriority21);
			style23.Append(unhideWhenUsed12);
			style23.Append(rsid15);
			style23.Append(styleParagraphProperties9);

			Style style24 = new Style() {Type = StyleValues.Character, StyleId = "HeaderChar", CustomStyle = true};
			StyleName styleName24 = new StyleName() {Val = "Header Char"};
			BasedOn basedOn16 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle13 = new LinkedStyle() {Val = "Header"};
			UIPriority uIPriority22 = new UIPriority() {Val = 99};
			Rsid rsid16 = new Rsid() {Val = "00442AD3"};

			style24.Append(styleName24);
			style24.Append(basedOn16);
			style24.Append(linkedStyle13);
			style24.Append(uIPriority22);
			style24.Append(rsid16);

			Style style25 = new Style() {Type = StyleValues.Paragraph, StyleId = "Footer"};
			StyleName styleName25 = new StyleName() {Val = "footer"};
			BasedOn basedOn17 = new BasedOn() {Val = "Normal"};
			LinkedStyle linkedStyle14 = new LinkedStyle() {Val = "FooterChar"};
			UIPriority uIPriority23 = new UIPriority() {Val = 99};
			UnhideWhenUsed unhideWhenUsed13 = new UnhideWhenUsed();
			Rsid rsid17 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties10 = new StyleParagraphProperties();

			Tabs tabs5 = new Tabs();
			TabStop tabStop10 = new TabStop() {Val = TabStopValues.Center, Position = 4680};
			TabStop tabStop11 = new TabStop() {Val = TabStopValues.Right, Position = 9360};

			tabs5.Append(tabStop10);
			tabs5.Append(tabStop11);
			SpacingBetweenLines spacingBetweenLines14 = new SpacingBetweenLines()
			                                            {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties10.Append(tabs5);
			styleParagraphProperties10.Append(spacingBetweenLines14);

			style25.Append(styleName25);
			style25.Append(basedOn17);
			style25.Append(linkedStyle14);
			style25.Append(uIPriority23);
			style25.Append(unhideWhenUsed13);
			style25.Append(rsid17);
			style25.Append(styleParagraphProperties10);

			Style style26 = new Style() {Type = StyleValues.Character, StyleId = "FooterChar", CustomStyle = true};
			StyleName styleName26 = new StyleName() {Val = "Footer Char"};
			BasedOn basedOn18 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle15 = new LinkedStyle() {Val = "Footer"};
			UIPriority uIPriority24 = new UIPriority() {Val = 99};
			Rsid rsid18 = new Rsid() {Val = "00442AD3"};

			style26.Append(styleName26);
			style26.Append(basedOn18);
			style26.Append(linkedStyle15);
			style26.Append(uIPriority24);
			style26.Append(rsid18);

			Style style27 = new Style() {Type = StyleValues.Paragraph, StyleId = "BalloonText"};
			StyleName styleName27 = new StyleName() {Val = "Balloon Text"};
			BasedOn basedOn19 = new BasedOn() {Val = "Normal"};
			LinkedStyle linkedStyle16 = new LinkedStyle() {Val = "BalloonTextChar"};
			UIPriority uIPriority25 = new UIPriority() {Val = 99};
			SemiHidden semiHidden9 = new SemiHidden();
			UnhideWhenUsed unhideWhenUsed14 = new UnhideWhenUsed();
			Rsid rsid19 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties11 = new StyleParagraphProperties();
			SpacingBetweenLines spacingBetweenLines15 = new SpacingBetweenLines()
			                                            {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			styleParagraphProperties11.Append(spacingBetweenLines15);

			StyleRunProperties styleRunProperties9 = new StyleRunProperties();
			RunFonts runFonts10 = new RunFonts() {Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma"};
			FontSize fontSize10 = new FontSize() {Val = "16"};
			FontSizeComplexScript fontSizeComplexScript10 = new FontSizeComplexScript() {Val = "16"};

			styleRunProperties9.Append(runFonts10);
			styleRunProperties9.Append(fontSize10);
			styleRunProperties9.Append(fontSizeComplexScript10);

			style27.Append(styleName27);
			style27.Append(basedOn19);
			style27.Append(linkedStyle16);
			style27.Append(uIPriority25);
			style27.Append(semiHidden9);
			style27.Append(unhideWhenUsed14);
			style27.Append(rsid19);
			style27.Append(styleParagraphProperties11);
			style27.Append(styleRunProperties9);

			Style style28 = new Style() {Type = StyleValues.Character, StyleId = "BalloonTextChar", CustomStyle = true};
			StyleName styleName28 = new StyleName() {Val = "Balloon Text Char"};
			BasedOn basedOn20 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle17 = new LinkedStyle() {Val = "BalloonText"};
			UIPriority uIPriority26 = new UIPriority() {Val = 99};
			SemiHidden semiHidden10 = new SemiHidden();
			Rsid rsid20 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties10 = new StyleRunProperties();
			RunFonts runFonts11 = new RunFonts() {Ascii = "Tahoma", HighAnsi = "Tahoma", ComplexScript = "Tahoma"};
			FontSize fontSize11 = new FontSize() {Val = "16"};
			FontSizeComplexScript fontSizeComplexScript11 = new FontSizeComplexScript() {Val = "16"};

			styleRunProperties10.Append(runFonts11);
			styleRunProperties10.Append(fontSize11);
			styleRunProperties10.Append(fontSizeComplexScript11);

			style28.Append(styleName28);
			style28.Append(basedOn20);
			style28.Append(linkedStyle17);
			style28.Append(uIPriority26);
			style28.Append(semiHidden10);
			style28.Append(rsid20);
			style28.Append(styleRunProperties10);

			Style style29 = new Style() {Type = StyleValues.Paragraph, StyleId = "Title"};
			StyleName styleName29 = new StyleName() {Val = "Title"};
			BasedOn basedOn21 = new BasedOn() {Val = "Normal"};
			NextParagraphStyle nextParagraphStyle4 = new NextParagraphStyle() {Val = "Normal"};
			LinkedStyle linkedStyle18 = new LinkedStyle() {Val = "TitleChar"};
			UIPriority uIPriority27 = new UIPriority() {Val = 10};
			PrimaryStyle primaryStyle7 = new PrimaryStyle();
			Rsid rsid21 = new Rsid() {Val = "00442AD3"};

			StyleParagraphProperties styleParagraphProperties12 = new StyleParagraphProperties();

			ParagraphBorders paragraphBorders2 = new ParagraphBorders();
			BottomBorder bottomBorder5 = new BottomBorder()
			                             {
			                             	Val = BorderValues.Single,
			                             	Color = "4F81BD",
			                             	ThemeColor = ThemeColorValues.Accent1,
			                             	Size = (UInt32Value) 8U,
			                             	Space = (UInt32Value) 4U
			                             };

			paragraphBorders2.Append(bottomBorder5);
			SpacingBetweenLines spacingBetweenLines16 = new SpacingBetweenLines()
			                                            {After = "300", Line = "240", LineRule = LineSpacingRuleValues.Auto};
			ContextualSpacing contextualSpacing2 = new ContextualSpacing();

			styleParagraphProperties12.Append(paragraphBorders2);
			styleParagraphProperties12.Append(spacingBetweenLines16);
			styleParagraphProperties12.Append(contextualSpacing2);

			StyleRunProperties styleRunProperties11 = new StyleRunProperties();
			RunFonts runFonts12 = new RunFonts()
			                      {
			                      	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                      	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                      	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                      	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                      };
			Color color7 = new Color() {Val = "17365D", ThemeColor = ThemeColorValues.Text2, ThemeShade = "BF"};
			Spacing spacing4 = new Spacing() {Val = 5};
			Kern kern3 = new Kern() {Val = (UInt32Value) 28U};
			FontSize fontSize12 = new FontSize() {Val = "52"};
			FontSizeComplexScript fontSizeComplexScript12 = new FontSizeComplexScript() {Val = "52"};

			styleRunProperties11.Append(runFonts12);
			styleRunProperties11.Append(color7);
			styleRunProperties11.Append(spacing4);
			styleRunProperties11.Append(kern3);
			styleRunProperties11.Append(fontSize12);
			styleRunProperties11.Append(fontSizeComplexScript12);

			style29.Append(styleName29);
			style29.Append(basedOn21);
			style29.Append(nextParagraphStyle4);
			style29.Append(linkedStyle18);
			style29.Append(uIPriority27);
			style29.Append(primaryStyle7);
			style29.Append(rsid21);
			style29.Append(styleParagraphProperties12);
			style29.Append(styleRunProperties11);

			Style style30 = new Style() {Type = StyleValues.Character, StyleId = "TitleChar", CustomStyle = true};
			StyleName styleName30 = new StyleName() {Val = "Title Char"};
			BasedOn basedOn22 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle19 = new LinkedStyle() {Val = "Title"};
			UIPriority uIPriority28 = new UIPriority() {Val = 10};
			Rsid rsid22 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties12 = new StyleRunProperties();
			RunFonts runFonts13 = new RunFonts()
			                      {
			                      	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                      	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                      	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                      	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                      };
			Color color8 = new Color() {Val = "17365D", ThemeColor = ThemeColorValues.Text2, ThemeShade = "BF"};
			Spacing spacing5 = new Spacing() {Val = 5};
			Kern kern4 = new Kern() {Val = (UInt32Value) 28U};
			FontSize fontSize13 = new FontSize() {Val = "52"};
			FontSizeComplexScript fontSizeComplexScript13 = new FontSizeComplexScript() {Val = "52"};

			styleRunProperties12.Append(runFonts13);
			styleRunProperties12.Append(color8);
			styleRunProperties12.Append(spacing5);
			styleRunProperties12.Append(kern4);
			styleRunProperties12.Append(fontSize13);
			styleRunProperties12.Append(fontSizeComplexScript13);

			style30.Append(styleName30);
			style30.Append(basedOn22);
			style30.Append(linkedStyle19);
			style30.Append(uIPriority28);
			style30.Append(rsid22);
			style30.Append(styleRunProperties12);

			Style style31 = new Style() {Type = StyleValues.Character, StyleId = "Strong"};
			StyleName styleName31 = new StyleName() {Val = "Strong"};
			BasedOn basedOn23 = new BasedOn() {Val = "DefaultParagraphFont"};
			UIPriority uIPriority29 = new UIPriority() {Val = 22};
			PrimaryStyle primaryStyle8 = new PrimaryStyle();
			Rsid rsid23 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties13 = new StyleRunProperties();
			Bold bold14 = new Bold();
			BoldComplexScript boldComplexScript14 = new BoldComplexScript();

			styleRunProperties13.Append(bold14);
			styleRunProperties13.Append(boldComplexScript14);

			style31.Append(styleName31);
			style31.Append(basedOn23);
			style31.Append(uIPriority29);
			style31.Append(primaryStyle8);
			style31.Append(rsid23);
			style31.Append(styleRunProperties13);

			Style style32 = new Style() {Type = StyleValues.Character, StyleId = "Heading2Char", CustomStyle = true};
			StyleName styleName32 = new StyleName() {Val = "Heading 2 Char"};
			BasedOn basedOn24 = new BasedOn() {Val = "DefaultParagraphFont"};
			LinkedStyle linkedStyle20 = new LinkedStyle() {Val = "Heading2"};
			UIPriority uIPriority30 = new UIPriority() {Val = 9};
			Rsid rsid24 = new Rsid() {Val = "00442AD3"};

			StyleRunProperties styleRunProperties14 = new StyleRunProperties();
			RunFonts runFonts14 = new RunFonts()
			                      {
			                      	AsciiTheme = ThemeFontValues.MajorHighAnsi,
			                      	HighAnsiTheme = ThemeFontValues.MajorHighAnsi,
			                      	EastAsiaTheme = ThemeFontValues.MajorEastAsia,
			                      	ComplexScriptTheme = ThemeFontValues.MajorBidi
			                      };
			Bold bold15 = new Bold();
			BoldComplexScript boldComplexScript15 = new BoldComplexScript();
			Color color9 = new Color() {Val = "4F81BD", ThemeColor = ThemeColorValues.Accent1};
			FontSize fontSize14 = new FontSize() {Val = "26"};
			FontSizeComplexScript fontSizeComplexScript14 = new FontSizeComplexScript() {Val = "26"};

			styleRunProperties14.Append(runFonts14);
			styleRunProperties14.Append(bold15);
			styleRunProperties14.Append(boldComplexScript15);
			styleRunProperties14.Append(color9);
			styleRunProperties14.Append(fontSize14);
			styleRunProperties14.Append(fontSizeComplexScript14);

			style32.Append(styleName32);
			style32.Append(basedOn24);
			style32.Append(linkedStyle20);
			style32.Append(uIPriority30);
			style32.Append(rsid24);
			style32.Append(styleRunProperties14);

			styles2.Append(docDefaults2);
			styles2.Append(latentStyles2);
			styles2.Append(style17);
			styles2.Append(style18);
			styles2.Append(style19);
			styles2.Append(style20);
			styles2.Append(style21);
			styles2.Append(style22);
			styles2.Append(style23);
			styles2.Append(style24);
			styles2.Append(style25);
			styles2.Append(style26);
			styles2.Append(style27);
			styles2.Append(style28);
			styles2.Append(style29);
			styles2.Append(style30);
			styles2.Append(style31);
			styles2.Append(style32);

			styleDefinitionsPart1.Styles = styles2;
		}

		// Generates content of customXmlPart1.
		private void GenerateCustomXmlPart1Content(CustomXmlPart customXmlPart1)
		{
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(customXmlPart1.GetStream(System.IO.FileMode.Create),
			                                                               System.Text.Encoding.UTF8);
			writer.WriteRaw(
				"<b:Sources SelectedStyle=\"\\APA.XSL\" StyleName=\"APA\" xmlns:b=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\" xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\"></b:Sources>\r\n");
			writer.Flush();
			writer.Close();
		}

		// Generates content of customXmlPropertiesPart1.
		private void GenerateCustomXmlPropertiesPart1Content(CustomXmlPropertiesPart customXmlPropertiesPart1)
		{
			Ds.DataStoreItem dataStoreItem1 = new Ds.DataStoreItem() {ItemId = "{12C84303-DA55-4352-970A-110622CFBA37}"};
			dataStoreItem1.AddNamespaceDeclaration("ds", "http://schemas.openxmlformats.org/officeDocument/2006/customXml");

			Ds.SchemaReferences schemaReferences1 = new Ds.SchemaReferences();
			Ds.SchemaReference schemaReference1 = new Ds.SchemaReference()
			                                      {Uri = "http://schemas.openxmlformats.org/officeDocument/2006/bibliography"};

			schemaReferences1.Append(schemaReference1);

			dataStoreItem1.Append(schemaReferences1);

			customXmlPropertiesPart1.DataStoreItem = dataStoreItem1;
		}

		// Generates content of footnotesPart1.
		private void GenerateFootnotesPart1Content(FootnotesPart footnotesPart1)
		{
			Footnotes footnotes1 = new Footnotes() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			footnotes1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			footnotes1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			footnotes1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			footnotes1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			footnotes1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			footnotes1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			footnotes1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			footnotes1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			footnotes1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			footnotes1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			footnotes1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			footnotes1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			footnotes1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			footnotes1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			footnotes1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			Footnote footnote1 = new Footnote() {Type = FootnoteEndnoteValues.Separator, Id = -1};

			Paragraph paragraph25 = new Paragraph()
			                        {
			                        	RsidParagraphAddition = "00B32099",
			                        	RsidParagraphProperties = "00442AD3",
			                        	RsidRunAdditionDefault = "00B32099"
			                        };

			ParagraphProperties paragraphProperties6 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines17 = new SpacingBetweenLines()
			                                            {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			paragraphProperties6.Append(spacingBetweenLines17);

			Run run41 = new Run();
			SeparatorMark separatorMark2 = new SeparatorMark();

			run41.Append(separatorMark2);

			paragraph25.Append(paragraphProperties6);
			paragraph25.Append(run41);

			footnote1.Append(paragraph25);

			Footnote footnote2 = new Footnote() {Type = FootnoteEndnoteValues.ContinuationSeparator, Id = 0};

			Paragraph paragraph26 = new Paragraph()
			                        {
			                        	RsidParagraphAddition = "00B32099",
			                        	RsidParagraphProperties = "00442AD3",
			                        	RsidRunAdditionDefault = "00B32099"
			                        };

			ParagraphProperties paragraphProperties7 = new ParagraphProperties();
			SpacingBetweenLines spacingBetweenLines18 = new SpacingBetweenLines()
			                                            {After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto};

			paragraphProperties7.Append(spacingBetweenLines18);

			Run run42 = new Run();
			ContinuationSeparatorMark continuationSeparatorMark2 = new ContinuationSeparatorMark();

			run42.Append(continuationSeparatorMark2);

			paragraph26.Append(paragraphProperties7);
			paragraph26.Append(run42);

			footnote2.Append(paragraph26);

			footnotes1.Append(footnote1);
			footnotes1.Append(footnote2);

			footnotesPart1.Footnotes = footnotes1;
		}

		// Generates content of headerPart1.
		private void GenerateHeaderPart1Content(HeaderPart headerPart1)
		{
			Header header1 = new Header() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			header1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			header1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			header1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			header1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			header1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			header1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			header1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			header1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			header1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			header1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			header1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			header1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			header1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			header1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			header1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			Paragraph paragraph27 = new Paragraph()
			                        {
			                        	RsidParagraphMarkRevision = "00442AD3",
			                        	RsidParagraphAddition = "0005059E",
			                        	RsidParagraphProperties = "00442AD3",
			                        	RsidRunAdditionDefault = "0005059E"
			                        };

			ParagraphProperties paragraphProperties8 = new ParagraphProperties();
			ParagraphStyleId paragraphStyleId2 = new ParagraphStyleId() {Val = "Title"};

			ParagraphMarkRunProperties paragraphMarkRunProperties2 = new ParagraphMarkRunProperties();
			RunStyle runStyle1 = new RunStyle() {Val = "Strong"};

			paragraphMarkRunProperties2.Append(runStyle1);

			paragraphProperties8.Append(paragraphStyleId2);
			paragraphProperties8.Append(paragraphMarkRunProperties2);

			Run run43 = new Run();
			Text text28 = new Text();
			text28.Text = "Class name here";

			run43.Append(text28);

			paragraph27.Append(paragraphProperties8);
			paragraph27.Append(run43);

			header1.Append(paragraph27);

			headerPart1.Header = header1;
		}

		// Generates content of webSettingsPart1.
		private void GenerateWebSettingsPart1Content(WebSettingsPart webSettingsPart1)
		{
			WebSettings webSettings1 = new WebSettings() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14"}};
			webSettings1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			webSettings1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			webSettings1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			webSettings1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			OptimizeForBrowser optimizeForBrowser1 = new OptimizeForBrowser();
			AllowPNG allowPNG1 = new AllowPNG();

			webSettings1.Append(optimizeForBrowser1);
			webSettings1.Append(allowPNG1);

			webSettingsPart1.WebSettings = webSettings1;
		}

		// Generates content of footerPart2.
		private void GenerateFooterPart2Content(FooterPart footerPart2)
		{
			Footer footer2 = new Footer() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			footer2.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			footer2.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			footer2.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			footer2.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			footer2.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			footer2.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			footer2.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			footer2.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			footer2.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			footer2.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			footer2.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			footer2.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			footer2.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			footer2.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			footer2.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			SdtBlock sdtBlock1 = new SdtBlock();

			SdtProperties sdtProperties1 = new SdtProperties();
			SdtId sdtId1 = new SdtId() {Val = -1382393284};

			SdtContentDocPartObject sdtContentDocPartObject1 = new SdtContentDocPartObject();
			DocPartGallery docPartGallery1 = new DocPartGallery() {Val = "Page Numbers (Bottom of Page)"};
			DocPartUnique docPartUnique1 = new DocPartUnique();

			sdtContentDocPartObject1.Append(docPartGallery1);
			sdtContentDocPartObject1.Append(docPartUnique1);

			sdtProperties1.Append(sdtId1);
			sdtProperties1.Append(sdtContentDocPartObject1);
			SdtEndCharProperties sdtEndCharProperties1 = new SdtEndCharProperties();

			SdtContentBlock sdtContentBlock1 = new SdtContentBlock();

			SdtBlock sdtBlock2 = new SdtBlock();

			SdtProperties sdtProperties2 = new SdtProperties();
			SdtId sdtId2 = new SdtId() {Val = 98381352};

			SdtContentDocPartObject sdtContentDocPartObject2 = new SdtContentDocPartObject();
			DocPartGallery docPartGallery2 = new DocPartGallery() {Val = "Page Numbers (Top of Page)"};
			DocPartUnique docPartUnique2 = new DocPartUnique();

			sdtContentDocPartObject2.Append(docPartGallery2);
			sdtContentDocPartObject2.Append(docPartUnique2);

			sdtProperties2.Append(sdtId2);
			sdtProperties2.Append(sdtContentDocPartObject2);
			SdtEndCharProperties sdtEndCharProperties2 = new SdtEndCharProperties();

			SdtContentBlock sdtContentBlock2 = new SdtContentBlock();

			Paragraph paragraph28 = new Paragraph() {RsidParagraphAddition = "0005059E", RsidRunAdditionDefault = "0005059E"};

			ParagraphProperties paragraphProperties9 = new ParagraphProperties();
			ParagraphStyleId paragraphStyleId3 = new ParagraphStyleId() {Val = "Footer"};

			paragraphProperties9.Append(paragraphStyleId3);

			Run run44 = new Run();
			Text text29 = new Text() {Space = SpaceProcessingModeValues.Preserve};
			text29.Text = "Page ";

			run44.Append(text29);

			Run run45 = new Run();

			RunProperties runProperties23 = new RunProperties();
			Bold bold16 = new Bold();
			BoldComplexScript boldComplexScript16 = new BoldComplexScript();
			FontSize fontSize15 = new FontSize() {Val = "24"};
			FontSizeComplexScript fontSizeComplexScript15 = new FontSizeComplexScript() {Val = "24"};

			runProperties23.Append(bold16);
			runProperties23.Append(boldComplexScript16);
			runProperties23.Append(fontSize15);
			runProperties23.Append(fontSizeComplexScript15);
			FieldChar fieldChar7 = new FieldChar() {FieldCharType = FieldCharValues.Begin};

			run45.Append(runProperties23);
			run45.Append(fieldChar7);

			Run run46 = new Run();

			RunProperties runProperties24 = new RunProperties();
			Bold bold17 = new Bold();
			BoldComplexScript boldComplexScript17 = new BoldComplexScript();

			runProperties24.Append(bold17);
			runProperties24.Append(boldComplexScript17);
			FieldCode fieldCode3 = new FieldCode() {Space = SpaceProcessingModeValues.Preserve};
			fieldCode3.Text = " PAGE ";

			run46.Append(runProperties24);
			run46.Append(fieldCode3);

			Run run47 = new Run();

			RunProperties runProperties25 = new RunProperties();
			Bold bold18 = new Bold();
			BoldComplexScript boldComplexScript18 = new BoldComplexScript();
			FontSize fontSize16 = new FontSize() {Val = "24"};
			FontSizeComplexScript fontSizeComplexScript16 = new FontSizeComplexScript() {Val = "24"};

			runProperties25.Append(bold18);
			runProperties25.Append(boldComplexScript18);
			runProperties25.Append(fontSize16);
			runProperties25.Append(fontSizeComplexScript16);
			FieldChar fieldChar8 = new FieldChar() {FieldCharType = FieldCharValues.Separate};

			run47.Append(runProperties25);
			run47.Append(fieldChar8);

			Run run48 = new Run() {RsidRunAddition = "00FB1F22"};

			RunProperties runProperties26 = new RunProperties();
			Bold bold19 = new Bold();
			BoldComplexScript boldComplexScript19 = new BoldComplexScript();
			NoProof noProof23 = new NoProof();

			runProperties26.Append(bold19);
			runProperties26.Append(boldComplexScript19);
			runProperties26.Append(noProof23);
			Text text30 = new Text();
			text30.Text = "2";

			run48.Append(runProperties26);
			run48.Append(text30);

			Run run49 = new Run();

			RunProperties runProperties27 = new RunProperties();
			Bold bold20 = new Bold();
			BoldComplexScript boldComplexScript20 = new BoldComplexScript();
			FontSize fontSize17 = new FontSize() {Val = "24"};
			FontSizeComplexScript fontSizeComplexScript17 = new FontSizeComplexScript() {Val = "24"};

			runProperties27.Append(bold20);
			runProperties27.Append(boldComplexScript20);
			runProperties27.Append(fontSize17);
			runProperties27.Append(fontSizeComplexScript17);
			FieldChar fieldChar9 = new FieldChar() {FieldCharType = FieldCharValues.End};

			run49.Append(runProperties27);
			run49.Append(fieldChar9);

			Run run50 = new Run();
			Text text31 = new Text() {Space = SpaceProcessingModeValues.Preserve};
			text31.Text = " of ";

			run50.Append(text31);

			Run run51 = new Run();

			RunProperties runProperties28 = new RunProperties();
			Bold bold21 = new Bold();
			BoldComplexScript boldComplexScript21 = new BoldComplexScript();
			FontSize fontSize18 = new FontSize() {Val = "24"};
			FontSizeComplexScript fontSizeComplexScript18 = new FontSizeComplexScript() {Val = "24"};

			runProperties28.Append(bold21);
			runProperties28.Append(boldComplexScript21);
			runProperties28.Append(fontSize18);
			runProperties28.Append(fontSizeComplexScript18);
			FieldChar fieldChar10 = new FieldChar() {FieldCharType = FieldCharValues.Begin};

			run51.Append(runProperties28);
			run51.Append(fieldChar10);

			Run run52 = new Run();

			RunProperties runProperties29 = new RunProperties();
			Bold bold22 = new Bold();
			BoldComplexScript boldComplexScript22 = new BoldComplexScript();

			runProperties29.Append(bold22);
			runProperties29.Append(boldComplexScript22);
			FieldCode fieldCode4 = new FieldCode() {Space = SpaceProcessingModeValues.Preserve};
			fieldCode4.Text = " NUMPAGES  ";

			run52.Append(runProperties29);
			run52.Append(fieldCode4);

			Run run53 = new Run();

			RunProperties runProperties30 = new RunProperties();
			Bold bold23 = new Bold();
			BoldComplexScript boldComplexScript23 = new BoldComplexScript();
			FontSize fontSize19 = new FontSize() {Val = "24"};
			FontSizeComplexScript fontSizeComplexScript19 = new FontSizeComplexScript() {Val = "24"};

			runProperties30.Append(bold23);
			runProperties30.Append(boldComplexScript23);
			runProperties30.Append(fontSize19);
			runProperties30.Append(fontSizeComplexScript19);
			FieldChar fieldChar11 = new FieldChar() {FieldCharType = FieldCharValues.Separate};

			run53.Append(runProperties30);
			run53.Append(fieldChar11);

			Run run54 = new Run() {RsidRunAddition = "00FB1F22"};

			RunProperties runProperties31 = new RunProperties();
			Bold bold24 = new Bold();
			BoldComplexScript boldComplexScript24 = new BoldComplexScript();
			NoProof noProof24 = new NoProof();

			runProperties31.Append(bold24);
			runProperties31.Append(boldComplexScript24);
			runProperties31.Append(noProof24);
			Text text32 = new Text();
			text32.Text = "37";

			run54.Append(runProperties31);
			run54.Append(text32);

			Run run55 = new Run();

			RunProperties runProperties32 = new RunProperties();
			Bold bold25 = new Bold();
			BoldComplexScript boldComplexScript25 = new BoldComplexScript();
			FontSize fontSize20 = new FontSize() {Val = "24"};
			FontSizeComplexScript fontSizeComplexScript20 = new FontSizeComplexScript() {Val = "24"};

			runProperties32.Append(bold25);
			runProperties32.Append(boldComplexScript25);
			runProperties32.Append(fontSize20);
			runProperties32.Append(fontSizeComplexScript20);
			FieldChar fieldChar12 = new FieldChar() {FieldCharType = FieldCharValues.End};

			run55.Append(runProperties32);
			run55.Append(fieldChar12);

			paragraph28.Append(paragraphProperties9);
			paragraph28.Append(run44);
			paragraph28.Append(run45);
			paragraph28.Append(run46);
			paragraph28.Append(run47);
			paragraph28.Append(run48);
			paragraph28.Append(run49);
			paragraph28.Append(run50);
			paragraph28.Append(run51);
			paragraph28.Append(run52);
			paragraph28.Append(run53);
			paragraph28.Append(run54);
			paragraph28.Append(run55);

			sdtContentBlock2.Append(paragraph28);

			sdtBlock2.Append(sdtProperties2);
			sdtBlock2.Append(sdtEndCharProperties2);
			sdtBlock2.Append(sdtContentBlock2);

			sdtContentBlock1.Append(sdtBlock2);

			sdtBlock1.Append(sdtProperties1);
			sdtBlock1.Append(sdtEndCharProperties1);
			sdtBlock1.Append(sdtContentBlock1);

			Paragraph paragraph29 = new Paragraph() {RsidParagraphAddition = "0005059E", RsidRunAdditionDefault = "0005059E"};

			ParagraphProperties paragraphProperties10 = new ParagraphProperties();
			ParagraphStyleId paragraphStyleId4 = new ParagraphStyleId() {Val = "Footer"};

			paragraphProperties10.Append(paragraphStyleId4);

			paragraph29.Append(paragraphProperties10);

			footer2.Append(sdtBlock1);
			footer2.Append(paragraph29);

			footerPart2.Footer = footer2;
		}

		// Generates content of documentSettingsPart1.
		private void GenerateDocumentSettingsPart1Content(DocumentSettingsPart documentSettingsPart1)
		{
			Settings settings1 = new Settings() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14"}};
			settings1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			settings1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			settings1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			settings1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			settings1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			settings1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			settings1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			settings1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			settings1.AddNamespaceDeclaration("sl", "http://schemas.openxmlformats.org/schemaLibrary/2006/main");
			Zoom zoom1 = new Zoom() {Percent = "100"};
			ProofState proofState1 = new ProofState() {Spelling = ProofingStateValues.Clean, Grammar = ProofingStateValues.Clean};

			MailMerge mailMerge1 = new MailMerge();
			MainDocumentType mainDocumentType1 = new MainDocumentType() {Val = MailMergeDocumentValues.FormLetter};
			DataType dataType1 = new DataType() {Val = MailMergeDataValues.TextFile};
			ActiveRecord activeRecord1 = new ActiveRecord() {Val = -1};

			mailMerge1.Append(mainDocumentType1);
			mailMerge1.Append(dataType1);
			mailMerge1.Append(activeRecord1);
			DefaultTabStop defaultTabStop1 = new DefaultTabStop() {Val = 720};
			CharacterSpacingControl characterSpacingControl1 = new CharacterSpacingControl()
			                                                   {Val = CharacterSpacingValues.DoNotCompress};

			HeaderShapeDefaults headerShapeDefaults1 = new HeaderShapeDefaults();
			Ovml.ShapeDefaults shapeDefaults1 = new Ovml.ShapeDefaults()
			                                    {Extension = V.ExtensionHandlingBehaviorValues.Edit, MaxShapeId = 6145};

			headerShapeDefaults1.Append(shapeDefaults1);

			FootnoteDocumentWideProperties footnoteDocumentWideProperties1 = new FootnoteDocumentWideProperties();
			FootnoteSpecialReference footnoteSpecialReference1 = new FootnoteSpecialReference() {Id = -1};
			FootnoteSpecialReference footnoteSpecialReference2 = new FootnoteSpecialReference() {Id = 0};

			footnoteDocumentWideProperties1.Append(footnoteSpecialReference1);
			footnoteDocumentWideProperties1.Append(footnoteSpecialReference2);

			EndnoteDocumentWideProperties endnoteDocumentWideProperties1 = new EndnoteDocumentWideProperties();
			EndnoteSpecialReference endnoteSpecialReference1 = new EndnoteSpecialReference() {Id = -1};
			EndnoteSpecialReference endnoteSpecialReference2 = new EndnoteSpecialReference() {Id = 0};

			endnoteDocumentWideProperties1.Append(endnoteSpecialReference1);
			endnoteDocumentWideProperties1.Append(endnoteSpecialReference2);

			Compatibility compatibility1 = new Compatibility();
			CompatibilitySetting compatibilitySetting1 = new CompatibilitySetting()
			                                             {
			                                             	Name = CompatSettingNameValues.CompatibilityMode,
			                                             	Uri = "http://schemas.microsoft.com/office/word",
			                                             	Val = "14"
			                                             };
			CompatibilitySetting compatibilitySetting2 = new CompatibilitySetting()
			                                             {
			                                             	Name =
			                                             		CompatSettingNameValues.OverrideTableStyleFontSizeAndJustification,
			                                             	Uri = "http://schemas.microsoft.com/office/word",
			                                             	Val = "1"
			                                             };
			CompatibilitySetting compatibilitySetting3 = new CompatibilitySetting()
			                                             {
			                                             	Name = CompatSettingNameValues.EnableOpenTypeFeatures,
			                                             	Uri = "http://schemas.microsoft.com/office/word",
			                                             	Val = "1"
			                                             };
			CompatibilitySetting compatibilitySetting4 = new CompatibilitySetting()
			                                             {
			                                             	Name = CompatSettingNameValues.DoNotFlipMirrorIndents,
			                                             	Uri = "http://schemas.microsoft.com/office/word",
			                                             	Val = "1"
			                                             };

			compatibility1.Append(compatibilitySetting1);
			compatibility1.Append(compatibilitySetting2);
			compatibility1.Append(compatibilitySetting3);
			compatibility1.Append(compatibilitySetting4);

			Rsids rsids1 = new Rsids();
			RsidRoot rsidRoot1 = new RsidRoot() {Val = "002307CA"};
			Rsid rsid25 = new Rsid() {Val = "00021925"};
			Rsid rsid26 = new Rsid() {Val = "0005059E"};
			Rsid rsid27 = new Rsid() {Val = "002307CA"};
			Rsid rsid28 = new Rsid() {Val = "00267818"};
			Rsid rsid29 = new Rsid() {Val = "002E2CE3"};
			Rsid rsid30 = new Rsid() {Val = "00367B71"};
			Rsid rsid31 = new Rsid() {Val = "003C716F"};
			Rsid rsid32 = new Rsid() {Val = "00442AD3"};
			Rsid rsid33 = new Rsid() {Val = "00485B24"};
			Rsid rsid34 = new Rsid() {Val = "004B0395"};
			Rsid rsid35 = new Rsid() {Val = "005A7CCB"};
			Rsid rsid36 = new Rsid() {Val = "00664E23"};
			Rsid rsid37 = new Rsid() {Val = "006D0F9D"};
			Rsid rsid38 = new Rsid() {Val = "006F12E1"};
			Rsid rsid39 = new Rsid() {Val = "00731A43"};
			Rsid rsid40 = new Rsid() {Val = "00736D14"};
			Rsid rsid41 = new Rsid() {Val = "007977E5"};
			Rsid rsid42 = new Rsid() {Val = "007B10F8"};
			Rsid rsid43 = new Rsid() {Val = "007C67E7"};
			Rsid rsid44 = new Rsid() {Val = "00960953"};
			Rsid rsid45 = new Rsid() {Val = "009F02C7"};
			Rsid rsid46 = new Rsid() {Val = "00A056C7"};
			Rsid rsid47 = new Rsid() {Val = "00B32099"};
			Rsid rsid48 = new Rsid() {Val = "00B92CF0"};
			Rsid rsid49 = new Rsid() {Val = "00BC57A3"};
			Rsid rsid50 = new Rsid() {Val = "00BE6B68"};
			Rsid rsid51 = new Rsid() {Val = "00BF235B"};
			Rsid rsid52 = new Rsid() {Val = "00BF5126"};
			Rsid rsid53 = new Rsid() {Val = "00C91302"};
			Rsid rsid54 = new Rsid() {Val = "00CA68EB"};
			Rsid rsid55 = new Rsid() {Val = "00E33AA1"};
			Rsid rsid56 = new Rsid() {Val = "00E40D36"};
			Rsid rsid57 = new Rsid() {Val = "00E7460D"};
			Rsid rsid58 = new Rsid() {Val = "00E95156"};
			Rsid rsid59 = new Rsid() {Val = "00F06304"};
			Rsid rsid60 = new Rsid() {Val = "00FA5EBC"};
			Rsid rsid61 = new Rsid() {Val = "00FB1F22"};

			rsids1.Append(rsidRoot1);
			rsids1.Append(rsid25);
			rsids1.Append(rsid26);
			rsids1.Append(rsid27);
			rsids1.Append(rsid28);
			rsids1.Append(rsid29);
			rsids1.Append(rsid30);
			rsids1.Append(rsid31);
			rsids1.Append(rsid32);
			rsids1.Append(rsid33);
			rsids1.Append(rsid34);
			rsids1.Append(rsid35);
			rsids1.Append(rsid36);
			rsids1.Append(rsid37);
			rsids1.Append(rsid38);
			rsids1.Append(rsid39);
			rsids1.Append(rsid40);
			rsids1.Append(rsid41);
			rsids1.Append(rsid42);
			rsids1.Append(rsid43);
			rsids1.Append(rsid44);
			rsids1.Append(rsid45);
			rsids1.Append(rsid46);
			rsids1.Append(rsid47);
			rsids1.Append(rsid48);
			rsids1.Append(rsid49);
			rsids1.Append(rsid50);
			rsids1.Append(rsid51);
			rsids1.Append(rsid52);
			rsids1.Append(rsid53);
			rsids1.Append(rsid54);
			rsids1.Append(rsid55);
			rsids1.Append(rsid56);
			rsids1.Append(rsid57);
			rsids1.Append(rsid58);
			rsids1.Append(rsid59);
			rsids1.Append(rsid60);
			rsids1.Append(rsid61);

			M.MathProperties mathProperties1 = new M.MathProperties();
			M.MathFont mathFont1 = new M.MathFont() {Val = "Cambria Math"};
			M.BreakBinary breakBinary1 = new M.BreakBinary() {Val = M.BreakBinaryOperatorValues.Before};
			M.BreakBinarySubtraction breakBinarySubtraction1 = new M.BreakBinarySubtraction()
			                                                   {Val = M.BreakBinarySubtractionValues.MinusMinus};
			M.SmallFraction smallFraction1 = new M.SmallFraction() {Val = M.BooleanValues.Zero};
			M.DisplayDefaults displayDefaults1 = new M.DisplayDefaults();
			M.LeftMargin leftMargin1 = new M.LeftMargin() {Val = (UInt32Value) 0U};
			M.RightMargin rightMargin1 = new M.RightMargin() {Val = (UInt32Value) 0U};
			M.DefaultJustification defaultJustification1 = new M.DefaultJustification() {Val = M.JustificationValues.CenterGroup};
			M.WrapIndent wrapIndent1 = new M.WrapIndent() {Val = (UInt32Value) 1440U};
			M.IntegralLimitLocation integralLimitLocation1 = new M.IntegralLimitLocation()
			                                                 {Val = M.LimitLocationValues.SubscriptSuperscript};
			M.NaryLimitLocation naryLimitLocation1 = new M.NaryLimitLocation() {Val = M.LimitLocationValues.UnderOver};

			mathProperties1.Append(mathFont1);
			mathProperties1.Append(breakBinary1);
			mathProperties1.Append(breakBinarySubtraction1);
			mathProperties1.Append(smallFraction1);
			mathProperties1.Append(displayDefaults1);
			mathProperties1.Append(leftMargin1);
			mathProperties1.Append(rightMargin1);
			mathProperties1.Append(defaultJustification1);
			mathProperties1.Append(wrapIndent1);
			mathProperties1.Append(integralLimitLocation1);
			mathProperties1.Append(naryLimitLocation1);
			ThemeFontLanguages themeFontLanguages1 = new ThemeFontLanguages() {Val = "en-US"};
			ColorSchemeMapping colorSchemeMapping1 = new ColorSchemeMapping()
			                                         {
			                                         	Background1 = ColorSchemeIndexValues.Light1,
			                                         	Text1 = ColorSchemeIndexValues.Dark1,
			                                         	Background2 = ColorSchemeIndexValues.Light2,
			                                         	Text2 = ColorSchemeIndexValues.Dark2,
			                                         	Accent1 = ColorSchemeIndexValues.Accent1,
			                                         	Accent2 = ColorSchemeIndexValues.Accent2,
			                                         	Accent3 = ColorSchemeIndexValues.Accent3,
			                                         	Accent4 = ColorSchemeIndexValues.Accent4,
			                                         	Accent5 = ColorSchemeIndexValues.Accent5,
			                                         	Accent6 = ColorSchemeIndexValues.Accent6,
			                                         	Hyperlink = ColorSchemeIndexValues.Hyperlink,
			                                         	FollowedHyperlink = ColorSchemeIndexValues.FollowedHyperlink
			                                         };

			ShapeDefaults shapeDefaults2 = new ShapeDefaults();
			Ovml.ShapeDefaults shapeDefaults3 = new Ovml.ShapeDefaults()
			                                    {Extension = V.ExtensionHandlingBehaviorValues.Edit, MaxShapeId = 6145};

			Ovml.ShapeLayout shapeLayout1 = new Ovml.ShapeLayout() {Extension = V.ExtensionHandlingBehaviorValues.Edit};
			Ovml.ShapeIdMap shapeIdMap1 = new Ovml.ShapeIdMap() {Extension = V.ExtensionHandlingBehaviorValues.Edit, Data = "1"};

			shapeLayout1.Append(shapeIdMap1);

			shapeDefaults2.Append(shapeDefaults3);
			shapeDefaults2.Append(shapeLayout1);
			DecimalSymbol decimalSymbol1 = new DecimalSymbol() {Val = "."};
			ListSeparator listSeparator1 = new ListSeparator() {Val = ","};

			settings1.Append(zoom1);
			settings1.Append(proofState1);
			settings1.Append(mailMerge1);
			settings1.Append(defaultTabStop1);
			settings1.Append(characterSpacingControl1);
			settings1.Append(headerShapeDefaults1);
			settings1.Append(footnoteDocumentWideProperties1);
			settings1.Append(endnoteDocumentWideProperties1);
			settings1.Append(compatibility1);
			settings1.Append(rsids1);
			settings1.Append(mathProperties1);
			settings1.Append(themeFontLanguages1);
			settings1.Append(colorSchemeMapping1);
			settings1.Append(shapeDefaults2);
			settings1.Append(decimalSymbol1);
			settings1.Append(listSeparator1);

			documentSettingsPart1.Settings = settings1;
		}

		// Generates content of headerPart2.
		private void GenerateHeaderPart2Content(HeaderPart headerPart2)
		{
			Header header2 = new Header() {MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "w14 wp14"}};
			header2.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
			header2.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
			header2.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
			header2.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			header2.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
			header2.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
			header2.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			header2.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			header2.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
			header2.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			header2.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
			header2.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
			header2.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
			header2.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
			header2.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

			Paragraph paragraph30 = new Paragraph() {RsidParagraphAddition = "0005059E", RsidRunAdditionDefault = "0005059E"};

			ParagraphProperties paragraphProperties11 = new ParagraphProperties();
			ParagraphStyleId paragraphStyleId5 = new ParagraphStyleId() {Val = "Header"};

			paragraphProperties11.Append(paragraphStyleId5);

			paragraph30.Append(paragraphProperties11);

			Paragraph paragraph31 = new Paragraph() {RsidParagraphAddition = "0005059E", RsidRunAdditionDefault = "0005059E"};

			ParagraphProperties paragraphProperties12 = new ParagraphProperties();
			ParagraphStyleId paragraphStyleId6 = new ParagraphStyleId() {Val = "Header"};

			paragraphProperties12.Append(paragraphStyleId6);

			paragraph31.Append(paragraphProperties12);

			header2.Append(paragraph30);
			header2.Append(paragraph31);

			headerPart2.Header = header2;
		}

		// Generates content of themePart1.
		private void GenerateThemePart1Content(ThemePart themePart1)
		{
			A.Theme theme1 = new A.Theme() {Name = "Office Theme"};
			theme1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

			A.ThemeElements themeElements1 = new A.ThemeElements();

			A.ColorScheme colorScheme1 = new A.ColorScheme() {Name = "Office"};

			A.Dark1Color dark1Color1 = new A.Dark1Color();
			A.SystemColor systemColor1 = new A.SystemColor() {Val = A.SystemColorValues.WindowText, LastColor = "000000"};

			dark1Color1.Append(systemColor1);

			A.Light1Color light1Color1 = new A.Light1Color();
			A.SystemColor systemColor2 = new A.SystemColor() {Val = A.SystemColorValues.Window, LastColor = "FFFFFF"};

			light1Color1.Append(systemColor2);

			A.Dark2Color dark2Color1 = new A.Dark2Color();
			A.RgbColorModelHex rgbColorModelHex1 = new A.RgbColorModelHex() {Val = "1F497D"};

			dark2Color1.Append(rgbColorModelHex1);

			A.Light2Color light2Color1 = new A.Light2Color();
			A.RgbColorModelHex rgbColorModelHex2 = new A.RgbColorModelHex() {Val = "EEECE1"};

			light2Color1.Append(rgbColorModelHex2);

			A.Accent1Color accent1Color1 = new A.Accent1Color();
			A.RgbColorModelHex rgbColorModelHex3 = new A.RgbColorModelHex() {Val = "4F81BD"};

			accent1Color1.Append(rgbColorModelHex3);

			A.Accent2Color accent2Color1 = new A.Accent2Color();
			A.RgbColorModelHex rgbColorModelHex4 = new A.RgbColorModelHex() {Val = "C0504D"};

			accent2Color1.Append(rgbColorModelHex4);

			A.Accent3Color accent3Color1 = new A.Accent3Color();
			A.RgbColorModelHex rgbColorModelHex5 = new A.RgbColorModelHex() {Val = "9BBB59"};

			accent3Color1.Append(rgbColorModelHex5);

			A.Accent4Color accent4Color1 = new A.Accent4Color();
			A.RgbColorModelHex rgbColorModelHex6 = new A.RgbColorModelHex() {Val = "8064A2"};

			accent4Color1.Append(rgbColorModelHex6);

			A.Accent5Color accent5Color1 = new A.Accent5Color();
			A.RgbColorModelHex rgbColorModelHex7 = new A.RgbColorModelHex() {Val = "4BACC6"};

			accent5Color1.Append(rgbColorModelHex7);

			A.Accent6Color accent6Color1 = new A.Accent6Color();
			A.RgbColorModelHex rgbColorModelHex8 = new A.RgbColorModelHex() {Val = "F79646"};

			accent6Color1.Append(rgbColorModelHex8);

			A.Hyperlink hyperlink1 = new A.Hyperlink();
			A.RgbColorModelHex rgbColorModelHex9 = new A.RgbColorModelHex() {Val = "0000FF"};

			hyperlink1.Append(rgbColorModelHex9);

			A.FollowedHyperlinkColor followedHyperlinkColor1 = new A.FollowedHyperlinkColor();
			A.RgbColorModelHex rgbColorModelHex10 = new A.RgbColorModelHex() {Val = "800080"};

			followedHyperlinkColor1.Append(rgbColorModelHex10);

			colorScheme1.Append(dark1Color1);
			colorScheme1.Append(light1Color1);
			colorScheme1.Append(dark2Color1);
			colorScheme1.Append(light2Color1);
			colorScheme1.Append(accent1Color1);
			colorScheme1.Append(accent2Color1);
			colorScheme1.Append(accent3Color1);
			colorScheme1.Append(accent4Color1);
			colorScheme1.Append(accent5Color1);
			colorScheme1.Append(accent6Color1);
			colorScheme1.Append(hyperlink1);
			colorScheme1.Append(followedHyperlinkColor1);

			A.FontScheme fontScheme1 = new A.FontScheme() {Name = "Office"};

			A.MajorFont majorFont1 = new A.MajorFont();
			A.LatinFont latinFont1 = new A.LatinFont() {Typeface = "Cambria"};
			A.EastAsianFont eastAsianFont1 = new A.EastAsianFont() {Typeface = ""};
			A.ComplexScriptFont complexScriptFont1 = new A.ComplexScriptFont() {Typeface = ""};
			A.SupplementalFont supplementalFont1 = new A.SupplementalFont() {Script = "Jpan", Typeface = "ＭＳ ゴシック"};
			A.SupplementalFont supplementalFont2 = new A.SupplementalFont() {Script = "Hang", Typeface = "맑은 고딕"};
			A.SupplementalFont supplementalFont3 = new A.SupplementalFont() {Script = "Hans", Typeface = "宋体"};
			A.SupplementalFont supplementalFont4 = new A.SupplementalFont() {Script = "Hant", Typeface = "新細明體"};
			A.SupplementalFont supplementalFont5 = new A.SupplementalFont() {Script = "Arab", Typeface = "Times New Roman"};
			A.SupplementalFont supplementalFont6 = new A.SupplementalFont() {Script = "Hebr", Typeface = "Times New Roman"};
			A.SupplementalFont supplementalFont7 = new A.SupplementalFont() {Script = "Thai", Typeface = "Angsana New"};
			A.SupplementalFont supplementalFont8 = new A.SupplementalFont() {Script = "Ethi", Typeface = "Nyala"};
			A.SupplementalFont supplementalFont9 = new A.SupplementalFont() {Script = "Beng", Typeface = "Vrinda"};
			A.SupplementalFont supplementalFont10 = new A.SupplementalFont() {Script = "Gujr", Typeface = "Shruti"};
			A.SupplementalFont supplementalFont11 = new A.SupplementalFont() {Script = "Khmr", Typeface = "MoolBoran"};
			A.SupplementalFont supplementalFont12 = new A.SupplementalFont() {Script = "Knda", Typeface = "Tunga"};
			A.SupplementalFont supplementalFont13 = new A.SupplementalFont() {Script = "Guru", Typeface = "Raavi"};
			A.SupplementalFont supplementalFont14 = new A.SupplementalFont() {Script = "Cans", Typeface = "Euphemia"};
			A.SupplementalFont supplementalFont15 = new A.SupplementalFont() {Script = "Cher", Typeface = "Plantagenet Cherokee"};
			A.SupplementalFont supplementalFont16 = new A.SupplementalFont() {Script = "Yiii", Typeface = "Microsoft Yi Baiti"};
			A.SupplementalFont supplementalFont17 = new A.SupplementalFont() {Script = "Tibt", Typeface = "Microsoft Himalaya"};
			A.SupplementalFont supplementalFont18 = new A.SupplementalFont() {Script = "Thaa", Typeface = "MV Boli"};
			A.SupplementalFont supplementalFont19 = new A.SupplementalFont() {Script = "Deva", Typeface = "Mangal"};
			A.SupplementalFont supplementalFont20 = new A.SupplementalFont() {Script = "Telu", Typeface = "Gautami"};
			A.SupplementalFont supplementalFont21 = new A.SupplementalFont() {Script = "Taml", Typeface = "Latha"};
			A.SupplementalFont supplementalFont22 = new A.SupplementalFont() {Script = "Syrc", Typeface = "Estrangelo Edessa"};
			A.SupplementalFont supplementalFont23 = new A.SupplementalFont() {Script = "Orya", Typeface = "Kalinga"};
			A.SupplementalFont supplementalFont24 = new A.SupplementalFont() {Script = "Mlym", Typeface = "Kartika"};
			A.SupplementalFont supplementalFont25 = new A.SupplementalFont() {Script = "Laoo", Typeface = "DokChampa"};
			A.SupplementalFont supplementalFont26 = new A.SupplementalFont() {Script = "Sinh", Typeface = "Iskoola Pota"};
			A.SupplementalFont supplementalFont27 = new A.SupplementalFont() {Script = "Mong", Typeface = "Mongolian Baiti"};
			A.SupplementalFont supplementalFont28 = new A.SupplementalFont() {Script = "Viet", Typeface = "Times New Roman"};
			A.SupplementalFont supplementalFont29 = new A.SupplementalFont() {Script = "Uigh", Typeface = "Microsoft Uighur"};
			A.SupplementalFont supplementalFont30 = new A.SupplementalFont() {Script = "Geor", Typeface = "Sylfaen"};

			majorFont1.Append(latinFont1);
			majorFont1.Append(eastAsianFont1);
			majorFont1.Append(complexScriptFont1);
			majorFont1.Append(supplementalFont1);
			majorFont1.Append(supplementalFont2);
			majorFont1.Append(supplementalFont3);
			majorFont1.Append(supplementalFont4);
			majorFont1.Append(supplementalFont5);
			majorFont1.Append(supplementalFont6);
			majorFont1.Append(supplementalFont7);
			majorFont1.Append(supplementalFont8);
			majorFont1.Append(supplementalFont9);
			majorFont1.Append(supplementalFont10);
			majorFont1.Append(supplementalFont11);
			majorFont1.Append(supplementalFont12);
			majorFont1.Append(supplementalFont13);
			majorFont1.Append(supplementalFont14);
			majorFont1.Append(supplementalFont15);
			majorFont1.Append(supplementalFont16);
			majorFont1.Append(supplementalFont17);
			majorFont1.Append(supplementalFont18);
			majorFont1.Append(supplementalFont19);
			majorFont1.Append(supplementalFont20);
			majorFont1.Append(supplementalFont21);
			majorFont1.Append(supplementalFont22);
			majorFont1.Append(supplementalFont23);
			majorFont1.Append(supplementalFont24);
			majorFont1.Append(supplementalFont25);
			majorFont1.Append(supplementalFont26);
			majorFont1.Append(supplementalFont27);
			majorFont1.Append(supplementalFont28);
			majorFont1.Append(supplementalFont29);
			majorFont1.Append(supplementalFont30);

			A.MinorFont minorFont1 = new A.MinorFont();
			A.LatinFont latinFont2 = new A.LatinFont() {Typeface = "Calibri"};
			A.EastAsianFont eastAsianFont2 = new A.EastAsianFont() {Typeface = ""};
			A.ComplexScriptFont complexScriptFont2 = new A.ComplexScriptFont() {Typeface = ""};
			A.SupplementalFont supplementalFont31 = new A.SupplementalFont() {Script = "Jpan", Typeface = "ＭＳ 明朝"};
			A.SupplementalFont supplementalFont32 = new A.SupplementalFont() {Script = "Hang", Typeface = "맑은 고딕"};
			A.SupplementalFont supplementalFont33 = new A.SupplementalFont() {Script = "Hans", Typeface = "宋体"};
			A.SupplementalFont supplementalFont34 = new A.SupplementalFont() {Script = "Hant", Typeface = "新細明體"};
			A.SupplementalFont supplementalFont35 = new A.SupplementalFont() {Script = "Arab", Typeface = "Arial"};
			A.SupplementalFont supplementalFont36 = new A.SupplementalFont() {Script = "Hebr", Typeface = "Arial"};
			A.SupplementalFont supplementalFont37 = new A.SupplementalFont() {Script = "Thai", Typeface = "Cordia New"};
			A.SupplementalFont supplementalFont38 = new A.SupplementalFont() {Script = "Ethi", Typeface = "Nyala"};
			A.SupplementalFont supplementalFont39 = new A.SupplementalFont() {Script = "Beng", Typeface = "Vrinda"};
			A.SupplementalFont supplementalFont40 = new A.SupplementalFont() {Script = "Gujr", Typeface = "Shruti"};
			A.SupplementalFont supplementalFont41 = new A.SupplementalFont() {Script = "Khmr", Typeface = "DaunPenh"};
			A.SupplementalFont supplementalFont42 = new A.SupplementalFont() {Script = "Knda", Typeface = "Tunga"};
			A.SupplementalFont supplementalFont43 = new A.SupplementalFont() {Script = "Guru", Typeface = "Raavi"};
			A.SupplementalFont supplementalFont44 = new A.SupplementalFont() {Script = "Cans", Typeface = "Euphemia"};
			A.SupplementalFont supplementalFont45 = new A.SupplementalFont() {Script = "Cher", Typeface = "Plantagenet Cherokee"};
			A.SupplementalFont supplementalFont46 = new A.SupplementalFont() {Script = "Yiii", Typeface = "Microsoft Yi Baiti"};
			A.SupplementalFont supplementalFont47 = new A.SupplementalFont() {Script = "Tibt", Typeface = "Microsoft Himalaya"};
			A.SupplementalFont supplementalFont48 = new A.SupplementalFont() {Script = "Thaa", Typeface = "MV Boli"};
			A.SupplementalFont supplementalFont49 = new A.SupplementalFont() {Script = "Deva", Typeface = "Mangal"};
			A.SupplementalFont supplementalFont50 = new A.SupplementalFont() {Script = "Telu", Typeface = "Gautami"};
			A.SupplementalFont supplementalFont51 = new A.SupplementalFont() {Script = "Taml", Typeface = "Latha"};
			A.SupplementalFont supplementalFont52 = new A.SupplementalFont() {Script = "Syrc", Typeface = "Estrangelo Edessa"};
			A.SupplementalFont supplementalFont53 = new A.SupplementalFont() {Script = "Orya", Typeface = "Kalinga"};
			A.SupplementalFont supplementalFont54 = new A.SupplementalFont() {Script = "Mlym", Typeface = "Kartika"};
			A.SupplementalFont supplementalFont55 = new A.SupplementalFont() {Script = "Laoo", Typeface = "DokChampa"};
			A.SupplementalFont supplementalFont56 = new A.SupplementalFont() {Script = "Sinh", Typeface = "Iskoola Pota"};
			A.SupplementalFont supplementalFont57 = new A.SupplementalFont() {Script = "Mong", Typeface = "Mongolian Baiti"};
			A.SupplementalFont supplementalFont58 = new A.SupplementalFont() {Script = "Viet", Typeface = "Arial"};
			A.SupplementalFont supplementalFont59 = new A.SupplementalFont() {Script = "Uigh", Typeface = "Microsoft Uighur"};
			A.SupplementalFont supplementalFont60 = new A.SupplementalFont() {Script = "Geor", Typeface = "Sylfaen"};

			minorFont1.Append(latinFont2);
			minorFont1.Append(eastAsianFont2);
			minorFont1.Append(complexScriptFont2);
			minorFont1.Append(supplementalFont31);
			minorFont1.Append(supplementalFont32);
			minorFont1.Append(supplementalFont33);
			minorFont1.Append(supplementalFont34);
			minorFont1.Append(supplementalFont35);
			minorFont1.Append(supplementalFont36);
			minorFont1.Append(supplementalFont37);
			minorFont1.Append(supplementalFont38);
			minorFont1.Append(supplementalFont39);
			minorFont1.Append(supplementalFont40);
			minorFont1.Append(supplementalFont41);
			minorFont1.Append(supplementalFont42);
			minorFont1.Append(supplementalFont43);
			minorFont1.Append(supplementalFont44);
			minorFont1.Append(supplementalFont45);
			minorFont1.Append(supplementalFont46);
			minorFont1.Append(supplementalFont47);
			minorFont1.Append(supplementalFont48);
			minorFont1.Append(supplementalFont49);
			minorFont1.Append(supplementalFont50);
			minorFont1.Append(supplementalFont51);
			minorFont1.Append(supplementalFont52);
			minorFont1.Append(supplementalFont53);
			minorFont1.Append(supplementalFont54);
			minorFont1.Append(supplementalFont55);
			minorFont1.Append(supplementalFont56);
			minorFont1.Append(supplementalFont57);
			minorFont1.Append(supplementalFont58);
			minorFont1.Append(supplementalFont59);
			minorFont1.Append(supplementalFont60);

			fontScheme1.Append(majorFont1);
			fontScheme1.Append(minorFont1);

			A.FormatScheme formatScheme1 = new A.FormatScheme() {Name = "Office"};

			A.FillStyleList fillStyleList1 = new A.FillStyleList();

			A.SolidFill solidFill1 = new A.SolidFill();
			A.SchemeColor schemeColor1 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};

			solidFill1.Append(schemeColor1);

			A.GradientFill gradientFill1 = new A.GradientFill() {RotateWithShape = true};

			A.GradientStopList gradientStopList1 = new A.GradientStopList();

			A.GradientStop gradientStop1 = new A.GradientStop() {Position = 0};

			A.SchemeColor schemeColor2 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Tint tint1 = new A.Tint() {Val = 50000};
			A.SaturationModulation saturationModulation1 = new A.SaturationModulation() {Val = 300000};

			schemeColor2.Append(tint1);
			schemeColor2.Append(saturationModulation1);

			gradientStop1.Append(schemeColor2);

			A.GradientStop gradientStop2 = new A.GradientStop() {Position = 35000};

			A.SchemeColor schemeColor3 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Tint tint2 = new A.Tint() {Val = 37000};
			A.SaturationModulation saturationModulation2 = new A.SaturationModulation() {Val = 300000};

			schemeColor3.Append(tint2);
			schemeColor3.Append(saturationModulation2);

			gradientStop2.Append(schemeColor3);

			A.GradientStop gradientStop3 = new A.GradientStop() {Position = 100000};

			A.SchemeColor schemeColor4 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Tint tint3 = new A.Tint() {Val = 15000};
			A.SaturationModulation saturationModulation3 = new A.SaturationModulation() {Val = 350000};

			schemeColor4.Append(tint3);
			schemeColor4.Append(saturationModulation3);

			gradientStop3.Append(schemeColor4);

			gradientStopList1.Append(gradientStop1);
			gradientStopList1.Append(gradientStop2);
			gradientStopList1.Append(gradientStop3);
			A.LinearGradientFill linearGradientFill1 = new A.LinearGradientFill() {Angle = 16200000, Scaled = true};

			gradientFill1.Append(gradientStopList1);
			gradientFill1.Append(linearGradientFill1);

			A.GradientFill gradientFill2 = new A.GradientFill() {RotateWithShape = true};

			A.GradientStopList gradientStopList2 = new A.GradientStopList();

			A.GradientStop gradientStop4 = new A.GradientStop() {Position = 0};

			A.SchemeColor schemeColor5 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Shade shade1 = new A.Shade() {Val = 51000};
			A.SaturationModulation saturationModulation4 = new A.SaturationModulation() {Val = 130000};

			schemeColor5.Append(shade1);
			schemeColor5.Append(saturationModulation4);

			gradientStop4.Append(schemeColor5);

			A.GradientStop gradientStop5 = new A.GradientStop() {Position = 80000};

			A.SchemeColor schemeColor6 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Shade shade2 = new A.Shade() {Val = 93000};
			A.SaturationModulation saturationModulation5 = new A.SaturationModulation() {Val = 130000};

			schemeColor6.Append(shade2);
			schemeColor6.Append(saturationModulation5);

			gradientStop5.Append(schemeColor6);

			A.GradientStop gradientStop6 = new A.GradientStop() {Position = 100000};

			A.SchemeColor schemeColor7 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Shade shade3 = new A.Shade() {Val = 94000};
			A.SaturationModulation saturationModulation6 = new A.SaturationModulation() {Val = 135000};

			schemeColor7.Append(shade3);
			schemeColor7.Append(saturationModulation6);

			gradientStop6.Append(schemeColor7);

			gradientStopList2.Append(gradientStop4);
			gradientStopList2.Append(gradientStop5);
			gradientStopList2.Append(gradientStop6);
			A.LinearGradientFill linearGradientFill2 = new A.LinearGradientFill() {Angle = 16200000, Scaled = false};

			gradientFill2.Append(gradientStopList2);
			gradientFill2.Append(linearGradientFill2);

			fillStyleList1.Append(solidFill1);
			fillStyleList1.Append(gradientFill1);
			fillStyleList1.Append(gradientFill2);

			A.LineStyleList lineStyleList1 = new A.LineStyleList();

			A.Outline outline2 = new A.Outline()
			                     {
			                     	Width = 9525,
			                     	CapType = A.LineCapValues.Flat,
			                     	CompoundLineType = A.CompoundLineValues.Single,
			                     	Alignment = A.PenAlignmentValues.Center
			                     };

			A.SolidFill solidFill2 = new A.SolidFill();

			A.SchemeColor schemeColor8 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Shade shade4 = new A.Shade() {Val = 95000};
			A.SaturationModulation saturationModulation7 = new A.SaturationModulation() {Val = 105000};

			schemeColor8.Append(shade4);
			schemeColor8.Append(saturationModulation7);

			solidFill2.Append(schemeColor8);
			A.PresetDash presetDash1 = new A.PresetDash() {Val = A.PresetLineDashValues.Solid};

			outline2.Append(solidFill2);
			outline2.Append(presetDash1);

			A.Outline outline3 = new A.Outline()
			                     {
			                     	Width = 25400,
			                     	CapType = A.LineCapValues.Flat,
			                     	CompoundLineType = A.CompoundLineValues.Single,
			                     	Alignment = A.PenAlignmentValues.Center
			                     };

			A.SolidFill solidFill3 = new A.SolidFill();
			A.SchemeColor schemeColor9 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};

			solidFill3.Append(schemeColor9);
			A.PresetDash presetDash2 = new A.PresetDash() {Val = A.PresetLineDashValues.Solid};

			outline3.Append(solidFill3);
			outline3.Append(presetDash2);

			A.Outline outline4 = new A.Outline()
			                     {
			                     	Width = 38100,
			                     	CapType = A.LineCapValues.Flat,
			                     	CompoundLineType = A.CompoundLineValues.Single,
			                     	Alignment = A.PenAlignmentValues.Center
			                     };

			A.SolidFill solidFill4 = new A.SolidFill();
			A.SchemeColor schemeColor10 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};

			solidFill4.Append(schemeColor10);
			A.PresetDash presetDash3 = new A.PresetDash() {Val = A.PresetLineDashValues.Solid};

			outline4.Append(solidFill4);
			outline4.Append(presetDash3);

			lineStyleList1.Append(outline2);
			lineStyleList1.Append(outline3);
			lineStyleList1.Append(outline4);

			A.EffectStyleList effectStyleList1 = new A.EffectStyleList();

			A.EffectStyle effectStyle1 = new A.EffectStyle();

			A.EffectList effectList1 = new A.EffectList();

			A.OuterShadow outerShadow1 = new A.OuterShadow()
			                             {BlurRadius = 40000L, Distance = 20000L, Direction = 5400000, RotateWithShape = false};

			A.RgbColorModelHex rgbColorModelHex11 = new A.RgbColorModelHex() {Val = "000000"};
			A.Alpha alpha1 = new A.Alpha() {Val = 38000};

			rgbColorModelHex11.Append(alpha1);

			outerShadow1.Append(rgbColorModelHex11);

			effectList1.Append(outerShadow1);

			effectStyle1.Append(effectList1);

			A.EffectStyle effectStyle2 = new A.EffectStyle();

			A.EffectList effectList2 = new A.EffectList();

			A.OuterShadow outerShadow2 = new A.OuterShadow()
			                             {BlurRadius = 40000L, Distance = 23000L, Direction = 5400000, RotateWithShape = false};

			A.RgbColorModelHex rgbColorModelHex12 = new A.RgbColorModelHex() {Val = "000000"};
			A.Alpha alpha2 = new A.Alpha() {Val = 35000};

			rgbColorModelHex12.Append(alpha2);

			outerShadow2.Append(rgbColorModelHex12);

			effectList2.Append(outerShadow2);

			effectStyle2.Append(effectList2);

			A.EffectStyle effectStyle3 = new A.EffectStyle();

			A.EffectList effectList3 = new A.EffectList();

			A.OuterShadow outerShadow3 = new A.OuterShadow()
			                             {BlurRadius = 40000L, Distance = 23000L, Direction = 5400000, RotateWithShape = false};

			A.RgbColorModelHex rgbColorModelHex13 = new A.RgbColorModelHex() {Val = "000000"};
			A.Alpha alpha3 = new A.Alpha() {Val = 35000};

			rgbColorModelHex13.Append(alpha3);

			outerShadow3.Append(rgbColorModelHex13);

			effectList3.Append(outerShadow3);

			A.Scene3DType scene3DType1 = new A.Scene3DType();

			A.Camera camera1 = new A.Camera() {Preset = A.PresetCameraValues.OrthographicFront};
			A.Rotation rotation1 = new A.Rotation() {Latitude = 0, Longitude = 0, Revolution = 0};

			camera1.Append(rotation1);

			A.LightRig lightRig1 = new A.LightRig()
			                       {Rig = A.LightRigValues.ThreePoints, Direction = A.LightRigDirectionValues.Top};
			A.Rotation rotation2 = new A.Rotation() {Latitude = 0, Longitude = 0, Revolution = 1200000};

			lightRig1.Append(rotation2);

			scene3DType1.Append(camera1);
			scene3DType1.Append(lightRig1);

			A.Shape3DType shape3DType1 = new A.Shape3DType();
			A.BevelTop bevelTop1 = new A.BevelTop() {Width = 63500L, Height = 25400L};

			shape3DType1.Append(bevelTop1);

			effectStyle3.Append(effectList3);
			effectStyle3.Append(scene3DType1);
			effectStyle3.Append(shape3DType1);

			effectStyleList1.Append(effectStyle1);
			effectStyleList1.Append(effectStyle2);
			effectStyleList1.Append(effectStyle3);

			A.BackgroundFillStyleList backgroundFillStyleList1 = new A.BackgroundFillStyleList();

			A.SolidFill solidFill5 = new A.SolidFill();
			A.SchemeColor schemeColor11 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};

			solidFill5.Append(schemeColor11);

			A.GradientFill gradientFill3 = new A.GradientFill() {RotateWithShape = true};

			A.GradientStopList gradientStopList3 = new A.GradientStopList();

			A.GradientStop gradientStop7 = new A.GradientStop() {Position = 0};

			A.SchemeColor schemeColor12 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Tint tint4 = new A.Tint() {Val = 40000};
			A.SaturationModulation saturationModulation8 = new A.SaturationModulation() {Val = 350000};

			schemeColor12.Append(tint4);
			schemeColor12.Append(saturationModulation8);

			gradientStop7.Append(schemeColor12);

			A.GradientStop gradientStop8 = new A.GradientStop() {Position = 40000};

			A.SchemeColor schemeColor13 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Tint tint5 = new A.Tint() {Val = 45000};
			A.Shade shade5 = new A.Shade() {Val = 99000};
			A.SaturationModulation saturationModulation9 = new A.SaturationModulation() {Val = 350000};

			schemeColor13.Append(tint5);
			schemeColor13.Append(shade5);
			schemeColor13.Append(saturationModulation9);

			gradientStop8.Append(schemeColor13);

			A.GradientStop gradientStop9 = new A.GradientStop() {Position = 100000};

			A.SchemeColor schemeColor14 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Shade shade6 = new A.Shade() {Val = 20000};
			A.SaturationModulation saturationModulation10 = new A.SaturationModulation() {Val = 255000};

			schemeColor14.Append(shade6);
			schemeColor14.Append(saturationModulation10);

			gradientStop9.Append(schemeColor14);

			gradientStopList3.Append(gradientStop7);
			gradientStopList3.Append(gradientStop8);
			gradientStopList3.Append(gradientStop9);

			A.PathGradientFill pathGradientFill1 = new A.PathGradientFill() {Path = A.PathShadeValues.Circle};
			A.FillToRectangle fillToRectangle1 = new A.FillToRectangle()
			                                     {Left = 50000, Top = -80000, Right = 50000, Bottom = 180000};

			pathGradientFill1.Append(fillToRectangle1);

			gradientFill3.Append(gradientStopList3);
			gradientFill3.Append(pathGradientFill1);

			A.GradientFill gradientFill4 = new A.GradientFill() {RotateWithShape = true};

			A.GradientStopList gradientStopList4 = new A.GradientStopList();

			A.GradientStop gradientStop10 = new A.GradientStop() {Position = 0};

			A.SchemeColor schemeColor15 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Tint tint6 = new A.Tint() {Val = 80000};
			A.SaturationModulation saturationModulation11 = new A.SaturationModulation() {Val = 300000};

			schemeColor15.Append(tint6);
			schemeColor15.Append(saturationModulation11);

			gradientStop10.Append(schemeColor15);

			A.GradientStop gradientStop11 = new A.GradientStop() {Position = 100000};

			A.SchemeColor schemeColor16 = new A.SchemeColor() {Val = A.SchemeColorValues.PhColor};
			A.Shade shade7 = new A.Shade() {Val = 30000};
			A.SaturationModulation saturationModulation12 = new A.SaturationModulation() {Val = 200000};

			schemeColor16.Append(shade7);
			schemeColor16.Append(saturationModulation12);

			gradientStop11.Append(schemeColor16);

			gradientStopList4.Append(gradientStop10);
			gradientStopList4.Append(gradientStop11);

			A.PathGradientFill pathGradientFill2 = new A.PathGradientFill() {Path = A.PathShadeValues.Circle};
			A.FillToRectangle fillToRectangle2 = new A.FillToRectangle()
			                                     {Left = 50000, Top = 50000, Right = 50000, Bottom = 50000};

			pathGradientFill2.Append(fillToRectangle2);

			gradientFill4.Append(gradientStopList4);
			gradientFill4.Append(pathGradientFill2);

			backgroundFillStyleList1.Append(solidFill5);
			backgroundFillStyleList1.Append(gradientFill3);
			backgroundFillStyleList1.Append(gradientFill4);

			formatScheme1.Append(fillStyleList1);
			formatScheme1.Append(lineStyleList1);
			formatScheme1.Append(effectStyleList1);
			formatScheme1.Append(backgroundFillStyleList1);

			themeElements1.Append(colorScheme1);
			themeElements1.Append(fontScheme1);
			themeElements1.Append(formatScheme1);
			A.ObjectDefaults objectDefaults1 = new A.ObjectDefaults();
			A.ExtraColorSchemeList extraColorSchemeList1 = new A.ExtraColorSchemeList();

			theme1.Append(themeElements1);
			theme1.Append(objectDefaults1);
			theme1.Append(extraColorSchemeList1);

			themePart1.Theme = theme1;
		}

		private void SetPackageProperties(OpenXmlPackage document)
		{
			document.PackageProperties.Creator = "David";
			document.PackageProperties.Title = "";
			document.PackageProperties.Revision = "3";
			document.PackageProperties.Created = System.Xml.XmlConvert.ToDateTime("2012-05-13T15:07:00Z",
			                                                                      System.Xml.XmlDateTimeSerializationMode.
			                                                                      	RoundtripKind);
			document.PackageProperties.Modified = System.Xml.XmlConvert.ToDateTime("2012-05-13T15:13:00Z",
			                                                                       System.Xml.XmlDateTimeSerializationMode.
			                                                                       	RoundtripKind);
			document.PackageProperties.LastModifiedBy = "David";
		}
	}
}