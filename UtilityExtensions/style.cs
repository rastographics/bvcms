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
        public string gr = "color:#AAAAAA;";
        public string sm = "font-size:12px;";
        public string ff = "font-family:Arial,Helvetica;";
        public string lg = "font-size:15px;";
        public string bd = "font-weight:bold;";
        public string pb = "padding-bottom:6px;";
        public string tp = "vertical-align: text-top;";
        public string rt = "text-align:right;padding-right:5px;";
        public string bb = "border-bottom: solid 1px #D8D8D8";
        public string lb => ff + sm + gr + rt + tp;
        public string dd => ff + lg + bd;
        public string dl => ff + lg + gr;
    }
}