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

namespace SystemBRPresentation.Controllers
{
    public class GestaoComercialController : Controller
    {
        private readonly IContratoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IContratoSolicitacaoAprovacaoAppService solApp;
        private readonly IContaReceberAppService crApp;
        private readonly IContaBancariaAppService cbApp;

        private String msg;
        private Exception exception;
        CONTRATO objetoContrato = new CONTRATO();
        CONTRATO objetoContratoAntes = new CONTRATO();
        List<CONTRATO> listaContratoMaster = new List<CONTRATO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao = String.Empty;
        CONTA_RECEBER objetoCR = new CONTA_RECEBER();
        CONTA_RECEBER objetoCRAntes = new CONTA_RECEBER();

        public GestaoComercialController(IContratoAppService baseApps, ILogAppService logApps, IMatrizAppService matrizApps, IContratoSolicitacaoAprovacaoAppService solApps, IContaReceberAppService crApps, IContaBancariaAppService cbApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            matrizApp = matrizApps;
            solApp = solApps;
            crApp = crApps;
            cbApp = cbApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            CONTRATO item = new CONTRATO();
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            listaContratoMaster = new List<CONTRATO>();
            SessionMocks.listaContrato = null;
            return RedirectToAction("CarregarComercial", "BaseComercial");
        }

