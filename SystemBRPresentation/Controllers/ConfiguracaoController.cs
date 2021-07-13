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
    public class ConfiguracaoController : Controller
    {
        private readonly IConfiguracaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private String msg;
        private Exception exception;
        CONFIGURACAO objeto = new CONFIGURACAO();
        CONFIGURACAO objetoAntes = new CONFIGURACAO();
        List<CONFIGURACAO> listaMaster = new List<CONFIGURACAO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();

        public ConfiguracaoController(IConfiguracaoAppService baseApps, IUsuarioAppService usuApps)
        {
            baseApp = baseApps;
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

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult GetConfiguracao()
        {
            var config = baseApp.GetItemById(SessionMocks.IdAssinante);
            var serialConfig = new CONFIGURACAO
            {
                CONF_CD_ID = config.CONF_CD_ID,
                ASSI_CD_ID = config.ASSI_CD_ID,
                CONF_NR_FALHAS_DIA = config.CONF_NR_FALHAS_DIA,
                CONF_NM_HOST_SMTP = config.CONF_NM_HOST_SMTP,
                CONF_NM_PORTA_SMTP = config.CONF_NM_PORTA_SMTP,
                CONF_NM_EMAIL_EMISSOO = config.CONF_NM_EMAIL_EMISSOO,
                CONF_NM_SENHA_EMISSOR = config.CONF_NM_SENHA_EMISSOR,
                CONF_NR_DIAS_CP = config.CONF_NR_DIAS_CP,
                CONF_NR_DIAS_PATRIMONIO = config.CONF_NR_DIAS_PATRIMONIO,
                CONF_NR_DIAS_ATENDIMENTO = config.CONF_NR_DIAS_ATENDIMENTO,
                CONF_NR_REFRESH_DASH = config.CONF_NR_REFRESH_DASH,
                CONF_NM_ARQUIVO_ALARME = config.CONF_NM_ARQUIVO_ALARME,
                CONF_NR_REFRESH_NOTIFICACAO = config.CONF_NR_REFRESH_NOTIFICACAO,
                CONF_NM_SENDGRID_KEY = config.CONF_NM_SENDGRID_KEY,
                CONF_NM_NOME_EMPRESA = config.CONF_NM_NOME_EMPRESA,
                CONF_SG_LOGIN_SMS = config.CONF_SG_LOGIN_SMS,
                CONF_SG_SENHA_SMS = config.CONF_SG_SENHA_SMS,
                CONF_IN_TOAST_TAREFAS = config.CONF_IN_TOAST_TAREFAS,
                CONF_IN_TOAST_AGENDAS = config.CONF_IN_TOAST_AGENDAS,
                CONF_IN_PERMITE_DUPLO_CLIENTE = config.CONF_IN_PERMITE_DUPLO_CLIENTE,
                CONF_IN_PERMITE_DUPLO_FORNECEDOR = config.CONF_IN_PERMITE_DUPLO_FORNECEDOR,
                CONF_IN_LINHAS_GRID = config.CONF_IN_LINHAS_GRID

            };

            return Json(serialConfig);
        }

        [HttpGet]
        public ActionResult MontarTelaConfiguracao()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensConfiguracao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            objeto = baseApp.GetItemById(SessionMocks.IdAssinante);
            Session["Configuracao"] = objeto;

            ViewBag.Listas = (CONFIGURACAO)Session["Configuracao"];
            ViewBag.Title = "Configuracao";
            var listaGrid = new List<SelectListItem>();
            listaGrid.Add(new SelectListItem() { Text = "10", Value = "10" });
            listaGrid.Add(new SelectListItem() { Text = "25", Value = "25" });
            listaGrid.Add(new SelectListItem() { Text = "50", Value = "50" });
            listaGrid.Add(new SelectListItem() { Text = "100", Value = "100" });
            ViewBag.ListaGrid = new SelectList(listaGrid, "Value", "Text");

            // Indicadores

            // Mensagem

            // Abre view
            Session["MensConfiguracao"] = 0;
            objetoAntes = objeto;
            if (objeto.CONF_NR_FALHAS_DIA == null)
            {
                objeto.CONF_NR_FALHAS_DIA = 3;
            }
            if (objeto.CONF_NR_DIAS_CP == null)
            {
                objeto.CONF_NR_DIAS_CP = 2;
            }
            if (objeto.CONF_NR_DIAS_ATENDIMENTO == null)
            {
                objeto.CONF_NR_DIAS_ATENDIMENTO = 2;
            }
            if (objeto.CONF_IN_PERMITE_DUPLO_CLIENTE == null)
            {
                objeto.CONF_IN_PERMITE_DUPLO_CLIENTE = 0;
            }
            if (objeto.CONF_IN_PERMITE_DUPLO_FORNECEDOR == null)
            {
                objeto.CONF_IN_PERMITE_DUPLO_FORNECEDOR = 0;
            }
            if (objeto.CONF_IN_LINHAS_GRID == null)
            {
                objeto.CONF_IN_LINHAS_GRID = 25;
            }
            if (objeto.CONF_IN_TOAST_TAREFAS == null)
            {
                objeto.CONF_IN_TOAST_TAREFAS = 1;
            }
            if (objeto.CONF_IN_TOAST_AGENDAS == null)
            {
                objeto.CONF_IN_TOAST_AGENDAS = 1;
            }
            Session["Configuracao"] = objeto;
            Session["IdVolta"] = 1;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(objeto);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaConfiguracao(ConfiguracaoViewModel vm)
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
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["Configuracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
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

        public ActionResult VoltarBaseConfiguracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaConfiguracao");
        }

        [HttpGet]
        public ActionResult EditarConfiguracao(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONFIGURACAO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Configuracao"] = item;
            Session["IdVolta"] = id;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarConfiguracao(ConfiguracaoViewModel vm)
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
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
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

    }
}