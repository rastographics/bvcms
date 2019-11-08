using System;
using System.Web;
using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public static IHandlebars RegisterHelpers(CMSDataContext db, PythonModel pm = null, IHandlebars handlebars = null)
        {
            handlebars = handlebars ?? Handlebars.Create();
            handlebars.RegisterHelper("BottomBorder", (writer, context, args) => { writer.Write(CssStyle.BottomBorder); });
            handlebars.RegisterHelper("AlignTop", (writer, context, args) => { writer.Write(CssStyle.AlignTop); });
            handlebars.RegisterHelper("AlignRight", (writer, context, args) => { writer.Write(CssStyle.AlignRight); });
            handlebars.RegisterHelper("DataLabelStyle", (writer, context, args) => { writer.Write(CssStyle.DataLabelStyle); });
            handlebars.RegisterHelper("LabelStyle", (writer, context, args) => { writer.Write(CssStyle.LabelStyle); });
            handlebars.RegisterHelper("DataStyle", (writer, context, args) => { writer.Write(CssStyle.DataStyle); });

            handlebars.RegisterHelper("ServerLink", (writer, context, args) => { writer.Write(db.ServerLink().TrimEnd('/')); });
            handlebars.RegisterHelper("FmtZip", (writer, context, args) => { writer.Write(args[0].ToString().FmtZip()); });
            handlebars.RegisterHelper("HtmlComment", (writer, context, args) =>
            {
#if DEBUG
                writer.Write($"<h6>{args[0].ToString()} {args[1].ToString()}</h6>");
#else
                writer.Write($"<!--{args[0].ToString()} {args[1].ToString()}-->");
#endif
            });
            handlebars.RegisterHelper("IfEqual", (writer, options, context, args) =>
            {
                if (IsEqual(args))
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            handlebars.RegisterHelper("IfNotEqual", (writer, options, context, args) =>
            {
                if (!IsEqual(args))
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            handlebars.RegisterHelper("IfCond", (writer, options, context, args) =>
            {
                var op = HttpUtility.HtmlDecode(args[1].ToString());
                bool b = false;
                switch (op)
                {
                    case "==":
                        b = Compare(args) == 0;
                        break;
                    case "!=":
                        b = Compare(args) != 0;
                        break;
                    case "<":
                        b = Compare(args) < 0;
                        break;
                    case ">":
                        b = Compare(args) > 0;
                        break;
                    case ">=":
                        b = Compare(args) >= 0;
                        break;
                    case "<=":
                        b = Compare(args) <= 0;
                        break;
                    case "&&":
                        b = NumTrue(args) == 2;
                        break;
                    case "||":
                        b = NumTrue(args) >= 1;
                        break;
                }
                if (b)
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            handlebars.RegisterHelper("IfGT", (writer, options, context, args) =>
            {
                if (Compare2(args) > 0)
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            handlebars.RegisterHelper("IfLT", (writer, options, context, args) =>
            {
                if (Compare2(args) < 0)
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            handlebars.RegisterHelper("IfGE", (writer, options, context, args) =>
            {
                if (Compare2(args) <= 0)
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            handlebars.RegisterHelper("IfLE", (writer, options, context, args) =>
            {
                if (Compare2(args) <= 0)
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            handlebars.RegisterHelper("GetToken", (writer, context, args) =>
            {
                var s = args[0].ToString();
                var n = args[1].ToInt();
                var ntoks = args.Length > 2 ? args[2].ToInt() : 2;
                var sep = args.Length > 3 ? args[3].ToString() : " ";
                var a = s.SplitStr(sep, ntoks);
                writer.Write(a[n]?.Trim() ?? "");
            });

            handlebars.RegisterHelper("FmtMDY", (writer, context, args) =>
            {
                var s = args[0].ToString();
                if(DateTime.TryParse(s, out DateTime dt))
                    writer.Write(dt.ToShortDateString());
            });
            handlebars.RegisterHelper("FmtDate", (writer, context, args) =>
            {
                var s = args[0].ToString();
                if(DateTime.TryParse(s, out DateTime dt))
                    writer.Write(dt.ToShortDateString());
            });
            handlebars.RegisterHelper("FmtMoney", (writer, context, args) =>
            {
                var s = args[0].ToString();
                if(decimal.TryParse(s, out decimal d))
                    writer.Write(d.ToString("C"));
            });

            // Format helper in form of:  {{Fmt value "fmt"}}
            // ex. {{Fmt Total "C"}}
            // fmt is required. Uses standard/custom dotnet format strings
            handlebars.RegisterHelper("Fmt", (writer, context, args) =>
            {
                var fmt = $"{{0:{args[1]}}}";
                writer.Write(fmt, args[0]);
            });

            // FmtPhone helper in form of:  {{FmtPhone phone# "prefix"}}
            handlebars.RegisterHelper("FmtPhone", (writer, context, args) => { writer.Write(args[0].ToString().FmtFone($"{args[1]}")); });

            handlebars.RegisterHelper("ReplaceCode", (writer, context, args) =>
            {
                EmailReplacements r = context.Replacements as EmailReplacements
                    ?? (context.Replacements = new EmailReplacements(db));
                var code = args[0].ToString();
                var p = db.LoadPersonById(args[1].ToInt());
                int? oid = null;
                if (args.Length == 3)
                    oid = args[2].ToInt2();
                writer.Write(r.RenderCode(code, p, oid));
            });
            handlebars.RegisterHelper("Json", (writer, options, context, args) =>
            {
                dynamic a = JsonDeserialize2(args[0].ToString());
                foreach (var item in a)
                {
                    options.Template(writer, item);
                }
            });

            handlebars.RegisterHelper("Calc", (writer, context, args) =>
            {
                var calcAmt = args[0].ToDouble() - args[1].ToDouble();
                var calcAmtfmt = $"{{0:{'c'}}}";
                writer.Write(calcAmtfmt, calcAmt);
            });

            handlebars.RegisterHelper("ThrowError", (writer, context, args) =>
            {
                throw new Exception("ThrowError called in Handlebars Helper");
            });

            return handlebars;
        }

        private static bool IsEqual(object[] args)
        {
            // if length == 3 then n = 2 and the operator is 1
            // if length == 2 then n = 1 and the operator is implied
            var n2 = args.Length - 1;
            // use the XOR operator: true if one arg is null and the other is not
            if (args[0] == null ^ args[n2] == null)
                return false;
            // at this point, either both are null or both are not null
            if (args[0] == null)
                return true;  // both must be null 
            // at this point both are not null
            var eq = args[0].Equals(args[n2]);
            if (!eq && args[0] is int)
                eq = args[0].ToString() == args[n2]?.ToString();
            return eq;
        }
        private static int? Compare(object[] args)
        {
            var a1 = args[0];
            var a2 = args[2];
            if (a1 is int)
                a2 = args[2].ToInt();
            if (a1 is decimal)
                a2 = args[2].ToNullableDecimal() ?? 0m;

            var a1C = a1 as IComparable;
            var a2C = a2 as IComparable;
            if (a1C == null || a2C == null)
                return null;
            return a1C.CompareTo(a2C);
        }
        private static int? Compare2(object[] args)
        {
            var a1 = args[0];
            var a2 = args[1];
            if (a1 is int)
                a2 = args[1].ToInt();
            if (a1 is decimal)
                a2 = args[1].ToNullableDecimal() ?? 0m;

            var a1C = a1 as IComparable;
            var a2C = a2 as IComparable;
            if (a1C == null || a2C == null)
                return null;
            return a1C.CompareTo(a2C);
        }
        private static int NumTrue(object[] args)
        {
            return (IsTrue(args[0]) ? 1 : 0)
                + (IsTrue(args[2]) ? 1 : 0);
        }
        private static bool IsTrue(object arg)
        {
            if (arg == null)
                return false;
            if (arg is int && (int)arg != 0)
                return true;
            if (arg is decimal && (decimal)arg != 0)
                return true;
            if (arg.ToString().Length > 0)
                return true;
            return false;
        }

        public EmailReplacements Replacements { get; set; }

        public string RenderTemplate(string source)
        {
            return RenderTemplate(source, Data);
        }

        public string RenderTemplate(string source, object data)
        {
            return db.RenderTemplate(source, data);
        }
    }
}
