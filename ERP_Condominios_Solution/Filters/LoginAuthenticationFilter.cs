using EntitiesServices.Work_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace SystemBRPresentation.Filters
{
    public class LoginAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        private String[] allowedProfiles { get; set; }

        public LoginAuthenticationFilter(String[] profiles)
        {
            allowedProfiles = profiles;
        }

        void IAuthenticationFilter.OnAuthentication(AuthenticationContext filterContext)
        {
            if (SessionMocks.UserCredentials == null)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else if (!allowedProfiles.Contains(SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA))
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Perfil não possui acesso há página");
            }
            else if (filterContext.ActionDescriptor.ActionName.Contains("Editar") && filterContext.Controller.ValueProvider.GetValue("id") == null)
            {
                filterContext.Result = new HttpStatusCodeResult(500, "Parametro nao fornecido");
            }
        }

        void IAuthenticationFilter.OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "ControleAcesso" },
                        { "action", "Login" }
                    });
            }
            else if (filterContext.Result is HttpStatusCodeResult && (filterContext.Result as HttpStatusCodeResult).StatusCode == 403)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "BaseAdmin" },
                        { "action", "CarregarBase" }
                    });
            }
            else if (filterContext.Result is HttpStatusCodeResult && (filterContext.Result as HttpStatusCodeResult).StatusCode == 500)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", filterContext.RouteData.Values["Controller"].ToString() },
                        { "action", filterContext.HttpContext.Request.UrlReferrer.Segments[2].ToString() }
                    });
            }
        }
    }
}