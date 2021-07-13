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
using System.Data.Entity;

namespace SystemBRPresentation.Controllers
{
    public class FinanceiroController : Controller
    {
        private readonly IContaReceberAppService crApp;
        private readonly IClienteAppService cliApp;
        private readonly ILogAppService logApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IContaPagarAppService cpApp;
        private readonly IFornecedorAppService forApp;
        private readonly IPlanoContaAppService plaApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IContratoAppService ctApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly IFormaPagamentoAppService fpApp;
        private readonly IPeriodicidadeAppService perApp;
        private readonly IContaReceberParcelaAppService pcApp;
        private readonly IContaPagarParcelaAppService ppApp;

        private String msg;
        private Exception exception;
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao = String.Empty;
        CONTA_RECEBER objetoCR = new CONTA_RECEBER();
        CONTA_RECEBER objetoCRAntes = new CONTA_RECEBER();
        List<CONTA_RECEBER> listaCRMaster = new List<CONTA_RECEBER>();
        CONTA_PAGAR objetoCP = new CONTA_PAGAR();
        CONTA_PAGAR objetoCPAntes = new CONTA_PAGAR();
        List<CONTA_PAGAR> listaCPMaster = new List<CONTA_PAGAR>();
        CONTA_RECEBER_PARCELA objetoCRP = new CONTA_RECEBER_PARCELA();
        CONTA_RECEBER_PARCELA objetoCRPAntes = new CONTA_RECEBER_PARCELA();
        List<CONTA_RECEBER_PARCELA> listaCRPMaster = new List<CONTA_RECEBER_PARCELA>();
        CONTA_PAGAR_PARCELA objetoCPP = new CONTA_PAGAR_PARCELA();
        CONTA_PAGAR_PARCELA objetoCPPAntes = new CONTA_PAGAR_PARCELA();
        List<CONTA_PAGAR_PARCELA> listaCPPMaster = new List<CONTA_PAGAR_PARCELA>();
        CONTA_BANCARIA contaPadrao = new CONTA_BANCARIA();

        public FinanceiroController(IContaReceberAppService crApps, ILogAppService logApps, IMatrizAppService matrizApps, IClienteAppService cliApps, IContaPagarAppService cpApps, IFornecedorAppService forApps, IPlanoContaAppService plaApps, ICentroCustoAppService ccApps, IContratoAppService ctApps, IContaBancariaAppService cbApps, IFormaPagamentoAppService fpApps, IPeriodicidadeAppService perApps, IContaReceberParcelaAppService pcApps, IContaPagarParcelaAppService ppApps)
        {
            logApp = logApps;
            matrizApp = matrizApps;
            crApp = crApps;
            cliApp = cliApps;
            cpApp = cpApps;
            forApp = forApps;
            plaApp = plaApps;
            ccApp = ccApps;
            ctApp = ctApps;
            cbApp = cbApps;
            fpApp = fpApps;
            perApp = perApps;
            pcApp = pcApps;
            ppApp = ppApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("CarregarFinanceiro", "BaseAdmin");
        }

        public ActionResult Voltar()
        {
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            listaCPMaster = new List<CONTA_PAGAR>();
            SessionMocks.listaCP = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        public ActionResult VoltarDashboard()
        {
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            listaCPMaster = new List<CONTA_PAGAR>();
            SessionMocks.listaCP = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaCR()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaCR == null)
            {
                listaCRMaster = crApp.GetAllItens();
                SessionMocks.listaCR = listaCRMaster;
            }
            ViewBag.Listas = SessionMocks.listaCR;
            ViewBag.Title = "Contas a Receber";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");

            // Indicadores
            ViewBag.CRS = listaCRMaster.Count;
            ViewBag.Recebido = crApp.GetTotalRecebimentosMes(DateTime.Now.Date);
            ViewBag.AReceber = crApp.GetTotalAReceberMes(DateTime.Now.Date);
            ViewBag.Atrasos = crApp.GetItensAtrasoCliente().Sum(x => x.CARE_VL_VALOR);
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objetoCR = new CONTA_RECEBER();
            objetoCR.CARE_DT_LANCAMENTO = DateTime.Now.Date;
            //if (SessionMocks.filtroCR != null)
            //{
            //    objetoCR = SessionMocks.filtroCR;
            //}
            return View(objetoCR);
        }

        public ActionResult RetirarFiltroCR()
        {
            SessionMocks.listaCR = null;
            SessionMocks.filtroCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        public ActionResult MostrarTudoCR()
        {
            listaCRMaster = crApp.GetAllItensAdm();
            SessionMocks.filtroCR = null;
            SessionMocks.listaCR = listaCRMaster;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpPost]
        public ActionResult FiltrarCR(CONTA_RECEBER item)
        {
            try
            {
                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                SessionMocks.filtroCR = item;
                Int32 volta = crApp.ExecuteFilter(item.CLIE_CD_ID, item.CARE_DT_LANCAMENTO, item.CARE_DS_DESCRICAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaCRMaster = listaObj;
                SessionMocks.listaCR = listaObj;
                return RedirectToAction("MontarTelaCR");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCR");
            }
        }

        public ActionResult VoltarBaseCR()
        {
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult MontarTelaCP()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaCP == null)
            {
                listaCPMaster = cpApp.GetAllItens();
                SessionMocks.listaCP = listaCPMaster;
            }
            ViewBag.Listas = SessionMocks.listaCP;
            ViewBag.Title = "Contas a Pagar";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Forn = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");

            // Indicadores
            ViewBag.CPS = listaCPMaster.Count;
            ViewBag.Pago = cpApp.GetTotalPagoMes(DateTime.Now.Date);
            ViewBag.APagar = cpApp.GetTotalAPagarMes(DateTime.Now.Date);
            ViewBag.Atrasos = cpApp.GetItensAtraso().Sum(x => x.CAPA_VL_VALOR);
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objetoCP = new CONTA_PAGAR();
            objetoCP.CAPA_DT_LANCAMENTO = DateTime.Now.Date;
            return View(objetoCP);
        }

        public ActionResult RetirarFiltroCP()
        {
            SessionMocks.listaCP = null;
            SessionMocks.filtroCP = null;
            return RedirectToAction("MontarTelaCP");
        }

        public ActionResult MostrarTudoCP()
        {
            listaCPMaster = cpApp.GetAllItensAdm();
            SessionMocks.filtroCP = null;
            SessionMocks.listaCP = listaCPMaster;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpPost]
        public ActionResult FiltrarCP(CONTA_PAGAR item)
        {
            try
            {
                // Executa a operação
                List<CONTA_PAGAR> listaObj = new List<CONTA_PAGAR>();
                SessionMocks.filtroCP = item;
                Int32 volta = cpApp.ExecuteFilter(item.FORN_CD_ID, item.CAPA_DT_LANCAMENTO, item.CAPA_DS_DESCRICAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaCPMaster = listaObj;
                SessionMocks.listaCP = listaObj;
                return RedirectToAction("MontarTelaCP");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCP");
            }
        }

        public ActionResult VoltarBaseCP()
        {
            listaCPMaster = new List<CONTA_PAGAR>();
            SessionMocks.listaCP = null;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult VerRecebimentosMes()
        {
            List<CONTA_RECEBER> lista = crApp.GetRecebimentosMes(DateTime.Now.Date);
            ViewBag.ListaCR = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CARE_VL_VALOR_LIQUIDADO);
            return View();
        }

        [HttpGet]
        public ActionResult VerPagamentosMes()
        {
            List<CONTA_PAGAR> lista = cpApp.GetPagamentosMes(DateTime.Now.Date);
            ViewBag.ListaCP = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CAPA_VL_VALOR_PAGO);
            return View();
        }

        public ActionResult VoltarBaseCaixa()
        {
            return RedirectToAction("MontarTelaCaixa");
        }

        [HttpGet]
        public ActionResult VerAReceberMes()
        {
            List<CONTA_RECEBER> lista = crApp.GetAReceberMes(DateTime.Now.Date);
            ViewBag.ListaCR = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CARE_VL_VALOR);
            return View();
        }

