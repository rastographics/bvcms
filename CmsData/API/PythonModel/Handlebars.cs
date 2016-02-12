using HandlebarsDotNet;
using IronPython.Modules;
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
                var a = s.SplitStr(" ", 2);
                writer.Write(a[n]);
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
            var eq = args[0] == args[1];
            if (args[0] == null ^ args[1] == null)
                eq = false;
            else if (!eq && args[0] is int)
                eq = args[0].ToString() == args[1]?.ToString();
            return eq;
        }
        public string RenderTemplate(string source)
        {
            return RenderTemplate(source, Data);
        }

        public string RenderTemplate(string source, object data)
        {
            RegisterHelpers(db);
            var template = Handlebars.Compile(source);
            var result = template(data);
            return result;
        }
    }
}