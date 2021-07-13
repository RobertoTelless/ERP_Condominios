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
    public class BancoController : Controller
    {
        private readonly IBancoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IContaBancariaAppService contaApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IFilialAppService filApp;
        private readonly IContaPagarAppService pagApp;
        private readonly IContaReceberAppService recApp;

        private String msg;
        private Exception exception;
        String extensao = String.Empty;
        BANCO objetoBanco = new BANCO();
        BANCO objetoBancoAntes = new BANCO();
        List<BANCO> listaMasterBanco = new List<BANCO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        CONTA_BANCO objConta = new CONTA_BANCO();
        CONTA_BANCO objContaAntes = new CONTA_BANCO();
        List<CONTA_BANCO> listaMasterConta = new List<CONTA_BANCO>();
        CONTA_BANCO contaPadrao = new CONTA_BANCO();

        public BancoController(IBancoAppService baseApps, ILogAppService logApps, IContaBancariaAppService contaApps, IUsuarioAppService usuApps, IFilialAppService filApps, IContaPagarAppService pagApps, IContaReceberAppService recApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            contaApp = contaApps;
            usuApp = usuApps;
            filApp = filApps;
            pagApp = pagApps;
            recApp = recApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            BANCO item = new BANCO();
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
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
            
            listaMasterBanco = new List<BANCO>();
            listaMasterConta = new List<CONTA_BANCO>();
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            
            listaMasterBanco = new List<BANCO>();
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaBanco()
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
            if (SessionMocks.listaBanco == null)
            {
                listaMasterBanco = baseApp.GetAllItens();
                SessionMocks.listaBanco = listaMasterBanco;
            }
            ViewBag.Listas = SessionMocks.listaBanco;
            ViewBag.Title = "Bancos";

            // Indicadores
            ViewBag.Bancos = SessionMocks.listaBanco.Count;
            ViewBag.Contas = contaApp.GetAllItens().Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objetoBanco = new BANCO();
            return View(objetoBanco);
        }

        public ActionResult RetirarFiltroBanco()
        {
            
            SessionMocks.listaBanco = null;
            return RedirectToAction("MontarTelaBanco");
        }

        public ActionResult MostrarTudoBanco()
        {
            
            listaMasterBanco = baseApp.GetAllItensAdm();
            SessionMocks.listaBanco = listaMasterBanco;
            return RedirectToAction("MontarTelaBanco");
        }

        [HttpPost]
        public ActionResult FiltrarBanco(BANCO item)
        {
            
            try
            {
                // Executa a operação
                List<BANCO> listaObj = new List<BANCO>();
                Int32 volta = baseApp.ExecuteFilter(item.BANC_SG_CODIGO, item.BANC_NM_NOME, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0017", CultureInfo.CurrentCulture);
                    listaMasterBanco = new List<BANCO>();
                    return RedirectToAction("MontarTelaBanco");
                }

                // Sucesso
                listaMasterBanco = listaObj;
                SessionMocks.listaBanco = listaObj;
                return RedirectToAction("MontarTelaBanco");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaBanco");
            }
        }

        public ActionResult VoltarBaseBanco()
        {
            
            listaMasterBanco = new List<BANCO>();
            SessionMocks.listaBanco = null;
            return RedirectToAction("MontarTelaBanco");
        }

        [HttpGet]
        public ActionResult IncluirBanco()
        {
            
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            BANCO item = new BANCO();
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            vm.BANC_IN_ATIVO = 1;
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirBanco(BancoViewModel vm)
        {
            
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0066", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    SessionMocks.banco = item;
                    listaMasterBanco = new List<BANCO>();
                    SessionMocks.listaBanco = null;
                    SessionMocks.voltaConta = 1;
                    return RedirectToAction("IncluirConta");
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
        public ActionResult EditarBanco(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            BANCO item = baseApp.GetItemById(id);
            objetoBancoAntes = item;
            SessionMocks.idBanco = id;
            SessionMocks.banco = item;
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarBanco(BancoViewModel vm)
        {
            
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoBancoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0066", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterBanco = new List<BANCO>();
                    SessionMocks.listaBanco = null;
                    return RedirectToAction("MontarTelaBanco");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("EditarBanco", new { id = SessionMocks.idBanco });
                }
            }
            else
            {
                return RedirectToAction("EditarBanco", new { id = SessionMocks.idBanco });
            }
        }

        [HttpGet]
        public ActionResult ExcluirBanco(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            BANCO item = baseApp.GetItemById(id);
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirBanco(BancoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0067", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMasterBanco = new List<BANCO>();
                SessionMocks.listaBanco = null;
                return RedirectToAction("MontarTelaBanco");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        [HttpGet]
        public ActionResult ReativarBanco(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            BANCO item = baseApp.GetItemById(id);
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarBanco(BancoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterBanco = new List<BANCO>();
                SessionMocks.listaBanco = null;
                return RedirectToAction("MontarTelaBanco");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        [HttpGet]
        public ActionResult IncluirConta()
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(SessionMocks.TipoContas, "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTA_BANCO item = new CONTA_BANCO();
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            vm.BANC_CD_ID = SessionMocks.banco.BANC_CD_ID;
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.COBA_DT_ABERTURA = DateTime.Today;
            vm.COBA_VL_SALDO_INICIAL = 0;
            vm.COBA_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirConta(ContaBancariaViewModel vm)
        {
            
            ViewBag.Tipos = new SelectList(SessionMocks.TipoContas, "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = contaApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0068", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterConta = new List<CONTA_BANCO>();
                    SessionMocks.listaContaBancaria = null;
                    SessionMocks.ContasBancarias = contaApp.GetAllItens();
                    return RedirectToAction("EditarBanco", new { id = SessionMocks.banco.BANC_CD_ID });
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

        public ActionResult RetirarFiltroLancamento()
        {
            
            SessionMocks.FiltroLancamento = null;
            return RedirectToAction("EditarConta", new { id = SessionMocks.idVolta });
            ;
        }

        [HttpGet]
        public ActionResult EditarConta(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(SessionMocks.TipoContas, "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            List<CONTA_BANCO_LANCAMENTO> tipo = new List<CONTA_BANCO_LANCAMENTO>();
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Crédito", CBLA_IN_TIPO = 1 });
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Débito", CBLA_IN_TIPO = 2 });
            ViewBag.TipoLanc = new SelectList(tipo.Select(x => new { x.CBLA_IN_TIPO, x.CBLA_DS_DESCRICAO }).ToList(), "CBLA_IN_TIPO", "CBLA_DS_DESCRICAO");
            ViewBag.TabDadosGer = "active";

            // Prepara view
            CONTA_BANCO item = contaApp.GetItemById(id);
            ViewBag.Lanc = item.CONTA_BANCO_LANCAMENTO.Where(x => x.CBLA_IN_ATIVO == 1).Count();
            ViewBag.Pagar = pagApp.GetAllItens().Where(p => p.COBA_CD_ID == id).ToList().Count;
            ViewBag.Receber = recApp.GetAllItens().Where(p => p.COBA_CD_ID == id).ToList().Count;

            objContaAntes = item;

            if (SessionMocks.FiltroLancamento != null)
            {
                ViewBag.TabDadosGer = "";
                ViewBag.TabLanc = "active";

                CONTA_BANCO_LANCAMENTO cbl = SessionMocks.FiltroLancamento;
                SessionMocks.FiltroLancamento = null;
                List<CONTA_BANCO_LANCAMENTO> lstLanc = new List<CONTA_BANCO_LANCAMENTO>();
                Int32 volta = contaApp.ExecuteFilterLanc(cbl.COBA_CD_ID, cbl.CBLA_DT_LANCAMENTO, cbl.CBLA_IN_TIPO, cbl.CBLA_DS_DESCRICAO, out lstLanc);

                if (volta == 0)
                {
                    item.CONTA_BANCO_LANCAMENTO = new List<CONTA_BANCO_LANCAMENTO>();
                    item.CONTA_BANCO_LANCAMENTO = lstLanc;
                }
                else
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                }
            }

            SessionMocks.idVolta = id;
            SessionMocks.contaPadrao = item;
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            SessionMocks.FiltroLancamento = null;
            return View(vm); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarConta(ContaBancariaViewModel vm)
        {
            
            ViewBag.Tipos = new SelectList(SessionMocks.TipoContas, "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            List<CONTA_BANCO_LANCAMENTO> tipo = new List<CONTA_BANCO_LANCAMENTO>();
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Crédito", CBLA_IN_TIPO = 1 });
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Débito", CBLA_IN_TIPO = 2 });
            ViewBag.TipoLanc = new SelectList(tipo, "CBLA_IN_TIPO", "CBLA_DS_DESCRICAO");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                    Int32 volta = contaApp.ValidateEdit(item, objContaAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterConta = new List<CONTA_BANCO>();
                    SessionMocks.listaContaBancaria = null;
                    return RedirectToAction("EditarBanco", new { id = SessionMocks.banco.BANC_CD_ID });
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

        public ActionResult FiltrarLancamento(CONTA_BANCO_LANCAMENTO item)
        {
            SessionMocks.FiltroLancamento = item;
            return RedirectToAction("EditarConta", new { id = SessionMocks.idVolta });
        }

        [HttpGet]
        public ActionResult ExcluirConta(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            CONTA_BANCO item = contaApp.GetItemById(id);
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirConta(ContaBancariaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                Int32 volta = contaApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0069", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMasterConta = new List<CONTA_BANCO>();
                SessionMocks.listaContaBancaria = null;
                return RedirectToAction("EditarBanco", new { id = SessionMocks.banco.BANC_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        [HttpGet]
        public ActionResult ReativarConta(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            CONTA_BANCO item = contaApp.GetItemById(id);
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarConta(ContaBancariaViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                Int32 volta = contaApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterConta = new List<CONTA_BANCO>();
                SessionMocks.listaContaBancaria = null;
                return RedirectToAction("EditarBanco", new { id = SessionMocks.banco.BANC_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        public ActionResult VoltarBaseConta()
        {
            

            if (Session["voltaLiquidacao"] != null && (Int32)Session["voltaLiquidacao"] == 1)
            {
                return RedirectToAction("VerCP", "ContaPagar", new { id = (Int32)Session["idContaPagar"], liquidar = 1 });
            }

                listaMasterConta = new List<CONTA_BANCO>();
            SessionMocks.listaContaBancaria = null;
            return RedirectToAction("EditarBanco", new { id = SessionMocks.banco.BANC_CD_ID });
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
        {
            
            // Prepara view
            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            ContaBancariaContatoViewModel vm = Mapper.Map<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(ContaBancariaContatoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_BANCO_CONTATO item = Mapper.Map<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>(vm);
                    Int32 volta = contaApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
            
            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            item.CBCT_IN_ATIVO = 0;
            Int32 volta = contaApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
        {
            
            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            item.CBCT_IN_ATIVO = 1;
            Int32 volta = contaApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult IncluirContato()
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTA_BANCO_CONTATO item = new CONTA_BANCO_CONTATO();
            ContaBancariaContatoViewModel vm = Mapper.Map<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>(item);
            vm.COBA_CD_ID = SessionMocks.idVolta;
            vm.CBCT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(ContaBancariaContatoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCO_CONTATO item = Mapper.Map<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = contaApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
        public ActionResult IncluirLancamento()
        {
            
            // Prepara view
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTA_BANCO_LANCAMENTO item = new CONTA_BANCO_LANCAMENTO();
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            vm.COBA_CD_ID = SessionMocks.idVolta;
            vm.CBLA_IN_ATIVO = 1;
            vm.CBLA_IN_ORIGEM = 1;
            vm.CBLA_DT_LANCAMENTO = DateTime.Today.Date;
            vm.CONTA_BANCO = SessionMocks.contaPadrao;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirLancamento(ContaBancariaLancamentoViewModel vm)
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
                    CONTA_BANCO_LANCAMENTO item = Mapper.Map<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = contaApp.ValidateCreateLancamento(item);
                    Int32 volta1 = AcertaSaldo(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
        public ActionResult EditarLancamento(Int32 id)
        {
            
            // Prepara view
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");
            CONTA_BANCO_LANCAMENTO item = contaApp.GetLancamentoById(id);
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarLancamento(ContaBancariaLancamentoViewModel vm)
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
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_BANCO_LANCAMENTO item = Mapper.Map<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>(vm);
                    Int32 volta = contaApp.ValidateEditLancamento(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoConta");
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
        public ActionResult ExcluirLancamento(Int32 id)
        {
            
            CONTA_BANCO_LANCAMENTO item = contaApp.GetLancamentoById(id);
            item.CBLA_IN_ATIVO = 0;
            if (item.CBLA_IN_TIPO == 1)
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
            }
            else
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
            }
            
            Int32 volta = contaApp.ValidateEditLancamento(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult ReativarLancamento(Int32 id)
        {
            
            CONTA_BANCO_LANCAMENTO item = contaApp.GetLancamentoById(id);
            item.CBLA_IN_ATIVO = 1;
            if (item.CBLA_IN_TIPO == 1)
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
            }
            else
            {
                item.CONTA_BANCO.COBA_VL_SALDO_ATUAL = item.CONTA_BANCO.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
            }

            Int32 volta = contaApp.ValidateEditLancamento(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        public ActionResult VoltarAnexoConta()
        {
            
            return RedirectToAction("EditarConta", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarAnexoBanco()
        {
            
            return RedirectToAction("EditarBanco", new { id = SessionMocks.idBanco });
        }

        public Int32 AcertaSaldo(CONTA_BANCO_LANCAMENTO item)
        {
            try
            {
                // Acerta saldo
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CONTA_BANCO_LANCAMENTO lanc = contaApp.GetLancamentoById(item.CBLA_CD_ID);
                if (item.CBLA_IN_TIPO == 1)
                {
                    lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL = lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL + item.CBLA_VL_VALOR.Value;
                }
                else
                {
                    lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL = lanc.CONTA_BANCO.COBA_VL_SALDO_ATUAL - item.CBLA_VL_VALOR.Value;
                }
                Int32 volta = contaApp.ValidateEditLancamento(lanc);
                return volta;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}