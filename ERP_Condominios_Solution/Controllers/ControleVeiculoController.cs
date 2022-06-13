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
using System.Data.Entity;

namespace ERP_Condominios_Solution.Controllers
{
    public class ControleVeiculoController : Controller
    {
        private readonly IControleVeiculoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IFornecedorAppService forApp;

        private String msg;
        private Exception exception;
        CONTROLE_VEICULO objeto = new CONTROLE_VEICULO();
        CONTROLE_VEICULO objetoAntes = new CONTROLE_VEICULO();
        List<CONTROLE_VEICULO> listaMaster = new List<CONTROLE_VEICULO>();
        String extensao;

        public ControleVeiculoController(IControleVeiculoAppService baseApps, ILogAppService logApps, IConfiguracaoAppService confApps, IFornecedorAppService forApps)
        {
            baseApp = baseApps; ;
            logApp = logApps;
            confApp = confApps;
            forApp = forApps;
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
        public ActionResult MontarTelaControleVeiculo()
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
                    Session["MensControleVeiculo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<CONTROLE_VEICULO>)Session["ListaControleVeiculo"] == null)
            {
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    listaMaster = baseApp.GetByUnidade(usuario.UNID_CD_ID.Value);
                    Session["ListaControleVeiculo"] = listaMaster;
                }
                else if (usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMaster = baseApp.GetAllItens(idAss);
                    Session["ListaControleVeiculo"] = listaMaster.Where(p => p.COVE_DT_ENTRADA.Value.Date == DateTime.Today.Date).ToList();
                }
                Session["FiltroControleVeiculo"] = null;
            }

            ViewBag.Listas = ((List<CONTROLE_VEICULO>)Session["ListaControleVeiculo"]);
            ViewBag.Title = "Controle de Veiculos";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Veics = ((List<CONTROLE_VEICULO>)Session["ListaControleVeiculo"]).Count;

            // Mensagem
            if ((Int32)Session["MensControleVeiculo"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
  
            // Abre view
            Session["MensControleVeiculo"] = 0;
            Session["VoltaControleVeiculo"] = 1;
            objeto = new CONTROLE_VEICULO();
            objeto.COVE_DT_ENTRADA = DateTime.Today.Date;
            return View(objeto);
        }

        public ActionResult RetirarFiltroControleVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaControleVeiculo"] = null;
            Session["FiltroControleVeiculo"] = null;
            listaMaster = new List<CONTROLE_VEICULO>();
            return RedirectToAction("MontarTelaControleVeiculo");
        }

        public ActionResult MostrarTudoControleVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaControleVeiculo"] = listaMaster;
            return RedirectToAction("MontarTelaControleVeiculo");
        }

        public ActionResult MostrarGeralControleVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItens(idAss);
            Session["ListaControleVeiculo"] = listaMaster;
            return RedirectToAction("MontarTelaControleVeiculo");
        }

        [HttpPost]
        public ActionResult FiltrarControleVeiculo(CONTROLE_VEICULO item)
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

                    List<CONTROLE_VEICULO> listaObj = new List<CONTROLE_VEICULO>();
                    Int32 volta = baseApp.ExecuteFilter(item.COVE_NM_PLACA, item.COVE_NM_MARCA, item.UNID_CD_ID, item.TIVE_CD_ID, item.COVE_DT_ENTRADA, item.COVE_DT_SAIDA, idAss, out listaObj);
                    Session["FiltroControleVeiculo"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensControleVeiculo"] = 1;
                    }

