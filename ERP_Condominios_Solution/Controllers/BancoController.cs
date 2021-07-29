using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Canducci.Zip;

namespace ERP_Condominios_Solution.Controllers
{
    public class BancoController : Controller
    {
        private readonly IBancoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IContaBancariaAppService contaApp;
        private readonly IUsuarioAppService usuApp;

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

        public BancoController(IBancoAppService baseApps, ILogAppService logApps, IContaBancariaAppService contaApps, IUsuarioAppService usuApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            contaApp = contaApps;
            usuApp = usuApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaBanco()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" )
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaBanco"] == null)
            {
                listaMasterBanco = baseApp.GetAllItens(idAss);
                Session["ListaBanco"] = listaMasterBanco;
            }
            ViewBag.Listas = ((List<BANCO>)Session["ListaBanco"]).ToList();
            ViewBag.Title = "Bancos";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensBanco"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensBanco"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0039", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoBanco = new BANCO();
            Session["MensBanco"] = 0;
            return View(objetoBanco);
        }

        public ActionResult RetirarFiltroBanco()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaBanco"] = null;
            return RedirectToAction("MontarTelaBanco");
        }

        public ActionResult MostrarTudoBanco()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterBanco = baseApp.GetAllItensAdm(idAss);
            Session["ListaBanco"] = listaMasterBanco;
            return RedirectToAction("MontarTelaBanco");
        }

        [HttpPost]
        public ActionResult FiltrarBanco(BANCO item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<BANCO> listaObj = new List<BANCO>();
                Int32 volta = baseApp.ExecuteFilter(item.BANC_SG_CODIGO, item.BANC_NM_NOME, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensBanco"] = 1;
                    return RedirectToAction("MontarTelaBanco");
                }

                // Sucesso
                listaMasterBanco = listaObj;
                Session["ListaBanco"] = listaObj;
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
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMasterBanco = new List<BANCO>();
            Session["ListaBanco"] = null;
            return RedirectToAction("MontarTelaBanco");
        }

        [HttpGet]
        public ActionResult IncluirBanco()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            BANCO item = new BANCO();
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            vm.BANC_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirBanco(BancoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensBanco"] = 3;
                        return RedirectToAction("MontarTelaBanco", "Banco");
                    }

                    // Sucesso
                    Session["Banco"] = item;
                    Session["IdBanco"] = item.BANC_CD_ID;
                    listaMasterBanco = new List<BANCO>();
                    Session["ListaBanco"] = null;
                    Session["VoltaConta"] = 1;
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
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (Session["MensBanco"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensBanco"] == 6)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 5)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensBanco"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            BANCO item = baseApp.GetItemById(id);
            objetoBancoAntes = item;
            Session["IdBanco"] = id;
            Session["Banco"] = item;
            BancoViewModel vm = Mapper.Map<BANCO, BancoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarBanco(BancoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoBancoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensBanco"] = 3;
                        return RedirectToAction("MontarTelaBanco", "Banco");
                    }

                    // Sucesso
                    listaMasterBanco = new List<BANCO>();
                    Session["ListaBanco"] = null;
                    return RedirectToAction("MontarTelaBanco");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                }
            }
            else
            {
                return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
            }
        }

        [HttpGet]
        public ActionResult ExcluirBanco(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

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
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensBanco"] = 4;
                    return RedirectToAction("MontarTelaBanco", "Banco");
                }

                // Sucesso
                listaMasterBanco = new List<BANCO>();
                Session["ListaBanco"] = null;
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
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

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
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                BANCO item = Mapper.Map<BancoViewModel, BANCO>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterBanco = new List<BANCO>();
                Session["ListaBanco"] = null;
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
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");

            // Prepara view
            CONTA_BANCO item = new CONTA_BANCO();
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            vm.BANC_CD_ID = (Int32)Session["IdBanco"];
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.COBA_DT_ABERTURA = DateTime.Today.Date;
            vm.COBA_VL_SALDO_INICIAL = 0;
            vm.COBA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirConta(ContaBancariaViewModel vm)
        {

            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = contaApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensBanco"] = 5;
                        return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                    }

                    // Sucesso
                    listaMasterConta = new List<CONTA_BANCO>();
                    Session["ListaContaBancaria"] = null;
                    Session["ContasBancarias"] = contaApp.GetAllItens(idAss);
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"]  });
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

            Session["FiltroLancamento"] = null;
            return RedirectToAction("EditarConta", new { id = (Int32)Session["IdConta"] });
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarConta(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");
            List<CONTA_BANCO_LANCAMENTO> tipo = new List<CONTA_BANCO_LANCAMENTO>();
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Crédito", CBLA_IN_TIPO = 1 });
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Débito", CBLA_IN_TIPO = 2 });
            ViewBag.TipoLanc = new SelectList(tipo.Select(x => new { x.CBLA_IN_TIPO, x.CBLA_DS_DESCRICAO }).ToList(), "CBLA_IN_TIPO", "CBLA_DS_DESCRICAO");
            ViewBag.TabDadosGer = "active";

            // Prepara view
            CONTA_BANCO item = contaApp.GetItemById(id);
            ViewBag.Lanc = item.CONTA_BANCO_LANCAMENTO.Where(x => x.CBLA_IN_ATIVO == 1).Count();
            //ViewBag.Pagar = pagApp.GetAllItens().Where(p => p.COBA_CD_ID == id).ToList().Count;
            //ViewBag.Receber = recApp.GetAllItens().Where(p => p.COBA_CD_ID == id).ToList().Count;
            objContaAntes = item;

            if (Session["FiltroLancamento"] != null)
            {
                ViewBag.TabDadosGer = "";
                ViewBag.TabLanc = "active";

                CONTA_BANCO_LANCAMENTO cbl = (CONTA_BANCO_LANCAMENTO)Session["FiltroLancamento"];
                Session["FiltroLancamento"] = null;
                List<CONTA_BANCO_LANCAMENTO> lstLanc = new List<CONTA_BANCO_LANCAMENTO>();
                Int32 volta = contaApp.ExecuteFilterLanc(cbl.COBA_CD_ID, cbl.CBLA_DT_LANCAMENTO, cbl.CBLA_IN_TIPO, cbl.CBLA_DS_DESCRICAO, out lstLanc);

                if (volta == 0)
                {
                    item.CONTA_BANCO_LANCAMENTO = new List<CONTA_BANCO_LANCAMENTO>();
                    item.CONTA_BANCO_LANCAMENTO = lstLanc;
                }
                else
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
            }

            Session["IdVolta"] = id;
            Session["IdConta"] = id;
            Session["ContaPadrao"] = item;
            ContaBancariaViewModel vm = Mapper.Map<CONTA_BANCO, ContaBancariaViewModel>(item);
            Session["FiltroLancamento"] = null;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarConta(ContaBancariaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(contaApp.GetAllTipos(idAss).OrderBy(x => x.TICO_NM_NOME).ToList<TIPO_CONTA>(), "TICO_CD_ID", "TICO_NM_NOME");
            List<CONTA_BANCO_LANCAMENTO> tipo = new List<CONTA_BANCO_LANCAMENTO>();
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Crédito", CBLA_IN_TIPO = 1 });
            tipo.Add(new CONTA_BANCO_LANCAMENTO() { CBLA_DS_DESCRICAO = "Débito", CBLA_IN_TIPO = 2 });
            ViewBag.TipoLanc = new SelectList(tipo.Select(x => new { x.CBLA_IN_TIPO, x.CBLA_DS_DESCRICAO }).ToList(), "CBLA_IN_TIPO", "CBLA_DS_DESCRICAO");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                    Int32 volta = contaApp.ValidateEdit(item, objContaAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterConta = new List<CONTA_BANCO>();
                    Session["ListaContaBancaria"] = null;
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
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
            Session["FiltroLancamento"] = item;
            return RedirectToAction("EditarConta", new { id = (Int32)Session["IdConta"] });
        }

        [HttpGet]
        public ActionResult ExcluirConta(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

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
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                Int32 volta = contaApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensFornecedor"] = 6;
                    return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
                }

                // Sucesso
                listaMasterConta = new List<CONTA_BANCO>();
                Session["ListaContaBancaria"] = null;
                return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
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
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

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
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTA_BANCO item = Mapper.Map<ContaBancariaViewModel, CONTA_BANCO>(vm);
                Int32 volta = contaApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterConta = new List<CONTA_BANCO>();
                Session["ListaContaBancaria"] = null;
                return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoBanco);
            }
        }

        public ActionResult VoltarBaseConta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            //if (Session["voltaLiquidacao"] != null && (Int32)Session["voltaLiquidacao"] == 1)
            //{
            //    return RedirectToAction("VerCP", "ContaPagar", new { id = (Int32)Session["idContaPagar"], liquidar = 1 });
            //}

            listaMasterConta = new List<CONTA_BANCO>();
            Session["ListaContaBancaria"] = null;
            return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            // Prepara view
            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            ContaBancariaContatoViewModel vm = Mapper.Map<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(ContaBancariaContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            item.CBCT_IN_ATIVO = 0;
            Int32 volta = contaApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            CONTA_BANCO_CONTATO item = contaApp.GetContatoById(id);
            item.CBCT_IN_ATIVO = 1;
            Int32 volta = contaApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoConta");
        }

        [HttpGet]
        public ActionResult IncluirContato()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CONTA_BANCO_CONTATO item = new CONTA_BANCO_CONTATO();
            ContaBancariaContatoViewModel vm = Mapper.Map<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>(item);
            vm.COBA_CD_ID = (Int32)Session["IdConta"];
            vm.CBCT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(ContaBancariaContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_BANCO_CONTATO item = Mapper.Map<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            List<SelectListItem> tipoLancamento = new List<SelectListItem>();
            tipoLancamento.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoLancamento.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoLancamento, "Value", "Text");

            CONTA_BANCO_LANCAMENTO item = new CONTA_BANCO_LANCAMENTO();
            ContaBancariaLancamentoViewModel vm = Mapper.Map<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>(item);
            vm.COBA_CD_ID = (Int32)Session["IdVolta"];
            vm.CBLA_IN_ATIVO = 1;
            vm.CBLA_IN_ORIGEM = 1;
            vm.CBLA_DT_LANCAMENTO = DateTime.Today.Date;
            vm.CONTA_BANCO = (CONTA_BANCO)Session["ContaPadrao"];
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirLancamento(ContaBancariaLancamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_BANCO conta = (CONTA_BANCO)Session["ContaPadrao"];
                    Int32 volta = contaApp.ValidateCreateLancamento(item, conta);
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensBanco"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

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

            return RedirectToAction("EditarConta", new { id = (Int32)Session["IdConta"] });
        }

        public ActionResult VoltarAnexoBanco()
        {

            return RedirectToAction("EditarBanco", new { id = (Int32)Session["IdBanco"] });
        }

        public Int32 AcertaSaldo(CONTA_BANCO_LANCAMENTO item)
        {
            try
            {
                // Acerta saldo
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
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