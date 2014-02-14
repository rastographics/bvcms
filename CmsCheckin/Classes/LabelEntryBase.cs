using System;

namespace CmsCheckin.Classes
{
	class LabelEntryBase
	{
		public const int TYPE_STRING = 1;
		public const int TYPE_LINE = 2;
		public const int TYPE_BARCODE = 3;
		public const int TYPE_LABEL = 4;

		public const int ALIGN_LEFT = 1;
		public const int ALIGN_TOP = 1;
		public const int ALIGN_CENTER = 2;
		public const int ALIGN_RIGHT = 3;
		public const int ALIGN_BOTTOM = 3;

		public const int OFFSET = 2;

		public int iType = 0;
		public int iRepeat = 0;
		public float fRepeatOffset = 0;

		public string sFontName = "";
		public string sText = "";

		public int iWidth = 0;
		public int iHeight = 0;

		public int iAlignX = 0;
		public int iAlignY = 0;

		public float fSize = 0;
		public float fStartX = 0;
		public float fStartY = 0;
		public float fEndX = 0;
		public float fEndY = 0;

		public static LabelEntryBase create(string sFormat)
		{
			LabelEntryBase lebTemp = new LabelEntryBase();
			return lebTemp.populate(sFormat);
		}

		public LabelEntryBase populate(string sFormat)
		{
			string[] sParts = sFormat.Split(new char[] { ',' });

			iType = int.Parse(sParts[0]);
			iRepeat = int.Parse(sParts[1]);
			fRepeatOffset = float.Parse(sParts[2]);

			switch (iType)
			{
				case TYPE_STRING:
				case TYPE_LABEL:
					sFontName = sParts[OFFSET + 1];
					fSize = float.Parse(sParts[OFFSET + 2]);
					sText = sParts[OFFSET + 3];

					fStartX = float.Parse(sParts[OFFSET + 4]);
					fStartY = float.Parse(sParts[OFFSET + 5]);

					iAlignX = int.Parse(sParts[OFFSET + 6]);
					iAlignY = int.Parse(sParts[OFFSET + 7]);
					break;

				case TYPE_LINE:
					fSize = float.Parse(sParts[OFFSET + 1]);

					fStartX = float.Parse(sParts[OFFSET + 2]);
					fStartY = float.Parse(sParts[OFFSET + 3]);

					fEndX = float.Parse(sParts[OFFSET + 4]);
					fEndY = float.Parse(sParts[OFFSET + 5]);
					break;

				case TYPE_BARCODE:
					sText = sParts[OFFSET + 1];

					fStartX = float.Parse(sParts[OFFSET + 2]);
					fStartY = float.Parse(sParts[OFFSET + 3]);

					iWidth = int.Parse(sParts[OFFSET + 4]);
					iHeight = int.Parse(sParts[OFFSET + 5]);

					iAlignX = int.Parse(sParts[OFFSET + 6]);
					iAlignY = int.Parse(sParts[OFFSET + 7]);
					break;
			}

			return this;
		}

		public void copy(LabelEntryBase lebFrom)
		{
			iType = lebFrom.iType;
			iRepeat = lebFrom.iRepeat;
			fRepeatOffset = lebFrom.fRepeatOffset;

			sFontName = String.Copy(lebFrom.sFontName);
			sText = String.Copy(lebFrom.sText);

			iWidth = lebFrom.iWidth;
			iHeight = lebFrom.iHeight;

			iAlignX = lebFrom.iAlignX;
			iAlignY = lebFrom.iAlignY;

			fSize = lebFrom.fSize;
			fStartX = lebFrom.fStartX;
			fStartY = lebFrom.fStartY;
			fEndX = lebFrom.fEndX;
			fEndY = lebFrom.fEndY;
		}

		public void adjust(int iTimes)
		{
			fStartY += (fRepeatOffset * iTimes);
		}

		public LabelEntryBase textToUpper()
		{
			sText = sText.ToUpper();
			return this;
		}
	}
}
