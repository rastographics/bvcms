using System.Collections.Generic;
using System.Linq;

namespace CmsCheckin.Classes
{
	class LabelPage
	{
		public List<LabelEntry> leEntries = new List<LabelEntry>();

		public LabelPage() { }

		public int populate(List<LabelEntryBase> lList)
		{
			foreach (LabelEntryBase lebItem in lList)
			{
				for (int iX = 0; iX < lebItem.iRepeat; iX++)
				{
					LabelEntry leNew = new LabelEntry();
					leNew.copy(lebItem.textToUpper());
					leNew.adjust(iX);
					leEntries.Add(leNew);
				}
			}

			return 0;
		}

		public void populate(List<LabelEntryBase> lebList, List<LabelInfo> liItem, int iOffset)
		{
			foreach (LabelEntryBase lebItem in lebList)
			{
				if (lebItem.iRepeat == 1)
				{
					LabelEntry leNew = new LabelEntry();
					leNew.copy(lebItem);
					leNew.fill(liItem.ElementAt(iOffset));
					leEntries.Add(leNew);
				}
				else
				{
					for (int iX = iOffset; iX < (lebItem.iRepeat + iOffset) && iX < liItem.Count(); iX++)
					{
						LabelEntry leNew = new LabelEntry();
						leNew.copy(lebItem);
						leNew.adjust(iX - iOffset);
						leNew.fill(liItem.ElementAt(iX));
						leEntries.Add(leNew);
					}
				}
			}

		}
	}
}
