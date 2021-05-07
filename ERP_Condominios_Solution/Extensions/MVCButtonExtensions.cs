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
    public static class MVCButtonExtensions
    {
        /// <summary>
        /// Currents the controller.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        internal static string _currentController(this HtmlHelper html)
        {
            return (html.ViewContext.RequestContext.RouteData.Values["controller"] ?? string.Empty).ToString();
        }

        /// <summary>
        /// Currents the action.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        internal static string _currentAction(this HtmlHelper html)
        {
            return (html.ViewContext.RequestContext.RouteData.Values["action"] ?? string.Empty).ToString();
        }

        /// <summary>
        /// Buttons the index of the link.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="text">The text.</param>
        /// <param name="action">The action.</param>
        /// <param name="buttonClass">The button class.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <param name="clearReturnUrl">if set to <c>true</c> [clear return URL].</param>
        /// <returns></returns>
        public static MvcHtmlString ButtonLinkIndex(this HtmlHelper html, string text = "Voltar", string action = "Index", string controller = "Home", string buttonClass = "warning",
            object routeValues = null, object htmlAttributes = null, bool clearReturnUrl = true)
        {
            if (String.IsNullOrEmpty(controller))
            {
                controller = html._currentController();
            }
            var retUrl = HttpContext.Current.Session[string.Format("{0}_{1}", "ReturnUrl", controller).ToLower()];
            if (retUrl != null)
            {
                var builder = new TagBuilder("a");
                var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
                builder.InnerHtml = text;

                attrs.Add("class", "btn btn-sm btn-" + buttonClass);
                attrs.Add("href", retUrl);

                builder.MergeAttributes(attrs);
                if(clearReturnUrl)
                {
                    HttpContext.Current.Session[string.Format("{0}_{1}", "ReturnUrl", controller).ToLower()] = null;
                }
                return MvcHtmlString.Create(builder.ToString());
            }
            return html.ButtonLink(action, controller, routeValues, htmlAttributes, buttonClass, text);
        }

        /// <summary>
        /// Buttons the link create.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="text">The text.</param>
        /// <param name="action">The action.</param>
        /// <param name="buttonClass">The button class.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString ButtonLinkCreate(this HtmlHelper html, string text = "Novo", string action = "create", string buttonClass = "success",
            object routeValues = null, object htmlAttributes = null)
        {
            return html.ButtonLink(action, html._currentController(), routeValues, htmlAttributes, buttonClass, text);
        }

        /// <summary>
        /// Buttons the link.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="action">The action.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <param name="buttonClass">The button class.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper html, string action, string controller, object routeValues, object htmlAttributes, string buttonClass, string text)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            attrs.Add("class", "btn btn-sm btn-" + buttonClass);
            return html.ActionLink(text, action, controller, new RouteValueDictionary(routeValues), attrs);
        }

        /// <summary>
        /// Gets the route values table link.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        private static object _getRouteValuesTableLink(long id, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl)) { return new { id = id, returnUrl = returnUrl }; }
            else { return new { id = id }; }
        }

        /// <summary>
        /// Tables the link update.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="action">The action.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="title">The title.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public static MvcHtmlString TableLinkUpdate(this HtmlHelper html, long id, string action = "Details", string icon = "fa-edit", string title = "Consultar", string returnUrl = null)
        {
            return html.TableLinkAction(action: action, controller: html._currentController(),
                routeValues: _getRouteValuesTableLink(id, returnUrl), icon: icon, title: title);

        }

        /// <summary>
        /// Tables the link details.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="action">The action.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="title">The title.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public static MvcHtmlString TableLinkDetails(this HtmlHelper html, long id, string action = "Details", string icon = "fa-file-o", string title = "Detalhes", string returnUrl = null)
        {
            return html.TableLinkAction(action: action, controller: html._currentController(),
                routeValues: _getRouteValuesTableLink(id, returnUrl), icon: icon, title: title);
        }

        /// <summary>
        /// Tables the link action.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="action">The action.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="title">The title.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static MvcHtmlString TableLinkAction(this HtmlHelper html, string action, string controller, object routeValues, string icon, string title, string target = "_self")
        {
            return html.ActionLink(" ", action, controller, routeValues, new { @class = "tbl-link fa-lg fa " + icon, @alt = title, @title = title, @target = target });
        }

        /// <summary>
        /// Buttons the action submit.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <param name="buttonClass">The button class.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString ButtonActionSubmit(this HtmlHelper html, string text = "Salvar", string type = "submit", string buttonClass = "success", object htmlAttributes = null)
        {
            return html.ButtonAction(htmlAttributes, buttonClass, type, text);
        }

        /// <summary>
        /// Buttons the action filter.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <param name="buttonClass">The button class.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString ButtonActionFilter(this HtmlHelper html, string text = "Filtrar", string type = "submit", string buttonClass = "info", object htmlAttributes = null)
        {
            return html.ButtonAction(htmlAttributes, buttonClass, type, text);
        }

        /// <summary>
        /// Buttons the action.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <param name="buttonClass">The button class.</param>
        /// <param name="type">The type.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static MvcHtmlString ButtonAction(this HtmlHelper html, object htmlAttributes, string buttonClass, string type, string text)
        {
            var builder = new TagBuilder("button");
            builder.InnerHtml = text;

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) ?? new RouteValueDictionary();
            attrs.Add("class", "btn btn-sm btn-" + buttonClass);
            attrs.Add("type", type);

            builder.MergeAttributes(attrs);
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}