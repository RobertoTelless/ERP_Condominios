using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;

namespace ERP_Condominios_Solution.Controllers
{
    public class LogController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly ILogAppService logApp;


        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public LogController(IUsuarioAppService baseApps, ILogAppService logApps)
        {
            baseApp = baseApps;
            logApp = logApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");

        }

        [HttpGet]
        public ActionResult MontarTelaLog()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensLog"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            ViewBag.Usuarios = new SelectList(baseApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            if (Session["ListaLog"] == null || ((List<LOG>)Session["ListaLog"]).Count == 0)
            {
                listaMasterLog = logApp.GetAllItensDataCorrente(idAss);
                Session["ListaLog"] = listaMasterLog;
            }
            ViewBag.Listas = (List<LOG>)Session["ListaLog"];

            if (Session["ListaLog"] == null || ((List<LOG>)Session["ListaLog"]).Count == 0)
            {
                ModelState.AddModelError("", "Nenhum log na data corrente");
            }

            ViewBag.Logs = ((List<LOG>)Session["ListaLog"]).Count;
            ViewBag.LogsDataCorrente = logApp.GetAllItensDataCorrente(idAss).Count;
            ViewBag.LogsMesCorrente = logApp.GetAllItensMesCorrente(idAss).Count;
            ViewBag.LogsMesAnterior = logApp.GetAllItensMesAnterior(idAss).Count;
            ViewBag.Title = "Auditoria";

            if ((Int32)Session["MensLog"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                Session["MensLog"] = 0;
            }

            // Abre view
            Session["MensLog"] = 0;
            objLog = new LOG();
            objLog.LOG_DT_DATA = DateTime.Today;
            return View(objLog);
        }

        public ActionResult RetirarFiltroLog()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaLog"] = null;
            Session["FiltroLog"] = null;
            return RedirectToAction("MontarTelaLog");
        }

        [HttpPost]
        public ActionResult FiltrarLog(LOG item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOG> listaObj = new List<LOG>();
                Session["FiltroLog"] = item;
                Int32 volta = logApp.ExecuteFilter(item.USUA_CD_ID, item.LOG_DT_DATA, item.LOG_NM_OPERACAO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    Session["MensLog"] = 1;
                    return RedirectToAction("MontarTelaLog");
                }

                // Sucesso
                listaMasterLog = listaObj;
                Session["ListaLog"] = listaMasterLog;
                Session["MensLog"] = 0;
                return RedirectToAction("MontarTelaLog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaLog");
            }
        }

        [HttpGet]
        public ActionResult VerLog(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            LOG item = logApp.GetById(id);
            objLogAntes = item;
            LogViewModel vm = Mapper.Map<LOG, LogViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarBaseLog()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaLog");
        }

        public ActionResult VoltarLog()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMasterLog = new List<LOG>();
            Session["ListaLog"] = null;
            Session["FiltroLog"] = null;
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaLogGerencia()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensLog"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            ViewBag.Usuarios = new SelectList(baseApp.GetAllUsuarios(idAss), "USUA_CD_ID", "USUA_NM_EMAIL");
            if (Session["ListaLog"] == null)
            {
                listaMasterLog = logApp.GetAllItens(idAss);
                Session["ListaLog"] = listaMasterLog;
            }
            ViewBag.Listas = (List<LOG>)Session["ListaLog"];
            ViewBag.Logs = ((List<LOG>)Session["ListaLog"]).Count;
            ViewBag.Title = "Auditoria";

            // Abre view
            objLog = new LOG();
            objLog.LOG_DT_DATA = DateTime.Today;
            return View();
        }

    }
}