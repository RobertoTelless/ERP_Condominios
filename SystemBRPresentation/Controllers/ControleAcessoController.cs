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

namespace SystemBRPresentation.Controllers
{
    public class ControleAcessoController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly IMatrizAppService matrizApp;
        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public ControleAcessoController(IUsuarioAppService baseApps, IMatrizAppService matrizApps)
        {
            baseApp = baseApps;
            matrizApp = matrizApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            USUARIO item = new USUARIO();
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult Login()
        {
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario;
                SessionMocks.UserCredentials = null;
                ViewBag.Usuario = null;
                USUARIO login = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateLogin(login.USUA_NM_LOGIN, login.USUA_NM_SENHA, out usuario);
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 9)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Armazena credenciais para autorização
                SessionMocks.UserCredentials = usuario;
                Session["UserCredentials"] = usuario;
                SessionMocks.Usuario = usuario;
                SessionMocks.IdAssinante = usuario.ASSI_CD_ID;
                SessionMocks.tipoAssinante = usuario.ASSINANTE.ASSI_IN_TIPO.Value;
                SessionMocks.assinante = usuario.ASSINANTE;
                if (usuario.FILI_CD_ID != null)
                {
                    SessionMocks.idFilial = usuario.FILI_CD_ID.Value;
                }
                else
                {
                    SessionMocks.idFilial = 1;
                }

                // Atualiza view
                String frase = String.Empty;
                String nome = usuario.USUA_NM_NOME;
                if (DateTime.Now.Hour <= 12)
                {
                    frase = "Bom dia, " + nome;
                }
                else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
                {
                    frase = "Boa tarde, " + nome;
                }
                else
                {
                    frase = "Boa noite, " + nome;
                }
                ViewBag.Greeting = frase;
                ViewBag.Nome = usuario.USUA_NM_NOME;
                ViewBag.Cargo = usuario.CARGO.CARG_NM_NOME;
                ViewBag.Foto = usuario.USUA_AQ_FOTO;

                SessionMocks.Greeting = frase;
                SessionMocks.Nome = usuario.USUA_NM_NOME;
                SessionMocks.Cargo = usuario.CARGO.CARG_NM_NOME;
                SessionMocks.Foto = usuario.USUA_AQ_FOTO;
                SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
                Session["Matriz"] = SessionMocks.Matriz;
                SessionMocks.perfil = usuario.PERFIL;
                SessionMocks.flagInicial = 0;
                SessionMocks.filtroData = 1;
                SessionMocks.filtroStatus = 1;
                Session["Ativa"] = "1";
                Session["Login"] = 1;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;

                // Route
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    return RedirectToAction("TrocarSenhaInicio", "ControleAcesso");
                }
                if (SessionMocks.UserCredentials != null)
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public ActionResult Logout()
        {
            SessionMocks.UserCredentials = null;
            SessionMocks.filtroAtendimento = null;
            SessionMocks.listaAtendimento = null;
            Session.Clear();
            return RedirectToAction("Login", "ControleAcesso");
        }

        public ActionResult Cancelar()
        {
            return RedirectToAction("Login", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult TrocarSenha()
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

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usuario);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = SessionMocks.UserCredentials;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                SessionMocks.UserCredentials = null;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaInicio()
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

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usuario);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenhaInicio(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = SessionMocks.UserCredentials;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                SessionMocks.UserCredentials = null;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult GerarSenha()
        {
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GerarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                // Processa
                SessionMocks.UserCredentials = null;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.GenerateNewPassword(item.USUA_NM_EMAIL);
                if (volta == 1)
                {
                    //ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    return Json(SystemBR_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    //ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                    return Json(SystemBR_Resource.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                }
                if (volta == 3)
                {
                    //ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return Json(SystemBR_Resource.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if (volta == 4)
                {
                    //ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return Json(SystemBR_Resource.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                }
                return Json(1);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Json(ex.Message);
            }
        }
    }
}