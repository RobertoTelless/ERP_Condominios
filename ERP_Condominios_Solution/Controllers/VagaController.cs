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

namespace ERP_Condominios_Solution.Controllers
{
    public class VagaController : Controller
    {
        private readonly IVagaAppService baseApp;
        private readonly ILogAppService logApp;

        private String msg;
        private Exception exception;
        VAGA objeto = new VAGA();
        VAGA objetoAntes = new VAGA();
        List<VAGA> listaMaster = new List<VAGA>();
        String extensao;

        public VagaController(IVagaAppService baseApps, ILogAppService logApps)
        {
            baseApp = baseApps; ;
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
        public ActionResult MontarTelaVaga()
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
                    Session["MensVaga"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<UNIDADE>)Session["ListaVaga"] == null)
            {
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    listaMaster = baseApp.GetAllItens(idAss).Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
                }
                else
                {
                    listaMaster = baseApp.GetAllItens(idAss);
                }
                Session["ListaVaga"] = listaMaster;
                Session["FiltroVaga"] = null;
            }
            ViewBag.Listas = (List<VAGA>)Session["ListaVaga"];
            ViewBag.Title = "Vagas";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVA_CD_ID", "TIVA_NM_NOME");
            ViewBag.Unids = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");

            // Indicadores
            ViewBag.Vagas = ((List<VAGA>)Session["ListaVaga"]).Count;

            // Mensagem
            if ((Int32)Session["MensVaga"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensVaga"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensVaga"] == 3)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0021", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 4)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0022", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensVaga"] = 0;
            Session["VoltaVaga"] = 1;
            objeto = new VAGA();
            return View(objeto);
        }

        public ActionResult RetirarFiltroVaga()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaVaga"] = null;
            Session["FiltroVaga"] = null;
            listaMaster = new List<VAGA>();
            return RedirectToAction("MontarTelaVaga");
        }

        public ActionResult MostrarTudoVaga()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaVaga"] = listaMaster;
            return RedirectToAction("MontarTelaVaga");
        }

        [HttpPost]
        public ActionResult FiltrarVaga(VAGA item)
        {
            try
            {
                try
                {
                    // Executa a operação
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Login", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    List<VAGA> listaObj = new List<VAGA>();
                    Int32 volta = baseApp.ExecuteFilter(item.VAGA_NR_NUMERO, item.VAGA_NR_ANDAR, item.UNID_CD_ID, item.TIVA_CD_ID, idAss, out listaObj);
                    Session["FiltroVaga"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensVaga"] = 1;
                        ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        return RedirectToAction("MontarTelaVaga");
                    }

                    // Sucesso
                    Session["MensVaga"] = 0;
                    listaMaster = listaObj;
                    Session["ListaVaga"] = listaObj;
                    return RedirectToAction("MontarTelaVaga");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaVaga");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaVaga");
            }
        }

        public ActionResult VoltarBaseVaga()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaVaga");
        }

        [HttpGet]
        public ActionResult IncluirVaga()
        {
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
                    Session["MensVaga"] = 2;
                    return RedirectToAction("MontarTelaVaga", "Unidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVA_CD_ID", "TIVA_NM_NOME");

            // Prepara view
            VAGA item = new VAGA();
            VagaViewModel vm = Mapper.Map<VAGA, VagaViewModel>(item);
            vm.VAGA_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirVaga(VagaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVA_CD_ID", "TIVA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    VAGA item = Mapper.Map<VagaViewModel, VAGA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensVaga"] = 3;
                        return RedirectToAction("MontarTelaVaga", "Vaga");
                    }

                    // Sucesso
                    listaMaster = new List<VAGA>();
                    Session["ListaVaga"] = null;
                    Session["VoltaVaga"] = 1;
                    Session["IdVagaVolta"] = item.UNID_CD_ID;
                    Session["Vaga"] = item;
                    Session["MensVaga"] = 0;
                    return RedirectToAction("MontarTelaVaga");
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
        public ActionResult EditarVaga(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensVaga"] = 2;
                    return RedirectToAction("MontarTelaVaga", "Vaga");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVA_CD_ID", "TIVA_NM_NOME");
            ViewBag.Unids = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");

            VAGA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Vaga"] = item;
            Session["IdVolta"] = id;
            Session["IdVaga"] = id;
            VagaViewModel vm = Mapper.Map<VAGA, VagaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarVaga(VagaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVA_CD_ID", "TIVA_NM_NOME");
            ViewBag.Unids = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    VAGA item = Mapper.Map<VagaViewModel, VAGA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<VAGA>();
                    Session["ListaVaga"] = null;
                    Session["MensVaga"] = 0;
                    return RedirectToAction("MontarTelaVaga");
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
        public ActionResult VerVaga(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensVaga"] = 2;
                    return RedirectToAction("MontarTelaVaga", "Vaga");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            VAGA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Vaga"] = item;
            Session["IdVolta"] = id;
            Session["IdVaga"] = id;
            VagaViewModel vm = Mapper.Map<VAGA, VagaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirVaga(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensVaga"] = 2;
                    return RedirectToAction("MontarTelaVaga", "Vaga");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            VAGA item = baseApp.GetItemById(id);
            objetoAntes = (VAGA)Session["Vaga"];
            item.VAGA_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensVaga"] = 4;
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0022", CultureInfo.CurrentCulture));
                return RedirectToAction("MontarTelaVaga");
            }
            listaMaster = new List<VAGA>();
            Session["ListaVaga"] = null;
            Session["FiltroVaga"] = null;
            return RedirectToAction("MontarTelaVaga");
        }

        [HttpGet]
        public ActionResult ReativarVaga(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensVaga"] = 2;
                    return RedirectToAction("MontarTelaVaga", "Vaga");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            VAGA item = baseApp.GetItemById(id);
            objetoAntes = (VAGA)Session["Vaga"];
            item.VAGA_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<VAGA>();
            Session["ListaVaga"] = null;
            Session["FiltroVaga"] = null;
            return RedirectToAction("MontarTelaVaga");
        }

        [HttpGet]
        public ActionResult GerarNotificacaoVaga()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensVaga"] = 2;
                    return RedirectToAction("MontarTelaVaga", "Vaga");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 vaga = (Int32)Session["IdVaga"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss).ToList();

            // Prepara view
            ViewBag.Cats = new SelectList(baseApp.GetAllCatNotificacao(idAss), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usuarios = new SelectList(lista, "USUA_CD_ID", "USUA_NM_NOME");

            NOTIFICACAO item = new NOTIFICACAO();
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            vm.NOTI_DT_EMISSAO = DateTime.Today.Date;
            vm.ASSI_CD_ID = idAss;
            vm.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTI_IN_ATIVO = 1;
            vm.NOTI_IN_NIVEL = 1;
            vm.NOTI_IN_ORIGEM = 1;
            vm.NOTI_IN_STATUS = 1;
            vm.NOTI_IN_VISTA = 0;
            vm.NOTI_NM_TITULO = "Notificação para Morador / Vaga";
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoVaga(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 unidade = (Int32)Session["IdUnidade"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss).ToList();
            ViewBag.Cats = new SelectList(baseApp.GetAllCatNotificacao(idAss), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usuarios = new SelectList(lista, "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    Int32 volta = baseApp.GerarNotificacao(item, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<VAGA>();
                    return RedirectToAction("VoltarBaseVaga");
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