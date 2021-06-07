using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;

namespace ERP_Condominios_Solution.Controllers
{
    public class ControleAcessoController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public ControleAcessoController(IUsuarioAppService baseApps)
        {
            baseApp = baseApps;
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
                Session["UserCredentials"] = null;
                ViewBag.Usuario = null;
                USUARIO login = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateLogin(login.USUA_NM_LOGIN, login.USUA_NM_SENHA, out usuario);
                if (volta == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 9)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Armazena credenciais para autorização
                Session["UserCredentials"] = usuario;
                Session["Usuario"] = usuario;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["TipoAssinante"] = usuario.ASSINANTE.ASSI_IN_TIPO.Value;
                Session["Assinante"] = usuario.ASSINANTE;

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

                Session["Greeting"] = frase;
                Session["Nome"] = usuario.USUA_NM_NOME;
                Session["Cargo"] = usuario.CARGO.CARG_NM_NOME;
                Session["Foto"] = usuario.USUA_AQ_FOTO;
                Session["Perfil"] = usuario.PERFIL;
                Session["FlagInicial"] = 0;
                Session["FiltroData"] = 1;
                Session["FiltroStatus"] = 1;
                Session["Ativa"] = "1";
                Session["Login"] = 1;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;

                // Route
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    return RedirectToAction("TrocarSenhaInicio", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
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
            Session["UserCredentials"] = null;
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
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
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
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = ERP_Condominios_Resource.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                Session["UserCredentials"] = null;
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
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
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
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = ERP_Condominios_Resource.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                Session["UserCredentials"] = null;
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
                Session["UserCredentials"] = null;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.GenerateNewPassword(item.USUA_NM_EMAIL);
                if (volta == 1)
                {
                    return Json(ERP_Condominios_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(ERP_Condominios_Resource.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                }
                if (volta == 3)
                {
                    return Json(ERP_Condominios_Resource.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if (volta == 4)
                {
                    return Json(ERP_Condominios_Resource.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
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