        [HttpGet]
        public ActionResult MontarTelaContrato()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaContrato == null)
            {
                listaContratoMaster = baseApp.GetAllItens();
                SessionMocks.listaContrato = listaContratoMaster;
            }
            ViewBag.Listas = SessionMocks.listaContrato;
            ViewBag.Title = "Contratosxx";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(), "TICT_CD_ID", "TICT_NM_NOME");
            ViewBag.Categorias = new SelectList(baseApp.GetAllCategorias(), "CACT_CD_ID", "CACT_NM_NOME");
            ViewBag.Status = new SelectList(baseApp.GetAllStatus(), "STCT_CD_ID", "STCT_NM_NOME");
            List<SelectListItem> etapas = new List<SelectListItem>();
            etapas.Add(new SelectListItem() { Text = "Aprovação", Value = "1" });
            etapas.Add(new SelectListItem() { Text = "Preparação", Value = "2" });
            etapas.Add(new SelectListItem() { Text = "Operação", Value = "3" });
            etapas.Add(new SelectListItem() { Text = "Encerramento", Value = "4" });
            etapas.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Fluxo = new SelectList(etapas, "Value", "Text");

            // Indicadores
            ViewBag.Contratos = baseApp.GetAllItens().Count;
            ViewBag.ContratosOperacao = baseApp.GetAllItensOperacao().Count;
            SessionMocks.emailEnviado = 0;

            // Abre view
            objetoContrato = new CONTRATO();
            if (SessionMocks.filtroContrato != null)
            {
                objetoContrato = SessionMocks.filtroContrato;
            }
            SessionMocks.voltaContrato = 1;
            return View(objetoContrato);
        }

        public ActionResult RetirarFiltroContrato()
        {
            SessionMocks.listaContrato = null;
            SessionMocks.filtroContrato = null;
            return RedirectToAction("MontarTelaContrato");
        }

        public ActionResult MostrarTudoContrato()
        {
            listaContratoMaster = baseApp.GetAllItensAdm();
            SessionMocks.filtroContrato = null;
            SessionMocks.listaContrato = listaContratoMaster;
            return RedirectToAction("MontarTelaContrato");
        }

        [HttpPost]
        public ActionResult FiltrarContrato(CONTRATO item)
        {
            try
            {
                // Executa a operação
                List<CONTRATO> listaObj = new List<CONTRATO>();
                SessionMocks.filtroContrato = item;
                Int32 volta = baseApp.ExecuteFilter(item.CACT_CD_ID, item.TICT_CD_ID, item.CONT_IN_WORKFLOW, item.CONT_NM_NOME, item.CONT_DS_DESCRICAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaContratoMaster = listaObj;
                SessionMocks.listaContrato = listaObj;
                return RedirectToAction("MontarTelaContrato");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaContrato");
            }
        }

        public ActionResult VoltarBaseContrato()
        {
            if (SessionMocks.voltaContrato == 1)
            {
                return RedirectToAction("MontarTelaContrato");
            }
            return RedirectToAction("WorkflowContrato", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarContrato()
        {
            return RedirectToAction("MontarTelaContrato");
        }

        [HttpGet]
        public ActionResult IncluirContrato()
        {
            // Prepara listas
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(), "TICT_CD_ID", "TICT_NM_NOME");
            ViewBag.Categorias = new SelectList(baseApp.GetAllCategorias(), "CACT_CD_ID", "CACT_NM_NOME");
            ViewBag.Centros = new SelectList(baseApp.GetAllCentros(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Clientes = new SelectList(baseApp.GetAllClientes(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllForma(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Nomenclaturas = new SelectList(baseApp.GetAllNomenclatura(), "NBSE_CD_ID", "NBSE_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Planos = new SelectList(baseApp.GetAllPlanoConta(), "PLCO_CD_ID", "PLCO_NM_CONTA");
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplates(), "TEMP_CD_ID", "TEMP_NM_NOME");
            ViewBag.Vendedores = new SelectList(baseApp.GetAllVendedores(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Status = new SelectList(baseApp.GetAllStatus(), "STCT_CD_ID", "STCT_NM_NOME");
            ViewBag.Responsaveis = new SelectList(baseApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");

            List<SelectListItem> periodoCobranca = new List<SelectListItem>();
            periodoCobranca.Add(new SelectListItem() { Text = "Nenhum", Value = "1" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Atual", Value = "2" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Anterior", Value = "3" });
            ViewBag.PeriodoCobranca = new SelectList(periodoCobranca, "Value", "Text");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTRATO item = new CONTRATO();
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante.Value;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.CONT_IN_ATIVO = 1;
            vm.CONT_DT_INICIO = DateTime.Today;
            vm.CONT_DT_FINAL = DateTime.Today.AddDays(365);
            vm.CONT_IN_STATUS = 1;
            return View(vm);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirContrato(ContratoViewModel vm)
        {
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(), "TICT_CD_ID", "TICT_NM_NOME");
            ViewBag.Categorias = new SelectList(baseApp.GetAllCategorias(), "CACT_CD_ID", "CACT_NM_NOME");
            ViewBag.Centros = new SelectList(baseApp.GetAllCentros(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Clientes = new SelectList(baseApp.GetAllClientes(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllForma(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Nomenclaturas = new SelectList(baseApp.GetAllNomenclatura(), "NBSE_CD_ID", "NBSE_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Planos = new SelectList(baseApp.GetAllPlanoConta(), "PLCO_CD_ID", "PLCO_NM_CONTA");
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplates(), "TEMP_CD_ID", "TEMP_NM_NOME");
            ViewBag.Vendedores = new SelectList(baseApp.GetAllVendedores(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Status = new SelectList(baseApp.GetAllStatus(), "STCT_CD_ID", "STCT_NM_NOME");
            ViewBag.Responsaveis = new SelectList(baseApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            List<SelectListItem> periodoCobranca = new List<SelectListItem>();
            periodoCobranca.Add(new SelectListItem() { Text = "Nenhum", Value = "1" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Atual", Value = "2" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Anterior", Value = "3" });
            ViewBag.PeriodoCobranca = new SelectList(periodoCobranca, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTRATO item = Mapper.Map<ContratoViewModel, CONTRATO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Contratos/" + item.CONT_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaContratoMaster = new List<CONTRATO>();
                    SessionMocks.listaContrato = null;
                    return RedirectToAction("MontarTelaContrato");
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
        public ActionResult EditarContrato(Int32 id)
        {
            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(), "TICT_CD_ID", "TICT_NM_NOME");
            ViewBag.Categorias = new SelectList(baseApp.GetAllCategorias(), "CACT_CD_ID", "CACT_NM_NOME");
            ViewBag.Centros = new SelectList(baseApp.GetAllCentros(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Clientes = new SelectList(baseApp.GetAllClientes(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllForma(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Nomenclaturas = new SelectList(baseApp.GetAllNomenclatura(), "NBSE_CD_ID", "NBSE_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Planos = new SelectList(baseApp.GetAllPlanoConta(), "PLCO_CD_ID", "PLCO_NM_CONTA");
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplates(), "TEMP_CD_ID", "TEMP_NM_NOME");
            ViewBag.Vendedores = new SelectList(baseApp.GetAllVendedores(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Status = new SelectList(baseApp.GetAllStatus(), "STCT_CD_ID", "STCT_NM_NOME");
            ViewBag.Responsaveis = new SelectList(baseApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");

            List<SelectListItem> periodoCobranca = new List<SelectListItem>();
            periodoCobranca.Add(new SelectListItem() { Text = "Nenhum", Value = "1" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Atual", Value = "2" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Anterior", Value = "3" });
            ViewBag.PeriodoCobranca = new SelectList(periodoCobranca, "Value", "Text");

            CONTRATO item = baseApp.GetItemById(id);
            objetoContratoAntes = item;
            SessionMocks.contrato = item;
            SessionMocks.idVolta = id;
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContrato(ContratoViewModel vm)
        {
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(), "TICT_CD_ID", "TICT_NM_NOME");
            ViewBag.Categorias = new SelectList(baseApp.GetAllCategorias(), "CACT_CD_ID", "CACT_NM_NOME");
            ViewBag.Centros = new SelectList(baseApp.GetAllCentros(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Clientes = new SelectList(baseApp.GetAllClientes(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllForma(), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Nomenclaturas = new SelectList(baseApp.GetAllNomenclatura(), "NBSE_CD_ID", "NBSE_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Planos = new SelectList(baseApp.GetAllPlanoConta(), "PLCO_CD_ID", "PLCO_NM_CONTA");
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplates(), "TEMP_CD_ID", "TEMP_NM_NOME");
            ViewBag.Vendedores = new SelectList(baseApp.GetAllVendedores(), "COLA_CD_ID", "COLA_NM_NOME");
            ViewBag.Status = new SelectList(baseApp.GetAllStatus(), "STCT_CD_ID", "STCT_NM_NOME");
            ViewBag.Responsaveis = new SelectList(baseApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            List<SelectListItem> periodoCobranca = new List<SelectListItem>();
            periodoCobranca.Add(new SelectListItem() { Text = "Nenhum", Value = "1" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Atual", Value = "2" });
            periodoCobranca.Add(new SelectListItem() { Text = "Mês Anterior", Value = "3" });
            ViewBag.PeriodoCobranca = new SelectList(periodoCobranca, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTRATO item = Mapper.Map<ContratoViewModel, CONTRATO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaContratoMaster = new List<CONTRATO>();
                    SessionMocks.listaContrato = null;
                    return RedirectToAction("MontarTelaContrato");
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
        public ActionResult ExcluirContrato(Int32 id)
        {
            // Prepara view
            CONTRATO item = baseApp.GetItemById(id);
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirContrato(ContratoViewModel vm)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CONTRATO item = Mapper.Map<ContratoViewModel, CONTRATO>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaContratoMaster = new List<CONTRATO>();
                SessionMocks.listaContrato = null;
                return RedirectToAction("MontarTelaContrato");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarContrato(Int32 id)
        {
            // Prepara view
            CONTRATO item = baseApp.GetItemById(id);
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarContrato(ContratoViewModel vm)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CONTRATO item = Mapper.Map<ContratoViewModel, CONTRATO>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaContratoMaster = new List<CONTRATO>();
                SessionMocks.listaContrato = null;
                return RedirectToAction("MontarTelaContrato");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoContratoPDF(Int32 id)
        {
            // Prepara view
            CONTRATO_ANEXO item = baseApp.GetAnexoById(id);
            String embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"500px\" height=\"300px\">";
            embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            embed += "</object>";
            TempData["Embed"] = string.Format(embed, VirtualPathUtility.ToAbsolute(item.COAN_AQ_ARQUIVO));
            return View(item);
        }

        [HttpPost]
        public ActionResult VerAnexoContratoPDF()
        {
            return RedirectToAction("EditarContrato", new { id = SessionMocks.idVolta });
        }

        public ActionResult VerAnexoContrato(Int32 id)
        {
            // Prepara view
            CONTRATO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoContrato()
        {
            return RedirectToAction("EditarContrato", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarWorkflowContrato()
        {
            return RedirectToAction("WorkflowContrato", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadContrato(Int32 id)
        {
            CONTRATO_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.COAN_AQ_ARQUIVO;
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
        public ActionResult UploadFileContrato(HttpPostedFileBase file)
        {
            if (file == null)
                return Content("Nenhum arquivo selecionado");

            CONTRATO item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Contratos/" + item.CONT_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTRATO_ANEXO foto = new CONTRATO_ANEXO();
            foto.COAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.COAN_DT_ANEXO = DateTime.Today;
            foto.COAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.COAN_IN_TIPO = tipo;
            foto.COAN_NM_TITULO = fileName;
            foto.CONT_CD_ID = item.CONT_CD_ID;

            item.CONTRATO_ANEXO.Add(foto);
            objetoContratoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarAnexoContrato");
        }

        [HttpGet]
        public ActionResult VisualizarContrato(Int32 id)
        {
            // Prepara view
            CONTRATO item = baseApp.GetItemById(id);
            objetoContratoAntes = item;
            SessionMocks.contrato = item;
            SessionMocks.idVolta = id;
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult WorkflowContrato(Int32 id)
        {
            // Prepara view
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplates(), "TEMP_CD_ID", "TEMP_NM_NOME");
            ViewBag.Responsaveis = new SelectList(baseApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            SessionMocks.voltaContrato = 2;
            ViewBag.Usuario = SessionMocks.UserCredentials;

            ViewBag.Atrasos = baseApp.GetItensAtrasoContrato(id);
            SessionMocks.contratoAtraso = baseApp.GetItensAtrasoContrato(id).Count;

            CONTRATO item = baseApp.GetItemById(id);
            ViewBag.comentarios = item.CONTRATO_COMENTARIO.Count;
            ViewBag.Abertos = crApp.GetItensAbertoContrato(id);
            objetoContratoAntes = item;
            SessionMocks.contrato = item;
            SessionMocks.idVolta = id;
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            if (item.CONT_IN_DIAS_MENS_RENOVACAO != null)
            {
                ViewBag.AvisoFinal = item.CONT_DT_FINAL.Value.AddDays(item.CONT_IN_DIAS_MENS_RENOVACAO.Value * (-1)).ToShortDateString();
            }
            else
            {
                ViewBag.AvisoFinal = item.CONT_DT_FINAL.Value.AddDays(-5).ToShortDateString();
            }

            // Ajustes
            if (vm.CONT_IN_WORKFLOW == 1)
            {
                if (vm.CONT_IN_ENVIO_APROVACAO == 0)
                {
                    vm.CONT_DT_SOLICITACAO_APROVACAO = DateTime.Today;
                    vm.CONT_DS_APROVACAO = baseApp.GetTextoAprovacao();
                }
                else
                {
                    vm.CONT_NM_RESPONSAVEL = baseApp.GetResponsavelById(vm.CONT_CD_RESPONSAVEL.Value).COLA_NM_NOME;
                }
            }
            if (SessionMocks.emailEnviado == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0044", CultureInfo.CurrentCulture);
                SessionMocks.emailEnviado = 0;
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult SolicitarAprovacaoContrato()
        {
            // Prepara view
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplates(), "TEMP_CD_ID", "TEMP_NM_NOME");
            ViewBag.Responsaveis = new SelectList(baseApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");

            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoContratoAntes = item;
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            vm.CONT_DT_SOLICITACAO_APROVACAO = DateTime.Today;
            vm.CONT_DS_APROVACAO = baseApp.GetTextoAprovacao();
            return View(vm);
        }

        [HttpPost]
        public ActionResult SolicitarAprovacaoContrato(ContratoViewModel vm)
        {
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplates(), "TEMP_CD_ID", "TEMP_NM_NOME");
            ViewBag.Responsaveis = new SelectList(baseApp.GetAllResponsaveis(), "COLA_CD_ID", "COLA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTRATO cont = Mapper.Map<ContratoViewModel, CONTRATO>(vm);
                    Int32 volta = baseApp.EmitirAprovacaoContrato(cont, usuarioLogado);

                    // Exibe mensagem
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0039", CultureInfo.CurrentCulture);

                    // Sucesso
                    return RedirectToAction("VoltarWorkflowContrato");
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
        public ActionResult CancelarContrato()
        {
            // Prepara view
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoContratoAntes = item;
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            vm.CONT_DT_CANCELAMENTO = DateTime.Today;
            vm.CONT_DS_JUSTIFICATIVA = baseApp.GetTextoCancelamento();
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelarContrato(ContratoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTRATO cont = Mapper.Map<ContratoViewModel, CONTRATO>(vm);
                    Int32 volta = baseApp.CancelarContrato(cont, usuarioLogado);

                    // Exibe mensagem
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);

                    // Sucesso
                    return RedirectToAction("VoltarWorkflowContrato");
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
        public ActionResult GerarRetornoAprovacaoContrato()
        {
            // Prepara view
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoContratoAntes = item;
            List<SelectListItem> statusResposta = new List<SelectListItem>();
            statusResposta.Add(new SelectListItem() { Text = "Aprovado", Value = "1" });
            statusResposta.Add(new SelectListItem() { Text = "Não Aprovado", Value = "0" });
            ViewBag.StatusResposta = new SelectList(statusResposta, "Value", "Text");

            ContratoSolicitacaoAprovacaoViewModel vm = new ContratoSolicitacaoAprovacaoViewModel();
            vm.CONT_CD_ID = item.CONT_CD_ID;
            vm.CONTRATO = item;
            vm.CTSA_DT_DATA = DateTime.Today.Date;
            vm.CTSA_IN_ATIVO = 1;
            vm.CTSA_IN_STATUS = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarRetornoAprovacaoContrato(ContratoSolicitacaoAprovacaoViewModel vm)
        {
            List<SelectListItem> statusResposta = new List<SelectListItem>();
            statusResposta.Add(new SelectListItem() { Text = "Aprovado", Value = "1" });
            statusResposta.Add(new SelectListItem() { Text = "Não Aprovado", Value = "0" });
            ViewBag.StatusResposta = new SelectList(statusResposta, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTRATO_SOLICITACAO_APROVACAO cont = Mapper.Map<ContratoSolicitacaoAprovacaoViewModel, CONTRATO_SOLICITACAO_APROVACAO>(vm);

                    CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
                    objetoContratoAntes = item;
                    Int32 volta = baseApp.ValidateRespostaAprovacao(item, objetoContratoAntes, cont, usuarioLogado);

                    // Exibe mensagem
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture);

                    // Sucesso
                    return RedirectToAction("VoltarWorkflowContrato");
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
        public ActionResult VerRetornoAprovacaoContrato(Int32 id)
        {
            // Prepara view
            CONTRATO_SOLICITACAO_APROVACAO item = solApp.GetItemById(id);
            ContratoSolicitacaoAprovacaoViewModel vm = Mapper.Map<CONTRATO_SOLICITACAO_APROVACAO, ContratoSolicitacaoAprovacaoViewModel>(item);
            vm.CTSA_NM_STATUS = vm.CTSA_IN_STATUS == 1 ? "Aprovado" : "Não Aprovado";
            return View(vm);
        }

        public ActionResult LembrarAprovacaoContrato()
        {
            // Prepara view
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            Int32 volta = baseApp.LembrarAprovacaoContrato(item, usuarioLogado);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult MudarStatusContratoPreparacao()
        {
            // Recupera Contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Verifica condições
            if (item.CONT_IN_CANCELADO == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_APROVADO == 0)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_WORKFLOW != 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }

            // Prepara view
            item.CONT_IN_WORKFLOW = 2;
            item.CONT_IN_PREPARADO = 0;

            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            objetoContratoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult GerarParcelasContrato()
        {
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            String dataInicio = item.CONT_NR_DIA_PAGAMENTO.Value.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
            DateTime dataParcela = Convert.ToDateTime(dataInicio);
            if (dataParcela.Date <= DateTime.Today.Date)
            {
                dataParcela = dataParcela.AddMonths(1);
            }

            for (int i = 1; i <= item.CONT_NR_PARCELAS; i++)
            {
                CONTRATO_PARCELAS parc = new CONTRATO_PARCELAS();
                parc.CONT_CD_ID = item.CONT_CD_ID;
                parc.CTPA_DT_RECEBIDO = null;
                parc.CTPA_DT_VENCIMENTO = dataParcela;
                parc.CTPA_IN_ATIVO = 1;
                parc.CTPA_IN_QUITADA = 0;
                parc.CTPA_NR_NUMERO = i.ToString() + "/" + item.CONT_NR_PARCELAS.Value.ToString();
                parc.CTPA_VL_RECEBIDO = 0;
                parc.CTPA_VL_VALOR = item.CONT_VL_PARCELA;
                item.CONTRATO_PARCELAS.Add(parc);
                if (item.PERIODICIDADE.PERI_NM_NOME == "Mensal")
                {
                    dataParcela = dataParcela.AddMonths(1);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Bimestral")
                {
                    dataParcela = dataParcela.AddMonths(2);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Trimestral")
                {
                    dataParcela = dataParcela.AddMonths(3);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Semestral")
                {
                    dataParcela = dataParcela.AddMonths(6);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Anual")
                {
                    dataParcela = dataParcela.AddYears(1);
                }
                else
                {
                    dataParcela = dataParcela.AddDays(item.PERIODICIDADE.PERI_NR_DIAS);
                }
            }

            item.CONT_IN_PARCELA_GERADA = 1;
            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult GerarLancamentoCRContrato()
        {
            // Recupera contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Recupera conta padrao
            CONTA_BANCARIA padrao = cbApp.GetContaPadrao();

            // Acerta datas
            String dataInicio = item.CONT_NR_DIA_PAGAMENTO.Value.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
            DateTime dataParcela = Convert.ToDateTime(dataInicio);
            if (dataParcela.Date <= DateTime.Today.Date)
            {
                dataParcela = dataParcela.AddMonths(1);
            }

            // Gera lançamentos
            for (int i = 1; i <= item.CONT_NR_PARCELAS; i++)
            {
                CONTA_RECEBER parc = new CONTA_RECEBER();
                USUARIO usuario = SessionMocks.UserCredentials;
                parc.ASSI_CD_ID = usuario.ASSI_CD_ID;
                parc.CARE_DS_DESCRICAO = "Parcela" + i.ToString() + "/" + item.CONT_NR_PARCELAS.Value.ToString() + " do contrato " + item.CONT_NM_NUMERO_CONTRATO;
                parc.CARE_DT_LANCAMENTO = DateTime.Today.Date;
                parc.CARE_DT_VENCIMENTO = dataParcela;
                parc.CARE_IN_ATIVO = 1;
                parc.CARE_IN_LIQUIDADA = 0;
                parc.CARE_IN_TIPO_LANCAMENTO = 2;
                parc.CARE_NM_FAVORECIDO = usuario.ASSINANTE.ASSI_NM_EMPRESA;
                parc.CARE_NM_FORMA_PAGAMENTO = item.FORMA_PAGAMENTO.FOPA_NM_NOME;
                parc.CARE_VL_VALOR = item.CONT_VL_PARCELA.Value;
                parc.CLIE_CD_ID = item.CLIE_CD_ID;
                parc.CONT_CD_ID = item.CONT_CD_ID;
                parc.FILI_CD_ID = usuario.COLABORADOR.FILI_CD_ID;
                parc.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
                parc.PLCO_CD_ID = item.PLCO_CD_ID.Value;
                parc.USUA_CD_ID = usuario.USUA_CD_ID;
                parc.COBA_CD_ID = padrao.COBA_CD_ID;
                parc.COLA_CD_ID = item.COLA_CD_ID;
                
                item.CONTA_RECEBER.Add(parc);
                if (item.PERIODICIDADE.PERI_NM_NOME == "Mensal")
                {
                    dataParcela = dataParcela.AddMonths(1);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Bimestral")
                {
                    dataParcela = dataParcela.AddMonths(2);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Trimestral")
                {
                    dataParcela = dataParcela.AddMonths(3);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Semestral")
                {
                    dataParcela = dataParcela.AddMonths(6);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Quinzenal")
                {
                    dataParcela = dataParcela.AddDays(15);
                }
                else if (item.PERIODICIDADE.PERI_NM_NOME == "Anual")
                {
                    dataParcela = dataParcela.AddYears(1);
                }
                else
                {
                    dataParcela = dataParcela.AddDays(item.PERIODICIDADE.PERI_NR_DIAS);
                }
            }

            // Grava contrato
            item.CONT_IN_CR_GERADA = 1;
            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult MudarStatusContratoAprovacao()
        {
            // Recupera Contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Verifica condições
            if (item.CONT_IN_CANCELADO == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_WORKFLOW != 2)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }

            // Prepara objeto
            item.CONT_IN_WORKFLOW = 1;
            item.CONT_IN_PREPARADO = 0;




            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            Int32 volta = baseApp.LembrarAprovacaoContrato(item, usuarioLogado);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult MudarStatusContratoOperacao()
        {
            // Recupera Contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Verifica condições
            if (item.CONT_IN_CANCELADO == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_APROVADO == 0)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_WORKFLOW != 2)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_PARCELA_GERADA == 0)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_SINCRONIZA == 1)
            {
                if (item.CONT_IN_CR_GERADA == 0)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                    return RedirectToAction("VoltarWorkflowContrato");
                }
            }        

            // Prepara view
            item.CONT_IN_WORKFLOW = 3;

            ViewBag.Atrasos = baseApp.GetItensAtrasoContrato(SessionMocks.idVolta);
            SessionMocks.contratoAtraso = baseApp.GetItensAtrasoContrato(SessionMocks.idVolta).Count;
            
            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            objetoContratoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult MudarStatusContratoEncerramento()
        {
            // Recupera Contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Verifica condições
            if (item.CONT_IN_CANCELADO == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_APROVADO == 0)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_WORKFLOW != 3)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }

            // Prepara view
            item.CONT_IN_WORKFLOW = 4;

            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            objetoContratoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult ContratoEncerramentoNormal()
        {
            // Recupera Contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Verifica condições
            if (item.CONT_IN_CANCELADO == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_APROVADO == 0)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_WORKFLOW != 4)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }

            // Prepara view
            item.CONT_IN_WORKFLOW = 5;
            item.CONT_DT_ENCERRAMENTO = DateTime.Today.Date;
            item.CONT_IN_ENCERRADO = 1;

            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            objetoContratoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult ContratoEncerramentoAberto()
        {
            // Recupera Contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Verifica condições
            if (item.CONT_IN_CANCELADO == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_APROVADO == 0)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_WORKFLOW != 4)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }

            // Prepara view
            item.CONT_IN_WORKFLOW = 5;
            item.CONT_DT_ENCERRAMENTO = DateTime.Today.Date;
            item.CONT_IN_ENCERRADO = 1;

            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            objetoContratoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);

            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult ContratoProrrogacao(Int32 dias)
        {
            // Recupera Contrato
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);

            // Verifica condições
            if (item.CONT_IN_CANCELADO == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_APROVADO == 0)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }
            if (item.CONT_IN_WORKFLOW != 4)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarWorkflowContrato");
            }

            // Prepara view
            item.CONT_IN_WORKFLOW = 3;
            item.CONT_DT_ENCERRAMENTO = item.CONT_DT_ENCERRAMENTO.Value.AddDays(dias);
            item.CONT_IN_PRORROGADO = 1;

            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            objetoContratoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoContratoAntes);
            return RedirectToAction("VoltarWorkflowContrato");
        }


        [HttpGet]
        public ActionResult ManipularFinanceiroContrato(Int32 id)
        {
            // Prepara view
            CONTA_RECEBER item = baseApp.GetCRById(id);
            objetoCRAntes = item;
            SessionMocks.cr = item;
            SessionMocks.idCRVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManipularFinanceiroContrato(ContaReceberViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    Int32 volta = crApp.ValidateEdit(item, objetoCRAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VoltarWorkflowContrato");
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
        public ActionResult ReceberParcelaContrato(Int32 id)
        {
            // Prepara view
            CONTA_RECEBER item = baseApp.GetCRById(id);
            objetoCRAntes = item;
            SessionMocks.cr = item;
            SessionMocks.idCRVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_DT_DATA_LIQUIDACAO = vm.CARE_DT_VENCIMENTO;
            vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_VALOR;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceberParcelaContrato(ContaReceberViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    Int32 volta = crApp.ValidateEditReceber(item, objetoCRAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VoltarWorkflowContrato");
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

        public ActionResult EmitirAvisoParcelaAtraso(Int32 id)
        {
            // Prepara view
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoContratoAntes = item;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;

            CONTA_RECEBER cr = baseApp.GetCRById(id);
            objetoCRAntes = cr;
            SessionMocks.cr = cr;
            SessionMocks.idCRVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(cr);
            Int32 volta = crApp.EmitirAvisoParcelaAtraso(item, cr, usuarioLogado);
            SessionMocks.emailEnviado = 1;
            ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0044", CultureInfo.CurrentCulture);
            return RedirectToAction("VoltarWorkflowContrato");
        }

        public ActionResult MontarTelaComentariosContrato()
        {
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            ContratoViewModel vm = Mapper.Map<CONTRATO, ContratoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadFileLancamento(HttpPostedFileBase file)
        {
            if (file == null)
                return Content("Nenhum arquivo selecionado");

            CONTA_RECEBER item = crApp.GetById(SessionMocks.idCRVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.Value.ToString() + "/Financeiro/CR/Contrato/" + item.CONT_CD_ID.ToString() + "/" + item.CARE_CD_ID.ToString() + "/Anexos/";
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
            return RedirectToAction("VoltarWorkflowContrato");
        }

        [HttpGet]
        public ActionResult VerAnexoLancamento(Int32 id)
        {
            // Prepara view
            CONTA_RECEBER_ANEXO item = crApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadLancamento(Int32 id)
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

        [HttpGet]
        public ActionResult IncluirComentarioContrato()
        {
            CONTRATO item = baseApp.GetItemById(SessionMocks.idVolta);
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            CONTRATO_COMENTARIO coment = new CONTRATO_COMENTARIO();
            ContratoComentarioViewModel vm = Mapper.Map<CONTRATO_COMENTARIO, ContratoComentarioViewModel>(coment);
            vm.COCO_DT_COMENTARIO = DateTime.Today;
            vm.COCO_IN_ATIVO = 1;
            vm.CONT_CD_ID = item.CONT_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirComentarioContrato(ContratoComentarioViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTRATO_COMENTARIO item = Mapper.Map<ContratoComentarioViewModel, CONTRATO_COMENTARIO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTRATO not = baseApp.GetItemById(SessionMocks.idVolta);

                    item.USUARIO = null;
                    not.CONTRATO_COMENTARIO.Add(item);
                    objetoContratoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoContratoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("MontarTelaComentariosContrato", new { id = SessionMocks.idVolta });
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