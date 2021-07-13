using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Presentation.Extensions
{
    public static class WebMvcExtensions
    {
        /// <summary>
        /// Gets the bytes from file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static byte[] GetBytesFromFile(this HttpPostedFileBase file)
        {
            MemoryStream target = new MemoryStream();
            file.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            target.Close();
            return data;
        }

        /// <summary>
        /// Remover o input hidden para checkbox booleano na tela de pesquisa
        /// http://stackoverflow.com/a/9666218
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString BasicCheckBoxFor<T>(this HtmlHelper<T> html,
                                                Expression<Func<T, bool>> expression,
                                                object htmlAttributes = null)
        {
            var result = html.CheckBoxFor(expression, htmlAttributes).ToString();
            const string pattern = @"<input name=""[^""]+"" type=""hidden"" value=""false"" />";
            var single = Regex.Replace(result, pattern, "");
            return MvcHtmlString.Create(single);
        }

        /// <summary>
        /// Icons the checkbox.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="val">The value.</param>
        /// <param name="titleTrue">The title true.</param>
        /// <param name="titleFalse">The title false.</param>
        /// <returns></returns>
        public static MvcHtmlString IconCheckbox(this HtmlHelper html, bool? val, string titleTrue = "Sim", string titleFalse = "Não")
        {
            return MvcHtmlString.Create(val.HasValue && val.Value ? "<i class=\"fa fa-lg fa-check-square-o\" title=\"" + titleTrue + "\"></i>" : "<i class=\"fa fa-lg fa-square-o\" title=\"" + titleFalse + "\"></i>");
        }

        /// <summary>
        /// Labels the default for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="width">The width.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString LabelDefaultFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, 
            string width = "",
            object htmlAttributes = null)
        {
            var htmlAttributesDic = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            htmlAttributesDic.Add("class", "control-label "+width);
            
            return html.LabelFor(expression, htmlAttributesDic);
        }

        /// <summary>
        /// Labels the default.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="value">The value.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString LabelDefault(this HtmlHelper html, string value, object htmlAttributes = null)
        {
            var htmlAttributesDic = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            htmlAttributesDic.Add("class", "label");
            return html.Label(value, htmlAttributesDic);
        }

        /// <summary>
        /// Texts the box disabled if for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="disabled">if set to <c>true</c> [disabled].</param>
        /// <param name="format">The format.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString TextBoxDisabledIfFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool disabled, string format = null, object htmlAttributes = null)
        {
            var htmlAttributesDic = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            if (disabled && !htmlAttributesDic.Keys.Contains("disabled"))
            {
                htmlAttributesDic.Add("disabled", "disabled");
            }
            return htmlHelper.TextBoxFor(expression, format, htmlAttributesDic);
        }

        /// <summary>
        /// Texts the box disabled.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="value">The value.</param>
        /// <param name="disabled">if set to <c>true</c> [disabled].</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString TextBoxDisabled(this HtmlHelper htmlHelper, string value, bool disabled = true, object htmlAttributes = null)
        {
            var htmlAttributesDic = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            if (disabled && !htmlAttributesDic.Keys.Contains("disabled"))
            {
                htmlAttributesDic.Add("disabled", "disabled");
                htmlAttributesDic.Add("class", "form-control");
            }
            return htmlHelper.TextBox(name: "#", value: value, htmlAttributes: htmlAttributesDic);
        }

        /// <summary>
        /// Sets the defaults attrs form.
        /// </summary>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private static RouteValueDictionary _setDefaultsAttrsForm(object htmlAttributes, string name, string id)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            attrs.Add("class", "form-horizontal");
            attrs.Add("id", id);
            attrs.Add("name", name);
            return attrs;
        }

        /// <summary>
        /// Begins the crud form.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="idModel">The identifier model.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcForm BeginCrudForm(this HtmlHelper htmlHelper, long idModel, string id = null, string name = null, string action = null, string controller = null,
            object routeValues = null, object htmlAttributes = null)
        {
            var attrs = _setDefaultsAttrsForm(htmlAttributes, name ?? "frmAddEdit", id ?? "frmAddEdit");
            return htmlHelper.BeginForm(
                action ?? (idModel > 0 ? "update" : "create"),
                controller ?? (htmlHelper.CurrentController()),
                new RouteValueDictionary(routeValues),
                FormMethod.Post,
                attrs);
        }

        /// <summary>
        /// Begins the filter form.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcForm BeginFilterForm(this HtmlHelper htmlHelper, string id = null, string name = null, string action = null, string controller = null,
            object routeValues = null, object htmlAttributes = null)
        {
            var attrs = _setDefaultsAttrsForm(htmlAttributes, name ?? "frmFilter", id ?? "frmFilter");
            return htmlHelper.BeginForm(
                action ?? "filter",
                controller ?? (htmlHelper.CurrentController()),
                new RouteValueDictionary(routeValues),
                FormMethod.Get,
                attrs);
        }

        /// <summary>
        /// Begins the details form.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="idModel">The identifier model.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcForm BeginDetailsForm(this HtmlHelper htmlHelper, long idModel, string id = null, string name = null, string action = null, string controller = null,
            object routeValues = null, object htmlAttributes = null)
        {
            var attrs = _setDefaultsAttrsForm(htmlAttributes, name ?? "frmDetails", id ?? "frmDetails");

            var routeValsDic = new RouteValueDictionary(routeValues);
            routeValsDic.Add("id", idModel);

            return htmlHelper.BeginForm(
                action ?? "Details",
                controller ?? (htmlHelper.CurrentController()),
                routeValsDic,
                FormMethod.Get,
                attrs);
        }

        /// <summary>
        /// Subs the header.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="text">The text.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString SubHeader(this HtmlHelper html, string text, object htmlAttributes = null)
        {
            var builder = new TagBuilder("h5");
            builder.InnerHtml = text;
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            var css = "sub-header";
            if (attrs.Keys.Contains("class"))
            {
                css = css + " " + attrs.Where(m => m.Key == "class").SingleOrDefault().Value;
                attrs.Remove("class");
            }
            attrs.Add("class", css);
            builder.MergeAttributes(attrs);
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// Subs the header first.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="text">The text.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString SubHeaderFirst(this HtmlHelper html, string text, object htmlAttributes = null)
        {
            var builder = new TagBuilder("h5");
            builder.InnerHtml = text;
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            var css = "sub-header margin-top-0";
            if (attrs.Keys.Contains("class"))
            {
                css = css + " " + attrs.Where(m => m.Key == "class").SingleOrDefault().Value;
                attrs.Remove("class");
            }
            attrs.Add("class", css);
            builder.MergeAttributes(attrs);
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// Gets the order mode.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="orderName">Name of the order.</param>
        /// <param name="orderNameSelected">The order name selected.</param>
        /// <param name="orderModeSelected">The order mode selected.</param>
        /// <returns></returns>
        public static string GetOrderMode(this HtmlHelper helper, string orderName, string orderNameSelected, string orderModeSelected)
        {
            if (orderName == orderNameSelected)
            {
                return orderModeSelected == "asc" ? "desc" : "asc";
            }
            return "asc";
        }

        /// <summary>
        /// Gets the order mode icon.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="orderName">Name of the order.</param>
        /// <param name="orderNameSelected">The order name selected.</param>
        /// <param name="orderModeSelected">The order mode selected.</param>
        /// <returns></returns>
        public static string GetOrderModeIcon(this HtmlHelper helper, string orderName, string orderNameSelected, string orderModeSelected)
        {
            if (orderName == orderNameSelected)
            {
                return orderModeSelected == "asc" ? "ti-arrow-up" : "ti-arrow-down";
            }
            return "ti-arrows-vertical order-inactive";
        }

        /// <summary>
        /// Selectizes the drop down list for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TPropertyId">The type of the property identifier.</typeparam>
        /// <typeparam name="TPropertyLabel">The type of the property label.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expressionForId">The expression for identifier.</param>
        /// <param name="expressionForLabel">The expression for label.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString SelectizeDropDownListFor<TModel, TPropertyId, TPropertyLabel>(
                this HtmlHelper<TModel> htmlHelper,
                Expression<Func<TModel, TPropertyId>> expressionForId,
                Expression<Func<TModel, TPropertyLabel>> expressionForLabel,
                object htmlAttributes = null
            )
        {
            var model = htmlHelper.ViewData.Model;
            var id = expressionForId.Compile()(model);
            var selectList = new Dictionary<string, string>();

            if (id != null)
            {
                var label = expressionForLabel.Compile()(model);

                selectList.Add(id.ToString(), label.ToString());
            }

            return htmlHelper.DropDownListFor(
                    expressionForId,
                    new SelectList(selectList, "key", "value"),
                    htmlAttributes
                );

        }

        /// <summary>
        /// Currents the controller.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static string CurrentController(this HtmlHelper htmlHelper)
        {
            return
                htmlHelper.ViewContext
                    .Controller.ControllerContext
                    .RequestContext
                    .RouteData.Values["controller"].ToString();
        }
    }
}