using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public static void RegisterHelpers(CMSDataContext db)
        {
            Handlebars.RegisterHelper("BottomBorder", (writer, context, args) => { writer.Write(CssStyle.BottomBorder); });
            Handlebars.RegisterHelper("AlignTop", (writer, context, args) => { writer.Write(CssStyle.AlignTop); });
            Handlebars.RegisterHelper("AlignRight", (writer, context, args) => { writer.Write(CssStyle.AlignRight); });
            Handlebars.RegisterHelper("DataLabelStyle", (writer, context, args) => { writer.Write(CssStyle.DataLabelStyle); });
            Handlebars.RegisterHelper("LabelStyle", (writer, context, args) => { writer.Write(CssStyle.LabelStyle); });
            Handlebars.RegisterHelper("DataStyle", (writer, context, args) => { writer.Write(CssStyle.DataStyle); });

            Handlebars.RegisterHelper("ServerLink", (writer, context, args) => { writer.Write(db.ServerLink().TrimEnd('/')); });
            Handlebars.RegisterHelper("FmtZip", (writer, context, args) => { writer.Write(args[0].ToString().FmtZip()); });
            Handlebars.RegisterHelper("IfEqual", (writer, options, context, args) =>
            {
                if (IsEqual(args))
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            Handlebars.RegisterHelper("IfNotEqual", (writer, options, context, args) =>
            {
                if (!IsEqual(args))
                    options.Template(writer, (object)context);
                else
                    options.Inverse(writer, (object)context);
            });
            Handlebars.RegisterHelper("GetToken", (writer, context, args) =>
            {
                var s = args[0].ToString();
                var n = args[1].ToInt();
                var ntoks = args.Length > 2 ? args[2].ToInt() : 2;
                var sep = args.Length > 3 ? args[3].ToString() : " ";
                var a = s.SplitStr(sep, ntoks);
                writer.Write(a[n].trim());
            });

            // Format helper in form of:  {{Fmt value "fmt"}}
            // ex. {{Fmt Total "C"}}
            // fmt is required. Uses standard/custom dotnet format strings
            Handlebars.RegisterHelper("Fmt", (writer, context, args) =>
            {
                var fmt = $"{{0:{args[1]}}}";
                writer.Write(fmt, args[0]);
            });

            // FmtPhone helper in form of:  {{FmtPhone phone# "prefix"}}
            Handlebars.RegisterHelper("FmtPhone", (writer, context, args) => { writer.Write(args[0].ToString().FmtFone($"{args[1]}")); });
        }

        private static bool IsEqual(object[] args)
        {
            // use the XOR operator: true if one arg is null and the other is not
            if (args[0] == null ^ args[1] == null)
                return false;
            // at this point, either both are null or both are not null
            if (args[0] == null)
                return true;  // both must be null 
            // at this point both are not null
            var eq = args[0].Equals(args[1]);
            if (!eq && args[0] is int)
                eq = args[0].ToString() == args[1]?.ToString();
            return eq;
        }
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