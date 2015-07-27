using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CmsWeb.Areas.Public.Models
{
	public class Code39BarCode
	{
		public enum BarCodeWeight
		{
			Small = 1,
			Medium = 2,
			Large = 3
		}

		#region Private Member Variables
		private const int SpacingBetweenBarCodeAndText = 5;
		private const string InterchangeGap = "0";
		private const string code39alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

		private readonly string[] code39code =
		{
			/* 0 */ "000110100",
			/* 1 */ "100100001",
			/* 2 */ "001100001",
			/* 3 */ "101100000",
			/* 4 */ "000110001",
			/* 5 */ "100110000",
			/* 6 */ "001110000",
			/* 7 */ "000100101",
			/* 8 */ "100100100",
			/* 9 */ "001100100",
			/* A */ "100001001",
			/* B */ "001001001",
			/* C */ "101001000",
			/* D */ "000011001",
			/* E */ "100011000",
			/* F */ "001011000",
			/* G */ "000001101",
			/* H */ "100001100",
			/* I */ "001001100",
			/* J */ "000011100",
			/* K */ "100000011",
			/* L */ "001000011",
			/* M */ "101000010",
			/* N */ "000010011",
			/* O */ "100010010",
			/* P */ "001010010",
			/* Q */ "000000111",
			/* R */ "100000110",
			/* S */ "001000110",
			/* T */ "000010110",
			/* U */ "110000001",
			/* V */ "011000001",
			/* W */ "111000000",
			/* X */ "010010001",
			/* Y */ "110010000",
			/* Z */ "011010000",
			/* - */ "010000101",
			/* . */ "110000100",
			/*' '*/ "011000100",
			/* $ */ "010101000",
			/* / */ "010100010",
			/* + */ "010001010",
			/* % */ "000101010",
			/* * */ "010010100"
		};
		#endregion

		#region Constructors
		public Code39BarCode() : this(string.Empty) { }
		public Code39BarCode(string barCodeText) : this(barCodeText, 100) { }
		public Code39BarCode(string barCodeText, int height)
		{
			BarCodeText = barCodeText;
			Height = height;
			BarCodePadding = 5;
			ShowBarCodeText = true;
			Weight = BarCodeWeight.Medium;
			BarCodeTextFont = new Font("Arial", 10.0F);
			ImageFormat = ImageFormat.Gif;
		}
		#endregion

		#region Properties
		public string BarCodeText { get; set; }
		public BarCodeWeight Weight { get; set; }
		public int BarCodePadding { get; set; }
		public int Height { get; set; }
		public bool ShowBarCodeText { get; set; }
		public Font BarCodeTextFont { get; set; }
		public ImageFormat ImageFormat { get; set; }
		#endregion

		public byte[] Generate()
		{
			// Ensure BarCode property has been set
			if (string.IsNullOrWhiteSpace(this.BarCodeText))
				throw new ArgumentException("BarCode must be set prior to calling Generate");

			// Ensure BarCode does not contain invalid characters
			for (var i = 0; i < this.BarCodeText.Length; i++)
				if (code39alphabet.IndexOf(this.BarCodeText[i]) == -1)
					throw new ArgumentException($"Invalid character for barcode: '{this.BarCodeText[i]}' is not a valid code 39 character");


			// Create the encoded string
			var codeToGenerate = "*" + this.BarCodeText + "*";
			var encodedString = string.Empty;
			for (var i = 0; i < codeToGenerate.Length; i++)
			{
				if (i > 0)
					encodedString += InterchangeGap;

				encodedString += code39code[code39alphabet.IndexOf(codeToGenerate[i])];
			}

			// Determine the barcode width
			var widthOfBarCodeString = 0;
			const double wideToNarrowRatio = 3.0;
			for (var i = 0; i < encodedString.Length; i++)
			{
				if (encodedString[i] == '1')
					widthOfBarCodeString += (int)(wideToNarrowRatio * (int)Weight);
				else
					widthOfBarCodeString += (int)Weight;
			}

			var widthOfImage = widthOfBarCodeString + (BarCodePadding * 2);


			// Create Bitmap class
			using (var bmp = new Bitmap(widthOfImage, this.Height, PixelFormat.Format32bppArgb))
			{
				using (var gfx = Graphics.FromImage(bmp))
				{
					// Start with a white background
					gfx.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

					// Determine offset to center barCodeText and the height of barcode
					var barCodeTextSize = gfx.MeasureString(this.BarCodeText, this.BarCodeTextFont);
					var barCodeTextX = (widthOfImage - (int)barCodeTextSize.Width) / 2;
					var barCodeHeight = this.Height - (this.BarCodePadding * 2);
					if (this.ShowBarCodeText)
						barCodeHeight -= SpacingBetweenBarCodeAndText + (int)barCodeTextSize.Height;

					var x = BarCodePadding;
					var barCodeTop = BarCodePadding;
					var barCodeBottom = barCodeTop + barCodeHeight;

					for (var i = 0; i < encodedString.Length; i++)
					{
						int lineWidth;

						if (encodedString[i] == '1')
							lineWidth = (int)(wideToNarrowRatio * (int)Weight);
						else
							lineWidth = (int)Weight;

						gfx.FillRectangle(i % 2 == 0 ? Brushes.Black : Brushes.White, x, barCodeTop, lineWidth, barCodeBottom);

						x += lineWidth;
					}

					if (this.ShowBarCodeText)
					{
						var barCodeTextTop = barCodeBottom + SpacingBetweenBarCodeAndText;

						gfx.DrawString(this.BarCodeText, this.BarCodeTextFont, Brushes.Black, barCodeTextX, barCodeTextTop);
					}

					var output = new MemoryStream();
					bmp.Save(output, ImageFormat);
					return output.ToArray();
				}
			}
		}

	}
}
