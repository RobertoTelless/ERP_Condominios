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
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class DepartamentoController : Controller
    {
        private readonly IDepartamentoAppService deptApp;
        private readonly ILogAppService logApp;

        private String msg;
        private Exception exception;
        DEPARTAMENTO objetoDept = new DEPARTAMENTO();
        DEPARTAMENTO objetoDeptAntes = new DEPARTAMENTO();
        List<DEPARTAMENTO> listaMasterDept = new List<DEPARTAMENTO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public DepartamentoController(IDepartamentoAppService deptApps, ILogAppService logApps)
        {
            deptApp = deptApps;
            logApp = logApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            DEPARTAMENTO item = new DEPARTAMENTO();
            DepartamentoViewModel vm = Mapper.Map<DEPARTAMENTO, DepartamentoViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            
            listaMasterDept = new List<DEPARTAMENTO>();
            SessionMocks.listaDepartamento = null;
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaDepartamento()
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
            if (SessionMocks.listaDepartamento == null)
            {
                listaMasterDept = deptApp.GetAllItens();
                SessionMocks.listaDepartamento = listaMasterDept;
            }
            ViewBag.Listas = SessionMocks.listaDepartamento;
            ViewBag.Title = "Departamentoxxx";
            ViewBag.Error = SessionMocks.Error;

            // Indicadores
            ViewBag.Itens = listaMasterDept.Count;

            // Mensagens
            if ((Int32)Session["MensDept"] == 1)
            {
                Session["MensDept"] = 0;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture));
                return RedirectToAction("MontarTelaDepartamento");
            }

            // Abre view
            Session["MensDept"] = 0;
            objetoDept = new DEPARTAMENTO();
            return View(objetoDept);
        }

        public ActionResult MostrarTudoDepartamento()
        {
            
            listaMasterDept = deptApp.GetAllItensAdm();
            SessionMocks.listaDepartamento = listaMasterDept;
            return RedirectToAction("MontarTelaDepartamento");
        }

        [HttpGet]
        public ActionResult IncluirDepartamento(Int32? id)
        {
            
            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            DEPARTAMENTO item = new DEPARTAMENTO();
            SessionMocks.voltaDepartamento = id.Value;
            DepartamentoViewModel vm = Mapper.Map<DEPARTAMENTO, DepartamentoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.DEPT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirDepartamento(DepartamentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    DEPARTAMENTO item = Mapper.Map<DepartamentoViewModel, DEPARTAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = deptApp.ValidateCreate(item, usuarioLogado);

                    // SucessoDepartamento
                    listaMasterDept = new List<DEPARTAMENTO>();
                    SessionMocks.listaDepartamento = null;
                    if (SessionMocks.voltaDepartamento == 1)
                    {
                        return RedirectToAction("MontarTelaDepartamento");
                    }
                    if (SessionMocks.voltaDepartamento == 2)
                    {
                        return RedirectToAction("IncluirDepartamento", "Cadastros");
                    }
                    if (SessionMocks.voltaDepartamento == 3)
                    {
                        return RedirectToAction("EditarDepartamento", "Cadastros", new { id = SessionMocks.idVolta });
                    }
                    return RedirectToAction("MontarTelaDepartamento");
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
        public ActionResult EditarDepartamento(Int32 id)
        {
            
            DEPARTAMENTO item = deptApp.GetItemById(id);
            objetoDeptAntes = item;
            SessionMocks.departamento = item;
            SessionMocks.idVolta = id;
            DepartamentoViewModel vm = Mapper.Map<DEPARTAMENTO, DepartamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDepartamento(DepartamentoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    DEPARTAMENTO item = Mapper.Map<DepartamentoViewModel, DEPARTAMENTO>(vm);
                    Int32 volta = deptApp.ValidateEdit(item, objetoDeptAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterDept = new List<DEPARTAMENTO>();
                    SessionMocks.listaDepartamento = null;
                    return RedirectToAction("MontarTelaDepartamento");
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
        public ActionResult ExcluirDepartamento(Int32 id)
        {
            
            USUARIO usu = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usu = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usu.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Executar
            DEPARTAMENTO item = deptApp.GetItemById(id);
            objetoDeptAntes = SessionMocks.departamento;
            item.DEPT_IN_ATIVO = 0;
            Int32 volta = deptApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                SessionMocks.Error = SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture);
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture));
                return RedirectToAction("MontarTelaDepartamento");
            }
            listaMasterDept = new List<DEPARTAMENTO>();
            SessionMocks.listaDepartamento = null;
            return RedirectToAction("MontarTelaDepartamento");
        }

        [HttpGet]
        public ActionResult ReativarDepartamento(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            DEPARTAMENTO item = deptApp.GetItemById(id);
            objetoDeptAntes = SessionMocks.departamento;
            item.DEPT_IN_ATIVO = 1;
            Int32 volta = deptApp.ValidateReativar(item, usu);
            listaMasterDept = new List<DEPARTAMENTO>();
            SessionMocks.listaCatProd = null;
            return RedirectToAction("MontarTelaDepartamento");
        }

        public ActionResult VoltarBaseDepartamento()
        {
            
            SessionMocks.listaDepartamento = deptApp.GetAllItens();
            if (SessionMocks.voltaDepartamento == 1)
            {
                return RedirectToAction("MontarTelaDepartamento");
            }
            if (SessionMocks.voltaDepartamento == 2)
            {
                return RedirectToAction("IncluirDepartamento", "Cadastros");
            }
            if (SessionMocks.voltaDepartamento == 3)
            {
                return RedirectToAction("EditarDepartamento", "Cadastros", new { id = SessionMocks.idVolta });
            }
            return RedirectToAction("MontarTelaDepartamento");
        }
    }
}