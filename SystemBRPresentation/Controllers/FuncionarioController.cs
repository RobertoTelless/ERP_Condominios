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
using Org.BouncyCastle.Crypto.Agreement.Srp;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class FuncionarioController : Controller
    {
        private readonly ILogAppService logApp;
        private readonly IFuncionarioAppService funApp;
        private readonly IFornecedorAppService fornApp;

        private String msg;
        private Exception exception;
        String extensao = String.Empty;
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        FUNCIONARIO objetoFun = new FUNCIONARIO();
        FUNCIONARIO objetoFunAntes = new FUNCIONARIO();
        List<FUNCIONARIO> listaMasterFun = new List<FUNCIONARIO>();

        public FuncionarioController(ILogAppService logApps, IFuncionarioAppService funApps, IFornecedorAppService fornApps)
        {
            logApp = logApps;
            funApp = funApps;
            fornApp = fornApps;
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
            listaMasterFun = new List<FUNCIONARIO>();
            SessionMocks.funcionario = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaFunc()
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            usuario = SessionMocks.UserCredentials;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaFunc == null)
            {
                listaMasterFun = funApp.GetAllItens();
                SessionMocks.listaFunc = listaMasterFun;
            }
            ViewBag.Listas = SessionMocks.listaFunc;
            ViewBag.Title = "Funcionários";
            ViewBag.Situacao = new SelectList(SessionMocks.Situacoes, "SITU_CD_ID", "SITU_NM_NOME");
            ViewBag.Funcao = new SelectList(SessionMocks.Funcoes, "FNCA_CD_ID", "FNCA_NM_NOME");

            // Indicadores
            ViewBag.Func = listaMasterFun.Count;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objetoFun = new FUNCIONARIO();
            SessionMocks.voltaFunc = 1;
            return View(objetoFun);
        }

        public ActionResult RetirarFiltroFunc()
        {
            
            SessionMocks.listaFunc = null;
            if (SessionMocks.voltaFunc == 2)
            {
                return RedirectToAction("VerCardsFunc");
            }
            return RedirectToAction("MontarTelaFunc");
        }

        public ActionResult MostrarTudoFunc()
        {
            
            listaMasterFun = funApp.GetAllItensAdm();
            SessionMocks.listaFunc = listaMasterFun;
            if (SessionMocks.voltaFunc == 2)
            {
                return RedirectToAction("VerCardsFunc");
            }
            return RedirectToAction("MontarTelaFunc");
        }

        [HttpPost]
        public ActionResult FiltrarFunc(FUNCIONARIO item)
        {
            
            try
            {
                // Executa a operação
                SessionMocks.filtroFunc = item;
                List<FUNCIONARIO> listaObj = new List<FUNCIONARIO>();
                Int32 volta = funApp.ExecuteFilter(item.SITU_CD_ID, item.FUNC_NM_NOME, item.FUNC_NR_CPF, item.FUNC_NR_RG, item.FNCA_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0017", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterFun = listaObj;
                SessionMocks.listaFunc = listaObj;
                if (SessionMocks.voltaFunc == 2)
                {
                    return RedirectToAction("VerCardsFunc");
                }
                return RedirectToAction("MontarTelaFunc");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaFunc");
            }
        }

        public ActionResult VoltarBaseFunc()
        {
            
            SessionMocks.listaFunc = null;
            if (SessionMocks.voltaFunc == 2)
            {
                return RedirectToAction("VerCardsFunc");
            }
            return RedirectToAction("MontarTelaFunc");
        }

        [HttpGet]
        public ActionResult IncluirFunc()
        {
            
            // Prepara listas
            ViewBag.Situacao = new SelectList(SessionMocks.Situacoes, "SITU_CD_ID", "SITU_NM_NOME");
            ViewBag.Sexo = new SelectList(SessionMocks.Sexos, "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.Escol = new SelectList(SessionMocks.Escolaridades, "ESCO_CD_ID", "ESCO_NM_NOME");
            ViewBag.UF = new SelectList(SessionMocks.UFs, "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Funcao = new SelectList(SessionMocks.Funcoes, "FNCA_CD_ID", "FNCA_NM_NOME");

            // Prepara vi
            USUARIO usuario = SessionMocks.UserCredentials;
            FUNCIONARIO item = new FUNCIONARIO();
            FuncionarioViewModel vm = Mapper.Map<FUNCIONARIO, FuncionarioViewModel>(item);
            vm.FUNC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirFunc(FuncionarioViewModel vm)
        {
            
            ViewBag.Situacao = new SelectList(SessionMocks.Situacoes, "SITU_CD_ID", "SITU_NM_NOME");
            ViewBag.Sexo = new SelectList(SessionMocks.Sexos, "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.Escol = new SelectList(SessionMocks.Escolaridades, "ESCO_CD_ID", "ESCO_NM_NOME");
            ViewBag.UF = new SelectList(SessionMocks.UFs, "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Funcao = new SelectList(SessionMocks.Funcoes, "FNCA_CD_ID", "FNCA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FUNCIONARIO item = Mapper.Map<FuncionarioViewModel, FUNCIONARIO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = funApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0077", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Carrega foto e processa alteracao
                    item.FUNC_AQ_FOTO = "~/Imagens/Base/FotoBase.jpg";
                    volta = funApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Funcionario/" + item.FUNC_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Funcionario/" + item.FUNC_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterFun = new List<FUNCIONARIO>();
                    SessionMocks.listaFunc = null;
                    if (SessionMocks.voltaFunc == 2)
                    {
                        return RedirectToAction("VerCardsFunc");
                    }

                    SessionMocks.idVolta = item.FUNC_CD_ID;
                    return View(vm);
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
        public ActionResult EditarFunc(Int32 id)
        {
            
            // Prepara view
            ViewBag.Situacao = new SelectList(SessionMocks.Situacoes, "SITU_CD_ID", "SITU_NM_NOME");
            ViewBag.Sexo = new SelectList(SessionMocks.Sexos, "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.Escol = new SelectList(SessionMocks.Escolaridades, "ESCO_CD_ID", "ESCO_NM_NOME");
            ViewBag.UF = new SelectList(SessionMocks.UFs, "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Funcao = new SelectList(SessionMocks.Funcoes, "FNCA_CD_ID", "FNCA_NM_NOME");

            FUNCIONARIO item = funApp.GetItemById(id);
            objetoFunAntes = item;
            SessionMocks.funcionario = item;
            SessionMocks.idVolta = id;
            FuncionarioViewModel vm = Mapper.Map<FUNCIONARIO, FuncionarioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarFunc(FuncionarioViewModel vm)
        {
            
            ViewBag.Situacao = new SelectList(SessionMocks.Situacoes, "SITU_CD_ID", "SITU_NM_NOME");
            ViewBag.Sexo = new SelectList(SessionMocks.Sexos, "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.Escol = new SelectList(SessionMocks.Escolaridades, "ESCO_CD_ID", "ESCO_NM_NOME");
            ViewBag.UF = new SelectList(SessionMocks.UFs, "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Funcao = new SelectList(SessionMocks.Funcoes, "FNCA_CD_ID", "FNCA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    FUNCIONARIO item = Mapper.Map<FuncionarioViewModel, FUNCIONARIO>(vm);
                    Int32 volta = funApp.ValidateEdit(item, objetoFunAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFun = new List<FUNCIONARIO>();
                    SessionMocks.listaFunc = null;
                    if (SessionMocks.voltaFunc == 2)
                    {
                        return RedirectToAction("VerCardsFunc");
                    }
                    return RedirectToAction("MontarTelaFunc");
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
        public ActionResult ExcluirFunc(Int32 id)
        {
            
            // Prepara view
            FUNCIONARIO item = funApp.GetItemById(id);
            FuncionarioViewModel vm = Mapper.Map<FUNCIONARIO, FuncionarioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirFunc(FuncionarioViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                FUNCIONARIO item = Mapper.Map<FuncionarioViewModel, FUNCIONARIO>(vm);
                Int32 volta = funApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0078", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMasterFun = new List<FUNCIONARIO>();
                SessionMocks.listaFunc = null;
                return RedirectToAction("MontarTelaFunc");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarFunc(Int32 id)
        {
            
            // Prepara view
            FUNCIONARIO item = funApp.GetItemById(id);
            FuncionarioViewModel vm = Mapper.Map<FUNCIONARIO, FuncionarioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarFunc(FuncionarioViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                FUNCIONARIO item = Mapper.Map<FuncionarioViewModel, FUNCIONARIO>(vm);
                Int32 volta = funApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterFun = new List<FUNCIONARIO>();
                SessionMocks.listaFunc = null;
                return RedirectToAction("MontarTelaFunc");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public ActionResult VerCardsFunc()
        {
            
            // Carrega listas
            if (SessionMocks.listaFunc == null)
            {
                listaMasterFun = funApp.GetAllItens();
                SessionMocks.listaFunc = listaMasterFun;
            }
            ViewBag.Listas = SessionMocks.listaFunc;
            ViewBag.Title = "Funcionários";
            ViewBag.Situacao = new SelectList(SessionMocks.Situacoes, "SITU_CD_ID", "SITU_NM_NOME");
            ViewBag.Funcao = new SelectList(SessionMocks.Funcoes, "FNCA_CD_ID", "FNCA_NM_NOME");

            // Indicadores
            ViewBag.Func = listaMasterFun.Count;

            // Abre view
            objetoFun = new FUNCIONARIO();
            SessionMocks.voltaFunc = 2;
            return View(objetoFun);
        }

        [HttpGet]
        public ActionResult VerAnexoFunc(Int32 id)
        {
            
            // Prepara view
            FUNCIONARIO_ANEXO item = funApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoFunc()
        {
            
            return RedirectToAction("EditarFunc", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadFunc(Int32 id)
        {
            FUNCIONARIO_ANEXO item = funApp.GetAnexoById(id);
            String arquivo = item.FUAN_AQ_ARQUIVO;
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

        [HttpPost]
        public ActionResult UploadFileFunc(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFunc");
            }

            FUNCIONARIO item = funApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);

            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFunc");
            }

            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Funcionario/" + item.FUNC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            FUNCIONARIO_ANEXO foto = new FUNCIONARIO_ANEXO();
            foto.FUAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.FUAN_DT_ANEXO = DateTime.Today;
            foto.FUAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.FUAN_IN_TIPO = tipo;
            foto.FUAN_NM_TITULO = fileName;
            foto.FUNC_CD_ID = item.FUNC_CD_ID;

            item.FUNCIONARIO_ANEXO.Add(foto);
            objetoFunAntes = item;
            Int32 volta = funApp.ValidateEdit(item, objetoFunAntes);
            return RedirectToAction("VoltarAnexoFunc");
        }

        [HttpPost]
        public JsonResult UploadFileFunc_Inclusao(IEnumerable<HttpPostedFileBase> files, Int32 perfil)
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
                    UploadFotoFunc(file);

                    count++;
                }
                else
                {
                    UploadFileFunc(file);
                }
            }

            return Json("1"); // VoltarAnexoCliente
        }

        [HttpPost]
        public ActionResult UploadFotoFunc(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFunc");
            }

            // Recupera arquivo
            FUNCIONARIO item = funApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoFunc");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Funcionario/" + item.FUNC_CD_ID.ToString() + "/Fotos/";
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
                item.FUNC_AQ_FOTO = "~" + caminho + fileName;
                objetoFunAntes = item;
                Int32 volta = funApp.ValidateEdit(item, objetoFunAntes);
            }
            else
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
            return RedirectToAction("VoltarAnexoFunc");
        }

        [HttpGet]
        public ActionResult SlideShowFunc()
        {
            
            // Prepara view
            FUNCIONARIO item = funApp.GetItemById(SessionMocks.idVolta);
            objetoFunAntes = item;
            FuncionarioViewModel vm = Mapper.Map<FUNCIONARIO, FuncionarioViewModel>(item);
            return View(vm);
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "FuncLista" + "_" + data + ".pdf";
            List<FUNCIONARIO> lista = SessionMocks.listaFunc;
            FUNCIONARIO filtro = SessionMocks.filtroFunc;
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

            cell = new PdfPCell(new Paragraph("Funcionários - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 60f, 150f, 60f, 60f, 100f, 60f, 60f, 80f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Funcionários selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 9;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Situação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CPF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("RG", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Função", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Telefone", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular", meuFont))
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

            foreach (FUNCIONARIO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.SITUACAO.SITU_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.FUNC_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.FUNC_NR_CPF, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.FUNC_NR_RG, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.FUNCAO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FUNCAO.FNCA_NM_NOME, meuFont))
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
                if (item.FUNC_NR_TELEFONE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FUNC_NR_TELEFONE, meuFont))
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
                if (item.FINC_NR_CELULAR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FINC_NR_CELULAR, meuFont))
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
                if (System.IO.File.Exists(Server.MapPath(item.FUNC_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.FUNC_AQ_FOTO));
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
                if (filtro.SITU_CD_ID > 0)
                {
                    parametros += "Situação: " + filtro.SITU_CD_ID;
                    ja = 1;
                }
                if (filtro.FUNC_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.FUNC_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Nome: " + filtro.FUNC_NM_NOME;
                    }
                }
                if (filtro.FUNC_NR_CPF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CPF: " + filtro.FUNC_NR_CPF;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CPF: " + filtro.FUNC_NR_CPF;
                    }
                }
                if (filtro.FUNC_NR_RG != null)
                {
                    if (ja == 0)
                    {
                        parametros += "RG: " + filtro.FUNC_NR_RG;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e RG: " + filtro.FUNC_NR_RG;
                    }
                }
                if (filtro.FNCA_CD_ID > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Função: " + filtro.FNCA_CD_ID.ToString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Função: " + filtro.FNCA_CD_ID.ToString();
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

            return RedirectToAction("MontarTelaFunc");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            // Prepara geração
            FUNCIONARIO aten = funApp.GetItemById(SessionMocks.idVolta);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Funcionario_" + aten.FUNC_CD_ID.ToString() + "_" + data + ".pdf";
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

            cell = new PdfPCell(new Paragraph("Funcionário - Detalhes", meuFont2))
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
            
            cell = new PdfPCell();
            cell.Border = 0;
            cell.Colspan = 1;
            image = Image.GetInstance(Server.MapPath(aten.FUNC_AQ_FOTO));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Situação: " + aten.SITUACAO.SITU_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Função: " + aten.FUNCAO.FNCA_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.FUNC_NR_SALARIO != null)
            {
                cell = new PdfPCell(new Paragraph("Salário: " + CrossCutting.Formatters.DecimalFormatter(aten.FUNC_NR_SALARIO.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Salário: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.FUNC_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CPF: " + aten.FUNC_NR_CPF, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("RG: " + aten.FUNC_NR_RG, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.FUNC_DT_EMISSAO_RG != null)
            {
                cell = new PdfPCell(new Paragraph("Emissão: " + aten.FUNC_DT_EMISSAO_RG.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Emissão: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Órgão Emissor: " + aten.FUNC_NM_ORGAO_RG, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.FUNC_UF_FG_ID != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF1.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.FUNC_DT_NASCIMENTO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Nascimento: " + aten.FUNC_DT_NASCIMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Nascimento: " , meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.SEXO != null)
            {
                cell = new PdfPCell(new Paragraph("Sexo: " + aten.SEXO.SEXO_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Sexo: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.ESCOLARIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Escolaridade: " + aten.ESCOLARIDADE.ESCO_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Escolaridade: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.FUNC_DT_ADMISSAO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Admissão: " + aten.FUNC_DT_ADMISSAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Admissão: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.FUNC_DT_DEMISSAO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Demissão: " + aten.FUNC_DT_DEMISSAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Demissão: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("Motivo: " + aten.FUNC_NM_MOTIVO_DEMISSAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Endereços
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Endereço", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço: " + aten.FUNC_NM_ENDERECO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Bairro: " + aten.FUNC_NM_BAIRRO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade: " + aten.FUNC_NM_CIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.UF != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("CEP: " + aten.FUNC_NR_CEP, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Contatos
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Contatos", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("E-Mail: " + aten.FUNC_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Telefone: " + aten.FUNC_NR_TELEFONE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular: " + aten.FINC_NR_CELULAR, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Documentação
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Documentação", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("NIS: " + aten.FUNC_NR_NIS, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("CTPS: " + aten.FUNC_NR_CTPS, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Série: " + aten.FUNC_NR_CTPS_SERIE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.FUNC_DT_CTPS_EMISSAO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Emissão: " + aten.FUNC_DT_CTPS_EMISSAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Emissão: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.UF1 != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Título Eleitor: " + aten.FUNC_NR_TITULO_ELEITOR, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Zona: " + aten.FUNC_NR_ZONA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Seção: " + aten.FUNC_NR_SECAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.FUNC_DT_EMISSAO_TITULO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Emissão: " + aten.FUNC_DT_EMISSAO_TITULO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Emissão: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Município: " + aten.FUNC_NM_MUNICIPIO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cert.Reservista: " + aten.FUNC_NR_RESERVISTA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Situação: " + aten.FUNC_NM_RESERVISTA_SITUACAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("O.M.: " + aten.FUNC_NM_RESERVISTA_OM, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.FUNC_DT_RESERVISTA_EMISSAO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Emissão: " + aten.FUNC_DT_RESERVISTA_EMISSAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Emissão: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados Pessoais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Pessoais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Profissão: " + aten.FUNC_NM_PROFISSAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome do Pai: " + aten.FUNC_NM_PAI, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome da Mãe: " + aten.FUNC_NM_MAE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome do Cônjuge: " + aten.FUNC_NM_CONJUGE, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Naturalidade: " + aten.FUNC_NM_NATURALIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.UF2 != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("Nacionalidade: " + aten.FUNC_NM_NACIONALIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" " ,meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.FUNC_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoFunc");
        }

    }
}