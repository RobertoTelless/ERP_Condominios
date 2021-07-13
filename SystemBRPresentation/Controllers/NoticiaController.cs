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
            
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        public ActionResult VerNoticia(Int32 id)
        {
            
            SessionMocks.idVolta = id;
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
            
            NOTICIA item = baseApp.GetItemById(SessionMocks.idVolta);
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            NOTICIA_COMENTARIO coment = new NOTICIA_COMENTARIO();
            NoticiaComentarioViewModel vm = Mapper.Map<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>(coment);
            vm.NOCO_DT_COMENTARIO = DateTime.Today;
            vm.NOCO_IN_ATIVO = 1;
            vm.NOTC_CD_ID = item.NOTC_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirComentario(NoticiaComentarioViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    NOTICIA_COMENTARIO item = Mapper.Map<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    NOTICIA not = baseApp.GetItemById(SessionMocks.idVolta);

                    item.USUARIO = null;
                    not.NOTICIA_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VerNoticia", new { id = SessionMocks.idVolta });
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
            if (SessionMocks.listaNoticia == null)
            {
                listaMaster = baseApp.GetAllItensValidos();
                SessionMocks.listaNoticia = listaMaster;
            }
            USUARIO usuario = SessionMocks.UserCredentials;
            ViewBag.Listas = SessionMocks.listaNoticia;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Notícias";

            // Indicadores
            ViewBag.Noticias = baseApp.GetAllItensValidos().Count;

            // Abre view
            objeto = new NOTICIA();
            return View(objeto);
        }

        public ActionResult Voltar()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarBase()
        {
            
            return RedirectToAction("VerNoticia", new { id = SessionMocks.idVolta });
        }

        [HttpPost]
        public ActionResult FiltrarNoticia(NOTICIA item)
        {
            
            try
            {
                // Executa a operação
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Int32 volta = baseApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                }

                // Sucesso
                listaMaster = listaObj;
                SessionMocks.listaNoticia = listaObj;
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
            
            SessionMocks.listaNoticia = null;
            return RedirectToAction("MontarTelaUsuario");
        }

        public ActionResult MostrarTudoNoticia()
        {
            
            listaMaster = baseApp.GetAllItens();
            SessionMocks.listaNoticia = listaMaster;
            return RedirectToAction("MontarTelaUsuario");
        }

       [HttpGet]
        public ActionResult MontarTelaNoticia()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaNoticia == null)
            {
                listaMaster = baseApp.GetAllItens();
                SessionMocks.listaNoticia = listaMaster;
            }
            ViewBag.Listas = SessionMocks.listaNoticia;
            ViewBag.Title = "Notícias";

            // Indicadores
            ViewBag.Noticias = SessionMocks.listaNoticia.Count;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objeto = new NOTICIA();
            SessionMocks.voltaNoticia = 1;
            return View(objeto);
        }

        public ActionResult RetirarFiltroNoticiaGeral()
        {
            
            SessionMocks.listaNoticia = null;
            if (SessionMocks.voltaNoticia == 2)
            {
                return RedirectToAction("VerCardsNoticias");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        public ActionResult MostrarTudoNoticiaGeral()
        {
            
            listaMaster = baseApp.GetAllItensAdm();
            SessionMocks.listaNoticia = listaMaster;
            if (SessionMocks.voltaNoticia == 2)
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
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Int32 volta = baseApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                }

                // Sucesso
                listaMaster = listaObj;
                SessionMocks.listaNoticia = listaObj;
                if (SessionMocks.voltaNoticia == 2)
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
            
            if (SessionMocks.voltaNoticia == 2)
            {
                return RedirectToAction("VerCardsNoticias");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        [HttpGet]
        public ActionResult IncluirNoticia()
        {
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.NOTC_DT_EMISSAO = DateTime.Today.Date;
            vm.NOTC_IN_ATIVO = 1;
            vm.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTC_NR_ACESSO = 0;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirNoticia(NoticiaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Carrega foto e processa alteracao
                    item.NOTC_AQ_FOTO = "~/Imagens/p_big2.jpg";
                    volta = baseApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    SessionMocks.listaNoticia = null;
                    if (SessionMocks.voltaNoticia == 2)
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
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            NOTICIA item = baseApp.GetItemById(id);
            objetoAntes = item;
            SessionMocks.noticia = item;
            SessionMocks.idVolta = id;
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarNoticia(NoticiaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    SessionMocks.listaNoticia = null;
                    if (SessionMocks.voltaNoticia == 2)
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
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
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
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<NOTICIA>();
                SessionMocks.listaNoticia = null;
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
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
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
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<NOTICIA>();
                SessionMocks.listaNoticia = null;
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
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoNoticia");
            }
            NOTICIA item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoNoticia");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
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
            
            return RedirectToAction("EditarNoticia", new { id = SessionMocks.idVolta });
        }

    }
}