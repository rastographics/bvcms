using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsData
{
    public static class CssStyle
    {
        public static string Light = "color:#AAAAAA;";
        public static string Small = "font-size:12px;";
        public static string SansSerif = "font-family:Arial,Helvetica;";
        public static string Medium = "font-size:15px;";
        public static string Bold = "font-weight:bold;";
        public static string PadBottom = "padding-bottom:6px;";
        public static string AlignTop = "vertical-align: text-top;";
        public static string AlignRight = "text-align:right;padding-right:5px;";
        public static string BottomBorder = "border-bottom: solid 1px #D8D8D8";
        public static string LabelStyle => SansSerif + Small + Light + AlignRight + AlignTop;
        public static string DataStyle => SansSerif + Medium + Bold;
        public static string DataLabelStyle => SansSerif + Medium + Light;
    }
}