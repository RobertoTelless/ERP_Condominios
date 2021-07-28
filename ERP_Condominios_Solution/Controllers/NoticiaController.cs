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
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;

namespace ERP_Condominios_Solution.Controllers
{
    public class NoticiaController : Controller
    {
        private readonly INoticiaAppService baseApp;
        private readonly ILogAppService logApp;

        private String msg;
        private Exception exception;
        NOTICIA objeto = new NOTICIA();
        NOTICIA objetoAntes = new NOTICIA();
        List<NOTICIA> listaMaster = new List<NOTICIA>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public NoticiaController(INoticiaAppService baseApps, ILogAppService logApps)
        {
            baseApp = baseApps;
            logApp = logApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        public ActionResult VerNoticia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["IdVolta"] = id;
            NOTICIA item = baseApp.GetItemById(id);
            item.NOTC_NR_ACESSO = ++item.NOTC_NR_ACESSO;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult IncluirComentario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdVolta"];
            NOTICIA item = baseApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            NOTICIA_COMENTARIO coment = new NOTICIA_COMENTARIO();
            NoticiaComentarioViewModel vm = Mapper.Map<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>(coment);
            vm.NOCO_DT_COMENTARIO = DateTime.Now;
            vm.NOCO_IN_ATIVO = 1;
            vm.NOTC_CD_ID = item.NOTC_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentario(NoticiaComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    NOTICIA_COMENTARIO item = Mapper.Map<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTICIA not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.NOTICIA_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VerNoticia", new { id = idNot});
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

        public ActionResult MontarTelaUsuario()
        {
            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            usuario = (USUARIO)Session["UserCredentials"];

            if ((List<NOTICIA>)Session["ListaNoticia"] == null)
            {
                listaMaster = baseApp.GetAllItensValidos(idAss);
                Session["ListaNoticia"] = listaMaster;
            }
            ViewBag.Listas = (List<NOTICIA>)Session["ListaNoticia"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Notícias";

            // Indicadores
            ViewBag.Noticias = ((List<NOTICIA>)Session["ListaNoticia"]).Count;

            // Mensagem
            if ((Int32)Session["MensNoticia"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensNoticia"] = 0;
            Session["VoltaNoticia"] = 2;
            objeto = new NOTICIA();
            return View(objeto);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaNoticia"] == 1)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["VoltaNoticia"] == 2)
            {
                return RedirectToAction("MontarTelaUsuario", "Noticia");
            }
            return RedirectToAction("MontarTelaNoticia", "Noticia");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("VerNoticia", new { id = idNot });
        }

        [HttpPost]
        public ActionResult FiltrarNoticia(NOTICIA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Int32 volta = baseApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensNoticia"] = 1;
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }

                // Sucesso
                Session["MensNoticia"] = 0;
                listaMaster = listaObj;
                Session["ListaNoticia"] = listaObj;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaUsuario");
            }
        }

        public ActionResult RetirarFiltroNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaNoticia"] = null;
            return RedirectToAction("MontarTelaUsuario");
        }

        public ActionResult MostrarTudoNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItens(idAss);
            Session["ListaNoticia"] = listaMaster;
            return RedirectToAction("MontarTelaUsuario");
        }

       [HttpGet]
        public ActionResult MontarTelaNoticia()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensNoticia"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((List<NOTICIA>)Session["ListaNoticia"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaNoticia"] = listaMaster;
            }
            ViewBag.Listas = (List<NOTICIA>)Session["ListaNoticia"];
            ViewBag.Title = "Notícias";

            // Indicadores
            ViewBag.Noticias = ((List<NOTICIA>)Session["ListaNoticia"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagem
            if ((Int32)Session["MensNoticia"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensNoticia"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            // Abre view
            objeto = new NOTICIA();
            Session["VoltaNoticia"] = 1;
            Session["MensNoticia"] = 0;
            return View(objeto);
        }

        public ActionResult RetirarFiltroNoticiaGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaNoticia"] = null;
            if ((Int32)Session["VoltaNoticia"] == 2)
            {
                return RedirectToAction("VerCardsNoticias");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        public ActionResult MostrarTudoNoticiaGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaNoticia"] = listaMaster;
            if ((Int32)Session["VoltaNoticia"] == 2)
            {
                return RedirectToAction("VerCardsNoticias");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        [HttpPost]
        public ActionResult FiltrarNoticiaGeral(NOTICIA item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Int32 volta = baseApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensNoticia"] = 1;
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }

                // Sucesso
                Session["MensNoticia"] = 0;
                listaMaster = listaObj;
                Session["ListaNoticia"]  = listaObj;
                if ((Int32)Session["VoltaNoticia"] == 2)
                {
                    return RedirectToAction("VerCardsNoticias");
                }
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaNoticia");
            }
        }

        public ActionResult VoltarBaseNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaNoticia"] == 2)
            {
                return RedirectToAction("VerCardsNoticias");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        [HttpGet]
        public ActionResult IncluirNoticia()
        {
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensNoticia"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            vm.ASSI_CD_ID = (Int32)Session["IdAssinante"];
            vm.NOTC_DT_EMISSAO = DateTime.Today.Date;
            vm.NOTC_IN_ATIVO = 1;
            vm.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTC_NR_ACESSO = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirNoticia(NoticiaViewModel vm)
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
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Carrega foto e processa alteracao
                    item.NOTC_AQ_FOTO = "~/Imagens/p_big2.jpg";
                    volta = baseApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["VoltaNoticia"] = 1;
                    Session["IdNoticiaVolta"] = item.NOTC_CD_ID;
                    Session["Noticia"] = item;
                    Session["MensNoticia"] = 0;
                    if ((Int32)Session["VoltaNoticia"] == 2)
                    {
                        return RedirectToAction("VerCardsNoticias");
                    }
                    return RedirectToAction("MontarTelaNoticia");
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
        public ActionResult EditarNoticia(Int32 id)
        {
            // Prepara view
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensAcesso"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            NOTICIA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Noticia"] = item;
            Session["IdVolta"] = id;
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarNoticia(NoticiaViewModel vm)
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
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["MensNoticia"] = 0;
                    if ((Int32)Session["VoltaNoticia"] == 2)
                    {
                        return RedirectToAction("VerCardsNoticias");
                    }
                    return RedirectToAction("MontarTelaNoticia");
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
        public ActionResult ExcluirNoticia(Int32 id)
        {
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensNoticia"] = 2;
                    return RedirectToAction("MontarTelaNoticia", "Noticia");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            NOTICIA item = baseApp.GetItemById(id);
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirNoticia(NoticiaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<NOTICIA>();
                Session["ListaNoticia"] = null;
                Session["MensNoticia"] = 0;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarNoticia(Int32 id)
        {
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensNoticia"] = 2;
                    return RedirectToAction("MontarTelaNoticia", "Noticia");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            NOTICIA item = baseApp.GetItemById(id);
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarNoticia(NoticiaViewModel vm)
        {
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<NOTICIA>();
                Session["ListaNoticia"] = null;
                Session["MensNoticia"] = 0;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpPost]
        public ActionResult UploadFotoNoticia(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoNoticia");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 idNot = (Int32)Session["IdVolta"];

            NOTICIA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoNoticia");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.NOTC_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoNoticia");
        }

        public ActionResult VoltarAnexoNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("EditarNoticia", new { id = idNot });
        }

    }
}