using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace CmsData.Classes.Barcodes
{
	public class BarcodeHelper
	{
		public static byte[] generateQRCode( string contents, int width )
		{
			Bitmap qrBitmap = new Bitmap( width, width, PixelFormat.Format24bppRgb );
			BitMatrix qrCode = new QRCodeWriter().encode( contents, BarcodeFormat.QR_CODE, width, width );

			for( int iX = 0; iX < width; iX++ ) {
				for( int iY = 0; iY < width; iY++ ) {
					qrBitmap.SetPixel( iX, iY, qrCode[iX, iY] ? Color.Black : Color.White );
				}
			}

			using( MemoryStream stream = new MemoryStream() ) {
				qrBitmap.Save( stream, ImageFormat.Png );

				return stream.ToArray();
			}
		}
	}
}