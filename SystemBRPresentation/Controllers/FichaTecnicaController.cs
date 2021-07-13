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
using System.Text.RegularExpressions;
using EntitiesServices.WorkClasses;
using System.Collections;
using System.Threading.Tasks;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class FichaTecnicaController : Controller
    {
        private readonly IFichaTecnicaAppService ftApp;
        private readonly IMateriaPrimaAppService matApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IProdutoAppService prodApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private String msg;
        private Exception exception;
        FICHA_TECNICA objetoFt = new FICHA_TECNICA();
        FICHA_TECNICA objetoFtAntes = new FICHA_TECNICA();
        List<FICHA_TECNICA> listaMasterFt = new List<FICHA_TECNICA>();
        MATERIA_PRIMA objetoMat = new MATERIA_PRIMA();
        MATERIA_PRIMA objetoMatAntes = new MATERIA_PRIMA();
        List<MATERIA_PRIMA> listaMasterMat = new List<MATERIA_PRIMA>();
        String extensao = String.Empty;

        public FichaTecnicaController(IFichaTecnicaAppService ftApps, IMateriaPrimaAppService matApps, IMatrizAppService matrizApps, IProdutoAppService prodApps, ICategoriaProdutoAppService cpApps)
        {
            ftApp = ftApps;
            matApp = matApps;
            matrizApp = matrizApps;
            prodApp = prodApps;
            cpApp = cpApps;
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

        public ActionResult DashboardAdministracao()
        {
            
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        public JsonResult GetUnidadeInsumo(Int32 id)
        {
            var result = new Hashtable();
            result.Add("unidade", matApp.GetById(id).UNIDADE.UNID_NM_NOME);

            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaFT()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            usuario = SessionMocks.UserCredentials;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            SessionMocks.produto = null;

            // Carrega listas
            if (SessionMocks.listaFT == null)
            {
                listaMasterFt = ftApp.GetAllItens();
                SessionMocks.listaFT = listaMasterFt;
            }
            if (SessionMocks.listaFT.Count == 0)
            {
                listaMasterFt = ftApp.GetAllItens();
                SessionMocks.listaFT = listaMasterFt;
            }
            ViewBag.Listas = SessionMocks.listaFT;
            ViewBag.Title = "Fichas Técnicas";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().Where(x => x.PROD_IN_COMPOSTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Categorias = new SelectList(cpApp.GetAllItens().OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            SessionMocks.VoltaComposto = 0;

            // Indicadores
            ViewBag.FT = ftApp.GetAllItens().Count;
            if (Session["MensFT"] != null)
            {
                if ((Int32)Session["MensFT"] == 1)
                {
                    SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                    Session["MensFT"] = 0;
                }
            }
            
            // Abre view
            objetoFt = new FICHA_TECNICA();
            return View(objetoFt);
        }

        [HttpGet]
        public ActionResult MontarTelaFTProd()
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            usuario = SessionMocks.UserCredentials;
            //if (SessionMocks.UserCredentials != null)
            //{
            //    usuario = SessionMocks.UserCredentials;

            //    // Verfifica permissão
            //    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            //    {
            //        return RedirectToAction("CarregarBase", "BaseAdmin");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Login", "ControleAcesso");
            //}

            // Carrega listas
            if (SessionMocks.listaFT == null)
            {
                listaMasterFt = ftApp.GetAllItens();
                SessionMocks.listaFT = listaMasterFt;
            }
            ViewBag.Listas = SessionMocks.listaFT;
            ViewBag.Title = "Fichas Técnicas";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            // Indicadores
            ViewBag.FT = ftApp.GetAllItens().Count;

            // Abre view
            objetoFt = new FICHA_TECNICA();
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
                    Session["MensFT"] = 1;
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
            

            SessionMocks.listaFtd = null;

            if (SessionMocks.produto != null)
            {
                return RedirectToAction("EditarProduto", "Produto", new { id = SessionMocks.produto.PROD_CD_ID });
            }
            return RedirectToAction("MontarTelaFT");
        }

        [HttpPost]
        public void MontaListaInsumos(FICHA_TECNICA_DETALHE item)
        {
            Task.Run(() => {
                if (SessionMocks.listaFtd == null)
                {
                    SessionMocks.listaFtd = new List<FICHA_TECNICA_DETALHE>();
                }

                SessionMocks.listaFtd.Add(item);
            });
        }

        [HttpPost]
        public void RemoveInsumoTabela(Int32 id)
        {
            if (SessionMocks.listaFtd != null)
            {
                SessionMocks.listaFtd.RemoveAll(x => x.MAPR_CD_ID == id);
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IncluirFT()
        {
            
            // Prepara listas
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().Where(x => x.PROD_IN_COMPOSTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Insumos = new SelectList(matApp.GetAllItens().OrderBy(x => x.MAPR_NM_NOME).ToList<MATERIA_PRIMA>(), "MAPR_CD_ID", "MAPR_NM_NOME");

            PRODUTO prod = new PRODUTO();
            if (SessionMocks.idProduto != null)
            {
                prod = prodApp.GetById(SessionMocks.idProduto);
            }

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            FICHA_TECNICA item = new FICHA_TECNICA();
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.FITE_DT_CADASTRO = DateTime.Today;
            vm.FITE_IN_ATIVO = 1;
            if (prod.PROD_CD_ID != 0)
            {
                vm.PROD_CD_ID = prod.PROD_CD_ID;
            }
            return View(vm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult IncluirFT(FichaTecnicaViewModel vm)
        {
            
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = ftApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Carrega foto e processa alteracao
                    item.FITE_AQ_APRESENTACAO = "~/Imagens/Base/FotoBase.jpg";
                    volta = ftApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    SessionMocks.idVolta = item.FITE_CD_ID;
                    if (Session["FileQueueFT"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueFT"];

                        foreach (var file in fq)
                        {
                            UploadFotoQueueFT(file);
                        }

                        Session["FileQueueFT"] = null;
                    }

                    foreach (var i in SessionMocks.listaFtd)
                    {
                        var ftd = new FICHA_TECNICA_DETALHE
                        {
                            FITD_CD_ID = i.FITD_CD_ID,
                            FITE_CD_ID = item.FITE_CD_ID,
                            MAPR_CD_ID = i.MAPR_CD_ID,
                            FITD_QN_QUANTIDADE = i.FITD_QN_QUANTIDADE,
                            FITD_NM_NOME = "",
                            FITD_DS_DESCRICAO = i.FITD_DS_DESCRICAO,
                            FITD_DT_CADASTRO = DateTime.Now.Date,
                            FITD_IN_ATIVO = 1
                        };

                        IncluirTabelaInsumoFT(ftd);
                    }

                    SessionMocks.listaFtd = null;

                    // Sucesso
                    if (SessionMocks.VoltaComposto == 0)
                    {
                        listaMasterFt = new List<FICHA_TECNICA>();
                        SessionMocks.listaFT = null;
                        return RedirectToAction("MontarTelaFT");
                    }
                    else
                    {
                        SessionMocks.listaProduto = null;
                        return RedirectToAction("MontarTelaProduto", "Produto");
                    }
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
        public ActionResult EditarInsumoFT(Int32 id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            FICHA_TECNICA_DETALHE item = ftApp.GetDetalheById(id);
            FichaTecnicaDetalheViewModel vm = Mapper.Map<FICHA_TECNICA_DETALHE, FichaTecnicaDetalheViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarInsumoFT(FichaTecnicaDetalheViewModel vm)
        {
            
            ViewBag.Insumos = new SelectList(matApp.GetAllItens(), "MAPR_CD_ID", "MAPR_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FICHA_TECNICA_DETALHE item = Mapper.Map<FichaTecnicaDetalheViewModel, FICHA_TECNICA_DETALHE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = ftApp.ValidateEditInsumo(item);

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
        public ActionResult EditarFT(Int32 id)
        {
            
            // Prepara view
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            FICHA_TECNICA item = ftApp.GetItemById(id);
            objetoFtAntes = item;
            SessionMocks.voltaFT = 1;
            SessionMocks.fichaTecnica = item;
            SessionMocks.idVolta = id;
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditarFT(FichaTecnicaViewModel vm)
        {
            
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
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

                    if (SessionMocks.produto != null)
                    {
                        return RedirectToAction("EditarProduto", "Produto", new { id = SessionMocks.produto.PROD_CD_ID });
                    }

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
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            FICHA_TECNICA item = ftApp.GetItemById(prodApp.GetItemById(SessionMocks.idVolta).FICHA_TECNICA.FirstOrDefault().FITE_CD_ID);
            objetoFtAntes = item;
            Session["idFicha"] = item.FITE_CD_ID;
            SessionMocks.fichaTecnica = item;
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisualizarEditarFTProduto(FichaTecnicaViewModel vm)
        {
            
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
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
                return View(vm);
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
                return View(vm);
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

        [HttpPost]
        public void IncluirTabelaInsumoFT(FICHA_TECNICA_DETALHE item)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                Int32 volta = ftApp.ValidateCreateInsumo(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
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

        [HttpGet]
        public ActionResult CriarFichaTecnicaProduto()
        {
            
            // Prepara listas
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            FICHA_TECNICA item = new FICHA_TECNICA();
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.FITE_DT_CADASTRO = DateTime.Today;
            vm.FITE_IN_ATIVO = 1;
            vm.PROD_CD_ID = SessionMocks.idVolta;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult CriarFichaTecnicaProduto(FichaTecnicaViewModel vm)
        {
            
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens().OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = ftApp.ValidateCreateProduto(item, usuarioLogado);

                    // Verifica retorno

                    // Carrega foto e processa alteracao
                    item.FITE_AQ_APRESENTACAO = "~/Imagens/Base/FotoBase.jpg";
                    volta = ftApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterFt = new List<FICHA_TECNICA>();
                    SessionMocks.listaFT = null;
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
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

        public ActionResult VoltarAnexoFT()
        {
            
            return RedirectToAction("EditarFT", new { id = SessionMocks.idVolta });
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

            Session["FileQueueFT"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFotoQueueFT(FileQueue file)
        {
            
            string random = CrossCutting.RandomStringGenerator.RandomString(10);

            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFT");
            }

            FICHA_TECNICA item = ftApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = random + file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFT");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.FITE_AQ_APRESENTACAO = "~" + caminho + fileName;
            objetoFtAntes = item;
            Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes);
            return RedirectToAction("VoltarAnexoFT");
        }

        [HttpPost]
        public ActionResult UploadFotoFT(HttpPostedFileBase file)
        {
            
            string random = CrossCutting.RandomStringGenerator.RandomString(10);

            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFT");
            }

            FICHA_TECNICA item = ftApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = random + Path.GetExtension(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFT");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.FITE_AQ_APRESENTACAO = "~" + caminho + fileName;
            objetoFtAntes = item;
            Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes);
            return RedirectToAction("VoltarAnexoFT");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "FTLista" + "_" + data + ".pdf";
            List<FICHA_TECNICA> lista = SessionMocks.listaFT;
            FICHA_TECNICA filtro = SessionMocks.filtroFT;
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
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

            cell = new PdfPCell(new Paragraph("Fichas Técnicas - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 100f, 120f, 120f, 90f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Fichas Técnicas selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 4;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
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
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Imagem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (FICHA_TECNICA item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.PRODUTO.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.FITE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.FITE_AQ_APRESENTACAO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.FITE_AQ_APRESENTACAO));
                    image.ScaleAbsolute(40, 40);
                    cell.AddElement(image);
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
                if (filtro.PROD_CD_ID > 0)
                {
                    PRODUTO pro = prodApp.GetItemById(filtro.PROD_CD_ID);
                    parametros += "Produto: " + pro.PROD_NM_NOME;
                    ja = 1;
                }
                if (filtro.FITE_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.FITE_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Nome: " + filtro.FITE_NM_NOME;
                    }
                }
                if (filtro.FITE_S_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.FITE_S_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.FITE_S_DESCRICAO;
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

            return RedirectToAction("MontarTelaFT");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            // Prepara geração
            FICHA_TECNICA aten = ftApp.GetItemById(SessionMocks.idVolta);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "FichaTecnica_" + aten.FITE_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

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

            cell = new PdfPCell(new Paragraph("Ficha Técnica - Detalhes", meuFont2))
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

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (System.IO.File.Exists(Server.MapPath(aten.FITE_AQ_APRESENTACAO)))
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 2;
                image = Image.GetInstance(Server.MapPath(aten.FITE_AQ_APRESENTACAO));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Produto: " + aten.PRODUTO.PROD_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome: " + aten.FITE_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Modo de Produção: ", meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            var pattern = @"<(img|a)[^>]*>(?<content>[^<]*)<";
            var regex = new Regex(pattern);
            var m = regex.Match(aten.FITE_S_DESCRICAO);
            if (m.Success)
            {
                aten.FITE_S_DESCRICAO = m.Groups["content"].Value;
            }
            cell = new PdfPCell(new Paragraph(Regex.Replace(aten.FITE_S_DESCRICAO, @"<.*?>", ""), meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Apresentação: ", meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(Regex.Replace(aten.FITE_DS_APRESENTACAO, @"<.*?>", ""), meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Lista de Insumos
            if (aten.FICHA_TECNICA_DETALHE.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 70f, 70f, 300f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Insumo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Unidade", meuFont))
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
                cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (FICHA_TECNICA_DETALHE item in aten.FICHA_TECNICA_DETALHE)
                {
                    cell = new PdfPCell(new Paragraph(item.MATERIA_PRIMA.MAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MATERIA_PRIMA.UNIDADE.UNID_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FITD_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FITD_DS_DESCRICAO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("VoltarAnexoFT");
        }
    }
}