using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using HandlebarsDotNet;

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

        public static void RegisterHelpers()
        {
            Handlebars.RegisterHelper("BottomBorder", (writer, context, args) => { writer.Write(BottomBorder); });
            Handlebars.RegisterHelper("AlignTop", (writer, context, args) => { writer.Write(AlignTop); });
            Handlebars.RegisterHelper("AlignRight", (writer, context, args) => { writer.Write(AlignRight); });
            Handlebars.RegisterHelper("DataLabelStyle", (writer, context, args) => { writer.Write(DataLabelStyle); });
            Handlebars.RegisterHelper("LabelStyle", (writer, context, args) => { writer.Write(LabelStyle); });
            Handlebars.RegisterHelper("DataStyle", (writer, context, args) => { writer.Write(DataStyle); });
            Handlebars.RegisterHelper("ServerLink", (writer, context, args) => { writer.Write(DbUtil.Db.ServerLink().TrimEnd('/')); });
        }

    }
}