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
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class TabelasAuxiliaresController : Controller
    {
        private readonly ICategoriaMateriaAppService cmApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly ILogAppService logApp;
        private readonly ISubcategoriaProdutoAppService spApp;
        private readonly ICategoriaClienteAppService clApp;
        private readonly ICategoriaFornecedorAppService foApp;
        private readonly ICategoriaContratoAppService ccApp;
        private readonly IFormaPagamentoAppService fpApp;
        private readonly IPeriodicidadeAppService peApp;
        private readonly ITipoContratoAppService tcApp;
        private readonly ITamanhoAppService taApp;
        private readonly ITipoPessoaAppService psApp;
        private readonly ICategoriaEquipamentoAppService ceApp;
        private readonly IRegimeTributarioAppService rtApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICargoAppService cargoApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IValorComissaoAppService vcApp;
        private readonly ICategoriaAtendimentoAppService caApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly ISubcategoriaMateriaAppService scmpApp;
        private readonly IGrupoAppService gruApp;
        private readonly ISubgrupoAppService subApp;
        private readonly ICategoriaServicoAppService csApp;

        private String msg;
        private Exception exception;
        CATEGORIA_PRODUTO objetoCP = new CATEGORIA_PRODUTO();
        CATEGORIA_PRODUTO objetoAntesCP = new CATEGORIA_PRODUTO();
        List<CATEGORIA_PRODUTO> listaMasterCP = new List<CATEGORIA_PRODUTO>();
        CATEGORIA_SERVICO objetoCS = new CATEGORIA_SERVICO();
        CATEGORIA_SERVICO objetoAntesCS = new CATEGORIA_SERVICO();
        List<CATEGORIA_SERVICO> listaMasterCS = new List<CATEGORIA_SERVICO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        CATEGORIA_MATERIA objetoCM = new CATEGORIA_MATERIA();
        CATEGORIA_MATERIA objetoAntesCM = new CATEGORIA_MATERIA();
        List<CATEGORIA_MATERIA> listaMasterCM = new List<CATEGORIA_MATERIA>();
        SUBCATEGORIA_PRODUTO objetoSP = new SUBCATEGORIA_PRODUTO();
        SUBCATEGORIA_PRODUTO objetoAntesSP = new SUBCATEGORIA_PRODUTO();
        List<SUBCATEGORIA_PRODUTO> listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
        SUBCATEGORIA_MATERIA objetoSM = new SUBCATEGORIA_MATERIA();
        SUBCATEGORIA_MATERIA objetoAntesSM = new SUBCATEGORIA_MATERIA();
        List<SUBCATEGORIA_MATERIA> listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
        CATEGORIA_CLIENTE objetoCC = new CATEGORIA_CLIENTE();
        CATEGORIA_CLIENTE objetoAntesCC = new CATEGORIA_CLIENTE();
        List<CATEGORIA_CLIENTE> listaMasterCC = new List<CATEGORIA_CLIENTE>();
        CATEGORIA_FORNECEDOR objetoCF = new CATEGORIA_FORNECEDOR();
        CATEGORIA_FORNECEDOR objetoAntesCF = new CATEGORIA_FORNECEDOR();
        List<CATEGORIA_FORNECEDOR> listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
        String extensao = String.Empty;
        CATEGORIA_CONTRATO objetoCT = new CATEGORIA_CONTRATO();
        CATEGORIA_CONTRATO objetoAntesCT = new CATEGORIA_CONTRATO();
        List<CATEGORIA_CONTRATO> listaMasterCT = new List<CATEGORIA_CONTRATO>();
        FORMA_PAGAMENTO objetoFP = new FORMA_PAGAMENTO();
        FORMA_PAGAMENTO objetoAntesFP = new FORMA_PAGAMENTO();
        List<FORMA_PAGAMENTO> listaMasterFP = new List<FORMA_PAGAMENTO>();
        PERIODICIDADE objetoPE = new PERIODICIDADE();
        PERIODICIDADE objetoAntesPE = new PERIODICIDADE();
        List<PERIODICIDADE> listaMastePE = new List<PERIODICIDADE>();
        SEXO objetoSX = new SEXO();
        SEXO objetoAntesSX = new SEXO();
        List<SEXO> listaMasteSX = new List<SEXO>();
        TIPO_CONTRATO objetoTC = new TIPO_CONTRATO();
        TIPO_CONTRATO objetoAntesTC = new TIPO_CONTRATO();
        List<TIPO_CONTRATO> listaMasteTC = new List<TIPO_CONTRATO>();
        TAMANHO objetoTA = new TAMANHO();
        TAMANHO objetoAntesTA = new TAMANHO();
        List<TAMANHO> listaMasteTA = new List<TAMANHO>();
        TIPO_PESSOA objetoPS = new TIPO_PESSOA();
        TIPO_PESSOA objetoAntesPS = new TIPO_PESSOA();
        List<TIPO_PESSOA> listaMastePS = new List<TIPO_PESSOA>();
        CATEGORIA_EQUIPAMENTO objetoCE = new CATEGORIA_EQUIPAMENTO();
        CATEGORIA_EQUIPAMENTO objetoAntesCE = new CATEGORIA_EQUIPAMENTO();
        List<CATEGORIA_EQUIPAMENTO> listaMasteCE = new List<CATEGORIA_EQUIPAMENTO>();
        REGIME_TRIBUTARIO objetoRT = new REGIME_TRIBUTARIO();
        REGIME_TRIBUTARIO objetoAntesRT = new REGIME_TRIBUTARIO();
        List<REGIME_TRIBUTARIO> listaMasteRT = new List<REGIME_TRIBUTARIO>();
        UNIDADE objetoUN = new UNIDADE();
        UNIDADE objetoAntesUN = new UNIDADE();
        List<UNIDADE> listaMasterUN = new List<UNIDADE>();
        CARGO objCargo = new CARGO();
        CARGO objAntesCargo = new CARGO();
        List<CARGO> listaMasterCargo = new List<CARGO>();
        CATEGORIA_ATENDIMENTO objetoCA = new CATEGORIA_ATENDIMENTO();
        CATEGORIA_ATENDIMENTO objetoCAAntes = new CATEGORIA_ATENDIMENTO();
        List<CATEGORIA_ATENDIMENTO> listaCAMaster = new List<CATEGORIA_ATENDIMENTO>();
        GRUPO objetoGru = new GRUPO();
        GRUPO objetoGruAntes = new GRUPO();
        List<GRUPO> listaMasterGru = new List<GRUPO>();
        SUBGRUPO objetoSub = new SUBGRUPO();
        SUBGRUPO objetoSubAntes = new SUBGRUPO();
        List<SUBGRUPO> listaMasterSub = new List<SUBGRUPO>();

        public TabelasAuxiliaresController(ICategoriaProdutoAppService cpApps, ILogAppService logApps, ICategoriaMateriaAppService cmApps, ISubcategoriaProdutoAppService spApps, ICategoriaClienteAppService clApps, ICategoriaFornecedorAppService foApps,ICategoriaContratoAppService ccApps, IFormaPagamentoAppService fpApps, IPeriodicidadeAppService peApps, ITipoContratoAppService tcApps, ITamanhoAppService taApps, ITipoPessoaAppService psApps, ICategoriaEquipamentoAppService ceApps, IRegimeTributarioAppService rtApps, IUnidadeAppService unApps, ICargoAppService carApps, IMatrizAppService matrizApps, IValorComissaoAppService vcApps, ICategoriaAtendimentoAppService caApps, IContaBancariaAppService cbApps, ISubcategoriaMateriaAppService scmpApps, IGrupoAppService gruApps, ISubgrupoAppService subApps, ICategoriaServicoAppService csApps)
        {
            cpApp = cpApps;
            logApp = logApps;
            cmApp = cmApps;
            spApp = spApps;
            clApp = clApps;
            foApp = foApps;
            ccApp = ccApps;
            fpApp = fpApps;
            peApp = peApps;
            tcApp = tcApps;
            taApp = taApps;
            psApp = psApps; 
            ceApp = ceApps;
            rtApp = rtApps;
            unApp = unApps;
            cargoApp = carApps;
            matrizApp = matrizApps;
            vcApp = vcApps;
            caApp = caApps;
            cbApp = cbApps;
            scmpApp = scmpApps;
            gruApp = gruApps;
            subApp = subApps;
            csApp = csApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            CATEGORIA_PRODUTO item = new CATEGORIA_PRODUTO();
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            
            listaMasterCP = new List<CATEGORIA_PRODUTO>();
            listaMasterCM = new List<CATEGORIA_MATERIA>();
            listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
            SessionMocks.listaCatProd = null;
            SessionMocks.listaCatMat = null;
            SessionMocks.listaSubCatProd = null;
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaCategoriaProduto()
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
            if (SessionMocks.listaCatProd == null)
            {
                listaMasterCP = cpApp.GetAllItens();
                SessionMocks.listaCatProd = listaMasterCP;
            }
            ViewBag.Listas = SessionMocks.listaCatProd;
            ViewBag.Title = "Categorias de Produtoxxx";

            // Indicadores
            ViewBag.Itens = listaMasterCP.Count;

            // Abre view
            objetoCP = new CATEGORIA_PRODUTO();
            return View(objetoCP);
        }

        public ActionResult MostrarTudoCategoriaProduto()
        {
            
            listaMasterCP = cpApp.GetAllItensAdm();
            SessionMocks.listaCatProd = listaMasterCP;
            return RedirectToAction("MontarTelaCategoriaProduto");
        }

        public ActionResult VoltarBaseCategoriaProduto()
        {
            
            SessionMocks.CatsProduto = cpApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaCategoriaProduto");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirProduto", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarProduto", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaCategoriaProduto");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaProduto(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_PRODUTO item = new CATEGORIA_PRODUTO();
            if (id != null)
            {
                SessionMocks.voltaPop = id.Value;
            }
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CAPR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaProduto(CategoriaProdutoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = cpApp.ValidateCreate(item, usuarioLogado);

                    if (SessionMocks.CategoriaToProduto && volta == 1)
                    {
                        ReativarCategoriaProduto((Int32)cpApp.GetAllItensAdm().Where(x => x.CAPR_NM_NOME == item.CAPR_NM_NOME).Select(x => x.CAPR_CD_ID).First());
                    }

                    // Sucesso
                    listaMasterCP = new List<CATEGORIA_PRODUTO>();
                    SessionMocks.listaCatProd = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaCategoriaProduto");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarProduto", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Fornecedor");
                    }

                    if (SessionMocks.CategoriaToProduto)
                    {
                        SessionMocks.CategoriaToProduto = false;
                        return RedirectToAction("IncluirProduto", "Produto");
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaCategoriaProduto");
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
        public ActionResult EditarCategoriaProduto(Int32 id)
        {
            
            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            objetoAntesCP = item;
            SessionMocks.catProduto = item;
            SessionMocks.idVolta = id;
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaProduto(CategoriaProdutoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    Int32 volta = cpApp.ValidateEdit(item, objetoAntesCP, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCP = new List<CATEGORIA_PRODUTO>();
                    SessionMocks.listaCatProd = null;
                    return RedirectToAction("MontarTelaCategoriaProduto");
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
        public ActionResult ExcluirCategoriaProduto(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            objetoAntesCP = SessionMocks.catProduto;
            item.CAPR_IN_ATIVO = 0;
            Int32 volta = cpApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterCP = new List<CATEGORIA_PRODUTO>();
            SessionMocks.listaCatProd = null;
            return RedirectToAction("MontarTelaCategoriaProduto");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaProduto(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            objetoAntesCP = SessionMocks.catProduto;
            item.CAPR_IN_ATIVO = 1;
            Int32 volta = cpApp.ValidateReativar(item, usu);
            listaMasterCP = new List<CATEGORIA_PRODUTO>();
            SessionMocks.listaCatProd = null;
            return RedirectToAction("MontarTelaCategoriaProduto");
        }

        #region CATEGORIA_SERVICO

        [HttpGet]
        public ActionResult MontarTelaCategoriaServico()
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
            if (SessionMocks.listaCatServ == null)
            {
                listaMasterCS = csApp.GetAllItens();
                SessionMocks.listaCatServ = listaMasterCS;
            }
            ViewBag.Listas = SessionMocks.listaCatServ;
            ViewBag.Title = "Categorias de Servicoxxx";

            // Indicadores
            ViewBag.Itens = listaMasterCS.Count;

            if (Session["MensCatServ"] != null)
            {
                if ((Int32)Session["MensCatServ"] == 1)
                {
                    Session["MensCatServ"] = 0;
                    ModelState.AddModelError("",SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoCS = new CATEGORIA_SERVICO();
            return View(objetoCS);
        }

        public ActionResult MostrarTudoCategoriaServico()
        {
            
            listaMasterCS = csApp.GetAllItensAdm();
            SessionMocks.listaCatServ = listaMasterCS;
            return RedirectToAction("MontarTelaCategoriaServico");
        }

        public ActionResult VoltarBaseCategoriaServico()
        {
            
            SessionMocks.listaCatServ = csApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaCategoriaServico");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirServico", "Servico");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarServiso", "Servico", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaCategoriaServico");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaServico(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_SERVICO item = new CATEGORIA_SERVICO();
            if (id != null)
            {
                SessionMocks.voltaPop = id.Value;
            }
            CategoriaServicoViewModel vm = Mapper.Map<CATEGORIA_SERVICO, CategoriaServicoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CASE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaServico(CategoriaServicoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_SERVICO item = Mapper.Map<CategoriaServicoViewModel, CATEGORIA_SERVICO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = csApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterCS = new List<CATEGORIA_SERVICO>();
                    SessionMocks.listaCatServ = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaCategoriaServico");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirServico", "Servico");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarServico", "Servico", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaCategoriaServico");
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
        public ActionResult EditarCategoriaServico(Int32 id)
        {
            
            CATEGORIA_SERVICO item = csApp.GetItemById(id);
            objetoAntesCS = item;
            SessionMocks.catServico = item;
            SessionMocks.idVolta = id;
            CategoriaServicoViewModel vm = Mapper.Map<CATEGORIA_SERVICO, CategoriaServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaServico(CategoriaServicoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_SERVICO item = Mapper.Map<CategoriaServicoViewModel, CATEGORIA_SERVICO>(vm);
                    Int32 volta = csApp.ValidateEdit(item, objetoAntesCS, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCS = new List<CATEGORIA_SERVICO>();
                    SessionMocks.listaCatServ = null;
                    return RedirectToAction("MontarTelaCategoriaServico");
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
        public ActionResult ExcluirCategoriaServico(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_SERVICO item = csApp.GetItemById(id);
            objetoAntesCS = SessionMocks.catServico;
            item.CASE_IN_ATIVO = 0;
            Int32 volta = csApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                Session["MensCatServ"] = 1;
            }
            listaMasterCS = new List<CATEGORIA_SERVICO>();
            SessionMocks.listaCatServ = null;
            return RedirectToAction("MontarTelaCategoriaServico");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaServico(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_SERVICO item = csApp.GetItemById(id);
            objetoAntesCS = SessionMocks.catServico;
            item.CASE_IN_ATIVO = 1;
            Int32 volta = csApp.ValidateReativar(item, usu);
            listaMasterCS = new List<CATEGORIA_SERVICO>();
            SessionMocks.listaCatServ = null;
            return RedirectToAction("MontarTelaCategoriaServico");
        }

        #endregion

        [HttpGet]
        public ActionResult MontarTelaCategoriaMateria()
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
            if (SessionMocks.listaCatMat == null)
            {
                listaMasterCM = cmApp.GetAllItens();
                SessionMocks.listaCatMat = listaMasterCM;
            }
            ViewBag.Listas = SessionMocks.listaCatMat;
            ViewBag.Title = "Categorias de Insumo";

            // Indicadores
            ViewBag.Itens = listaMasterCM.Count;

            // Abre view
            objetoCM = new CATEGORIA_MATERIA();
            return View(objetoCM);
        }

        public ActionResult MostrarTudoCategoriaMateria()
        {
            
            listaMasterCM = cmApp.GetAllItensAdm();
            SessionMocks.listaCatMat = listaMasterCM;
            return RedirectToAction("MontarTelaCategoriaMateria");
        }

        public ActionResult VoltarBaseCategoriaMateria()
        {
            
            SessionMocks.CatsInsumos = cmApp.GetAllItens();
            return RedirectToAction("MontarTelaCategoriaMateria");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaMateria()
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_MATERIA item = new CATEGORIA_MATERIA();
            CategoriaMateriaViewModel vm = Mapper.Map<CATEGORIA_MATERIA, CategoriaMateriaViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CAMA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaMateria(CategoriaMateriaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_MATERIA item = Mapper.Map<CategoriaMateriaViewModel, CATEGORIA_MATERIA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = cmApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterCM = new List<CATEGORIA_MATERIA>();
                    SessionMocks.listaCatMat = null;

                    if (SessionMocks.CategoriaToMateria)
                    {
                        SessionMocks.CategoriaToMateria = false;
                        return RedirectToAction("IncluirMateria", "Insumo");
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaCategoriaMateria");
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
        public ActionResult EditarCategoriaMateria(Int32 id)
        {
            
            CATEGORIA_MATERIA item = cmApp.GetItemById(id);
            objetoAntesCM = item;
            SessionMocks.catMateria = item;
            SessionMocks.idVolta = id;
            CategoriaMateriaViewModel vm = Mapper.Map<CATEGORIA_MATERIA, CategoriaMateriaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaMateria(CategoriaMateriaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_MATERIA item = Mapper.Map<CategoriaMateriaViewModel, CATEGORIA_MATERIA>(vm);
                    Int32 volta = cmApp.ValidateEdit(item, objetoAntesCM, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCM = new List<CATEGORIA_MATERIA>();
                    SessionMocks.listaCatMat = null;
                    return RedirectToAction("MontarTelaCategoriaMateria");
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
        public ActionResult ExcluirCategoriaMateria(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_MATERIA item = cmApp.GetItemById(id);
            objetoAntesCM = SessionMocks.catMateria;
            item.CAMA_IN_ATIVO = 0;
            Int32 volta = cmApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterCM = new List<CATEGORIA_MATERIA>();
            SessionMocks.listaCatMat = null;
            return RedirectToAction("MontarTelaCategoriaMateria");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaMateria(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_MATERIA item = cmApp.GetItemById(id);
            objetoAntesCM = SessionMocks.catMateria;
            item.CAMA_IN_ATIVO = 1;
            Int32 volta = cmApp.ValidateReativar(item, usu);
            listaMasterCM = new List<CATEGORIA_MATERIA>();
            SessionMocks.listaCatMat = null;
            return RedirectToAction("MontarTelaCategoriaMateria");
        }

        [HttpGet]
        public ActionResult MontarTelaSubCategoriaProduto()
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
            if (SessionMocks.listaSubCatProd == null)
            {
                listaMasterSP = spApp.GetAllItens();
                SessionMocks.listaSubCatProd = listaMasterSP;
            }
            ViewBag.Listas = SessionMocks.listaSubCatProd;
            ViewBag.Title = "Subcategorias de Produto";

            // Indicadores
            ViewBag.Itens = listaMasterSP.Count;

            // Abre view
            objetoSP = new SUBCATEGORIA_PRODUTO();
            return View(objetoSP);
        }

        public ActionResult MostrarTudoSubCategoriaProduto()
        {
            
            listaMasterSP = spApp.GetAllItensAdm();
            SessionMocks.listaSubCatProd = listaMasterSP;
            return RedirectToAction("MontarTelaSubCategoriaProduto");
        }

        public ActionResult VoltarBaseSubCategoriaProduto()
        {
            
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaSubCategoriaProduto");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirProduto", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarProduto", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaSubCategoriaProduto");
        }

        [HttpGet]
        public ActionResult IncluirSubCategoriaProduto(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            SUBCATEGORIA_PRODUTO item = new SUBCATEGORIA_PRODUTO();
            if (id != null)
            {
                SessionMocks.voltaPop = id.Value;
            }
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            ViewBag.Cats = new SelectList(cpApp.GetAllItens(), "CAPR_CD_ID", "CAPR_NM_NOME");
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.SCPR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirSubCategoriaProduto(SubCategoriaProdutoViewModel vm)
        {
            
            ViewBag.Cats = new SelectList(cpApp.GetAllItens(), "CAPR_CD_ID", "CAPR_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = spApp.ValidateCreate(item, usuarioLogado);

                    if (SessionMocks.SubCategoriaToProduto && volta == 1)
                    {
                        ReativarSubCategoriaProduto((Int32)spApp.GetAllItensAdm().Where(x => x.SCPR_NM_NOME == item.SCPR_NM_NOME).Select(x => x.SCPR_CD_ID).First());
                    }

                    // Sucesso
                    listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
                    SessionMocks.listaSubCatProd = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaSubCategoriaProduto");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarProduto", "Cadastros", new { id = SessionMocks.idVolta });
                    }

                    if (SessionMocks.SubCategoriaToProduto)
                    {
                        SessionMocks.SubCategoriaToProduto = false;
                        return RedirectToAction("IncluirProduto", "Produto");
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaSubCategoriaProduto");
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
        public ActionResult EditarSubCategoriaProduto(Int32 id)
        {
            
            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoAntesSP = item;
            SessionMocks.subCatProduto = item;
            SessionMocks.idVolta = id;
            ViewBag.Cats = new SelectList(cpApp.GetAllItens(), "CAPR_CD_ID", "CAPR_NM_NOME");
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarSubCategoriaProduto(SubCategoriaProdutoViewModel vm)
        {
            
            ViewBag.Cats = new SelectList(cpApp.GetAllItens(), "CAPR_CD_ID", "CAPR_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    Int32 volta = spApp.ValidateEdit(item, objetoAntesSP, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
                    SessionMocks.listaSubCatProd = null;
                    return RedirectToAction("MontarTelaSubCategoriaProduto");
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
        public ActionResult ExcluirSubCategoriaProduto(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoAntesSP = SessionMocks.subCatProduto;
            item.SCPR_IN_ATIVO = 0;
            Int32 volta = spApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
            SessionMocks.listaSubCatProd = null;
            return RedirectToAction("MontarTelaSubCategoriaProduto");
        }

        [HttpGet]
        public ActionResult ReativarSubCategoriaProduto(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoAntesSP = SessionMocks.subCatProduto;
            item.SCPR_IN_ATIVO = 1;
            Int32 volta = spApp.ValidateReativar(item, usu);
            listaMasterSP = new List<SUBCATEGORIA_PRODUTO>();
            SessionMocks.listaSubCatProd = null;
            return RedirectToAction("MontarTelaSubCategoriaProduto");
        }

        [HttpGet]
        public ActionResult MontarTelaSubCategoriaMateria()
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
            if (SessionMocks.listaSubCatMat == null)
            {
                listaMasterSM = scmpApp.GetAllItens();
                SessionMocks.listaSubCatMat = listaMasterSM;
            }
            ViewBag.Listas = SessionMocks.listaSubCatMat;
            ViewBag.Title = "Subcategorias de Materia";

            // Indicadores
            ViewBag.Itens = listaMasterSM.Count;

            // Abre view
            objetoSM = new SUBCATEGORIA_MATERIA();
            return View(objetoSM);
        }

        public ActionResult MostrarTudoSubCategoriaMateria()
        {
            
            listaMasterSM = scmpApp.GetAllItensAdm();
            SessionMocks.listaSubCatMat = listaMasterSM;
            return RedirectToAction("MontarTelaSubCategoriaMateria");
        }

        public ActionResult VoltarBaseSubCategoriaMateria()
        {
            
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaSubCategoriaMateria");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirMateria", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarMateria", "Cadastros", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.SubCategoriaToMateria)
            {
                return RedirectToAction("IncluirMateria", "Insumo");
            }
            return RedirectToAction("MontarTelaSubCategoriaMateria");
        }

        [HttpGet]
        public ActionResult IncluirSubCategoriaMateria(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            SUBCATEGORIA_MATERIA item = new SUBCATEGORIA_MATERIA();
            if (id != null)
            {
                SessionMocks.voltaPop = id.Value;
            }
            SubCategoriaMateriaViewModel vm = Mapper.Map<SUBCATEGORIA_MATERIA, SubCategoriaMateriaViewModel>(item);
            ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.SCMP_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirSubCategoriaMateria(SubCategoriaMateriaViewModel vm)
        {
            
            var categorias = cmApp.GetById(vm.CAMA_CD_ID);

            if (categorias.SUBCATEGORIA_MATERIA.Any(x => x.SCMP_NM_NOME == vm.SCMP_NM_NOME))
            {
                var idSubMat = scmpApp.GetAllItens().Where(x => x.SCMP_NM_NOME == vm.SCMP_NM_NOME).Select(x => x.SCMP_CD_ID);

                return RedirectToAction("MontarTelaSubCategoriaMateria");
                //return RedirectToAction("EditarSubCategoriaMateria", "TabelasAuxiliares", new { id = (Int32)idSubMat.First() });
            }
            

            ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SUBCATEGORIA_MATERIA item = Mapper.Map<SubCategoriaMateriaViewModel, SUBCATEGORIA_MATERIA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = scmpApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
                    SessionMocks.listaSubCatMat = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaSubCategoriaMateria");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirMateria", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarMateria", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.SubCategoriaToMateria)
                    {
                        SessionMocks.SubCategoriaToMateria = false;
                        return RedirectToAction("IncluirMateria", "Insumo");
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaSubCategoriaMateria");
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
        public ActionResult EditarSubCategoriaMateria(Int32 id)
        {
            
            SUBCATEGORIA_MATERIA item = scmpApp.GetItemById(id);
            objetoAntesSM = item;
            SessionMocks.subCatMateria = item;
            SessionMocks.idVolta = id;
            ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
            SubCategoriaMateriaViewModel vm = Mapper.Map<SUBCATEGORIA_MATERIA, SubCategoriaMateriaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarSubCategoriaMateria(SubCategoriaMateriaViewModel vm)
        {
            
            ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    SUBCATEGORIA_MATERIA item = Mapper.Map<SubCategoriaMateriaViewModel, SUBCATEGORIA_MATERIA>(vm);
                    Int32 volta = scmpApp.ValidateEdit(item, objetoAntesSM, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
                    SessionMocks.listaSubCatMat = null;
                    return RedirectToAction("MontarTelaSubCategoriaMateria");
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
        public ActionResult ExcluirSubCategoriaMateria(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            SUBCATEGORIA_MATERIA item = scmpApp.GetItemById(id);
            objetoAntesSM = SessionMocks.subCatMateria;
            Int32 volta = scmpApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
            SessionMocks.listaSubCatMat = null;
            return RedirectToAction("MontarTelaSubCategoriaMateria");
        }

        [HttpGet]
        public ActionResult ReativarSubCategoriaMateria(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            SUBCATEGORIA_MATERIA item = scmpApp.GetItemById(id);
            objetoAntesSM = SessionMocks.subCatMateria;
            item.SCMP_IN_ATIVO = 1;
            item.ASSINANTE = null;
            item.CATEGORIA_MATERIA = null;
            Int32 volta = scmpApp.ValidateReativar(item, usu);
            listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
            SessionMocks.listaSubCatMat = null;
            return RedirectToAction("MontarTelaSubCategoriaMateria");
        }

        //[HttpGet]
        //public ActionResult MontarTelaSubCategoriaMateria()
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if (SessionMocks.UserCredentials != null)
        //    {
        //        usuario = SessionMocks.UserCredentials;

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            return RedirectToAction("CarregarBase", "BaseAdmin");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }

        //    // Carrega listas
        //    if (SessionMocks.listaSubCatMat == null)
        //    {
        //        listaMasterSM = smApp.GetAllItens();
        //        SessionMocks.listaSubCatMat = listaMasterSM;
        //    }
        //    ViewBag.Listas = SessionMocks.listaSubCatMat;
        //    ViewBag.Title = "Subcategorias de Insumos";

        //    // Indicadores
        //    ViewBag.Itens = listaMasterSM.Count;

        //    // Abre view
        //    objetoSM = new SUBCATEGORIA_MATERIA();
        //    return View(objetoSM);
        //}

        //public ActionResult MostrarTudoSubCategoriaMateria()
        //{
        //    listaMasterSM = smApp.GetAllItensAdm();
        //    SessionMocks.listaSubCatMat = listaMasterSM;
        //    return RedirectToAction("MontarTelaSubCategoriaMateria");
        //}

        //public ActionResult VoltarBaseSubCategoriaMateria()
        //{
        //    return RedirectToAction("MontarTelaSubCategoriaMateria");
        //}

        //[HttpGet]
        //public ActionResult IncluirSubCategoriaMateria()
        //{
        //    // Prepara view
        //    USUARIO usuario = SessionMocks.UserCredentials;
        //    SUBCATEGORIA_MATERIA item = new SUBCATEGORIA_MATERIA();
        //    SubCategoriaMateriaViewModel vm = Mapper.Map<SUBCATEGORIA_MATERIA, SubCategoriaMateriaViewModel>(item);
        //    ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
        //    vm.ASSI_CD_ID = SessionMocks.IdAssinante.Value;
        //    vm.SCMA_IN_ATIVO = 1;
        //    return View(vm);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult IncluirSubCategoriaMateria(SubCategoriaMateriaViewModel vm)
        //{
        //    ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Executa a operação
        //            SUBCATEGORIA_MATERIA item = Mapper.Map<SubCategoriaMateriaViewModel, SUBCATEGORIA_MATERIA>(vm);
        //            USUARIO usuarioLogado = SessionMocks.UserCredentials;
        //            Int32 volta = smApp.ValidateCreate(item, usuarioLogado);

        //            // Sucesso
        //            listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
        //            SessionMocks.listaSubCatMat = null;
        //            return RedirectToAction("MontarTelaSubCategoriaMateria");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            return View(vm);
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //[HttpGet]
        //public ActionResult EditarSubCategoriaMateria(Int32 id)
        //{
        //    SUBCATEGORIA_MATERIA item = smApp.GetItemById(id);
        //    objetoAntesSM = item;
        //    SessionMocks.subCatMateria = item;
        //    SessionMocks.idVolta = id;
        //    ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
        //    SubCategoriaMateriaViewModel vm = Mapper.Map<SUBCATEGORIA_MATERIA, SubCategoriaMateriaViewModel>(item);
        //    return View(vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditarSubCategoriaMateria(SubCategoriaMateriaViewModel vm)
        //{
        //    ViewBag.Cats = new SelectList(cmApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Executa a operação
        //            USUARIO usuarioLogado = SessionMocks.UserCredentials;
        //            SUBCATEGORIA_MATERIA item = Mapper.Map<SubCategoriaMateriaViewModel, SUBCATEGORIA_MATERIA>(vm);
        //            Int32 volta = smApp.ValidateEdit(item, objetoAntesSM, usuarioLogado);

        //            // Verifica retorno

        //            // Sucesso
        //            listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
        //            SessionMocks.listaSubCatMat = null;
        //            return RedirectToAction("MontarTelaSubCategoriaMateria");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            return View(vm);
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //[HttpGet]
        //public ActionResult ExcluirSubCategoriaMateria(Int32 id)
        //{
        //    USUARIO usu = SessionMocks.UserCredentials;
        //    SUBCATEGORIA_MATERIA item = smApp.GetItemById(id);
        //    objetoAntesSM = SessionMocks.subCatMateria;
        //    item.SCMA_IN_ATIVO = 0;
        //    Int32 volta = smApp.ValidateDelete(item, usu);
        //    if (volta == 1)
        //    {
        //        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
        //    }
        //    listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
        //    SessionMocks.listaSubCatMat = null;
        //    return RedirectToAction("MontarTelaSubCategoriaMateria");
        //}

        //[HttpGet]
        //public ActionResult ReativarSubCategoriaMateria(Int32 id)
        //{
        //    USUARIO usu = SessionMocks.UserCredentials;
        //    SUBCATEGORIA_MATERIA item = smApp.GetItemById(id);
        //    objetoAntesSM = SessionMocks.subCatMateria;
        //    item.SCMA_IN_ATIVO = 1;
        //    Int32 volta = smApp.ValidateReativar(item, usu);
        //    listaMasterSM = new List<SUBCATEGORIA_MATERIA>();
        //    SessionMocks.listaSubCatMat = null;
        //    return RedirectToAction("MontarTelaSubCategoriaMateria");
        //}

        [HttpGet]
        public ActionResult MontarTelaCategoriaCliente()
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
            if (SessionMocks.listaCatCli == null)
            {
                listaMasterCC = clApp.GetAllItens();
                SessionMocks.listaCatCli = listaMasterCC;
            }
            ViewBag.Listas = SessionMocks.listaCatCli;
            ViewBag.Title = "Categorias de Clientes";

            // Indicadores
            ViewBag.Itens = listaMasterCC.Count;

            // Abre view
            objetoCC = new CATEGORIA_CLIENTE();
            return View(objetoCC);
        }

        public ActionResult MostrarTudoCategoriaCliente()
        {
            
            listaMasterCC = clApp.GetAllItensAdm();
            SessionMocks.listaCatCli = listaMasterCC;
            return RedirectToAction("MontarTelaCategoriaCliente");
        }

        public ActionResult VoltarBaseCategoriaCliente()
        {
            SessionMocks.CatsClientes = clApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaCategoriaCliente");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirCliente", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarCliente", "Cadastros");
            }
            return RedirectToAction("MontarTelaCategoriaCliente");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaCliente(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_CLIENTE item = new CATEGORIA_CLIENTE();
            SessionMocks.voltaPop = id.Value;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CACL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaCliente(CategoriaClienteViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = clApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterCC = new List<CATEGORIA_CLIENTE>();
                    SessionMocks.listaCatCli = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaCategoriaCliente");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirCliente", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarCliente", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaCategoriaCliente");
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
        public ActionResult EditarCategoriaCliente(Int32 id)
        {
            
            CATEGORIA_CLIENTE item = clApp.GetItemById(id);
            objetoAntesCC = item;
            SessionMocks.catCliente = item;
            SessionMocks.idVolta = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaCliente(CategoriaClienteViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    Int32 volta = clApp.ValidateEdit(item, objetoAntesCC, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCC = new List<CATEGORIA_CLIENTE>();
                    SessionMocks.listaCatCli = null;
                    return RedirectToAction("MontarTelaCategoriaCliente");
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
        public ActionResult ExcluirCategoriaCliente(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_CLIENTE item = clApp.GetItemById(id);
            objetoAntesCC = SessionMocks.catCliente;
            item.CACL_IN_ATIVO = 0;
            Int32 volta = clApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterCC = new List<CATEGORIA_CLIENTE>();
            SessionMocks.listaCatCli = null;
            return RedirectToAction("MontarTelaCategoriaCliente");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaCliente(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_CLIENTE item = clApp.GetItemById(id);
            objetoAntesCC = SessionMocks.catCliente;
            item.CACL_IN_ATIVO = 1;
            Int32 volta = clApp.ValidateReativar(item, usu);
            listaMasterCC = new List<CATEGORIA_CLIENTE>();
            SessionMocks.listaCatCli = null;
            return RedirectToAction("MontarTelaCategoriaCliente");
        }

        [HttpGet]
        public ActionResult MontarTelaCategoriaFornecedor()
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
            if (SessionMocks.listaCatFor == null)
            {
                listaMasterCF = foApp.GetAllItens();
                SessionMocks.listaCatFor = listaMasterCF;
            }
            ViewBag.Listas = SessionMocks.listaCatFor;
            ViewBag.Title = "Categorias de Fornecedores";

            // Indicadores
            ViewBag.Itens = listaMasterCF.Count;

            // Abre view
            objetoCF = new CATEGORIA_FORNECEDOR();
            return View(objetoCF);
        }

        public ActionResult MostrarTudoCategoriaFornecedor()
        {
            
            listaMasterCF = foApp.GetAllItensAdm();
            SessionMocks.listaCatFor = listaMasterCF;
            return RedirectToAction("MontarTelaCategoriaFornecedor");
        }

        public ActionResult VoltarBaseCategoriaFornecedor()
        {
            
            SessionMocks.CatsForns = foApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaCategoriaFornecedor");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirFornecedor", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarFornecedor", "Cadastros", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.voltaPop == 4)
            {
                return RedirectToAction("IncluirFornecedor", "Fornecedor");
            }
            return RedirectToAction("MontarTelaCategoriaFornecedor");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaFornecedor()
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_FORNECEDOR item = new CATEGORIA_FORNECEDOR();
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CAFO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaFornecedor(CategoriaFornecedorViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = foApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
                    SessionMocks.listaCatFor = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaCategoriaFornecedor");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirFornecedor", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarFornecedor", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 4)
                    {
                        return RedirectToAction("IncluirFornecedor", "Fornecedor");
                    }
                    return RedirectToAction("MontarTelaCategoriaFornecedor");

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
        public ActionResult EditarCategoriaFornecedor(Int32 id)
        {
            
            CATEGORIA_FORNECEDOR item = foApp.GetItemById(id);
            objetoAntesCF = item;
            SessionMocks.catFornecedor = item;
            SessionMocks.idVolta = id;
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaFornecedor(CategoriaFornecedorViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    Int32 volta = foApp.ValidateEdit(item, objetoAntesCF, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
                    SessionMocks.listaCatFor = null;
                    return RedirectToAction("MontarTelaCategoriaFornecedor");
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
        public ActionResult ExcluirCategoriaFornecedor(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_FORNECEDOR item = foApp.GetItemById(id);
            objetoAntesCF = SessionMocks.catFornecedor;
            item.CAFO_IN_ATIVO = 0;
            Int32 volta = foApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
            SessionMocks.listaCatFor = null;
            return RedirectToAction("MontarTelaCategoriaFornecedor");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaFornecedor(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_FORNECEDOR item = foApp.GetItemById(id);
            objetoAntesCF = SessionMocks.catFornecedor;
            item.CAFO_IN_ATIVO = 1;
            Int32 volta = foApp.ValidateReativar(item, usu);
            listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
            SessionMocks.listaCatFor = null;
            return RedirectToAction("MontarTelaCategoriaFornecedor");
        }

        [HttpGet]
        public ActionResult MontarTelaCategoriaContrato()
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
            if (SessionMocks.listaCatCont == null)
            {
                listaMasterCT = ccApp.GetAllItens();
                SessionMocks.listaCatCont = listaMasterCT;
            }
            ViewBag.Listas = SessionMocks.listaCatCont;
            ViewBag.Title = "Categorias de Contratos";

            // Indicadores
            ViewBag.Itens = listaMasterCT.Count;

            // Abre view
            objetoCT = new CATEGORIA_CONTRATO();
            return View(objetoCT);
        }

        public ActionResult MostrarTudoCategoriaContrato()
        {
            
            listaMasterCT = ccApp.GetAllItensAdm();
            SessionMocks.listaCatCont = listaMasterCT;
            return RedirectToAction("MontarTelaCategoriaContrato");
        }

        public ActionResult VoltarBaseCategoriaContrato()
        {
            
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaCategoriaContrato");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirContrato", "GestaoComercial");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaCategoriaContrato");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaContrato(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_CONTRATO item = new CATEGORIA_CONTRATO();
            SessionMocks.voltaPop = id.Value;
            CategoriaContratoViewModel vm = Mapper.Map<CATEGORIA_CONTRATO, CategoriaContratoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CACT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaContrato(CategoriaContratoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_CONTRATO item = Mapper.Map<CategoriaContratoViewModel, CATEGORIA_CONTRATO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = ccApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterCT = new List<CATEGORIA_CONTRATO>();
                    SessionMocks.listaCatCont = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaCategoriaContrato");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirContrato", "GestaoComercial");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaCategoriaContrato");
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
        public ActionResult EditarCategoriaContrato(Int32 id)
        {
            
            CATEGORIA_CONTRATO item = ccApp.GetItemById(id);
            objetoAntesCT = item;
            SessionMocks.catContrato = item;
            SessionMocks.idVolta = id;
            CategoriaContratoViewModel vm = Mapper.Map<CATEGORIA_CONTRATO, CategoriaContratoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaContrato(CategoriaContratoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_CONTRATO item = Mapper.Map<CategoriaContratoViewModel, CATEGORIA_CONTRATO>(vm);
                    Int32 volta = ccApp.ValidateEdit(item, objetoAntesCT, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCT = new List<CATEGORIA_CONTRATO>();
                    SessionMocks.listaCatCont = null;
                    return RedirectToAction("MontarTelaCategoriaContrato");
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
        public ActionResult ExcluirCategoriaContrato(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_CONTRATO item = ccApp.GetItemById(id);
            objetoAntesCT = SessionMocks.catContrato;
            item.CACT_IN_ATIVO = 0;
            Int32 volta = ccApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterCT = new List<CATEGORIA_CONTRATO>();
            SessionMocks.listaCatCont = null;
            return RedirectToAction("MontarTelaCategoriaContrato");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaContrato(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_CONTRATO item = ccApp.GetItemById(id);
            objetoAntesCT = SessionMocks.catContrato;
            item.CACT_IN_ATIVO = 1;
            Int32 volta = ccApp.ValidateReativar(item, usu);
            listaMasterCT = new List<CATEGORIA_CONTRATO>();
            SessionMocks.listaCatCont = null;
            return RedirectToAction("MontarTelaCategoriaContrato");
        }

        [HttpGet]
        public ActionResult MontarTelaFormaPagamento()
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

            // Carrega listas
            if (SessionMocks.listaForma == null)
            {
                listaMasterFP = fpApp.GetAllItens();
                SessionMocks.listaForma = listaMasterFP;
            }
            ViewBag.Listas = SessionMocks.listaForma;
            ViewBag.Title = "Formas de Pagamento";
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");

            // Indicadores
            ViewBag.Itens = listaMasterFP.Count;

            // Abre view
            objetoFP = new FORMA_PAGAMENTO();
            return View(objetoFP);
        }

        public ActionResult MostrarTudoFormaPagamento()
        {
            
            listaMasterFP = fpApp.GetAllItensAdm();
            SessionMocks.listaForma = listaMasterFP;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        public ActionResult VoltarBaseFormaPagamento()
        {
            
            SessionMocks.Formas = fpApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaFormaPagamento");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirContrato", "GestaoComercial");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        [HttpGet]
        public ActionResult IncluirFormaPagamento(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            FORMA_PAGAMENTO item = new FORMA_PAGAMENTO();
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            SessionMocks.voltaPop = id.Value;
            FormaPagamentoViewModel vm = Mapper.Map<FORMA_PAGAMENTO, FormaPagamentoViewModel>(item);
            vm.FOPA_IN_ATIVO = 1;
            vm.FOPA_IN_CHEQUE = 0;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirFormaPagamento(FormaPagamentoViewModel vm)
        {
            
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMA_PAGAMENTO item = Mapper.Map<FormaPagamentoViewModel, FORMA_PAGAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = fpApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterFP = new List<FORMA_PAGAMENTO>();
                    SessionMocks.listaForma = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaFormaPagamento");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirContrato", "GestaoComercial");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaFormaPagamento");
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
        public ActionResult EditarFormaPagamento(Int32 id)
        {
            
            FORMA_PAGAMENTO item = fpApp.GetItemById(id);
            objetoAntesFP = item;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            SessionMocks.forma = item;
            SessionMocks.idVolta = id;
            FormaPagamentoViewModel vm = Mapper.Map<FORMA_PAGAMENTO, FormaPagamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFormaPagamento(FormaPagamentoViewModel vm)
        {
            
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    FORMA_PAGAMENTO item = Mapper.Map<FormaPagamentoViewModel, FORMA_PAGAMENTO>(vm);
                    Int32 volta = fpApp.ValidateEdit(item, objetoAntesFP, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFP = new List<FORMA_PAGAMENTO>();
                    SessionMocks.listaForma = null;
                    return RedirectToAction("MontarTelaFormaPagamento");
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
        public ActionResult ExcluirFormaPagamento(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            FORMA_PAGAMENTO item = fpApp.GetItemById(id);
            objetoAntesFP = SessionMocks.forma;
            item.FOPA_IN_ATIVO = 0;
            Int32 volta = fpApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterFP = new List<FORMA_PAGAMENTO>();
            SessionMocks.listaForma = null;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        [HttpGet]
        public ActionResult ReativarFormaPagamento(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            FORMA_PAGAMENTO item = fpApp.GetItemById(id);
            objetoAntesFP = SessionMocks.forma;
            item.FOPA_IN_ATIVO = 1;
            Int32 volta = fpApp.ValidateReativar(item, usu);
            listaMasterFP = new List<FORMA_PAGAMENTO>();
            SessionMocks.listaForma = null;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        [HttpGet]
        public ActionResult MontarTelaPeriodicidade()
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
            if (SessionMocks.listaPeriod == null)
            {
                listaMastePE = peApp.GetAllItens();
                SessionMocks.listaPeriod = listaMastePE;
            }
            ViewBag.Listas = SessionMocks.listaPeriod;
            ViewBag.Title = "Periodicidades";

            // Indicadores
            ViewBag.Itens = listaMastePE.Count;

            // Abre view
            objetoPE = new PERIODICIDADE();
            return View(objetoPE);
        }

        public ActionResult MostrarTudoPeriodicidade()
        {
            
            listaMastePE = peApp.GetAllItensAdm();
            SessionMocks.listaPeriod = listaMastePE;
            return RedirectToAction("MontarTelaPeriodicidade");
        }

        public ActionResult VoltarBasePeriodicidade()
        {
            
            SessionMocks.Periodicidades = peApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaPeriodicidade");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirContrato", "GestaoComercial");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.voltaPop == 4)
            {
                return RedirectToAction("EditarEquipamento", "Cadastros", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.voltaPop == 5)
            {
                return RedirectToAction("IncluirEquipamento", "Cadastros");
            }
            return RedirectToAction("MontarTelaPeriodicidade");
        }

        [HttpGet]
        public ActionResult IncluirPeriodicidade(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            PERIODICIDADE item = new PERIODICIDADE();
            SessionMocks.voltaPop = id.Value;
            PeriodicidadeViewModel vm = Mapper.Map<PERIODICIDADE, PeriodicidadeViewModel>(item);
            vm.PERI_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirPeriodicidade(PeriodicidadeViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PERIODICIDADE item = Mapper.Map<PeriodicidadeViewModel, PERIODICIDADE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = peApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMastePE = new List<PERIODICIDADE>();
                    SessionMocks.listaPeriod = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaPeriodicidade");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirContrato", "GestaoComercial");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 4)
                    {
                        return RedirectToAction("EditarEquipamento", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 5)
                    {
                        return RedirectToAction("IncluirEquipamento", "Cadastros");
                    }
                    return RedirectToAction("MontarTelaPeriodicidade");
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
        public ActionResult EditarPeriodicidade(Int32 id)
        {
            
            PERIODICIDADE item = peApp.GetItemById(id);
            objetoAntesPE = item;
            SessionMocks.period = item;
            SessionMocks.idVolta = id;
            PeriodicidadeViewModel vm = Mapper.Map<PERIODICIDADE, PeriodicidadeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPeriodicidade(PeriodicidadeViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    PERIODICIDADE item = Mapper.Map<PeriodicidadeViewModel, PERIODICIDADE>(vm);
                    Int32 volta = peApp.ValidateEdit(item, objetoAntesPE, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMastePE = new List<PERIODICIDADE>();
                    SessionMocks.listaPeriod = null;
                    return RedirectToAction("MontarTelaPeriodicidade");
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
        public ActionResult ExcluirPeriodicidade(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            PERIODICIDADE item = peApp.GetItemById(id);
            objetoAntesPE = SessionMocks.period;
            item.PERI_IN_ATIVO = 0;
            Int32 volta = peApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMastePE = new List<PERIODICIDADE>();
            SessionMocks.listaPeriod = null;
            return RedirectToAction("MontarTelaPeriodicidade");
        }

        [HttpGet]
        public ActionResult ReativarPeriodicidade(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            PERIODICIDADE item = peApp.GetItemById(id);
            objetoAntesPE = SessionMocks.period;
            item.PERI_IN_ATIVO = 1;
            Int32 volta = peApp.ValidateReativar(item, usu);
            listaMastePE = new List<PERIODICIDADE>();
            SessionMocks.listaPeriod = null;
            return RedirectToAction("MontarTelaPeriodicidade");
        }

        //[HttpGet]
        //public ActionResult MontarTelaSexo()
        //{
        //    // Verifica se tem usuario logado
        //    USUARIO usuario = new USUARIO();
        //    if (SessionMocks.UserCredentials != null)
        //    {
        //        usuario = SessionMocks.UserCredentials;

        //        // Verfifica permissão
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            return RedirectToAction("CarregarBase", "BaseAdmin");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "ControleAcesso");
        //    }

        //    // Carrega listas
        //    if (SessionMocks.listaSexo == null)
        //    {
        //        listaMasteSX = sxApp.GetAllItens();
        //        SessionMocks.listaSexo = listaMasteSX;
        //    }
        //    ViewBag.Listas = SessionMocks.listaSexo;
        //    ViewBag.Title = "Sexos";

        //    // Indicadores
        //    ViewBag.Itens = listaMasteSX.Count;

        //    // Abre view
        //    objetoSX = new SEXO();
        //    return View(objetoSX);
        //}

        //public ActionResult MostrarTudoSexo()
        //{
        //    listaMasteSX = sxApp.GetAllItensAdm();
        //    SessionMocks.listaSexo = listaMasteSX;
        //    return RedirectToAction("MontarTelaSexo");
        //}

        //public ActionResult VoltarBaseSexo()
        //{
        //    if (SessionMocks.voltaPop == 1)
        //    {
        //        return RedirectToAction("MontarTelaSexo");
        //    }
        //    if (SessionMocks.voltaPop == 2)
        //    {
        //        return RedirectToAction("IncluirCliente", "Cadastros");
        //    }
        //    if (SessionMocks.voltaPop == 3)
        //    {
        //        return RedirectToAction("EditarCliente", "Cadastros", new { id = SessionMocks.idVolta });
        //    }
        //    return RedirectToAction("MontarTelaSexo");
        //}

        //[HttpGet]
        //public ActionResult IncluirSexo(Int32? id)
        //{
        //    // Prepara view
        //    USUARIO usuario = SessionMocks.UserCredentials;
        //    SEXO item = new SEXO();
        //    SessionMocks.voltaPop = id.Value;
        //    SexoViewModel vm = Mapper.Map<SEXO, SexoViewModel>(item);
        //    vm.ASSI_CD_ID = SessionMocks.IdAssinante.Value;
        //    vm.SEXO_IN_ATIVO = 1;
        //    return View(vm);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult IncluirSexo(SexoViewModel vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Executa a operação
        //            SEXO item = Mapper.Map<SexoViewModel, SEXO>(vm);
        //            USUARIO usuarioLogado = SessionMocks.UserCredentials;
        //            Int32 volta = sxApp.ValidateCreate(item, usuarioLogado);

        //            // Sucesso
        //            listaMasteSX = new List<SEXO>();
        //            SessionMocks.listaSexo = null;
        //            if (SessionMocks.voltaPop == 1)
        //            {
        //                return RedirectToAction("MontarTelaSexo");
        //            }
        //            if (SessionMocks.voltaPop == 2)
        //            {
        //                return RedirectToAction("IncluirCliente", "Cadastros");
        //            }
        //            if (SessionMocks.voltaPop == 3)
        //            {
        //                return RedirectToAction("EditarCliente", "Cadastros", new { id = SessionMocks.idVolta });
        //            }
        //            return RedirectToAction("MontarTelaSexo");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            return View(vm);
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //[HttpGet]
        //public ActionResult EditarSexo(Int32 id)
        //{
        //    SEXO item = sxApp.GetItemById(id);
        //    objetoAntesSX = item;
        //    SessionMocks.sexo = item;
        //    SessionMocks.idVolta = id;
        //    SexoViewModel vm = Mapper.Map<SEXO, SexoViewModel>(item);
        //    return View(vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditarSexo(SexoViewModel vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Executa a operação
        //            USUARIO usuarioLogado = SessionMocks.UserCredentials;
        //            SEXO item = Mapper.Map<SexoViewModel, SEXO>(vm);
        //            Int32 volta = sxApp.ValidateEdit(item, objetoAntesSX, usuarioLogado);

        //            // Verifica retorno

        //            // Sucesso
        //            listaMasteSX = new List<SEXO>();
        //            SessionMocks.listaSexo = null;
        //            return RedirectToAction("MontarTelaSexo");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            return View(vm);
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //[HttpGet]
        //public ActionResult ExcluirSexo(Int32 id)
        //{
        //    USUARIO usu = SessionMocks.UserCredentials;
        //    SEXO item = sxApp.GetItemById(id);
        //    objetoAntesSX = SessionMocks.sexo;
        //    item.SEXO_IN_ATIVO = 0;
        //    Int32 volta = sxApp.ValidateDelete(item, usu);
        //    if (volta == 1)
        //    {
        //        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
        //    }
        //    listaMasteSX = new List<SEXO>();
        //    SessionMocks.listaSexo = null;
        //    return RedirectToAction("MontarTelaSexo");
        //}

        //[HttpGet]
        //public ActionResult ReativarSexo(Int32 id)
        //{
        //    USUARIO usu = SessionMocks.UserCredentials;
        //    SEXO item = sxApp.GetItemById(id);
        //    objetoAntesSX = SessionMocks.sexo;
        //    item.SEXO_IN_ATIVO = 1;
        //    Int32 volta = sxApp.ValidateReativar(item, usu);
        //    listaMasteSX = new List<SEXO>();
        //    SessionMocks.listaSexo = null;
        //    return RedirectToAction("MontarTelaSexo");
        //}

        [HttpGet]
        public ActionResult MontarTelaTipoContrato()
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
            if (SessionMocks.listaTipoCont == null)
            {
                listaMasteTC = tcApp.GetAllItens();
                SessionMocks.listaTipoCont = listaMasteTC;
            }
            ViewBag.Listas = SessionMocks.listaTipoCont;
            ViewBag.Title = "Tipos de Contrato";

            // Indicadores
            ViewBag.Itens = listaMasteTC.Count;

            // Abre view
            objetoTC = new TIPO_CONTRATO();
            return View(objetoTC);
        }

        public ActionResult MostrarTudoTipoContrato()
        {
            
            listaMasteTC = tcApp.GetAllItensAdm();
            SessionMocks.listaTipoCont = listaMasteTC;
            return RedirectToAction("MontarTelaTipoContrato");
        }

        public ActionResult VoltarBaseTipoContrato()
        {
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaTipoContrato");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirContrato", "GestaoComercial");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaTipoContrato");
        }

        [HttpGet]
        public ActionResult IncluirTipoContrato(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            TIPO_CONTRATO item = new TIPO_CONTRATO();
            SessionMocks.voltaPop = id.Value;
            TipoContratoViewModel vm = Mapper.Map<TIPO_CONTRATO, TipoContratoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.TICT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirTipoContrato(TipoContratoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TIPO_CONTRATO item = Mapper.Map<TipoContratoViewModel, TIPO_CONTRATO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = tcApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasteTC = new List<TIPO_CONTRATO>();
                    SessionMocks.listaTipoCont = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaTipoContrato");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirContrato", "GestaoComercial");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarContrato", "GestaoComercial", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaTipoContrato");
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
        public ActionResult EditarTipoContrato(Int32 id)
        {
            
            TIPO_CONTRATO item = tcApp.GetItemById(id);
            objetoAntesTC = item;
            SessionMocks.tipoContrato = item;
            SessionMocks.idVolta = id;
            TipoContratoViewModel vm = Mapper.Map<TIPO_CONTRATO, TipoContratoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTipoContrato(TipoContratoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    TIPO_CONTRATO item = Mapper.Map<TipoContratoViewModel, TIPO_CONTRATO>(vm);
                    Int32 volta = tcApp.ValidateEdit(item, objetoAntesTC, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasteTC = new List<TIPO_CONTRATO>();
                    SessionMocks.listaTipoCont = null;
                    return RedirectToAction("MontarTelaTipoContrato");
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
        public ActionResult ExcluirTipoContrato(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            TIPO_CONTRATO item = tcApp.GetItemById(id);
            objetoAntesTC = SessionMocks.tipoContrato;
            item.TICT_IN_ATIVO = 0;
            Int32 volta = tcApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasteTC = new List<TIPO_CONTRATO>();
            SessionMocks.listaTipoCont = null;
            return RedirectToAction("MontarTelaTipoContrato");
        }

        [HttpGet]
        public ActionResult ReativarTipoContrato(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            TIPO_CONTRATO item = tcApp.GetItemById(id);
            objetoAntesTC = SessionMocks.tipoContrato;
            item.TICT_IN_ATIVO = 1;
            Int32 volta = tcApp.ValidateReativar(item, usu);
            listaMasteTC = new List<TIPO_CONTRATO>();
            SessionMocks.listaTipoCont = null;
            return RedirectToAction("MontarTelaTipoContrato");
        }

       [HttpGet]
        public ActionResult MontarTelaTamanho()
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
            if (SessionMocks.listaTamanho == null)
            {
                listaMasteTA = taApp.GetAllItens();
                SessionMocks.listaTamanho = listaMasteTA;
            }
            ViewBag.Listas = SessionMocks.listaTamanho;
            ViewBag.Title = "Tamanhos";

            // Indicadores
            ViewBag.Itens = listaMasteTA.Count;

            // Abre view
            objetoTA = new TAMANHO();
            return View(objetoTA);
        }

        public ActionResult MostrarTudoTamanho()
        {
            
            listaMasteTA = taApp.GetAllItensAdm();
            SessionMocks.listaTamanho = listaMasteTA;
            return RedirectToAction("MontarTelaTamanho");
        }

        public ActionResult VoltarBaseTamanho()
        {
            
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaTamanho");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirProdutoGrade", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarProdutoGrade", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaTamanho");
        }

        [HttpGet]
        public ActionResult IncluirTamanho(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            TAMANHO item = new TAMANHO();
            SessionMocks.voltaPop = id.Value;
            TamanhoViewModel vm = Mapper.Map<TAMANHO, TamanhoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.TAMA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirTamanho(TamanhoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TAMANHO item = Mapper.Map<TamanhoViewModel, TAMANHO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = taApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasteTA = new List<TAMANHO>();
                    SessionMocks.listaTamanho = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaTamanho");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirProdutoGrade", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarProdutoGrade", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaTamanho");
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
        public ActionResult EditarTamanho(Int32 id)
        {
            
            TAMANHO item = taApp.GetItemById(id);
            objetoAntesTA = item;
            SessionMocks.tamanho = item;
            SessionMocks.idVolta = id;
            TamanhoViewModel vm = Mapper.Map<TAMANHO, TamanhoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTamanho(TamanhoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    TAMANHO item = Mapper.Map<TamanhoViewModel, TAMANHO>(vm);
                    Int32 volta = taApp.ValidateEdit(item, objetoAntesTA, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasteTA = new List<TAMANHO>();
                    SessionMocks.listaTamanho = null;
                    return RedirectToAction("MontarTelaTamanho");
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
        public ActionResult ExcluirTamanho(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            TAMANHO item = taApp.GetItemById(id);
            objetoAntesTA = SessionMocks.tamanho;
            item.TAMA_IN_ATIVO = 0;
            Int32 volta = taApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasteTA = new List<TAMANHO>();
            SessionMocks.listaTamanho = null;
            return RedirectToAction("MontarTelaTamanho");
        }

        [HttpGet]
        public ActionResult ReativarTamanho(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            TAMANHO item = taApp.GetItemById(id);
            objetoAntesTA = SessionMocks.tamanho;
            item.TAMA_IN_ATIVO = 1;
            Int32 volta = taApp.ValidateReativar(item, usu);
            listaMasteTA = new List<TAMANHO>();
            SessionMocks.listaTamanho = null;
            return RedirectToAction("MontarTelaTamanho");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoPessoa()
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
            if (SessionMocks.listaTipoPessoa == null)
            {
                listaMastePS = psApp.GetAllItens();
                SessionMocks.listaTipoPessoa = listaMastePS;
            }
            ViewBag.Listas = SessionMocks.listaTipoPessoa;
            ViewBag.Title = "Tipos de Pessoa";

            // Indicadores
            ViewBag.Itens = listaMastePS.Count;

            // Abre view
            objetoPS = new TIPO_PESSOA();
            return View(objetoPS);
        }

        public ActionResult MostrarTudoTipoPessoa()
        {
            
            listaMastePS = psApp.GetAllItensAdm();
            SessionMocks.listaTipoPessoa = listaMastePS;
            return RedirectToAction("MontarTelaTipoPessoa");
        }

        public ActionResult VoltarBaseTipoPessoa()
        {
            
            SessionMocks.TiposPessoas = psApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaTipoPessoa");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirCliente", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarCliente", "Cadastros", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.voltaPop == 4)
            {
                return RedirectToAction("IncluirFilial", "Banco");
            }
            if (SessionMocks.voltaPop == 5)
            {
                return RedirectToAction("EditarFilial", "Banco", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.voltaPop == 6)
            {
                return RedirectToAction("IncluirFornecedor", "Cadastros");
            }
            if (SessionMocks.voltaPop == 7)
            {
                return RedirectToAction("EditarFornecedor", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaTipoPessoa");
        }

        [HttpGet]
        public ActionResult IncluirTipoPessoa(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            TIPO_PESSOA item = new TIPO_PESSOA();
            SessionMocks.voltaPop = id.Value;
            TipoPessoaViewModel vm = Mapper.Map<TIPO_PESSOA, TipoPessoaViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.TIPE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirTipoPessoa(TipoPessoaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TIPO_PESSOA item = Mapper.Map<TipoPessoaViewModel, TIPO_PESSOA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = psApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMastePS = new List<TIPO_PESSOA>();
                    SessionMocks.listaTipoPessoa = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaTipoPessoa");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirCliente", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarCliente", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 4)
                    {
                        return RedirectToAction("IncluirFilial", "Banco");
                    }
                    if (SessionMocks.voltaPop == 5)
                    {
                        return RedirectToAction("EditarFilial", "Banco", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 6)
                    {
                        return RedirectToAction("IncluirFornecedor", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 7)
                    {
                        return RedirectToAction("EditarFornecedor", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaTipoPessoa");
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
        public ActionResult EditarTipoPessoa(Int32 id)
        {
            
            TIPO_PESSOA item = psApp.GetItemById(id);
            objetoAntesPS = item;
            SessionMocks.tipoPessoa = item;
            SessionMocks.idVolta = id;
            TipoPessoaViewModel vm = Mapper.Map<TIPO_PESSOA, TipoPessoaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTipoPessoa(TipoPessoaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    TIPO_PESSOA item = Mapper.Map<TipoPessoaViewModel, TIPO_PESSOA>(vm);
                    Int32 volta = psApp.ValidateEdit(item, objetoAntesPS, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMastePS = new List<TIPO_PESSOA>();
                    SessionMocks.listaTipoPessoa = null;
                    return RedirectToAction("MontarTelaTipoPessoa");
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
        public ActionResult ExcluirTipoPessoa(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            TIPO_PESSOA item = psApp.GetItemById(id);
            objetoAntesPS = SessionMocks.tipoPessoa;
            item.TIPE_IN_ATIVO = 0;
            Int32 volta = psApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMastePS = new List<TIPO_PESSOA>();
            SessionMocks.listaTipoPessoa = null;
            return RedirectToAction("MontarTelaTipoPessoa");
        }

        [HttpGet]
        public ActionResult ReativarTipoPessoa(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            TIPO_PESSOA item = psApp.GetItemById(id);
            objetoAntesPS = SessionMocks.tipoPessoa;
            item.TIPE_IN_ATIVO = 1;
            Int32 volta = psApp.ValidateReativar(item, usu);
            listaMastePS = new List<TIPO_PESSOA>();
            SessionMocks.listaTipoPessoa = null;
            return RedirectToAction("MontarTelaTipoPessoa");
        }

        [HttpGet]
        public ActionResult MontarTelaCategoriaEquipamento()
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
            if (SessionMocks.listaCatEquip == null)
            {
                listaMasteCE = ceApp.GetAllItens();
                SessionMocks.listaCatEquip = listaMasteCE;
            }
            ViewBag.Listas = SessionMocks.listaCatEquip;
            ViewBag.Title = "Categorias de Equipamentosxxx";

            // Indicadores
            ViewBag.Itens = listaMasteCE.Count;

            // Abre view
            objetoCE = new CATEGORIA_EQUIPAMENTO();
            return View(objetoCE);
        }

        public ActionResult MostrarTudoCategoriaEquipamento()
        {
            
            listaMasteCE = ceApp.GetAllItensAdm();
            SessionMocks.listaCatEquip = listaMasteCE;
            return RedirectToAction("MontarTelaCategoriaEquipamento");
        }

        public ActionResult VoltarBaseCategoriaEquipamento()
        {
            
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaCategoriaEquipamento");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirEquipamento", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarEquipamento", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaCategoriaEquipamento");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaEquipamento(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_EQUIPAMENTO item = new CATEGORIA_EQUIPAMENTO();
            if (id != null)
            {
                SessionMocks.voltaPop = id.Value;
            }
            CategoriaEquipamentoViewModel vm = Mapper.Map<CATEGORIA_EQUIPAMENTO, CategoriaEquipamentoViewModel>(item);
            vm.CAEQ_IN_ATIVO = 1;
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaEquipamento(CategoriaEquipamentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_EQUIPAMENTO item = Mapper.Map<CategoriaEquipamentoViewModel, CATEGORIA_EQUIPAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = ceApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasteCE = new List<CATEGORIA_EQUIPAMENTO>();
                    SessionMocks.listaCatEquip = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaCategoriaEquipamento");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirEquipamento", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarEquipamento", "Cadastros", new { id = SessionMocks.idVolta });
                    }

                    if (SessionMocks.CategoriaToEquipamento == 1)
                    {
                        SessionMocks.CategoriaToEquipamento = 0;
                        return RedirectToAction("IncluirEquipamento", "Equipamento");
                    }
                    else if (SessionMocks.CategoriaToEquipamento == 2)
                    {
                        SessionMocks.CategoriaToEquipamento = 0;
                        return RedirectToAction("IncluirEquipamento", "Equipamento", new { id = SessionMocks.idVolta });
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaCategoriaEquipamento");
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
        public ActionResult EditarCategoriaEquipamento(Int32 id)
        {
            
            CATEGORIA_EQUIPAMENTO item = ceApp.GetItemById(id);
            objetoAntesCE = item;
            SessionMocks.catEquip = item;
            SessionMocks.idVolta = id;
            CategoriaEquipamentoViewModel vm = Mapper.Map<CATEGORIA_EQUIPAMENTO, CategoriaEquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaEquipamento(CategoriaEquipamentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_EQUIPAMENTO item = Mapper.Map<CategoriaEquipamentoViewModel, CATEGORIA_EQUIPAMENTO>(vm);
                    Int32 volta = ceApp.ValidateEdit(item, objetoAntesCE, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasteCE = new List<CATEGORIA_EQUIPAMENTO>();
                    SessionMocks.listaCatEquip = null;
                    return RedirectToAction("MontarTelaCategoriaEquipamento");
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
        public ActionResult ExcluirCategoriaEquipamento(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_EQUIPAMENTO item = ceApp.GetItemById(id);
            objetoAntesCE = SessionMocks.catEquip;
            item.CAEQ_IN_ATIVO = 0;
            Int32 volta = ceApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasteCE = new List<CATEGORIA_EQUIPAMENTO>();
            SessionMocks.listaCatEquip = null;
            return RedirectToAction("MontarTelaCategoriaEquipamento");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaEquipamento(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CATEGORIA_EQUIPAMENTO item = ceApp.GetItemById(id);
            objetoAntesCE = SessionMocks.catEquip;
            item.CAEQ_IN_ATIVO = 1;
            Int32 volta = ceApp.ValidateReativar(item, usu);
            listaMasteCE = new List<CATEGORIA_EQUIPAMENTO>();
            SessionMocks.listaCatEquip = null;
            return RedirectToAction("MontarTelaCategoriaEquipamento");
        }

        [HttpGet]
        public ActionResult MontarTelaRegimeTributario()
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
            if (SessionMocks.listaRegTrib == null)
            {
                listaMasteRT = rtApp.GetAllItens();
                SessionMocks.listaRegTrib = listaMasteRT;
            }
            ViewBag.Listas = SessionMocks.listaRegTrib;
            ViewBag.Title = "Regimes Tributários";

            // Indicadores
            ViewBag.Itens = listaMasteRT.Count;

            // Abre view
            objetoRT = new REGIME_TRIBUTARIO();
            return View(objetoRT);
        }

        public ActionResult MostrarTudoRegimeTributario()
        {
            
            listaMasteRT = rtApp.GetAllItensAdm();
            SessionMocks.listaRegTrib = listaMasteRT;
            return RedirectToAction("MontarTelaRegimeTributario");
        }

        public ActionResult VoltarBaseRegimeTributario()
        {
            
            SessionMocks.Regimes = rtApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaRegimeTributario");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirCliente", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarCliente", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaRegimeTributario");
        }

        [HttpGet]
        public ActionResult IncluirRegimeTributario(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            REGIME_TRIBUTARIO item = new REGIME_TRIBUTARIO();
            SessionMocks.voltaPop = id.Value;
            RegimeTributarioViewModel vm = Mapper.Map<REGIME_TRIBUTARIO, RegimeTributarioViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.RETR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirRegimeTributario(RegimeTributarioViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    REGIME_TRIBUTARIO item = Mapper.Map<RegimeTributarioViewModel, REGIME_TRIBUTARIO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = rtApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasteRT = new List<REGIME_TRIBUTARIO>();
                    SessionMocks.listaRegTrib = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaRegimeTributario");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirCliente", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarCliente", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaRegimeTributario");
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
        public ActionResult EditarRegimeTributario(Int32 id)
        {
            
            REGIME_TRIBUTARIO item = rtApp.GetItemById(id);
            objetoAntesRT = item;
            SessionMocks.regTrib = item;
            SessionMocks.idVolta = id;
            RegimeTributarioViewModel vm = Mapper.Map<REGIME_TRIBUTARIO, RegimeTributarioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarRegimeTributario(RegimeTributarioViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    REGIME_TRIBUTARIO item = Mapper.Map<RegimeTributarioViewModel, REGIME_TRIBUTARIO>(vm);
                    Int32 volta = rtApp.ValidateEdit(item, objetoAntesRT, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasteRT = new List<REGIME_TRIBUTARIO>();
                    SessionMocks.listaRegTrib = null;
                    return RedirectToAction("MontarTelaRegimeTributario");
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
        public ActionResult ExcluirRegimeTributario(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            REGIME_TRIBUTARIO item = rtApp.GetItemById(id);
            objetoAntesRT = SessionMocks.regTrib;
            item.RETR_IN_ATIVO = 0;
            Int32 volta = rtApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasteRT = new List<REGIME_TRIBUTARIO>();
            SessionMocks.listaRegTrib = null;
            return RedirectToAction("MontarTelaRegimeTributario");
        }

        [HttpGet]
        public ActionResult ReativarRegimeTributario(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            REGIME_TRIBUTARIO item = rtApp.GetItemById(id);
            objetoAntesRT = SessionMocks.regTrib;
            item.RETR_IN_ATIVO = 1;
            Int32 volta = rtApp.ValidateReativar(item, usu);
            listaMasteRT = new List<REGIME_TRIBUTARIO>();
            SessionMocks.listaRegTrib = null;
            return RedirectToAction("MontarTelaRegimeTributario");
        }

        [HttpGet]
        public ActionResult MontarTelaUnidade()
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
            if (SessionMocks.listaUnidade == null)
            {
                listaMasterUN = unApp.GetAllItens();
                SessionMocks.listaUnidade = listaMasterUN;
            }
            ViewBag.Listas = SessionMocks.listaUnidade;
            ViewBag.Title = "Unidades";

            // Indicadores
            ViewBag.Itens = listaMasterUN.Count;

            // Abre view
            objetoUN = new UNIDADE();
            return View(objetoUN);
        }

        public ActionResult MostrarTudoUnidade()
        {
            
            listaMasterUN = unApp.GetAllItensAdm();
            SessionMocks.listaUnidade = listaMasterUN;
            return RedirectToAction("MontarTelaUnidade");
        }

        public ActionResult VoltarBaseUnidade()
        {
            
            SessionMocks.Unidades = unApp.GetAllItens();
            if (SessionMocks.voltaPop == 1)
            {
                return RedirectToAction("MontarTelaUnidade");
            }
            if (SessionMocks.voltaPop == 2)
            {
                return RedirectToAction("IncluirProduto", "Cadastros");
            }
            if (SessionMocks.voltaPop == 3)
            {
                return RedirectToAction("EditarPoduto", "Cadastros", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.voltaPop == 4)
            {
                return RedirectToAction("IncluirMateria", "Cadastros");
            }
            if (SessionMocks.voltaPop == 5)
            {
                return RedirectToAction("EditarMateria", "Cadastros", new { id = SessionMocks.idVolta });
            }
            if (SessionMocks.voltaPop == 6)
            {
                return RedirectToAction("IncluirServico", "Cadastros");
            }
            if (SessionMocks.voltaPop == 7)
            {
                return RedirectToAction("EditarServico", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult IncluirUnidade(Int32? id)
        {
            
            List<SelectListItem> tipoUnd = new List<SelectListItem>();
            tipoUnd.Add(new SelectListItem() { Text = "Serviço", Value = "1" });
            tipoUnd.Add(new SelectListItem() { Text = "Unidade de Medida", Value = "2" });
            ViewBag.TipoUnidade = new SelectList(tipoUnd, "Value", "Text");
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            UNIDADE item = new UNIDADE();
            SessionMocks.voltaPop = id.Value;
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.UNID_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirUnidade(UnidadeViewModel vm)
        {
            
            List<SelectListItem> tipoUnd = new List<SelectListItem>();
            tipoUnd.Add(new SelectListItem() { Text = "Serviço", Value = "1" });
            tipoUnd.Add(new SelectListItem() { Text = "Unidade de Medida", Value = "2" });
            ViewBag.TipoUnidade = new SelectList(tipoUnd, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = unApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterUN = new List<UNIDADE>();
                    SessionMocks.listaUnidade = null;
                    if (SessionMocks.voltaPop == 1)
                    {
                        return RedirectToAction("MontarTelaUnidade");
                    }
                    if (SessionMocks.voltaPop == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 3)
                    {
                        return RedirectToAction("EditarPoduto", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 4)
                    {
                        return RedirectToAction("IncluirMateria", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 5)
                    {
                        return RedirectToAction("EditarMateria", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    if (SessionMocks.voltaPop == 6)
                    {
                        return RedirectToAction("IncluirServico", "Cadastros");
                    }
                    if (SessionMocks.voltaPop == 7)
                    {
                        return RedirectToAction("EditarServico", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaUnidade");
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
        public ActionResult EditarUnidade(Int32 id)
        {
            
            List<SelectListItem> tipoUnd = new List<SelectListItem>();
            tipoUnd.Add(new SelectListItem() { Text = "Serviço", Value = "1" });
            tipoUnd.Add(new SelectListItem() { Text = "Unidade de Medida", Value = "2" });
            ViewBag.TipoUnidade = new SelectList(tipoUnd, "Value", "Text");
            UNIDADE item = unApp.GetItemById(id);
            objetoAntesUN = item;
            SessionMocks.unidade = item;
            SessionMocks.idVolta = id;
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUnidade(UnidadeViewModel vm)
        {
            
            List<SelectListItem> tipoUnd = new List<SelectListItem>();
            tipoUnd.Add(new SelectListItem() { Text = "Serviço", Value = "1" });
            tipoUnd.Add(new SelectListItem() { Text = "Unidade de Medida", Value = "2" });
            ViewBag.TipoUnidade = new SelectList(tipoUnd, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    Int32 volta = unApp.ValidateEdit(item, objetoAntesUN, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterUN = new List<UNIDADE>();
                    SessionMocks.listaUnidade = null;
                    return RedirectToAction("MontarTelaUnidade");
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
        public ActionResult ExcluirUnidade(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            UNIDADE item = unApp.GetItemById(id);
            objetoAntesUN = SessionMocks.unidade;
            item.UNID_IN_ATIVO = 0;
            Int32 volta = unApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
            }
            listaMasterUN = new List<UNIDADE>();
            SessionMocks.listaUnidade = null;
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult ReativarUnidade(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            UNIDADE item = unApp.GetItemById(id);
            objetoAntesUN = SessionMocks.unidade;
            item.UNID_IN_ATIVO = 1;
            Int32 volta = unApp.ValidateReativar(item, usu);
            listaMasterUN = new List<UNIDADE>();
            SessionMocks.listaUnidade = null;
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult MontarTelaCargo()
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
            if (SessionMocks.listaCargo == null || SessionMocks.listaCargo.Count == 0)
            {
                listaMasterCargo = cargoApp.GetAllItens();
                SessionMocks.listaCargo = listaMasterCargo;
            }
            ViewBag.Listas = SessionMocks.listaCargo;
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Valores = new SelectList(vcApp.GetAllItens(), "VACO_CD_ID", "VACO_NM_NOME");
            ViewBag.Title = "Cargos";

            // Indicadores
            ViewBag.Cargos = SessionMocks.listaCargo.Count;

            // Abre view
            objCargo = new CARGO();
            return View(objCargo);
        }

        public ActionResult RetirarFiltroCargo()
        {
            
            SessionMocks.listaCargo = null;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult MostrarTudoCargo()
        {
            
            listaMasterCargo = cargoApp.GetAllItensAdm();
            SessionMocks.listaCargo = listaMasterCargo;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult VoltarBaseCargo()
        {
            
            SessionMocks.Cargos = cargoApp.GetAllItens();
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult IncluirCargo()
        {
            
            // Prepara listas

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            ViewBag.Valores = new SelectList(vcApp.GetAllItens(), "VACO_CD_ID", "VACO_NM_NOME");
            CARGO item = new CARGO();
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CARG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCargo(CargoViewModel vm)
        {
            
            ViewBag.Valores = new SelectList(vcApp.GetAllItens(), "VACO_CD_ID", "VACO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = cargoApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0032", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterCargo = new List<CARGO>();
                    SessionMocks.listaCargo = null;
                    return RedirectToAction("MontarTelaCargo");
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
        public ActionResult EditarCargo(Int32 id)
        {
            
            // Prepara view
            CARGO item = cargoApp.GetItemById(id);
            ViewBag.Valores = new SelectList(vcApp.GetAllItens(), "VACO_CD_ID", "VACO_NM_NOME");
            objAntesCargo = item;
            SessionMocks.cargo = item;
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCargo(CargoViewModel vm)
        {
            
            ViewBag.Valores = new SelectList(vcApp.GetAllItens(), "VACO_CD_ID", "VACO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    Int32 volta = cargoApp.ValidateEdit(item, objAntesCargo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCargo = new List<CARGO>();
                    SessionMocks.listaCargo = null;
                    return RedirectToAction("MontarTelaCargo");
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
        public ActionResult ExcluirCargo(Int32 id)
        {
            
            // Prepara view
            CARGO item = cargoApp.GetItemById(id);
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirCargo(CargoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                Int32 volta = cargoApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0033", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMasterCargo = new List<CARGO>();
                SessionMocks.listaCargo = null;
                return RedirectToAction("MontarTelaCargo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarCargo(Int32 id)
        {
            
            // Prepara view
            CARGO item = cargoApp.GetItemById(id);
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarCargo(CargoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                Int32 volta = cargoApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterCargo = new List<CARGO>();
                SessionMocks.listaCargo = null;
                return RedirectToAction("MontarTelaCargo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaCategoriaAtendimento()
        {
            // Verifica se tem usuario logado e permissões
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture);
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaCatAten == null)
            {
                listaCAMaster = caApp.GetAllItens();
                SessionMocks.listaCatAten = listaCAMaster;
            }
            ViewBag.Listas = SessionMocks.listaCatAten;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Categorias de Atendimento";

            // Indicadores
            ViewBag.Cats = SessionMocks.listaCatAten.Count;

            // Abre view
            objetoCA = new CATEGORIA_ATENDIMENTO();
            return View(objetoCA);
        }

        public ActionResult MostrarTudoCategoriaAtendimento()
        {
            
            listaCAMaster = caApp.GetAllItensAdm();
            SessionMocks.listaCatAten = listaCAMaster;
            return RedirectToAction("MontarTelaCategoriaAtendimento");
        }

        public ActionResult VoltarBaseCategoriaAtendimento()
        {
            
            return RedirectToAction("MontarTelaCategoriaAtendimento");
        }

        [HttpGet]
        public ActionResult IncluirCategoriaAtendimento()
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA != "ADM" & usu.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture);
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CATEGORIA_ATENDIMENTO item = new CATEGORIA_ATENDIMENTO();
            CategoriaAtendimentoViewModel vm = Mapper.Map<CATEGORIA_ATENDIMENTO, CategoriaAtendimentoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CAAT_IN_ATIVO = 1;
            vm.CAAT_CD_ID = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCategoriaAtendimento(CategoriaAtendimentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_ATENDIMENTO item = Mapper.Map<CategoriaAtendimentoViewModel, CATEGORIA_ATENDIMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = caApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0030", CultureInfo.CurrentCulture);
                        return View(vm);
                    }

                    // Sucesso
                    listaCAMaster = new List<CATEGORIA_ATENDIMENTO>();
                    SessionMocks.listaCatAten = null;
                    return RedirectToAction("MontarTelaCategoriaAtendimento");
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
        public ActionResult EditarCategoriaAtendimento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA != "ADM" & usu.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture);
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            CATEGORIA_ATENDIMENTO item = caApp.GetItemById(id);
            objetoCAAntes = item;
            SessionMocks.catAten = item;
            SessionMocks.idVolta = id;
            CategoriaAtendimentoViewModel vm = Mapper.Map<CATEGORIA_ATENDIMENTO, CategoriaAtendimentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCategoriaAtendimento(CategoriaAtendimentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CATEGORIA_ATENDIMENTO item = Mapper.Map<CategoriaAtendimentoViewModel, CATEGORIA_ATENDIMENTO>(vm);
                    Int32 volta = caApp.ValidateEdit(item, objetoCAAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaCAMaster = new List<CATEGORIA_ATENDIMENTO>();
                    SessionMocks.listaCatAten = null;
                    return RedirectToAction("MontarTelaCategoriaAtendimento");
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
        public ActionResult ExcluirCategoriaAtendimento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA != "ADM" & usu.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture);
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Executar
            CATEGORIA_ATENDIMENTO item = caApp.GetItemById(id);
            CATEGORIA_ATENDIMENTO ex = new CATEGORIA_ATENDIMENTO();
            ex.CAAT_CD_ID = item.CAAT_CD_ID;
            ex.ASSI_CD_ID = item.ASSI_CD_ID;
            ex.CAAT_NM_NOME = item.CAAT_NM_NOME;
            ex.CAAT_IN_ATIVO = 0;
            ex.CAAT_IN_SLA = item.CAAT_IN_SLA;
            objetoCAAntes = SessionMocks.catAten;
            Int32 volta = caApp.ValidateDelete(ex, usu);
            if (volta == 1)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0059", CultureInfo.CurrentCulture);
            }
            listaCAMaster = new List<CATEGORIA_ATENDIMENTO>();
            SessionMocks.listaCatAten = null;
            return RedirectToAction("MontarTelaCategoriaAtendimento");
        }

        [HttpGet]
        public ActionResult ReativarCategoriaAtendimento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA != "ADM" & usu.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture);
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Executar
            CATEGORIA_ATENDIMENTO item = caApp.GetItemById(id);
            CATEGORIA_ATENDIMENTO ex = new CATEGORIA_ATENDIMENTO();
            ex.CAAT_CD_ID = item.CAAT_CD_ID;
            ex.ASSI_CD_ID = item.ASSI_CD_ID;
            ex.CAAT_NM_NOME = item.CAAT_NM_NOME;
            ex.CAAT_IN_ATIVO = 1;
            ex.CAAT_IN_SLA = item.CAAT_IN_SLA;
            objetoCAAntes = SessionMocks.catAten;
            Int32 volta = caApp.ValidateReativar(ex, usu);
            listaCAMaster = new List<CATEGORIA_ATENDIMENTO>();
            SessionMocks.listaCatAten = null;
            return RedirectToAction("MontarTelaCategoriaAtendimento");
        }

        [HttpGet]
        public ActionResult MontarTelaGrupo()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((List<GRUPO>)Session["ListaGrupo"] == null)
            {
                listaMasterGru = gruApp.GetAllItens();
                Session["ListaGrupo"] = listaMasterGru;
            }
            ViewBag.Listas = (List<GRUPO>)Session["ListaGrupo"];
            ViewBag.Title = "Grupos";

            // Indicadores
            ViewBag.Grupos = ((List<GRUPO>)Session["ListaGrupo"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagem
            if ((Int32)Session["MensGrupo"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensGrupo"] == 3)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0116", CultureInfo.CurrentCulture));
            }

            // Abre view
            objetoGru = new GRUPO();
            Session["VoltaGrupo"] = 1;
            Session["MensGrupo"] = 0;
            return View(objetoGru);
        }

        public ActionResult MostrarTudoGrupo()
        {
            
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterGru = gruApp.GetAllItensAdm();
            Session["ListaGrupo"] = listaMasterGru;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarBaseGrupo()
        {
            
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaGrupo"] = gruApp.GetAllItens();
            if ((Int32)Session["VoltaGrupo"] == 2)
            {
                return RedirectToAction("IncluirCC", "CentroCusto");
            }
            if ((Int32)Session["VoltaGrupo"] == 3)
            {
                return RedirectToAction("EditarCC", "CentroCusto");
            }
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult IncluirGrupo(Int32? id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            GRUPO item = new GRUPO();
            if (id != null)
            {
                Session["VoltaGrupo"] = id.Value;
            }
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.GRUP_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirGrupo(GrupoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    Int32 volta = gruApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterGru = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    return RedirectToAction("MontarTelaGrupo");
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
        public ActionResult EditarGrupo(Int32 id)
        {
            
            GRUPO item = gruApp.GetItemById(id);
            objetoGruAntes = item;
            Session["Grupo"] = item;
            Session["idVolta"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarGrupo(GrupoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    Int32 volta = gruApp.ValidateEdit(item, objetoGruAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterGru = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    return RedirectToAction("MontarTelaGrupo");
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
        public ActionResult ExcluirGrupo(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            GRUPO item = gruApp.GetItemById(id);
            objetoGruAntes = item;
            item.GRUP_IN_ATIVO = 0;
            Int32 volta = gruApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensGrupo"] = 3;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0118", CultureInfo.CurrentCulture));
            }
            listaMasterGru = new List<GRUPO>();
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult ReativarGrupo(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            GRUPO item = gruApp.GetItemById(id);
            objetoGruAntes = item;
            item.GRUP_IN_ATIVO = 1;
            Int32 volta = gruApp.ValidateReativar(item, usuario);
            listaMasterGru = new List<GRUPO>();
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult MontarTelaSubGrupo()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensSubGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((List<SUBGRUPO>)Session["ListaSubGrupo"] == null)
            {
                listaMasterSub = subApp.GetAllItens();
                Session["ListaSubGrupo"] = listaMasterSub;
            }
            ViewBag.Listas = (List<SUBGRUPO>)Session["ListaSubGrupo"];
            ViewBag.Title = "SubGrupos";

            // Indicadores
            ViewBag.SubGrupos = ((List<SUBGRUPO>)Session["ListaSubGrupo"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagem
            if ((Int32)Session["MensSubGrupo"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensSubGrupo"] == 3)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0117", CultureInfo.CurrentCulture));
            }

            // Abre view
            objetoSub = new SUBGRUPO();
            Session["VoltaSubGrupo"] = 1;
            Session["MensSubGrupo"] = 0;
            return View(objetoSub);
        }

        public ActionResult MostrarTudoSubGrupo()
        {
            
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSub = subApp.GetAllItensAdm();
            Session["ListaSubGrupo"] = listaMasterSub;
            return RedirectToAction("MontarTelaSubGrupo");
        }

        public ActionResult VoltarBaseSubGrupo()
        {
            
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaSubGrupo"] = gruApp.GetAllItens();
            if ((Int32)Session["VoltaSubGrupo"] == 2)
            {
                return RedirectToAction("IncluirCC", "CentroCusto");
            }
            if ((Int32)Session["VoltaSubGrupo"] == 3)
            {
                return RedirectToAction("EditarCC", "CentroCusto");
            }
            return RedirectToAction("MontarTelaSubGrupo");
        }

        [HttpGet]
        public ActionResult IncluirSubGrupo(Int32? id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensSubGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            SUBGRUPO item = new SUBGRUPO();
            if (id != null)
            {
                Session["VoltaGrupo"] = id.Value;
            }
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SUBG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirSubGrupo(SubgrupoViewModel vm)
        {
            
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = subApp.ValidateCreate(item, usuarioLogado);

                    // Sucesso
                    listaMasterSub = new List<SUBGRUPO>();
                    Session["ListaSubGrupo"] = null;
                    return RedirectToAction("MontarTelaSubGrupo");
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
        public ActionResult EditarSubGrupo(Int32 id)
        {
            
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            SUBGRUPO item = subApp.GetItemById(id);
            objetoSubAntes = item;
            Session["SubGrupo"] = item;
            Session["idVolta"] = id;
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubGrupo(SubgrupoViewModel vm)
        {
            
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    Int32 volta = subApp.ValidateEdit(item, objetoSubAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSub = new List<SUBGRUPO>();
                    Session["ListaSubGrupo"] = null;
                    return RedirectToAction("MontarTelaSubGrupo");
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
        public ActionResult ExcluirSubGrupo(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensSubGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SUBGRUPO item = subApp.GetItemById(id);
            objetoSubAntes = item;
            item.SUBG_IN_ATIVO = 0;
            Int32 volta = subApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensGrupo"] = 3;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0118", CultureInfo.CurrentCulture));
            }
            listaMasterSub = new List<SUBGRUPO>();
            Session["ListaSubGrupo"] = null;
            return RedirectToAction("MontarTelaSubGrupo");
        }

        [HttpGet]
        public ActionResult ReativarSubGrupo(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensSubGrupo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SUBGRUPO item = subApp.GetItemById(id);
            objetoSubAntes = item;
            item.SUBG_IN_ATIVO = 1;
            Int32 volta = subApp.ValidateReativar(item, usuario);
            listaMasterSub = new List<SUBGRUPO>();
            Session["ListaSubGrupo"] = null;
            return RedirectToAction("MontarTelaSubGrupo");
        }

    }
}