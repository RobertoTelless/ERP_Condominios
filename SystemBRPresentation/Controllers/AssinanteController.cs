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
    public class AssinanteController : Controller
    {
        private readonly IAssinanteAppService assApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICategoriaServicoAppService csApp;
        private readonly IFilialAppService filApp;

        private String msg;
        private Exception exception;
        ASSINANTE objetoAss = new ASSINANTE();
        ASSINANTE objetoAssAntes = new ASSINANTE();
        List<ASSINANTE> listaMasterAss = new List<ASSINANTE>();
        String extensao;

        public AssinanteController(IAssinanteAppService assApps, IUsuarioAppService usuApps)
        {
            assApp = assApps;
            usuApp = usuApps;
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

        [HttpGet]
        public ActionResult MontarTelaAssinante()
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
            if (SessionMocks.listaAssinante == null)
            {
                listaMasterAss = assApp.GetAllItens();
                SessionMocks.listaAssinante = listaMasterAss;
            }
            ViewBag.Listas = SessionMocks.listaAssinante;
            ViewBag.Title = "Assinantes";
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 1", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 2", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 3", Value = "3" });
            tipo.Add(new SelectListItem() { Text = "Pro", Value = "4" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

            // Indicadores
            ViewBag.Assinantes = SessionMocks.listaAssinante.Count;

            // Abre view
            objetoAss = new ASSINANTE();
            return View(objetoAss);
        }

        public ActionResult RetirarFiltroAssinante()
        {
            
            SessionMocks.listaAssinante = null;
            return RedirectToAction("MontarTelaAssinante");
        }

        public ActionResult MostrarTudoAssinante()
        {
            
            listaMasterAss = assApp.GetAllItensAdm();
            SessionMocks.listaAssinante = listaMasterAss;
            return RedirectToAction("MontarTelaAssinante");
        }

        [HttpPost]
        public ActionResult FiltrarAssinante(ASSINANTE item)
        {
            
            try
            {
                // Executa a operação
                List<ASSINANTE> listaObj = new List<ASSINANTE>();
                Int32 volta = assApp.ExecuteFilter(item.ASSI_IN_TIPO.Value, item.ASSI_NM_NOME, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterAss = listaObj;
                SessionMocks.listaAssinante = listaObj;
                return RedirectToAction("MontarTelaAssinante");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaAssinante");
            }
        }

        public ActionResult VoltarBaseAssinante()
        {
            return RedirectToAction("MontarTelaAssinante");
        }

        [HttpGet]
        public ActionResult IncluirAssinante()
        {
            
            // Prepara listas
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 1", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 2", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 3", Value = "3" });
            tipo.Add(new SelectListItem() { Text = "Pro", Value = "4" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            ASSINANTE item = new ASSINANTE();
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(item);
            vm.ASSI_DT_INICIO = DateTime.Today.Date;
            vm.ASSI_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirAssinante(AssinanteViewModel vm)
        {
            
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 1", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 2", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 3", Value = "3" });
            tipo.Add(new SelectListItem() { Text = "Pro", Value = "4" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = assApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0106", CultureInfo.CurrentCulture);
                        return View(vm);
                    }
                  
                    // Sucesso
                    listaMasterAss = new List<ASSINANTE>();
                    SessionMocks.listaAssinante = null;
                    SessionMocks.voltaAssinante = 1;
                    SessionMocks.IdAssinanteVolta = item.ASSI_CD_ID;
                    SessionMocks.assinante = item;
                    return RedirectToAction("IncluirUsuarioAssinante", "Administracao");
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
        public ActionResult EditarAssinante(Int32 id)
        {
            
            // Prepara view
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 1", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 2", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 3", Value = "3" });
            tipo.Add(new SelectListItem() { Text = "Pro", Value = "4" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            ASSINANTE item = assApp.GetItemById(id);
            objetoAssAntes = item;
            SessionMocks.assinante = item;
            SessionMocks.idVolta = id;
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAssinante(AssinanteViewModel vm)
        {
            
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 1", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 2", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Basic Tipo 3", Value = "3" });
            tipo.Add(new SelectListItem() { Text = "Pro", Value = "4" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                    Int32 volta = assApp.ValidateEdit(item, objetoAssAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterAss = new List<ASSINANTE>();
                    SessionMocks.listaAssinante = null;
                    return RedirectToAction("MontarTelaAssinante");
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
        public ActionResult ExcluirAssinante(Int32 id)
        {
            
            // Prepara view
            ASSINANTE item = assApp.GetItemById(id);
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirAssinante(AssinanteViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                Int32 volta = assApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0107", CultureInfo.CurrentCulture);
                    return View(vm);
                }

                // Sucesso
                listaMasterAss = new List<ASSINANTE>();
                SessionMocks.listaAssinante = null;
                return RedirectToAction("MontarTelaAssinante");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarAssinante(Int32 id)
        {
            
            // Prepara view
            ASSINANTE item = assApp.GetItemById(id);
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarAssinante(AssinanteViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                Int32 volta = assApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterAss = new List<ASSINANTE>();
                SessionMocks.listaAssinante = null;
                return RedirectToAction("MontarTelaAssinante");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }
    }
}