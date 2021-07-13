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
    public class ValorComissaoController : Controller
    {
        private readonly ILogAppService logApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IFilialAppService filialApp;
        private readonly ICargoAppService cargoApp;
        private readonly IValorComissaoAppService comissaoApp;

        private String msg;
        private Exception exception;
        String extensao = String.Empty;
        VALOR_COMISSAO objComissao = new VALOR_COMISSAO();
        VALOR_COMISSAO objComissaoAntes = new VALOR_COMISSAO();
        List<VALOR_COMISSAO> listaMasterComissao = new List<VALOR_COMISSAO>();

        public ValorComissaoController(ILogAppService logApps, IMatrizAppService matrizApps, IFilialAppService filialApps, ICargoAppService cargoApps, IValorComissaoAppService comissaoApps)
        {
            logApp = logApps;
            matrizApp = matrizApps;
            filialApp = filialApps;
            cargoApp = cargoApps;
            comissaoApp = comissaoApps;
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

        public ActionResult VoltarDashboard()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

       [HttpGet]
        public ActionResult MontarTelaComissao()
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
            if (SessionMocks.listaComissao == null)
            {
                listaMasterComissao = comissaoApp.GetAllItens();
                SessionMocks.listaComissao = listaMasterComissao;
            }
            ViewBag.Listas = SessionMocks.listaComissao;
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Title = "Valores de Comissão";
            ViewBag.Categorias = new SelectList(comissaoApp.GetAllCategorias().OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Filiais = new SelectList(comissaoApp.GetAllFiliais().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(comissaoApp.GetAllTipos(), "TICO_CD_ID", "TICO_NM_NOME");

            // Indicadores
            ViewBag.Comissoes = listaMasterComissao.Count;

            // Abre view
            objComissao = new VALOR_COMISSAO();
            if (SessionMocks.filtroComissao != null)
            {
                objComissao = SessionMocks.filtroComissao;
            }
            return View(objComissao);
        }

        public ActionResult RetirarFiltroComissao()
        {
            
            SessionMocks.listaComissao = null;
            SessionMocks.filtroComissao = null;
            return RedirectToAction("MontarTelaComissao");
        }

        public ActionResult MostrarTudoComissao()
        {
            
            listaMasterComissao = comissaoApp.GetAllItensAdm();
            SessionMocks.filtroComissao = null;
            SessionMocks.listaComissao = listaMasterComissao;
            return RedirectToAction("MontarTelaComissao");
        }

        [HttpPost]
        public ActionResult FiltrarComissao(VALOR_COMISSAO item)
        {
            
            try
            {
                // Executa a operação
                List<VALOR_COMISSAO> listaObj = new List<VALOR_COMISSAO>();
                SessionMocks.filtroComissao = item;
                Int32 volta = comissaoApp.ExecuteFilter(item.CAPR_CD_ID, item.TICO_CD_ID, item.VACO_NM_NOME, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterComissao = listaObj;
                SessionMocks.listaComissao = listaObj;
                return RedirectToAction("MontarTelaComissao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaComissao");
            }
        }

        public ActionResult VoltarBaseComissao()
        {
            
            return RedirectToAction("MontarTelaComissao");
        }

        [HttpGet]
        public ActionResult IncluirComissao()
        {
            
            // Prepara listas
            ViewBag.Categorias = new SelectList(comissaoApp.GetAllCategorias().OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Filiais = new SelectList(comissaoApp.GetAllFiliais().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(comissaoApp.GetAllTipos(), "TICO_CD_ID", "TICO_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            VALOR_COMISSAO item = new VALOR_COMISSAO();
            ValorComissaoViewModel vm = Mapper.Map<VALOR_COMISSAO, ValorComissaoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.FILI_CD_ID = usuario.FILI_CD_ID;
            vm.VACO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirComissao(ValorComissaoViewModel vm)
        {
            
            ViewBag.Categorias = new SelectList(comissaoApp.GetAllCategorias().OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Filiais = new SelectList(comissaoApp.GetAllFiliais().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(comissaoApp.GetAllTipos(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    VALOR_COMISSAO item = Mapper.Map<ValorComissaoViewModel, VALOR_COMISSAO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = comissaoApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterComissao = new List<VALOR_COMISSAO>();
                    SessionMocks.listaComissao = null;
                    return RedirectToAction("MontarTelaComissao");
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
        public ActionResult EditarComissao(Int32 id)
        {
            
            // Prepara view
            ViewBag.Categorias = new SelectList(comissaoApp.GetAllCategorias().OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Filiais = new SelectList(comissaoApp.GetAllFiliais().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(comissaoApp.GetAllTipos(), "TICO_CD_ID", "TICO_NM_NOME");
            VALOR_COMISSAO item = comissaoApp.GetItemById(id);
            objComissaoAntes = item;
            SessionMocks.comissao = item;
            ValorComissaoViewModel vm = Mapper.Map<VALOR_COMISSAO, ValorComissaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarComissao(ValorComissaoViewModel vm)
        {
            
            ViewBag.Categorias = new SelectList(comissaoApp.GetAllCategorias().OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Filiais = new SelectList(comissaoApp.GetAllFiliais().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(comissaoApp.GetAllTipos(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    VALOR_COMISSAO item = Mapper.Map<ValorComissaoViewModel, VALOR_COMISSAO>(vm);
                    Int32 volta = comissaoApp.ValidateEdit(item, objComissaoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterComissao = new List<VALOR_COMISSAO>();
                    SessionMocks.listaComissao = null;
                    return RedirectToAction("MontarTelaComissao");
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
        public ActionResult ExcluirComissao(Int32 id)
        {
            
            // Prepara view
            VALOR_COMISSAO item = comissaoApp.GetItemById(id);
            ValorComissaoViewModel vm = Mapper.Map<VALOR_COMISSAO, ValorComissaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirComissao(ValorComissaoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                VALOR_COMISSAO item = Mapper.Map<ValorComissaoViewModel, VALOR_COMISSAO>(vm);
                Int32 volta = comissaoApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMasterComissao = new List<VALOR_COMISSAO>();
                SessionMocks.listaComissao = null;
                return RedirectToAction("MontarTelaComissao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objComissao);
            }
        }

        [HttpGet]
        public ActionResult ReativarComissao(Int32 id)
        {
            
            // Prepara view
            VALOR_COMISSAO item = comissaoApp.GetItemById(id);
            ValorComissaoViewModel vm = Mapper.Map<VALOR_COMISSAO, ValorComissaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarComissao(ValorComissaoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                VALOR_COMISSAO item = Mapper.Map<ValorComissaoViewModel, VALOR_COMISSAO>(vm);
                Int32 volta = comissaoApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterComissao = new List<VALOR_COMISSAO>();
                SessionMocks.listaComissao = null;
                return RedirectToAction("MontarTelaComissao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objComissao);
            }
        }
    }
}