        [HttpGet]
        public ActionResult VerAPagarMes()
        {
            List<CONTA_PAGAR> lista = cpApp.GetAPagarMes(DateTime.Now.Date);
            ViewBag.ListaCP = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CAPA_VL_VALOR);
            return View();
        }

        [HttpGet]
        public ActionResult VerLancamentosAtraso()
        {
            List<CONTA_RECEBER> lista = crApp.GetItensAtrasoCliente();
            ViewBag.ListaCR = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CARE_VL_VALOR);
            return View();
        }

        [HttpGet]
        public ActionResult VerLancamentosAtrasoCP()
        {
            List<CONTA_PAGAR> lista = cpApp.GetItensAtrasoFornecedor();
            ViewBag.ListaCP = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CAPA_VL_VALOR);
            return View();
        }

        [HttpGet]
        public ActionResult VerCR(Int32 id)
        {
            // Prepara view
            CONTA_RECEBER item = crApp.GetItemById(id);
            SessionMocks.contaReceber = item;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            SessionMocks.idVolta = id;
            SessionMocks.idCRVolta = 1;
            return View(vm);
        }

        [HttpGet]
        public ActionResult VerCP(Int32 id)
        {
            // Prepara view
            CONTA_PAGAR item = cpApp.GetItemById(id);
            SessionMocks.contaPagar = item;
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
            SessionMocks.idVolta = id;
            SessionMocks.idCRVolta = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadFileLancamentoCR(HttpPostedFileBase file)
        {
            if (file == null)
                return Content("Nenhum arquivo selecionado");

            CONTA_RECEBER item = crApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Financeiro/CR/" + item.CARE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_RECEBER_ANEXO foto = new CONTA_RECEBER_ANEXO();
            foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRAN_DT_ANEXO = DateTime.Today;
            foto.CRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.CRAN_IN_TIPO = tipo;
            foto.CRAN_NM_TITULO = fileName;
            foto.CARE_CD_ID = item.CARE_CD_ID;

            item.CONTA_RECEBER_ANEXO.Add(foto);
            objetoCRAntes = item;
            Int32 volta = crApp.ValidateEdit(item, objetoCRAntes, usu);
            if (SessionMocks.idCRVolta == 1)
            {
                return RedirectToAction("VerCR");
            }
            return RedirectToAction("EditarCR");
        }

        [HttpPost]
        public ActionResult UploadFileLancamentoCP(HttpPostedFileBase file)
        {
            if (file == null)
                return Content("Nenhum arquivo selecionado");

            CONTA_PAGAR item = cpApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Financeiro/CP/" + item.CAPA_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_PAGAR_ANEXO foto = new CONTA_PAGAR_ANEXO();
            foto.CPAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CPAN_DT_ANEXO = DateTime.Today;
            foto.CPAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.CPAN_IN_TIPO = tipo;
            foto.CPAN_NM_TITULO = fileName;
            foto.CAPA_CD_ID = item.CAPA_CD_ID;

            item.CONTA_PAGAR_ANEXO.Add(foto);
            objetoCPAntes = item;
            Int32 volta = cpApp.ValidateEdit(item, objetoCPAntes, usu);
            if (SessionMocks.idCRVolta == 1)
            {
                return RedirectToAction("VerCP");
            }
            return RedirectToAction("EditarCP");
        }

        [HttpGet]
        public ActionResult VerAnexoLancamentoCR(Int32 id)
        {
            // Prepara view
            CONTA_RECEBER_ANEXO item = crApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoLancamentoCP(Int32 id)
        {
            // Prepara view
            CONTA_PAGAR_ANEXO item = cpApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadLancamentoCR(Int32 id)
        {
            CONTA_RECEBER_ANEXO item = crApp.GetAnexoById(id);
            String arquivo = item.CRAN_AQ_ARQUIVO;
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

        public FileResult DownloadLancamentoCP(Int32 id)
        {
            CONTA_PAGAR_ANEXO item = cpApp.GetAnexoById(id);
            String arquivo = item.CPAN_AQ_ARQUIVO;
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

        [HttpGet]
        public ActionResult SlideShowCR()
        {
            // Prepara view
            CONTA_RECEBER item = crApp.GetItemById(SessionMocks.idVolta);
            objetoCRAntes = item;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SlideShowCP()
        {
            // Prepara view
            CONTA_PAGAR item = cpApp.GetItemById(SessionMocks.idVolta);
            objetoCPAntes = item;
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirCR(Int32 id)
        {
            USUARIO usu = SessionMocks.UserCredentials;
            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = SessionMocks.contaReceber;
            item.CARE_IN_ATIVO = 0;
            Int32 volta = crApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture);
            }
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult ExcluirCP(Int32 id)
        {
            USUARIO usu = SessionMocks.UserCredentials;
            CONTA_PAGAR item = cpApp.GetItemById(id);
            objetoCPAntes = SessionMocks.contaPagar;
            item.CAPA_IN_ATIVO = 0;
            Int32 volta = cpApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture);
            }
            listaCPMaster = new List<CONTA_PAGAR>();
            SessionMocks.listaCP = null;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult ReativarCR(Int32 id)
        {
            USUARIO usu = SessionMocks.UserCredentials;
            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = SessionMocks.contaReceber;
            item.CARE_IN_ATIVO = 1;
            Int32 volta = crApp.ValidateReativar(item, usu);
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult ReativarCP(Int32 id)
        {
            USUARIO usu = SessionMocks.UserCredentials;
            CONTA_PAGAR item = cpApp.GetItemById(id);
            objetoCPAntes = SessionMocks.contaPagar;
            item.CAPA_IN_ATIVO = 1;
            Int32 volta = cpApp.ValidateReativar(item, usu);
            listaCPMaster = new List<CONTA_PAGAR>();
            SessionMocks.listaCP = null;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult VerParcelaCR(Int32 id)
        {
            // Prepara view
            CONTA_RECEBER_PARCELA item = crApp.GetParcelaById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerParcelaCP(Int32 id)
        {
            // Prepara view
            CONTA_PAGAR_PARCELA item = cpApp.GetParcelaById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoVerCR()
        {
            return RedirectToAction("VerCR", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarAnexoVerCP()
        {
            return RedirectToAction("VerCP", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarAnexoCR()
        {
            return RedirectToAction("EditarCR", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarAnexoCP()
        {
            return RedirectToAction("EditarCP", new { id = SessionMocks.idVolta });
        }

        [HttpGet]
        public ActionResult IncluirCR()
        {
            // Prepara listas
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTA_RECEBER item = new CONTA_RECEBER();
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante.Value;
            vm.CARE_DT_LANCAMENTO = DateTime.Today.Date;
            vm.CARE_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.FILI_CD_ID = usuario.COLABORADOR.FILI_CD_ID;
            vm.CARE_DT_COMPETENCIA = DateTime.Today.Date;
            vm.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            vm.CARE_IN_LIQUIDADA = 0;
            vm.CARE_IN_PAGA_PARCIAL = 0;
            vm.CARE_IN_PARCELADA = 0;
            vm.CARE_IN_PARCELAS = 0;
            vm.CARE_VL_SALDO = 0;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCR(ContaReceberViewModel vm)
        {
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = crApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Financeiro/CR/" + item.CARE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;
                    return RedirectToAction("MontarTelaCR");
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
        public ActionResult IncluirCP()
        {
            // Prepara listas
            ViewBag.Forn = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTA_PAGAR item = new CONTA_PAGAR();
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante.Value;
            vm.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
            vm.CAPA_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.FILI_CD_ID = usuario.COLABORADOR.FILI_CD_ID;
            vm.CAPA_DT_COMPETENCIA = DateTime.Today.Date;
            vm.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            vm.CAPA_IN_LIQUIDADA = 0;
            vm.CAPA_IN_PAGA_PARCIAL = 0;
            vm.CAPA_IN_PARCELADA = 0;
            vm.CAPA_IN_PARCELAS = 0;
            vm.CAPA_VL_SALDO = 0;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCP(ContaPagarViewModel vm)
        {
            ViewBag.Forn = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_PAGAR item = Mapper.Map<ContaPagarViewModel, CONTA_PAGAR>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = cpApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Financeiro/CP/" + item.CAPA_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaCPMaster = new List<CONTA_PAGAR>();
                    SessionMocks.listaCP = null;
                    return RedirectToAction("MontarTelaCP");
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
        public ActionResult EditarCR(Int32 id)
        {
            // Prepara view
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 0;
            SessionMocks.liquidaCR = 0;

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            SessionMocks.contaReceber = item;
            SessionMocks.idVolta = id;
            SessionMocks.idCRVolta = 2;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            if (vm.CARE_IN_PAGA_PARCIAL == 1)
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_SALDO;
            }
            else
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_VALOR;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCR(ContaReceberViewModel vm)
        {
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 0;
            SessionMocks.liquidaCR = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    Int32 volta = crApp.ValidateEdit(item, SessionMocks.contaReceber, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0049", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0050", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 7)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 8)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;
                    return RedirectToAction("MontarTelaCR");
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
        public ActionResult EditarCP(Int32 id)
        {
            // Prepara view
            ViewBag.Forn = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 0;
            SessionMocks.liquidaCP = 0;

            CONTA_PAGAR item = cpApp.GetItemById(id);
            objetoCPAntes = item;
            SessionMocks.contaPagar = item;
            SessionMocks.idVolta = id;
            SessionMocks.idCRVolta = 2;
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
            vm.CAPA_VL_PARCELADO = vm.CAPA_VL_VALOR;
            if (vm.CAPA_IN_PAGA_PARCIAL == 1)
            {
                vm.CAPA_VL_VALOR_PAGO = vm.CAPA_VL_SALDO;
            }
            else
            {
                vm.CAPA_VL_VALOR_PAGO = vm.CAPA_VL_VALOR;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCP(ContaPagarViewModel vm)
        {
            ViewBag.Forn = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 0;
            SessionMocks.liquidaCP = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_PAGAR item = Mapper.Map<ContaPagarViewModel, CONTA_PAGAR>(vm);
                    SessionMocks.eParcela = 0;
                    Int32 volta = cpApp.ValidateEdit(item, SessionMocks.contaPagar, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0049", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0050", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 7)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 8)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Sucesso
                    listaCPMaster = new List<CONTA_PAGAR>();
                    SessionMocks.listaCP = null;
                    return RedirectToAction("MontarTelaCP");
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
        public ActionResult ParcelarCR(Int32 id)
        {
            // Prepara view
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            SessionMocks.contaReceber = item;
            SessionMocks.idVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_DT_INICIO_PARCELA = DateTime.Today.Date;
            vm.CARE_IN_PARCELADA = 1;
            vm.CARE_IN_PARCELAS = 2;
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParcelarCR(ContaReceberViewModel vm)
        {
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            //if (ModelState.IsValid)
            //{
                try
                {
                    // Processa parcelas
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    DateTime dataParcela = item.CARE_DT_INICIO_PARCELA.Value;
                    PERIODICIDADE period = perApp.GetItemById(item.PERI_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= item.CARE_IN_PARCELAS; i++)
                    {
                        CONTA_RECEBER_PARCELA parc = new CONTA_RECEBER_PARCELA();
                        parc.CARE_CD_ID = item.CARE_CD_ID;
                        parc.CRPA_DT_QUITACAO = null;
                        parc.CRPA_DT_VENCIMENTO = dataParcela;
                        parc.CRPA_IN_ATIVO = 1;
                        parc.CRPA_IN_QUITADA = 0;
                        parc.CRPA_NR_PARCELA = i.ToString() + "/" + item.CARE_IN_PARCELAS.Value.ToString();
                        parc.CRPA_VL_RECEBIDO = 0;
                        parc.CRPA_VL_VALOR = item.CARE_VL_PARCELADO;
                        item.CONTA_RECEBER_PARCELA.Add(parc);
                        dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                    }

                    item.CARE_IN_PARCELADA = 1;
                    objetoCRAntes = item;
                    Int32 volta = crApp.ValidateEdit(item, objetoCRAntes, usuarioLogado);
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;
                    return RedirectToAction("MontarTelaCR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            //}
            //else
            //{
            //    return View(vm);
            //}
        }

        public ActionResult DuplicarCR()
        {
            // Monta novo lançamento
            CONTA_RECEBER item = crApp.GetItemById(SessionMocks.idVolta);
            CONTA_RECEBER novo = new CONTA_RECEBER();
            novo.ASSI_CD_ID = SessionMocks.IdAssinante.Value;
            novo.CARE_DS_DESCRICAO = "Lançamento Duplicado - " + item.CARE_DS_DESCRICAO;
            novo.CARE_DT_COMPETENCIA = item.CARE_DT_COMPETENCIA;
            novo.CARE_DT_LANCAMENTO = DateTime.Today.Date;
            novo.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            novo.CARE_IN_ATIVO = 1;
            novo.CARE_IN_LIQUIDADA = 0;
            novo.CARE_IN_PAGA_PARCIAL = 0;
            novo.CARE_IN_PARCELADA = 0;
            novo.CARE_IN_PARCELAS = 0;
            novo.CARE_IN_TIPO_LANCAMENTO = 0;
            novo.CARE_NM_FAVORECIDO = item.CARE_NM_FAVORECIDO;
            novo.CARE_NR_DOCUMENTO = item.CARE_NR_DOCUMENTO;
            novo.CARE_TX_OBSERVACOES = item.CARE_TX_OBSERVACOES;
            novo.CARE_VL_DESCONTO = 0;
            novo.CARE_VL_JUROS = 0;
            novo.CARE_VL_PARCELADO = 0;
            novo.CARE_VL_PARCIAL = 0;
            novo.CARE_VL_SALDO = 0;
            novo.CARE_VL_TAXAS = 0;
            novo.CARE_VL_VALOR = item.CARE_VL_VALOR;
            novo.CARE_VL_VALOR_LIQUIDADO = 0;
            novo.CECU_CD_ID = item.CECU_CD_ID;
            novo.CLIE_CD_ID = item.CLIE_CD_ID;
            novo.COBA_CD_ID = item.COBA_CD_ID;
            novo.COLA_CD_ID = item.COLA_CD_ID;
            novo.CONT_CD_ID = item.CONT_CD_ID;
            novo.FILI_CD_ID = item.FILI_CD_ID;
            novo.FOPA_CD_ID = item.FOPA_CD_ID;
            novo.MATR_CD_ID = item.MATR_CD_ID;
            novo.PERI_CD_ID = item.PERI_CD_ID;
            novo.PEVE_CD_ID = item.PEVE_CD_ID;
            novo.PLCO_CD_ID = item.PLCO_CD_ID;
            novo.TIFA_CD_ID = item.TIFA_CD_ID;
            novo.USUA_CD_ID = item.USUA_CD_ID;

            // Grava
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            Int32 volta = crApp.ValidateCreate(novo, usuarioLogado);

            // Cria pastas
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Financeiro/CR/" + novo.CARE_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));

            // Sucesso
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        public ActionResult DuplicarCP()
        {
            // Monta novo lançamento
            CONTA_PAGAR item = cpApp.GetItemById(SessionMocks.idVolta);
            CONTA_PAGAR novo = new CONTA_PAGAR();
            novo.ASSI_CD_ID = SessionMocks.IdAssinante.Value;
            novo.CAPA_DS_DESCRICAO = "Lançamento Duplicado - " + item.CAPA_DS_DESCRICAO;
            novo.CAPA_DT_COMPETENCIA = item.CAPA_DT_COMPETENCIA;
            novo.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
            novo.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            novo.CAPA_IN_ATIVO = 1;
            novo.CAPA_IN_LIQUIDADA = 0;
            novo.CAPA_IN_PAGA_PARCIAL = 0;
            novo.CAPA_IN_PARCELADA = 0;
            novo.CAPA_IN_PARCELAS = 0;
            novo.CAPA_IN_TIPO_LANCAMENTO = 0;
            novo.CAPA_NM_FAVORECIDO = item.CAPA_NM_FAVORECIDO;
            novo.CAPA_NR_DOCUMENTO = item.CAPA_NR_DOCUMENTO;
            novo.CAPA_TX_OBSERVACOES = item.CAPA_TX_OBSERVACOES;
            novo.CAPA_VL_DESCONTO = 0;
            novo.CAPA_VL_JUROS = 0;
            novo.CAPA_VL_PARCELADO = 0;
            novo.CAPA_VL_PARCIAL = 0;
            novo.CAPA_VL_SALDO = 0;
            novo.CAPA_VL_TAXAS = 0;
            novo.CAPA_VL_VALOR = item.CAPA_VL_VALOR;
            novo.CAPA_VL_VALOR_PAGO = 0;
            novo.CECU_CD_ID = item.CECU_CD_ID;
            novo.FORN_CD_ID = item.FORN_CD_ID;
            novo.COBA_CD_ID = item.COBA_CD_ID;
            novo.COLA_CD_D = item.COLA_CD_D;
            novo.FILI_CD_ID = item.FILI_CD_ID;
            novo.FOPA_CD_ID = item.FOPA_CD_ID;
            novo.MATR_CD_ID = item.MATR_CD_ID;
            novo.PERI_CD_ID = item.PERI_CD_ID;
            novo.PECO_CD_ID = item.PECO_CD_ID;
            novo.PLCO_CD_ID = item.PLCO_CD_ID;
            novo.TIFA_CD_ID = item.TIFA_CD_ID;
            novo.USUA_CD_ID = item.USUA_CD_ID;

            // Grava
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            Int32 volta = cpApp.ValidateCreate(novo, usuarioLogado);

            // Cria pastas
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Financeiro/CP/" + novo.CAPA_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));

            // Sucesso
            listaCPMaster = new List<CONTA_PAGAR>();
            SessionMocks.listaCP = null;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult LiquidarParcelaCR(Int32 id)
        {
            // Prepara view
            CONTA_RECEBER_PARCELA item = pcApp.GetItemById(id);
            objetoCRPAntes = item;
            SessionMocks.contaReceberParcela = item;
            SessionMocks.idVoltaCRP = id;
            ContaReceberParcelaViewModel vm = Mapper.Map<CONTA_RECEBER_PARCELA, ContaReceberParcelaViewModel>(item);
            vm.CRPA_VL_DESCONTO = 0;
            vm.CRPA_VL_JUROS = 0;
            vm.CRPA_VL_TAXAS = 0;
            vm.CRPA_VL_RECEBIDO = vm.CRPA_VL_VALOR;
            vm.CRPA_DT_QUITACAO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarParcelaCR(ContaReceberParcelaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER_PARCELA item = Mapper.Map<ContaReceberParcelaViewModel, CONTA_RECEBER_PARCELA>(vm);
                    Int32 volta = pcApp.ValidateEdit(item, SessionMocks.contaReceberParcela, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Acerta saldo
                    CONTA_RECEBER rec = crApp.GetItemById(SessionMocks.idVolta);
                    rec.CARE_VL_SALDO = rec.CARE_VL_SALDO - item.CRPA_VL_RECEBIDO;                    
                    
                    // Verifica se liquidou todas
                    List<CONTA_RECEBER_PARCELA> lista = rec.CONTA_RECEBER_PARCELA.Where(p => p.CRPA_IN_QUITADA == 0).ToList<CONTA_RECEBER_PARCELA>();
                    if (lista.Count == 0)
                    {
                        rec.CARE_IN_LIQUIDADA = 1;
                        rec.CARE_DT_DATA_LIQUIDACAO = DateTime.Today.Date;
                        rec.CARE_VL_VALOR_LIQUIDADO = rec.CONTA_RECEBER_PARCELA.Sum(p => p.CRPA_VL_RECEBIDO);
                        rec.CARE_VL_SALDO = 0;
                    }
                    volta = crApp.ValidateEdit(rec, SessionMocks.contaReceber, usuarioLogado);

                    // Sucesso
                    listaCRPMaster = new List<CONTA_RECEBER_PARCELA>();
                    SessionMocks.listaCRP = null;
                    return RedirectToAction("VoltarAnexoCR");
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
        public ActionResult LiquidarParcelaCP(Int32 id)
        {
            // Prepara view
            CONTA_PAGAR_PARCELA item = ppApp.GetItemById(id);
            objetoCPPAntes = item;
            SessionMocks.contaPagarParcela = item;
            SessionMocks.idVoltaCPP = id;
            ContaPagarParcelaViewModel vm = Mapper.Map<CONTA_PAGAR_PARCELA, ContaPagarParcelaViewModel>(item);
            vm.CPPA_VL_DESCONTO = 0;
            vm.CPPA_VL_JUROS = 0;
            vm.CPPA_VL_TAXAS = 0;
            vm.CPPA_VL_VALOR_PAGO = vm.CPPA_VL_VALOR;
            vm.CPPA_DT_QUITACAO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarParcelaCP(ContaPagarParcelaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_PAGAR_PARCELA item = Mapper.Map<ContaPagarParcelaViewModel, CONTA_PAGAR_PARCELA>(vm);
                    Int32 volta = ppApp.ValidateEdit(item, SessionMocks.contaPagarParcela, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Acerta saldo
                    CONTA_PAGAR rec = cpApp.GetItemById(SessionMocks.idVolta);
                    rec.CAPA_VL_SALDO = rec.CAPA_VL_SALDO - item.CPPA_VL_VALOR_PAGO;

                    // Verifica se liquidou todas
                    List<CONTA_PAGAR_PARCELA> lista = rec.CONTA_PAGAR_PARCELA.Where(p => p.CPPA_IN_QUITADA == 0).ToList<CONTA_PAGAR_PARCELA>();
                    if (lista.Count == 0)
                    {
                        rec.CAPA_IN_LIQUIDADA = 1;
                        rec.CAPA_DT_LIQUIDACAO = DateTime.Today.Date;
                        rec.CAPA_VL_VALOR_PAGO = rec.CONTA_PAGAR_PARCELA.Sum(p => p.CPPA_VL_VALOR_PAGO);
                        rec.CAPA_VL_SALDO = 0;
                    }
                    SessionMocks.eParcela = 1;
                    volta = cpApp.ValidateEdit(rec, SessionMocks.contaPagar, usuarioLogado);

                    // Sucesso
                    listaCPPMaster = new List<CONTA_PAGAR_PARCELA>();
                    SessionMocks.listaCPP = null;
                    return RedirectToAction("VoltarAnexoCP");
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
        public ActionResult LiquidarCR(Int32 id)
        {
            // Prepara view
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 1;
            SessionMocks.liquidaCR = 1;

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            SessionMocks.contaReceber = item;
            SessionMocks.idVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            if (vm.CARE_IN_PAGA_PARCIAL == 1)
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_SALDO;
            }
            else
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_VALOR;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarCR(ContaReceberViewModel vm)
        {
            ViewBag.Clientes = new SelectList(cliApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 1;
            SessionMocks.liquidaCR = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    Int32 volta = crApp.ValidateEdit(item, SessionMocks.contaReceber, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0049", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0050", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 7)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 8)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;
                    return RedirectToAction("MontarTelaCR");
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
        public ActionResult LiquidarCP(Int32 id)
        {
            // Prepara view
            ViewBag.Forn = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 1;
            SessionMocks.liquidaCP = 1;

            CONTA_PAGAR item = cpApp.GetItemById(id);
            objetoCPAntes = item;
            SessionMocks.contaPagar = item;
            SessionMocks.idVolta = id;
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
            vm.CAPA_VL_PARCELADO = vm.CAPA_VL_VALOR;
            if (vm.CAPA_IN_PAGA_PARCIAL == 1)
            {
                vm.CAPA_VL_VALOR_PAGO = vm.CAPA_VL_SALDO;
            }
            else
            {
                vm.CAPA_VL_VALOR_PAGO = vm.CAPA_VL_VALOR;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarCP(ContaPagarViewModel vm)
        {
            ViewBag.Forn = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Planos = new SelectList(plaApp.GetAllItens(), "PLCO_CD_ID", "PLCO_NM_NUMERO_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Colaboradores = new SelectList(ctApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 1;
            SessionMocks.liquidaCR = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_PAGAR item = Mapper.Map<ContaPagarViewModel, CONTA_PAGAR>(vm);
                    Int32 volta = cpApp.ValidateEdit(item, SessionMocks.contaPagar, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0049", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0050", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 7)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 8)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Sucesso
                    listaCPMaster = new List<CONTA_PAGAR>();
                    SessionMocks.listaCP = null;
                    return RedirectToAction("MontarTelaCP");
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
        public ActionResult MontarTelaCaixa()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            Int32 mes = 0;
            Int32 ano = 0;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.trocaConta == 0)
            {
                if (SessionMocks.contaPadrao == null)
                {
                    contaPadrao = cbApp.GetContaPadrao();
                    SessionMocks.contaPadrao = contaPadrao;
                }
                else
                {
                    contaPadrao = SessionMocks.contaPadrao;    
                }
                mes = DateTime.Today.Month;
                ano = DateTime.Today.Year;
                SessionMocks.mesCaixa = mes;
                SessionMocks.anoCaixa = ano;
                SessionMocks.dataReferencia = DateTime.Today.Date;
            }
            else
            {
                contaPadrao = SessionMocks.contaPadrao;
                mes = SessionMocks.mesCaixa;
                ano = SessionMocks.anoCaixa;
            }
            List<CONTA_BANCARIA_LANCAMENTO> lanc = cbApp.GetLancamentosMes(contaPadrao.COBA_CD_ID, mes);
            ViewBag.Lancamentos = lanc;
            ViewBag.Receitas = lanc.Where(p => p.CBLA_IN_TIPO == 1).ToList();
            ViewBag.Despesas = lanc.Where(p => p.CBLA_IN_TIPO == 2).ToList();

            ViewBag.Conta = SessionMocks.contaPadrao;
            ViewBag.Title = "Caixaxxx";
            ViewBag.Mes = CrossCutting.Formatters.TraduzMes(mes) + "/" + ano.ToString();
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();

            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");

            // Indicadores
            if (lanc.Count > 0)
            {
                ViewBag.TotalReceita = cbApp.GetTotalReceita(contaPadrao.COBA_CD_ID);
                ViewBag.TotalDespesa = cbApp.GetTotalDespesa(contaPadrao.COBA_CD_ID);
                ViewBag.TotalReceitaMes = cbApp.GetTotalReceitaMes(contaPadrao.COBA_CD_ID, mes);
                ViewBag.TotalDespesaMes = cbApp.GetTotalDespesaMes(contaPadrao.COBA_CD_ID, mes);
            }
            else
            {
                ViewBag.TotalReceita = 0;
                ViewBag.TotalDespesa = 0;
                ViewBag.TotalReceitaMes = 0;
                ViewBag.TotalDespesaMes = 0;
            }
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            SessionMocks.trocaConta = 0;
            contaPadrao.COBA_IN_CONTA_SELECAO = contaPadrao.COBA_CD_ID;
            contaPadrao.COBA_DT_SELECAO = SessionMocks.dataReferencia;

            // Abre view
            return View(contaPadrao);
        }

        [HttpPost]
        public ActionResult MontarTelaCaixa(CONTA_BANCARIA conta)
        {
            if (conta.COBA_IN_CONTA_SELECAO != null)
            {
                CONTA_BANCARIA cb = cbApp.GetItemById(conta.COBA_IN_CONTA_SELECAO.Value);
                SessionMocks.contaPadrao = cb;
                SessionMocks.trocaConta = 1;
            }
            if (conta.COBA_DT_SELECAO != null)
            {
                DateTime data = conta.COBA_DT_SELECAO.Value;
                Int32 mes = data.Month;
                Int32 ano = data.Year;
                SessionMocks.mesCaixa = mes;
                SessionMocks.anoCaixa = ano;
                SessionMocks.dataReferencia = conta.COBA_DT_SELECAO.Value;
                SessionMocks.trocaConta = 1;
            }
            return RedirectToAction("MontarTelaCaixa");
        }

        [HttpGet]
        public ActionResult TrocarContaCaixa(Int32 id)
        {
            CONTA_BANCARIA conta = cbApp.GetItemById(id);
            SessionMocks.trocaConta = 1;
            SessionMocks.contaPadrao = conta;
            return RedirectToAction("MontarTelaCaixa");
        }

        [HttpGet]
        public ActionResult IncluirLancamentoCB()
        {
            // Prepara view
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTA_BANCARIA_LANCAMENTO item = new CONTA_BANCARIA_LANCAMENTO();
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCARIA_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            vm.COBA_CD_ID = SessionMocks.contaPadrao.COBA_CD_ID;
            vm.CBLA_IN_ATIVO = 1;
            vm.CBLA_IN_ORIGEM = 1;
            vm.CBLA_DT_LANCAMENTO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirLancamentoCB(ContaBancariaLancamentoViewModel vm)
        {
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCARIA_LANCAMENTO item = Mapper.Map<ContaBancariaLancamentoViewModel, CONTA_BANCARIA_LANCAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = cbApp.ValidateCreateLancamento(item);

                    // Acerta Saldo
                    CONTA_BANCARIA conta = cbApp.GetItemById(item.COBA_CD_ID);
                    if (item.CBLA_IN_TIPO == 1)
                    {
                        conta.COBA_VL_SALDO_ATUAL = conta.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
                    }
                    else
                    {
                        conta.COBA_VL_SALDO_ATUAL = conta.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
                    }
                    Int32 vv = cbApp.ValidateEdit(conta, conta, usuarioLogado);

                    // Verifica retorno
                    SessionMocks.contaPadrao = conta;
                    return RedirectToAction("MontarTelaCaixaNova");
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
        public ActionResult VerLancamentoCaixa(Int32 id)
        {
            // Prepara view
            CONTA_BANCARIA_LANCAMENTO item = cbApp.GetLancamentoById(id);
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCARIA_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult MontarTelaCaixaNova()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            Int32 mes = 0;
            Int32 ano = 0;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            List<CONTA_BANCARIA_LANCAMENTO> lanc = new List<CONTA_BANCARIA_LANCAMENTO>();
            if (SessionMocks.contaPadrao == null)
            {
                contaPadrao = cbApp.GetContaPadrao();
                SessionMocks.contaPadrao = contaPadrao;
                mes = DateTime.Today.Month;
                ano = DateTime.Today.Year;
                SessionMocks.mesCaixa = mes;
                SessionMocks.anoCaixa = ano;
                SessionMocks.dataReferencia = DateTime.Today.Date;
                lanc = cbApp.GetLancamentosMes(contaPadrao.COBA_CD_ID, mes);
                ViewBag.Lancamentos = lanc;
                ViewBag.Mes = CrossCutting.Formatters.TraduzMes(mes) + "/" + ano.ToString();
            }
            else
            {
                contaPadrao = SessionMocks.contaPadrao;
                if (SessionMocks.tipoFiltro == 0 || SessionMocks.tipoFiltro == 1)
                {
                    mes = SessionMocks.mesCaixa;
                    ano = SessionMocks.anoCaixa;
                    lanc = cbApp.GetLancamentosMes(contaPadrao.COBA_CD_ID, mes);
                    ViewBag.Lancamentos = lanc;
                    ViewBag.Mes = CrossCutting.Formatters.TraduzMes(mes) + "/" + ano.ToString();
                }
                else if (SessionMocks.tipoFiltro == 3)
                {
                    lanc = cbApp.GetLancamentosDia(contaPadrao.COBA_CD_ID, SessionMocks.dataReferencia);
                    ViewBag.Lancamentos = lanc;
                    ViewBag.Mes = SessionMocks.dataReferencia.ToShortDateString();
                    mes = SessionMocks.dataReferencia.Month;
                    ano = SessionMocks.dataReferencia.Year;
                }
                else if (SessionMocks.tipoFiltro == 2)
                {
                    DateTime now = SessionMocks.dataReferencia;
                    Int32 dayOfWeek = (Int32)now.DayOfWeek;
                    dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
                    DateTime inicio = now.AddDays(1 - (Int32)now.DayOfWeek);
                    DateTime final = inicio.AddDays(7);
                    lanc = cbApp.GetLancamentosFaixa(contaPadrao.COBA_CD_ID, inicio, final);
                    ViewBag.Lancamentos = lanc;
                    mes = SessionMocks.dataReferencia.Month;
                    ano = SessionMocks.dataReferencia.Year;
                    ViewBag.Mes = CrossCutting.Formatters.TraduzMes(mes) + "/" + ano.ToString();
                }
            }

            ViewBag.Receitas = lanc.Where(p => p.CBLA_IN_TIPO == 1).ToList();
            ViewBag.Despesas = lanc.Where(p => p.CBLA_IN_TIPO == 2).ToList();

            ViewBag.Conta = SessionMocks.contaPadrao;
            ViewBag.Title = "Caixaxxx";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();

            // Indicadores
            if (lanc.Count > 0)
            {
                ViewBag.TotalReceita = cbApp.GetTotalReceita(contaPadrao.COBA_CD_ID);
                ViewBag.TotalDespesa = cbApp.GetTotalDespesa(contaPadrao.COBA_CD_ID);
                ViewBag.TotalReceitaMes = cbApp.GetTotalReceitaMes(contaPadrao.COBA_CD_ID, mes);
                ViewBag.TotalDespesaMes = cbApp.GetTotalDespesaMes(contaPadrao.COBA_CD_ID, mes);
            }
            else
            {
                ViewBag.TotalReceita = 0;
                ViewBag.TotalDespesa = 0;
                ViewBag.TotalReceitaMes = 0;
                ViewBag.TotalDespesaMes = 0;
            }
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Carrega filtros
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            List<SelectListItem> escopo = new List<SelectListItem>();
            escopo.Add(new SelectListItem() { Text = "Por Mês", Value = "1" });
            escopo.Add(new SelectListItem() { Text = "Por Semana", Value = "2" });
            escopo.Add(new SelectListItem() { Text = "Por Dia", Value = "3" });
            ViewBag.Escopo = new SelectList(escopo, "Value", "Text");
            contaPadrao.COBA_IN_CONTA_SELECAO = contaPadrao.COBA_CD_ID;
            contaPadrao.COBA_DT_SELECAO = DateTime.Today.Date;

            // Abre view
            if (SessionMocks.filtroCB != null)
            {
                contaPadrao = SessionMocks.filtroCB;
            }
            return View(contaPadrao);
        }

        public ActionResult RetirarFiltroCaixa()
        {
            SessionMocks.contaPadrao = null;
            SessionMocks.filtroCB = null;
            return RedirectToAction("MontarTelaCaixaNova");
        }

        [HttpPost]
        public ActionResult FiltrarCaixa(CONTA_BANCARIA item)
        {
            try
            {
                // Executa a operação
                CONTA_BANCARIA conta = new CONTA_BANCARIA();

                // Processa filtro
                if (item.COBA_IN_TIPO_FILTRO == null)
                {
                    SessionMocks.tipoFiltro = 1;
                }
                else
                {
                    SessionMocks.tipoFiltro = item.COBA_IN_TIPO_FILTRO.Value;
                }
                if (SessionMocks.tipoFiltro == 0)
                {
                    if (item.COBA_IN_CONTA_SELECAO != null)
                    {
                        conta = cbApp.GetItemById(item.COBA_IN_CONTA_SELECAO.Value);
                        SessionMocks.contaPadrao = conta;
                        SessionMocks.filtroCB = conta;
                    }
                    if (item.COBA_DT_SELECAO != null)
                    {
                        SessionMocks.mesCaixa = item.COBA_DT_SELECAO.Value.Month;
                        SessionMocks.anoCaixa = item.COBA_DT_SELECAO.Value.Year;
                        SessionMocks.dataReferencia = item.COBA_DT_SELECAO.Value;
                    }
                }
                if (SessionMocks.tipoFiltro == 1)
                {
                    if (item.COBA_IN_CONTA_SELECAO != null)
                    {
                        conta = cbApp.GetItemById(item.COBA_IN_CONTA_SELECAO.Value);
                        SessionMocks.contaPadrao = conta;
                        SessionMocks.filtroCB = conta;
                    }
                    SessionMocks.mesCaixa = item.COBA_DT_SELECAO.Value.Month;
                    SessionMocks.anoCaixa = item.COBA_DT_SELECAO.Value.Year;
                    SessionMocks.dataReferencia = item.COBA_DT_SELECAO.Value;
                }
                if (SessionMocks.tipoFiltro == 3)
                {
                    if (item.COBA_IN_CONTA_SELECAO != null)
                    {
                        conta = cbApp.GetItemById(item.COBA_IN_CONTA_SELECAO.Value);
                        SessionMocks.contaPadrao = conta;
                        SessionMocks.filtroCB = conta;
                    }
                    SessionMocks.dataReferencia = item.COBA_DT_SELECAO.Value;
                }
                if (SessionMocks.tipoFiltro == 2)
                {
                    if (item.COBA_IN_CONTA_SELECAO != null)
                    {
                        conta = cbApp.GetItemById(item.COBA_IN_CONTA_SELECAO.Value);
                        SessionMocks.contaPadrao = conta;
                        SessionMocks.filtroCB = conta;
                    }
                    SessionMocks.dataReferencia = DateTime.Today.Date;
                }




                // Sucesso
                return RedirectToAction("MontarTelaCaixaNova");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCaixaNova");
            }
        }

       [HttpGet]
        public ActionResult MontarTelaCaixaFinal()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            Int32 mes = 0;
            Int32 ano = 0;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            List<CONTA_BANCARIA_LANCAMENTO> lanc = new List<CONTA_BANCARIA_LANCAMENTO>();
            if (SessionMocks.contaPadrao == null)
            {
                contaPadrao = cbApp.GetContaPadrao();
                SessionMocks.contaPadrao = contaPadrao;
                mes = DateTime.Today.Month;
                ano = DateTime.Today.Year;
                SessionMocks.mesCaixa = mes;
                SessionMocks.anoCaixa = ano;
                SessionMocks.dataReferencia = DateTime.Today.Date;
                lanc = cbApp.GetLancamentosMes(contaPadrao.COBA_CD_ID, mes);
                ViewBag.Lancamentos = lanc;
                ViewBag.Mes = CrossCutting.Formatters.TraduzMes(mes) + "/" + ano.ToString();
            }
            else
            {
                contaPadrao = SessionMocks.contaPadrao;
                mes = SessionMocks.mesCaixa;
                ano = SessionMocks.anoCaixa;
                if (mes != 0)
                {
                    lanc = cbApp.GetLancamentosMes(contaPadrao.COBA_CD_ID, mes);
                    ViewBag.Mes = CrossCutting.Formatters.TraduzMes(mes) + "/" + ano.ToString();
                }
                else
                {
                    lanc = cbApp.GetLancamentosFaixa(contaPadrao.COBA_CD_ID, SessionMocks.dataInicio, SessionMocks.dataFinal);
                    ViewBag.Mes = CrossCutting.Formatters.TraduzMes(mes) + "/" + ano.ToString();
                }
            }

            ViewBag.Receitas = lanc.Where(p => p.CBLA_IN_TIPO == 1).ToList();
            ViewBag.Despesas = lanc.Where(p => p.CBLA_IN_TIPO == 2).ToList();

            ViewBag.Conta = SessionMocks.contaPadrao;
            ViewBag.Title = "Caixa";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();

            // Indicadores
            if (lanc.Count > 0)
            {
                ViewBag.TotalReceita = cbApp.GetTotalReceita(contaPadrao.COBA_CD_ID);
                ViewBag.TotalDespesa = cbApp.GetTotalDespesa(contaPadrao.COBA_CD_ID);
                ViewBag.TotalReceitaMes = cbApp.GetTotalReceitaMes(contaPadrao.COBA_CD_ID, mes);
                ViewBag.TotalDespesaMes = cbApp.GetTotalDespesaMes(contaPadrao.COBA_CD_ID, mes);
            }
            else
            {
                ViewBag.TotalReceita = 0;
                ViewBag.TotalDespesa = 0;
                ViewBag.TotalReceitaMes = 0;
                ViewBag.TotalDespesaMes = 0;
            }
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Carrega filtros
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            List<SelectListItem> listaMes = new List<SelectListItem>();
            listaMes.Add(new SelectListItem() { Text = "Janeiro", Value = "1" });
            listaMes.Add(new SelectListItem() { Text = "Fevereiro", Value = "2" });
            listaMes.Add(new SelectListItem() { Text = "Março", Value = "3" });
            listaMes.Add(new SelectListItem() { Text = "Abril", Value = "4" });
            listaMes.Add(new SelectListItem() { Text = "Maio", Value = "5" });
            listaMes.Add(new SelectListItem() { Text = "Junho", Value = "6" });
            listaMes.Add(new SelectListItem() { Text = "Julho", Value = "7" });
            listaMes.Add(new SelectListItem() { Text = "Agosto", Value = "8" });
            listaMes.Add(new SelectListItem() { Text = "Setembro", Value = "9" });
            listaMes.Add(new SelectListItem() { Text = "Outubro", Value = "10" });
            listaMes.Add(new SelectListItem() { Text = "Novembro", Value = "11" });
            listaMes.Add(new SelectListItem() { Text = "Dezembro", Value = "12" });
            ViewBag.ListaMes = new SelectList(listaMes, "Value", "Text");

            contaPadrao.COBA_IN_CONTA_SELECAO = contaPadrao.COBA_CD_ID;
            contaPadrao.COBA_DT_SELECAO = DateTime.Today.Date;
            contaPadrao.COBA_DT_SELECAO_FINAL = DateTime.Today.Date;

            // Abre view
            if (SessionMocks.filtroCB != null)
            {
                contaPadrao = SessionMocks.filtroCB;
            }
            return View(contaPadrao);
        }

        public ActionResult RetirarFiltroCaixaFinal()
        {
            SessionMocks.contaPadrao = null;
            SessionMocks.filtroCB = null;
            return RedirectToAction("MontarTelaCaixaFinal");
        }

        [HttpPost]
        public ActionResult FiltrarCaixaFinal(CONTA_BANCARIA item)
        {
            try
            {
                // Executa a operação
                CONTA_BANCARIA conta = new CONTA_BANCARIA();
                SessionMocks.tipoFiltro1 = 0;
                SessionMocks.tipoFiltro2 = 0;
                SessionMocks.tipoFiltro3 = 0;
                SessionMocks.tipoFiltro4 = 0;

                // Processa filtro
                conta = cbApp.GetItemById(item.COBA_IN_CONTA_SELECAO.Value);
                if (item.COBA_IN_MES == null)
                {
                    SessionMocks.mesCaixa = 0;
                }
                else
                {
                    SessionMocks.mesCaixa = item.COBA_IN_MES.Value;
                }
                SessionMocks.dataInicio = item.COBA_DT_SELECAO.Value;
                SessionMocks.dataFinal = item.COBA_DT_SELECAO_FINAL.Value;
                SessionMocks.dataReferencia = item.COBA_DT_SELECAO.Value;
                SessionMocks.anoCaixa = SessionMocks.dataReferencia.Year;







                //if (item.COBA_IN_CONTA_SELECAO != null)
                //{
                //    conta = cbApp.GetItemById(item.COBA_IN_CONTA_SELECAO.Value);
                //    SessionMocks.contaPadrao = conta;
                //    SessionMocks.filtroCB = conta;
                //    SessionMocks.tipoFiltro1 = 1;
                //}
                //if (item.COBA_IN_MES != null)
                //{
                //    SessionMocks.mesCaixa = item.COBA_IN_MES.Value;
                //    SessionMocks.anoCaixa = DateTime.Today.Date.Year;
                //    SessionMocks.tipoFiltro2 = 1;
                //}
                //if (item.COBA_DT_SELECAO != null & item.COBA_DT_SELECAO_FINAL != null & item.COBA_DT_SELECAO_FINAL == item.COBA_DT_SELECAO)
                //{
                //    SessionMocks.mesCaixa = item.COBA_DT_SELECAO.Value.Month;
                //    SessionMocks.anoCaixa = item.COBA_DT_SELECAO.Value.Year;
                //    SessionMocks.dataReferencia = item.COBA_DT_SELECAO.Value;
                //    SessionMocks.dataInicio = item.COBA_DT_SELECAO.Value;
                //    SessionMocks.dataFinal = item.COBA_DT_SELECAO_FINAL.Value;
                //    SessionMocks.tipoFiltro3 = 1;
                //}

                // Sucesso
                return RedirectToAction("MontarTelaCaixaFinal");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCaixaFinal");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirLancamentoCBFinal(ContaBancariaLancamentoViewModel vm)
        {
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCARIA_LANCAMENTO item = Mapper.Map<ContaBancariaLancamentoViewModel, CONTA_BANCARIA_LANCAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = cbApp.ValidateCreateLancamento(item);

                    // Acerta Saldo
                    CONTA_BANCARIA conta = cbApp.GetItemById(item.COBA_CD_ID);
                    if (item.CBLA_IN_TIPO == 1)
                    {
                        conta.COBA_VL_SALDO_ATUAL = conta.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
                    }
                    else
                    {
                        conta.COBA_VL_SALDO_ATUAL = conta.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
                    }
                    Int32 vv = cbApp.ValidateEdit(conta, conta, usuarioLogado);

                    // Verifica retorno
                    SessionMocks.contaPadrao = conta;
                    return RedirectToAction("MontarTelaCaixaFinal");
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
        public ActionResult VerLancamentoCaixaFinal(Int32 id)
        {
            // Prepara view
            CONTA_BANCARIA_LANCAMENTO item = cbApp.GetLancamentoById(id);
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCARIA_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            return View(vm);
        }



    }
}