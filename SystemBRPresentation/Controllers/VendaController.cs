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
using System.Collections;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class VendaController : Controller
    {
        private readonly IPedidoVendaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IClienteAppService forApp;
        private readonly IProdutoAppService proApp;
        private readonly IMateriaPrimaAppService insApp;
        private readonly IProdutoEstoqueFilialAppService pefApp;

        private String msg;
        private Exception exception;
        PEDIDO_VENDA objeto = new PEDIDO_VENDA();
        PEDIDO_VENDA objetoAntes = new PEDIDO_VENDA();
        List<PEDIDO_VENDA> listaMaster = new List<PEDIDO_VENDA>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public VendaController(IPedidoVendaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, ICentroCustoAppService ccApps, IClienteAppService forApps, IProdutoAppService proApps, IMateriaPrimaAppService insApps, IProdutoEstoqueFilialAppService pefApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            ccApp = ccApps;
            forApp = forApps;
            proApp = proApps;
            insApp = insApps;
            pefApp = pefApps;
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

        [HttpPost]
        public JsonResult GetInfoProduto(Int32 id)
        {
            PRODUTO item = proApp.GetById(id);
            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial(id, SessionMocks.idFilial);

            Hashtable result = new Hashtable();
            result.Add("precoVenda", item.PROD_VL_PRECO_VENDA);
            result.Add("precoPromo", item.PRTP_VL_PRECO_PROMOCAO);
            result.Add("qtdeEstoque", pef.PREF_QN_ESTOQUE);

            return Json(result);
        }

        public FileResult DownloadPedidoVenda(Int32 id)
        {
            PEDIDO_VENDA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.PEVA_AQ_ARQUIVO;
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
        public ActionResult MontarTelaPedidoVenda()
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
            if (SessionMocks.listaVenda == null)
            {
                listaMaster = baseApp.GetAllItens();
                SessionMocks.listaVenda = listaMaster;
            }
            ViewBag.Listas = SessionMocks.listaVenda;
            ViewBag.Title = "Pedidos de Venda";

            // Indicadores
            ViewBag.Pedidos = SessionMocks.listaVenda.Count;
            ViewBag.Encerradas = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 5).ToList().Count;
            ViewBag.Canceladas = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 6).ToList().Count;
            ViewBag.Atrasadas = SessionMocks.listaVenda.Where(p => p.PEVE_DT_PREVISTA < DateTime.Today.Date).ToList().Count;
            ViewBag.EncerradasLista = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 5).ToList();
            ViewBag.CanceladasLista = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 6).ToList();
            ViewBag.AtrasadasLista = SessionMocks.listaVenda.Where(p => p.PEVE_DT_PREVISTA < DateTime.Today.Date).ToList();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Carrega listas
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");

            // Abre view
            objeto = new PEDIDO_VENDA();
            objeto.PEVE_DT_DATA = DateTime.Today.Date;
            return View(objeto);
        }

        public ActionResult RetirarFiltroPedidoVenda()
        {
            
            SessionMocks.listaVenda = null;
            return RedirectToAction("MontarTelaPedidoVenda");
        }

        public ActionResult MostrarTudoPedidoVenda()
        {
            
            listaMaster = baseApp.GetAllItensAdm();
            SessionMocks.listaVenda = listaMaster;
            return RedirectToAction("MontarTelaPedidoVenda");
        }

        [HttpPost]
        public ActionResult FiltrarPedidoVenda(PEDIDO_VENDA item)
        {
            
            try
            {
                // Executa a operação
                List<PEDIDO_VENDA> listaObj = new List<PEDIDO_VENDA>();
                Int32 volta = baseApp.ExecuteFilter(item.USUA_CD_ID, item.PEVE_NM_NOME, item.PEVE_NR_NUMERO, item.PEVE_DT_DATA, item.PEVE_IN_STATUS, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMaster = listaObj;
                SessionMocks.listaVenda = listaObj;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPedidoVenda");

            }
        }

        public ActionResult VoltarBaseMontarTelaPedidoVenda()
        {
            
            return RedirectToAction("MontarTelaPedidoVenda");
        }

                [HttpGet]
        public ActionResult IncluirPedidoVenda()
        {
            
            // Prepara listas
            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            ViewBag.CentroCusto = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Cliente = new SelectList(forApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            PEDIDO_VENDA item = new PEDIDO_VENDA();
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.PEVE_DT_DATA = DateTime.Today.Date;
            vm.PEVE_IN_ATIVO = 1;
            vm.PEVE_IN_STATUS = 3;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.USUA_CD_ID = SessionMocks.UserCredentials.USUA_CD_ID;
            vm.PEVE_DT_PREVISTA = DateTime.Today.Date.AddDays(30);
            vm.PEVE_NR_NUMERO = "1";
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirPedidoVenda(PedidoVendaViewModel vm)
        {
            
            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Cliente = new SelectList(forApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            ViewBag.CentroCusto = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0099", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Acerta numero do pedido
                    item.PEVE_NR_NUMERO = item.PEVE_CD_ID.ToString();
                    volta = baseApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/PedidoVenda/" + item.PEVE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    
                    // Sucesso
                    listaMaster = new List<PEDIDO_VENDA>();
                    SessionMocks.listaVenda = null;

                    SessionMocks.idVolta = item.PEVE_CD_ID;
                    return Json(item.PEVE_CD_ID); //RedirectToAction("EditarPedidoVenda", new { id = SessionMocks.idVolta });
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
        public ActionResult EditarPedidoVenda(Int32 id)
        {
            
            // Prepara view
            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Cliente = new SelectList(forApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CentroCusto = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            PEDIDO_VENDA item = baseApp.GetItemById(id);
            objetoAntes = item;
            SessionMocks.pedidoVenda = item;
            SessionMocks.idVolta = id;
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPedidoVenda(PedidoVendaViewModel vm)
        {
            
            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Cliente = new SelectList(forApp.GetAllItens(), "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CentroCusto = new SelectList(ccApp.GetAllItens(), "CECU_CD_ID", "CECU_NM_NOME");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<PEDIDO_VENDA>();
                    SessionMocks.listaVenda = null;
                    return RedirectToAction("MontarTelaPedidoVenda");
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
        public ActionResult ExcluirPedidoVenda(Int32 id)
        {
            
            // Prepara view
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirPedidoVenda(PedidoVendaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0100", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_VENDA>();
                SessionMocks.listaVenda = null;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarPedidoVenda(Int32 id)
        {
            
            // Prepara view
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarPedidoVenda(PedidoVendaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<PEDIDO_VENDA>();
                SessionMocks.listaVenda = null;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPedidoVenda(Int32 id)
        {
            
            // Prepara view
            PEDIDO_VENDA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoPedidoVenda()
        {
            
            return RedirectToAction("EditarPedidoVenda", new { id = SessionMocks.idVolta });
        }

        [HttpPost]
        public ActionResult UploadFilePedidoVenda(HttpPostedFileBase file)
        {
            
            if (file == null)
                return Content("Nenhum arquivo selecionado");

            PEDIDO_VENDA item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture);
                return RedirectToAction("VoltarAnexoPedidoVenda");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/PedidoVenda/" + item.PEVE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PEDIDO_VENDA_ANEXO foto = new PEDIDO_VENDA_ANEXO();
            foto.PEVA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PEVA_DT_ANEXO = DateTime.Today;
            foto.PEVA_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.PEVA_IN_TIPO = tipo;
            foto.PEVA_NM_TITULO = fileName;
            foto.PEVE_CD_ID = item.PEVE_CD_ID;

            item.PEDIDO_VENDA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoPedidoVenda");
        }

        [HttpPost]
        public JsonResult UploadFilePedidoVenda_Inclusao(IEnumerable<HttpPostedFileBase> files, Int32 perfil)
        {
            var count = 0;

            if (perfil == 0)
            {
                count++;
            }

            foreach (var file in files)
            {
                if (count == 0)
                {
                    //UploadFotoCliente(file);

                    //count++;
                }
                else
                {
                    UploadFilePedidoVenda(file);
                }
            }

            return Json("1"); // VoltarAnexoCliente
        }

        [HttpGet]
        public ActionResult EnviarOrcamentoPedidoVenda(Int32 id)
        {
            
            // Prepara view
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            Int32 volta = baseApp.ValidateEnvioAprovacao(item);

            // Verifica retorno
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0102", CultureInfo.CurrentCulture);
                return View(vm);
            }

            // Sucesso
            listaMaster = new List<PEDIDO_VENDA>();
            SessionMocks.listaVenda = null;
            return RedirectToAction("MontarTelaPedidoVenda");
        }

        [HttpGet]
        public ActionResult EnviarAprovacaoPedidoVenda(Int32 id)
        {
            
            PEDIDO_VENDA item = baseApp.GetById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EnviarAprovacaoPedidoVenda(PedidoVendaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                Int32 volta = baseApp.ValidateEnvioAprovacao(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0102", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_VENDA>();
                SessionMocks.listaVenda = null;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult AprovarPedidoVenda(Int32 id)
        {
            
            // Prepara view
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult AprovarPedidoVenda(PedidoVendaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                Int32 volta = baseApp.ValidateAprovacao(item);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<PEDIDO_VENDA>();
                SessionMocks.listaVenda = null;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult EncerrarPedidoVenda(Int32 id)
        {
            
            // Prepara view
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EncerrarPedidoVenda(PedidoVendaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                Int32 volta = baseApp.ValidateEncerramento(item);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<PEDIDO_VENDA>();
                SessionMocks.listaVenda = null;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult CancelarPedidoVenda(Int32 id)
        {
            
            // Prepara view
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelarPedidoVenda(PedidoVendaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                Int32 volta = baseApp.ValidateCancelamento(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0105", CultureInfo.CurrentCulture);
                    return View(vm);
                }
                // Sucesso
                listaMaster = new List<PEDIDO_VENDA>();
                SessionMocks.listaVenda = null;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        public ActionResult VerPedidoVenda(Int32 id)
        {
            
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        public ActionResult VerPedidoVendaUsuario(Int32 id)
        {
            
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        public ActionResult VerItemPedidoVenda(Int32 id)
        {
            
            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            ItemPedidoVendaViewModel vm = Mapper.Map<ITEM_PEDIDO_VENDA, ItemPedidoVendaViewModel>(item);
            return View(vm);
        }

        public ActionResult VerItemPedidoVendaUsuario(Int32 id)
        {
            
            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            ItemPedidoVendaViewModel vm = Mapper.Map<ITEM_PEDIDO_VENDA, ItemPedidoVendaViewModel>(item);
            return View(vm);
        }


        public ActionResult VerAtrasados()
        {
            
            // Prepara view
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");

            // Abre view
            ViewBag.Pedidos = SessionMocks.listaCompra.Count;
            ViewBag.Atrasadas = SessionMocks.listaVenda.Where(p => p.PEVE_DT_PREVISTA < DateTime.Today.Date).ToList().Count;
            ViewBag.AtrasadasLista = SessionMocks.listaVenda.Where(p => p.PEVE_DT_PREVISTA < DateTime.Today.Date).ToList();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            objeto = new PEDIDO_VENDA();
            SessionMocks.voltaVenda = 1;
            SessionMocks.voltaConsulta = 3;
            return View(objeto);
        }

        public ActionResult VerEncerrados()
        {
            
            // Prepara view
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");

            // Abre view
            ViewBag.Pedidos = SessionMocks.listaCompra.Count;
            ViewBag.Encerradas = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 5).ToList().Count;
            ViewBag.EncerradasLista = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 5).ToList();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            objeto = new PEDIDO_VENDA();
            SessionMocks.voltaVenda = 1;
            SessionMocks.voltaConsulta = 3;
            return View(objeto);
        }

        public ActionResult VerCancelados()
        {
            
            // Prepara view
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");

            // Abre view
            ViewBag.Pedidos = SessionMocks.listaCompra.Count;
            ViewBag.Canceladas = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 6).ToList().Count;
            ViewBag.CanceladasLista = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 6).ToList();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            objeto = new PEDIDO_VENDA();
            SessionMocks.voltaCompra = 1;
            SessionMocks.voltaConsulta = 3;
            return View(objeto);
        }

        [HttpGet]
        public ActionResult EditarItemPedidoVendaUsuario(Int32 id)
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");

            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            objetoAntes = SessionMocks.pedidoVenda;
            ItemPedidoVendaViewModel vm = Mapper.Map<ITEM_PEDIDO_VENDA, ItemPedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarItemPedidoVendaUsuario(ItemPedidoVendaViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ITEM_PEDIDO_VENDA item = Mapper.Map<ItemPedidoVendaViewModel, ITEM_PEDIDO_VENDA>(vm);
                    Int32 volta = baseApp.ValidateEditItemVenda(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoPedidoVendaUsuario");
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
        public ActionResult ExcluirItemPedidoVendaUsuario(Int32 id)
        {
            
            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            objetoAntes = SessionMocks.pedidoVenda;
            item.ITPE_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditItemVenda(item);
            return RedirectToAction("VoltarAnexoPedidoVendaUsuario");
        }

        [HttpGet]
        public ActionResult ReativarItemPedidoVendaUsuario(Int32 id)
        {
            
            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            objetoAntes = SessionMocks.pedidoVenda;
            item.ITPE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditItemVenda(item);
            return RedirectToAction("VoltarAnexoPedidoVendaUsuario");
        }

        [HttpGet]
        public ActionResult IncluirItemPedidoVendaUsuario()
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            USUARIO usuario = SessionMocks.UserCredentials;
            ITEM_PEDIDO_VENDA item = new ITEM_PEDIDO_VENDA();
            ItemPedidoVendaViewModel vm = Mapper.Map<ITEM_PEDIDO_VENDA, ItemPedidoVendaViewModel>(item);
            vm.PEVE_CD_ID = SessionMocks.idVolta;
            vm.ITPE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirItemPedidoVendaUsuario(ItemPedidoVendaViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ITEM_PEDIDO_VENDA item = Mapper.Map<ItemPedidoVendaViewModel, ITEM_PEDIDO_VENDA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreateItemVenda(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoPedidoVendaUsuario");
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
        public ActionResult EditarItemPedidoVenda(Int32 id)
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            if (SessionMocks.idFilial != 0)
            {
                ViewBag.QuantidadeEstoque = item.PRODUTO.PRODUTO_ESTOQUE_FILIAL.Where(x => x.FILI_CD_ID == SessionMocks.idFilial).FirstOrDefault().PREF_QN_ESTOQUE;
            }
            ViewBag.PrecoTotal = item.ITPE_QN_QUANTIDADE * item.PRODUTO.PROD_VL_PRECO_VENDA;
            objetoAntes = SessionMocks.pedidoVenda;
            ItemPedidoVendaViewModel vm = Mapper.Map<ITEM_PEDIDO_VENDA, ItemPedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarItemPedidoVenda(ItemPedidoVendaViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ITEM_PEDIDO_VENDA item = Mapper.Map<ItemPedidoVendaViewModel, ITEM_PEDIDO_VENDA>(vm);
                    Int32 volta = baseApp.ValidateEditItemVenda(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoPedidoVenda");
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
        public ActionResult ExcluirItemPedidoVenda(Int32 id)
        {
            
            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            objetoAntes = SessionMocks.pedidoVenda;
            item.ITPE_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditItemVenda(item);
            return RedirectToAction("VoltarAnexoPedidoVenda");
        }

        [HttpGet]
        public ActionResult ReativarItemPedidoVenda(Int32 id)
        {
            
            ITEM_PEDIDO_VENDA item = baseApp.GetItemVendaById(id);
            objetoAntes = SessionMocks.pedidoVenda;
            item.ITPE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditItemVenda(item);
            return RedirectToAction("VoltarAnexoPedidoVendaUsuario");
        }

        [HttpGet]
        public ActionResult IncluirItemPedidoVenda()
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");

            USUARIO usuario = SessionMocks.UserCredentials;
            ITEM_PEDIDO_VENDA item = new ITEM_PEDIDO_VENDA();
            ItemPedidoVendaViewModel vm = Mapper.Map<ITEM_PEDIDO_VENDA, ItemPedidoVendaViewModel>(item);
            vm.PEVE_CD_ID = SessionMocks.idVolta;
            vm.ITPE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirItemPedidoVenda(ItemPedidoVendaViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ITEM_PEDIDO_VENDA item = Mapper.Map<ItemPedidoVendaViewModel, ITEM_PEDIDO_VENDA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreateItemVenda(item);
                    // Verifica retorno
                    return RedirectToAction("IncluirItemPedidoVenda");
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
        public ActionResult ProcessarEnviarAprovacaoPedidoVenda(Int32 id)
        {
            
            PEDIDO_VENDA item = baseApp.GetItemById(id);
            PedidoVendaViewModel vm = Mapper.Map<PEDIDO_VENDA, PedidoVendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ProcessarEnviarAprovacaoPedidoVenda(PedidoVendaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_VENDA item = Mapper.Map<PedidoVendaViewModel, PEDIDO_VENDA>(vm);
                Int32 volta = baseApp.ValidateEnvioAprovacao(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0102", CultureInfo.CurrentCulture);
                    return View(vm);
                }
                if (volta == 2)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0103", CultureInfo.CurrentCulture);
                    return View(vm);
                }
                if (volta == 3)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0104", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_VENDA>();
                SessionMocks.listaVenda = null;
                return RedirectToAction("MontarTelaPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }


        [HttpGet]
        public ActionResult AcompanhamentoPedidoVenda()
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
            if (SessionMocks.listaVenda == null)
            {
                listaMaster = baseApp.GetAllItens();
                SessionMocks.listaVenda = listaMaster;
            }
            ViewBag.Listas = SessionMocks.listaVenda;
            ViewBag.Title = "Pedidos de Venda";

            // Indicadores
            ViewBag.Pedidos = SessionMocks.listaVenda.Count;
            ViewBag.Encerradas = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 5).ToList().Count;
            ViewBag.Canceladas = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 6).ToList().Count;
            ViewBag.Atrasadas = SessionMocks.listaVenda.Where(p => p.PEVE_DT_PREVISTA < DateTime.Today.Date).ToList().Count;
            ViewBag.EncerradasLista = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 5).ToList();
            ViewBag.CanceladasLista = SessionMocks.listaVenda.Where(p => p.PEVE_IN_STATUS == 6).ToList();
            ViewBag.AtrasadasLista = SessionMocks.listaVenda.Where(p => p.PEVE_DT_PREVISTA < DateTime.Today.Date).ToList();

            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Carrega listas
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");

            // Abre view
            objeto = new PEDIDO_VENDA();
            objeto.PEVE_DT_DATA = DateTime.Today.Date;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarAcompanhamentoPedidoVenda(PEDIDO_VENDA item)
        {
            
            try
            {
                // Executa a operação
                List<PEDIDO_VENDA> listaObj = new List<PEDIDO_VENDA>();
                Int32 volta = baseApp.ExecuteFilter(item.USUA_CD_ID, item.PEVE_NM_NOME, item.PEVE_NR_NUMERO, item.PEVE_DT_DATA, item.PEVE_IN_STATUS, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMaster = listaObj;
                SessionMocks.listaVenda = listaObj;
                return RedirectToAction("AcompanhamentoPedidoVenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("AcompanhamentoPedidoVenda");

            }
        }

        public ActionResult RetirarFiltroAcompanhamentoPedidoVenda()
        {
            
            SessionMocks.listaVenda = null;
            return RedirectToAction("AcompanhamentoPedidoVenda");
        }

    }
}