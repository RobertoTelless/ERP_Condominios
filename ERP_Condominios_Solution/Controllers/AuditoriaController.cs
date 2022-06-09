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
    public class AuditoriaController : Controller
    {
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        LOG objeto = new LOG();
        LOG objetoAntes = new LOG();
        List<LOG> listaMaster = new List<LOG>();
        String extensao;

        public AuditoriaController(ILogAppService logApps, IUsuarioAppService usuApps)
        {
            logApp = logApps;
            usuApp = usuApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if ((List<LOG>)Session["ListaLog"] == null)
            {
                listaMaster = logApp.GetAllItens(idAss);
                Session["ListaLog"] = listaMaster;
                Session["FiltroLog"] = null;
            }
            ViewBag.Listas = (List<LOG>)Session["ListaLog"];
            ViewBag.Logs = ((List<LOG>)Session["ListaLog"]).Count;
            ViewBag.LogsDataCorrente = logApp.GetAllItensDataCorrente(idAss).Count;
            ViewBag.LogsMesCorrente = logApp.GetAllItensMesCorrente(idAss).Count;
            ViewBag.LogsMesAnterior = logApp.GetAllItensMesAnterior(idAss).Count;

            // Abre view
            objeto = new LOG();
            objeto.LOG_DT_DATA = DateTime.Today.Date;
            return View(objeto);
        }

        public ActionResult RetirarFiltroLog()
        {
            Session["ListaLog"] = null;
            Session["FiltroLog"] = null;
            return RedirectToAction("MontarTelaLog");
        }

        [HttpPost]
        public ActionResult FiltrarLog(LOG item)
        {
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
                    return RedirectToAction("MontarTelaLog");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaLog"] = listaObj;
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

            // Prepara view
            LOG item = logApp.GetById(id);
            LogViewModel vm = Mapper.Map<LOG, LogViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarBaseLog()
        {

            return RedirectToAction("MontarTelaLog");
        }
    }
}