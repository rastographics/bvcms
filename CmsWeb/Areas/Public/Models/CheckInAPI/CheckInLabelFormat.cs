using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace CmsWeb.CheckInAPI
{
    public class CheckInLabelFormat
    {
        // From database
        [JsonIgnore]
        public string name = "";

        [JsonIgnore]
        public int size = 0;

        [JsonIgnore]
        public string format = "";

        // Common
        public int type = 0;
        public int repeat = 0;
        public decimal offset = 0;
        public string text = "";

        public decimal startX = 0;
        public decimal startY = 0;

        public decimal alignX = 0;
        public decimal alignY = 0;

        // Replacement String: Type 1 & Label Text: Type 4
        public string font = "";
        public int fontSize = 0;

        // Line: Type 2
        public int lineWidth = 0;
        public decimal endX = 0;
        public decimal endY = 0;

        // Barcode: Type 3
        public int width = 0;
        public int height = 0;

        public void parse()
        {
            string[] entries = format.Split(',');

            Int32.TryParse(entries[ENTRY_TYPE], out type);
            Int32.TryParse(entries[ENTRY_REPEAT], out repeat);
            Decimal.TryParse(entries[ENTRY_OFFSET], out offset);

            switch (type)
            {
                case TYPE_REPLACEMENT:
                {
                    font = entries[ENTRY_REPLACE_FONT];
                    text = entries[ENTRY_REPLACE_TEXT];

                    Int32.TryParse(entries[ENTRY_REPLACE_FONT_SIZE], out fontSize);

                    Decimal.TryParse(entries[ENTRY_REPLACE_STARTX], out startX);
                    Decimal.TryParse(entries[ENTRY_REPLACE_STARTY], out startY);

                    Decimal.TryParse(entries[ENTRY_REPLACE_ALIGNX], out alignX);
                    Decimal.TryParse(entries[ENTRY_REPLACE_ALIGNY], out alignY);
                    break;
                }

                case TYPE_LINE:
                {
                    Int32.TryParse(entries[ENTRY_LINE_WIDTH], out lineWidth);

                    Decimal.TryParse(entries[ENTRY_LINE_STARTX], out startX);
                    Decimal.TryParse(entries[ENTRY_LINE_STARTY], out startY);

                    Decimal.TryParse(entries[ENTRY_LINE_ENDX], out endX);
                    Decimal.TryParse(entries[ENTRY_LINE_ENDY], out endY);
                    break;
                }

                case TYPE_BARCODE:
                {
                    text = entries[ENTRY_BARCODE_TEXT];

                    Decimal.TryParse(entries[ENTRY_BARCODE_STARTX], out startX);
                    Decimal.TryParse(entries[ENTRY_BARCODE_STARTY], out startY);

                    Int32.TryParse(entries[ENTRY_BARCODE_WIDTH], out width);
                    Int32.TryParse(entries[ENTRY_BARCODE_HEIGHT], out height);

                    Decimal.TryParse(entries[ENTRY_BARCODE_ALIGNX], out alignX);
                    Decimal.TryParse(entries[ENTRY_BARCODE_ALIGNY], out alignY);
                    break;
                }

                case TYPE_LABEL:
                {
                    font = entries[ENTRY_LABEL_FONT];
                    text = entries[ENTRY_LABEL_TEXT];

                    Int32.TryParse(entries[ENTRY_LABEL_FONT_SIZE], out fontSize);

                    Decimal.TryParse(entries[ENTRY_LABEL_STARTX], out startX);
                    Decimal.TryParse(entries[ENTRY_LABEL_STARTY], out startY);

                    Decimal.TryParse(entries[ENTRY_LABEL_ALIGNX], out alignX);
                    Decimal.TryParse(entries[ENTRY_LABEL_ALIGNY], out alignY);
                    break;
                }
            }
        }

        // Types
        private const int TYPE_REPLACEMENT = 1;
        private const int TYPE_LINE = 2;
        private const int TYPE_BARCODE = 3;
        private const int TYPE_LABEL = 4;

        // Common
        private static int ENTRY_TYPE = 0;
        private static int ENTRY_REPEAT = 1;
        private static int ENTRY_OFFSET = 2;

        // Replacement
        private static int ENTRY_REPLACE_FONT = 3;
        private static int ENTRY_REPLACE_FONT_SIZE = 4;
        private static int ENTRY_REPLACE_TEXT = 5;
        private static int ENTRY_REPLACE_STARTX = 6;
        private static int ENTRY_REPLACE_STARTY = 7;
        private static int ENTRY_REPLACE_ALIGNX = 8;
        private static int ENTRY_REPLACE_ALIGNY = 9;

        // Line
        private static int ENTRY_LINE_WIDTH = 3;
        private static int ENTRY_LINE_STARTX = 4;
        private static int ENTRY_LINE_STARTY = 5;
        private static int ENTRY_LINE_ENDX = 6;
        private static int ENTRY_LINE_ENDY = 7;


        // Barcode
        private static int ENTRY_BARCODE_TEXT = 3;
        private static int ENTRY_BARCODE_STARTX = 4;
        private static int ENTRY_BARCODE_STARTY = 5;
        private static int ENTRY_BARCODE_WIDTH = 6;
        private static int ENTRY_BARCODE_HEIGHT = 7;
        private static int ENTRY_BARCODE_ALIGNX = 8;
        private static int ENTRY_BARCODE_ALIGNY = 9;

        // Label
        private static int ENTRY_LABEL_FONT = 3;
        private static int ENTRY_LABEL_FONT_SIZE = 4;
        private static int ENTRY_LABEL_TEXT = 5;
        private static int ENTRY_LABEL_STARTX = 6;
        private static int ENTRY_LABEL_STARTY = 7;
        private static int ENTRY_LABEL_ALIGNX = 8;
        private static int ENTRY_LABEL_ALIGNY = 9;
    }
}