using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsCheckin.Classes
{
    class LabelSet
    {
        public List<LabelPage> lPages = new List<LabelPage>();
        public List<LabelEntryBase> lFormats;

        public LabelSet() { }

        public int getCount() { return lPages.Count(); }

        public void addBlank()
        {
            LabelPage lpNew = new LabelPage();
            lPages.Add(lpNew);
        }

        public void addPages(string sLabelFormat, List<LabelInfo> lItems)
        {
            lFormats = new List<LabelEntryBase>();

            string[] sTextList = sLabelFormat.Split(new char[] { '~' });
            foreach (string sItem in sTextList)
            {
                if (sItem.Length > 0) lFormats.Add(LabelEntryBase.create(sItem));
            }

            if (lItems == null)
            {
                LabelPage lpNew = new LabelPage();
                lpNew.populate(lFormats);
                lPages.Add(lpNew);
            }
            else
            {
                int iMaxRepeat = getMaxRepeat(lFormats);

                if (lItems.Count() == 0 || iMaxRepeat == 0) return;

                int iCount = (int)Math.Ceiling((decimal)lItems.Count() / iMaxRepeat);

                for (int iX = 0; iX < iCount; iX++)
                {
                    LabelPage lpNew = new LabelPage();
                    lpNew.populate(lFormats, lItems, iX * iMaxRepeat);
                    lPages.Add(lpNew);
                }
            }
        }

        public int getMaxRepeat(List<LabelEntryBase> lebList)
        {
            int iMax = 0;

            foreach (LabelEntryBase lebItem in lebList)
            {
                if (lebItem.iRepeat > iMax) iMax = lebItem.iRepeat;
            }

            return iMax;
        }
    }
}
