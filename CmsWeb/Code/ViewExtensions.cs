/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Collections;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;
using System.Configuration;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace CmsWeb
{
    public enum ListType
    {
        Ordered,
        Unordered,
        TableCell
    }
    public static class ViewExtensions2
    {
        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var viewData = new ViewDataDictionary(helper.ViewData) { TemplateInfo = new TemplateInfo { HtmlFieldPrefix = name } };
            return helper.Partial(partialViewName, model, viewData);
        }
        public static string GetNameFor<M, P>(this M model, Expression<Func<M, P>> ex)
        {
            return ExpressionHelper.GetExpressionText(ex);
        }
        public static string RegisterScript(this HtmlHelper helper, string scriptFileName)
        {
            string scriptRoot = VirtualPathUtility.ToAbsolute("~/Scripts");
            string scriptFormat = "<script src=\"{0}/{1}\" type=\"text/javascript\"></script>\r\n";
            return string.Format(scriptFormat, scriptRoot, scriptFileName);

        }
        public static string ToFormattedList(this IEnumerable list, ListType listType)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerator en = list.GetEnumerator();

            string outerListFormat = "";
            string listFormat = "";

            switch (listType)
            {
                case ListType.Ordered:
                    outerListFormat = "<ol>{0}</ol>";
                    listFormat = "<li>{0}</li>";
                    break;
                case ListType.Unordered:
                    outerListFormat = "<ul>{0}</ul>";
                    listFormat = "<li>{0}</li>";
                    break;
                case ListType.TableCell:
                    outerListFormat = "{0}";
                    listFormat = "<td>{0}</td>";
                    break;
                default:
                    break;
            }
            return string.Format(outerListFormat, ToFormattedList(list, listFormat));
        }
        public static string ToFormattedList(IEnumerable list, string format)
        {
            var sb = new StringBuilder();
            foreach (object item in list)
                sb.AppendFormat(format, item.ToString());
            return sb.ToString();
        }

        public static string GetSiteUrl(this ViewPage pg)
        {
            string Port = pg.ViewContext.HttpContext.Request.ServerVariables["SERVER_PORT"];
            if (Port == null || Port == "80" || Port == "443")
                Port = "";
            else
                Port = ":" + Port;

            string Protocol = pg.ViewContext.HttpContext.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (Protocol == null || Protocol == "0")
                Protocol = "http://";
            else
                Protocol = "http://";

            string appPath = pg.ViewContext.HttpContext.Request.ApplicationPath;
            if (appPath == "/")
                appPath = "";

            string sOut = Protocol + pg.ViewContext.HttpContext.Request.ServerVariables["SERVER_NAME"] + Port + appPath;
            return sOut;
        }
        public static HtmlString PageSizesDropDown(this HtmlHelper helper, string id, string onchange)
        {
            var tb = new TagBuilder("select");
            tb.MergeAttribute("id", id);
            if (onchange.HasValue())
                tb.MergeAttribute("onchange", onchange);
            var sb = new StringBuilder();
            foreach (var o in PageSizes(null))
            {
                var ot = new TagBuilder("option");
                ot.MergeAttribute("value", o.Value);
                if (o.Selected)
                    ot.MergeAttribute("selected", "selected");
                ot.SetInnerText(o.Text);
                sb.Append(ot.ToString());
            }
            tb.InnerHtml = sb.ToString();
            return new HtmlString(tb.ToString());
        }
        public static IEnumerable<SelectListItem> PageSizes(this HtmlHelper helper)
        {
            var sizes = new int[] { 10, 25, 50, 75, 100, 200 };
            var list = new List<SelectListItem>();
            foreach (var size in sizes)
                list.Add(new SelectListItem { Text = size.ToString() });
            return list;
        }
        public static HtmlString SpanIf(this HtmlHelper helper, bool condition, string text, object htmlAttributes)
        {
            if (!condition)
                return null;
            var tb = new TagBuilder("span");
            var attr = new RouteValueDictionary(htmlAttributes);
            tb.InnerHtml = text;
            tb.MergeAttributes<string, object>(attr);
            return new HtmlString(tb.ToString());
        }
        public static HtmlString Span(this HtmlHelper helper, string text, object htmlAttributes)
        {
            var tb = new TagBuilder("span");
            var attr = new RouteValueDictionary(htmlAttributes);
            tb.InnerHtml = text;
            tb.MergeAttributes<string, object>(attr);
            return new HtmlString(tb.ToString());
        }
        private static string TryGetModel(this HtmlHelper helper, string name)
        {
            ModelState val;
            helper.ViewData.ModelState.TryGetValue(name, out val);
            string s = null;
            if (val != null)
                s = val.Value.AttemptedValue;
            return s;
        }
        public static HtmlString DropDownList2(this HtmlHelper helper, string name, IEnumerable<SelectListItem> list, bool visible)
        {
            var tb = new TagBuilder("select");
            tb.MergeAttribute("id", name);
            tb.MergeAttribute("name", name);
            if (!visible)
                tb.MergeAttribute("style", "display: none");
            var s = helper.TryGetModel(name);
            var sb = new StringBuilder();
            foreach (var o in list)
            {
                var ot = new TagBuilder("option");
                ot.MergeAttribute("value", o.Value);
                bool selected = false;
                if (s.HasValue())
                    selected = s == o.Value;
                else if (o.Selected)
                    selected = true;
                if (selected)
                    ot.MergeAttribute("selected", "selected");
                ot.SetInnerText(o.Text);
                sb.Append(ot.ToString());
            }
            tb.InnerHtml = sb.ToString();
            return new HtmlString(tb.ToString());
        }
        public static HtmlString DropDownList3(this HtmlHelper helper, string id, string name, IEnumerable<SelectListItem> list, string value)
        {
            var tb = new TagBuilder("select");
            if (id.HasValue())
                tb.MergeAttribute("id", id);
            tb.MergeAttribute("name", name);
            var sb = new StringBuilder();
            foreach (var o in list)
            {
                var ot = new TagBuilder("option");
                ot.MergeAttribute("value", o.Value);
                if (value == o.Value)
                    ot.MergeAttribute("selected", "selected");
                ot.SetInnerText(o.Text);
                sb.Append(ot.ToString());
            }
            tb.InnerHtml = sb.ToString();
            return new HtmlString(tb.ToString());
        }
        public static HtmlString DropDownList4(this HtmlHelper helper, string id, string name, IEnumerable<CmsWeb.Models.OnlineRegPersonModel.SelectListItemFilled> list, string value, string cssClass = "")
        {
            var tb = new TagBuilder("select");
            if (id.HasValue())
                tb.MergeAttribute("id", id);
            tb.MergeAttribute("name", name);
            if (cssClass.HasValue())
                tb.MergeAttribute("class", cssClass);
            var sb = new StringBuilder();
            foreach (var o in list)
            {
                var ot = new TagBuilder("option");
                ot.MergeAttribute("value", o.Value);
                if (value == o.Value)
                    ot.MergeAttribute("selected", "selected");
                //				if (o.Filled)
                //					ot.MergeAttribute("disabled", "disabled");
                ot.SetInnerText(o.Text);
                sb.Append(ot.ToString());
            }
            tb.InnerHtml = sb.ToString();
            return new HtmlString(tb.ToString());
        }
        public static HtmlString TextBox2(this HtmlHelper helper, string name, bool visible)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("type", "text");
            tb.MergeAttribute("id", name);
            tb.MergeAttribute("name", name);
            if (!visible)
                tb.MergeAttribute("style", "display: none");
            var s = helper.TryGetModel(name);
            var viewDataValue = Convert.ToString(helper.ViewData.Eval(name));
            tb.MergeAttribute("value", s ?? viewDataValue);
            return new HtmlString(tb.ToString());
        }
        public static HtmlString TextBox3(this HtmlHelper helper, string id, string name, string value)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("type", "text");
            tb.MergeAttribute("id", id);
            tb.MergeAttribute("name", name);
            tb.MergeAttribute("value", value);
            return new HtmlString(tb.ToString());
        }
        public static HtmlString TextBox3(this HtmlHelper helper, string id, string name, string value, object htmlAttributes)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("type", "text");
            tb.MergeAttribute("id", id);
            tb.MergeAttribute("name", name);
            tb.MergeAttribute("value", value);
            var attr = new RouteValueDictionary(htmlAttributes);
            tb.MergeAttributes<string, object>(attr);
            ModelState state;
            if (helper.ViewData.ModelState.TryGetValue(name, out state) && (state.Errors.Count > 0))
                tb.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            return new HtmlString(tb.ToString());
        }
        public static HtmlString TextBoxClass(this HtmlHelper helper, string name, string @class)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("type", "text");
            tb.MergeAttribute("id", name);
            tb.MergeAttribute("name", name);
            tb.MergeAttribute("class", @class);
            var s = helper.TryGetModel(name);
            var viewDataValue = Convert.ToString(helper.ViewData.Eval(name));
            tb.MergeAttribute("value", s ?? viewDataValue);
            return new HtmlString(tb.ToString());
        }
        public static HtmlString HiddenFor2<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("type", "hidden");
            var name = ExpressionHelper.GetExpressionText(expression);
            var v = htmlHelper.ViewData.Eval(name);
            var prefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (prefix.HasValue())
                name = prefix + "." + name;
            tb.MergeAttribute("name", name);
            if (v != null)
                tb.MergeAttribute("value", v.ToString());
            else
                tb.MergeAttribute("value", "");
            return new HtmlString(tb.ToString());
        }
        public static HtmlString DatePicker(this HtmlHelper helper, string name)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("type", "text");
            tb.MergeAttribute("id", name.Replace('.', '_'));
            tb.MergeAttribute("name", name);
            tb.MergeAttribute("class", "datepicker");
            var s = helper.TryGetModel(name);
            var viewDataValue = (DateTime?)helper.ViewData.Eval(name);
            tb.MergeAttribute("value", viewDataValue.FormatDate());
            return new HtmlString(tb.ToString());
        }
        public static HtmlString CheckBoxReadonly(this HtmlHelper helper, bool? ck)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("type", "checkbox");
            tb.MergeAttribute("disabled", "disabled");
            if (ck == true)
                tb.MergeAttribute("checked", "checked");
            return new HtmlString(tb.ToString());
        }
        public static HtmlString CodeDesc(this HtmlHelper helper, string name, IEnumerable<SelectListItem> list)
        {
            var tb = new TagBuilder("span");
            var viewDataValue = helper.ViewData.Eval(name);
            var i = (int?)viewDataValue ?? 0;

            var si = list.SingleOrDefault(v => v.Value == i.ToString());
            if (si != null)
                tb.InnerHtml = si.Text;
            else
                tb.InnerHtml = "?";
            return new HtmlString(tb.ToString());
        }
        public static HtmlString Hidden3(this HtmlHelper helper, string id, string name, object value)
        {
            var tb = new TagBuilder("input");
            if (id.HasValue())
                tb.MergeAttribute("id", id);
            tb.MergeAttribute("type", "hidden");
            tb.MergeAttribute("name", name);
            tb.MergeAttribute("value", value != null ? value.ToString() : "");
            return new HtmlString(tb.ToString());
        }
        public static HtmlString Hidden3(this HtmlHelper helper, string name, object value)
        {
            return helper.Hidden3(null, name, value);
        }
        public static HtmlString HiddenIf(this HtmlHelper helper, string name, bool? include)
        {
            if (include == true)
            {
                var tb = new TagBuilder("input");
                tb.MergeAttribute("type", "hidden");
                tb.MergeAttribute("id", name);
                tb.MergeAttribute("name", name);
                var viewDataValue = helper.ViewData.Eval(name);
                tb.MergeAttribute("value", viewDataValue.ToString());
                return new HtmlString(tb.ToString());
            }
            return new HtmlString("");
        }
        public static HtmlString IsRequired(this HtmlHelper helper, bool? Required)
        {
            //var tb = new TagBuilder("img");
            //tb.MergeAttribute("border", "0");
            //tb.MergeAttribute("width", "11");
            //tb.MergeAttribute("height", "12");
            //if ((Required ?? true) == true)
            //{
            //    tb.MergeAttribute("src", "/images/req.gif");
            //    tb.MergeAttribute("alt", "req");
            //    return tb.ToString();
            //}
            //tb.MergeAttribute("src", "/images/notreq.gif");
            //tb.MergeAttribute("alt", "not req");
            var tb = new TagBuilder("span");
            tb.MergeAttribute("class", "asterisk");
            if ((Required ?? true) == true)
            {
                tb.InnerHtml = "*";
                return new HtmlString(tb.ToString());
            }
            tb.InnerHtml = "&nbsp;";
            return new HtmlString(tb.ToString());
        }
        public static HtmlString Required(this HtmlHelper helper)
        {
            return helper.IsRequired(true);
        }
        public static HtmlString NotRequired(this HtmlHelper helper)
        {
            return helper.IsRequired(false);
        }
        public static HtmlString HiddenIf(this HtmlHelper helper, string name, object value, bool? include)
        {
            if (include == true)
            {
                var tb = new TagBuilder("input");
                tb.MergeAttribute("type", "hidden");
                tb.MergeAttribute("id", name);
                tb.MergeAttribute("name", name);
                tb.MergeAttribute("value", value.ToString());
                return new HtmlString(tb.ToString());
            }
            return new HtmlString("");
        }
        public static HtmlString ValidationMessage2(this HtmlHelper helper, string name)
        {
            var m = helper.ViewData.ModelState[name];
            if (m == null || m.Errors.Count == 0)
                return new HtmlString("");
            var e = m.Errors[0].ErrorMessage;
            var b = new TagBuilder("span");
            b.AddCssClass(HtmlHelper.ValidationMessageCssClassName);
            b.SetInnerText(e);
            return new HtmlString(b.ToString());
        }
        public static string Json(this HtmlHelper html, string variableName, object model)
        {
            TagBuilder tag = new TagBuilder("script");
            tag.Attributes.Add("type", "text/javascript");
            var jsonSerializer = new JavaScriptSerializer();
            tag.InnerHtml = "var " + variableName + " = " + jsonSerializer.Serialize(model) + ";";
            return tag.ToString();
        }
        public static string NameFor2<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }

        public class HelpMessage
        {
            public HtmlString message;
            public string errorClass;
        }

        public static HelpMessage HelpMessageFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string classname)
        {
            var name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            var help = helper.ViewContext.ViewData["help"] as string;
            var m = helper.ViewData.ModelState[name];
            if (m == null && help == null)
                return new HelpMessage();
            var b = new TagBuilder("span");
            b.AddCssClass(classname);
            var hasError = false;
            if (m != null && m.Errors.Count > 0)
            {
                b.SetInnerText(m.Errors[0].ErrorMessage);
                hasError = true;
            }
            else if (help != null)
                b.InnerHtml = help;
            else
                return new HelpMessage();
            return new HelpMessage { message = new HtmlString(b.ToString()), errorClass = hasError ? "error" : "" };
        }

        public static string RenderPartialViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            try
            {
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                    var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static string RenderPartialViewToString2(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static CollectionItemNamePrefixScope BeginCollectionItem<TModel>(this HtmlHelper<TModel> html, string collectionName)
        {
            string itemIndex = GetCollectionItemIndex(collectionName);
            var collectionItemName = String.Format("{0}[{1}]", collectionName, itemIndex);

            var indexField = new TagBuilder("input");
            indexField.MergeAttributes(new Dictionary<string, string>() 
            {
                { "name", String.Format("{0}.Index", collectionName) },
                { "value", itemIndex },
                { "type", "hidden" },
                { "autocomplete", "off" }
            });

            return new CollectionItemNamePrefixScope(
                html.ViewData.TemplateInfo,
                collectionItemName,
                indexField.ToString(TagRenderMode.SelfClosing));
        }
        private static string GetCollectionItemIndex(string collectionIndexFieldName)
        {
            Queue<string> previousIndices = (Queue<string>)HttpContext.Current.Items[collectionIndexFieldName];
            if (previousIndices == null)
            {
                HttpContext.Current.Items[collectionIndexFieldName] = previousIndices = new Queue<string>();

                var previousIndicesValues = HttpContext.Current.Request[collectionIndexFieldName];
                if (!string.IsNullOrWhiteSpace(previousIndicesValues))
                {
                    foreach (var index in previousIndicesValues.Split(','))
                        previousIndices.Enqueue(index);
                }
            }

            return previousIndices.Count > 0 ? previousIndices.Dequeue() : Guid.NewGuid().ToString();
        }

        public class CollectionItemNamePrefixScope : IDisposable
        {
            private readonly TemplateInfo _templateInfo;
            private readonly string _previousPrefix;
            public string hiddenindex { get; set; }
            public string SuitableId
            {
                get { return _templateInfo.HtmlFieldPrefix.ToSuitableId(); }
            }
            public string CollectionName
            {
                get { return _templateInfo.HtmlFieldPrefix; }
            }

            public CollectionItemNamePrefixScope(TemplateInfo templateInfo, string collectionItemName, string hiddenindex)
            {
                this._templateInfo = templateInfo;

                _previousPrefix = templateInfo.HtmlFieldPrefix;
                templateInfo.HtmlFieldPrefix = collectionItemName;
                this.hiddenindex = hiddenindex;
            }

            public void Dispose()
            {
                _templateInfo.HtmlFieldPrefix = _previousPrefix;
            }
        }
    }
}