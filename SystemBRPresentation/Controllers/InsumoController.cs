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
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using EntitiesServices.WorkClasses;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class InsumoController : Controller
    {
        private readonly IFornecedorAppService fornApp;
        private readonly IMateriaPrimaAppService matApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IFilialAppService filApp;
        private readonly ICategoriaMateriaAppService cmApp;
        private readonly IMateriaPrimaPrecoAppService mppApp;
        private readonly ISubcategoriaMateriaAppService scmpApp;

        private String msg;
        private Exception exception;
        FORNECEDOR objetoForn = new FORNECEDOR();
        FORNECEDOR objetoFornAntes = new FORNECEDOR();
        List<FORNECEDOR> listaMasterForn = new List<FORNECEDOR>();
        MATERIA_PRIMA objetoMat = new MATERIA_PRIMA();
        MATERIA_PRIMA objetoMatAntes = new MATERIA_PRIMA();
        List<MATERIA_PRIMA> listaMasterMat = new List<MATERIA_PRIMA>();
        String extensao = String.Empty;

        public InsumoController(IFornecedorAppService fornApps, IMateriaPrimaAppService matApps, IMatrizAppService matrizApps, IFilialAppService filApps, ICategoriaMateriaAppService cmApps, IMateriaPrimaPrecoAppService mppApps, ISubcategoriaMateriaAppService scmpApps)
        {
            fornApp = fornApps;
            matApp = matApps;
            matrizApp = matrizApps;
            filApp = filApps;
            cmApp = cmApps;
            mppApp = mppApps;
            scmpApp = scmpApps;
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

        [HttpPost]
        public JsonResult GetFornecedor(Int32 id)
        {
            var forn = fornApp.GetItemById(id);

            var hash = new Hashtable();
            hash.Add("cnpj", forn.FORN_NR_CNPJ);
            hash.Add("email", forn.FORN_NM_EMAIL);
            hash.Add("tel", forn.FORN_NM_TELEFONES);

            return Json(hash);
        }

        public ActionResult IncluirCategoriaMateria()
        {
            
            SessionMocks.CategoriaToMateria = true;
            return RedirectToAction("IncluirCategoriaMateria", "TabelasAuxiliares");
        }

        public ActionResult IncluirSubCategoriaMateria()
        {
            
            SessionMocks.SubCategoriaToMateria = true;
            return RedirectToAction("IncluirSubCategoriaMateria", "TabelasAuxiliares");
        }

        [HttpGet]
        public ActionResult MontarTelaMateria()
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

            // Carrega listas
            if (SessionMocks.listaMateria == null)
            {
                listaMasterMat = matApp.GetAllItens();
                SessionMocks.listaMateria = listaMasterMat;
            }
            if (SessionMocks.listaMateria.Count == 0)
            {
                listaMasterMat = matApp.GetAllItens();
                SessionMocks.listaMateria = listaMasterMat;
            }

            ViewBag.Listas = SessionMocks.listaMateria;
            ViewBag.Materias = SessionMocks.listaMateria.Count;

            if (usuario.FILIAL != null)
            {
                ViewBag.FilialUsuario = usuario.FILIAL.FILI_NM_NOME;
            }

            ViewBag.Title = "Insumos";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(SessionMocks.CatsInsumos.OrderBy(x => x.CAMA_NM_NOME).ToList<CATEGORIA_MATERIA>(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Subs = new SelectList(scmpApp.GetAllItens().OrderBy(x => x.SCMP_NM_NOME).ToList<SUBCATEGORIA_MATERIA>(), "SCMP_CD_ID", "SCMP_NM_NOME");
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            ViewBag.CdUsuario = SessionMocks.Usuario.USUA_CD_ID;

            // Indicadores
            SessionMocks.pontoPedidoIns = matApp.GetAllItens().Where(p => (p.MAPR_QN_ESTOQUE < p.MAPR_QN_ESTOQUE_MINIMO || p.MAPR_QN_ESTOQUE == 0) & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            SessionMocks.estoqueZeradoIns = matApp.GetAllItens().Where(p => p.MAPR_QN_ESTOQUE == 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();

            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                ViewBag.PontoPedido = SessionMocks.pontoPedidoIns.Count;
                ViewBag.EstoqueZerado = SessionMocks.estoqueZeradoIns.Count;
            }
            else
            {
                ViewBag.PontoPedido = SessionMocks.pontoPedidoIns.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList().Count;
                ViewBag.EstoqueZerado = SessionMocks.estoqueZeradoIns.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList().Count;
            }
            ViewBag.PontoPedidos = SessionMocks.pontoPedidoIns;
            ViewBag.EstoqueZerados = SessionMocks.estoqueZeradoIns;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMateria"] != null)
            {
                if ((Int32)Session["MensMateria"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensMateria"] = 0;
                }
            }

            // Abre view
            objetoMat = new MATERIA_PRIMA();
            SessionMocks.voltaConsulta = 1;
            SessionMocks.voltaMateria = 1;
            objetoMat.MAPR_IN_ATIVO = 1;
            return View(objetoMat);
        }

        public ActionResult MontarTelaMateriaProd()
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
            if (SessionMocks.listaMateria == null)
            {
                listaMasterMat = matApp.GetAllItens();
                SessionMocks.listaMateria = listaMasterMat;
            }
            ViewBag.Listas = SessionMocks.listaMateria;
            ViewBag.Title = "Matéria-Prima";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(SessionMocks.CatsInsumos, "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");

            // Indicadores
            SessionMocks.pontoPedidoIns = SessionMocks.listaMateria.Where(p => p.MAPR_QN_ESTOQUE < p.MAPR_QN_ESTOQUE_MINIMO & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            SessionMocks.estoqueZeradoIns = SessionMocks.listaMateria.Where(p => p.MAPR_QN_ESTOQUE <= 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            ViewBag.Materias = SessionMocks.listaMateria.Count;
            ViewBag.PontoPedido = SessionMocks.pontoPedidoIns.Count;
            ViewBag.EstoqueZerado = SessionMocks.estoqueZeradoIns.Count;
            ViewBag.PontoPedidos = SessionMocks.pontoPedidoIns;
            ViewBag.EstoqueZerados = SessionMocks.estoqueZeradoIns;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objetoMat = new MATERIA_PRIMA();
            SessionMocks.voltaConsulta = 1;
            SessionMocks.voltaMateria = 1;
            SessionMocks.flagVoltaIns = 1;
            if (SessionMocks.filtroMateria != null)
            {
                objetoMat = SessionMocks.filtroMateria;
            }
            return View(objetoMat);
        }

        public ActionResult RetirarFiltroMateria()
        {
            
            SessionMocks.listaMateria = null;
            SessionMocks.filtroMateria = null;
            SessionMocks.pontoPedidoIns = null;
            SessionMocks.estoqueZeradoIns = null;
            if (SessionMocks.voltaMateria == 2)
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
                Int32? ativo = null;
                if (SessionMocks.voltaConsulta != 2 && SessionMocks.voltaConsulta != 3)
                {
                    ativo = item.MAPR_IN_ATIVO;
                }
                Int32 volta = matApp.ExecuteFilter(item.CAMA_CD_ID, item.SCMP_CD_ID, item.MAPR_NM_NOME, item.MAPR_DS_DESCRICAO, item.MAPR_CD_CODIGO, ativo, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensMateria"] = 1;
                }

                // Sucesso
                if (SessionMocks.voltaMateria == 2)
                {
                    listaMasterMat = listaObj;
                    SessionMocks.listaMateria = listaObj;
                    return RedirectToAction("VerCardsMateria");
                }
                if (SessionMocks.voltaConsulta == 2)
                {
                    listaMasterMat = listaObj.Where(p => (p.MAPR_QN_ESTOQUE < p.MAPR_QN_ESTOQUE_MINIMO || p.MAPR_QN_ESTOQUE == 0) & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
                    SessionMocks.pontoPedidoIns = listaObj.Where(p => (p.MAPR_QN_ESTOQUE < p.MAPR_QN_ESTOQUE_MINIMO || p.MAPR_QN_ESTOQUE == 0) & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
                    if (SessionMocks.pontoPedidoIns.Count == 0)
                    {
                        Session["MensMateria"] = 1;
                    }
                    return RedirectToAction("VerInsumosPontoPedido");
                }
                if (SessionMocks.voltaConsulta == 3)
                {
                    listaMasterMat = listaObj.Where(p => p.MAPR_QN_ESTOQUE == 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
                    SessionMocks.estoqueZeradoIns = listaObj.Where(p => p.MAPR_QN_ESTOQUE == 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
                    if (SessionMocks.estoqueZeradoIns.Count == 0)
                    {
                        Session["MensMateria"] = 1;
                    }
                    return RedirectToAction("VerInsumosEstoqueZerado");
                }
                listaMasterMat = listaObj;
                SessionMocks.listaMateria = listaObj;
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
            
            if (SessionMocks.voltaFT == 1)
            {
                return RedirectToAction("EditarFT", "FichaTecnica", new { id = SessionMocks.idVolta });
            }
            if ((Int32)Session["VoltaEstoque"] == 1)
            {
                return RedirectToAction("MontarTelaEstoqueInsumo", "Estoque");
            }
            if (SessionMocks.flagVoltaIns == 1)
            {
                if (SessionMocks.voltaMateria == 2)
                {
                    return RedirectToAction("VerCardsMateria");
                }
                return RedirectToAction("MontarTelaMateria");
            }
            else
            {
                return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirMateria()
        {
            
            // Prepara listas
            ViewBag.Tipos = new SelectList(cmApp.GetAllItens().OrderBy(x => x.CAMA_NM_NOME).ToList<CATEGORIA_MATERIA>(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Subs = new SelectList(scmpApp.GetAllItens().OrderBy(x => x.SCMP_NM_NOME).ToList<SUBCATEGORIA_MATERIA>(), "SCMP_CD_ID", "SCMP_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            MATERIA_PRIMA item = new MATERIA_PRIMA();
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.MAPR_DT_CADASTRO = DateTime.Today;
            vm.MAPR_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public void MontaListaPreco(MATERIA_PRIMA_PRECO item)
        {
            Task.Run(() => {
                if (SessionMocks.listaPrecoMateria == null)
                {
                    SessionMocks.listaPrecoMateria = new List<MATERIA_PRIMA_PRECO>();
                }

                SessionMocks.listaPrecoMateria.Add(item);
            });
        }

        [HttpPost]
        public void RemovePrecoTabela(MATERIA_PRIMA_PRECO item)
        {
            Task.Run(() => {
                if (SessionMocks.listaPrecoMateria != null)
                {
                    SessionMocks.listaPrecoMateria.RemoveAll(x => x.FILI_CD_ID == item.FILI_CD_ID);
                }
            });
        }

        [HttpPost]
        public ActionResult IncluirMateria(MateriaPrimaViewModel vm, String tabelaMateria)
        {
            
            vm.SCMA_CD_ID = vm.SCMP_CD_ID;

            ViewBag.Tipos = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Subs = new SelectList(scmpApp.GetAllItens(), "SCMP_CD_ID", "SCMP_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    vm.MAPR_QN_QUANTIDADE_INICIAL = 0;

                    // Executa a operação
                    MATERIA_PRIMA item = Mapper.Map<MateriaPrimaViewModel, MATERIA_PRIMA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = matApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Acerta codigo do insumo
                    item.MAPR_CD_CODIGO = item.MAPR_CD_ID.ToString();
                    volta = matApp.ValidateEdit(item, item, usuarioLogado);

                    // Carrega foto e processa alteracao
                    item.MAPR_AQ_FOTO = "~/Imagens/Base/FotoBase.jpg";
                    volta = matApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + item.MAPR_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + item.MAPR_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    SessionMocks.idVolta = item.MAPR_CD_ID;

                    if (Session["FileQueueMateria"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueMateria"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueMateria(file);
                            }
                            else
                            {
                                UploadFotoQueueMateria(file);
                            }
                        }

                        Session["FileQueueMateria"] = null;
                    }

                    // Sucesso
                    vm.MAPR_CD_ID = item.MAPR_CD_ID;
                    IncluirTabelaMateriaPrima(vm, tabelaMateria);

                    listaMasterMat = new List<MATERIA_PRIMA>();
                    SessionMocks.listaMateria = null;
                    if (SessionMocks.voltaMateria == 2)
                    {
                        return RedirectToAction("VerCardsMateria");
                    }
                    return RedirectToAction("MontarTelaMateria"); // MontarTelaMateria
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
        public ActionResult EditarMateria(Int32 id)
        {
            
            // Prepara view
            ViewBag.Tipos = new SelectList(cmApp.GetAllItens().OrderBy(x => x.CAMA_NM_NOME).ToList<CATEGORIA_MATERIA>(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            MATERIA_PRIMA item = matApp.GetItemById(id);
            ViewBag.Subs = new SelectList(scmpApp.GetAllItens().OrderBy(x => x.SCMP_NM_NOME).ToList<SUBCATEGORIA_MATERIA>(), "SCMP_CD_ID", "SCMP_NM_NOME", item.SCMP_CD_ID);
            objetoMatAntes = item;

            if ((Int32)Session["MensMateriaPrima"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
            }

            Session["MensMateriaPrima"] = 0;
            SessionMocks.materiaPrima = item;
            SessionMocks.idVolta = id;
            SessionMocks.flagVoltaIns = 1;
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
            if (SessionMocks.UserCredentials.FILI_CD_ID != null)
            {
                vm.MOVIMENTO_ESTOQUE_MATERIA_PRIMA = vm.MOVIMENTO_ESTOQUE_MATERIA_PRIMA.Where(x => x.FILI_CD_ID != null || x.FILI_CD_ID == SessionMocks.UserCredentials.FILI_CD_ID).ToList<MOVIMENTO_ESTOQUE_MATERIA_PRIMA>();
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMateria(MateriaPrimaViewModel vm)
        {
            
            vm.SCMA_CD_ID = vm.SCMP_CD_ID;
            ViewBag.Tipos = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Subs = new SelectList(scmpApp.GetAllItens(), "SCMP_CD_ID", "SCMP_NM_NOME");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    MATERIA_PRIMA item = Mapper.Map<MateriaPrimaViewModel, MATERIA_PRIMA>(vm);
                    Int32 volta = matApp.ValidateEdit(item, objetoMatAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMat = new List<MATERIA_PRIMA>();
                    SessionMocks.listaMateria = null;
                    if (SessionMocks.voltaFT == 1)
                    {
                        return RedirectToAction("EditarFT", "FichaTecnica", new { id = SessionMocks.idVolta });
                    }
                    if ((Int32)Session["VoltaEstoque"] == 1)
                    {
                        return RedirectToAction("MontarTelaEstoqueInsumo", "Estoque");
                    }
                    if (SessionMocks.voltaMateria == 2)
                    {
                        return RedirectToAction("VerCardsMateria");
                    }
                    return RedirectToAction("MontarTelaMateria");
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
        public ActionResult ConsultarMateria(Int32 id)
        {
            
            var usu = SessionMocks.UserCredentials;

            // Prepara view
            ViewBag.Tipos = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            MATERIA_PRIMA item = matApp.GetItemById(id);
            ViewBag.Subs = new SelectList(scmpApp.GetAllItens(), "SCMP_CD_ID", "SCMP_NM_NOME", item.SCMP_CD_ID);
            ViewBag.Perfil = usu.PERFIL.PERF_SG_SIGLA;
            ViewBag.CdUsuario = usu.USUA_CD_ID;
            if (usu.FILIAL != null)
            {
                ViewBag.FilialUsuario = usu.FILIAL.FILI_CD_ID;
            }
            objetoMatAntes = item;

            if ((Int32)Session["MensMateriaPrima"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
            }

            Session["MensMateriaPrima"] = 0;
            SessionMocks.materiaPrima = item;
            SessionMocks.idVolta = id;
            SessionMocks.flagVoltaIns = 1;
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);

            if (item.SCMP_CD_ID != null)
            {
                vm.SUBCATEGORIA_MATERIA = scmpApp.GetItemById((Int32)item.SCMP_CD_ID);
            }

            return View(vm);
        }

        // Filtro em cascata de subcategoria
        [HttpPost]
        public JsonResult FiltrarSubCategoriaMateria(Int32? id)
        {
            var listaSubFiltrada = new List<SUBCATEGORIA_MATERIA>();

            // Filtro para caso o placeholder seja selecionado
            if (id == null)
            {
                listaSubFiltrada = scmpApp.GetAllItens();
            }
            else
            {
                listaSubFiltrada = scmpApp.GetAllItens().Where(x => x.CAMA_CD_ID == id).ToList();
            }

            return Json(listaSubFiltrada.Select(x => new { x.SCMP_CD_ID, x.SCMP_NM_NOME }));
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
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0098", CultureInfo.CurrentCulture));
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
                return View(vm);
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
                return View(vm);
            }
        }

        public ActionResult VerCardsMateria()
        {
            
            // Carrega listas
            if (SessionMocks.listaMateria == null)
            {
                listaMasterMat = matApp.GetAllItens();
                SessionMocks.listaMateria = listaMasterMat;
            }
            ViewBag.Listas = SessionMocks.listaMateria;
            ViewBag.Title = "Insumos";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(SessionMocks.CatsInsumos, "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");

            // Indicadores
            ViewBag.Materias = matApp.GetAllItens().Count;

            // Abre view
            objetoMat = new MATERIA_PRIMA();
            if (SessionMocks.filtroMateria != null)
            {
                objetoMat = SessionMocks.filtroMateria;
            }
            SessionMocks.voltaMateria = 2;
            return View(objetoMat);
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

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
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

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }

                queue.Add(f);
            }

            Session["FileQueueMateria"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueMateria(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }

            MATERIA_PRIMA item = matApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + item.MAPR_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            MATERIA_PRIMA_ANEXO foto = new MATERIA_PRIMA_ANEXO();
            foto.MAPA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.MAPA_DT_ANEXO = DateTime.Today;
            foto.MAPA_IN_ATIVO = 1;
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
            foto.MAPA_IN_TIPO = tipo;
            foto.MAPA_NM_TITULO = fileName;
            foto.MAPR_D_ID = item.MAPR_CD_ID;

            item.MATERIA_PRIMA_ANEXO.Add(foto);
            objetoMatAntes = item;
            Int32 volta = matApp.ValidateEdit(item, objetoMatAntes);
            return RedirectToAction("VoltarAnexoMateria");
        }

        [HttpPost]
        public ActionResult UploadFileMateria(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }

            MATERIA_PRIMA item = matApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + item.MAPR_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            MATERIA_PRIMA_ANEXO foto = new MATERIA_PRIMA_ANEXO();
            foto.MAPA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.MAPA_DT_ANEXO = DateTime.Today;
            foto.MAPA_IN_ATIVO = 1;
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
            foto.MAPA_IN_TIPO = tipo;
            foto.MAPA_NM_TITULO = fileName;
            foto.MAPR_D_ID = item.MAPR_CD_ID;

            item.MATERIA_PRIMA_ANEXO.Add(foto);
            objetoMatAntes = item;
            Int32 volta = matApp.ValidateEdit(item, objetoMatAntes);
            return RedirectToAction("VoltarAnexoMateria");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueMateria(FileQueue file)
        {
            
            var random = CrossCutting.RandomStringGenerator.RandomString(10);

            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }

            MATERIA_PRIMA item = matApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + item.MAPR_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.MAPR_AQ_FOTO = "~" + caminho + fileName;
            objetoMatAntes = item;
            Int32 volta = matApp.ValidateEdit(item, objetoMatAntes);
            return RedirectToAction("VoltarAnexoMateria");
        }

        [HttpPost]
        public ActionResult UploadFotoMateria(HttpPostedFileBase file)
        {
            
            var random = CrossCutting.RandomStringGenerator.RandomString(10);

            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }

            MATERIA_PRIMA item = matApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = random + Path.GetExtension(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoMateria");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + item.MAPR_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.MAPR_AQ_FOTO = "~" + caminho + fileName;
            objetoMatAntes = item;
            Int32 volta = matApp.ValidateEdit(item, objetoMatAntes);
            return RedirectToAction("VoltarAnexoMateria");
        }

        public ActionResult VerMovimentacaoEstoqueMateria()
        {
            
            // Prepara view
            MATERIA_PRIMA item = matApp.GetItemById(SessionMocks.idVolta);
            objetoMatAntes = item;
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
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
        public ActionResult EditarMateriaFornecedor(Int32 id)
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            MATERIA_PRIMA_FORNECEDOR item = matApp.GetFornecedorById(id);
            objetoMatAntes = SessionMocks.materiaPrima;
            MateriaPrimaFornecedorViewModel vm = Mapper.Map<MATERIA_PRIMA_FORNECEDOR, MateriaPrimaFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarMateriaFornecedor(MateriaPrimaFornecedorViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    MATERIA_PRIMA_FORNECEDOR item = Mapper.Map<MateriaPrimaFornecedorViewModel, MATERIA_PRIMA_FORNECEDOR>(vm);
                    Int32 volta = matApp.ValidateEditFornecedor(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoMateria");
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
        public ActionResult ExcluirMateriaFornecedor(Int32 id)
        {
            
            MATERIA_PRIMA_FORNECEDOR item = matApp.GetFornecedorById(id);
            objetoMatAntes = SessionMocks.materiaPrima;
            item.MAFO_IN_ATIVO = 0;
            Int32 volta = matApp.ValidateEditFornecedor(item);
            return RedirectToAction("VoltarAnexoMateria");
        }

        [HttpGet]
        public ActionResult ReativarMateriaFornecedor(Int32 id)
        {
            
            MATERIA_PRIMA_FORNECEDOR item = matApp.GetFornecedorById(id);
            objetoMatAntes = SessionMocks.materiaPrima;
            item.MAFO_IN_ATIVO = 1;
            Int32 volta = matApp.ValidateEditFornecedor(item);
            return RedirectToAction("VoltarAnexoMateria");
        }

        [HttpGet]
        public ActionResult IncluirMateriaFornecedor()
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            USUARIO usuario = SessionMocks.UserCredentials;
            MATERIA_PRIMA_FORNECEDOR item = new MATERIA_PRIMA_FORNECEDOR();
            MateriaPrimaFornecedorViewModel vm = Mapper.Map<MATERIA_PRIMA_FORNECEDOR, MateriaPrimaFornecedorViewModel>(item);
            vm.MAPR_CD_ID = SessionMocks.idVolta;
            vm.MAFO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMateriaFornecedor(MateriaPrimaFornecedorViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MATERIA_PRIMA_FORNECEDOR item = Mapper.Map<MateriaPrimaFornecedorViewModel, MATERIA_PRIMA_FORNECEDOR>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = matApp.ValidateCreateFornecedor(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoMateria");
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

        public ActionResult VerInsumosPontoPedido()
        {
            
            // Prepara view
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(SessionMocks.CatsInsumos, "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");

            if (SessionMocks.pontoPedidoIns == null)
            {
                SessionMocks.pontoPedidoIns = matApp.GetAllItens().Where(p => (p.MAPR_QN_ESTOQUE < p.MAPR_QN_ESTOQUE_MINIMO || p.MAPR_QN_ESTOQUE == 0) & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            }
            if (SessionMocks.pontoPedidoIns.Count == 0)
            {
                SessionMocks.pontoPedidoIns = matApp.GetAllItens().Where(p => (p.MAPR_QN_ESTOQUE < p.MAPR_QN_ESTOQUE_MINIMO || p.MAPR_QN_ESTOQUE == 0) & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            }

            ViewBag.PontoPedido = SessionMocks.pontoPedidoIns.Count;
            ViewBag.PontoPedidos = SessionMocks.pontoPedidoIns;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMateria"] != null)
            {
                if ((Int32)Session["MensMateria"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensMateria"] = 0;
                }
            }

            // Abre view
            objetoMat = new MATERIA_PRIMA();
            SessionMocks.voltaMateria = 1;
            SessionMocks.voltaConsulta = 2;
            SessionMocks.clonar = 0;
            if (SessionMocks.filtroMateria != null)
            {
                objetoMat = SessionMocks.filtroMateria;
            }
            return View(objetoMat);
        }

        public ActionResult VerInsumosEstoqueZerado()
        {
            
            // Prepara view
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(SessionMocks.CatsInsumos, "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");

            if (SessionMocks.estoqueZeradoIns == null)
            {
                SessionMocks.estoqueZeradoIns = matApp.GetAllItens().Where(p => p.MAPR_QN_ESTOQUE == 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            }
            if (SessionMocks.estoqueZeradoIns.Count == 0)
            {
                SessionMocks.estoqueZeradoIns = matApp.GetAllItens().Where(p => p.MAPR_QN_ESTOQUE == 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            }

            ViewBag.EstoqueZerado = SessionMocks.estoqueZeradoIns.Count;
            ViewBag.EstoqueZerados = SessionMocks.estoqueZeradoIns;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMateria"] != null)
            {
                if ((Int32)Session["MensMateria"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensMateria"] = 0;
                }
            }

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

        public ActionResult VerInsumosEstoqueNegativo()
        {
            
            // Prepara view
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(SessionMocks.CatsInsumos, "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades.Where(x => x.UNID_IN_TIPO_UNIDADE == 1).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");

            if (SessionMocks.estoqueZeradoIns == null)
            {
                SessionMocks.estoqueZeradoIns = matApp.GetAllItens().Where(p => p.MAPR_QN_ESTOQUE <= 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            }
            if (SessionMocks.estoqueZeradoIns.Count == 0)
            {
                SessionMocks.estoqueZeradoIns = matApp.GetAllItens().Where(p => p.MAPR_QN_ESTOQUE <= 0 & p.MAPR_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList();
            }

            ViewBag.EstoqueNegativo = SessionMocks.estoqueNegativoIns.Count;
            ViewBag.EstoqueNegativos = SessionMocks.estoqueNegativoIns;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensMateria"] != null)
            {
                if ((Int32)Session["MensMateria"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    Session["MensMateria"] = 0;
                }
            }

            // Abre view
            objetoMat = new MATERIA_PRIMA();
            SessionMocks.voltaMateria = 1;
            SessionMocks.voltaConsulta = 4;
            SessionMocks.clonar = 0;
            if (SessionMocks.filtroMateria != null)
            {
                objetoMat = SessionMocks.filtroMateria;
            }
            return View(objetoMat);
        }

        public ActionResult ClonarMateria(Int32 id)
        {
            
            // Prepara objeto
            USUARIO usuario = SessionMocks.UserCredentials;
            MATERIA_PRIMA item = matApp.GetItemById(id);
            MATERIA_PRIMA novo = new MATERIA_PRIMA();
            novo.ASSI_CD_ID = item.ASSI_CD_ID;
            novo.CAMA_CD_ID = item.CAMA_CD_ID;
            novo.FILI_CD_ID = item.FILI_CD_ID;
            novo.MATR_CD_ID = item.MATR_CD_ID;
            novo.MAPR_AQ_FOTO = item.MAPR_AQ_FOTO;
            novo.MAPR_CD_CODIGO = item.MAPR_CD_CODIGO;
            novo.MAPR_CD_CODIGO = item.MAPR_CD_CODIGO;
            novo.MAPR_DT_CADASTRO = DateTime.Today.Date;
            novo.MAPR_DT_ULTIMA_MOVIMENTACAO = DateTime.Today.Date;
            novo.MAPR_DT_VALIDADE = DateTime.Today.Date.AddDays(300);
            novo.MAPR_IN_ATIVO = 1;
            novo.MAPR_IN_AVISA_MINIMO = item.MAPR_IN_AVISA_MINIMO;
            novo.MAPR_NM_NOME = "====== INSUMO DUPLICADO ======";
            novo.MAPR_NM_REFERENCIA = item.MAPR_NM_REFERENCIA;
            novo.MAPR_NR_FATOR_CONVERSAO = item.MAPR_NR_FATOR_CONVERSAO;
            novo.MAPR_PERDA_PROCESSAMENTO = item.MAPR_PERDA_PROCESSAMENTO;
            novo.MAPR_QN_ESTOQUE = item.MAPR_QN_ESTOQUE;
            novo.MAPR_QN_ESTOQUE_INICIAL = item.MAPR_QN_ESTOQUE_INICIAL;
            novo.MAPR_QN_ESTOQUE_MAXIMO = item.MAPR_QN_ESTOQUE_MAXIMO;
            novo.MAPR_QN_ESTOQUE_MINIMO = item.MAPR_QN_ESTOQUE_MINIMO;
            novo.MAPR_QN_QUANTIDADE_INICIAL = item.MAPR_QN_QUANTIDADE_INICIAL;
            novo.MAPR_QN_QUANTIDADE_M = item.MAPR_QN_QUANTIDADE_M;
            novo.MAPR_QN_RESERVA_ESTOQUE = item.MAPR_QN_RESERVA_ESTOQUE;
            novo.MAPR_TX_OBSERVACOES = item.MAPR_TX_OBSERVACOES;
            novo.UNID_CD_ID = item.UNID_CD_ID;       

            Int32 volta = matApp.ValidateCreateLeve(novo, usuario);

            // Acerta codigo do insumo
            novo.MAPR_CD_CODIGO = novo.MAPR_CD_ID.ToString();
            Int32 volta1 = matApp.ValidateEdit(item, item, usuario);

            // Carrega foto e processa alteracao
            novo.MAPR_AQ_FOTO = "~/Imagens/Base/FotoBase.jpg";
            volta1 = matApp.ValidateEdit(item, item, usuario);

            // Cria pastas
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + novo.MAPR_CD_ID.ToString() + "/Fotos/";
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Materia/" + novo.MAPR_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));

            SessionMocks.idVolta = novo.MAPR_CD_ID;
            SessionMocks.clonar = 1;
            return RedirectToAction("VoltarAnexoMateria");
        }

        public ActionResult GerarRelatorioFiltro()
        {
            
            return RedirectToAction("GerarRelatorioLista", new { id = 1 });
        }

        public ActionResult GerarRelatorioZerado()
        {
            
            return RedirectToAction("GerarRelatorioLista", new { id = 2 });
        }

        public ActionResult GerarRelatorioPonto()
        {
            
            return RedirectToAction("GerarRelatorioLista", new { id = 3 });
        }

        public ActionResult GerarRelatorioNegativo()
        {
            
            return RedirectToAction("GerarRelatorioLista", new { id = 4 });
        }

        public ActionResult GerarRelatorioLista(Int32 id)
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = String.Empty;
            String titulo = String.Empty;
            List<MATERIA_PRIMA> lista = new List<MATERIA_PRIMA>();
            if (id == 1)
            {
                nomeRel = "InsumoLista" + "_" + data + ".pdf";
                titulo = "Insumos - Listagem";
                lista = SessionMocks.listaMateria;
            }
            else if (id == 2 )
            {
                nomeRel = "InsumoZerado" + "_" + data + ".pdf";
                titulo = "Insumos - Estoque Zerado";
                lista = SessionMocks.estoqueZeradoIns;
            }
            else if (id == 3)
            {
                nomeRel = "InsumoPonto" + "_" + data + ".pdf";
                titulo = "Insumos - Ponto de Pedido";
                lista = SessionMocks.pontoPedidoIns;
            }
            else
            {
                nomeRel = "InsumoPonto" + "_" + data + ".pdf";
                titulo = "Insumos - Estoque Negativo";
                lista = SessionMocks.pontoPedidoIns;
            }
            MATERIA_PRIMA filtro = SessionMocks.filtroMateria;
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
            table = new PdfPTable(new float[] { 80f, 80f, 150f, 80f, 80f, 80f, 80f, 50f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Insumos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código", meuFont))
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
            cell = new PdfPCell(new Paragraph("Unidade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quant.Mínima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quant.Estoque", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Reserva Estoque", meuFont))
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

            foreach (MATERIA_PRIMA item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_MATERIA.CAMA_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_CD_CODIGO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.UNIDADE.UNID_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_QN_QUANTIDADE_M.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_QN_ESTOQUE.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_QN_RESERVA_ESTOQUE.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.MAPR_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.MAPR_AQ_FOTO));
                    image.ScaleAbsolute(20, 20);
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
                if (filtro.CAMA_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CAMA_CD_ID.ToString();
                    ja = 1;
                }
                if (filtro.MAPR_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.MAPR_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.MAPR_NM_NOME;
                    }
                }
                if (filtro.MAPR_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.MAPR_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.MAPR_DS_DESCRICAO;
                    }
                }
                if (filtro.MAPR_CD_CODIGO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Código: " + filtro.MAPR_CD_CODIGO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Código: " + filtro.MAPR_CD_CODIGO;
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

            return RedirectToAction("MontarTelaMateria");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            // Prepara geração
            MATERIA_PRIMA aten = matApp.GetItemById(SessionMocks.idVolta);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Insumo_" + aten.MAPR_CD_ID.ToString() + "_" + data + ".pdf";
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

            cell = new PdfPCell(new Paragraph("Insumo - Detalhes", meuFont2))
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

            if (System.IO.File.Exists(Server.MapPath(aten.MAPR_AQ_FOTO)))
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = Image.GetInstance(Server.MapPath(aten.MAPR_AQ_FOTO));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell();

            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_MATERIA.CAMA_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código: " + aten.MAPR_CD_CODIGO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Unidade: " + aten.UNIDADE.UNID_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.MAPR_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.MAPR_DT_VALIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Data de Validade: " + aten.MAPR_DT_VALIDADE.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data de Validade: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.MAPR_PERDA_PROCESSAMENTO != null)
            {
                cell = new PdfPCell(new Paragraph("Perda no Processo (%): " + aten.MAPR_PERDA_PROCESSAMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Perda no Processo (%): ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.MAPR_NR_FATOR_CONVERSAO != null)
            {
                cell = new PdfPCell(new Paragraph("Fator de Conversão:" + aten.MAPR_NR_FATOR_CONVERSAO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Fator de Conversão: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.MAPR_VL_CUSTO != null)
            {
                cell = new PdfPCell(new Paragraph("Preço de Custo (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.MAPR_PERDA_PROCESSAMENTO.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Preço de Custo (R$): ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Descrição: " + aten.MAPR_DS_DESCRICAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Estoque
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Estoque", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Estoque Atual: " + aten.MAPR_QN_ESTOQUE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Estoque Inicial: " + aten.MAPR_QN_QUANTIDADE_INICIAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Estoque Máximo: " + aten.MAPR_QN_ESTOQUE_MAXIMO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Estoque Mínimo: " + aten.MAPR_QN_QUANTIDADE_M, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Reserva de Estoque: " + aten.MAPR_QN_RESERVA_ESTOQUE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.MAPR_DT_ULTIMA_MOVIMENTACAO != null)
            {
                cell = new PdfPCell(new Paragraph("Último Movimento: " + aten.MAPR_DT_ULTIMA_MOVIMENTACAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Último Movimento: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" " , meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Lista de Fornecedores
            if (aten.MATERIA_PRIMA_FORNECEDOR.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (MATERIA_PRIMA_FORNECEDOR item in aten.MATERIA_PRIMA_FORNECEDOR)
                {
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_TELEFONES, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.MAFO_IN_ATIVO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Inativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.MAPR_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Movimentações 
            if (aten.MOVIMENTO_ESTOQUE_MATERIA_PRIMA.Count > 0)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Movimentações de Estoque (Mais recentes)", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Movimentos
                table = new PdfPTable(new float[] { 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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

                foreach (MOVIMENTO_ESTOQUE_MATERIA_PRIMA item in aten.MOVIMENTO_ESTOQUE_MATERIA_PRIMA.OrderByDescending(a => a.MOEM_DT_MOVIMENTO).Take(10))
                {
                    cell = new PdfPCell(new Paragraph(item.MOEM_DT_MOVIMENTO.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.MOEM_IN_TIPO_MOVIMENTO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Entrada", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Saída", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    cell = new PdfPCell(new Paragraph(item.MOEM_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("VoltarAnexoMateria");
        }

        [HttpPost]
        public ActionResult IncluirTabelaMateriaPrima(MateriaPrimaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                Int32 idAss = SessionMocks.IdAssinante;
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                MATERIA_PRIMA item = Mapper.Map<MateriaPrimaViewModel, MATERIA_PRIMA>(vm);
                item.FILIAL = filApp.GetById(item.FILI_CD_ID);
                Int32 volta = matApp.IncluirMateriaPrimaPreco(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    //Session["MensProduto"] = 1;
                    Session["MensMateriaPrima"] = 1;
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
                    return RedirectToAction("VoltarAnexoMateria");
                }

                // Sucesso
                Session["MensMateriaPrima"] = 0;
                return RedirectToAction("VoltarAnexoMateria");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VoltarAnexoMateria");
            }
        }

        public void IncluirTabelaMateriaPrima(MateriaPrimaViewModel vm, String tabelaMateria)
        {
            foreach (var itemPreco in SessionMocks.listaPrecoMateria)
            {
                try
                {
                    // Executa a operação
                    Int32 idAss = SessionMocks.IdAssinante;
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    MATERIA_PRIMA item = Mapper.Map<MateriaPrimaViewModel, MATERIA_PRIMA>(vm);

                    item.FILI_CD_ID = itemPreco.FILI_CD_ID;
                    item.MPPR_VL_PRECO = itemPreco.MPPR_VL_PRECO;

                    Int32 volta = matApp.IncluirMateriaPrimaPreco(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            SessionMocks.listaPrecoMateria = null;
        }

        [HttpGet]
        public ActionResult ReativarTabelaMateriaPrima(Int32 id)
        {
            
            // Verifica se tem usuario logado
            Int32 idAss = SessionMocks.IdAssinante;
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            MATERIA_PRIMA_PRECO item = mppApp.GetItemById(id);
            item.MPPR_CD_ID = 1;
            Int32 volta = matApp.ValidateEditMateriaPrimaPreco(item);
            return RedirectToAction("VoltarAnexoMateria");
        }

        [HttpGet]
        public ActionResult ExcluirTabelaMateriaPrima(Int32 id)
        {
            
            // Verifica se tem usuario logado
            Int32 idAss = SessionMocks.IdAssinante;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;

            MATERIA_PRIMA rot = SessionMocks.materiaPrima;
            MATERIA_PRIMA_PRECO rl = mppApp.GetItemById(id);
            Int32 volta = mppApp.ValidateDelete(rl);
            return RedirectToAction("VoltarAnexoMateria");
        }

    }
}