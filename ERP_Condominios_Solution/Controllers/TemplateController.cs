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
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Canducci.Zip;
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ITemplateAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        TEMPLATE objetoForn = new TEMPLATE();
        TEMPLATE objetoFornAntes = new TEMPLATE();
        List<TEMPLATE> listaMasterForn = new List<TEMPLATE>();
        String extensao;

        public TemplateController(ITemplateAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            confApp = confApps;
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

        public ActionResult VoltarGeral()
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
        public ActionResult MontarTelaTemplate()
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if (Session["ListaTemplate"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaTemplate"] = listaMasterForn;
            }
            ViewBag.Listas = (List<TEMPLATE>)Session["ListaTemplate"];
            ViewBag.Title = "Templates";

            // Indicadores
            ViewBag.Templates = ((List<TEMPLATE>)Session["ListaTemplate"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensTemplate"] != null)
            {
                if ((Int32)Session["MensTemplate"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0111", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new TEMPLATE();
            objetoForn.TEMP_IN_ATIVO = 1;
            Session["MensTemplate"] = 0;
            Session["VoltaTemplate"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroTemplate()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaTemplate"] = null;
            Session["FiltroTemplate"] = null;
            return RedirectToAction("MontarTelaTemplate");
        }

        public ActionResult MostrarTudoTemplate()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroTemplate"] = null;
            Session["ListaTemplate"] = listaMasterForn;
            return RedirectToAction("MontarTelaTemplate");
        }

        [HttpPost]
        public ActionResult FiltrarTemplate(TEMPLATE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE> listaObj = new List<TEMPLATE>();
                Session["FiltroTemplate"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.TEMP_SG_SIGLA, item.TEMP_NM_NOME, item.TEMP_TX_CORPO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensTemplate"] = 1;
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaTemplate"] = listaObj;
                return RedirectToAction("MontarTelaTemplate");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaTemplate");
            }
        }

        public ActionResult VoltarBaseTemplate()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaTemplate");
        }

        [HttpGet]
        public ActionResult IncluirTemplate()
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
                    Session["MensTemplate"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            TEMPLATE item = new TEMPLATE();
            TemplateViewModel vm = Mapper.Map<TEMPLATE, TemplateViewModel>(item);
            vm.TEMP_IN_ATIVO = 1;
            vm.TEMP_DT_CRIACAO = DateTime.Today.Date;
            vm.TEMP_IN_EDITAVEL = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplate(TemplateViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TEMPLATE item = Mapper.Map<TemplateViewModel, TEMPLATE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTemplate"] = 3;
                        return RedirectToAction("MontarTelaTemplate", "Template");
                    }

                    // Sucesso
                    listaMasterForn = new List<TEMPLATE>();
                    Session["ListaTemplate"] = null;
                    Session["Templates"] = fornApp.GetAllItens(idAss);
                    Session["IdVolta"] = item.TEMP_CD_ID;
                    return RedirectToAction("MontarTelaTemplate");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTemplate(Int32 id)
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
                    Session["MensTemplate"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            TEMPLATE item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Template"] = item;
            Session["IdVolta"] = id;
            Session["IdTemplate"] = id;
            Session["VoltaCEP"] = 1;
            TemplateViewModel vm = Mapper.Map<TEMPLATE, TemplateViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplate(TemplateViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TEMPLATE item = Mapper.Map<TemplateViewModel, TEMPLATE>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<TEMPLATE>();
                    Session["ListaTemplate"] = null;
                    return RedirectToAction("MontarTelaTemplate");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTemplate(Int32 id)
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            TEMPLATE item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Template"] = item;
            Session["IdVolta"] = id;
            Session["IdTemplate"] = id;
            Session["VoltaCEP"] = 1;
            TemplateViewModel vm = Mapper.Map<TEMPLATE, TemplateViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirTemplate(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" || usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensTemplate"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TEMPLATE item = fornApp.GetItemById(id);
            objetoFornAntes = (TEMPLATE)Session["Template"];
            item.TEMP_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateDelete(item, usuario);
            listaMasterForn = new List<TEMPLATE>();
            Session["ListaTemplate"] = null;
            Session["FiltroTemplate"] = null;
            return RedirectToAction("MontarTelaTemplate");
        }

        [HttpGet]
        public ActionResult ReativarTemplate(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" || usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensTemplate"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TEMPLATE item = fornApp.GetItemById(id);
            objetoFornAntes = (TEMPLATE)Session["Template"];
            item.TEMP_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateReativar(item, usuario);
            listaMasterForn = new List<TEMPLATE>();
            Session["ListaTemplate"] = null;
            Session["FiltroTemplate"] = null;
            return RedirectToAction("MontarTelaTemplate");
        }

    }
}