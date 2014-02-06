namespace CmsCheckin.Classes
{
	class LabelEntry : LabelEntryBase
	{
		public LabelEntry() { }

		public void fill(LabelInfo liItem)
		{
			switch (iType)
			{
				case TYPE_STRING:
					if( liItem.GetType().GetProperty(sText) != null )
						sText = liItem.GetType().GetProperty(sText).GetValue(liItem, null).ToString();
					else
						sText = "";
					break;

				case TYPE_BARCODE:
					if (liItem.GetType().GetProperty(sText) != null)
						sText = liItem.GetType().GetProperty(sText).GetValue(liItem, null).ToString();
					else
						sText = "";
					break;
			}
		}
	}
}