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
    public class EntradaSaidaController : Controller
    {
        private readonly IEntradaSaidaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        ENTRADA_SAIDA objeto = new ENTRADA_SAIDA();
        ENTRADA_SAIDA objetoAntes = new ENTRADA_SAIDA();
        List<ENTRADA_SAIDA> listaMaster = new List<ENTRADA_SAIDA>();
        String extensao;

        public EntradaSaidaController(IEntradaSaidaAppService baseApps, ILogAppService logApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps; ;
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

        [HttpGet]
        public ActionResult MontarTelaES()
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
                    Session["MensES"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<ENTRADA_SAIDA>)Session["ListaES"] == null)
            {
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    listaMaster = baseApp.GetByUnidade(usuario.UNID_CD_ID.Value);
                    Session["ListaES"] = listaMaster;
                }
                else if (usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMaster = baseApp.GetAllItens(idAss);
                    Session["ListaES"] = listaMaster.Where(p => p.ENSA_DT_ENTRADA == DateTime.Today.Date).ToList();
                }
                Session["FiltroES"] = null;
            }

            ViewBag.Listas = ((List<ENTRADA_SAIDA>)Session["ListaES"]);
            ViewBag.Title = "Entradas de Saídas";
            ViewBag.Autorizacoes = new SelectList(baseApp.GetAllAutorizacoes(idAss), "AUAC_CD_ID", "AUAC_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Completa", Value = "1" });
            status.Add(new SelectListItem() { Text = "Pendente", Value = "2" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Entradas = ((List<ENTRADA_SAIDA>)Session["ListaES"]).Count;

            // Mensagem
            if ((Int32)Session["MensES"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensES"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensES"] = 0;
            Session["VoltaES"] = 1;
            objeto = new ENTRADA_SAIDA();
            objeto.ENSA_DT_ENTRADA = DateTime.Today.Date;
            return View(objeto);
        }

        public ActionResult RetirarFiltroES()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaES"] = null;
            Session["FiltroES"] = null;
            listaMaster = new List<ENTRADA_SAIDA>();
            return RedirectToAction("MontarTelaES");
        }

        public ActionResult MostrarTudoES()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaES"] = listaMaster;
            return RedirectToAction("MontarTelaES");
        }

        public ActionResult MostrarGeralES()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItens(idAss);
            Session["ListaES"] = listaMaster;
            return RedirectToAction("MontarTelaES");
        }

        [HttpPost]
        public ActionResult FiltrarES(ENTRADA_SAIDA item)
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

                    List<ENTRADA_SAIDA> listaObj = new List<ENTRADA_SAIDA>();
                    Int32 volta = baseApp.ExecuteFilter(item.ENSA_NM_NOME, item.ENSA_NR_DOCUMENTO, item.UNID_CD_ID, item.AUAC_CD_ID, item.ENSA_DT_ENTRADA, item.ENSA_DT_SAIDA, item.ENSA_IN_STATUS, idAss, out listaObj);
                    Session["FiltroES"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensES"] = 1;
                        ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        return RedirectToAction("MontarTelaES");
                    }

                    // Sucesso
                    Session["MensES"] = 0;
                    listaMaster = listaObj;
                    Session["ListaES"] = listaObj;
                    return RedirectToAction("MontarTelaES");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaES");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaES");
            }
        }

        public ActionResult VoltarBaseES()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaES"] = null;
            return RedirectToAction("MontarTelaES");
        }

        [HttpGet]
        public ActionResult IncluirES()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensES"] = 2;
                    return RedirectToAction("MontarTelaES", "EntradaSaida");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Graus = new SelectList(baseApp.GetAllGraus(idAss), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Autorizacoes = new SelectList(baseApp.GetAllAutorizacoes(idAss), "AUAC_CD_ID", "AUAC_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            ENTRADA_SAIDA item = new ENTRADA_SAIDA();
            EntradaSaidaViewModel vm = Mapper.Map<ENTRADA_SAIDA, EntradaSaidaViewModel>(item);
            vm.ENSA_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.ENSA_DT_ENTRADA = DateTime.Now;
            vm.ENSA_IN_STATUS = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirES(EntradaSaidaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Graus = new SelectList(baseApp.GetAllGraus(idAss), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Autorizacoes = new SelectList(baseApp.GetAllAutorizacoes(idAss), "AUAC_CD_ID", "AUAC_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ENTRADA_SAIDA item = Mapper.Map<EntradaSaidaViewModel, ENTRADA_SAIDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<ENTRADA_SAIDA>();
                    Session["ListaES"] = null;
                    Session["VoltaES"] = 1;
                    Session["IdESVolta"] = item.ENSA_CD_ID;
                    Session["ES"] = item;
                    Session["MensES"] = 0;

                    // Notificação
                    if (item.UNID_CD_ID != null & item.UNID_CD_ID > 0)
                    {
                        return RedirectToAction("GerarNotificacaoES");
                    }
                    return RedirectToAction("MontarTelaES");
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
        public ActionResult EditarES(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensES"] = 2;
                    return RedirectToAction("MontarTelaES", "EntradaSaida");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Graus = new SelectList(baseApp.GetAllGraus(idAss), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Autorizacoes = new SelectList(baseApp.GetAllAutorizacoes(idAss), "AUAC_CD_ID", "AUAC_NM_NOME");

            ENTRADA_SAIDA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["ES"] = item;
            Session["IdVolta"] = id;
            Session["IdES"] = id;
            Session["VoltaES"] = 2;
            EntradaSaidaViewModel vm = Mapper.Map<ENTRADA_SAIDA, EntradaSaidaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarES(EntradaSaidaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Graus = new SelectList(baseApp.GetAllGraus(idAss), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Autorizacoes = new SelectList(baseApp.GetAllAutorizacoes(idAss), "AUAC_CD_ID", "AUAC_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ENTRADA_SAIDA item = Mapper.Map<EntradaSaidaViewModel, ENTRADA_SAIDA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<ENTRADA_SAIDA>();
                    Session["ListaES"] = null;
                    Session["MensES"] = 0;
                    return RedirectToAction("MontarTelaES");
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
        public ActionResult VerES(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    Session["MensES"] = 2;
                    return RedirectToAction("MontarTelaES", "EntradaSaida");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Unidade = usuario.UNID_CD_ID;

            // Prepara view
            ENTRADA_SAIDA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["ES"] = item;
            Session["IdVolta"] = id;
            Session["IdES"] = id;
            EntradaSaidaViewModel vm = Mapper.Map<ENTRADA_SAIDA, EntradaSaidaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirES(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensES"] = 2;
                    return RedirectToAction("MontarTelaES", "EntradaSaida");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            ENTRADA_SAIDA item = baseApp.GetItemById(id);
            objetoAntes = (ENTRADA_SAIDA)Session["ES"];
            item.ENSA_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            listaMaster = new List<ENTRADA_SAIDA>();
            Session["ListaES"] = null;
            Session["FiltroES"] = null;
            return RedirectToAction("MontarTelaES");
        }

        [HttpGet]
        public ActionResult ReativarES(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensES"] = 2;
                    return RedirectToAction("MontarTelaES", "EntradaSaida");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            ENTRADA_SAIDA item = baseApp.GetItemById(id);
            objetoAntes = (ENTRADA_SAIDA)Session["ES"];
            item.ENSA_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<ENTRADA_SAIDA>();
            Session["ListaES"] = null;
            Session["FiltroES"] = null;
            return RedirectToAction("MontarTelaES");
        }

        public ActionResult VoltarAnexoES()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idVeic = (Int32)Session["IdES"];
            if ((Int32)Session["VoltaES"] == 2)
            {
                return RedirectToAction("EditarES", new { id = idVeic });
            }
            return RedirectToAction("MontarTelaES");
        }

        [HttpGet]
        public ActionResult GerarNotificacaoES()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensES"] = 2;
                    return RedirectToAction("MontarTelaES", "EntradaSaida");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ENTRADA_SAIDA entrada = (ENTRADA_SAIDA)Session["ES"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss).Where(p => p.UNID_CD_ID == entrada.UNID_CD_ID).ToList();

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
            vm.NOTI_NM_TITULO = "Notificação para Morador - Visitante";
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoES(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ENTRADA_SAIDA entrada = (ENTRADA_SAIDA)Session["ES"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss);
            ViewBag.Cats = new SelectList(baseApp.GetAllCatNotificacao(idAss), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usuarios = new SelectList(lista, "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    Int32 volta = baseApp.GerarNotificacao(item, usuarioLogado, entrada, "NOTIENSA");

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<ENTRADA_SAIDA>();
                    return RedirectToAction("VoltarBaseES");
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

        public ActionResult IncluirComentarioES()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ENTRADA_SAIDA item = baseApp.GetItemById((Int32)Session["IdES"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ENTRADA_SAIDA_COMENTARIO coment = new ENTRADA_SAIDA_COMENTARIO();
            EntradaSaidaComentarioViewModel vm = Mapper.Map<ENTRADA_SAIDA_COMENTARIO, EntradaSaidaComentarioViewModel>(coment);
            vm.ESCO_DT_COMENTARIO = DateTime.Now;
            vm.ESCO_IN_ATIVO = 1;
            vm.ENSA_CD_ID = item.ENSA_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirComentarioES(EntradaSaidaComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ENTRADA_SAIDA_COMENTARIO item = Mapper.Map<EntradaSaidaComentarioViewModel, ENTRADA_SAIDA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ENTRADA_SAIDA not = baseApp.GetItemById((Int32)Session["IdES"]);

                    item.USUARIO = null;
                    not.ENTRADA_SAIDA_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarES", new { id = (Int32)Session["IdES"] });
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