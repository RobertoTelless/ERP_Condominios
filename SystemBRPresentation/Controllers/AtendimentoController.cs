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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using EntitiesServices.WorkClasses;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU", "VEN", "PRO" })]
    public class AtendimentoController : Controller
    {
        private readonly IAtendimentoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IClienteAppService cliApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IServicoAppService serApp;
        private readonly ICategoriaAtendimentoAppService caApp;
        private readonly IProdutoAppService proApp;
        private readonly IDepartamentoAppService depApp;
        private readonly ITarefaAppService tarApp;
        private readonly IConfiguracaoAppService conApp;
        private readonly IAtendimentoAgendaAppService aAgenApp;
        private readonly IAgendaAppService agenApp;

        //private readonly IOrdemServicoAppService osApp;

        private String msg; 
        private Exception exception;
        String extensao = String.Empty;
        ATENDIMENTO objeto = new ATENDIMENTO();
        ATENDIMENTO objetoAntes = new ATENDIMENTO();
        List<ATENDIMENTO> listaMaster = new List<ATENDIMENTO>();

        public AtendimentoController(IAtendimentoAppService baseApps, ILogAppService logApps, IClienteAppService cliApps, IUsuarioAppService usuApps, IServicoAppService serApps, ICategoriaAtendimentoAppService caApps, IProdutoAppService proApps, IDepartamentoAppService depApps, ITarefaAppService tarApps, IConfiguracaoAppService conApps, IAtendimentoAgendaAppService aAgenApps, IAgendaAppService agenApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            cliApp = cliApps;
            usuApp = usuApps;
            serApp = serApps;
            caApp = caApps;
            //osApp = osApps;
            proApp = proApps;
            depApp = depApps;
            tarApp = tarApps;
            conApp = conApps;
            aAgenApp = aAgenApps;
            agenApp = agenApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Voltar()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarDashboard()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult CalculaDataPrevista(Int32? id)
        {
            Hashtable result = new Hashtable();

            if (id != null)
            {
                CATEGORIA_ATENDIMENTO ca = caApp.GetItemById((Int32)id);
                result.Add("dtPrevista", DateTime.Now.AddDays(ca.CAAT_IN_SLA == null ? 0 : (Int32)ca.CAAT_IN_SLA).ToString("dd/MM/yyyy HH:mm"));
            }
            else
            {
                result.Add("dtPrevista", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();

            SessionMocks.Clientes = cliApp.GetAllItens();

            if (nome != null)
            {
                List<CLIENTE> lstCliente = SessionMocks.Clientes.Where(x => x.CLIE_NM_NOME != null && x.CLIE_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = SessionMocks.Clientes.Where(x => x.CLIE_NM_RAZAO != null).ToList<CLIENTE>();
                    lstCliente = lstCliente.Where(x => x.CLIE_NM_RAZAO.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.CLIE_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.CLIE_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.CLIE_NM_NOME + " (" + item.CLIE_NM_RAZAO + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }

            return Json(listResult);
        }

        [HttpPost]
        public ActionResult IncluirAgendamento(AGENDA agenda, Int32 ATEN_CD_ID)
        {
            USUARIO usuario = SessionMocks.UserCredentials;

            Hashtable error = new Hashtable();

            ATENDIMENTO_AGENDA aa = new ATENDIMENTO_AGENDA();
            aa.ATEN_CD_ID = ATEN_CD_ID;
            aa.ATAG_IN_ATIVO = 1;

            USUARIO user = SessionMocks.UserCredentials;
            agenda.ASSI_CD_ID = SessionMocks.IdAssinante;
            agenda.USUA_CD_ID = user.USUA_CD_ID;
            agenda.AGEN_IN_ATIVO = 1;
            agenda.AGEN_IN_STATUS = 1;
            Int32 id = (Int32)Session["idAtendimento"];

            if (agenda.CAAG_CD_ID == null)
            {
                error.Add("categoria", "Campo CATEGORIA obrigatorio");
            }

            if (agenda.AGEN_DT_DATA == DateTime.MinValue)
            {
                error.Add("data", "Campo DATA obrigatorio");
                if (agenda.AGEN_HR_HORA == TimeSpan.Zero)
                {
                    error.Add("hora", "Campo HORA obrigatorio");
                }
            }

            if (agenda.AGEN_NM_TITULO == null)
            {
                error.Add("titulo", "Campo TÍTULO obrigatorio");
            }

            Int32? volta = null;
            if (error.Count == 0)
            {
                volta = agenApp.ValidateCreate(agenda, user);

                // Cria pastas
                String caminho = "/Imagens/Agenda/" + SessionMocks.IdAssinante.ToString() + "/" + agenda.AGEN_CD_ID.ToString() + "/Anexos/";
                Directory.CreateDirectory(Server.MapPath(caminho));

                SessionMocks.listaAgenda = null;
            }
            else
            {
                SessionMocks.errosAgendamento = error;
                return RedirectToAction("EditarAtendimento", "Atendimento", new { id = id });
            }

            if (volta != 0)
            {
                error.Add("cadastro", "Agendamento não incluído, verifique e tente novamente");
                SessionMocks.errosAgendamento = error;
            }

            aa.AGEN_CD_ID = agenda.AGEN_CD_ID;
            Int32 voltaAA = aAgenApp.ValidateCreate(aa, usuario);

            if (voltaAA != 0)
            {
                error.Add("cadastro", "Agendamento não incluído, verifique e tente novamente");
                SessionMocks.errosAgendamento = error;
            }

            Session["agenLista"] = 1;
            return RedirectToAction("EditarAtendimento", "Atendimento", new { id = id });
        }

        [HttpGet]
        public ActionResult MontarTelaAtendimento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            if (SessionMocks.filtroAtendimento != null)
            {
                FiltrarAtendimento(SessionMocks.filtroAtendimento);
            }

            if (SessionMocks.idAtendimento != 0)
            {
                ViewBag.CodigoAtendimento = SessionMocks.idAtendimento;
            }

            SessionMocks.errosAgendamento = null;
            SessionMocks.consultaAtendimento = 0;

            // Carrega listas
            if (SessionMocks.listaAtendimento == null || SessionMocks.listaAtendimento.Count == 0)
            {
                listaMaster = baseApp.GetAllItens().Where(x => x.ATEN_IN_STATUS != 3).ToList();
                SessionMocks.listaAtendimento = listaMaster;
            }

            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" && usuario.PERFIL.PERF_SG_SIGLA != "GER")
            {
                SessionMocks.listaAtendimento = SessionMocks.listaAtendimento.Where(p => p.USUA_CD_ID == SessionMocks.UserCredentials.USUA_CD_ID).ToList();
            }

            if (SessionMocks.filtroAtendimento == null)
            {
                ViewBag.Listas = SessionMocks.listaAtendimento.Where(x => x.ATEN_IN_STATUS != 3).ToList<ATENDIMENTO>();
            }
            else
            {
                ViewBag.Listas = SessionMocks.listaAtendimento;
            }
            ViewBag.Title = "Atendimentos";

            // Indicadores
            ViewBag.Atendimentos = SessionMocks.listaAtendimento.Count;
            ViewBag.Criados = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 1).ToList().Count;
            ViewBag.Aguardando = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 2).ToList().Count;
            ViewBag.Canceladas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 3).ToList().Count;
            ViewBag.Execucoes = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 4).ToList().Count;
            ViewBag.Encerradas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 5).ToList().Count;
            ViewBag.ForaSLA = SessionMocks.listaAtendimento.Count(p => p.ATEN_DT_PREVISTA != null && DateTime.Now.Date > p.ATEN_DT_PREVISTA.Value.Date);

            ViewBag.ListaCriados = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 1).ToList();
            ViewBag.ListaAguardando = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 2).ToList();
            ViewBag.ListaCanceladas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 3).ToList();
            ViewBag.ListaExecucoes = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 4).ToList();
            ViewBag.ListaEncerradas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 5).ToList();

            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Carrega listas
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Criado", Value = "1" });
            //status.Add(new SelectListItem() { Text = "Aguardando Informações", Value = "2" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "3" });
            status.Add(new SelectListItem() { Text = "Em execução", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Tipos = new SelectList(caApp.GetAllItens().OrderBy(x => x.CAAT_NM_NOME).ToList<CATEGORIA_ATENDIMENTO>(), "CAAT_CD_ID", "CAAT_NM_NOME");
            ViewBag.Departamentos = new SelectList(depApp.GetAllItens().OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");

            // Mensagem
            if ((Int32)Session["MensAtendimento"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensAtendimento"] = 0;
            }

            // Abre view
            Session["MensAtendimento"] = 0;
            SessionMocks.voltaAtendimento = 1;
            objeto = new ATENDIMENTO();
            if (SessionMocks.filtroAtendimento != null)
            {
                objeto = SessionMocks.filtroAtendimento;
            }
            else
            {
                objeto.ATEN_DT_INICIO = null;
            }
            return View(objeto);
        }

        [HttpPost]
        public JsonResult EditarStatusAtendimento(Int32 id, Int32 status, DateTime? dtEnc, String justificativa)
        {
            var aten = baseApp.GetById(id);
            aten.ATEN_IN_STATUS = status;

            if (status == 3)
            {
                var result = new Hashtable();
                if (dtEnc == null)
                {
                    result.Add("dt", "Campo DATA DE CANCELAMENTO é obrigatorio");
                }
                if (justificativa == null)
                {
                    result.Add("jus", "Campo JUSTIFICATIVA DE CANCELAMENTO é obrigatorio");
                }
            }

            var item = new ATENDIMENTO();
            item.ATEN_IN_STATUS = aten.ATEN_IN_STATUS;
            item.ATEN_CD_ID = aten.ATEN_CD_ID;
            item.ATEN_DS_DESCRICAO = aten.ATEN_DS_DESCRICAO;
            item.ASSI_CD_ID = aten.ASSI_CD_ID;
            if (justificativa != null)
            {
                item.ATEN_DS_CANCELAMENTO = justificativa;
            }
            else
            {
                item.ATEN_DS_CANCELAMENTO = aten.ATEN_DS_CANCELAMENTO;
            }
            item.ATEN_DS_ENCERRAMENTO = aten.ATEN_DS_ENCERRAMENTO;
            if (dtEnc != null)
            {
                item.ATEN_DT_CANCELAMENTO = dtEnc;
            }
            item.ATEN_DT_ENCERRAMENTO = aten.ATEN_DT_ENCERRAMENTO;
            item.ATEN_DT_INICIO = aten.ATEN_DT_INICIO;
            item.ATEN_DT_PREVISTA = aten.ATEN_DT_PREVISTA;
            item.ATEN_HR_CANCELAMENTO = aten.ATEN_HR_CANCELAMENTO;
            item.ATEN_HR_ENCERRAMENTO = aten.ATEN_HR_ENCERRAMENTO;
            item.ATEN_HR_INICIO = aten.ATEN_HR_INICIO;
            item.ATEN_IN_ATIVO = aten.ATEN_IN_ATIVO;
            item.ATEN_IN_DESTINO = aten.ATEN_IN_DESTINO;
            item.ATEN_IN_PRIORIDADE = aten.ATEN_IN_PRIORIDADE;
            item.ATEN_IN_TIPO = aten.ATEN_IN_TIPO;
            item.ATEN_NM_ASSUNTO = aten.ATEN_NM_ASSUNTO;
            item.ATEN_TX_OBSERVACOES = aten.ATEN_TX_OBSERVACOES;
            item.CAAT_CD_ID = aten.CAAT_CD_ID;
            item.CLIE_CD_ID = aten.CLIE_CD_ID;
            item.DEPT_CD_ID = aten.DEPT_CD_ID;
            item.PEVE_CD_ID = aten.PEVE_CD_ID;
            item.PROD_CD_ID = aten.PROD_CD_ID;
            item.SERV_CD_ID = aten.SERV_CD_ID;
            item.USUA_CD_ID = aten.USUA_CD_ID;

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    return Json(SystemBR_Resource.ResourceManager.GetString("M0095", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(SystemBR_Resource.ResourceManager.GetString("M0096", CultureInfo.CurrentCulture));
                }

                SessionMocks.listaAtendimento = null;

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetAtendimento()
        {
            var usuario = SessionMocks.UserCredentials;
            List<ATENDIMENTO> lstAt = new List<ATENDIMENTO>();

            // Carrega listas
            if (SessionMocks.listaAtendimento == null && SessionMocks.listaAtendimento.Count != 0)
            {
                listaMaster = baseApp.GetAllItens().Where(x => x.ATEN_IN_STATUS != 3).ToList<ATENDIMENTO>();
                SessionMocks.listaAtendimento = listaMaster;
            }

            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" && usuario.PERFIL.PERF_SG_SIGLA != "GER")
            {
                SessionMocks.listaAtendimento = SessionMocks.listaAtendimento.Where(p => p.USUA_CD_ID == SessionMocks.UserCredentials.USUA_CD_ID).ToList();
            }

            if (Session["FirstAccessAtendimento"] == null)
            {
                lstAt = SessionMocks.listaAtendimento.Where(x => x.ATEN_DT_PREVISTA.Value.Month == DateTime.Now.Month).ToList<ATENDIMENTO>();
                Session["FirstAccessAtendimento"] = 0;
            }
            else
            {
                lstAt = SessionMocks.listaAtendimento;
            } 

            var result = new List<Hashtable>();

            foreach (var item in lstAt)
            {
                var hash = new Hashtable();
                hash.Add("ATEN_IN_STATUS", item.ATEN_IN_STATUS);
                hash.Add("ATEN_CD_ID", item.ATEN_CD_ID);
                hash.Add("ATEN_NM_ASSUNTO", item.ATEN_NM_ASSUNTO);
                if (item.CLIENTE != null)
                {
                    hash.Add("CLIE_NM_NOME", item.CLIENTE.CLIE_NM_NOME);
                }
                hash.Add("ATEN_DT_INICIO", item.ATEN_DT_INICIO.Value.ToString("dd/MM/yyyy"));
                hash.Add("ATEN_DT_PREVISTA", item.ATEN_DT_PREVISTA == null ? " " : item.ATEN_DT_PREVISTA.Value.ToString("dd/MM/yyyy"));

                result.Add(hash);
            }

            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaAtendimentoKanban()
        {
            

            // Verifica se tem usuario logado
            SessionMocks.consultaAtendimento = 1;
            USUARIO usuario = new USUARIO();
            usuario = SessionMocks.UserCredentials;
            ViewBag.Title = "Tarefas";

            // Indicadores

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Criado", Value = "1" });
            status.Add(new SelectListItem() { Text = "Aguardando Informações", Value = "2" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "3" });
            status.Add(new SelectListItem() { Text = "Em execução", Value = "4" });
            status.Add(new SelectListItem() { Text = "Finalizado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            // Mensagem
            if ((Int32)Session["MensTarefa"] == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture);
            }

            // Abre view
            Session["MensTarefa"] = 0;
            objeto = new ATENDIMENTO();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult MontarTelaAtendimentoQuadro()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarAdmin", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaAtendimento == null)
            {
                listaMaster = baseApp.GetAllItens();
                listaMaster = listaMaster.Where(p => p.USUA_CD_ID == SessionMocks.UserCredentials.USUA_CD_ID).ToList();
                SessionMocks.listaAtendimento = listaMaster;
            }
            ViewBag.Listas = SessionMocks.listaAtendimento;
            ViewBag.Title = "Atendimentos";

            // Indicadores
            ViewBag.Atendimentos = SessionMocks.listaAtendimento.Count;
            ViewBag.Criados = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 1).ToList().Count;
            ViewBag.Aguardando = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 2).ToList().Count;
            ViewBag.Canceladas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 3).ToList().Count;
            ViewBag.Execucoes = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 4).ToList().Count;
            ViewBag.Encerradas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 5).ToList().Count;


            ViewBag.ListaCriados = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 1).ToList();
            ViewBag.ListaAguardando = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 2).ToList();
            ViewBag.ListaCanceladas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 3).ToList();
            ViewBag.ListaExecucoes = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 4).ToList();
            ViewBag.ListaEncerradas = SessionMocks.listaAtendimento.Where(p => p.ATEN_IN_STATUS == 5).ToList();

            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Carrega listas
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Criado", Value = "1" });
            status.Add(new SelectListItem() { Text = "Aguardando Informações", Value = "2" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "3" });
            status.Add(new SelectListItem() { Text = "Em execução", Value = "4" });
            status.Add(new SelectListItem() { Text = "Finalizado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Tipos = new SelectList(caApp.GetAllItens().OrderBy(x => x.CAAT_NM_NOME).ToList<CATEGORIA_ATENDIMENTO>(), "CAAT_CD_ID", "CAAT_NM_NOME");

            // Mensagem
            if ((Int32)Session["MensAtendimento"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensAtendimento"] = 0;
            }

            // Abre view
            Session["MensAtendimento"] = 0;
            SessionMocks.voltaAtendimento = 1;
            objeto = new ATENDIMENTO();
            objeto.ATEN_DT_INICIO = DateTime.Today.Date;
            objeto.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(objeto);
        }

        public ActionResult RetirarFiltroAtendimento()
        {
            
            SessionMocks.filtroAtendimento = null;
            SessionMocks.listaAtendimento = null;
            return RedirectToAction("MontarTelaAtendimento");
        }

        public ActionResult AbrirOrdemServicos()
        {
            
            SessionMocks.voltaOS = 4;
            return RedirectToAction("MontarTelaOrdemServicoFiltro", "OrdemServico", new { id = SessionMocks.idAtendimento });
        }

        public ActionResult MostrarTudoAtendimento()
        {
            
            USUARIO usuario = SessionMocks.UserCredentials;
            listaMaster = baseApp.GetAllItensAdm();
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" && usuario.PERFIL.PERF_SG_SIGLA != "GER")
            {
                listaMaster = listaMaster.Where(p => p.USUA_CD_ID == SessionMocks.UserCredentials.USUA_CD_ID).ToList();
            }
            SessionMocks.listaAtendimento = listaMaster;
            SessionMocks.filtroAtendimento = new ATENDIMENTO();
            return RedirectToAction("MontarTelaAtendimento");
        }

        public ActionResult MostrarTudoAtendimentoGerencia()
        {
            
            listaMaster = baseApp.GetAllItensAdm();
            SessionMocks.listaAtendimento = listaMaster;
            return RedirectToAction("MontarTelaAtendimento");
        }

        [HttpPost]
        public ActionResult FiltrarAtendimento(ATENDIMENTO item)
        {
            
            try
            {
                // Executa a operação
                SessionMocks.filtroAtendimento = item;
                List<ATENDIMENTO> listaObj = new List<ATENDIMENTO>();
                Int32 volta = baseApp.ExecuteFilter(item.CAAT_CD_ID, item.CLIE_CD_ID, item.PROD_CD_ID, item.ATEN_DT_PREVISTA, item.ATEN_IN_STATUS, item.ATEN_NM_ASSUNTO, item.DEPT_CD_ID, item.ATEN_IN_PRIORIDADE, item.USUA_CD_ID, item.SERV_CD_ID, out listaObj);

                if (item.ATEN_IN_STATUS == 0)
                {
                    listaObj = listaObj.Where(x => x.ATEN_IN_STATUS != 3).ToList();
                }

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAtendimento"] = 1;
                }

                // Sucesso
                Session["MensAtendimento"] = 0;
                listaMaster = listaObj;
                SessionMocks.listaAtendimento = listaObj;
                return RedirectToAction("MontarTelaAtendimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaAtendimento");

            }
        }

        public ActionResult VoltarBaseAtendimento()
        {
            
            if (SessionMocks.consultaAtendimento == 1)
            {
                return RedirectToAction("MontarTelaAtendimentoKanban");
            }
            if (SessionMocks.voltaNotificacao == 1)
            {
                SessionMocks.voltaNotificacao = null;
                return RedirectToAction("MontarTelaNotificacao", "Notificacao");
            }
            return RedirectToAction("MontarTelaAtendimento");
        }

        [HttpGet]
        public ActionResult IncluirAtendimento()
        {
            
            SessionMocks.voltaCliente = 6;

            // Prepara listas
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            SessionMocks.Clientes = cliApp.GetAllItens();
            ViewBag.Produtos = new SelectList(proApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Cats = new SelectList(caApp.GetAllItens().OrderBy(x => x.CAAT_NM_NOME).ToList<CATEGORIA_ATENDIMENTO>(), "CAAT_CD_ID", "CAAT_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens().OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(serApp.GetAllItens().OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");

            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");
            Session["VoltaCliente"] = 2;

            // Prepara view
            CONFIGURACAO conf = conApp.GetItemById(1);
            USUARIO usuario = SessionMocks.UserCredentials;
            ATENDIMENTO item = new ATENDIMENTO();
            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.ATEN_DT_INICIO = DateTime.Now.AddHours(-3);
            vm.ATEN_HR_INICIO = TimeSpan.Parse(DateTime.Now.AddHours(-3).ToString("hh:mm:ss"));
            vm.ATEN_HR_INICIO = DateTime.Now.TimeOfDay;
            vm.ATEN_IN_ATIVO = 1;
            vm.ATEN_IN_STATUS = 1;
            vm.ATEN_IN_DESTINO = SessionMocks.UserCredentials.USUA_CD_ID;
            vm.ATEN_DT_PREVISTA = DateTime.Today.Date.AddDays(conf.CONF_NR_DIAS_ATENDIMENTO.Value).AddHours(12);
            vm.USUA_CD_ID = SessionMocks.UserCredentials.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult IncluirAtendimento(AtendimentoViewModel vm)
        {
            
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Cats = new SelectList(caApp.GetAllItens().OrderBy(x => x.CAAT_NM_NOME).ToList<CATEGORIA_ATENDIMENTO>(), "CAAT_CD_ID", "CAAT_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens().OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(serApp.GetAllItens().OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");

            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");

            if (vm.PROD_CD_ID != null && vm.SERV_CD_ID != null)
            {
                ModelState.AddModelError("", "PRODUTO e SERVIÇO específicados, apenas um pode ser preenchido");
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ATENDIMENTO item = Mapper.Map<AtendimentoViewModel, ATENDIMENTO>(vm);
                    CATEGORIA_ATENDIMENTO ca = caApp.GetItemById(item.CAAT_CD_ID);
                    item.ATEN_DT_PREVISTA = DateTime.Now.AddDays((Int32)ca.CAAT_IN_SLA);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0071", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Atendimentos/" + item.ATEN_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    Session["MensAtendimento"] = 0;
                    listaMaster = new List<ATENDIMENTO>();
                    SessionMocks.listaAtendimento = null;

                    SessionMocks.idVolta = item.ATEN_CD_ID;
                    SessionMocks.idAtendimento = item.ATEN_CD_ID;

                    if (SessionMocks.voltaAtendimento == 2)
                    {
                        return RedirectToAction("MontarTelaAtendimento", new { id = item.ATEN_CD_ID });
                    }

                    if (Session["FileQueueAtendimento"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueAtendimento"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueAtendimento(file);
                        }

                        Session["FileQueueAtendimento"] = null;
                    }
                    return RedirectToAction("MontarTelaAtendimento");
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
        public ActionResult EditarAtendimento(Int32 id)
        {
            
            SessionMocks.idAtendimentoAgenda = null;
            SessionMocks.agenda = null;

            listaMaster = null;
            SessionMocks.listaAtendimento = null;

            // Prepara view
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Cats = new SelectList(caApp.GetAllItens().OrderBy(x => x.CAAT_NM_NOME).ToList<CATEGORIA_ATENDIMENTO>(), "CAAT_CD_ID", "CAAT_NM_NOME");
            ViewBag.CatsAgenda = new SelectList(agenApp.GetAllTipos().OrderBy(x => x.CAAG_NM_NOME).ToList<CATEGORIA_AGENDA>(), "CAAG_CD_ID", "CAAG_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens().OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(serApp.GetAllItens().OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (SessionMocks.voltaAcompanhamento)
            {
                ViewBag.DadosGerais = "";
                ViewBag.Acompanhamento = "active";
                SessionMocks.voltaAcompanhamento = false;
            }
            else
            {
                ViewBag.DadosGerais = "active";
                ViewBag.Acompanhamento = "";
            }

            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");

            ATENDIMENTO item = baseApp.GetItemById(id);

            ViewBag.CorSLA = DateTime.Now.Date <= item.ATEN_DT_PREVISTA.Value.Date ? "bg-primary" : "red-bg";
            ViewBag.MsgSLA = DateTime.Now.Date <= item.ATEN_DT_PREVISTA.Value.Date ? "Dentro do SLA" : "Fora do SLA";

            objetoAntes = item;
            SessionMocks.atendimento = item;
            SessionMocks.idVolta = id;
            SessionMocks.idAtendimento = id;
            SessionMocks.consultaAtendimento = 3;
            ViewBag.Status = (item.ATEN_IN_STATUS == 1 ? "Criação" : (item.ATEN_IN_STATUS == 2 ? "Suspensa" : (item.ATEN_IN_STATUS == 3 ? "Cancelado" : (item.ATEN_IN_STATUS == 4 ? "Ativado" : "Encerrado"))));
            ViewBag.Prior = (item.ATEN_IN_PRIORIDADE == 1 ? "Normal" : (item.ATEN_IN_PRIORIDADE == 2 ? "Baixa" : (item.ATEN_IN_PRIORIDADE == 3 ? "Alta" : "Urgente")));

            var listaAA = aAgenApp.GetAgendaByAtendimento(item);
            List<AGENDA> lstAgenda = new List<AGENDA>();
            foreach (var aa in listaAA)
            {
                lstAgenda.Add(agenApp.GetById(aa.AGEN_CD_ID));
            }
            ViewBag.Agendamentos = lstAgenda;

            if (Session["TarefaAtendimento"] != null)
            {
                if ((Int32)Session["TarefaAtendimento"] == 0)
                {
                    ViewBag.TarefaAgen = 1;
                }
                if ((Int32)Session["TarefaAtendimento"] == 1)
                {
                    ModelState.AddModelError("", "Tarefa já cadastrada");
                }
                if ((Int32)Session["TarefaAtendimento"] == 2)
                {
                    ModelState.AddModelError("", "Erro ao cadastrar tarefa");
                }
            }

            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            if (SessionMocks.errosAgendamento != null)
            {
                Hashtable erros = SessionMocks.errosAgendamento;

                if (erros["cadastro"] != null)
                {
                    ModelState.AddModelError("", erros["cadastro"].ToString());
                }
                else
                {
                    if (erros["categoria"] != null)
                    {
                        ModelState.AddModelError("", erros["categoria"].ToString());
                    }
                    if (erros["data"] != null)
                    {
                        ModelState.AddModelError("", erros["data"].ToString());
                    }
                    if (erros["hora"] != null)
                    {
                        ModelState.AddModelError("", erros["hora"].ToString());
                    }
                    if (erros["titulo"] != null)
                    {
                        ModelState.AddModelError("", erros["titulo"].ToString());
                    }
                }
            }

            if (Session["agenLista"] != null)
            {
                if ((Int32)Session["agenLista"] == 1)
                {
                    ViewBag.DadosGerais = "";
                    ViewBag.Acompanhamento = "";
                    ViewBag.AgenLista = "active";
                    Session["agenLista"] = 0;
                }
            }

            Session["idAtendimento"] = id;
            SessionMocks.voltaAtendimento = 1;
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarAgenda(Int32 id)
        {
            
            return RedirectToAction("EditarAgenda", "Agenda", new { id = id });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarAtendimento(AtendimentoViewModel vm)
        {
            
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Cats = new SelectList(caApp.GetAllItens().OrderBy(x => x.CAAT_NM_NOME).ToList<CATEGORIA_ATENDIMENTO>(), "CAAT_CD_ID", "CAAT_NM_NOME");
            ViewBag.CatsAgenda = new SelectList(agenApp.GetAllTipos().OrderBy(x => x.CAAG_NM_NOME).ToList<CATEGORIA_AGENDA>(), "CAAG_CD_ID", "CAAG_NM_NOME");
            ViewBag.Depts = new SelectList(depApp.GetAllItens().OrderBy(x => x.DEPT_NM_NOME).ToList<DEPARTAMENTO>(), "DEPT_CD_ID", "DEPT_NM_NOME");
            ViewBag.Servicos = new SelectList(serApp.GetAllItens().OrderBy(x => x.SERV_NM_NOME).ToList<SERVICO>(), "SERV_CD_ID", "SERV_NM_NOME");
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> prior = new List<SelectListItem>();
            prior.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            prior.Add(new SelectListItem() { Text = "Baixa", Value = "2" });
            prior.Add(new SelectListItem() { Text = "Alta", Value = "3" });
            prior.Add(new SelectListItem() { Text = "Urgente", Value = "4" });
            ViewBag.Prioridade = new SelectList(prior, "Value", "Text");

            ViewBag.CorSLA = DateTime.Now.Date <= vm.ATEN_DT_PREVISTA.Value.Date ? "bg-primary" : "red-bg";
            ViewBag.MsgSLA = DateTime.Now.Date <= vm.ATEN_DT_PREVISTA.Value.Date ? "Dentro do SLA" : "Fora do SLA";

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ATENDIMENTO item = Mapper.Map<AtendimentoViewModel, ATENDIMENTO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, SessionMocks.atendimento, usuarioLogado);

                    // Verifica retorno
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    Session["MensAtendimento"] = 0;
                    listaMaster = new List<ATENDIMENTO>();
                    SessionMocks.listaAtendimento = null;

                    if (SessionMocks.filtroAtendimento != null)
                    {
                        FiltrarAtendimento(SessionMocks.filtroAtendimento);
                    }

                    if (SessionMocks.voltaAtendimento == 2)
                    {
                        return RedirectToAction("MontarTelaAtendimentoQuadro");
                    }
                    if (SessionMocks.consultaAtendimento == 1)
                    {
                        return RedirectToAction("VoltarAnexoAtendimento");
                    }
                    return RedirectToAction("MontarTelaAtendimento");
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
        public ActionResult CancelarAtendimento(Int32? id)
        {
            
            // Prepara view
            ATENDIMENTO item = new ATENDIMENTO();
            if (id == null)
            {
                item = baseApp.GetItemById(SessionMocks.idVolta);
            }
            else
            {
                item = baseApp.GetItemById((Int32)id);
            }
            objetoAntes = item;
            SessionMocks.atendimento = item;
            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            vm.ATEN_DT_CANCELAMENTO = DateTime.Now;
            vm.ATEN_HR_CANCELAMENTO = DateTime.Now.TimeOfDay;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CancelarAtendimento(AtendimentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ATENDIMENTO item = Mapper.Map<AtendimentoViewModel, ATENDIMENTO>(vm);
                    item.ATEN_IN_STATUS = 3;
                    item.ATEN_IN_ATIVO = 0;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["MensAtendimento"] = 0;
                    //listaMaster = new List<ATENDIMENTO>();
                    //SessionMocks.listaAtendimento = null;
                    if (SessionMocks.voltaAtendimento == 2)
                    {
                        return RedirectToAction("EditarAtendimento", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaAtendimento");
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
        public ActionResult EncerrarAtendimento()
        {
            

            // Prepara view
            ATENDIMENTO item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoAntes = item;
            SessionMocks.atendimento = item;
            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            vm.ATEN_DT_ENCERRAMENTO = DateTime.Now;
            vm.ATEN_HR_ENCERRAMENTO = DateTime.Now.TimeOfDay;
            vm.ATEN_IN_STATUS = 5;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EncerrarAtendimento(AtendimentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ATENDIMENTO item = Mapper.Map<AtendimentoViewModel, ATENDIMENTO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    Session["MensAtendimento"] = 0;
                    listaMaster = new List<ATENDIMENTO>();
                    SessionMocks.listaAtendimento = null;
                    return RedirectToAction("MontarTelaAtendimento");
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

        public ActionResult AtivarAtendimento()
        {
            
            // Prepara view
            ATENDIMENTO item = baseApp.GetItemById(SessionMocks.idVolta);
            SessionMocks.atendimento = item;
            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            vm.ATEN_IN_STATUS = 4;
            return View(vm);
        }

        [HttpGet]
        public ActionResult VerAtendimento(Int32 id, Int32? volta)
        {
            
            // Volta para grid de notificação
            SessionMocks.voltaNotificacao = volta;

            // Prepara view
            ATENDIMENTO item = baseApp.GetItemById(id);
            SessionMocks.atendimento = item;
            SessionMocks.idVolta = id;
            SessionMocks.consultaAtendimento = 2;
            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            ViewBag.Status = (item.ATEN_IN_STATUS == 1 ? "Criação" : (item.ATEN_IN_STATUS == 2 ? "Suspensa" : (item.ATEN_IN_STATUS == 3 ? "Cancelado" : (item.ATEN_IN_STATUS == 4 ? "Ativado" : "Encerrado"))));
            ViewBag.Prior = (item.ATEN_IN_PRIORIDADE == 1 ? "Normal" : (item.ATEN_IN_PRIORIDADE == 2 ? "Baixa" : (item.ATEN_IN_PRIORIDADE == 3 ? "Alta" : "Urgente")));
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirAtendimento(Int32 id)
        {
            
            // Prepara view
            ATENDIMENTO item = baseApp.GetItemById(id);
            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExcluirAtendimento(AtendimentoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                ATENDIMENTO item = Mapper.Map<AtendimentoViewModel, ATENDIMENTO>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0072", CultureInfo.CurrentCulture));
                    Session["MensAtendimento"] = 1;
                    return View(vm);
                }

                // Sucesso
                Session["MensAtendimento"] = 0;
                listaMaster = new List<ATENDIMENTO>();
                SessionMocks.listaAtendimento = null;
                if (SessionMocks.voltaAtendimento == 2)
                {
                    return RedirectToAction("MontarTelaAtendimentoQuadro");
                }
                return RedirectToAction("MontarTelaAtendimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarAtendimento(Int32 id)
        {
            
            // Prepara view
            ATENDIMENTO item = baseApp.GetItemById(id);
            AtendimentoViewModel vm = Mapper.Map<ATENDIMENTO, AtendimentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReativarAtendimento(AtendimentoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                ATENDIMENTO item = Mapper.Map<AtendimentoViewModel, ATENDIMENTO>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<ATENDIMENTO>();
                SessionMocks.listaAtendimento = null;
                if (SessionMocks.voltaAtendimento == 2)
                {
                    return RedirectToAction("MontarTelaAtendimentoQuadro");
                }
                return RedirectToAction("MontarTelaAtendimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoAtendimento(Int32 id)
        {
            
            // Prepara view
            ATENDIMENTO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoAtendimento()
        {
            
            if (SessionMocks.consultaAtendimento == 3)
            {
                return RedirectToAction("EditarAtendimento", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaAtendimento");
        }

        public ActionResult VoltarConsultaAtendimento()
        {
            
            return RedirectToAction("VerAtendimento", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadAtendimento(Int32 id)
        {
            ATENDIMENTO_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.ATAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files)
        {
            List<FileQueue> queue = new List<FileQueue>();

            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                queue.Add(f);
            }

            Session["FileQueueAtendimento"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueAtendimento(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoAtendimento");
            }

            ATENDIMENTO item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;

            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTarefa");
            }

            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Atendimentos/" + item.ATEN_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            ATENDIMENTO_ANEXO foto = new ATENDIMENTO_ANEXO();
            foto.ATAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.ATAN_DT_ANEXO = DateTime.Today;
            foto.ATAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 2;
            }
            foto.ATAN_IN_TIPO = tipo;
            foto.ATAN_NM_TITULO = fileName;
            foto.ATEN_CD_ID = item.ATEN_CD_ID;

            item.ATENDIMENTO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoAtendimento");
        }

        [HttpPost]
        public ActionResult UploadFileAtendimento(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoAtendimento");
            }

            ATENDIMENTO item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);

            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTarefa");
            }

            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Atendimentos/" + item.ATEN_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            ATENDIMENTO_ANEXO foto = new ATENDIMENTO_ANEXO();
            foto.ATAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.ATAN_DT_ANEXO = DateTime.Today;
            foto.ATAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.ATAN_IN_TIPO = tipo;
            foto.ATAN_NM_TITULO = fileName;
            foto.ATEN_CD_ID = item.ATEN_CD_ID;

            item.ATENDIMENTO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoAtendimento");
        }

        public ActionResult IncluirAcompanhamento()
        {
            
            ATENDIMENTO item = baseApp.GetItemById(SessionMocks.idVolta);
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            ATENDIMENTO_ACOMPANHAMENTO coment = new ATENDIMENTO_ACOMPANHAMENTO();
            AtendimentoAcompanhamentoViewModel vm = Mapper.Map<ATENDIMENTO_ACOMPANHAMENTO, AtendimentoAcompanhamentoViewModel>(coment);
            vm.ATAC_DT_ACOMPANHAMENTO = DateTime.Now;
            vm.ATAC_IN_ATIVO = 1;
            vm.ATEN_CD_ID = item.ATEN_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcompanhamento(AtendimentoAcompanhamentoViewModel vm)
        {
            
            SessionMocks.voltaAcompanhamento = true;

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ATENDIMENTO_ACOMPANHAMENTO item = Mapper.Map<AtendimentoAcompanhamentoViewModel, ATENDIMENTO_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ATENDIMENTO not = baseApp.GetItemById(SessionMocks.idVolta);

                    item.USUARIO = null;
                    not.ATENDIMENTO_ACOMPANHAMENTO.Add(item);
                    not.ATEN_IN_STATUS = 4;
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarAtendimento", new { id = SessionMocks.idVolta });
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

        public ActionResult IncluirAcompanhamentoEncerrar()
        {
            
            ATENDIMENTO item = baseApp.GetItemById(SessionMocks.idVolta);
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            ATENDIMENTO_ACOMPANHAMENTO coment = new ATENDIMENTO_ACOMPANHAMENTO();
            AtendimentoAcompanhamentoViewModel vm = Mapper.Map<ATENDIMENTO_ACOMPANHAMENTO, AtendimentoAcompanhamentoViewModel>(coment);
            vm.ATAC_DT_ACOMPANHAMENTO = DateTime.Today;
            vm.ATAC_IN_ATIVO = 1;
            vm.ATAC_DT_ENCERRAMENTO = DateTime.Now;
            vm.ATEN_CD_ID = item.ATEN_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcompanhamentoEncerrar(AtendimentoAcompanhamentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ATENDIMENTO_ACOMPANHAMENTO item = Mapper.Map<AtendimentoAcompanhamentoViewModel, ATENDIMENTO_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ATENDIMENTO not = baseApp.GetItemById(SessionMocks.idVolta);

                    item.USUARIO = null;
                    not.ATENDIMENTO_ACOMPANHAMENTO.Add(item);
                    not.ATEN_IN_STATUS = 5;
                    not.ATEN_DT_ENCERRAMENTO = item.ATAC_DT_ENCERRAMENTO;
                    not.ATEN_DS_ENCERRAMENTO = item.ATAC_DS_ENCERRAMENTO;
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarAtendimento", new { id = SessionMocks.idVolta });
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

        public ActionResult GerarOSAtendimento()
        {
            
            return RedirectToAction("IncluirOrdemServico", "OrdemServico", new { id = SessionMocks.idAtendimento });
        }

        public ActionResult AbrirOrdemServico()
        {
            
            return RedirectToAction("MontarTelaOrdemServicoFiltro", "OrdemServico", new { id = SessionMocks.idAtendimento });
        }

        public ActionResult GerarTarefaAtendimento(String cliente)
        {
            
            TAREFA tarefa = new TAREFA();
            tarefa.ASSI_CD_ID = SessionMocks.IdAssinante;
            tarefa.TARE_DS_DESCRICAO = "Atendimento " + SessionMocks.atendimento.ATEN_CD_ID.ToString() + ". Assunto: " + SessionMocks.atendimento.ATEN_NM_ASSUNTO;
            tarefa.TARE_DT_CADASTRO = DateTime.Today.Date;
            tarefa.TARE_DT_ESTIMADA = DateTime.Today.Date.AddDays(30);
            tarefa.TARE_IN_ATIVO = 1;
            tarefa.TARE_IN_AVISA = 1;
            tarefa.TARE_IN_PRIORIDADE = 1;
            tarefa.TARE_IN_STATUS = 1;
            tarefa.TARE_NM_TITULO = "Atendimento " + SessionMocks.atendimento.ATEN_CD_ID.ToString() + " - " + cliente;
            tarefa.TITR_CD_ID = 1;
            tarefa.USUA_CD_ID = SessionMocks.UserCredentials.USUA_CD_ID;
            Int32 volta = tarApp.ValidateCreate(tarefa, SessionMocks.UserCredentials);

            // Cria pastas
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Tarefas/" + tarefa.TARE_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));

            if (volta == 0)
            {
                Session["TarefaAtendimento"] = 0;
            }
            else if (volta == 1)
            {
                Session["TarefaAtendimento"] = 1;
            }
            else if (volta == 2)
            {
                Session["TarefaAtendimento"] = 2;
            }

            return RedirectToAction("VoltarAnexoAtendimento");
        }

        public ActionResult GerarRelatorioResumo()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AtendimentoResumo" + "_" + data + ".pdf";
            List<ATENDIMENTO> lista = SessionMocks.listaAtendimento;
            ATENDIMENTO filtro = SessionMocks.filtroAtendimento;
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Atendimentos - Resumo", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 70f, 100f, 100f, 100f, 50f, 50f, 50f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Atendimentos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Produto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Assunto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Início", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Hora", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Status", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (ATENDIMENTO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_ATENDIMENTO.CAAT_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.PRODUTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.ATEN_NM_ASSUNTO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.ATEN_DT_INICIO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ATEN_DT_INICIO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.ATEN_HR_INICIO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.ATEN_HR_INICIO.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.ATEN_IN_STATUS == 1)
                {
                    cell = new PdfPCell(new Paragraph("Criado", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.ATEN_IN_STATUS == 2)
                {
                    cell = new PdfPCell(new Paragraph("Aguardando", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.ATEN_IN_STATUS == 3)
                {
                    cell = new PdfPCell(new Paragraph("Cancelado", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.ATEN_IN_STATUS == 4)
                {
                    cell = new PdfPCell(new Paragraph("Executando", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.ATEN_IN_STATUS == 5)
                {
                    cell = new PdfPCell(new Paragraph("Encerrado", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CAAT_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CAAT_CD_ID;
                    ja = 1;
                }
                if (filtro.CLIE_CD_ID > 0)
                {
                    CLIENTE cli = cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    if (ja == 0)
                    {
                        parametros += "Cliente: " + cli.CLIE_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Cliente: " + cli.CLIE_NM_NOME;
                    }
                }
                if (filtro.PROD_CD_ID > 0)
                {
                    PRODUTO pro = proApp.GetItemById(filtro.PROD_CD_ID.Value);
                    if (ja == 0)
                    {
                        parametros += "Produto: " + pro.PROD_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Produto: " + pro.PROD_NM_NOME;
                    }
                }
                if (filtro.ATEN_DT_INICIO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Início: " + filtro.ATEN_DT_INICIO.Value.ToShortDateString();
                        ja = 1;

                    }
                    else
                    {
                        parametros += " e CNPJ: " + filtro.ATEN_DT_INICIO.Value.ToShortDateString();
                    }
                }
                if (filtro.ATEN_NM_ASSUNTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Assunto: " + filtro.ATEN_NM_ASSUNTO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Assunto: " + filtro.ATEN_NM_ASSUNTO;
                    }
                }
                if (filtro.ATEN_IN_STATUS > 0)
                {
                    String status = filtro.ATEN_IN_STATUS == 1 ? "Criado" : filtro.ATEN_IN_STATUS == 2 ? "Aguardando" : filtro.ATEN_IN_STATUS == 3 ? "Cancelado" : filtro.ATEN_IN_STATUS == 4 ? "Executando" : "Encerrado";
                    if (ja == 0)
                    {
                        parametros += "Status: " + status;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Status: " + status;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaAtendimento");
        }

        public ActionResult GerarRelatorioAtendimento()
        {
            
            // Prepara geração
            ATENDIMENTO aten = baseApp.GetItemById(SessionMocks.idAtendimento);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Atendimento_" + aten.ATEN_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Atendimento - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            if (aten.ATEN_IN_STATUS == 1)
            {
                cell = new PdfPCell(new Paragraph("Status: Criado", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (aten.ATEN_IN_STATUS == 2)
            {
                cell = new PdfPCell(new Paragraph("Status: Aguardando", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (aten.ATEN_IN_STATUS == 3)
            {
                cell = new PdfPCell(new Paragraph("Status: Cancelado", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (aten.ATEN_IN_STATUS == 4)
            {
                cell = new PdfPCell(new Paragraph("Status: Em Execução", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (aten.ATEN_IN_STATUS == 5)
            {
                cell = new PdfPCell(new Paragraph("Status: Encerrado", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_ATENDIMENTO.CAAT_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Responsável: " + aten.USUARIO.USUA_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Departamento: " + aten.DEPARTAMENTO.DEPT_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente: " + aten.CLIENTE.CLIE_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.PRODUTO != null)
            {
                cell = new PdfPCell(new Paragraph("Produto: " + aten.PRODUTO.PROD_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Produto: Não especificado", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);


            cell = new PdfPCell(new Paragraph("Data: " + aten.ATEN_DT_INICIO.Value.ToShortDateString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Hora: " + aten.ATEN_HR_INICIO.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.ATEN_DT_PREVISTA == null)
            {
                cell = new PdfPCell(new Paragraph("Data Prevista: Não especificada", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Prevista: " + aten.ATEN_DT_PREVISTA.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.ATEN_IN_TIPO == 1)
            {
                cell = new PdfPCell(new Paragraph("Tipo: Interno" + aten.PRODUTO.PROD_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Tipo: Externo", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Prioridade: " + aten.ATEN_IN_PRIORIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Assunto: " + aten.ATEN_NM_ASSUNTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Descrição: " + aten.ATEN_DS_DESCRICAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;   
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Cancelamento
            if (aten.ATEN_IN_STATUS == 3)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data Cancelamento: " + aten.ATEN_DT_CANCELAMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Hore Cancelamento: " + aten.ATEN_HR_CANCELAMENTO.Value.ToString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Justificativa: " + aten.ATEN_DS_CANCELAMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }

            // Encerramento
            if (aten.ATEN_IN_STATUS == 5)
            {
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data Encerramento: " + aten.ATEN_DT_ENCERRAMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Hore Encerramemto: " + aten.ATEN_HR_ENCERRAMENTO.Value.ToString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Justificativa: " + aten.ATEN_DS_ENCERRAMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observacoes
            Chunk chunk1 = new Chunk("Observações: " + aten.ATEN_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoAtendimento");
        }



    }
}