using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using EntitiesServices.WorkClasses;

namespace ERP_Condominios_Solution.Controllers
{
    public class EncomendaController : Controller
    {
        private readonly IEncomendaAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUnidadeAppService uniApp;
        private String msg;
        private Exception exception;
        ENCOMENDA objetoForn = new ENCOMENDA();
        ENCOMENDA objetoFornAntes = new ENCOMENDA();
        List<ENCOMENDA> listaMasterForn = new List<ENCOMENDA>();
        String extensao;

        public EncomendaController(IEncomendaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IUnidadeAppService uniApps)
        {
            fornApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            uniApp = uniApps;
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
        public ActionResult MontarTelaEncomenda()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaEncomenda"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaEncomenda"] = listaMasterForn;
            }
            ViewBag.Listas = (List<ENCOMENDA>)Session["ListaEncomenda"];
            ViewBag.Title = "Encomendas";
            ViewBag.Formas = new SelectList(fornApp.GetAllFormas(idAss).OrderBy(x => x.FOEN_NM_NOME), "FOEN_CD_ID", "FOEN_NM_NOME");
            ViewBag.Tipos = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIEN_NM_NOME), "TIEN_CD_ID", "TIEN_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Recebida", Value = "1" });
            status.Add(new SelectListItem() { Text = "Entregue", Value = "2" });
            status.Add(new SelectListItem() { Text = "Recusada", Value = "3" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            Session["IncluirEnc"] = 0;

            // Indicadores
            ViewBag.Encomendas = ((List<ENCOMENDA>)Session["ListaEncomenda"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensEncomenda"] != null)
            {
                // Mensagem
                //if ((Int32)Session["MensOcorrencia"] == 1)
                //{
                //    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                //}
                if ((Int32)Session["MensEncomenda"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new ENCOMENDA();
            objetoForn.ENCO_IN_ATIVO = 1;
            objetoForn.ENCO_DT_CHEGADA = DateTime.Today.Date;
            Session["MensEncomenda"] = 0;
            Session["VoltaEncomenda"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaEncomenda"] = null;
            Session["FiltroEncomenda"] = null;
            return RedirectToAction("MontarTelaEncomenda");
        }

        public ActionResult MostrarTudoEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroEncomenda"] = null;
            Session["ListaEncomenda"] = listaMasterForn;
            return RedirectToAction("MontarTelaEncomenda");
        }

        [HttpPost]
        public ActionResult FiltrarEncomenda(ENCOMENDA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ENCOMENDA> listaObj = new List<ENCOMENDA>();
                Session["FiltroEncomenda"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.UNID_CD_ID, item.FOEN_CD_ID, item.TIEN_CD_ID, item.ENCO_DT_CHEGADA, item.ENCO_IN_STATUS, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEncomenda"] = 1;
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaEncomenda"] = listaObj;
                return RedirectToAction("MontarTelaEncomenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEncomenda");
            }
        }

        public ActionResult VoltarBaseEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaEncomenda"] == 2)
            {
                return RedirectToAction("MontarTelaMorador", "Morador");
            }
            return RedirectToAction("MontarTelaEncomenda");
        }








    }
}