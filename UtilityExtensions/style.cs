using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace UtilityExtensions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Sty
    {
        public string Light = "color:#AAAAAA;";
        public string Small = "font-size:12px;";
        public string SansSerif = "font-family:Arial,Helvetica;";
        public string Medium = "font-size:15px;";
        public string Bold = "font-weight:bold;";
        public string PadBottom = "padding-bottom:6px;";
        public string AlignTop = "vertical-align: text-top;";
        public string AlignRight = "text-align:right;padding-right:5px;";
        public string BottomBorder = "border-bottom: solid 1px #D8D8D8";
        public string LabelStyle => SansSerif + Small + Light + AlignRight + AlignTop;
        public string DataStyle => SansSerif + Medium + Bold;
        public string DataLabelStyle => SansSerif + Medium + Light;
    }
}