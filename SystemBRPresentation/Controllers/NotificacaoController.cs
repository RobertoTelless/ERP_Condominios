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
using System.Collections;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class NotificacaoController : Controller
    {
        private readonly INotificacaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        NOTIFICACAO objeto = new NOTIFICACAO();
        NOTIFICACAO objetoAntes = new NOTIFICACAO();
        List<NOTIFICACAO> listaMaster = new List<NOTIFICACAO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public NotificacaoController(INotificacaoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            NOTIFICACAO item = new NOTIFICACAO();
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public JsonResult GetNotificacaoRefreshTime()
        {
            var refresh = confApp.GetById(1).CONF_NR_REFRESH_NOTIFICACAO;

            if (refresh == null)
            {
                refresh = 60;
            }

            return Json(refresh);
        }

        [HttpPost]
        public JsonResult GetNotificacaoNaoLida()
        {
            var usu = SessionMocks.UserCredentials;

            var conf = confApp.GetItemById(SessionMocks.IdAssinante);

            listaMaster = baseApp.GetNotificacaoNovas(usu.USUA_CD_ID).Where(x => x.NOTI_DT_VISTA == null && x.NOTI_DT_DATA == DateTime.Today.Date).ToList();

            if (listaMaster.Count == 1)
            {
                // Ver Notificacao
                //return RedirectToAction("VerNotificacao", new { id = listaMaster.FirstOrDefault().NOTI_CD_ID });

                var hash = new Hashtable();
                hash.Add("msg", "Você possui 1 notificação não lida");

                return Json(hash);
            }
            else if (listaMaster.Count > 1)
            {
                // Você possui x notificações não lidas
                //return RedirectToAction("MontarTelaNotificacao", new { lista = listaMaster });

                var hash = new Hashtable();
                hash.Add("msg", "Você possui " + listaMaster.Count + " notificações não lidas");

                return Json(hash);
            }
            else
            {
                return null; // Sem notificacoes
            }
        }

        [HttpPost]
        public ActionResult NovaNotificacaoClick()
        {
            
            var usu = SessionMocks.UserCredentials;

            listaMaster = baseApp.GetNotificacaoNovas(usu.USUA_CD_ID).Where(x => x.NOTI_DT_VISTA == null).ToList();

            if (listaMaster.Count == 1)
            {
                return Json(listaMaster.FirstOrDefault().NOTI_CD_ID);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpGet]
        public ActionResult VerNotificacao(Int32 id)
        {
            
            SessionMocks.idVolta = id;
            NOTIFICACAO item = baseApp.GetItemById(id);
            item.NOTI_IN_VISTA = 1;
            item.NOTI_DT_VISTA = DateTime.Now;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);

            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult VerNotificacao(NotificacaoViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    item.NOTI_IN_VISTA = 1;
                    item.NOTI_DT_VISTA = DateTime.Now;
                    objetoAntes = item;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                        
                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("CarregarBase", "BaseAdmin");
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

        public ActionResult VoltarBase()
        {
            
            return RedirectToAction("VerNotificacao", new { id = SessionMocks.idVolta });
        }

        [HttpGet]
        public ActionResult MontarTelaNotificacao(Int32? id)
        {
            
            // Carrega listas
            USUARIO usuario = SessionMocks.UserCredentials;
            if (SessionMocks.listaNotificacao == null)
            {
                listaMaster = baseApp.GetAllItensUser(usuario.USUA_CD_ID);
                SessionMocks.listaNotificacao = listaMaster;
                SessionMocks.filtroNotificacao = null;
            }

            if (id == null)
            {
                ViewBag.Listas = SessionMocks.listaNotificacao;
            }
            else
            {
                ViewBag.Listas = baseApp.GetNotificacaoNovas(usuario.USUA_CD_ID).Where(x => x.NOTI_DT_VISTA == null).ToList();
            }

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Notificações";

            // Indicadores
            ViewBag.Notificacoes = baseApp.GetNotificacaoNovas(usuario.USUA_CD_ID).Count;

            // Abre view
            objeto = new NOTIFICACAO();
            objeto.NOTI_DT_EMISSAO = DateTime.Today.Date;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarNotificacao(NOTIFICACAO item)
        {
            
            try
            {
                // Executa a operação
                List<NOTIFICACAO> listaObj = new List<NOTIFICACAO>();
                Int32 volta = baseApp.ExecuteFilter(item.NOTI_NM_TITULO, item.NOTI_DT_EMISSAO, item.NOTI_TX_TEXTO, out listaObj);
                SessionMocks.filtroNotificacao = item;

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                }

                // Sucesso
                listaMaster = listaObj;
                SessionMocks.listaNotificacao = listaObj;
                return RedirectToAction("MontarTelaNotificacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaNotificacao");
            }
        }

        public ActionResult RetirarFiltroNotificacao()
        {
            
            SessionMocks.listaNotificacao = null;
            return RedirectToAction("MontarTelaNotificacao");
        }

        public ActionResult MostrarTudoNotificacao()
        {
            
            USUARIO usuario = SessionMocks.UserCredentials;
            listaMaster = baseApp.GetAllItensUser(usuario.USUA_CD_ID);
            SessionMocks.listaNotificacao = listaMaster;
            return RedirectToAction("MontarTelaNotificacao");
        }

        [HttpPost]
        public ActionResult UploadFileNotificacao()
        {
            
            return RedirectToAction("MontarTelaNotificacao");
        }

        [HttpGet]
        public ActionResult VerAnexoNotificacao(Int32 id)
        {
            
            // Prepara view
            NOTIFICACAO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadNotificacao(Int32 id)
        {
            NOTIFICACAO_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.NOAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        public ActionResult VoltarAnexoNotificacao()
        {
            
            return RedirectToAction("VerNotificacao", new { id = SessionMocks.idVolta });
        }

        [HttpGet]
        public ActionResult MontarTelaNotificacaoGeral()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaNotificacao == null)
            {
                listaMaster = baseApp.GetAllItens();
                SessionMocks.listaNotificacao = listaMaster;
                SessionMocks.filtroNotificacao = null;
            }
            ViewBag.Listas = SessionMocks.listaNotificacao.Where(p => p.NOTI_DT_EMISSAO == DateTime.Now.Date).ToList();
            ViewBag.Title = "Notificações";
            ViewBag.Cats = new SelectList(baseApp.GetAllCategorias(), "CANO_CD_ID", "CANO_NM_NOME");

            // Indicadores
            ViewBag.Nots = SessionMocks.listaNotificacao.Count;
            ViewBag.NotsHoje = SessionMocks.listaNotificacao.Where(p => p.NOTI_DT_EMISSAO == DateTime.Today.Date).ToList().Count;

            // Abre view
            objeto = new NOTIFICACAO();
            return View(objeto);
        }

        public ActionResult RetirarFiltroNotificacaoGeral()
        {
            
            SessionMocks.listaNotificacao = null;
            listaMaster = new List<NOTIFICACAO>();
            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        public ActionResult MostrarTudoNotificacaoGeral()
        {
            
            listaMaster = baseApp.GetAllItensAdm();
            SessionMocks.listaNotificacao = listaMaster;
            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        [HttpPost]
        public ActionResult FiltrarNotificacaoGeral(NOTIFICACAO item)
        {
            
            try
            {
                try
                {
                    // Executa a operação
                    List<NOTIFICACAO> listaObj = new List<NOTIFICACAO>();
                    Int32 volta = baseApp.ExecuteFilter(item.NOTI_NM_TITULO, item.NOTI_DT_EMISSAO, item.NOTI_TX_TEXTO, out listaObj);
                    SessionMocks.filtroNotificacao = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    }

                    // Sucesso
                    listaMaster = listaObj;
                    SessionMocks.listaNotificacao = listaObj;
                    return RedirectToAction("MontarTelaNotificacaoGeral");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaNotificacaoGeral");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaNotificacaoGeral");
            }
        }

        public ActionResult VoltarBaseNotificacao()
        {
            
            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        [HttpGet]
        public ActionResult IncluirNotificacao()
        {
            
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Cats = new SelectList(baseApp.GetAllCategorias(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");

            // Prepara view
            NOTIFICACAO item = new NOTIFICACAO();
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            vm.NOTI_DT_EMISSAO = DateTime.Today.Date;
            vm.NOTI_IN_ATIVO = 1;
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTI_IN_NIVEL = 1;
            vm.NOTI_IN_VISTA = 0;          
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirNotificacao(NotificacaoViewModel vm)
        {
            
            var result = new Hashtable();

            ViewBag.Cats = new SelectList(baseApp.GetAllCategorias(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante + "/Notificacao/" + item.NOTI_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    
                    // Sucesso
                    listaMaster = new List<NOTIFICACAO>();
                    SessionMocks.listaNotificacao = null;

                    SessionMocks.idVolta = item.NOTI_CD_ID;
                    result.Add("id", item.NOTI_CD_ID);
                    return Json(result);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    result.Add("error", ex.Message);
                    return Json(result);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarNotificacao(Int32 id)
        {
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            ViewBag.Cats = new SelectList(baseApp.GetAllCategorias(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");

            NOTIFICACAO item = baseApp.GetItemById(id);
            objetoAntes = item;
            SessionMocks.notificacao = item;
            SessionMocks.idVolta = id;
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarNotificacao(NotificacaoViewModel vm)
        {
            
            ViewBag.Cats = new SelectList(baseApp.GetAllCategorias(), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usus = new SelectList(usuApp.GetAllItens(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<NOTIFICACAO>();
                    SessionMocks.listaNotificacao = null;
                    return RedirectToAction("MontarTelaNotificacaoGeral");
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
        public ActionResult ExcluirNotificacao(Int32 id)
        {
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            NOTIFICACAO item = baseApp.GetItemById(id);
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirNotificacao(NotificacaoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<NOTIFICACAO>();
                SessionMocks.listaNotificacao = null;
                return RedirectToAction("MontarTelaNotificacaoGeral");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarNotificacao(Int32 id)
        {
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "USU")
                {
                    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            NOTIFICACAO item = baseApp.GetItemById(id);
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarNotificacao(NotificacaoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<NOTIFICACAO>();
                SessionMocks.listaNotificacao = null;
                return RedirectToAction("MontarTelaNotificacaoGeral");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoNotificacaoGeral(Int32 id)
        {
            
            // Prepara view
            NOTIFICACAO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoNotificacaoGeral()
        {
            
            return RedirectToAction("EditarNotificacao", new { id = SessionMocks.idVolta });
        }

        [HttpPost]
        public ActionResult UploadFileNotificacaoGeral(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoNoticia");
            }

            NOTIFICACAO item = baseApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoNotificacaoGeral");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante + "/Notificacao/" + item.NOTI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            NOTIFICACAO_ANEXO foto = new NOTIFICACAO_ANEXO();
            foto.NOAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.NOAN_DT_ANEXO = DateTime.Today;
            foto.NOAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.NOAN_IN_TIPO = tipo;
            foto.NOAN_NM_TITULO = fileName;
            foto.NOTI_CD_ID = item.NOTI_CD_ID;

            item.NOTIFICACAO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoNotificacaoGeral");
        }

        [HttpPost]
        public JsonResult UploadFileNotificacao_Inclusao(IEnumerable<HttpPostedFileBase> files, Int32 perfil)
        {
            var count = 0;

            if (perfil == 0)
            {
                count++;
            }

            foreach (var file in files)
            {
                if (count == 0)
                {
                    //UploadFotoCliente(file);

                    //count++;
                }
                else
                {
                    UploadFileNotificacaoGeral(file);
                }
            }

            return Json("1"); // VoltarAnexoCliente
        }

        [HttpGet]
        public ActionResult SlideShowNotificacao()
        {
            
            // Prepara view
            NOTIFICACAO item = baseApp.GetItemById(SessionMocks.idVolta);
            objetoAntes = item;
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            return View(vm);
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "NotificacaoLista" + "_" + data + ".pdf";
            List<NOTIFICACAO> lista = SessionMocks.listaNotificacao;
            NOTIFICACAO filtro = SessionMocks.filtroNotificacao;
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Notificações - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 50f, 120f, 120f, 50f, 50f, 40f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Notificações selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 6;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Destinatário", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Foto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Título", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Validade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Lida?", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (NOTIFICACAO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_NOTIFICACAO.CANO_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.USUARIO.USUA_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.USUARIO.USUA_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.USUARIO.USUA_AQ_FOTO));
                    image.ScaleAbsolute(20, 20);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.NOTI_NM_TITULO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.NOTI_DT_EMISSAO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.NOTI_DT_VALIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.NOTI_DT_VALIDADE.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.NOTI_IN_VISTA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Sim", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Não", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.NOTI_NM_TITULO != null)
                {
                    parametros += "Título: " + filtro.NOTI_NM_TITULO;
                    ja = 1;
                }
                if (filtro.NOTI_DT_EMISSAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data: " + filtro.NOTI_DT_EMISSAO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data: " + filtro.NOTI_DT_EMISSAO.Value.ToShortDateString();
                    }
                }
                if (filtro.NOTI_TX_TEXTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Conteúdo: " + filtro.NOTI_TX_TEXTO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Conteúdo: " + filtro.NOTI_TX_TEXTO;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaNotificacaoGeral");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            // Prepara geração
            NOTIFICACAO aten = baseApp.GetItemById(SessionMocks.idVolta);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Notificacao_" + aten.NOTI_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Notificação - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            
            cell = new PdfPCell(new Paragraph("Destinatário: " + aten.USUARIO.USUA_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_NOTIFICACAO.CANO_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Título: " + aten.NOTI_NM_TITULO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.NOTI_DT_EMISSAO != null)
            {
                cell = new PdfPCell(new Paragraph("Emissão: " + aten.NOTI_DT_EMISSAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Emissão: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.NOTI_DT_VALIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Validade: " + aten.NOTI_DT_VALIDADE.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Validade: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.NOTI_DT_VISTA != null)
            {
                cell = new PdfPCell(new Paragraph("Data de Visualização: " + aten.NOTI_DT_VISTA.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data de Visualização: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Conteúdo: " + aten.NOTI_TX_TEXTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoNotificacao");
        }

    }
}