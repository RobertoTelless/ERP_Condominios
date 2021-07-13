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
using Newtonsoft.Json.Linq;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using System.Net.Mail;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class CompraController : Controller
    {
        private readonly IPedidoCompraAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IFornecedorAppService forApp;
        private readonly IProdutoAppService proApp;
        private readonly IMateriaPrimaAppService insApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly ITemplateAppService tempApp;
        private readonly IProdutotabelaPrecoAppService ptpApp;
        private String msg;
        private Exception exception;
        PEDIDO_COMPRA objeto = new PEDIDO_COMPRA();
        PEDIDO_COMPRA objetoAntes = new PEDIDO_COMPRA();
        List<PEDIDO_COMPRA> listaMaster = new List<PEDIDO_COMPRA>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public CompraController(IPedidoCompraAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, ICentroCustoAppService ccApps, IFornecedorAppService forApps, IProdutoAppService proApps, IMateriaPrimaAppService insApps, IContaBancariaAppService cbApps, ITemplateAppService tempApps, IProdutotabelaPrecoAppService ptpApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            ccApp = ccApps;
            forApp = forApps;
            proApp = proApps;
            insApp = insApps;
            cbApp = cbApps;
            tempApp = tempApps;
            ptpApp = ptpApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ValorTotal(Int32 qtde, Decimal valor)
        {
            return Json(qtde * valor);
        }

        public ActionResult Voltar()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult GetCustoProduto(Int32 id, Int32? fili)
        {
            var result = new Hashtable();
            if (fili != null)
            {
                var prod = proApp.GetById(id).PRODUTO_TABELA_PRECO.First(x => x.FILI_CD_ID == fili);
                result.Add("custo", prod.PRTP_VL_CUSTO == null ? 0 : prod.PRTP_VL_CUSTO);
                result.Add("markup", prod.PRTP_NR_MARKUP == null ? 0 : prod.PRTP_NR_MARKUP);
                result.Add("unidade", prod.PRODUTO.UNIDADE.UNID_NM_NOME);
            }
            else
            {
                var prod = proApp.GetById(id);
                result.Add("custo", prod.PROD_VL_CUSTO == null ? 0 : prod.PROD_VL_CUSTO);
                result.Add("markup", prod.PROD_VL_MARKUP_PADRAO == null ? 0 : prod.PROD_VL_MARKUP_PADRAO);
                result.Add("unidade", prod.UNIDADE == null ? "" : prod.UNIDADE.UNID_NM_NOME);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetCustoInsumo(Int32 id, Int32? fili)
        {
            var result = new Hashtable();
            if (fili != null)
            {
                var ins = insApp.GetById(id).MATERIA_PRIMA_PRECO.First(x => x.FILI_CD_ID == fili);
                result.Add("custo", ins.MPPR_VL_PRECO == null ? 0 : ins.MPPR_VL_PRECO);
                result.Add("unidade", ins.MATERIA_PRIMA.UNIDADE.UNID_NM_NOME);
            }
            else
            {
                var ins = insApp.GetById(id);
                result.Add("custo", ins.MPPR_VL_PRECO == null ? 0 : ins.MPPR_VL_PRECO);
                result.Add("unidade", ins.UNIDADE.UNID_NM_NOME);
            }

            return Json(result);
        }

        public FileResult DownloadPedidoCompra(Int32 id)
        {
            PEDIDO_COMPRA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.PECA_AQ_ARQUIVO;
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
        public ActionResult MontarTelaPedidoCompra()
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
            if (SessionMocks.idCompra != null)
            {
                SessionMocks.listaCompra = baseApp.GetAllItens().Where(x => x.PECO_CD_ID == SessionMocks.idCompra).ToList();
                SessionMocks.idCompra = 0;
            }
            if (SessionMocks.listaCompra == null || SessionMocks.listaCompra.Count == 0)
            {
                listaMaster = baseApp.GetAllItens();
                SessionMocks.listaCompra = listaMaster;
            }

            ViewBag.Listas = SessionMocks.listaCompra;
            ViewBag.Title = "Pedidos de Compraxxxx";

            // Indicadores
            ViewBag.Pedidos = baseApp.GetAllItens().Count;
            ViewBag.Encerradas = baseApp.GetAllItens().Count(p => p.PECO_IN_STATUS == 7);
            ViewBag.Canceladas = baseApp.GetAllItens().Count(p => p.PECO_IN_STATUS == 8);
            ViewBag.Atrasadas = baseApp.GetAllItens().Count(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8);
            ViewBag.EncerradasLista = baseApp.GetAllItens().Where(p => p.PECO_IN_STATUS == 7).ToList();
            ViewBag.CanceladasLista = baseApp.GetAllItens().Where(p => p.PECO_IN_STATUS == 8).ToList();
            ViewBag.AtrasadasLista = baseApp.GetAllItens().Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8).ToList();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            ViewBag.CotacaoEnvioSucesso = SessionMocks.smsEmailEnvio;

            SessionMocks.Fornecedores = forApp.GetAllItens();
            SessionMocks.Produtos = proApp.GetAllItens();
            SessionMocks.Insumos = insApp.GetAllItens();
            SessionMocks.smsEmailEnvio = 0;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            // Carrega listas
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Para Receber", Value = "5" });
            status.Add(new SelectListItem() { Text = "Em Recebimento", Value = "6" });
            status.Add(new SelectListItem() { Text = "Recebido", Value = "7" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "8" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");

            // Abre view
            Session["MensCompra"] = 0;
            objeto = SessionMocks.filtroCompra == null ? new PEDIDO_COMPRA() : SessionMocks.filtroCompra;
            objeto.PECO_DT_DATA = null;
            return View(objeto);
        }

        public ActionResult RetirarFiltroPedidoCompra()
        {
            
            SessionMocks.filtroCompra = null;
            SessionMocks.listaCompra = null;
            return RedirectToAction("MontarTelaPedidoCompra");
        }

        public ActionResult MostrarTudoPedidoCompra()
        {
            
            listaMaster = baseApp.GetAllItensAdm();
            SessionMocks.listaCompra = listaMaster;
            return RedirectToAction("MontarTelaPedidoCompra");
        }

        [HttpPost]
        public ActionResult FiltrarPedidoCompra(PEDIDO_COMPRA item)
        {
            
            try
            {
                SessionMocks.filtroCompra = item;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilter(item.USUA_CD_ID, item.PECO_NM_NOME, item.PECO_NR_NUMERO, item.PECO_NR_NOTA_FISCAL, item.PECO_DT_DATA, item.PECO_DT_PREVISTA, item.PECO_IN_STATUS, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }

                // Sucesso
                listaMaster = listaObj;
                SessionMocks.listaCompra = listaObj;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPedidoCompra");

            }
        }

        public ActionResult VoltarBaseMontarTelaPedidoCompra()
        {
            
            return RedirectToAction("MontarTelaPedidoCompra");
        }

        [HttpPost]
        public void MontaListaItemPedido(ITEM_PEDIDO_COMPRA item)
        {
            Task.Run(() => {
                if (SessionMocks.listaITPC == null)
                {
                    SessionMocks.listaITPC = new List<ITEM_PEDIDO_COMPRA>();
                }

                SessionMocks.listaITPC.Add(item);
            });
        }

        [HttpPost]
        public void RemoveItpcTabela(ITEM_PEDIDO_COMPRA item)
        {
            if (SessionMocks.listaITPC != null)
            {
                if (item.ITPC_IN_TIPO == 1)
                {
                    SessionMocks.listaITPC.RemoveAll(x => x.PROD_CD_ID == item.PROD_CD_ID);
                }
                else
                {
                    SessionMocks.listaITPC.RemoveAll(x => x.MAPR_CD_ID == item.MAPR_CD_ID);
                }
            }
        }

        [HttpGet]
        public ActionResult IncluirPedidoCompra()
        {
            SessionMocks.listaITPC = null;

            // Prepara listas
            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais.OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            List<PRODUTO> lista = SessionMocks.Produtos.OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos.OrderBy(x => x.MAPR_NM_NOME).ToList<MATERIA_PRIMA>(), "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            SessionMocks.voltaPop = 1;
            USUARIO usuario = SessionMocks.UserCredentials;
            PEDIDO_COMPRA item = new PEDIDO_COMPRA();
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.PECO_DT_DATA = DateTime.Today.Date;
            vm.PECO_IN_ATIVO = 1;
            vm.PECO_IN_STATUS = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.USUA_CD_ID = SessionMocks.UserCredentials.USUA_CD_ID;
            vm.PECO_DT_DATA = DateTime.Today.Date;
            vm.PECO_DT_PREVISTA = DateTime.Today.Date.AddDays(30);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirPedidoCompra(PedidoCompraViewModel vm)
        {
            

            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            List<PRODUTO> lista = SessionMocks.Produtos.Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos, "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            Hashtable result = new Hashtable();

            if (ModelState.IsValid)
            {
                if (SessionMocks.listaITPC == null)
                {
                    ModelState.AddModelError("", "Nenhum Item de Pedido cadastrado no pedido");
                    return View(vm);
                }

                try
                {
                    // Executa a operação
                    PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0062", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Acerta numero do pedido
                    item.PECO_NR_NUMERO = item.PECO_CD_ID.ToString();
                    volta = baseApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/PedidoCompra/" + item.PECO_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    
                    // Sucesso
                    listaMaster = new List<PEDIDO_COMPRA>();
                    SessionMocks.listaCompra = null;

                    foreach (var itpc in SessionMocks.listaITPC)
                    {
                        itpc.ITPC_NR_QUANTIDADE_REVISADA = itpc.ITPC_QN_QUANTIDADE;
                        itpc.ITPC_IN_ATIVO = 1;
                        itpc.PECO_CD_ID = item.PECO_CD_ID;

                        if (itpc.ITPC_IN_TIPO == 1)
                        {
                            PRODUTO prod = proApp.GetItemById((Int32)itpc.PROD_CD_ID);
                            itpc.UNID_CD_ID = prod.UNID_CD_ID;
                            itpc.ITPC_VL_PRECO_SELECIONADO = prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).Count() > 0 ? prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).First().PRTP_VL_CUSTO : null;
                        }
                        else
                        {
                            MATERIA_PRIMA ins = insApp.GetItemById((Int32)itpc.MAPR_CD_ID);
                            itpc.UNID_CD_ID = ins.UNID_CD_ID;
                            itpc.ITPC_VL_PRECO_SELECIONADO = ins.MATERIA_PRIMA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).Count() > 0 ? ins.MATERIA_PRIMA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).First().MPPR_VL_PRECO : null;
                        }

                        Int32 voltaItem = baseApp.ValidateCreateItemCompra(itpc);
                    }

                    SessionMocks.listaITPC = null;
                    SessionMocks.idVolta = item.PECO_CD_ID;
                    if (Session["FileQueueCompra"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCompra"];

                        foreach(var file in fq)
                        {
                            UploadFileQueuePedidoCompra(file);
                        }

                        Session["FileQueueCompra"] = null;
                    }

                    SessionMocks.idCompra = item.PECO_CD_ID;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }
                catch (Exception ex)
                {
                    SessionMocks.listaITPC = null;
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                SessionMocks.listaITPC = null;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarPedidoCompra(Int32 id)
        {
            
            // Prepara view
            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            List<PRODUTO> lista = SessionMocks.Produtos.Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos, "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 12)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
            }
            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 11)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
            }
            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 5)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", "Último item não pode ser desativado");
            }
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 2;

            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            objetoAntes = item;
            SessionMocks.pedidoCompra = item;
            SessionMocks.idVolta = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            vm.VALOR_TOTAL = custo;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarPedidoCompra(PedidoCompraViewModel vm)
        {
            
            ViewBag.CC = new SelectList(SessionMocks.CentroCustos, "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            List<PRODUTO> lista = SessionMocks.Produtos.Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos, "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<PEDIDO_COMPRA>();
                    SessionMocks.listaCompra = null;
                    SessionMocks.idCompra = item.PECO_CD_ID;
                    return RedirectToAction("MontarTelaPedidoCompra");
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
        public ActionResult ExcluirPedidoCompra(Int32 id)
        {
            
            // Prepara view
            Session["MensCompra"] = 0;
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirPedidoCompra(PedidoCompraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                Session["MensCompra"] = 0;
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarPedidoCompra(Int32 id)
        {
            
            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarPedidoCompra(PedidoCompraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPedidoCompra(Int32 id)
        {
            
            // Prepara view
            PEDIDO_COMPRA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoPedidoCompra()
        {
            
            if ((Int32)Session["VoltaCompra"] == 1)
            {
                return RedirectToAction("VerPedidoCompra", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaCompra"] == 3)
            {
                return RedirectToAction("CancelarPedidoCompra", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaCompra"] == 4)
            {
                return RedirectToAction("EnviarCotacaoPedidoCompra", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaCompra"] == 5)
            {
                return RedirectToAction("ReceberPedidoCompra", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaCompra"] == 6)
            {
                return RedirectToAction("AprovarPedidoCompra", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaCompra"] == 7)
            {
                return RedirectToAction("ProcessarReceberPedidoCompra", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaCompra"] == 8)
            {
                return RedirectToAction("ProcessarEnviarAprovacaoPedidoCompra", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaCompra"] == 9)
            {
                return RedirectToAction("ProcessarCotacaoPedidoCompra", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("EditarPedidoCompra", new { id = SessionMocks.idVolta });
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

            Session["FileQueueCompra"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueuePedidoCompra(FileQueue file)
        {
            
            if (file == null)
            {
                Session["MensCompra"] = 12;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }

            PEDIDO_COMPRA item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                Session["MensCompra"] = 11;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/PedidoCompra/" + item.PECO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = file.ContentType;
            String a = extensao;

            // Gravar registro
            PEDIDO_COMPRA_ANEXO foto = new PEDIDO_COMPRA_ANEXO();
            foto.PECA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PECA_DT_ANEXO = DateTime.Today;
            foto.PECA_IN_ATIVO = 1;
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
            foto.PECA_IN_TIPO = tipo;
            foto.PECA_NM_TITULO = fileName;
            foto.PECO_CD_ID = item.PECO_CD_ID;

            item.PEDIDO_COMPRA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoPedidoCompra");
        }

        [HttpPost]
        public ActionResult UploadFilePedidoCompra(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                Session["MensCompra"] = 12;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }

            PEDIDO_COMPRA item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                Session["MensCompra"] = 11;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/PedidoCompra/" + item.PECO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PEDIDO_COMPRA_ANEXO foto = new PEDIDO_COMPRA_ANEXO();
            foto.PECA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PECA_DT_ANEXO = DateTime.Today;
            foto.PECA_IN_ATIVO = 1;
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
            foto.PECA_IN_TIPO = tipo;
            foto.PECA_NM_TITULO = fileName;
            foto.PECO_CD_ID = item.PECO_CD_ID;

            item.PEDIDO_COMPRA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoPedidoCompra");
        }

        public void ProcessarFornecedor(Int32 id)
        {
            SessionMocks.fornCotacao = id;
        }

        [HttpGet]
        public ActionResult ProcessarCotacaoPedidoCompra(Int32 id)
        {
            
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            List<FORNECEDOR> forn = new List<FORNECEDOR>();
            foreach (var itpc in item.ITEM_PEDIDO_COMPRA)
            {
                if (itpc.PRODUTO != null)
                {
                    forn.AddRange(itpc.PRODUTO.PRODUTO_FORNECEDOR.Where(x => x.PRFO_IN_ATIVO == 1).Select(x => x.FORNECEDOR));
                }

                if (itpc.MATERIA_PRIMA != null)
                {
                    forn.AddRange(itpc.MATERIA_PRIMA.MATERIA_PRIMA_FORNECEDOR.Where(x => x.MAFO_IN_ATIVO == 1).Select(x => x.FORNECEDOR));
                }
            }
            Session["CotForn"] = forn.Distinct().ToList<FORNECEDOR>();
            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["CotForn"];
            objeto = item;
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            SessionMocks.idVolta = id;
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 9;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ProcessarCotacaoPedidoCompra(PedidoCompraViewModel vm)
        {
            
            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["CotForn"];
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);

                vm.FORN_CD_ID = SessionMocks.fornCotacao;

                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateCotacao(item, usuarioLogado);

                if (volta == 0)
                {
                    foreach (var itpc in item.ITEM_PEDIDO_COMPRA)
                    {
                        try
                        {
                            ITEM_PEDIDO_COMPRA itpcCotado = new ITEM_PEDIDO_COMPRA();
                            itpcCotado.ITPC_CD_ID = itpc.ITPC_CD_ID;
                            itpcCotado.PECO_CD_ID = itpc.PECO_CD_ID;
                            itpcCotado.PROD_CD_ID = itpc.PROD_CD_ID;
                            itpcCotado.MAPR_CD_ID = itpc.MAPR_CD_ID;
                            itpcCotado.UNID_CD_ID = itpc.UNID_CD_ID;
                            itpcCotado.ITPC_QN_QUANTIDADE = itpc.ITPC_QN_QUANTIDADE;
                            itpcCotado.ITPC_VL_PRECO_SELECIONADO = itpc.ITPC_VL_PRECO_SELECIONADO;
                            itpcCotado.ITPC_NR_QUANTIDADE_REVISADA = itpc.ITPC_NR_QUANTIDADE_REVISADA;
                            itpcCotado.ITPC_TX_OBSERVACOES = itpc.ITPC_TX_OBSERVACOES;
                            itpcCotado.ITPC_IN_ATIVO = itpc.ITPC_IN_ATIVO;
                            itpcCotado.ITPC_DT_COTACAO = DateTime.Now;
                            itpcCotado.ITPC_IN_TIPO = itpc.ITPC_IN_TIPO;
                            itpcCotado.ITPC_DS_JUSTIFICATIVA = itpc.ITPC_DS_JUSTIFICATIVA;
                            itpcCotado.ITPC_NR_QUANTIDADE_RECEBIDA = itpc.ITPC_NR_QUANTIDADE_RECEBIDA;

                            Int32 voltaITPC = baseApp.ValidateEditItemCompra(itpcCotado);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return View(objeto);
                        }
                    }
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult AprovarPedidoCompra(Int32 id)
        {
            
            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            SessionMocks.idVolta = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult AprovarPedidoCompra(PedidoCompraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateAprovacao(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0103", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0104", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0105", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ProcessarReceberPedidoCompra(Int32 id)
        {
            
            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            Session["VoltaCompra"] = 7;
            SessionMocks.idVolta = id;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ProcessarReceberPedidoCompra(PedidoCompraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateReceber(item);

                // Verifica retorno
                CONTA_PAGAR cp = new CONTA_PAGAR();
                if (volta == 0)
                {
                    // Recupera fornecedor
                    FORNECEDOR forn = forApp.GetItemById((Int32)item.FORN_CD_ID);

                    // Calcula valor
                    Decimal valor = 0;

                    foreach (var i in item.ITEM_PEDIDO_COMPRA)
                    {
                        if (i.ITPC_VL_PRECO_SELECIONADO != null)
                        {
                            valor += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (decimal)i.ITPC_VL_PRECO_SELECIONADO;
                        }
                    }

                    // Gera CP
                    cp.ASSI_CD_ID = SessionMocks.IdAssinante;
                    cp.CAPA_DS_DESCRICAO = "Lançamento a pagar referente ao pedido de compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO;
                    cp.CAPA_DT_COMPETENCIA = DateTime.Today.Date;
                    cp.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
                    cp.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
                    cp.CAPA_IN_ATIVO = 1;
                    cp.CAPA_IN_LIQUIDADA = 0;
                    cp.CAPA_IN_PAGA_PARCIAL = 0;
                    cp.CAPA_IN_PARCELADA = 0;
                    cp.CAPA_IN_PARCELAS = 0;
                    cp.CAPA_IN_TIPO_LANCAMENTO = 1;
                    cp.CAPA_NR_DOCUMENTO = item.PECO_NR_NUMERO;
                    cp.CAPA_VL_DESCONTO = 0;
                    cp.CAPA_VL_JUROS = 0;
                    cp.CAPA_VL_PARCELADO = 0;
                    cp.CAPA_VL_PARCIAL = 0;
                    cp.CAPA_VL_TAXAS = 0;
                    cp.CAPA_VL_VALOR_PAGO = 0;
                    cp.CAPA_VL_VALOR = valor;
                    cp.CAPA_VL_SALDO = valor;
                    cp.CECU_CD_ID = item.CECU_CD_ID;
                    cp.FOPA_CD_ID = 1;
                    cp.FORN_CD_ID = (Int32)item.FORN_CD_ID;
                    cp.PECO_CD_ID = item.PECO_CD_ID;
                    cp.USUA_CD_ID = item.USUA_CD_ID;
                    cp.COBA_CD_ID = cbApp.GetContaPadrao().COBA_CD_ID;
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                SessionMocks.contaPagar = cp;
                SessionMocks.voltaCompra = 1;
                return RedirectToAction("IncluirCP", "ContaPagar", new { voltaCompra = 1 });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReceberPedidoCompra(Int32 id)
        {
            
            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            Session["VoltaCompra"] = 5;
            SessionMocks.idVolta = id;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReceberPedidoCompra(PedidoCompraViewModel vm)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                    Decimal custo = 0;
                    foreach (var i in item.ITEM_PEDIDO_COMPRA)
                    {
                        if (i.ITPC_VL_PRECO_SELECIONADO != null)
                        {
                            custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                        }
                    }
                    ViewBag.CustoTotal = custo;
                    Int32 volta = baseApp.ValidateRecebido(item);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<PEDIDO_COMPRA>();
                    SessionMocks.listaCompra = null;
                    return RedirectToAction("MontarTelaPedidoCompra");
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
        public ActionResult ReceberItemPedidoCompra(Int32 id)
        {
            
            // Prepara view
            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReceberItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            
            try
            {
                if (vm.ITPC_NR_QUANTIDADE_REVISADA != vm.ITPC_NR_QUANTIDADE_RECEBIDA && vm.ITPC_DS_JUSTIFICATIVA == null)
                {
                    ModelState.AddModelError("", "Para quantidade recebida diferente do previsto, necessário justificativa");
                    return View(vm);
                }

                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateItemRecebido(item);

                // Verifica retorno
                if (volta == 2)
                {
                    SessionMocks.pedidoRecebido = vm.PECO_CD_ID;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                return RedirectToAction("ReceberPedidoCompra", new { id = SessionMocks.idVolta });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult CancelarPedidoCompra(Int32 id)
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            Session["CustoTotal"] = custo;
            if ((Int32)Session["MensCompra"] == 1)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0106", CultureInfo.CurrentCulture));
            }
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 3;
            SessionMocks.idVolta = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            vm.VALOR_TOTAL = custo;
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelarPedidoCompra(PedidoCompraViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            vm.VALOR_TOTAL = (decimal)Session["CustoTotal"];
            Session["CustoTotal"] = 0;
            try
            {
                if (vm.FORN_CD_ID == 0)
                {
                    ModelState.AddModelError("", "Fornecedor não selecionado");
                    return View(vm);
                }

                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateCancelamento(item);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0106", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        public ActionResult VerPedidoCompra(Int32 id)
        {
            
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            SessionMocks.idVolta = id;
            Session["VoltaCompra"] = 1;
            return View(vm);
        }

        public ActionResult VerItemPedidoCompra(Int32 id)
        {
            
            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        public ActionResult VerAtrasados()
        {
            
            // Prepara view
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");

            if (SessionMocks.listaCompraAtrasados == null || SessionMocks.listaCompraAtrasados.Count == 0)
            {
                SessionMocks.listaCompraAtrasados = baseApp.GetAllItens().Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8).ToList();
            }

            // Abre view
            ViewBag.Pedidos = SessionMocks.listaCompraAtrasados.Count;
            ViewBag.Atrasadas = SessionMocks.listaCompraAtrasados.Count(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8);
            ViewBag.AtrasadasLista = SessionMocks.listaCompraAtrasados.Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8).ToList();
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensCompra"] = null;
            }

            objeto = new PEDIDO_COMPRA();
            SessionMocks.voltaCompra = 1;
            SessionMocks.voltaConsulta = 3;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarAtrasados(PEDIDO_COMPRA item)
        {
            
            try
            {
                SessionMocks.filtroCompraAtrasados = item;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilterDash(item.PECO_NR_NUMERO, item.PECO_DT_FINAL, item.PECO_NM_NOME, item.USUA_CD_ID, null, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    return RedirectToAction("VerAtrasados");
                }

                // Sucesso
                SessionMocks.listaCompraAtrasados = listaObj;
                return RedirectToAction("VerAtrasados");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerAtrasados");

            }
        }

        public ActionResult RetirarFiltroAtrasados()
        {
            
            SessionMocks.filtroCompraAtrasados = null;
            SessionMocks.listaCompraAtrasados = null;
            return RedirectToAction("VerAtrasados");
        }

        public ActionResult VerEncerrados()
        {
            
            // Prepara view
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");

            if (SessionMocks.listaCompraEncerrados == null || SessionMocks.listaCompraEncerrados.Count == 0)
            {
                SessionMocks.listaCompraEncerrados = baseApp.GetAllItens().Where(p => p.PECO_IN_STATUS == 7).ToList();
            }

            // Abre view
            ViewBag.Pedidos = SessionMocks.listaCompraEncerrados.Count;
            ViewBag.EncerradasLista = SessionMocks.listaCompraEncerrados;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensCompra"] = null;
            }

            objeto = new PEDIDO_COMPRA();
            SessionMocks.voltaCompra = 1;
            SessionMocks.voltaConsulta = 3;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarEncerrados(PEDIDO_COMPRA item)
        {
            
            try
            {
                SessionMocks.filtroCompraEncerrados = item;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilterDash(item.PECO_NR_NUMERO, item.PECO_DT_FINAL, item.PECO_NM_NOME, item.USUA_CD_ID, 7, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    return RedirectToAction("VerEncerrados");
                }

                // Sucesso
                SessionMocks.listaCompraEncerrados = listaObj;
                return RedirectToAction("VerEncerrados");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerEncerrados");

            }
        }

        public ActionResult RetirarFiltroEncerrados()
        {
            
            SessionMocks.filtroCompraEncerrados = null;
            SessionMocks.listaCompraEncerrados = null;
            return RedirectToAction("VerEncerrados");
        }

        public ActionResult VerCancelados()
        {
            
            // Prepara view
            ViewBag.Usuarios = new SelectList(SessionMocks.Usuarios, "USUA_CD_ID", "USUA_NM_NOME");

            if (SessionMocks.listaCompraCancelados == null || SessionMocks.listaCompraCancelados.Count == 0)
            {
                SessionMocks.listaCompraCancelados = baseApp.GetAllItens().Where(p => p.PECO_IN_STATUS == 8).ToList();
            }

            // Abre view
            ViewBag.Pedidos = SessionMocks.listaCompraCancelados.Count;
            ViewBag.CanceladasLista = SessionMocks.listaCompraCancelados;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensCompra"] = null;
            }

            objeto = new PEDIDO_COMPRA();
            SessionMocks.voltaCompra = 1;
            SessionMocks.voltaConsulta = 3;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarCancelados(PEDIDO_COMPRA item)
        {
            
            try
            {
                SessionMocks.filtroCompraCancelados = item;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilterDash(item.PECO_NR_NUMERO, item.PECO_DT_FINAL, item.PECO_NM_NOME, item.USUA_CD_ID, 8, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    return RedirectToAction("VerCancelados");
                }

                // Sucesso
                SessionMocks.listaCompraCancelados = listaObj;
                return RedirectToAction("VerCancelados");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerCancelados");

            }
        }

        public ActionResult RetirarFiltroCancelados()
        {
            
            SessionMocks.filtroCompraCancelados = null;
            SessionMocks.listaCompraCancelados = null;
            return RedirectToAction("VerCancelados");
        }

        [HttpPost]
        public JsonResult EditarItemInline(ITEM_PEDIDO_COMPRA item)
        {
            Hashtable result = new Hashtable();

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                if (item.ITPC_DT_COTACAO == null)
                {
                    item.ITPC_DT_COTACAO = DateTime.Now;
                }
                Int32 volta = baseApp.ValidateEditItemCompra(item);

                // Verifica retorno
                if (volta == 0)
                {
                    result.Add("success", "Item editado com sucesso!");
                    if (item.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        result.Add("vlrTotal", item.ITPC_NR_QUANTIDADE_REVISADA * item.ITPC_VL_PRECO_SELECIONADO);
                    }
                }
                else
                {
                    result.Add("error", "Falha ao editar item");
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                result.Add("error", ex.Message);
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult EditarItemPedidoCompra(Int32 id)
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(SessionMocks.Produtos, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos, "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            objetoAntes = SessionMocks.pedidoCompra;
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(SessionMocks.Produtos, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos, "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                    Int32 volta = baseApp.ValidateEditItemCompra(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoPedidoCompra");
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

        [HttpPost]
        public void EditarItemPedidoCompraInline(Int32? id, Int32? qtde, decimal? preco)
        {
            if (id != null)
            {
                ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById((Int32)id);
                item.ITPC_NR_QUANTIDADE_REVISADA = qtde;
                item.ITPC_VL_PRECO_SELECIONADO = preco;
                Int32 volta = baseApp.ValidateEditItemCompra(item);
            }
        }

        [HttpGet]
        public ActionResult ExcluirItemPedidoCompra(Int32 id)
        {
            
            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateDeleteItemCompra(item);

                SessionMocks.idVolta = item.PECO_CD_ID;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarItemPedidoCompra(Int32 id)
        {
            
            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                PEDIDO_COMPRA ped = baseApp.GetItemById(item.PECO_CD_ID);
                if (item.ITPC_IN_TIPO == 1)
                {

                    if (ped.ITEM_PEDIDO_COMPRA.Where(x => x.PROD_CD_ID == item.PROD_CD_ID && x.ITPC_IN_ATIVO == 1).ToList().Count > 0)
                    {
                        ModelState.AddModelError("", "PRODUTO já existente no pedido");
                        return View(vm);
                    }
                }
                else if (item.ITPC_IN_TIPO == 2)
                {
                    if (ped.ITEM_PEDIDO_COMPRA.Where(x => x.MAPR_CD_ID == item.MAPR_CD_ID && x.ITPC_IN_ATIVO == 1).ToList().Count > 0)
                    {
                        ModelState.AddModelError("", "INSUMO já existente no pedido");
                        return View(vm);
                    }
                }

                Int32 volta = baseApp.ValidateReativarItemCompra(item);

                SessionMocks.idVolta = item.PECO_CD_ID;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirItemPedidoCompra()
        {
            
            // Prepara view
            List<PRODUTO> lista = SessionMocks.Produtos.Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos, "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            USUARIO usuario = SessionMocks.UserCredentials;
            ITEM_PEDIDO_COMPRA item = new ITEM_PEDIDO_COMPRA();
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            vm.PECO_CD_ID = SessionMocks.idVolta;
            vm.ITPC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(SessionMocks.Fornecedores, "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(SessionMocks.Produtos, "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Insumos = new SelectList(SessionMocks.Insumos, "MAPR_CD_ID", "MAPR_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    vm.ITPC_NR_QUANTIDADE_REVISADA = vm.ITPC_QN_QUANTIDADE;
                    if (vm.ITPC_IN_TIPO == 1)
                    {
                        vm.ITPC_VL_PRECO_SELECIONADO = ptpApp.GetByProdFilial((Int32)vm.PROD_CD_ID, (Int32)(baseApp.GetItemById(vm.PECO_CD_ID).FILI_CD_ID == null ? SessionMocks.idFilial : baseApp.GetItemById(vm.PECO_CD_ID).FILI_CD_ID)).PRTP_VL_PRECO;
                    }
                    else
                    {
                        vm.ITPC_VL_PRECO_SELECIONADO = insApp.GetItemById((Int32)vm.MAPR_CD_ID).MAPR_VL_CUSTO;
                    }

                    if (vm.ITPC_IN_TIPO == 1)
                    {
                        var prod = proApp.GetItemById((Int32)vm.PROD_CD_ID);
                        vm.UNID_CD_ID = prod.UNID_CD_ID;
                    }
                    else
                    {
                        var ins = insApp.GetItemById((Int32)vm.MAPR_CD_ID);
                        vm.UNID_CD_ID = ins.UNID_CD_ID;
                    }
                    // Executa a operação
                    ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreateItemCompra(item);

                    if (SessionMocks.idVolta == 0)
                    {
                        return RedirectToAction("IncluirItemPedidoCompra");
                    }

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoPedidoCompra");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    ModelState.AddModelError("", ex.Message);
                    return RedirectToAction("VoltarAnexoPedidoCompra");
                }
            }
            else
            {
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
        }

        //[HttpPost]
        //public Int32 IncluirItemPedidoCompra_Inclusao(List<ITEM_PEDIDO_COMPRA>, Int32 id)
        //{
        //    try
        //    {
        //        var jArray = JArray.Parse(tabelaItemPedido);

        //        foreach (var jObject in jArray)
        //        {

        //            ITEM_PEDIDO_COMPRA item = new ITEM_PEDIDO_COMPRA();
        //            item.PECO_CD_ID = id;
        //            item.ITPC_IN_ATIVO = 1;
        //            item.ITPC_IN_TIPO = (Int32)jObject["ITPC_IN_TIPO"];

        //            if (item.ITPC_IN_TIPO == 1)
        //            {
        //                var prod = proApp.GetItemById((Int32)jObject["PROD_CD_ID"]);
        //                item.PROD_CD_ID = (Int32)jObject["PROD_CD_ID"];
        //                item.UNID_CD_ID = prod.UNID_CD_ID;
        //            }
        //            else
        //            {
        //                var ins = insApp.GetItemById((Int32)jObject["MAPR_CD_ID"]);
        //                item.MAPR_CD_ID = (Int32)jObject["MAPR_CD_ID"];
        //                item.UNID_CD_ID = ins.UNID_CD_ID;
        //            }
        //            item.ITPC_QN_QUANTIDADE = (Int32)jObject["ITPC_QN_QUANTIDADE"];
        //            item.ITPC_TX_OBSERVACOES = jObject["ITPC_TX_OBSERVACOES"] == null ? "-" : (String)jObject["ITPC_TX_OBSERVACOES"];

        //            Int32 volta = baseApp.ValidateCreateItemCompra(item);
        //        }

        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 2;
        //    }
        //}

        [HttpPost]
        public JsonResult GetTemplate()
        {
            var result = new Hashtable();
            result.Add("TEMP_TX_CORPO", tempApp.GetByCode("COTFORN").TEMP_TX_CORPO);
            return Json(result);
        }

        [HttpPost]
        public void EnviarCotacaoFornecedor(Int32 forn, Int32 pedido)
        {
            var prod = baseApp.GetItemById(pedido).ITEM_PEDIDO_COMPRA.Where(x => x.PROD_CD_ID != null).SelectMany(x => x.PRODUTO.PRODUTO_FORNECEDOR).Where(x => x.PRFO_IN_ATIVO == 1).Select(x => new { FORN_CD_ID = x.FORN_CD_ID, PROD_CD_ID = x.PROD_CD_ID });
            prod = prod.Where(x => x.FORN_CD_ID == forn);
            var mat = baseApp.GetItemById(pedido).ITEM_PEDIDO_COMPRA.Where(x => x.MAPR_CD_ID != null).SelectMany(x => x.MATERIA_PRIMA.MATERIA_PRIMA_FORNECEDOR).Where(x => x.FORN_CD_ID != null && x.MAPR_CD_ID != null && x.MAFO_IN_ATIVO == 1).Select(x => new { FORN_CD_ID = x.FORN_CD_ID, MAPR_CD_ID = x.MAPR_CD_ID });
            mat = mat.Where(x => x.FORN_CD_ID == forn);

            if (prod != null && prod.Count() != 0)
            {
                foreach (var p in prod)
                {
                    if (SessionMocks.enviarCotacaoFornProd == null)
                    {
                        SessionMocks.enviarCotacaoFornProd = new List<PRODUTO_FORNECEDOR>();
                    }
                    SessionMocks.enviarCotacaoFornProd.Add(proApp.GetByProdForn(p.FORN_CD_ID, p.PROD_CD_ID));
                }
            }

            if (mat != null && mat.Count() != 0)
            {
                foreach (var m in mat)
                {
                    if (SessionMocks.enviarCotacaoFornMat == null)
                    {
                        SessionMocks.enviarCotacaoFornMat = new List<MATERIA_PRIMA_FORNECEDOR>();
                    }
                    SessionMocks.enviarCotacaoFornMat.Add(insApp.GetByMatForn(m.FORN_CD_ID.Value, m.MAPR_CD_ID.Value));
                }
            }
        }

        [HttpPost]
        public void RemoverCotacaoFornecedor(Int32 forn)
        {
            try
            {
                SessionMocks.enviarCotacaoFornProd.RemoveAll(x => x.FORN_CD_ID == forn);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
        }

        [HttpGet]
        public ActionResult EnviarCotacaoPedidoCompra(Int32 id)
        {
            
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            List<FORNECEDOR> forn = new List<FORNECEDOR>();
            foreach (var itpc in item.ITEM_PEDIDO_COMPRA)
            {                
                if (itpc.PRODUTO != null)
                {
                    forn.AddRange(itpc.PRODUTO.PRODUTO_FORNECEDOR.Where(x => x.PRFO_IN_ATIVO == 1).Select(x => x.FORNECEDOR));
                }

                if (itpc.MATERIA_PRIMA != null)
                {
                    forn.AddRange(itpc.MATERIA_PRIMA.MATERIA_PRIMA_FORNECEDOR.Where(x => x.MAFO_IN_ATIVO == 1).Select(x => x.FORNECEDOR));
                }
            }
            Session["ITPCForn"] = forn.Distinct().ToList<FORNECEDOR>();
            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["ITPCForn"];
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 4;
            SessionMocks.idVolta = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EnviarCotacaoPedidoCompra(PedidoCompraViewModel vm, Int32? enviaSms, String emailPers)
        {
            
            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["ITPCForn"];
            try
            {
                List<AttachmentForn> attachmentForn = new List<AttachmentForn>();
                String voltaSms = String.Empty;

                // Executa a operação
                PEDIDO_COMPRA ped = baseApp.GetItemById(vm.PECO_CD_ID);
                USUARIO usuarioLogado = SessionMocks.UserCredentials;

                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);

                if (voltaSms == "1")
                {
                    Session["MensMensagem"] = 1;
                    ModelState.AddModelError("", "Fornecedor Inexistente");
                    return View(vm);
                }

                if (voltaSms == "2")
                {
                    Session["MensMensagem"] = 1;
                    ModelState.AddModelError("", "Fornecedor não possuí celular cadastrado");
                    return View(vm);
                }

                if (voltaSms == "3")
                {
                    Session["MensMensagem"] = 1;
                    ModelState.AddModelError("", "SMS não enviado");
                    return View(vm);
                }

                if ((SessionMocks.enviarCotacaoFornProd != null && SessionMocks.enviarCotacaoFornProd.Count != 0) || (SessionMocks.enviarCotacaoFornMat != null && SessionMocks.enviarCotacaoFornMat.Count != 0))
                {
                    List<Int32> fornecedores = new List<Int32>();

                    if (SessionMocks.enviarCotacaoFornProd != null && SessionMocks.enviarCotacaoFornProd.Count != 0)
                    {
                        fornecedores.AddRange(SessionMocks.enviarCotacaoFornProd.Select(x => x.FORN_CD_ID).ToList());
                    }

                    if (SessionMocks.enviarCotacaoFornMat != null && SessionMocks.enviarCotacaoFornMat.Count != 0)
                    {
                        fornecedores.AddRange(SessionMocks.enviarCotacaoFornMat.Select(x => (Int32)x.FORN_CD_ID).ToList());
                    }

                    SessionMocks.enviarCotacaoFornProd = null;
                    SessionMocks.enviarCotacaoFornMat = null;

                    SessionMocks.listaEmailForn = fornecedores.Distinct().ToList();
                }
                else
                {
                    List<Int32> fornecedores = ped.ITEM_PEDIDO_COMPRA.Where(x => x.PRODUTO != null).SelectMany(x => x.PRODUTO.PRODUTO_FORNECEDOR.Where(y => y.PRFO_IN_ATIVO == 1)).Select(x => x.FORN_CD_ID).ToList();
                    fornecedores.AddRange(ped.ITEM_PEDIDO_COMPRA.Where(x => x.MATERIA_PRIMA != null).SelectMany(x => x.MATERIA_PRIMA.MATERIA_PRIMA_FORNECEDOR.Where(y => y.MAFO_IN_ATIVO == 1)).Where(x => x.FORN_CD_ID != null).Select(x => (Int32)x.FORN_CD_ID).ToList());
                    SessionMocks.listaEmailForn = fornecedores.Distinct().ToList();
                }

                if (enviaSms == 1)
                {
                    foreach (var f in SessionMocks.listaEmailForn)
                    {
                        var fornSms = forApp.GetItemById(f);
                        voltaSms = EnviaSmsCotacao(fornSms);
                    }                    
                }

                if ((SessionMocks.enviarCotacaoFornProd != null && SessionMocks.enviarCotacaoFornProd.Count != 0) || (SessionMocks.enviarCotacaoFornMat != null && SessionMocks.enviarCotacaoFornMat.Count != 0))
                {
                    if (SessionMocks.enviarCotacaoFornProd != null && SessionMocks.enviarCotacaoFornProd.Count != 0)
                    {
                        var prodFornecedores = SessionMocks.enviarCotacaoFornProd.GroupBy(x => x.FORN_CD_ID);

                        attachmentForn.AddRange(GeraAnexoProd(vm, ped, prodFornecedores));
                    }

                    if (SessionMocks.enviarCotacaoFornMat != null && SessionMocks.enviarCotacaoFornMat.Count != 0)
                    {
                        var matFornecedores = SessionMocks.enviarCotacaoFornMat.GroupBy(x => (Int32)x.FORN_CD_ID);

                        attachmentForn.AddRange(GeraAnexoMat(vm, ped, matFornecedores));
                    }

                    SessionMocks.enviarCotacaoFornProd = null;
                    SessionMocks.enviarCotacaoFornMat = null;
                }
                else
                {
                    var prodFornecedores = ped.ITEM_PEDIDO_COMPRA.Where(x => x.PRODUTO != null).SelectMany(x => x.PRODUTO.PRODUTO_FORNECEDOR.Where(y => y.PRFO_IN_ATIVO == 1)).GroupBy(x => x.FORN_CD_ID);
                    var matFornecedores = ped.ITEM_PEDIDO_COMPRA.Where(x => x.MATERIA_PRIMA != null).SelectMany(x => x.MATERIA_PRIMA.MATERIA_PRIMA_FORNECEDOR.Where(y => y.MAFO_IN_ATIVO == 1)).Where(x => x.FORN_CD_ID != null).GroupBy(x => (Int32)x.FORN_CD_ID);

                    attachmentForn.AddRange(GeraAnexoProd(vm, ped, prodFornecedores));
                    attachmentForn.AddRange(GeraAnexoMat(vm, ped, matFornecedores));
                }

                Int32 volta = baseApp.ValidateEnvioCotacao(item, attachmentForn, emailPers, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0102", CultureInfo.CurrentCulture));
                    return View(item);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                if (enviaSms != 1)
                {
                    SessionMocks.smsEmailEnvio = 2;
                }
                else
                {
                    SessionMocks.smsEmailEnvio = 1;
                }
                SessionMocks.idCompra = item.PECO_CD_ID;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public List<AttachmentForn> GeraAnexoProd(PedidoCompraViewModel vm, PEDIDO_COMPRA ped, IEnumerable<IGrouping<Int32, PRODUTO_FORNECEDOR>> prodFornecedores)
        {
            List<AttachmentForn> list = new List<AttachmentForn>();

            foreach (var group in prodFornecedores)
            {
                Int32 groupKey = group.Key;

                FORNECEDOR forn = forApp.GetItemById(groupKey);

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = "Itens - Pedido de Compra " + vm.PECO_CD_ID;
                List<ITEM_PEDIDO_COMPRA> lista = new List<ITEM_PEDIDO_COMPRA>();

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                MemoryStream ms = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

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

                cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
                table = new PdfPTable(new float[] { 100f, 100f, 150f, 80f, 40f, 100f, 100f, 40f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Itens de Pedido - " + forn.FORN_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 9;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Fabricante", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (var groupedItem in group)
                {
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ped.ITEM_PEDIDO_COMPRA.FirstOrDefault(x => x.PROD_CD_ID == groupedItem.PROD_CD_ID).ITPC_TX_OBSERVACOES, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_MODELO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_FABRICANTE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ped.ITEM_PEDIDO_COMPRA.FirstOrDefault(x => x.PROD_CD_ID == groupedItem.PROD_CD_ID).ITPC_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza Pdf
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                ms.Position = 0;

                Attachment anexo = new Attachment(ms, "ItensPedido" + ped.PECO_CD_ID + ".pdf");

                AttachmentForn af = new AttachmentForn();
                af.FORN_CD_ID = groupKey;
                af.ATTACHMENT = anexo;

                list.Add(af);
            }

            return list;
        }

        public List<AttachmentForn> GeraAnexoMat(PedidoCompraViewModel vm, PEDIDO_COMPRA ped, IEnumerable<IGrouping<Int32, MATERIA_PRIMA_FORNECEDOR>> matFornecedores)
        {
            List<AttachmentForn> list = new List<AttachmentForn>();

            foreach (var group in matFornecedores)
            {
                Int32 groupKey = group.Key;

                FORNECEDOR forn = forApp.GetItemById(groupKey);

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = "Itens - Pedido de Compra " + vm.PECO_CD_ID;
                List<ITEM_PEDIDO_COMPRA> lista = new List<ITEM_PEDIDO_COMPRA>();

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                MemoryStream ms = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

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

                cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
                table = new PdfPTable(new float[] { 100f, 100f, 150f, 80f, 40f, 100f, 100f, 40f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Itens de Pedido - " + forn.FORN_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 9;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Fabricante", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (var groupedItem in group)
                {
                    cell = new PdfPCell(new Paragraph(groupedItem.MATERIA_PRIMA.MAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ped.ITEM_PEDIDO_COMPRA.FirstOrDefault(x => x.MAPR_CD_ID == groupedItem.MAPR_CD_ID).ITPC_TX_OBSERVACOES, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ped.ITEM_PEDIDO_COMPRA.FirstOrDefault(x => x.MAPR_CD_ID == groupedItem.MAPR_CD_ID).ITPC_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza Pdf
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                ms.Position = 0;

                Attachment anexo = new Attachment(ms, "ItensPedido" + ped.PECO_CD_ID + ".pdf");

                AttachmentForn af = new AttachmentForn();
                af.FORN_CD_ID = groupKey;
                af.ATTACHMENT = anexo;

                list.Add(af);
            }

            return list;
        }

        [HttpPost]
        public String EnviaSmsCotacao(FORNECEDOR item)
        {
            Int32 idAss = SessionMocks.IdAssinante;
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            String volta = baseApp.ValidateCreateMensagem(item, usuarioLogado, idAss);

            return volta;
        }

        [HttpGet]
        public ActionResult ProcessarEnviarAprovacaoPedidoCompra(Int32 id)
        {
            
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            SessionMocks.idVolta = id;
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 8;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ProcessarEnviarAprovacaoPedidoCompra(PedidoCompraViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateEnvioAprovacao(item);

                // Sucesso
                Session["MensCompra"] = 0;
                listaMaster = new List<PEDIDO_COMPRA>();
                SessionMocks.listaCompra = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        public ActionResult IncluirAcompanhamento(Int32 id)
        {
            
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            PEDIDO_COMPRA_ACOMPANHAMENTO coment = new PEDIDO_COMPRA_ACOMPANHAMENTO();
            PedidoCompraAcompanhamentoViewModel vm = Mapper.Map<PEDIDO_COMPRA_ACOMPANHAMENTO, PedidoCompraAcompanhamentoViewModel>(coment);
            SessionMocks.idVolta = id;
            vm.PCAT_DT_ACOMPANHAMENTO = DateTime.Now;
            vm.PCAT_IN_ATIVO = 1;
            vm.PECO_CD_ID = id;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcompanhamento(PedidoCompraAcompanhamentoViewModel vm)
        {
            
            SessionMocks.voltaAcompanhamento = true;

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PEDIDO_COMPRA_ACOMPANHAMENTO item = Mapper.Map<PedidoCompraAcompanhamentoViewModel, PEDIDO_COMPRA_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;

                    Int32 volta = baseApp.ValidateCreateAcompanhamento(item);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VoltarAnexoPedidoCompra");
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