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
using ExternalServices;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class CadastrosController : Controller
    {
        private readonly IClienteAppService baseApp;
        private readonly IMatrizAppService matrizApp;
        private readonly ILogAppService logApp;
        private readonly IFornecedorAppService fornApp;
        private readonly IProdutoAppService prodApp;
        private readonly IMateriaPrimaAppService matApp;
        private readonly IServicoAppService servApp;
        private readonly ITransportadoraAppService tranApp;
        private readonly IPatrimonioAppService patrApp;
        private readonly IEquipamentoAppService equiApp;
        private readonly IFichaTecnicaAppService ftApp;
        private readonly IContaReceberAppService recApp;
        private String msg;
        private Exception exception;
        CLIENTE objeto = new CLIENTE();
        CLIENTE objetoAntes = new CLIENTE();
        List<CLIENTE> listaMaster = new List<CLIENTE>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao = String.Empty;
        FORNECEDOR objetoForn = new FORNECEDOR();
        FORNECEDOR objetoFornAntes = new FORNECEDOR();
        List<FORNECEDOR> listaMasterForn = new List<FORNECEDOR>();
        PRODUTO objetoProd = new PRODUTO();
        PRODUTO objetoProdAntes = new PRODUTO();
        List<PRODUTO> listaMasterProd = new List<PRODUTO>();
        MATERIA_PRIMA objetoMat = new MATERIA_PRIMA();
        MATERIA_PRIMA objetoMatAntes = new MATERIA_PRIMA();
        List<MATERIA_PRIMA> listaMasterMat = new List<MATERIA_PRIMA>();
        SERVICO objetoServ = new SERVICO();
        SERVICO objetoServAntes = new SERVICO();
        List<SERVICO> listaMasterServ = new List<SERVICO>();
        TRANSPORTADORA objetoTran = new TRANSPORTADORA();
        TRANSPORTADORA objetoTranAntes = new TRANSPORTADORA();
        List<TRANSPORTADORA> listaMasterTran = new List<TRANSPORTADORA>();
        PATRIMONIO objetoPatr = new PATRIMONIO();
        PATRIMONIO objetoPatrAntes = new PATRIMONIO();
        List<PATRIMONIO> listaMasterPatr = new List<PATRIMONIO>();
        EQUIPAMENTO objetoEqui = new EQUIPAMENTO();
        EQUIPAMENTO objetoEquiAntes = new EQUIPAMENTO();
        List<EQUIPAMENTO> listaMasterEqui = new List<EQUIPAMENTO>();
        FICHA_TECNICA objetoFt = new FICHA_TECNICA();
        FICHA_TECNICA objetoFtAntes = new FICHA_TECNICA();
        List<FICHA_TECNICA> listaMasterFt = new List<FICHA_TECNICA>();
        CONTA_RECEBER objetoRec = new CONTA_RECEBER();
        CONTA_RECEBER objetoRecAntes = new CONTA_RECEBER();
        List<CONTA_RECEBER> listaMasterRec = new List<CONTA_RECEBER>();

        public CadastrosController(IClienteAppService baseApps, ILogAppService logApps, IMatrizAppService matrizApps, IFornecedorAppService fornApps, IProdutoAppService prodApps, IMateriaPrimaAppService matApps, IServicoAppService servApps, ITransportadoraAppService tranApps, IPatrimonioAppService patrApps, IEquipamentoAppService equiApps, IFichaTecnicaAppService ftApps, IContaReceberAppService recApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            matrizApp = matrizApps;
            fornApp = fornApps;
            prodApp = prodApps;
            matApp = matApps;
            servApp = servApps;
            tranApp = tranApps;
            patrApp = patrApps;
            equiApp = equiApps;
            ftApp = ftApps;
            recApp = recApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            
            listaMaster = new List<CLIENTE>();
            listaMasterForn = new List<FORNECEDOR>();
            listaMasterProd = new List<PRODUTO>();
            listaMasterMat = new List<MATERIA_PRIMA>();
            SessionMocks.listaCliente = null;
            SessionMocks.listaFornecedor = null;
            SessionMocks.listaProduto = null;
            SessionMocks.listaMateria = null;
            SessionMocks.filtroMateria = null;
            SessionMocks.listaFT = null;
            SessionMocks.filtroFT = null;

            if (SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA == "ADM")
            {
                return RedirectToAction("CarregarAdmin", "BaseAdmin");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult RetirarFiltroCliente()
        {
            
            SessionMocks.listaCliente = null;
            SessionMocks.filtroCliente = null;
            if (SessionMocks.voltaCliente == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult MostrarTudoCliente()
        {
            
            listaMaster = baseApp.GetAllItensAdm();
            SessionMocks.filtroCliente = null;
            SessionMocks.listaCliente = listaMaster;
            if (SessionMocks.voltaCliente == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult VoltarBaseCliente()
        {
            
            if (SessionMocks.voltaCliente == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpGet]
        public ActionResult ExcluirCliente(Int32 id)
        {
            
            // Prepara view
            CLIENTE item = baseApp.GetItemById(id);
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirCliente(ClienteViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<CLIENTE>();
                SessionMocks.listaCliente = null;
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarCliente(Int32 id)
        {
            
            // Prepara view
            CLIENTE item = baseApp.GetItemById(id);
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarCliente(ClienteViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<CLIENTE>();
                SessionMocks.listaCliente = null;
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoCliente(Int32 id)
        {
            
            // Prepara view
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult ShowImage()
        {
            
            byte[] imgbytes = System.IO.File.ReadAllBytes(SessionMocks.arquivo);
            return File(imgbytes, "image/jpeg");
        }

        public ActionResult VoltarAnexoCliente()
        {
            
            return RedirectToAction("EditarCliente", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadCliente(Int32 id)
        {
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.CLAN_AQ_ARQUIVO;
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

        public ActionResult BuscarCEPCliente(ClienteViewModel item)
        {
            
            String cep = item.CLIE_NR_CEP;
            Endereco volta = ECT_Services.GetAdressCEPService(cep);
            item.CLIE_NM_ENDERECO = volta.ENDERECO + " " + volta.NUMERO + " " + volta.COMPLEMENTO;
            item.CLIE_NM_BAIRRO = volta.BAIRRO;
            item.CLIE_NM_CIDADE = volta.CIDADE;
            item.CLIE_SG_UF = volta.UF;
            CLIENTE vm = Mapper.Map<ClienteViewModel, CLIENTE>(item);
            SessionMocks.voltaCEP = 1;
            SessionMocks.cliente = vm;
            return RedirectToAction("IncluirCliente");
        }

        [HttpGet]
        public ActionResult VisualizarCliente(Int32 id)
        {
            
            // Prepara view
            CLIENTE item = baseApp.GetItemById(id);
            objetoAntes = item;
            SessionMocks.cliente = item;
            SessionMocks.idVolta = id;
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        public ActionResult VerFichaTecnicaProduto()
        {
            
            // Prepara view
            PRODUTO item = prodApp.GetItemById(SessionMocks.idVolta);
            objetoProdAntes = item;
            SessionMocks.produto = item;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return RedirectToAction("CarregarDesenvolvimento", "BaseAdmin");
            //return View(vm);
        }

        public ActionResult VerLancamentoAtraso(Int32 id)
        {
            
            // Prepara view
            CONTA_RECEBER item = recApp.GetItemById(id);
            objetoRecAntes = item;
            SessionMocks.cr = item;
            SessionMocks.idCRVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            return View(vm);
        }

        public ActionResult VerInsumosPontoPedido()
        {
            
            // Prepara view
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(matApp.GetAllTipos(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(matApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(matApp.GetAllUnidades(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.PontoPedido = matApp.GetPontoPedido().Count;
            ViewBag.PontoPedidos = matApp.GetPontoPedido();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            // Abre view
            objetoMat = new MATERIA_PRIMA();
            SessionMocks.voltaMateria = 1;
            SessionMocks.voltaConsulta = 2;
            SessionMocks.clonar = 0;
            if (SessionMocks.filtroMateria   != null)
            {
                objetoMat = SessionMocks.filtroMateria;
            }
            return View(objetoMat);
        }

        public ActionResult VerInsumosEstoqueZerado()
        {
            
            // Prepara view
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(matApp.GetAllTipos(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(matApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(matApp.GetAllUnidades(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.EstoqueZerado = matApp.GetEstoqueZerado().Count;
            ViewBag.EstoqueZerados = matApp.GetEstoqueZerado();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            // Abre view
            objetoMat = new MATERIA_PRIMA();
            SessionMocks.voltaMateria = 1;
            SessionMocks.voltaConsulta = 3;
            SessionMocks.clonar = 0;
            if (SessionMocks.filtroMateria != null)
            {
                objetoMat = SessionMocks.filtroMateria;
            }
            return View(objetoMat);
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
        {
            
            // Prepara view
            CLIENTE_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = SessionMocks.cliente;
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(ClienteContatoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult ExcluirContato(Int32 id)
        {
            
            CLIENTE_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = SessionMocks.cliente;
            item.CLCO_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
        {
            
            CLIENTE_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = SessionMocks.cliente;
            item.CLCO_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult IncluirContato()
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CLIENTE_CONTATO item = new CLIENTE_CONTATO();
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            vm.CLIE_CD_ID = SessionMocks.idVolta;
            vm.CLCO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(ClienteContatoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult EditarReferencia(Int32 id)
        {
            
            // Prepara view
            CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
            objetoAntes = SessionMocks.cliente;
            ClienteReferenciaViewModel vm = Mapper.Map<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarReferencia(ClienteReferenciaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CLIENTE_REFERENCIA item = Mapper.Map<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>(vm);
                    Int32 volta = baseApp.ValidateEditReferencia(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult ExcluirReferencia(Int32 id)
        {
            
            CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
            objetoAntes = SessionMocks.cliente;
            item.CLRE_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditReferencia(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult ReativarReferencia(Int32 id)
        {
            
            CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
            objetoAntes = SessionMocks.cliente;
            item.CLRE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditReferencia(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult IncluirReferencia()
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CLIENTE_REFERENCIA item = new CLIENTE_REFERENCIA();
            ClienteReferenciaViewModel vm = Mapper.Map<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>(item);
            vm.CLIE_CD_ID = SessionMocks.idVolta;
            vm.CLRE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirReferencia(ClienteReferenciaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE_REFERENCIA item = Mapper.Map<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreateReferencia(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult IncluirTag()
        {
            
            // Prepara view
            List<SelectListItem> tipoTag = new List<SelectListItem>();
            tipoTag.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
            tipoTag.Add(new SelectListItem() { Text = "Alarme", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Elogio", Value = "2" });
            ViewBag.TipoTag = new SelectList(tipoTag, "Value", "Text");
            
            USUARIO usuario = SessionMocks.UserCredentials;
            CLIENTE_TAG item = new CLIENTE_TAG();
            ClienteTagViewModel vm = Mapper.Map<CLIENTE_TAG, ClienteTagViewModel>(item);
            vm.CLIE_CD_ID = SessionMocks.idVolta;
            vm.CLTA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTag(ClienteTagViewModel vm)
        {
            
            List<SelectListItem> tipoTag = new List<SelectListItem>();
            tipoTag.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
            tipoTag.Add(new SelectListItem() { Text = "Alarme", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Elogio", Value = "2" });
            ViewBag.TipoTag = new SelectList(tipoTag, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE_TAG item = Mapper.Map<ClienteTagViewModel, CLIENTE_TAG>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreateTag(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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

        public ActionResult RetirarFiltroFornecedor()
        {
            
            SessionMocks.listaFornecedor = null;
            SessionMocks.filtroFornecedor = null;
            if (SessionMocks.voltaFornecedor == 2)
            {
                return RedirectToAction("VerCardsFornecedor");
            }
            return RedirectToAction("MontarTelaFornecedor");
        }

        public ActionResult MostrarTudoFornecedor()
        {
            
            listaMasterForn = fornApp.GetAllItensAdm();
            SessionMocks.filtroFornecedor = null;
            SessionMocks.listaFornecedor = listaMasterForn;
            if (SessionMocks.voltaFornecedor == 2)
            {
                return RedirectToAction("VerCardsFornecedor");
            }
            return RedirectToAction("MontarTelaFornecedor");
        }

        public ActionResult VoltarBaseFornecedor()
        {
            
            //listaMasterForn = new List<FORNECEDOR>();
            //SessionMocks.listaFornecedor = null;
            if (SessionMocks.voltaFornecedor == 2)
            {
                return RedirectToAction("VerCardsFornecedor");
            }
            return RedirectToAction("MontarTelaFornecedor");
        }

        [HttpGet]
        public ActionResult ExcluirFornecedor(Int32 id)
        {
            
            // Prepara view
            FORNECEDOR item = fornApp.GetItemById(id);
            FornecedorViewModel vm = Mapper.Map<FORNECEDOR, FornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirFornecedor(FornecedorViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                FORNECEDOR item = Mapper.Map<FornecedorViewModel, FORNECEDOR>(vm);
                Int32 volta = fornApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0021", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMasterForn = new List<FORNECEDOR>();
                SessionMocks.listaFornecedor = null;
                return RedirectToAction("MontarTelaFornecedor");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarFornecedor(Int32 id)
        {
            
            // Prepara view
            FORNECEDOR item = fornApp.GetItemById(id);
            FornecedorViewModel vm = Mapper.Map<FORNECEDOR, FornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarFornecedor(FornecedorViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                FORNECEDOR item = Mapper.Map<FornecedorViewModel, FORNECEDOR>(vm);
                Int32 volta = fornApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<FORNECEDOR>();
                SessionMocks.listaFornecedor = null;
                return RedirectToAction("MontarTelaFornecedor");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoFornecedor(Int32 id)
        {
            
            // Prepara view
            FORNECEDOR_ANEXO item = fornApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoFornecedor()
        {
            
            return RedirectToAction("EditarFornecedor", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadFornecedor(Int32 id)
        {
            FORNECEDOR_ANEXO item = fornApp.GetAnexoById(id);
            String arquivo = item.FOAN_AQ_ARQUVO;
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

        public ActionResult BuscarCEPFornecedor(FORNECEDOR item)
        {
            
            return RedirectToAction("IncluirFornecedorEspecial", new { objeto = item});
        }

        public ActionResult RetirarFiltroProduto()
        {
            
            SessionMocks.listaProduto = null;
            SessionMocks.filtroProduto = null;
            if (SessionMocks.voltaProduto == 2)
            {
                return RedirectToAction("VerCardsProduto");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        public ActionResult MostrarTudoProduto()
        {
            
            listaMasterProd = prodApp.GetAllItensAdm();
            SessionMocks.filtroProduto = null;
            SessionMocks.listaProduto = listaMasterProd;
            if (SessionMocks.voltaProduto == 2)
            {
                return RedirectToAction("VerCardsProduto");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        public ActionResult VoltarBaseProduto()
        {
            
            if (SessionMocks.clonar == 1)
            {
                SessionMocks.clonar = 0;
                listaMasterProd = new List<PRODUTO>();
                SessionMocks.listaProduto = null;
            }
            if (SessionMocks.voltaProduto == 2)
            {
                return RedirectToAction("VerCardsProduto");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        public ActionResult ClonarProduto(Int32 id)
        {
            
            // Prepara objeto
            USUARIO usuario = SessionMocks.UserCredentials;
            PRODUTO item = prodApp.GetItemById(id);
            PRODUTO novo = new PRODUTO();
            novo.ASSI_CD_ID = item.ASSI_CD_ID;
            novo.CAPR_CD_ID = item.CAPR_CD_ID;
            novo.FILI_CD_ID = item.FILI_CD_ID;
            novo.MATR_CD_ID = item.MATR_CD_ID;
            novo.PROD_AQ_FOTO = item.PROD_AQ_FOTO;
            novo.PROD_CD_GTIN_EAN = item.PROD_CD_GTIN_EAN;
            novo.PROD_DS_DESCRICAO = item.PROD_DS_DESCRICAO;
            novo.PROD_DS_INFORMACAO_NUTRICIONAL = item.PROD_DS_INFORMACAO_NUTRICIONAL;
            novo.PROD_DS_INFORMACOES = item.PROD_DS_INFORMACOES;
            novo.PROD_DT_CADASTRO = DateTime.Today;
            novo.PROD_IN_ATIVO = 1;
            novo.PROD_IN_AVISA_MINIMO = item.PROD_IN_AVISA_MINIMO;
            novo.PROD_IN_BALANCA_PDV = item.PROD_IN_BALANCA_PDV;
            novo.PROD_IN_BALANCA_RETAGUARDA = item.PROD_IN_BALANCA_RETAGUARDA;
            novo.PROD_IN_COBRAR_MAIOR = item.PROD_IN_COBRAR_MAIOR;
            novo.PROD_IN_DIVISAO = item.PROD_IN_DIVISAO;
            novo.PROD_IN_GERAR_ARQUIVO = item.PROD_IN_GERAR_ARQUIVO;
            novo.PROD_IN_OPCAO_COMBO = item.PROD_IN_OPCAO_COMBO;
            novo.PROD_IN_TIPO_COMBO = item.PROD_IN_TIPO_COMBO;
            novo.PROD_IN_TIPO_EMBALAGEM = item.PROD_IN_TIPO_EMBALAGEM;
            novo.PROD_IN_TIPO_PRODUTO = item.PROD_IN_TIPO_PRODUTO;
            novo.PROD_NM_LOCALIZACAO_ESTOQUE = item.PROD_NM_LOCALIZACAO_ESTOQUE;
            novo.PROD_NM_NOME = "====== PRODUTO DUPLICADO ======";
            novo.PROD_NM_ORIGEM = item.PROD_NM_ORIGEM;
            novo.PROD_NR_ALTURA = item.PROD_NR_ALTURA;
            novo.PROD_NR_COMPRIMENTO = item.PROD_NR_COMPRIMENTO;
            novo.PROD_NR_DIAMETRO = item.PROD_NR_DIAMETRO;
            novo.PROD_NR_DIAS_VALIDADE = item.PROD_NR_DIAS_VALIDADE;
            novo.PROD_NR_GARANTIA = item.PROD_NR_GARANTIA;
            novo.PROD_NR_LARGURA = item.PROD_NR_LARGURA;
            novo.PROD_NR_NCM = item.PROD_NR_NCM;
            novo.PROD_NR_REFERENCIA = item.PROD_NR_REFERENCIA;
            novo.PROD_NR_VALIDADE = item.PROD_NR_VALIDADE;
            novo.PROD_PC_MARKUP_MININO = item.PROD_PC_MARKUP_MININO;
            novo.PROD_QN_ESTOQUE = 0;
            novo.PROD_QN_PESO_BRUTO = item.PROD_QN_PESO_BRUTO;
            novo.PROD_QN_PESO_LIQUIDO = item.PROD_QN_PESO_LIQUIDO;
            novo.PROD_QN_QUANTIDADE_INICIAL = 0;
            novo.PROD_QN_QUANTIDADE_MAXIMA = 0;
            novo.PROD_QN_QUANTIDADE_MINIMA = 0;
            novo.PROD_QN_RESERVA_ESTOQUE = 0;
            novo.PROD_TX_OBSERVACOES = item.PROD_TX_OBSERVACOES;
            novo.PROD_VL_CUSTO = item.PROD_VL_CUSTO;
            novo.PROD_VL_MARKUP_PADRAO = item.PROD_VL_MARKUP_PADRAO;
            novo.PROD_VL_PRECO_MINIMO = item.PROD_VL_PRECO_MINIMO;
            novo.PROD_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_PROMOCAO;
            novo.PROD_VL_PRECO_VENDA = item.PROD_VL_PRECO_VENDA;
            novo.SCPR_CD_ID = item.SCPR_CD_ID;
            novo.UNID_CD_ID = item.UNID_CD_ID;

            Int32 volta = prodApp.ValidateCreateLeve(novo, usuario);
            SessionMocks.idVolta = novo.PROD_CD_ID;
            SessionMocks.clonar = 1;
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ExcluirProduto(Int32 id)
        {
            
            // Prepara view
            PRODUTO item = prodApp.GetItemById(id);
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirProduto(ProdutoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                Int32 volta = prodApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0023", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMasterProd = new List<PRODUTO>();
                SessionMocks.listaProduto = null;
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarProduto(Int32 id)
        {
            
            // Prepara view
            PRODUTO item = prodApp.GetItemById(id);
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarProduto(ProdutoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                Int32 volta = prodApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterProd = new List<PRODUTO>();
                SessionMocks.listaProduto = null;
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoProduto(Int32 id)
        {
            
            // Prepara view
            PRODUTO_ANEXO item = prodApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoProduto()
        {
            
            return RedirectToAction("EditarProduto", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadProduto(Int32 id)
        {
            PRODUTO_ANEXO item = prodApp.GetAnexoById(id);
            String arquivo = item.PRAN_AQ_ARQUIVO;
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

        public ActionResult BuscarCEPProduto(PRODUTO item)
        {
            
            return RedirectToAction("IncluirProdutoEspecial", new { objeto = item});
        }

        public ActionResult VerMovimentacaoEstoqueProduto()
        {
            
            // Prepara view
            PRODUTO item = prodApp.GetItemById(SessionMocks.idVolta);
            objetoProdAntes = item;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult RetirarFiltroMateria()
        {
            
            SessionMocks.listaMateria = null;
            SessionMocks.filtroMateria = null;
            if (SessionMocks.voltaMateria == 2)
            {
                return RedirectToAction("VerCardsMateria");
            }
            return RedirectToAction("MontarTelaMateria");
        }

        public ActionResult MostrarTudoMateria()
        {
            
            listaMasterMat = matApp.GetAllItensAdm();
            SessionMocks.listaMateria = listaMasterMat;
            SessionMocks.filtroMateria = null;
            if (SessionMocks.voltaMateria == 2)
            {
                return RedirectToAction("VerCardsMateria");
            }
            return RedirectToAction("MontarTelaMateria");
        }

        [HttpPost]
        public ActionResult FiltrarMateria(MATERIA_PRIMA item)
        {
            
            try
            {
                // Executa a operação
                List<MATERIA_PRIMA> listaObj = new List<MATERIA_PRIMA>();
                SessionMocks.filtroMateria = item;
                Int32 volta = matApp.ExecuteFilter(item.CAMA_CD_ID, item.SCMP_CD_ID, item.MAPR_NM_NOME, item.MAPR_DS_DESCRICAO, item.MAPR_CD_CODIGO, item.MAPR_IN_ATIVO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterMat = listaObj;
                SessionMocks.listaMateria = listaObj;
                if (SessionMocks.voltaProduto == 2)
                {
                    return RedirectToAction("VerCardsMateria");
                }
                if (SessionMocks.voltaConsulta == 2)
                {
                    return RedirectToAction("VerInsumosPontoPedido");
                }
                if (SessionMocks.voltaConsulta == 3)
                {
                    return RedirectToAction("VerInsumosEstoqueZerado");
                }
                return RedirectToAction("MontarTelaMateria");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMateria");
            }
        }

        public ActionResult VoltarBaseMateria()
        {
            
            //listaMasterMat = new List<MATERIA_PRIMA>();
            //SessionMocks.listaMateria = null;
            if (SessionMocks.voltaMateria == 2)
            {
                return RedirectToAction("VerCardsMateria");
            }
            return RedirectToAction("MontarTelaMateria");
        }

        [HttpGet]
        public ActionResult ExcluirMateria(Int32 id)
        {
            
            // Prepara view
            MATERIA_PRIMA item = matApp.GetItemById(id);
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirMateria(MateriaPrimaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                MATERIA_PRIMA item = Mapper.Map<MateriaPrimaViewModel, MATERIA_PRIMA>(vm);
                Int32 volta = matApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0025", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMasterMat = new List<MATERIA_PRIMA>();
                SessionMocks.listaMateria = null;
                return RedirectToAction("MontarTelaMateria");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarMateria(Int32 id)
        {
            
            // Prepara view
            MATERIA_PRIMA item = matApp.GetItemById(id);
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarMateria(MateriaPrimaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                MATERIA_PRIMA item = Mapper.Map<MateriaPrimaViewModel, MATERIA_PRIMA>(vm);
                Int32 volta = matApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterMat = new List<MATERIA_PRIMA>();
                SessionMocks.listaMateria = null;
                return RedirectToAction("MontarTelaMateria");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoMateria(Int32 id)
        {
            
            // Prepara view
            MATERIA_PRIMA_ANEXO item = matApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoMateria()
        {
            
            return RedirectToAction("EditarMateria", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadMateria(Int32 id)
        {
            MATERIA_PRIMA_ANEXO item = matApp.GetAnexoById(id);
            String arquivo = item.MAPA_AQ_ARQUIVO;
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

        public ActionResult VerMovimentacaoEstoqueMateria()
        {
            
            // Prepara view
            MATERIA_PRIMA item = matApp.GetItemById(SessionMocks.idVolta);
            objetoMatAntes = item;
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
            return View(vm);
        }

        public ActionResult RetirarFiltroServico()
        {
            
            SessionMocks.listaServico = null;
            SessionMocks.filtroServico = null;
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult MostrarTudoServico()
        {
            listaMasterServ = servApp.GetAllItensAdm();
            SessionMocks.filtroServico = null;
            SessionMocks.listaServico = listaMasterServ;
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult VoltarBaseServico()
        {
            
            return RedirectToAction("MontarTelaServico");
        }

        [HttpGet]
        public ActionResult ExcluirServico(Int32 id)
        {
            
            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirServico(ServicoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                Int32 volta = servApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0027", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMasterServ = new List<SERVICO>();
                SessionMocks.listaServico = null;
                return RedirectToAction("MontarTelaServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarServico(Int32 id)
        {
            
            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarServico(ServicoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                Int32 volta = servApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterServ = new List<SERVICO>();
                SessionMocks.listaServico = null;
                return RedirectToAction("MontarTelaServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoServico(Int32 id)
        {
            
            // Prepara view
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoServico()
        {
            
            return RedirectToAction("EditarServico", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarAnexoFT()
        {
            
            return RedirectToAction("EditarFT", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadServico(Int32 id)
        {
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            String arquivo = item.SEAN_AQ_ARQUIVO;
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
        public ActionResult MontarTelaTransportadora()
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
            if (SessionMocks.listaTransportadora == null)
            {
                listaMasterTran = tranApp.GetAllItens();
                SessionMocks.listaTransportadora = listaMasterTran;
            }
            ViewBag.Listas = SessionMocks.listaTransportadora;
            ViewBag.Title = "Transportadoras";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");

            // Indicadores
            ViewBag.Transportadoras = tranApp.GetAllItens().Count;
            
            // Abre view
            objetoTran = new TRANSPORTADORA();
            if (SessionMocks.filtroTransportadora != null)
            {
                objetoTran = SessionMocks.filtroTransportadora;
            }
            SessionMocks.voltaTransportadora = 1;
            return View(objetoTran);
        }

        public ActionResult RetirarFiltroTransportadora()
        {
            
            SessionMocks.listaTransportadora = null;
            SessionMocks.filtroTransportadora = null;
            return RedirectToAction("MontarTelaTransportadora");
        }

        public ActionResult MostrarTudoTransportadora()
        {
            
            listaMasterTran = tranApp.GetAllItensAdm();
            SessionMocks.filtroTransportadora = null;
            SessionMocks.listaTransportadora = listaMasterTran;
            return RedirectToAction("MontarTelaTransportadora");
        }

        [HttpPost]
        public ActionResult FiltrarTransportadora(TRANSPORTADORA item)
        {
            
            try
            {
                // Executa a operação
                List<TRANSPORTADORA> listaObj = new List<TRANSPORTADORA>();
                SessionMocks.filtroTransportadora = item;
                Int32 volta = tranApp.ExecuteFilter(item.TRAN_NM_NOME, item.TRAN_NR_CNPJ, item.TRAN_NM_EMAIL, item.TRAN_NM_CIDADE, item.TRAN_SG_UF, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterTran = listaObj;
                SessionMocks.listaTransportadora = listaObj;
                return RedirectToAction("MontarTelaTransportadora");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaTransportadora");
            }
        }

        public ActionResult VoltarBaseTransportadora()
        {
            
            return RedirectToAction("MontarTelaTransportadora");
        }

        [HttpGet]
        public ActionResult EditarTransportadora(Int32 id)
        {
            
            // Prepara view
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");
            TRANSPORTADORA item = tranApp.GetItemById(id);
            objetoTranAntes = item;
            SessionMocks.transportadora = item;
            SessionMocks.idVolta = id;
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTransportadora(TransportadoraViewModel vm)
        {
            
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    TRANSPORTADORA item = Mapper.Map<TransportadoraViewModel, TRANSPORTADORA>(vm);
                    Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTran = new List<TRANSPORTADORA>();
                    SessionMocks.listaTransportadora = null;
                    return RedirectToAction("MontarTelaTransportadora");
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
        public ActionResult ExcluirTransportadora(Int32 id)
        {
            
            // Prepara view
            TRANSPORTADORA item = tranApp.GetItemById(id);
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirTransportadora(TransportadoraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                TRANSPORTADORA item = Mapper.Map<TransportadoraViewModel, TRANSPORTADORA>(vm);
                Int32 volta = tranApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0029", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMasterTran = new List<TRANSPORTADORA>();
                SessionMocks.listaTransportadora = null;
                return RedirectToAction("MontarTelaTransportadora");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarTransportadora(Int32 id)
        {
            
            // Prepara view
            TRANSPORTADORA item = tranApp.GetItemById(id);
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarTransportadora(TransportadoraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                TRANSPORTADORA item = Mapper.Map<TransportadoraViewModel, TRANSPORTADORA>(vm);
                Int32 volta = tranApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterTran = new List<TRANSPORTADORA>();
                SessionMocks.listaTransportadora = null;
                return RedirectToAction("MontarTelaTransportadora");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        public ActionResult VerCardsTransportadora()
        {
            
            // Carrega listas
            if (SessionMocks.listaTransportadora == null)
            {
                listaMasterTran = tranApp.GetAllItens();
                SessionMocks.listaTransportadora = listaMasterTran;
            }
            ViewBag.Listas = SessionMocks.listaTransportadora;
            ViewBag.Title = "Transportadoras";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");

            // Indicadores
            ViewBag.Transportadoras = tranApp.GetAllItens().Count;

            // Abre view
            objetoTran = new TRANSPORTADORA();
            SessionMocks.voltaTransportadora = 2;
            if (SessionMocks.filtroTransportadora != null)
            {
                objetoTran = SessionMocks.filtroTransportadora;
            }
            return View(objetoTran);
        }

        [HttpGet]
        public ActionResult VerAnexoTransportadora(Int32 id)
        {
            
            // Prepara view
            TRANSPORTADORA_ANEXO item = tranApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoTransportadora()
        {
            
            return RedirectToAction("EditarTransportadora", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadTransportadora(Int32 id)
        {
            TRANSPORTADORA_ANEXO item = tranApp.GetAnexoById(id);
            String arquivo = item.TRAX_AQ_ARQUIVO;
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
        public ActionResult SlideShowTransportadora()
        {
            
            // Prepara view
            TRANSPORTADORA item = tranApp.GetItemById(SessionMocks.idVolta);
            objetoTranAntes = item;
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        public ActionResult RetirarFiltroPatrimonio()
        {
            
            SessionMocks.listaPatrimonio = null;
            SessionMocks.filtroPatrimonio = null;
            return RedirectToAction("MontarTelaPatrimonio");
        }

        public ActionResult MostrarTudoPatrimonio()
        {
            
            listaMasterPatr = patrApp.GetAllItensAdm();
            SessionMocks.filtroPatrimonio = null;
            SessionMocks.listaPatrimonio = listaMasterPatr;
            return RedirectToAction("MontarTelaPatrimonio");
        }

        [HttpPost]
        public ActionResult FiltrarPatrimonio(PATRIMONIO item)
        {
            
            try
            {
                // Executa a operação
                List<PATRIMONIO> listaObj = new List<PATRIMONIO>();
                SessionMocks.filtroPatrimonio = item;
                Int32 volta = patrApp.ExecuteFilter(item.CAPA_CD_ID, item.PATR_NM_NOME, item.PATR_NR_NUMERO_PATRIMONIO, item.FILI_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterPatr = listaObj;
                SessionMocks.listaPatrimonio = listaObj;
                return RedirectToAction("MontarTelaPatrimonio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPatrimonio");
            }
        }

        public ActionResult VoltarBasePatrimonio()
        {
            
            return RedirectToAction("MontarTelaPatrimonio");
        }

        [HttpGet]
        public ActionResult ExcluirPatrimonio(Int32 id)
        {
            
            // Prepara view
            PATRIMONIO item = patrApp.GetItemById(id);
            PatrimonioViewModel vm = Mapper.Map<PATRIMONIO, PatrimonioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirPatrimonio(PatrimonioViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PATRIMONIO item = Mapper.Map<PatrimonioViewModel, PATRIMONIO>(vm);
                Int32 volta = patrApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterPatr = new List<PATRIMONIO>();
                SessionMocks.listaPatrimonio = null;
                return RedirectToAction("MontarTelaPatrimonio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarPatrimonio(Int32 id)
        {
            
            // Prepara view
            PATRIMONIO item = patrApp.GetItemById(id);
            PatrimonioViewModel vm = Mapper.Map<PATRIMONIO, PatrimonioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarPatrimonio(PatrimonioViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PATRIMONIO item = Mapper.Map<PatrimonioViewModel, PATRIMONIO>(vm);
                Int32 volta = patrApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterPatr = new List<PATRIMONIO>();
                SessionMocks.listaPatrimonio = null;
                return RedirectToAction("MontarTelaPatrimonio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPatrimonio(Int32 id)
        {
            
            // Prepara view
            PATRIMONIO_ANEXO item = patrApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoPatrimonio()
        {
            
            return RedirectToAction("EditarPatrimonio", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadPatrimonio(Int32 id)
        {
            PATRIMONIO_ANEXO item = patrApp.GetAnexoById(id);
            String arquivo = item.PAAN_AQ_ARQUIVO;
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

        public ActionResult RetirarFiltroEquipamento()
        {
            
            SessionMocks.listaEquipamento = null;
            SessionMocks.filtroEquipamento = null;
            return RedirectToAction("MontarTelaequipamento");
        }

        public ActionResult MostrarTudoEquipamento()
        {
            
            listaMasterEqui = equiApp.GetAllItensAdm();
            SessionMocks.filtroEquipamento = null;
            SessionMocks.listaEquipamento = listaMasterEqui;
            return RedirectToAction("MontarTelaEquipamento");
        }

        public ActionResult VoltarBaseEquipamento()
        {
            
            return RedirectToAction("MontarTelaEquipamento");
        }

        [HttpGet]
        public ActionResult ExcluirEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO item = equiApp.GetItemById(id);
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirEquipamento(EquipamentoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                EQUIPAMENTO item = Mapper.Map<EquipamentoViewModel, EQUIPAMENTO>(vm);
                Int32 volta = equiApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterEqui = new List<EQUIPAMENTO>();
                SessionMocks.listaEquipamento = null;
                return RedirectToAction("MontarTelaEquipamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO item = equiApp.GetItemById(id);
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarEquipamento(EquipamentoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                EQUIPAMENTO item = Mapper.Map<EquipamentoViewModel, EQUIPAMENTO>(vm);
                Int32 volta = equiApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterEqui = new List<EQUIPAMENTO>();
                SessionMocks.listaEquipamento = null;
                return RedirectToAction("MontarTelaEquipamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO_ANEXO item = equiApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoEquipamento()
        {
            
            return RedirectToAction("EditarEquipamento", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadEquipamento(Int32 id)
        {
            EQUIPAMENTO_ANEXO item = equiApp.GetAnexoById(id);
            String arquivo = item.EQAN_AQ_ARQUIVO;
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
        public ActionResult VerManutencaoEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO_MANUTENCAO item = equiApp.GetItemManutencaoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult EditarProdutoFornecedor(Int32 id)
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            PRODUTO_FORNECEDOR item = prodApp.GetFornecedorById(id);
            objetoProdAntes = SessionMocks.produto;
            ProdutoFornecedorViewModel vm = Mapper.Map<PRODUTO_FORNECEDOR, ProdutoFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoFornecedor(ProdutoFornecedorViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    PRODUTO_FORNECEDOR item = Mapper.Map<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>(vm);
                    Int32 volta = prodApp.ValidateEditFornecedor(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoFornecedor(Int32 id)
        {
            
            PRODUTO_FORNECEDOR item = prodApp.GetFornecedorById(id);
            objetoProdAntes = SessionMocks.produto;
            item.PRFO_IN_ATIVO = 0;
            Int32 volta = prodApp.ValidateEditFornecedor(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ReativarProdutoFornecedor(Int32 id)
        {
            
            PRODUTO_FORNECEDOR item = prodApp.GetFornecedorById(id);
            objetoProdAntes = SessionMocks.produto;
            item.PRFO_IN_ATIVO = 1;
            Int32 volta = prodApp.ValidateEditFornecedor(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult IncluirProdutoFornecedor()
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            USUARIO usuario = SessionMocks.UserCredentials;
            PRODUTO_FORNECEDOR item = new PRODUTO_FORNECEDOR();
            ProdutoFornecedorViewModel vm = Mapper.Map<PRODUTO_FORNECEDOR, ProdutoFornecedorViewModel>(item);
            vm.PROD_CD_ID = SessionMocks.idVolta;
            vm.PRFO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoFornecedor(ProdutoFornecedorViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRODUTO_FORNECEDOR item = Mapper.Map<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = prodApp.ValidateCreateFornecedor(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult EditarProdutoGrade(Int32 id)
        {
            
            // Prepara view
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(), "TAMA_CD_ID", "TAMA_NM_NOME");
            PRODUTO_GRADE item = prodApp.GetGradeById(id);
            objetoProdAntes = SessionMocks.produto;
            ProdutoGradeViewModel vm = Mapper.Map<PRODUTO_GRADE, ProdutoGradeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoGrade(ProdutoGradeViewModel vm)
        {
            
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(), "TAMA_CD_ID", "TAMA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    PRODUTO_GRADE item = Mapper.Map<ProdutoGradeViewModel, PRODUTO_GRADE>(vm);
                    Int32 volta = prodApp.ValidateEditGrade(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoGrade(Int32 id)
        {
            
            PRODUTO_GRADE item = prodApp.GetGradeById(id);
            objetoProdAntes = SessionMocks.produto;
            item.PRGR_IN_ATIVO = 0;
            Int32 volta = prodApp.ValidateEditGrade(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ReativarProdutoGrade(Int32 id)
        {
            
            PRODUTO_GRADE item = prodApp.GetGradeById(id);
            objetoProdAntes = SessionMocks.produto;
            item.PRGR_IN_ATIVO = 1;
            Int32 volta = prodApp.ValidateEditGrade(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult IncluirProdutoGrade()
        {
            
            // Prepara view
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(), "TAMA_CD_ID", "TAMA_NM_NOME");
            USUARIO usuario = SessionMocks.UserCredentials;
            PRODUTO_GRADE item = new PRODUTO_GRADE();
            ProdutoGradeViewModel vm = Mapper.Map<PRODUTO_GRADE, ProdutoGradeViewModel>(item);
            vm.PROD_CD_ID = SessionMocks.idVolta;
            vm.PRGR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoGrade(ProdutoGradeViewModel vm)
        {
            
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(), "TAMA_CD_ID", "TAMA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRODUTO_GRADE item = Mapper.Map<ProdutoGradeViewModel, PRODUTO_GRADE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = prodApp.ValidateCreateGrade(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult SlideShow()
        {
            
            // Prepara view
            PRODUTO item = prodApp.GetItemById(SessionMocks.idVolta);
            objetoProdAntes = item;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SlideShowServico()
        {
            
            // Prepara view
            SERVICO item = servApp.GetItemById(SessionMocks.idVolta);
            objetoServAntes = item;
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SlideShowMateria()
        {
            
            // Prepara view
            MATERIA_PRIMA item = matApp.GetItemById(SessionMocks.idVolta);
            objetoMatAntes = item;
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SlideShowFornecedor()
        {
            
            // Prepara view
            FORNECEDOR item = fornApp.GetItemById(SessionMocks.idVolta);
            objetoFornAntes = item;
            FornecedorViewModel vm = Mapper.Map<FORNECEDOR, FornecedorViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SlideShowEquipamento()
        {
            
            // Prepara view
            EQUIPAMENTO item = equiApp.GetItemById(SessionMocks.idVolta);
            objetoEquiAntes = item;
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            return View(vm);
        }

        public ActionResult SlideShowPatrimonio()
        {
            
            // Prepara view
            PATRIMONIO item = patrApp.GetItemById(SessionMocks.idVolta);
            objetoPatrAntes = item;
            PatrimonioViewModel vm = Mapper.Map<PATRIMONIO, PatrimonioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SlideShowCliente()
        {
            
            // Prepara view
            CLIENTE item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoAntes = item;
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarContatoFornecedor(Int32 id)
        {
            
            // Prepara view
            FORNECEDOR_CONTATO item = fornApp.GetContatoById(id);
            objetoAntes = SessionMocks.cliente;
            FornecedorContatoViewModel vm = Mapper.Map<FORNECEDOR_CONTATO, FornecedorContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContatoFornecedor(FornecedorContatoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    FORNECEDOR_CONTATO item = Mapper.Map<FornecedorContatoViewModel, FORNECEDOR_CONTATO>(vm);
                    Int32 volta = fornApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoFornecedor");
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
        public ActionResult ExcluirContatoFornecedor(Int32 id)
        {
            
            FORNECEDOR_CONTATO item = fornApp.GetContatoById(id);
            objetoFornAntes = SessionMocks.fornecedor;
            item.FOCO_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoFornecedor");
        }

        [HttpGet]
        public ActionResult ReativarContatoFornecedor(Int32 id)
        {
            
            FORNECEDOR_CONTATO item = fornApp.GetContatoById(id);
            objetoFornAntes = SessionMocks.fornecedor;
            item.FOCO_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoFornecedor");
        }

        [HttpGet]
        public ActionResult IncluirContatoFornecedor()
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            FORNECEDOR_CONTATO item = new FORNECEDOR_CONTATO();
            FornecedorContatoViewModel vm = Mapper.Map<FORNECEDOR_CONTATO, FornecedorContatoViewModel>(item);
            vm.FORN_CD_ID = SessionMocks.idVolta;
            vm.FOCO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContatoFornecedor(FornecedorContatoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORNECEDOR_CONTATO item = Mapper.Map<FornecedorContatoViewModel, FORNECEDOR_CONTATO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = fornApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoFornecedor");
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
        public ActionResult MontarTelaFT()
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
            if (SessionMocks.listaFT == null)
            {
                listaMasterFt = ftApp.GetAllItens();
                SessionMocks.listaFT = listaMasterFt;
            }
            ViewBag.Listas = SessionMocks.listaFT;
            ViewBag.Title = "Fichas Técnicas";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");

            // Indicadores
            ViewBag.FT = ftApp.GetAllItens().Count;
            
            // Abre view
            objetoFt = new FICHA_TECNICA();
            if (SessionMocks.filtroFT != null)
            {
                objetoFt = SessionMocks.filtroFT;
            }
            return View(objetoFt);
        }

        [HttpGet]
        public ActionResult MontarTelaFTProd()
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
            if (SessionMocks.listaFT == null)
            {
                listaMasterFt = ftApp.GetAllItens();
                SessionMocks.listaFT = listaMasterFt;
            }
            ViewBag.Listas = SessionMocks.listaFT;
            ViewBag.Title = "Fichas Técnicas";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");

            // Indicadores
            ViewBag.FT = ftApp.GetAllItens().Count;

            // Abre view
            objetoFt = new FICHA_TECNICA();
            if (SessionMocks.filtroFT != null)
            {
                objetoFt = SessionMocks.filtroFT;
            }
            return View(objetoFt);
        }


        public ActionResult RetirarFiltroFT()
        {
            
            SessionMocks.listaFT = null;
            SessionMocks.filtroFT = null;
            return RedirectToAction("MontarTelaFT");
        }

        public ActionResult MostrarTudoFT()
        {
            
            listaMasterFt = ftApp.GetAllItensAdm();
            SessionMocks.filtroFT = null;
            SessionMocks.listaFT = listaMasterFt;
            return RedirectToAction("MontarTelaFT");
        }

        [HttpPost]
        public ActionResult FiltrarFT(FICHA_TECNICA item)
        {
            
            try
            {
                // Executa a operação
                List<FICHA_TECNICA> listaObj = new List<FICHA_TECNICA>();
                SessionMocks.filtroFT = item;
                Int32 volta = ftApp.ExecuteFilter(item.PROD_CD_ID, item.PRODUTO.CAPR_CD_ID, item.FITE_S_DESCRICAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterFt = listaObj;
                SessionMocks.listaFT = listaObj;
                return RedirectToAction("MontarTelaFT");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaFT");
            }
        }

        public ActionResult VoltarBaseFT()
        {
            
            return RedirectToAction("MontarTelaFT");
        }

        [HttpGet]
        public ActionResult EditarFT(Int32 id)
        {
            
            // Prepara view
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            FICHA_TECNICA item = ftApp.GetItemById(id);
            objetoFtAntes = item;
            SessionMocks.fichaTecnica = item;
            SessionMocks.idVolta = id;
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFT(FichaTecnicaViewModel vm)
        {
            
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFt = new List<FICHA_TECNICA>();
                    SessionMocks.listaFT = null;
                    return RedirectToAction("MontarTelaFT");
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
        public ActionResult VisualizarEditarFTProduto()
        {
            
            // Prepara view
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            FICHA_TECNICA item = ftApp.GetItemById(prodApp.GetItemById(SessionMocks.idVolta).FICHA_TECNICA.FirstOrDefault().FITE_CD_ID);
            objetoFtAntes = item;
            SessionMocks.fichaTecnica = item;
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisualizarEditarFTProduto(FichaTecnicaViewModel vm)
        {
            
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFt = new List<FICHA_TECNICA>();
                    SessionMocks.listaFT = null;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirFT(Int32 id)
        {
            
            // Prepara view
            FICHA_TECNICA item = ftApp.GetItemById(id);
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirFT(FichaTecnicaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                Int32 volta = ftApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                // Sucesso
                listaMasterFt = new List<FICHA_TECNICA>();
                SessionMocks.listaFT = null;
                return RedirectToAction("MontarTelaFT");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarFT(Int32 id)
        {
            
            // Prepara view
            FICHA_TECNICA item = ftApp.GetItemById(id);
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarFT(FichaTecnicaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                Int32 volta = ftApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterFt = new List<FICHA_TECNICA>();
                SessionMocks.listaFT = null;
                return RedirectToAction("MontarTelaFT");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult IncluirInsumoFT()
        {
            
            // Prepara view
            ViewBag.Insumos = new SelectList(matApp.GetAllItens(), "MAPR_CD_ID", "MAPR_NM_NOME");
            USUARIO usuario = SessionMocks.UserCredentials;
            FICHA_TECNICA_DETALHE item = new FICHA_TECNICA_DETALHE();
            FichaTecnicaDetalheViewModel vm = Mapper.Map<FICHA_TECNICA_DETALHE, FichaTecnicaDetalheViewModel>(item);
            vm.FITE_CD_ID = SessionMocks.idVolta;
            vm.FITD_DT_CADASTRO = DateTime.Today;
            vm.FITD_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirInsumoFT(FichaTecnicaDetalheViewModel vm)
        {
            
            ViewBag.Insumos = new SelectList(matApp.GetAllItens(), "MAPR_CD_ID", "MAPR_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FICHA_TECNICA_DETALHE item = Mapper.Map<FichaTecnicaDetalheViewModel, FICHA_TECNICA_DETALHE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = ftApp.ValidateCreateInsumo(item);
                    
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoFT");
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
        public ActionResult ExcluirInsumoFT(Int32 id)
        {
            
            FICHA_TECNICA_DETALHE item = ftApp.GetDetalheById(id);
            objetoFtAntes = SessionMocks.fichaTecnica;
            item.FITD_IN_ATIVO = 0;
            Int32 volta = ftApp.ValidateEditInsumo(item);
            return RedirectToAction("VoltarAnexoFT");
        }

        [HttpGet]
        public ActionResult ReativarInsumoFT(Int32 id)
        {
            
            FICHA_TECNICA_DETALHE item = ftApp.GetDetalheById(id);
            objetoFtAntes = SessionMocks.fichaTecnica;
            item.FITD_IN_ATIVO = 1;
            Int32 volta = ftApp.ValidateEditInsumo(item);
            return RedirectToAction("VoltarAnexoFT");
        }
    }
}