                    // Sucesso
                    Session["MensControleVeiculo"] = 0;
                    listaMaster = listaObj;
                    Session["ListaControleVeiculo"] = listaObj;
                    return RedirectToAction("MontarTelaControleVeiculo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaControleVeiculo");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaControleVeiculo");
            }
        }

        public ActionResult VoltarBaseControleVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaControleVeiculo"] = null;
            return RedirectToAction("MontarTelaControleVeiculo");
        }

        [HttpGet]
        public ActionResult IncluirControleVeiculo()
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
                    Session["MensControleVeiculo"] = 2;
                    return RedirectToAction("MontarTelaControleVeiculo", "ControleVeiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["TipoNota"] = 1;

            // Prepara view
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            CONTROLE_VEICULO item = new CONTROLE_VEICULO();
            ControleVeiculoViewModel vm = Mapper.Map<CONTROLE_VEICULO, ControleVeiculoViewModel>(item);
            vm.COVE_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.COVE_DT_ENTRADA = DateTime.Now;
            vm.COVE_DT_PREVISAO_SAIDA = DateTime.Now.AddHours(Convert.ToDouble(conf.CONF_IN_LIMITE_HORA_VEICULO));
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirControleVeiculo(ControleVeiculoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTROLE_VEICULO item = Mapper.Map<ControleVeiculoViewModel, CONTROLE_VEICULO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<CONTROLE_VEICULO>();
                    Session["ListaControleVeiculo"] = null;
                    Session["VoltaControleVeiculo"] = 1;
                    Session["IdControleVeiculoVolta"] = item.COVE_CD_ID;
                    Session["ControleVeiculo"] = item;
                    Session["MensControleVeiculo"] = 0;

                    // Notificação
                    if (item.UNID_CD_ID != null & item.UNID_CD_ID > 0)
                    {
                        return RedirectToAction("GerarNotificacaoControleVeiculo");
                    }
                    return RedirectToAction("MontarTelaControleVeiculo");
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
        public ActionResult EditarControleVeiculo(Int32 id)
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
                    Session["MensControleVeiculo"] = 2;
                    return RedirectToAction("MontarTelaControleVeiculo", "ControleVeiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            Session["TipoNota"] = 2;

            CONTROLE_VEICULO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["ControleVeiculo"] = item;
            Session["IdVolta"] = id;
            Session["IdControleVeiculo"] = id;
            Session["VoltaControleVeiculo"] = 2;
            ControleVeiculoViewModel vm = Mapper.Map<CONTROLE_VEICULO, ControleVeiculoViewModel>(item);
            if (vm.COVE_DT_SAIDA == null)
            {
                vm.COVE_DT_SAIDA = DateTime.Now;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarControleVeiculo(ControleVeiculoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTROLE_VEICULO item = Mapper.Map<ControleVeiculoViewModel, CONTROLE_VEICULO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<CONTROLE_VEICULO>();
                    Session["ListaControleVeiculo"] = null;
                    Session["MensControleVeiculo"] = 0;
                    if (item.UNID_CD_ID != null & item.UNID_CD_ID > 0)
                    {
                        return RedirectToAction("GerarNotificacaoControleVeiculo");
                    }
                    return RedirectToAction("MontarTelaControleVeiculo");
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
        public ActionResult VerControleVeiculo(Int32 id)
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
                    Session["MensControleVeiculo"] = 2;
                    return RedirectToAction("MontarTelaControleVeiculo", "ControleVeiculo");
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
            CONTROLE_VEICULO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["ControleVeiculo"] = item;
            Session["IdVolta"] = id;
            Session["IdControleVeiculo"] = id;
            ControleVeiculoViewModel vm = Mapper.Map<CONTROLE_VEICULO, ControleVeiculoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirControleVeiculo(Int32 id)
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
                    Session["MensControleVeiculo"] = 2;
                    return RedirectToAction("MontarTelaControleVeiculo", "ControleVeiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CONTROLE_VEICULO item = baseApp.GetItemById(id);
            objetoAntes = (CONTROLE_VEICULO)Session["ControleVeiculo"];
            item.COVE_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            listaMaster = new List<CONTROLE_VEICULO>();
            Session["ListaControleVeiculo"] = null;
            Session["FiltroControleVeiculo"] = null;
            return RedirectToAction("MontarTelaControleVeiculo");
        }

        [HttpGet]
        public ActionResult ReativarControleVeiculo(Int32 id)
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
                    Session["MensControleVeiculo"] = 2;
                    return RedirectToAction("MontarTelaControleVeiculo", "ControleVeiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CONTROLE_VEICULO item = baseApp.GetItemById(id);
            objetoAntes = (CONTROLE_VEICULO)Session["ControleVeiculo"];
            item.COVE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<CONTROLE_VEICULO>();
            Session["ListaControleVeiculo"] = null;
            Session["FiltroControleVeiculo"] = null;
            return RedirectToAction("MontarTelaControleVeiculo");
        }

        public ActionResult VoltarAnexoControleVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idVeic = (Int32)Session["IdControleVeiculo"];
            if ((Int32)Session["VoltaControleVeiculo"] == 2)
            {
                return RedirectToAction("EditarControleVeiculo", new { id = idVeic });
            }
            return RedirectToAction("MontarTelaControleVeiculo");
        }

        [HttpGet]
        public ActionResult GerarNotificacaoControleVeiculo()
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
                    Session["MensControleVeiculo"] = 2;
                    return RedirectToAction("MontarTelaControleVeiculo", "ControleVeiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            CONTROLE_VEICULO veiculo = (CONTROLE_VEICULO)Session["ControleVeiculo"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss).Where(p => p.UNID_CD_ID == veiculo.UNID_CD_ID).ToList();
            USUARIO topo = lista.First();

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
            vm.CANO_CD_ID = 1;
            vm.USUA_CD_ID = topo.USUA_CD_ID;
            vm.NOTI_NM_TITULO = "Notificação para Morador - Veículo Visitante";
            if ((Int32)Session["TipoNota"] == 1)
            {
                vm.NOTI_TX_TEXTO = "O veículo de placa " + veiculo.COVE_NM_PLACA + " deu entrada as " + veiculo.COVE_DT_ENTRADA.Value.ToShortDateString() + " " + veiculo.COVE_DT_ENTRADA.Value.ToShortTimeString() + ".";
            }
            else
            {
                veiculo = baseApp.GetItemById((Int32)Session["IdControleVeiculo"]);
                vm.NOTI_TX_TEXTO = "O veículo de placa " + veiculo.COVE_NM_PLACA + " saiu as " + veiculo.COVE_DT_SAIDA.Value.ToShortDateString() + " " + veiculo.COVE_DT_SAIDA.Value.ToShortTimeString() + ".";
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoControleVeiculo(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            CONTROLE_VEICULO veiculo = (CONTROLE_VEICULO)Session["ControleVeiculo"];
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
                    Int32 volta = baseApp.GerarNotificacao(item, usuarioLogado, veiculo, "NOTICOVE");

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<CONTROLE_VEICULO>();
                    return RedirectToAction("VoltarBaseControleVeiculo");
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

        public ActionResult IncluirAcompanhamentoControleVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CONTROLE_VEICULO item = baseApp.GetItemById((Int32)Session["IdControleVeiculo"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            CONTROLE_VEICULO_ACOMPANHAMENTO coment = new CONTROLE_VEICULO_ACOMPANHAMENTO();
            ControleVeiculoAcompanhamentoViewModel vm = Mapper.Map<CONTROLE_VEICULO_ACOMPANHAMENTO, ControleVeiculoAcompanhamentoViewModel>(coment);
            vm.CVAC_DT_ACOMPANHAMENTO = DateTime.Now;
            vm.CVAC_IN_ATIVO = 1;
            vm.COVE_CD_ID = item.COVE_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcompanhamentoControleVeiculo(ControleVeiculoAcompanhamentoViewModel vm)
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
                    CONTROLE_VEICULO_ACOMPANHAMENTO item = Mapper.Map<ControleVeiculoAcompanhamentoViewModel, CONTROLE_VEICULO_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTROLE_VEICULO not = baseApp.GetItemById((Int32)Session["IdControleVeiculo"]);

                    item.USUARIO = null;
                    not.CONTROLE_VEICULO_ACOMPANHAMENTO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarControleVeiculo", new { id = (Int32)Session["IdControleVeiculo"] });
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaUnidade"] == 1)
            {
                return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 2)
            {
                return RedirectToAction("CarregarPortaria", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 3)
            {
                return RedirectToAction("CarregarSindico", "BaseAdmin");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

    }
}