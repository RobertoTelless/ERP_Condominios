using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SystemBRPresentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using SystemBRPresentation.ViewModels;
using System.IO;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class TemplateController : Controller
    {
        private readonly ITemplateAppService baseApp;
        private readonly ILogAppService logApp;
        private String msg;
        private Exception exception;
        TEMPLATE objeto = new TEMPLATE();
        TEMPLATE objetoAntes = new TEMPLATE();
        List<TEMPLATE> listaMaster = new List<TEMPLATE>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public TemplateController(ITemplateAppService baseApps, ILogAppService logApps)
        {
            baseApp = baseApps;
            logApp = logApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            TEMPLATE item = new TEMPLATE();
            TemplateViewModel vm = Mapper.Map<TEMPLATE, TemplateViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            
            listaMasterLog = new List<LOG>();
            listaMaster = new List<TEMPLATE>();
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaTemplate()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "CarregarDashboardInicial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaTemplate == null)
            {
                listaMaster = baseApp.GetAllItens();
                SessionMocks.listaTemplate = listaMaster;
            }
            ViewBag.Listas = SessionMocks.listaTemplate;
            ViewBag.Title = "Templates";

            // Indicadores
            ViewBag.Templates = SessionMocks.listaTemplate.Count;

            // Abre view
            objeto = new TEMPLATE();
            return View(objeto);
        }

        public ActionResult RetirarFiltroTemplate()
        {
            
            SessionMocks.listaTemplate = null;
            return RedirectToAction("MontarTelaTemplate");
        }

        public ActionResult MostrarTudoTemplate()
        {
            
            listaMaster = baseApp.GetAllItensAdm();
            SessionMocks.listaTemplate = listaMaster;
            return RedirectToAction("MontarTelaTemplate");
        }

        [HttpPost]
        public ActionResult FiltrarTemplate(TEMPLATE item)
        {
            
            try
            {
                // Executa a operação
                List<TEMPLATE> listaObj = new List<TEMPLATE>();
                Int32 volta = baseApp.ExecuteFilter(item.TEMP_SG_SIGLA, item.TEMP_NM_NOME, item.TEMP_TX_CONTEUDO_LIMPO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0017", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMaster = listaObj;
                SessionMocks.listaTemplate = listaObj;
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
            
            return RedirectToAction("MontarTelaTemplate");
        }

        [HttpGet]
        public ActionResult IncluirTemplate()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "CarregarDashboardInicial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            TEMPLATE item = new TEMPLATE();
            TemplateViewModel vm = Mapper.Map<TEMPLATE, TemplateViewModel>(item);
            vm.TEMP_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirTemplate(TemplateViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TEMPLATE item = Mapper.Map<TemplateViewModel, TEMPLATE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMaster = new List<TEMPLATE>();
                    SessionMocks.listaTemplate = null;
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
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "CarregarDashboardInicial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            TEMPLATE item = baseApp.GetItemById(id);
            objetoAntes = item;
            SessionMocks.template = item;
            SessionMocks.idVolta = id;
            TemplateViewModel vm = Mapper.Map<TEMPLATE, TemplateViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditarTemplate(TemplateViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    TEMPLATE item = Mapper.Map<TemplateViewModel, TEMPLATE>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<TEMPLATE>();
                    SessionMocks.listaTemplate = null;
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
        public ActionResult ExcluirTemplate(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "CarregarDashboardInicial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            USUARIO usu = SessionMocks.UserCredentials;
            TEMPLATE item = baseApp.GetItemById(id);
            objetoAntes = SessionMocks.template;
            item.TEMP_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
            }
            listaMaster = new List<TEMPLATE>();
            SessionMocks.listaTemplate = null;
            return RedirectToAction("MontarTelaTemplate");
        }

        [HttpGet]
        public ActionResult ReativarTemplate(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "CarregarDashboardInicial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            USUARIO usu = SessionMocks.UserCredentials;
            TEMPLATE item = baseApp.GetItemById(id);
            objetoAntes = SessionMocks.template;
            item.TEMP_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usu);
            listaMaster = new List<TEMPLATE>();
            SessionMocks.listaTemplate = null;
            return RedirectToAction("MontarTelaTemplate");
        }
    }
}