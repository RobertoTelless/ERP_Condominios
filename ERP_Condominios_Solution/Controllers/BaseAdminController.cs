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
        private readonly IReservaAppService resApp;
        private readonly IMudancaAppService mudApp;


        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, INoticiaAppService notApps, ITarefaAppService tarApps, INotificacaoAppService notfApps, IUsuarioAppService usuApps, IAgendaAppService ageApps, IConfiguracaoAppService confApps, ITipoPessoaAppService tpApps, IEntradaSaidaAppService esApps, IControleVeiculoAppService cvApps, IAutorizacaoAppService autApps, IListaConvidadoAppService lisApps, IEncomendaAppService encApps, ITelefoneAppService telApps, IUnidadeAppService uniApps, IReservaAppService resApps, IMudancaAppService mudApps)
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
            resApp = resApps;
            mudApp = mudApps;
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
                listaCV = cvApp.GetAllItens (idAss);
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

            // Reservas
            List<RESERVA> listaReserva = resApp.GetAllItens(idAss);
            Int32 contaReserva = listaReserva.Where(p => p.RESE_DT_EVENTO >= DateTime.Today.Date).ToList().Count;
            ViewBag.Reserva = contaReserva;

            // Mudanca
            List<SOLICITACAO_MUDANCA> listaMudanca = mudApp.GetAllItens(idAss);
            Int32 contaMudanca = listaMudanca.Where(p => p.SOMU_DT_MUDANCA >= DateTime.Today.Date).ToList().Count;
            ViewBag.Mudanca = contaMudanca;
            Session["VoltaUnidade"] = 2;
            return View();
        }

        public ActionResult CarregarMorador()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Flags de retorno


            // Encomendas
            List<ENCOMENDA> listaEnc = encApp.GetAllItens(idAss);
            Int32 encHoje = listaEnc.Where(p => p.ENCO_DT_CHEGADA.Value.Date == DateTime.Today.Date & p.UNID_CD_ID == usuario.UNID_CD_ID).ToList().Count;
            Int32 encNao = listaEnc.Where(p => p.ENCO_IN_STATUS == 1).ToList().Count;
            ViewBag.EncomendaHoje = encHoje;
            ViewBag.EncomendaAtraso = encNao;

            // Visitantes
            List<ENTRADA_SAIDA> listaES = esApp.GetItemByData(DateTime.Today.Date, idAss);
            listaES = listaES.Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
            ViewBag.NumES = listaES.Count;
            if (listaES.Count == 0)
            {
                listaES = esApp.GetAllItens(idAss);
            }
            ViewBag.ListaES = listaES.Take(5).ToList();

            // Veiculos
            List<CONTROLE_VEICULO> listaCV = cvApp.GetItemByData(DateTime.Today.Date, idAss);
            listaCV = listaCV.Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
            ViewBag.NumCV = listaCV.Count;
            if (listaCV.Count == 0)
            {
                listaCV = cvApp.GetAllItens(idAss);
            }
            ViewBag.ListaCV = listaCV.Take(5).ToList();

            // Reservas
            List<RESERVA> listaReserva = resApp.GetAllItens(idAss).Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
            Int32 contaReserva = listaReserva.Where(p => p.RESE_DT_EVENTO >= DateTime.Today.Date).ToList().Count;
            ViewBag.Reserva = contaReserva;

            // Mudanca
            List<SOLICITACAO_MUDANCA> listaMudanca = mudApp.GetAllItens(idAss).Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
            Int32 contaMudanca = listaMudanca.Where(p => p.SOMU_DT_MUDANCA >= DateTime.Today.Date).ToList().Count;
            ViewBag.Mudanca = contaMudanca;
            return View();
        }

        public ActionResult CarregarSindico()
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

            // Reservas
            List<RESERVA> listaReserva = resApp.GetAllItens(idAss);
            Int32 contaReserva = listaReserva.Where(p => p.RESE_DT_EVENTO >= DateTime.Today.Date).ToList().Count;
            ViewBag.Reserva = contaReserva;

            // Mudanca
            List<SOLICITACAO_MUDANCA> listaMudanca = mudApp.GetAllItens(idAss);
            Int32 contaMudanca = listaMudanca.Where(p => p.SOMU_DT_MUDANCA >= DateTime.Today.Date).ToList().Count;
            ViewBag.Mudanca = contaMudanca;
            Session["VoltaUnidade"] = 3;
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
            Session["MensPermissao"] = 0;

            Session["VoltaNotificacao"] = 3;
            Session["VoltaNoticia"] = 1;
            Session["ExibeEncomenda"] = 1;
            Session["VoltaUnidade"] = 0;

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
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
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

            //// Resumo Usuário Unidade
            //List<Int32?> unidades = listaTotal.Select(p => p.UNID_CD_ID).Distinct().ToList();
            //List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
            //foreach (Int32 item in unidades)
            //{
            //    Int32 conta2 = listaTotal.Where(p => p.UNID_CD_ID == item).ToList().Count;
            //    ModeloViewModel mod1 = new ModeloViewModel();
            //    mod1.Nome = uniApp.GetItemById(item).UNID_NM_EXIBE;
            //    mod1.Valor = conta2;
            //    lista3.Add(mod1);
            //}
            //ViewBag.ListaUsuUnidade = lista3;
            //ViewBag.ContaUsuUnidade = lista3.Count;
            //Session["ListaUnidadesUsu"] = unidades;
            //Session["ListaUsuUnidade"] = lista3;
            Session["VoltaDash"] = 3;
            Session["VoltaUnidade"] = 1;
            return View(vm);
        }

        public JsonResult GetDadosGraficoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogResumo"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRSituacao()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogOp"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoLogOper()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogOp"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public ActionResult MontarCentralMensagens()
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
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Cria Base de mensagens
            CONFIGURACAO conf = confApp.GetItemById(idAss);
            List<MensagemWidgetViewModel> listaMensagens = new List<MensagemWidgetViewModel>();
            if (Session["ListaMensagem"] == null)
            {
                // Carrega notificações
                List<NOTIFICACAO> notificacoes = (List<NOTIFICACAO>)Session["Notificacoes"];
                List<NOTIFICACAO> naoLidas = notificacoes.Where(p => p.NOTI_IN_VISTA == 0).ToList();
                foreach (NOTIFICACAO item in naoLidas)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.NOTI_DT_EMISSAO.Value;
                    mens.Descrição = item.NOTI_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.NOTI_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 1;
                    mens.Categoria = item.CATEGORIA_NOTIFICACAO.CANO_NM_NOME;
                    mens.NomeCliente = item.NOTI_IN_VISTA == 1 ? "Lida" : "Não Lida";
                    listaMensagens.Add(mens);
                }

                // Carrega Agenda
                List<AGENDA> agendas = (List<AGENDA>)Session["Agendas"];
                List<AGENDA> hoje = agendas.Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList();
                foreach (AGENDA item in hoje)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.AGEN_DT_DATA;
                    mens.Descrição = item.AGEN_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.AGEN_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 2;
                    mens.Categoria = item.CATEGORIA_AGENDA.CAAG_NM_NOME;
                    mens.NomeCliente = item.AGEN_IN_STATUS == 1 ? "Ativa" : (item.AGEN_IN_STATUS == 2 ? "Suspensa" : "Encerrada");
                    listaMensagens.Add(mens);
                }

                // Carrega Tarefas
                List<TAREFA> tarefas = (List<TAREFA>)Session["ListaPendentes"];
                foreach (TAREFA item in tarefas)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.TARE_DT_ESTIMADA.Value;
                    mens.Descrição = item.TARE_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.TARE_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 3;
                    mens.Categoria = item.TIPO_TAREFA.TITR_NM_NOME;
                    if (item.TARE_IN_STATUS == 1)
                    {
                        mens.NomeCliente = "Ativa";

                    }
                    else if (item.TARE_IN_STATUS == 2)
                    {
                        mens.NomeCliente = "Em Andamento";

                    }
                    else if (item.TARE_IN_STATUS == 3)
                    {
                        mens.NomeCliente = "Suspensa";

                    }
                    else if (item.TARE_IN_STATUS == 4)
                    {
                        mens.NomeCliente = "Cancelada";

                    }
                    else if (item.TARE_IN_STATUS == 5)
                    {
                        mens.NomeCliente = "Encerrada";

                    }
                    listaMensagens.Add(mens);
                }

                //// Carrega CP
                //List<CONTA_PAGAR> cps = cpApp.GetItensAtraso(idAss);
                //foreach (CONTA_PAGAR item in cps)
                //{
                //    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                //    mens.DataMensagem = item.CAPA_DT_VENCIMENTO.Value;
                //    mens.Descrição = item.CAPA_NR_DOCUMENTO;
                //    mens.FlagUrgencia = item.CAPA_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO ? 1 : 0;
                //    mens.IdMensagem = item.CAPA_CD_ID;
                //    mens.NomeUsuario = usuario.USUA_NM_NOME;
                //    mens.TipoMensagem = 4;
                //    mens.Categoria = item.CAPA_NR_ATRASO.ToString();
                //    mens.NomeCliente = item.FORNECEDOR.FORN_NM_NOME;
                //    listaMensagens.Add(mens);
                //}

                //// Carrega CR
                //List<CONTA_RECEBER> crs = crApp.GetItensAtrasoCliente(idAss);
                //foreach (CONTA_RECEBER item in crs)
                //{
                //    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                //    mens.DataMensagem = item.CARE_DT_VENCIMENTO.Value;
                //    mens.Descrição = item.CARE_NR_DOCUMENTO;
                //    mens.FlagUrgencia = item.CARE_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO ? 1 : 0;
                //    mens.IdMensagem = item.CARE_CD_ID;
                //    mens.NomeUsuario = usuario.USUA_NM_NOME;
                //    mens.TipoMensagem = 5;
                //    mens.Categoria = item.CARE_NR_ATRASO.ToString();
                //    mens.NomeCliente = item.CLIENTE.CLIE_NM_NOME;
                //    listaMensagens.Add(mens);
                //}

                //// Carrega Compras
                //List<PEDIDO_COMPRA> pcs = pcApp.GetAllItens(idAss);
                //pcs = pcs.Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date & p.PECO_IN_STATUS < 7).ToList();
                //foreach (PEDIDO_COMPRA item in pcs)
                //{
                //    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                //    mens.DataMensagem = item.PECO_DT_PREVISTA.Value;
                //    mens.Descrição = item.PECO_NM_NOME;
                //    mens.FlagUrgencia = item.PECO_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO ? 1 : 0;
                //    mens.IdMensagem = item.PECO_CD_ID;
                //    mens.NomeUsuario = usuario.USUA_NM_NOME;
                //    mens.TipoMensagem = 6;
                //    mens.Categoria = item.PECO_NR_ATRASO.ToString();
                //    mens.NomeCliente = item.ITEM_PEDIDO_COMPRA.Count.ToString();
                //    if (item.PECO_IN_STATUS == 1)
                //    {
                //        mens.Status = "Para Cotação";

                //    }
                //    else if (item.PECO_IN_STATUS == 2)
                //    {
                //        mens.Status = "Em Cotação";

                //    }
                //    else if (item.PECO_IN_STATUS == 3)
                //    {
                //        mens.Status = "Para Aprovação";

                //    }
                //    else if (item.PECO_IN_STATUS == 4)
                //    {
                //        mens.Status = "Aprovada";

                //    }
                //    else if (item.PECO_IN_STATUS == 5)
                //    {
                //        mens.Status = "Para Receber";

                //    }
                //    else if (item.PECO_IN_STATUS == 6)
                //    {
                //        mens.Status = "Em Recebimento";

                //    }
                //    listaMensagens.Add(mens);
                //}

                //// Carrega CRM
                //List<CRM> crms = crmApp.GetAllItens(idAss);
                //crms = crms.Where(p => p.CRM1_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO & p.CRM1_IN_STATUS < 5).ToList();
                //foreach (CRM item in crms)
                //{
                //    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                //    mens.DataMensagem = item.CRM1_DT_CRIACAO.Value;
                //    mens.Descrição = item.CRM1_NM_NOME;
                //    mens.FlagUrgencia = item.CRM1_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO ? 1 : 0;
                //    mens.IdMensagem = item.CRM1_CD_ID;
                //    mens.NomeUsuario = usuario.USUA_NM_NOME;
                //    mens.TipoMensagem = 7;
                //    mens.Categoria = item.CRM1_NR_ATRASO.ToString();
                //    mens.NomeCliente = item.CLIENTE.CLIE_NM_NOME;
                //    if (item.CRM1_IN_STATUS == 1)
                //    {
                //        mens.Status = "Em Elaboração";

                //    }
                //    else if (item.CRM1_IN_STATUS == 2)
                //    {
                //        mens.Status = "Contato Realizado";

                //    }
                //    else if (item.CRM1_IN_STATUS == 3)
                //    {
                //        mens.Status = "Proposta Apresentada";

                //    }
                //    else if (item.CRM1_IN_STATUS == 4)
                //    {
                //        mens.Status = "Negociação";

                //    }
                //    else if (item.CRM1_IN_STATUS == 5)
                //    {
                //        mens.Status = "Encerrado";

                //    }
                //    listaMensagens.Add(mens);
                //}

                //// Carrega Ações
                //List<CRM_ACAO> acoes = crmApp.GetAllAcoes(idAss);
                //acoes = acoes.Where(p => p.CRAC_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO & p.CRAC_IN_STATUS < 2).ToList();
                //foreach (CRM_ACAO item in acoes)
                //{
                //    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                //    mens.DataMensagem = item.CRAC_DT_CRIACAO.Value;
                //    mens.Descrição = item.CRAC_NM_TITULO;
                //    mens.FlagUrgencia = item.CRAC_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO ? 1 : 0;
                //    mens.IdMensagem = item.CRAC_CD_ID;
                //    mens.NomeUsuario = usuario.USUA_NM_NOME;
                //    mens.TipoMensagem = 8;
                //    mens.Categoria = item.CRAC_NR_ATRASO.ToString();
                //    mens.NomeCliente = item.CRM.CRM1_NM_NOME;
                //    if (item.CRAC_IN_STATUS == 1)
                //    {
                //        mens.Status = "Ativa";

                //    }
                //    else if (item.CRAC_IN_STATUS == 2)
                //    {
                //        mens.Status = "Pendente";

                //    }
                //    else if (item.CRAC_IN_STATUS == 3)
                //    {
                //        mens.Status = "Encerrada";

                //    }
                //    listaMensagens.Add(mens);
                //}

                //// Carrega Propostas
                //List<CRM_PROPOSTA> props = crmApp.GetAllPropostas(idAss);
                //props = props.Where(p => p.CRPR_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO & p.CRPR_IN_STATUS < 3).ToList();
                //foreach (CRM_PROPOSTA item in props)
                //{
                //    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                //    mens.DataMensagem = item.CRPR_DT_PROPOSTA;
                //    mens.Descrição = item.CRPR_NR_NUMERO;
                //    mens.FlagUrgencia = item.CRPR_NR_ATRASO > conf.CONF_NR_MARGEM_ATRASO ? 1 : 0;
                //    mens.IdMensagem = item.CRPR_CD_ID;
                //    mens.NomeUsuario = usuario.USUA_NM_NOME;
                //    mens.TipoMensagem = 9;
                //    mens.Categoria = item.CRPR_NR_ATRASO.ToString();
                //    mens.NomeCliente = item.CRM.CRM1_NM_NOME;
                //    if (item.CRPR_IN_STATUS == 1)
                //    {
                //        mens.Status = "Em Elaboração";

                //    }
                //    else if (item.CRPR_IN_STATUS == 2)
                //    {
                //        mens.Status = "Enviada";

                //    }
                //    else if (item.CRPR_IN_STATUS == 3)
                //    {
                //        mens.Status = "Cancelada";

                //    }
                //    listaMensagens.Add(mens);
                //}
                Session["ListaMensagem"] = listaMensagens;
            }
            else
            {
                listaMensagens = (List<MensagemWidgetViewModel>)Session["ListaMensagem"];
            }

            // Prepara listas dos filtros
            List<SelectListItem> tipos = new List<SelectListItem>();
            tipos.Add(new SelectListItem() { Text = "Notificações", Value = "1" });
            tipos.Add(new SelectListItem() { Text = "Agenda", Value = "2" });
            tipos.Add(new SelectListItem() { Text = "Tarefas", Value = "3" });
            //tipos.Add(new SelectListItem() { Text = "Contas a Pagar", Value = "4" });
            //tipos.Add(new SelectListItem() { Text = "Contas a Receber", Value = "5" });
            //tipos.Add(new SelectListItem() { Text = "Compras", Value = "6" });
            //tipos.Add(new SelectListItem() { Text = "Processos CRM", Value = "7" });
            //tipos.Add(new SelectListItem() { Text = "Ações", Value = "8" });
            //tipos.Add(new SelectListItem() { Text = "Propostas", Value = "9" });
            ViewBag.Tipos = new SelectList(tipos, "Value", "Text");
            List<SelectListItem> urg = new List<SelectListItem>();
            urg.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            urg.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Urgencia = new SelectList(urg, "Value", "Text");

            // Exibe
            ViewBag.ListaMensagem = listaMensagens;
            MensagemWidgetViewModel mod = new MensagemWidgetViewModel();
            return View(mod);
        }

        public ActionResult RetirarFiltroCentralMensagens()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMensagem"] = null;
            return RedirectToAction("MontarCentralMensagens");
        }

        [HttpPost]
        public ActionResult FiltrarCentralMensagens(MensagemWidgetViewModel item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<MensagemWidgetViewModel> listaObj = (List<MensagemWidgetViewModel>)Session["ListaMensagem"];
                if (item.TipoMensagem != null)
                {
                    listaObj = listaObj.Where(p => p.TipoMensagem == item.TipoMensagem).ToList();
                }
                if (item.Descrição != null)
                {
                    listaObj = listaObj.Where(p => p.Descrição.Contains(item.Descrição)).ToList();
                }
                if (item.DataMensagem != null)
                {
                    listaObj = listaObj.Where(p => p.DataMensagem == item.DataMensagem).ToList();
                }
                if (item.FlagUrgencia != null)
                {
                    listaObj = listaObj.Where(p => p.FlagUrgencia == item.FlagUrgencia).ToList();
                }
                Session["ListaMensagem"] = listaObj;

                // Sucesso
                return RedirectToAction("MontarCentralMensagens");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarCentralMensagens");
            }
        }

    }
}