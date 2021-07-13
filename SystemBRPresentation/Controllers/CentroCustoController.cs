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
    public class CentroCustoController : Controller
    {
        private readonly ICentroCustoAppService ccApp;
        private readonly ILogAppService logApp;
        private readonly IGrupoAppService gruApp;
        private readonly ISubgrupoAppService sgApp;

        private String msg;
        private Exception exception;
        CENTRO_CUSTO objCC = new CENTRO_CUSTO();
        CENTRO_CUSTO objCCAntes = new CENTRO_CUSTO();
        List<CENTRO_CUSTO> listaMasterCC = new List<CENTRO_CUSTO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public CentroCustoController(ICentroCustoAppService ccApps, ILogAppService logApps, IGrupoAppService gruApps, ISubgrupoAppService sgApps)
        {
            ccApp = ccApps;
            logApp = logApps;
            gruApp = gruApps;
            sgApp = sgApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            CENTRO_CUSTO item = new CENTRO_CUSTO();
            CentroCustoViewModel vm = Mapper.Map<CENTRO_CUSTO, CentroCustoViewModel>(item);
            return View(vm);
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
            
            listaMasterCC = new List<CENTRO_CUSTO>();
            SessionMocks.cliente = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

       [HttpGet]
        public ActionResult MontarTelaCC()
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
            if (SessionMocks.listaCC == null)
            {
                listaMasterCC = ccApp.GetAllItens();
                SessionMocks.listaCC = listaMasterCC;
            }
            ViewBag.Listas = SessionMocks.listaCC;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Centros de Custos";
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens().OrderBy(x => x.GRUP_NM_NOME).ToList<GRUPO>(), "GRUP_CD_ID", "GR_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens().OrderBy(x => x.SUBG_NM_NOME).ToList<SUBGRUPO>(), "SUBG_CD_ID", "SUBG_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.Tipos = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");


            // Indicadores
            ViewBag.Cats = SessionMocks.listaCC.Count;

            // Abre view
            objCC = new CENTRO_CUSTO();
            return View(objCC);
        }

        public ActionResult RetirarFiltroCC()
        {
            
            SessionMocks.listaCC = null;
            SessionMocks.filtroCC = null;
            return RedirectToAction("MontarTelaCC");

            //if (SessionMocks.flagVoltaCC == 1)
            //{
            //    if (SessionMocks.voltaCC == 2)
            //    {
            //        return RedirectToAction("VerCardsProduto");
            //    }
            //    return RedirectToAction("MontarTelaCC");
            //}
            //else
            //{
            //    return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
            //}
        }

        public ActionResult MostrarTudoCC()
        {
            
            listaMasterCC = ccApp.GetAllItensAdm();
            SessionMocks.listaCC = listaMasterCC;
            return RedirectToAction("MontarTelaCC");
        }

        [HttpPost]
        public ActionResult FiltrarCC(CENTRO_CUSTO item)
        {
            
            try
            {
                // Executa a operação
                List<CENTRO_CUSTO> listaObj = new List<CENTRO_CUSTO>();
                SessionMocks.filtroCC = item;
                Int32 volta = ccApp.ExecuteFilter(item.GRUP_CD_ID, item.SUBG_CD_ID, item.CECU_IN_TIPO, item.CECU_IN_MOVTO, item.CECU_NR_NUMERO, item.CECU_NM_NOME, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterCC = listaObj;
                SessionMocks.listaCC = listaObj;

                //if (SessionMocks.voltaCC == 2)
                //{
                //    return RedirectToAction("VerCardsCC");
                //}
                //if (SessionMocks.voltaConsulta == 2)
                //{
                //    return RedirectToAction("VerCC");
                //}
                //if (SessionMocks.voltaConsulta == 3)
                //{
                //    return RedirectToAction("VerCC");
                //}

                return RedirectToAction("MontarTelaCC");

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCC");
            }
        }

        // Filtro em cascata de subgrupo
        [HttpPost]
        public JsonResult FiltroSubGrupoCC(Int32? id)
        {
            var listaSubFiltrada = new List<SUBGRUPO>();

            // Filtro para caso o placeholder seja selecionado
            if (id == null)
            {
                listaSubFiltrada = sgApp.GetAllItens();
            }
            else
            {
                listaSubFiltrada = sgApp.GetAllItens().Where(x => x.GRUP_CD_ID == id).ToList();
            }

            return Json(listaSubFiltrada.Select(x => new { x.SUBG_CD_ID, x.SUBG_NM_EXIBE }));
        }

        public ActionResult VoltarBaseCC()
        {
            SessionMocks.CentroCustos = ccApp.GetAllItens();
            return RedirectToAction("MontarTelaCC");
        }

        [HttpGet]
        public ActionResult IncluirCC()
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

            // Prepara listas
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(), "SUBG_CD_ID", "SUBG_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CENTRO_CUSTO item = new CENTRO_CUSTO();
            CentroCustoViewModel vm = Mapper.Map<CENTRO_CUSTO, CentroCustoViewModel>(item);
            vm.CECU_IN_ATIVO = 1;
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCC(CentroCustoViewModel vm)
        {
            
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(), "SUBG_CD_ID", "SUBG_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CENTRO_CUSTO item = Mapper.Map<CentroCustoViewModel, CENTRO_CUSTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = ccApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterCC = new List<CENTRO_CUSTO>();
                    SessionMocks.listaCC = null;
                    return RedirectToAction("MontarTelaCC");
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
        public ActionResult EditarCC(Int32 id)
        {
            // Verifica se tem usuario logado
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

            // Prepara listas
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(), "SUBG_CD_ID", "SUBG_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");

            // Prepara view
            CENTRO_CUSTO item = ccApp.GetItemById(id);
            objCCAntes = item;
            SessionMocks.ccusto = item;
            SessionMocks.idVolta = id;
            CentroCustoViewModel vm = Mapper.Map<CENTRO_CUSTO, CentroCustoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCC(CentroCustoViewModel vm)
        {
            
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(), "GRUP_CD_ID", "GR_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(), "SUBG_CD_ID", "SUBG_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Crédito", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Débito", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CENTRO_CUSTO item = Mapper.Map<CentroCustoViewModel, CENTRO_CUSTO>(vm);
                    Int32 volta = ccApp.ValidateEdit(item, objCCAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCC = new List<CENTRO_CUSTO>();
                    SessionMocks.listaCC = null;
                    return RedirectToAction("MontarTelaCC");
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
        public ActionResult ExcluirCC(Int32 id)
        {
            // Verifica se tem usuario logado
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
            CENTRO_CUSTO item = ccApp.GetItemById(id);
            objCCAntes = SessionMocks.ccusto;
            item.CECU_IN_ATIVO = 0;
            item.GRUPO = null;
            item.SUBGRUPO = null;
            item.ASSINANTE = null;
            Int32 volta = ccApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture));
            }
            listaMasterCC = new List<CENTRO_CUSTO>();
            SessionMocks.listaCC = null;
            return RedirectToAction("MontarTelaCC");
        }

        [HttpGet]
        public ActionResult ReativarCC(Int32 id)
        {
            // Verifica se tem usuario logado
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
            CENTRO_CUSTO item = ccApp.GetItemById(id);
            objCCAntes = SessionMocks.ccusto;
            item.CECU_IN_ATIVO = 1;
            item.GRUPO = null;
            item.SUBGRUPO = null;
            item.ASSINANTE = null;
            Int32 volta = ccApp.ValidateReativar(item, usu);
            listaMasterCC = new List<CENTRO_CUSTO>();
            SessionMocks.listaCC = null;
            return RedirectToAction("MontarTelaCC");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "CCLista" + "_" + data + ".pdf";
            List<CENTRO_CUSTO> lista = SessionMocks.listaCC;
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(6);
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

            cell = new PdfPCell(new Paragraph("Centros de Custo - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 6;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Número", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Grupo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Subgrupo", meuFont))
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
            cell.Colspan = 2;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tipo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Movimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CENTRO_CUSTO item in lista)
            {
                cell = new PdfPCell(new Paragraph((item.GRUPO == null ? "-" : item.GRUPO.GRUP_NR_NUMERO) + "." + (item.SUBGRUPO == null ? "-" : item.SUBGRUPO.SUBG_NR_NUMERO) + "." + item.CECU_NR_NUMERO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.GRUPO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.GRUPO.GRUP_NM_NOME, meuFont))
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
                if (item.SUBGRUPO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.SUBGRUPO.SUBG_NM_NOME, meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CECU_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                table.AddCell(cell);
                if (item.CECU_IN_TIPO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Crédito", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Débito", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CECU_IN_MOVTO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Todos", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else if (item.CECU_IN_MOVTO == 2)
                {
                    cell = new PdfPCell(new Paragraph("Compra", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Venda", meuFont))
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

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaCC");
        }
    }
}