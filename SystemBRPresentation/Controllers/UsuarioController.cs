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
    public class UsuarioController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly ILogAppService logApp;
        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public UsuarioController(IUsuarioAppService baseApps, ILogAppService logApps)
        {
            baseApp = baseApps;
            logApp = logApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            USUARIO item = new USUARIO();
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult MontarTelaPerfilUsuario()
        {
            
            // Carrega listas
            USUARIO usuario = SessionMocks.UserCredentials;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Usuário";

            // Abre view
            USUARIO usu = baseApp.GetItemById(usuario.USUA_CD_ID);
            objetoAntes = usu;
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usu);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MontarTelaPerfilUsuario(UsuarioViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    SessionMocks.UserCredentials = item;
                    return RedirectToAction("MontarTelaPerfilUsuario");
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

        public ActionResult Voltar()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarBaseUsuario()
        {
            
            SessionMocks.Usuarios = baseApp.GetAllUsuarios();
            return RedirectToAction("MontarTelaPerfilUsuario");
        }

        [HttpPost]
        public ActionResult UploadFotoUsuario(HttpPostedFileBase file)
        {
            
            if (file == null)
                return Content("Nenhum arquivo selecionado");

            // Recupera arquivo
            USUARIO item = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture);
                return RedirectToAction("MontarTelaPerfilUsuario");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante + "/Usuarios/" + item.USUA_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                file.SaveAs(path);

                // Gravar registro
                item.USUA_AQ_FOTO = "~" + caminho + fileName;
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item, item);
            }
            else
            {
                ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture);
            }
            return RedirectToAction("MontarTelaPerfilUsuario");
        }
    }
}