﻿using System;
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
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;

namespace ERP_Condominios_Solution.Controllers
{
    public class BaseAdminController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly INoticiaAppService notiApp;
        private readonly ILogAppService logApp;
        private readonly ITarefaAppService tarApp;
        private readonly INotificacaoAppService notfApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAgendaAppService ageApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITipoPessoaAppService tpApp;
        private readonly IEntradaSaidaAppService esApp;
        private readonly IControleVeiculoAppService cvApp;
        private readonly IAutorizacaoAppService autApp;
        private readonly IListaConvidadoAppService lisApp;
        private readonly IEncomendaAppService encApp;
        private readonly ITelefoneAppService telApp;
        private readonly IUnidadeAppService uniApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, INoticiaAppService notApps, ITarefaAppService tarApps, INotificacaoAppService notfApps, IUsuarioAppService usuApps, IAgendaAppService ageApps, IConfiguracaoAppService confApps, ITipoPessoaAppService tpApps, IEntradaSaidaAppService esApps, IControleVeiculoAppService cvApps, IAutorizacaoAppService autApps, IListaConvidadoAppService lisApps, IEncomendaAppService encApps, ITelefoneAppService telApps, IUnidadeAppService uniApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notApps;
            tarApp = tarApps;
            notfApp = notfApps;
            usuApp = usuApps;
            ageApp = ageApps;
            confApp = confApps;
            tpApp = tpApps;
            esApp = esApps;
            cvApp = cvApps;
            autApp = autApps;
            lisApp = lisApps;
            encApp = encApps;
            telApp = telApps;
            uniApp = uniApps;
        }

        public ActionResult CarregarAdmin()
        {
            Int32? idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = baseApp.GetAllUsuarios(idAss.Value).Count;
            ViewBag.Logs = logApp.GetAllItens(idAss.Value).Count;
            ViewBag.UsuariosLista = baseApp.GetAllUsuarios(idAss.Value);
            ViewBag.LogsLista = logApp.GetAllItens(idAss.Value);
            return View();

        }

        public ActionResult CarregarLandingPage()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public JsonResult GetRefreshTime()
        {
            return Json(confApp.GetById(1).CONF_NR_REFRESH_DASH);
        }

        public JsonResult GetConfigNotificacoes()
        {
            Int32? idAss = (Int32)Session["IdAssinante"];
            bool hasNotf;
            var hash = new Hashtable();
            USUARIO usu = (USUARIO)Session["Usuario"];
            CONFIGURACAO conf = confApp.GetById(1);

            if (baseApp.GetAllItensUser(usu.USUA_CD_ID, idAss.Value).Count > 0)
            {
                hasNotf = true;
            }
            else
            {
                hasNotf = false;
            }

            hash.Add("CONF_NM_ARQUIVO_ALARME", conf.CONF_NM_ARQUIVO_ALARME);
            hash.Add("NOTIFICACAO", hasNotf);
            return Json(hash);
        }

        public ActionResult CarregarPortaria()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Flags de retorno


            // Encomendas
            List<ENCOMENDA> listaEnc = encApp.GetAllItens(idAss);
            Int32 encHoje = listaEnc.Where(p => p.ENCO_DT_CHEGADA.Value.Date == DateTime.Today.Date).ToList().Count;
            Int32 encNao = listaEnc.Where(p => p.ENCO_IN_STATUS == 1).ToList().Count;
            ViewBag.EncomendaHoje = encHoje;
            ViewBag.EncomendaAtraso = encNao;

            // Visitantes
            List<ENTRADA_SAIDA> listaES = esApp.GetItemByData(DateTime.Today.Date, idAss);
            ViewBag.NumES = listaES.Count;
            if (listaES.Count == 0)
            {
                listaES = esApp.GetAllItens(idAss);
            }          
            ViewBag.ListaES = listaES.Take(5).ToList();

            // Veiculos
            List<CONTROLE_VEICULO> listaCV = cvApp.GetItemByData(DateTime.Today.Date, idAss);
            ViewBag.NumCV = listaCV.Count;
            if (listaCV.Count == 0)
            {
                listaCV = cvApp.GetAllItens(idAss);
            }
            ViewBag.ListaCV = listaCV.Take(5).ToList();

            // Autorizacao/Restricao
            List<AUTORIZACAO_ACESSO> listaBase = autApp.GetAllItens(idAss);
            Int32 aut = listaBase.Where(p => p.AUAC_IN_TIPO == 1).ToList().Count;
            Int32 res = listaBase.Where(p => p.AUAC_IN_TIPO == 2).ToList().Count;
            ViewBag.Autorizacao = aut;
            ViewBag.Restricao = res;

            // Listas de Convidados
            List<LISTA_CONVIDADO> listaCon = lisApp.GetAllItens(idAss);
            Int32 lis = listaCon.Where(p => p.LICO_DT_EVENTO >= DateTime.Today.Date).ToList().Count;
            ViewBag.Convidado = lis;


            return View();
        }

        public ActionResult CarregarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["Login"] == 1)
            {
                Session["Perfis"] = baseApp.GetAllPerfis();
                Session["Usuarios"] = usuApp.GetAllUsuarios(idAss);
                Session["TiposPessoas"] = tpApp.GetAllItens();
                Session["UFs"] = tpApp.GetAllItens();
            }

            Session["MensTarefa"] = 0;
            Session["MensNoticia"] = 0;
            Session["MensNotificacao"] = 0;
            Session["MensUsuario"] = 0;
            Session["MensLog"] = 0;
            Session["MensUsuarioAdm"] = 0;
            Session["MensAgenda"] = 0;
            Session["MensTemplate"] = 0;
            Session["MensConfiguracao"] = 0;
            Session["MensTelefone"] = 0;
            Session["MensCargo"] = 0;
            Session["MensGrupo"] = 0;
            Session["MensSubGrupo"] = 0;
            Session["MensCC"] = 0;
            Session["MensBanco"] = 0;
            Session["MensCondominio"] = 0;
            Session["MensUnidade"] = 0;
            Session["MensVaga"] = 0;
            Session["MensVeiculo"] = 0;
            Session["MensFornecedor"] = 0;
            Session["MensTelefone"] = 0;
            Session["MensAmbiente"] = 0;
            Session["MensAutorizacao"] = 0;
            Session["MensOcorrencia"] = 0;
            Session["MensCC"] = 0;
            Session["MensBanco"] = 0;
            Session["MensEquipamento"] = 0;
            Session["MensProduto"] = 0;
            Session["MensLista"] = 0;
            Session["MensSMSError"] = 0;
            Session["MensControleVeiculo"] = 0;
            Session["MensCorpo"] = 0;
            Session["MensMorador"] = 0;
            Session["MensMudanca"] = 0;
            Session["MensES"] = 0;
            Session["MensEncomenda"] = 0;
            Session["MensReserva"] = 0;

            Session["VoltaNotificacao"] = 3;
            Session["VoltaNoticia"] = 1;
            Session["ExibeEncomenda"] = 1;

            USUARIO usu = new USUARIO();
            UsuarioViewModel vm = new UsuarioViewModel();
            List<NOTIFICACAO> noti = new List<NOTIFICACAO>();

            ObjectCache cache = MemoryCache.Default;
            USUARIO usuContent = cache["usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID] as USUARIO;

            if (usuContent == null)
            {
                usu = usuApp.GetItemById(((USUARIO)Session["UserCredentials"]).USUA_CD_ID);
                vm = Mapper.Map<USUARIO, UsuarioViewModel>(usu);
                noti = notfApp.GetAllItens(idAss);
                DateTime expiration = DateTime.Now.AddDays(15);
                cache.Set("usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, usu, expiration);
                cache.Set("vm" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, vm, expiration);
                cache.Set("noti" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, noti, expiration);
            }

            usu = cache.Get("usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as USUARIO;
            vm = cache.Get("vm" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as UsuarioViewModel;
            noti = cache.Get("noti" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as List<NOTIFICACAO>;
            ViewBag.Perfil = usu.PERFIL.PERF_SG_SIGLA;

            noti = notfApp.GetAllItensUser(usu.USUA_CD_ID, usu.ASSI_CD_ID);
            Session["Notificacoes"] = noti;
            Session["ListaNovas"] = noti.Where(p => p.NOTI_IN_VISTA == 0).ToList().Take(5).OrderByDescending(p => p.NOTI_DT_EMISSAO).ToList();
            Session["NovasNotificacoes"] = noti.Where(p => p.NOTI_IN_VISTA == 0).Count();
            Session["Nome"] = usu.USUA_NM_NOME;

            Session["Noticias"] = notiApp.GetAllItensValidos(idAss);
            Session["NoticiasNumero"] = ((List<NOTICIA>)Session["Noticias"]).Count;

            Session["ListaPendentes"] = tarApp.GetTarefaStatus(usu.USUA_CD_ID, 1);
            Session["TarefasPendentes"] = ((List<TAREFA>)Session["ListaPendentes"]).Count;
            Session["TarefasLista"] = tarApp.GetByUser(usu.USUA_CD_ID);
            Session["Tarefas"] = ((List<TAREFA>)Session["TarefasLista"]).Count;

            Session["Agendas"] = ageApp.GetByUser(usu.USUA_CD_ID, usu.ASSI_CD_ID);
            Session["NumAgendas"] = ((List<AGENDA>)Session["Agendas"]).Count;
            Session["AgendasHoje"] = ((List<AGENDA>)Session["Agendas"]).Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList();
            Session["NumAgendasHoje"] = ((List<AGENDA>)Session["AgendasHoje"]).Count;

            Session["Telefones"] = telApp.GetAllItens(usu.ASSI_CD_ID);
            Session["NumTelefones"] = ((List<TELEFONE>)Session["Telefones"]).Count;
            Session["Logs"] = usu.LOG.Count;

            String frase = String.Empty;
            String nome = usu.USUA_NM_NOME.Substring(0, usu.USUA_NM_NOME.IndexOf(" "));
            Session["NomeGreeting"] = nome;
            if (DateTime.Now.Hour <= 12)
            {
                frase = "Bom dia, " + nome;
            }
            else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
            {
                frase = "Boa tarde, " + nome;
            }
            else
            {
                frase = "Boa noite, " + nome;
            }
            Session["Greeting"] = frase;
            Session["Foto"] = usu.USUA_AQ_FOTO;

            // Mensagens
            if ((Int32)Session["MensPermissao"] == 2)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            Session["MensPermissao"] = 0;
            return View(vm);
        }

        public ActionResult CarregarDesenvolvimento()
        {
            return View();
        }

        public ActionResult VoltarDashboard()
        {
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult MontarFaleConosco()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardAdministracao()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas usuarios
            List<USUARIO> listaTotal = baseApp.GetAllItens(idAss);
            List<USUARIO> bloqueados = listaTotal.Where(p => p.USUA_IN_BLOQUEADO == 1).ToList();

            Int32 numUsuarios = listaTotal.Count;
            Int32 numBloqueados = bloqueados.Count;
            Int32 numAcessos = listaTotal.Sum(p => p.USUA_NR_ACESSOS).Value;
            Int32 numFalhas = listaTotal.Sum(p => p.USUA_NR_FALHAS).Value;

            ViewBag.NumUsuarios = numUsuarios;
            ViewBag.NumBloqueados = numBloqueados;
            ViewBag.NumAcessos = numAcessos;
            ViewBag.NumFalhas = numFalhas;

            Session["TotalUsuarios"] = listaTotal.Count;
            Session["Bloqueados"] = numBloqueados;

            // Recupera listas log
            List<LOG> listaLog = logApp.GetAllItens(idAss);
            Int32 log = listaLog.Count;
            Int32 logDia = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == DateTime.Today.Date).ToList().Count;
            Int32 logMes = listaLog.Where(p => p.LOG_DT_DATA.Value.Month == DateTime.Today.Month & p.LOG_DT_DATA.Value.Year == DateTime.Today.Year).ToList().Count;

            ViewBag.Log = log;
            ViewBag.LogDia = logDia;
            ViewBag.LogMes = logMes;

            Session["TotalLog"] = log;
            Session["LogDia"] = logDia;
            Session["LogMes"] = logMes;

            // Resumo Log Diario
            List<DateTime> datasCR = listaLog.Where(m => m.LOG_DT_DATA != null).Select(p => p.LOG_DT_DATA.Value.Date).Distinct().ToList();
            List<ModeloViewModel> listaLogDia = new List<ModeloViewModel>();
            foreach (DateTime item in datasCR)
            {
                Int32 conta = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.Valor = conta;
                listaLogDia.Add(mod1);
            }
            ViewBag.ListaLogDia = listaLogDia;
            ViewBag.ContaLogDia = listaLogDia.Count;
            Session["ListaDatasLog"] = datasCR;
            Session["ListaLogResumo"] = listaLogDia;

            // Resumo Log Situacao  
            List<String> opLog = listaLog.Select(p => p.LOG_NM_OPERACAO).Distinct().ToList();
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            foreach (String item in opLog)
            {
                Int32 conta1 = listaLog.Where(p => p.LOG_NM_OPERACAO == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.Nome = item;
                mod1.Valor = conta1;
                lista2.Add(mod1);
            }
            ViewBag.ListaLogOp = lista2;
            ViewBag.ContaLogOp = lista2.Count;
            Session["ListaOpLog"] = opLog;
            Session["ListaLogOp"] = lista2;

            // Resumo Usuário Unidade
            List<Int32?> unidades = listaTotal.Select(p => p.UNID_CD_ID).Distinct().ToList();
            List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
            foreach (Int32 item in unidades)
            {
                Int32 conta2 = listaTotal.Where(p => p.UNID_CD_ID == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.Nome = uniApp.GetItemById(item).UNID_NM_EXIBE;
                mod1.Valor = conta2;
                lista3.Add(mod1);
            }
            ViewBag.ListaUsuUnidade = lista3;
            ViewBag.ContaUsuUnidade = lista3.Count;
            Session["ListaUnidadesUsu"] = unidades;
            Session["ListaUsuUnidade"] = lista3;
            Session["VoltaDash"] = 3;
            return View(vm);
        }

    }
}