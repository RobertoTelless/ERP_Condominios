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
using Newtonsoft.Json.Linq;
using System.Collections;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class EstoqueController : Controller
    {
        private readonly IProdutoAppService prodApp;
        private readonly IMateriaPrimaAppService insApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly ICategoriaMateriaAppService ciApp;
        private readonly IFilialAppService filApp;
        private readonly IMovimentoEstoqueMateriaAppService moemApp;
        private readonly IMovimentoEstoqueProdutoAppService moepApp;
        private readonly ISubcategoriaMateriaAppService scmpApp;
        private readonly IProdutoEstoqueFilialAppService pefApp;
        private readonly IMateriaEstoqueFilialAppService mefApp;
        private readonly IFornecedorAppService fornApp;

        private String msg;
        private Exception exception;
        PRODUTO objetoProd = new PRODUTO();
        PRODUTO objetoProdAntes = new PRODUTO();
        List<PRODUTO> listaMasterProd = new List<PRODUTO>();
        MATERIA_PRIMA objetoIns = new MATERIA_PRIMA();
        MATERIA_PRIMA objetoInsAntes = new MATERIA_PRIMA();
        List<MATERIA_PRIMA> listaMasterIns = new List<MATERIA_PRIMA>();
        MOVIMENTO_ESTOQUE_PRODUTO objetoMOVP = new MOVIMENTO_ESTOQUE_PRODUTO();
        MOVIMENTO_ESTOQUE_PRODUTO objetoMOVPAntes = new MOVIMENTO_ESTOQUE_PRODUTO();
        List<MOVIMENTO_ESTOQUE_PRODUTO> listaMasterMOEP = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
        MOVIMENTO_ESTOQUE_MATERIA_PRIMA objetoMOVM = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
        MOVIMENTO_ESTOQUE_MATERIA_PRIMA objetoMOVMAntes = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
        List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA> listaMasterMOEM = new List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA>();

        PRODUTO_ESTOQUE_FILIAL objetoProdFili = new PRODUTO_ESTOQUE_FILIAL();
        PRODUTO_ESTOQUE_FILIAL objetoProdFiliAntes = new PRODUTO_ESTOQUE_FILIAL();
        List<PRODUTO_ESTOQUE_FILIAL> listaMasterProdFili = new List<PRODUTO_ESTOQUE_FILIAL>();

        MATERIA_PRIMA_ESTOQUE_FILIAL objetoInsFili = new MATERIA_PRIMA_ESTOQUE_FILIAL();
        MATERIA_PRIMA_ESTOQUE_FILIAL objetoInsAntesFili = new MATERIA_PRIMA_ESTOQUE_FILIAL();
        List<MATERIA_PRIMA_ESTOQUE_FILIAL> listaMasterInsFili = new List<MATERIA_PRIMA_ESTOQUE_FILIAL>();

        String extensao;

        public EstoqueController(IProdutoAppService prodApps, IMateriaPrimaAppService insApps, ICategoriaMateriaAppService ciApps, ICategoriaProdutoAppService cpApps, IFilialAppService filApps, IMovimentoEstoqueMateriaAppService moemApps, IMovimentoEstoqueProdutoAppService moepApps, ISubcategoriaMateriaAppService scmpApps, IProdutoEstoqueFilialAppService pefApps, IMateriaEstoqueFilialAppService mefApps, IFornecedorAppService fornApps)
        {
            prodApp = prodApps;
            insApp = insApps;
            ciApp = ciApps;
            cpApp = cpApps;
            filApp = filApps;
            moemApp = moemApps;
            moepApp = moepApps;
            scmpApp = scmpApps;
            pefApp = pefApps;
            mefApp = mefApps;
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
            
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult FiltrarSubCategoriaProduto(Int32? id)
        {
            var listaSubFiltrada = prodApp.GetAllSubs();

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaSubFiltrada = prodApp.GetAllSubs().Where(x => x.CAPR_CD_ID == id).ToList();
            }

            return Json(listaSubFiltrada.Select(x => new { value = x.SCPR_CD_ID, text = x.SCPR_NM_NOME }));
        }

        [HttpPost]
        public JsonResult FiltrarCategoriaProduto(Int32? id)
        {
            var listaFiltrada = cpApp.GetAllItens();

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaFiltrada = listaFiltrada.Where(x => x.SUBCATEGORIA_PRODUTO.Any(s => s.SCPR_CD_ID == id)).ToList();
            }

            return Json(listaFiltrada.Select(x => new { value = x.CAPR_CD_ID, text = x.CAPR_NM_NOME }));
        }

        [HttpPost]
        public JsonResult FiltrarSubCategoriaMateria(Int32? id)
        {
            var listaSubFiltrada = scmpApp.GetAllItens();

            if (id != null)
            {
                listaSubFiltrada = listaSubFiltrada.Where(x => x.CAMA_CD_ID == id).ToList();
            }

            return Json(listaSubFiltrada.Select(x => new { value = x.SCMP_CD_ID, text = x.SCMP_NM_NOME }));
        }

        [HttpPost]
        public JsonResult FiltrarCategoriaMateria(Int32? id)
        {
            var listaFiltrada = ciApp.GetAllItens();

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaFiltrada = listaFiltrada.Where(x => x.SUBCATEGORIA_MATERIA.Any(s => s.SCMP_CD_ID == id)).ToList();
            }

            return Json(listaFiltrada.Select(x => new { value = x.CAMA_CD_ID, text = x.CAMA_NM_NOME }));
        }

        [HttpGet]
        public ActionResult MontarTelaEstoqueProduto()
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

            // Carrega listas
            if (SessionMocks.listaProdEstoqueFilial == null)
            {
                listaMasterProdFili = pefApp.GetAllItens();
                SessionMocks.listaProdEstoqueFilial = listaMasterProdFili;
            }
            if (SessionMocks.listaProdEstoqueFilial.Count == 0)
            {
                listaMasterProdFili = pefApp.GetAllItens();
                SessionMocks.listaProdEstoqueFilial = listaMasterProdFili;
            }

            if (usuario.FILI_CD_ID != null)
            {
                ViewBag.Listas = SessionMocks.listaProdEstoqueFilial.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).OrderBy(x => x.PRODUTO.PROD_NM_NOME).ToList<PRODUTO_ESTOQUE_FILIAL>();
            }
            else
            {
                ViewBag.Listas = SessionMocks.listaProdEstoqueFilial.OrderBy(x => x.PRODUTO.PROD_NM_NOME).ToList<PRODUTO_ESTOQUE_FILIAL>();
            }

            ViewBag.Title = "Estoque";
            ViewBag.CatProd = new SelectList(SessionMocks.CatsProduto, "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.CatIns = new SelectList(SessionMocks.CatsInsumos, "CAMA_CD_ID", "CAMA_NM_NOME");

            // Indicadores
            ViewBag.Produtos = SessionMocks.listaProdEstoqueFilial.Count;

            if (usuario.FILIAL != null)
            {
                ViewBag.FilialUsuario = usuario.FILIAL.FILI_NM_NOME;
            }

            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.FilialUsuario = usuario.FILI_CD_ID;

            // Mansagem
            if ((Int32)Session["MensEstoque"] == 1)
            {
                Session["MensEstoque"] = 0;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["VoltaEstoque"] = 0;
            Session["MensEstoque"] = 0;
            objetoProd = new PRODUTO();
            if (SessionMocks.filtroProduto != null)
            {
                objetoProd = SessionMocks.filtroProduto;
                SessionMocks.filtroProduto = null;
            }
            listaMasterProdFili = null;
            SessionMocks.listaProdEstoqueFilial = null;
            return View(objetoProd);
        }

        public ActionResult RetirarFiltroProduto()
        {
            
            SessionMocks.listaProdEstoqueFilial = null;
            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        public ActionResult RetirarFiltroMovimentacaoEstoqueProduto(Int32 id)
        {
            
            SessionMocks.filtroMvmtProd = false;
            return RedirectToAction("VerMovimentacaoEstoqueProduto", new { id = id });
        }

        [HttpPost]
        public ActionResult FiltrarProduto(PRODUTO item)
        {
            
            try
            {
                // Executa a operação
                List<PRODUTO_ESTOQUE_FILIAL> listaObj = new List<PRODUTO_ESTOQUE_FILIAL>();
                SessionMocks.filtroProduto = item;
                USUARIO usuario = SessionMocks.UserCredentials;
                if (usuario.PERF_CD_ID != 1)
                {
                    item.FILI_CD_ID = usuario.FILI_CD_ID;
                }
                Int32 volta = prodApp.ExecuteFilterEstoque(item.FILI_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_NR_BARCODE, item.CAPR_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEstoque"] = 1;
                    ModelState.AddModelError("", "Nenhum produto encontrado");
                }

                // Sucesso
                listaMasterProdFili = listaObj;
                SessionMocks.listaProdEstoqueFilial = listaObj;
                return RedirectToAction("MontarTelaEstoqueProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEstoqueProduto");
            }
        }

        public ActionResult VoltarBaseProduto()
        {
            
            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        public ActionResult GerarRelatorioEstoqueProduto()
        {
            
            var usuario = SessionMocks.UserCredentials;

            Int32 perfil = 0;
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                perfil = 1;
            }

            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ProdutoEstoqueLista" + "_" + data + ".pdf";
            List<PRODUTO> lista = SessionMocks.listaProduto;
            PRODUTO filtro = SessionMocks.filtroProduto;
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

            cell = new PdfPCell(new Paragraph("Estoque de Produtos - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Produtos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Filial", meuFont))
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
            cell = new PdfPCell(new Paragraph("Codigo de Barra", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Estoque", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Minima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Máxima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Última Movimentação", meuFont))
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

            foreach (PRODUTO item in lista)
            {
                if (item.FILIAL != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FILIAL.FILI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_NR_BARCODE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_QN_ESTOQUE.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_QN_QUANTIDADE_MINIMA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_QN_QUANTIDADE_MAXIMA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_DT_ULTIMA_MOVIMENTACAO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.PROD_AQ_FOTO != null)
                {
                    Image foto = Image.GetInstance(Server.MapPath(item.PROD_AQ_FOTO));
                    cell = new PdfPCell(foto, true);
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph(" - ", meuFont))
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
                if (filtro.PROD_NR_BARCODE != null)
                {
                    parametros += "Código de Barras: " + filtro.PROD_NR_BARCODE;
                    ja = 1;
                }
                if (filtro.PROD_CD_CODIGO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Código: " + filtro.PROD_CD_CODIGO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Código: " + filtro.PROD_CD_CODIGO;
                    }
                }
                if (filtro.CATEGORIA_PRODUTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Categoria: " + filtro.CATEGORIA_PRODUTO.CAPR_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Categoria: " + filtro.CATEGORIA_PRODUTO.CAPR_NM_NOME;
                    }
                }
                if (filtro.PROD_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.PROD_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.PROD_NM_NOME;
                    }
                }
                if (filtro.PROD_NM_MARCA != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Marca: " + filtro.PROD_NM_MARCA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Marca: " + filtro.PROD_NM_MARCA;
                    }
                }
                if (perfil == 1)
                {
                    if (filtro.FILI_CD_ID != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Filial: " + filtro.FILI_CD_ID;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Filial: " + filtro.FILI_CD_ID;
                        }
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

            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        [HttpGet]
        public ActionResult MontarTelaEstoqueInsumo()
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

            // Carrega listas
            if (SessionMocks.listaInsEstoqueFilial == null)
            {
                listaMasterInsFili = mefApp.GetAllItens();
                SessionMocks.listaInsEstoqueFilial = listaMasterInsFili;
            }
            if (SessionMocks.listaInsEstoqueFilial.Count == 0)
            {
                listaMasterInsFili = mefApp.GetAllItens();
                SessionMocks.listaInsEstoqueFilial = listaMasterInsFili;
            }

            if (usuario.FILI_CD_ID != null)
            {
                ViewBag.Listas = SessionMocks.listaInsEstoqueFilial.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).OrderBy(x => x.MATERIA_PRIMA.MAPR_NM_NOME).ToList<MATERIA_PRIMA_ESTOQUE_FILIAL>();
            }
            else
            {
                ViewBag.Listas = SessionMocks.listaInsEstoqueFilial.OrderBy(x => x.MATERIA_PRIMA.MAPR_NM_NOME).ToList<MATERIA_PRIMA_ESTOQUE_FILIAL>();
            }

            ViewBag.Title = "Estoque";
            ViewBag.CatProd = new SelectList(cpApp.GetAllItens(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.CatIns = new SelectList(ciApp.GetAllItens(), "CAMA_CD_ID", "CAMA_NM_NOME");

            // Indicadores
            //ViewBag.Insumos = SessionMocks.listaMateria.Count;

            if (usuario.FILIAL != null)
            {
                ViewBag.FilialUsuario = usuario.FILIAL.FILI_NM_NOME;
            }

            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.FilialUsuario = usuario.FILI_CD_ID;

            // Mansagem
            if (Session["MensEstoque"] != null)
            {
                if ((Int32)Session["MensEstoque"] == 1)
                {
                    Session["MensEstoque"] = 0;
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaEstoque"] = 0;
            objetoIns = new MATERIA_PRIMA();
            if (SessionMocks.filtroMateria != null)
            {
                objetoIns = SessionMocks.filtroMateria;
                SessionMocks.filtroMateria = null;
            }
            listaMasterInsFili = null;
            SessionMocks.listaInsEstoqueFilial = null;
            return View(objetoIns);
        }

        public ActionResult RetirarFiltroInsumo()
        {
            
            SessionMocks.listaInsEstoqueFilial = null;
            return RedirectToAction("MontarTelaEstoqueInsumo");
        }

        public ActionResult RetirarFiltroMovimentacaoEstoqueInsumo(Int32 id)
        {
            
            SessionMocks.filtroMvmtIns = false;
            return RedirectToAction("VerMovimentacaoEstoqueInsumo", new { id = id });
        }

        [HttpPost]
        public ActionResult FiltrarInsumo(MATERIA_PRIMA item)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuario = SessionMocks.UserCredentials;
                List<MATERIA_PRIMA_ESTOQUE_FILIAL> listaObj = new List<MATERIA_PRIMA_ESTOQUE_FILIAL>();
                SessionMocks.filtroMateria = item;
                if (usuario.FILI_CD_ID != null)
                {
                    item.FILI_CD_ID = usuario.FILI_CD_ID;
                }
                Int32 volta = insApp.ExecuteFilterEstoque(item.FILI_CD_ID, item.MAPR_NM_NOME, item.MAPR_DS_DESCRICAO, item.MAPR_CD_CODIGO, item.CAMA_CD_ID, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEstoque"] = 1;
                }

                // Sucesso
                listaMasterInsFili = listaObj;
                SessionMocks.listaInsEstoqueFilial = listaObj;
                return RedirectToAction("MontarTelaEstoqueInsumo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEstoqueInsumo");
            }
        }

        public ActionResult VoltarBaseInsumo()
        {
            
            return RedirectToAction("MontarTelaEstoqueInsumo");
        }

        public ActionResult GerarRelatorioEstoqueInsumo()
        {
            
            var usuario = SessionMocks.UserCredentials;

            Int32 perfil = 0;
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                perfil = 1;
            }

            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "InsumoEstoqueLista" + "_" + data + ".pdf";
            List<MATERIA_PRIMA> lista = SessionMocks.listaMateria;
            MATERIA_PRIMA filtro = SessionMocks.filtroMateria;
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

            cell = new PdfPCell(new Paragraph("Estoque de Insumos - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Insumos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Filial", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Insumo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 2;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade em Estoque", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Minima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Máxima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Última Movimentação", meuFont))
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

            foreach (MATERIA_PRIMA item in lista)
            {
                if (item.FILIAL != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FILIAL.FILI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.MAPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_QN_ESTOQUE.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_QN_ESTOQUE_MINIMO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_QN_ESTOQUE_MAXIMO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.MAPR_DT_ULTIMA_MOVIMENTACAO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.MAPR_AQ_FOTO != null)
                {
                    Image foto = Image.GetInstance(Server.MapPath(item.MAPR_AQ_FOTO));
                    cell = new PdfPCell(foto, true);
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else {
                    cell = new PdfPCell(new Paragraph(" - ", meuFont))
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
                if (filtro.CATEGORIA_MATERIA != null)
                {
                    parametros += "Categoria: " + filtro.CATEGORIA_MATERIA.CAMA_NM_NOME;
                    ja = 1;
                }
                if (filtro.MAPR_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.MAPR_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.MAPR_NM_NOME;
                    }
                }
                if (filtro.MAPR_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.MAPR_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.MAPR_DS_DESCRICAO;
                    }
                }
                if (filtro.MAPR_CD_CODIGO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.MAPR_CD_CODIGO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.MAPR_CD_CODIGO;
                    }
                }
                if (perfil == 1)
                {
                    if (filtro.FILI_CD_ID != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Filial: " + filtro.FILI_CD_ID;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Filial: " + filtro.FILI_CD_ID;
                        }
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

            return RedirectToAction("MontarTelaEstoqueInsumo");
        }

        [HttpGet]
        public ActionResult VerEstoqueProduto(Int32 id)
        {
            
            // Prepara view
            PRODUTO item = prodApp.GetItemById(id);
            Session["VoltaEstoque"] = 1;
            return RedirectToAction("ConsultarProduto", "Produto", new { id = id });

        }

        [HttpGet]
        public ActionResult AcertoManualProduto(Int32 id)
        {
            
            // Prepara view
            var usu = SessionMocks.UserCredentials;
            PRODUTO_ESTOQUE_FILIAL item = pefApp.GetItemById(id);
            objetoProdFiliAntes = item;
            SessionMocks.ProdutoEstoqueFilial = item;
            SessionMocks.idVolta = id;
            item.PREF_QN_QUANTIDADE_ALTERADA = item.PREF_QN_ESTOQUE;
            item.PREF_DS_JUSTIFICATIVA = String.Empty;
            return View(item);
        }

        [HttpPost]
        public ActionResult AcertoManualProduto(PRODUTO_ESTOQUE_FILIAL item)
        {
            
            if (item.PREF_QN_QUANTIDADE_ALTERADA == null)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM não pode ser nulo");
                return View(item);
            }
            if (item.PREF_QN_ESTOQUE == item.PREF_QN_QUANTIDADE_ALTERADA)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM com mesmo valor de ESTOQUE");
                return View(item);
            }
            if (item.PREF_DS_JUSTIFICATIVA == null)
            {
                ModelState.AddModelError("", "Campo JUSTIFICATIVA obrigatorio");
                return View(item);
            }
            if (item.FILIAL == null)
            {
                ModelState.AddModelError("", "Produto sem filial");
                return View(item);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = pefApp.ValidateEditEstoque(item, objetoProdFiliAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterProdFili = new List<PRODUTO_ESTOQUE_FILIAL>();
                    SessionMocks.listaProdEstoqueFilial = null;
                    return RedirectToAction("MontarTelaEstoqueProduto");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(objetoProdFiliAntes);
                }
            }
            else
            {
                return View(objetoProdFiliAntes);
            }
        }

        [HttpGet]
        public ActionResult AcertoManualInsumo(Int32 id)
        {
            
            // Prepara view
            var usu = SessionMocks.UserCredentials;
            MATERIA_PRIMA_ESTOQUE_FILIAL item = mefApp.GetItemById(id);
            objetoInsAntesFili = item;
            SessionMocks.MateriaEstoqueFilial = item;
            SessionMocks.idVolta = id;
            item.MPFE_QN_QUANTIDADE_ALTERADA = item.MPFE_QN_ESTOQUE;
            item.MPFE_DS_JUSTIFICATIVA = String.Empty;
            return View(item);
        }

        [HttpPost]
        public ActionResult AcertoManualInsumo(MATERIA_PRIMA_ESTOQUE_FILIAL item)
        {
            
            if (item.MPFE_QN_QUANTIDADE_ALTERADA == null)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM não pode ser nulo");
                return View(item);
            }
            if (item.MPFE_QN_ESTOQUE == item.MPFE_QN_QUANTIDADE_ALTERADA)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM com mesmo valor de ESTOQUE");
                return View(item);
            }
            if (item.MPFE_DS_JUSTIFICATIVA == null)
            {
                ModelState.AddModelError("", "Campo JUSTIFICATIVA obrigatorio");
                return View(item);
            }
            if (item.FILIAL == null)
            {
                ModelState.AddModelError("", "Insumo sem FILIAL");
                return View(item);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = mefApp.ValidateEditEstoque(item, objetoInsAntesFili, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterInsFili = new List<MATERIA_PRIMA_ESTOQUE_FILIAL>();
                    SessionMocks.listaInsEstoqueFilial = null;
                    return RedirectToAction("MontarTelaEstoqueInsumo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(objetoInsAntesFili);
                }
            }
            else
            {
                return View(objetoInsAntesFili);
            }
        }


        [HttpGet]
        public ActionResult VerEstoqueInsumo(Int32 id)
        {
            
            // Prepara view
            MATERIA_PRIMA item = insApp.GetItemById(id);
            Session["VoltaEstoque"] = 1;
            return RedirectToAction("ConsultarMateria", "Insumo", new { id = id });
        }

        public ActionResult VerMovimentacaoEstoqueProduto(Int32 id)
        {
            
            List<SelectListItem> filtroEntradaSaida = new List<SelectListItem>();
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.FiltroEntradaSaida = new SelectList(filtroEntradaSaida, "Value", "Text");

            USUARIO usu = SessionMocks.UserCredentials;

            // Prepara view
            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetItemById(id);
            PRODUTO item = prodApp.GetItemById(pef.PROD_CD_ID);
            if (SessionMocks.filtroMvmtProd)
            {
                SessionMocks.filtroMvmtProd = false;
                item.MOVIMENTO_ESTOQUE_PRODUTO = item.MOVIMENTO_ESTOQUE_PRODUTO.Where(x => x.MOEP_IN_TIPO_MOVIMENTO == SessionMocks.EntradaSaida).ToList();
                objetoProdAntes = item;
            }
            item.MOVIMENTO_ESTOQUE_PRODUTO = item.MOVIMENTO_ESTOQUE_PRODUTO.Where(x => x.FILI_CD_ID == pef.FILI_CD_ID).ToList();
            objetoProdAntes = item;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarAnexoProduto()
        {
            
            return RedirectToAction("VerEstoqueProduto", new { id = SessionMocks.idVolta });
        }

        public ActionResult VerMovimentacaoEstoqueInsumo(Int32 id)
        {
            
            List<SelectListItem> filtroEntradaSaida = new List<SelectListItem>();
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.FiltroEntradaSaida = new SelectList(filtroEntradaSaida, "Value", "Text");

            USUARIO usu = SessionMocks.UserCredentials;

            // Prepara View
            MATERIA_PRIMA_ESTOQUE_FILIAL mef = mefApp.GetItemById(id);
            MATERIA_PRIMA item = insApp.GetItemById(mef.MAPR_CD_ID);
            if (SessionMocks.filtroMvmtIns)
            {
                SessionMocks.filtroMvmtIns = false;
                item.MOVIMENTO_ESTOQUE_MATERIA_PRIMA = item.MOVIMENTO_ESTOQUE_MATERIA_PRIMA.Where(x => x.MOEM_IN_TIPO_MOVIMENTO == SessionMocks.EntradaSaida).ToList();
                objetoInsAntes = item;
            }
            item.MOVIMENTO_ESTOQUE_MATERIA_PRIMA = item.MOVIMENTO_ESTOQUE_MATERIA_PRIMA.Where(x => x.FILI_CD_ID == mef.FILI_CD_ID).ToList();
            objetoInsAntes = item;
            MateriaPrimaViewModel vm = Mapper.Map<MATERIA_PRIMA, MateriaPrimaViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarAnexoInsumo()
        {
            
            return RedirectToAction("VerEstoqueInsumo", new { id = SessionMocks.idVolta });
        }

        public ActionResult RetirarFiltroInventario()
        {
            
            Session["FiltroInventario"] = null;
            return RedirectToAction("MontarTelaInventario");
        }

        [HttpGet]
        public ActionResult MontarTelaInventario()
        {
            
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            InventarioViewModel vm = new InventarioViewModel();
            if (Session["FiltroInventario"] != null)
            {
                vm = (InventarioViewModel)Session["FiltroInventario"];
            }

            //if (SessionMocks.filtroInventario == null)
            //{
            //    SessionMocks.listaProdEstoqueFilial = null;
            //    SessionMocks.listaInsEstoqueFilial = null;
            //}

            if (SessionMocks.listaProdEstoqueFilial == null && SessionMocks.listaInsEstoqueFilial == null)
            {
                if (Session["FiltroInventario"] != null)
                {
                    vm = (InventarioViewModel)Session["FiltroInventario"];
                    Session["FiltroInventario"] = null;
                    return FiltrarInventario(vm);
                }
                else
                {
                    if (vm.PRODUTO != null)
                    {
                        vm.PRODUTO = new PRODUTO();
                    }
                    else if (vm.MATERIA_PRIMA != null)
                    {
                        vm.MATERIA_PRIMA = new MATERIA_PRIMA();
                    }
                    else
                    {
                        vm.PRODUTO = new PRODUTO();
                    }
                    return FiltrarInventario(vm);
                }
            }

            ViewBag.Title = "Inventário";
            ViewBag.ListaProd = SessionMocks.listaProdEstoqueFilial == null ? null : SessionMocks.listaProdEstoqueFilial.ToList<PRODUTO_ESTOQUE_FILIAL>();
            ViewBag.ListaIns = SessionMocks.listaInsEstoqueFilial == null ? null : SessionMocks.listaInsEstoqueFilial.ToList<MATERIA_PRIMA_ESTOQUE_FILIAL>();
            ViewBag.Diferenca = SessionMocks.listaValEstoqueAlterado;
            List<SelectListItem> filtroPM = new List<SelectListItem>();
            filtroPM.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            filtroPM.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.CatProd = new SelectList(cpApp.GetAllItens().OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.SubCatProd = new SelectList(prodApp.GetAllSubs().OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.CatIns = new SelectList(ciApp.GetAllItens().OrderBy(x => x.CAMA_NM_NOME).ToList<CATEGORIA_MATERIA>(), "CAMA_CD_ID", "CAMA_NM_NOME");
            ViewBag.SubCatIns= new SelectList(scmpApp.GetAllItens().OrderBy(x => x.SCMP_NM_NOME).ToList<SUBCATEGORIA_MATERIA>(), "SCMP_CD_ID", "SCMP_NM_NOME");
            ViewBag.ProdutoInsumo = new SelectList(filtroPM, "Value", "Text");
            if (SessionMocks.listaProdEstoqueFilial != null)
            {
                ViewBag.IsProdIns = 1;
            }
            else if (SessionMocks.listaProdEstoqueFilial != null) 
            {
                ViewBag.IsProdIns = 2;
            }
            ViewBag.CdFiliUsuario = usuario.FILI_CD_ID == null ? 1 : usuario.FILI_CD_ID;
            ViewBag.FilialUsuario = usuario.FILIAL == null ? "Sem Filial" : usuario.FILIAL.FILI_NM_NOME;
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME", "Selecionar");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensEstoque"] != null)
            {
                if ((Int32)Session["MensEstoque"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensEstoque"] = 0;
                }
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult FiltrarInventario(InventarioViewModel vm)
        {
            
            USUARIO usuario = SessionMocks.UserCredentials;

            Session["FiltroInventario"] = vm;

            PRODUTO prod = vm.PRODUTO;
            MATERIA_PRIMA ins = vm.MATERIA_PRIMA;
            if (vm.TIPO == 1 || vm.TIPO == null)
            {
                prod.FILI_CD_ID = vm.FILI_CD_ID_P;
                try
                {
                    // Executa a operação
                    List<PRODUTO_ESTOQUE_FILIAL> listaObj = new List<PRODUTO_ESTOQUE_FILIAL>();
                    if (usuario.FILI_CD_ID != null)
                    {
                        prod.FILI_CD_ID = usuario.FILI_CD_ID;
                    }
                    Int32 volta = prodApp.ExecuteFilterEstoque(prod.FILI_CD_ID, prod.PROD_NM_NOME, prod.PROD_NM_MARCA, prod.PROD_CD_CODIGO, prod.PROD_NR_BARCODE, prod.CAPR_CD_ID, out listaObj);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEstoque"] = 1;
                    }

                    // Sucesso
                    listaMasterProdFili = listaObj;
                    SessionMocks.listaProdEstoqueFilial = listaObj;

                    return RedirectToAction("MontarTelaInventario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaInventario");
                }
            }
            else if (vm.TIPO == 2)
            {
                ins.FILI_CD_ID = vm.FILI_CD_ID_I;
                try
                {
                    // Executa a operação
                    List<MATERIA_PRIMA_ESTOQUE_FILIAL> listaObj = new List<MATERIA_PRIMA_ESTOQUE_FILIAL>();
                    if (usuario.FILI_CD_ID != null)
                    {
                        ins.FILI_CD_ID = usuario.FILI_CD_ID;
                    }
                    Int32 volta = insApp.ExecuteFilterEstoque(ins.FILI_CD_ID, ins.MAPR_NM_NOME, ins.MAPR_DS_DESCRICAO, ins.MAPR_CD_CODIGO, ins.CAMA_CD_ID, out listaObj);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEstoque"] = 1;
                    }

                    // Sucesso
                    listaMasterInsFili = listaObj;
                    SessionMocks.listaInsEstoqueFilial = listaObj;

                    return RedirectToAction("MontarTelaInventario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaInventario");
                }
            }
            else
            {
                return RedirectToAction("MontarTelaInventario");
            }
        }

        [HttpPost]
        public JsonResult IncluirMovimentacaoEstoque(String grid, String tipo, String justificativa)
        {
            var usuario = SessionMocks.UserCredentials;
            var listaAlterado = new List<Hashtable>();
            var jArray = JArray.Parse(grid);
            Int32 count = 0;

            if (tipo == "PROD")
            {
                foreach (var jObject in jArray)
                {
                    try
                    {
                        var pef = pefApp.GetItemById((Int32)jObject["idPef"]);

                        if ((Int32)jObject["qtde"] != pef.PREF_QN_ESTOQUE)
                        {
                            Int32 operacao = pef.PREF_QN_ESTOQUE < (Int32)jObject["qtde"] ? 1 : 2;
                            Int32 quant = 0;
                            if ((Int32)jObject["qtde"] > pef.PREF_QN_ESTOQUE)
                            {
                                quant = (Int32)jObject["qtde"] - (Int32)pef.PREF_QN_ESTOQUE;
                            }
                            else
                            {
                                quant = (Int32)pef.PREF_QN_ESTOQUE - (Int32)jObject["qtde"];
                            }

                            MOVIMENTO_ESTOQUE_PRODUTO movto = new MOVIMENTO_ESTOQUE_PRODUTO();
                            movto.MOEP_IN_CHAVE_ORIGEM = 2;
                            movto.MOEP_IN_ORIGEM = "Inventário";
                            movto.MOEP_IN_OPERACAO = operacao;
                            movto.MOEP_IN_TIPO_MOVIMENTO = 0;

                            movto.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            movto.FILI_CD_ID = pef.FILI_CD_ID;
                            movto.MOEP_DT_MOVIMENTO = DateTime.Today;
                            movto.MOEP_IN_ATIVO = 1;
                            movto.MOEP_QN_QUANTIDADE = (Int32)jObject["qtde"] - (Int32)pef.PREF_QN_ESTOQUE;
                            movto.PROD_CD_ID = pef.PROD_CD_ID;
                            movto.USUA_CD_ID = usuario.USUA_CD_ID;
                            movto.ASSI_CD_ID = SessionMocks.IdAssinante;
                            movto.MOEP_DS_JUSTIFICATIVA = justificativa;
                            movto.MOEP_QN_ANTES = pef.PREF_QN_ESTOQUE;
                            movto.MOEP_QN_ALTERADA = (Int32)jObject["qtde"] - pef.PREF_QN_ESTOQUE;
                            movto.MOEP_QN_DEPOIS = (Int32)jObject["qtde"];

                            PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();
                            pef1.FILI_CD_ID = pef.FILI_CD_ID;
                            pef1.PREF_DS_JUSTIFICATIVA = justificativa;
                            pef1.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            pef1.PREF_IN_ATIVO = 1;
                            pef1.PREF_QN_ESTOQUE = (Int32)jObject["qtde"];
                            pef1.PROD_CD_ID = pef.PROD_CD_ID;
                            pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;
                            pef1.PREF_CD_ID = pef.PREF_CD_ID;
                            pef1.PREF_QN_QUANTIDADE_ALTERADA = (Int32)jObject["qtde"] - pef.PREF_QN_ESTOQUE;

                            Int32 v = pefApp.ValidateEdit(pef1, pef, usuario);

                            var valAlterado = new Hashtable();
                            valAlterado.Add("id", movto.PROD_CD_ID);
                            valAlterado.Add("fili", movto.FILI_CD_ID);
                            valAlterado.Add("val", movto.MOEP_QN_ALTERADA);
                            listaAlterado.Add(valAlterado);

                            Int32 volta = moepApp.ValidateCreate(movto, usuario);

                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = ex.Message;
                    }
                }

                SessionMocks.listaValEstoqueAlterado = listaAlterado;
                SessionMocks.listaProdEstoqueFilial = null;

                if (count == 0)
                {
                    return Json("Nenhum registro alterado");
                }
                else
                {
                    return Json("Foram alterados " + count + " produtos");
                }
            }
            else if (tipo == "INS")
            {
                foreach (var jObject in jArray)
                {
                    try
                    {
                        var mef = mefApp.GetById((Int32)jObject["idMef"]);

                        if ((Int32)jObject["qtde"] != mef.MPFE_QN_ESTOQUE)
                        {
                            Int32 operacao = mef.MPFE_QN_ESTOQUE < (Int32)jObject["qtde"] ? 1 : 2;
                            Int32 quant = 0;
                            if ((Int32)jObject["qtde"] > mef.MPFE_QN_ESTOQUE)
                            {
                                quant = (Int32)jObject["qtde"] - (Int32)mef.MPFE_QN_ESTOQUE;
                            }
                            else
                            {
                                quant = (Int32)mef.MPFE_QN_ESTOQUE - (Int32)jObject["qtde"];
                            }

                            MOVIMENTO_ESTOQUE_MATERIA_PRIMA movto = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
                            movto.MOEM_IN_CHAVE_ORIGEM = 2;
                            movto.MOEM_NM_ORIGEM = "Inventário";
                            movto.MOEM_IN_OPERACAO = operacao;
                            movto.MOEM_IN_TIPO_MOVIMENTO = 0;

                            movto.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            movto.FILI_CD_ID = mef.FILI_CD_ID;
                            movto.MOEM_DT_MOVIMENTO = DateTime.Today;
                            movto.MOEM_IN_ATIVO = 1;
                            movto.MOEM_QN_QUANTIDADE = (Int32)jObject["qtde"] - (Int32)mef.MPFE_QN_ESTOQUE;
                            movto.MAPR_CD_ID = mef.MAPR_CD_ID;
                            movto.USUA_CD_ID = usuario.USUA_CD_ID;
                            movto.ASSI_CD_ID = SessionMocks.IdAssinante;
                            movto.MOEM_DS_JUSTIFICATIVA = justificativa;
                            movto.MOEM_QN_ANTES = mef.MPFE_QN_ESTOQUE;
                            movto.MOEM_QN_ALTERADA = (Int32)jObject["qtde"] - mef.MPFE_QN_ESTOQUE;
                            movto.MOEM_QN_DEPOIS = (Int32)jObject["qtde"];

                            MATERIA_PRIMA_ESTOQUE_FILIAL mef1 = new MATERIA_PRIMA_ESTOQUE_FILIAL();
                            mef1.FILI_CD_ID = mef.FILI_CD_ID;
                            mef1.MPFE_DS_JUSTIFICATIVA = justificativa;
                            mef1.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            mef1.MPFE_IN_ATIVO = 1;
                            mef1.MPFE_QN_ESTOQUE = (Int32)jObject["qtde"];
                            mef1.MAPR_CD_ID = mef.MAPR_CD_ID;
                            mef1.MPFE_CD_ID = mef.MPFE_CD_ID;
                            mef1.MPFE_QN_QUANTIDADE_ALTERADA = (Int32)jObject["qtde"] - mef.MPFE_QN_ESTOQUE;

                            Int32 v = mefApp.ValidateEdit(mef1, mef, usuario);

                            var valAlterado = new Hashtable();
                            valAlterado.Add("id", movto.MAPR_CD_ID);
                            valAlterado.Add("fili", movto.FILI_CD_ID);
                            valAlterado.Add("val", movto.MOEM_QN_ALTERADA);
                            listaAlterado.Add(valAlterado);

                            Int32 volta = moemApp.ValidateCreate(movto, usuario);

                            count++;
                        }                        
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = ex.Message;
                    }
                }

                SessionMocks.listaValEstoqueAlterado = listaAlterado;
                SessionMocks.listaInsEstoqueFilial = null;

                if (count == 0)
                {
                    return Json("Nenhum registro alterado");
                }
                else
                {
                    return Json("Foram alterados " + count + " insumos");
                }
            }
            else
            {
                return null;
            }
        }

        public ActionResult GerarRelatorioInventarioFiltro()
        {
            
            return RedirectToAction("GerarRelatorioInventario", new { id = 1 });
        }

        [HttpGet]
        public ActionResult GerarRelatorioInventario(Int32 id)
        {
            
            Int32 filtroProdIns = 0;

            var listaProd = new List<PRODUTO>();
            var listaIns = new List<MATERIA_PRIMA>();
            var filtroProd = new PRODUTO();
            var filtroIns = new MATERIA_PRIMA();
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = String.Empty;
            String titulo = String.Empty;

            if (SessionMocks.listaProduto != null)
            {
                if (id == 1)
                {
                    nomeRel = "InventarioListaProduto" + "_" + data + ".pdf";
                    titulo = "Inventário - Produto - Listagem";
                    listaProd = SessionMocks.listaProduto;
                    filtroProdIns = 1;
                }
                else
                {
                    return RedirectToAction("MontarTelaInventario");
                }
            }

            if (SessionMocks.listaMateria != null)
            {
                if (id == 1)
                {
                    nomeRel = "InventarioListaMateriaPrima" + "_" + data + ".pdf";
                    titulo = "Inventário - Insumo - Listagem";
                    listaIns = SessionMocks.listaMateria;
                    filtroProdIns = 2;
                }
                else
                {
                    return RedirectToAction("MontarTelaInventario");
                }
            }
            filtroProd = SessionMocks.filtroProduto;
            filtroIns = SessionMocks.filtroMateria;
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

            cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
            table = new PdfPTable(new float[] { 100f, 100f, 150f, 80f, 40f, 100f, 100f, 40f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            String info = String.Empty;
            if (filtroProdIns == 1)
            {
                info = "Produtos selecionados pelos parametros de filtro abaixo";
            }
            else if (filtroProdIns == 2)
            {
                info = "Insumos selecionados pelos parametros de filtro abaixo";
            }

            cell = new PdfPCell(new Paragraph(info, meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
            cell = new PdfPCell(new Paragraph("Filial", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            if (filtroProdIns != 1)
            {
                cell.Colspan = 2;
            }
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Estoque", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor Alterado", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            if (filtroProdIns == 1)
            {
                cell = new PdfPCell(new Paragraph("Código de Barras", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
            }

            if (filtroProdIns == 1)
            {
                foreach (var prod in listaProd)
                {
                    cell = new PdfPCell(new Paragraph(prod.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(prod.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(prod.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    if (prod.FILIAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(prod.FILIAL.FILI_NM_NOME, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    cell = new PdfPCell(new Paragraph(prod.PROD_QN_ESTOQUE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    var valAlterado = SessionMocks.listaValEstoqueAlterado.Where(x => (Int32)x["id"] == prod.PROD_CD_ID).Select(x => x["val"]).FirstOrDefault();
                    cell = new PdfPCell(new Paragraph(valAlterado == null ? "0" : valAlterado.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(prod.PROD_NR_BARCODE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            else if (filtroProdIns == 2)
            {
                foreach (var ins in listaIns)
                {
                    if (ins.CATEGORIA_MATERIA != null)
                    {
                        cell = new PdfPCell(new Paragraph(ins.CATEGORIA_MATERIA.CAMA_NM_NOME, meuFont))
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
                    if (ins.SUBCATEGORIA_MATERIA != null)
                    {
                        cell = new PdfPCell(new Paragraph(ins.SUBCATEGORIA_MATERIA.SCMP_NM_NOME, meuFont))
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
                    cell = new PdfPCell(new Paragraph(ins.MAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    if (ins.FILIAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(ins.FILIAL.FILI_NM_NOME, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        table.AddCell(cell);
                    }
                    cell = new PdfPCell(new Paragraph(ins.MAPR_QN_ESTOQUE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    var valAlterado = SessionMocks.listaValEstoqueAlterado.Where(x => (Int32)x["id"] == ins.MAPR_CD_ID).Select(x => x["val"]).FirstOrDefault();
                    cell = new PdfPCell(new Paragraph(valAlterado == null ? "0" : valAlterado.ToString(), meuFont))
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

            if (filtroProdIns == 1)
            {
                if (filtroProd != null)
                {
                    if (filtroProd.CAPR_CD_ID > 0)
                    {
                        parametros += "Categoria: " + filtroProd.CAPR_CD_ID.ToString();
                        ja = 1;
                    }
                    if (filtroProd.SCPR_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Subcategoria: " + filtroProd.SCPR_CD_ID.ToString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Subcategoria: " + filtroProd.SCPR_CD_ID.ToString();
                        }
                    }
                    if (String.IsNullOrEmpty(filtroProd.PROD_NM_NOME))
                    {
                        if (ja == 0)
                        {
                            parametros += "Nome: " + filtroProd.PROD_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Nome: " + filtroProd.PROD_NM_NOME;
                        }
                    }
                    if (filtroProd.FILIAL != null)
                    {
                        if (String.IsNullOrEmpty(filtroProd.FILIAL.FILI_NM_NOME))
                        {
                            if (ja == 0)
                            {
                                parametros += "Filial: " + filtroProd.FILIAL.FILI_NM_NOME;
                                ja = 1;
                            }
                            else
                            {
                                parametros += "e Filial: " + filtroProd.FILIAL.FILI_NM_NOME;
                            }
                        }
                    }
                    else
                    {
                        if (ja == 0)
                        {
                            parametros += "Filial: " + "Sem Filial";
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Filial: " + "Sem Filial";
                        }
                    }
                    if (filtroProd.PROD_NR_BARCODE != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Código de Barras: " + filtroProd.PROD_NR_BARCODE;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Código de Barras: " + filtroProd.PROD_NR_BARCODE;
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
            }
            else if (filtroProdIns == 2)
            {
                if (filtroIns != null)
                {
                    if (filtroIns.CAMA_CD_ID > 0)
                    {
                        parametros += "Categoria: " + filtroIns.CAMA_CD_ID.ToString();
                        ja = 1;
                    }
                    if (filtroIns.SCMP_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Subcategoria: " + filtroIns.SCMP_CD_ID.ToString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Subcategoria: " + filtroIns.SCMP_CD_ID.ToString();
                        }
                    }
                    if (String.IsNullOrEmpty(filtroIns.MAPR_NM_NOME))
                    {
                        if (ja == 0)
                        {
                            parametros += "Nome: " + filtroIns.MAPR_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Nome: " + filtroIns.MAPR_NM_NOME;
                        }
                    }
                    if (filtroIns.FILIAL != null)
                    {
                        if (String.IsNullOrEmpty(filtroIns.FILIAL.FILI_NM_NOME))
                        {
                            if (ja == 0)
                            {
                                parametros += "Filial: " + filtroIns.FILIAL.FILI_NM_NOME;
                                ja = 1;
                            }
                            else
                            {
                                parametros += "e Filial: " + filtroIns.FILIAL.FILI_NM_NOME;
                            }
                        }
                    }
                    else
                    {
                        if (ja == 0)
                        {
                            parametros += "Filial: " + "Sem Filial";
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Filial: " + "Sem Filial";
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

            return RedirectToAction("MontarTelaInventario");
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacao(MOVIMENTO_ESTOQUE_PRODUTO prod, MOVIMENTO_ESTOQUE_MATERIA_PRIMA ins, Int32 filtroES)
        {
            
            var retorno = new Hashtable();

            if (prod != null)
            {
                try
                {
                    // Executa a operação
                    List<MOVIMENTO_ESTOQUE_PRODUTO> listaObjProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                    Int32 volta = moepApp.ExecuteFilter(prod.PRODUTO.CAPR_CD_ID, prod.PRODUTO.SCPR_CD_ID, prod.PRODUTO.PROD_NM_NOME, prod.PRODUTO.PROD_NR_BARCODE, prod.FILI_CD_ID, prod.MOEP_DT_MOVIMENTO, out listaObjProd);
                    SessionMocks.filtroMovimentoProduto = prod;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEstoque"] = 1;
                        retorno.Add("error", "Nenhum registro localizado");
                        return Json(retorno);
                    }

                    // Filtra Entrada/Saída
                    listaObjProd = listaObjProd.Where(x => x.MOEP_IN_TIPO_MOVIMENTO == filtroES).ToList();

                    // Sucesso
                    if (listaObjProd.Count > 0)
                    {
                        SessionMocks.listaMovimentoProduto = listaObjProd;
                    }
                    else
                    {
                        retorno.Add("error", "Nenhum registro localizado");
                        return Json(retorno);
                    }

                    if (filtroES == 1)
                    {
                        SessionMocks.filtroMovimentoEntrada = 1;
                        retorno.Add("url", "../Estoque/MontarTelaMovimentacaoEntrada");
                    }
                    else
                    {
                        SessionMocks.filtroMovimentoSaida = 1;
                        retorno.Add("url", "../Estoque/MontarTelaMovimentacaoSaida");
                    }

                    SessionMocks.filtroMovimentoProduto = prod;
                    return Json(retorno);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    retorno.Add("error", ex.Message);
                    return Json(retorno);
                }
            }
            else
            {
                try
                {
                    // Executa a operação
                    List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA> listaObjIns = new List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA>();
                    Int32 volta = moemApp.ExecuteFilter(ins.MATERIA_PRIMA.CAMA_CD_ID, ins.MATERIA_PRIMA.SCMP_CD_ID, ins.MATERIA_PRIMA.MAPR_NM_NOME, ins.FILI_CD_ID, ins.MOEM_DT_MOVIMENTO, out listaObjIns);
                    SessionMocks.filtroMovimentoInsumo = ins;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEstoque"] = 1;
                        retorno.Add("error", "Nenhum registro localizado");
                        return Json(retorno);
                    }

                    // Filtra Entrada/Saída
                    listaObjIns = listaObjIns.Where(x => x.MOEM_IN_TIPO_MOVIMENTO == filtroES).ToList();

                    // Sucesso
                    if (listaObjIns.Count > 0)
                    {
                        SessionMocks.listaMovimentoInsumo = listaObjIns;
                    }
                    else
                    {
                        retorno.Add("error", "Nenhum registro localizado");
                        return Json(retorno);
                    }

                    if (filtroES == 1)
                    {
                        SessionMocks.filtroMovimentoEntrada = 2;
                        retorno.Add("url", "../Estoque/MontarTelaMovimentacaoEntrada");
                    }
                    else
                    {
                        SessionMocks.filtroMovimentoSaida = 2;
                        retorno.Add("url", "../Estoque/MontarTelaMovimentacaoSaida");
                    }

                    SessionMocks.filtroMovimentoInsumo = ins;
                    return Json(retorno);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    retorno.Add("error", ex.Message);
                    return Json(retorno);
                }
            }
        }

        [HttpPost]
        public void LimparListas()
        {
            SessionMocks.listaProduto = null;
            SessionMocks.listaMateria = null;
            SessionMocks.listaMovimentoProduto = null;
            SessionMocks.listaMovimentoInsumo = null;
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacaoEstoqueProduto(ProdutoViewModel vm, Int32? EntradaSaida)
        {
            
            SessionMocks.filtroMvmtProd = true;
            SessionMocks.EntradaSaida = EntradaSaida;
            return RedirectToAction("VerMovimentacaoEstoqueProduto", new { id = vm.PROD_CD_ID });
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacaoEstoqueMateria(MateriaPrimaViewModel vm, Int32? EntradaSaida)
        {
            
            SessionMocks.filtroMvmtIns = true;
            SessionMocks.EntradaSaida = EntradaSaida;
            return RedirectToAction("VerMovimentacaoEstoqueInsumo", new { id = vm.MAPR_CD_ID });
        }

        public SelectList GetTipoEntrada()
        {
            List<SelectListItem> tipoEntrada = new List<SelectListItem>();
            tipoEntrada.Add(new SelectListItem() { Text = "Devolução", Value = "1" });
            tipoEntrada.Add(new SelectListItem() { Text = "Retorno de conserto", Value = "2" }); // Apenas Produto
            tipoEntrada.Add(new SelectListItem() { Text = "Reclassificação - À Definir", Value = "3" });
            tipoEntrada[2].Disabled = true;
            return new SelectList(tipoEntrada, "Value", "Text");
        }

        public SelectList GetTipoSaida()
        {
            List<SelectListItem> tipoSaida = new List<SelectListItem>();
            tipoSaida.Add(new SelectListItem() { Text = "Devolução do fornecedor", Value = "1" });
            tipoSaida.Add(new SelectListItem() { Text = "Remessa para conserto", Value = "2" }); //Apenas Produto
            tipoSaida.Add(new SelectListItem() { Text = "Baixa de insumos para produção - Em Desenvolvimento", Value = "3" });
            tipoSaida.Add(new SelectListItem() { Text = "Reclassificação - À Definir", Value = "4" });
            tipoSaida.Add(new SelectListItem() { Text = "Perda ou roubo", Value = "5" });
            tipoSaida.Add(new SelectListItem() { Text = "Descarte", Value = "6" });
            tipoSaida.Add(new SelectListItem() { Text = "Outras saídas", Value = "7" });
            tipoSaida[3].Disabled = true;
            tipoSaida[4].Disabled = true;
            return new SelectList(tipoSaida, "Value", "Text");
        }

        [HttpGet]
        public ActionResult MontarTelaMovimentacaoAvulsa()
        {
            
            var usuario = SessionMocks.UserCredentials;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            var vm = new MovimentacaoAvulsaGridViewModel();
            if (Session["FiltroMovEstoque"] != null)
            {
                vm = (MovimentacaoAvulsaGridViewModel)Session["FiltroMovEstoque"];
                Session["FiltroMovEstoque"] = null;
            }
            else
            {
                vm.ProdutoInsumo = 1;
                vm.MOVMT_DT_MOVIMENTO_INICIAL = DateTime.Now;
                FiltrarMovimentacaoAvulsa(vm);
            }

            ViewBag.Title = "Movimentações Avulsas";
            List<SelectListItem> prodIns = new List<SelectListItem>();
            prodIns.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            prodIns.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.ProdutoInsumo = new SelectList(prodIns, "Value", "Text");
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                lista.Add(new SelectListItem() { Text = "Transferência entre Filiais", Value = "4" });
            }
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            if (SessionMocks.flagMovmtAvulsa != null)
            {
                if (SessionMocks.flagMovmtAvulsa == 1)
                {
                    ViewBag.TipoRegistro = 1;
                    ViewBag.ListaMovimento = SessionMocks.listaMovimentoProduto.Where(x => x.PRODUTO != null && x.PRODUTO.PROD_IN_COMPOSTO == 0).Where(x => x.MOEP_IN_CHAVE_ORIGEM == 1 || x.MOEP_IN_CHAVE_ORIGEM == 5).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }
                else
                {
                    ViewBag.TipoRegistro = 2;
                    ViewBag.ListaMovimento = SessionMocks.listaMovimentoInsumo.Where(x => x.MOEM_IN_CHAVE_ORIGEM == 1 || x.MOEM_IN_CHAVE_ORIGEM == 5).ToList<MOVIMENTO_ESTOQUE_MATERIA_PRIMA>();
                }
            }
            ViewBag.LstProd = new SelectList(prodApp.GetAllItens().Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.LstIns = new SelectList(insApp.GetAllItens().OrderBy(x => x.MAPR_NM_NOME).ToList<MATERIA_PRIMA>(), "MAPR_CD_ID", "MAPR_NM_NOME");

            if (Session["MensAvulsa"] != null)
            {
                if ((Int32)Session["MensAvulsa"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensAvulsa"] = 0;
                }
            }            

            return View(vm);
        }

        public ActionResult RetirarFiltroMovimentacaoAvulsa()
        {
            
            SessionMocks.flagMovmtAvulsa = null;
            SessionMocks.listaMovimentoProduto = null;
            SessionMocks.listaMovimentoInsumo = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        public ActionResult MostrarTudoProduto()
        {
            
            SessionMocks.flagMovmtAvulsa = 1;
            SessionMocks.listaMovimentoProduto = moepApp.GetAllItensAdm().Where(x => x.PRODUTO != null && x.PRODUTO.PROD_IN_COMPOSTO == 0).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        public ActionResult MostrarTudoInsumo()
        {
            
            SessionMocks.flagMovmtAvulsa = 2;
            SessionMocks.listaMovimentoInsumo = moemApp.GetAllItensAdm();
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacaoAvulsa(MovimentacaoAvulsaGridViewModel vm)
        {
            
            Session["FiltroMovimentacaoAvulsa"] = vm;

            try
            {
                Int32 volta = 0;
                var lstProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                var lstIns = new List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA>();
                var usuario = SessionMocks.UserCredentials;
                Session["FiltroMovEstoque"] = vm;

                if (vm.ProdutoInsumo == 1)
                {
                    SessionMocks.flagMovmtAvulsa = 1;
                    volta = moepApp.ExecuteFilterAvulso(vm.MOVMT_IN_OPERACAO, vm.MOVMT_IN_TIPO_MOVIMENTO, vm.MOVMT_DT_MOVIMENTO_INICIAL, vm.MOVMT_DT_MOVIMENTO_FINAL, (usuario.FILI_CD_ID == null ? vm.FILI_CD_ID : usuario.FILI_CD_ID), vm.PROD_CD_ID, out lstProd);
                    SessionMocks.listaMovimentoProduto = lstProd;
                }
                else
                {
                    SessionMocks.flagMovmtAvulsa = 2;
                    volta = moemApp.ExecuteFilterAvulso(vm.MOVMT_IN_OPERACAO, vm.MOVMT_IN_TIPO_MOVIMENTO, vm.MOVMT_DT_MOVIMENTO_INICIAL, vm.MOVMT_DT_MOVIMENTO_FINAL, (usuario.FILI_CD_ID == null ? vm.FILI_CD_ID : usuario.FILI_CD_ID), vm.MAPR_CD_ID, out lstIns);
                    SessionMocks.listaMovimentoInsumo = lstIns;
                }

                if (volta == 1)
                {
                    Session["MensAvulsa"] = 1;
                }

                return RedirectToAction("MontarTelaMovimentacaoAvulsa");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMovimentacaoAvulsa");
            }
        }

        //PRODUTO
        [HttpGet]
        public ActionResult ExcluirMovimentoProduto(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = SessionMocks.UserCredentials;

            // Monta movimento
            var item = moepApp.GetById(id);
            MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
            mov.ASSI_CD_ID = item.ASSI_CD_ID;
            mov.FILI_CD_ID = item.FILI_CD_ID;
            mov.MATR_CD_ID = item.MATR_CD_ID;
            mov.MOEP_CD_ID = item.MOEP_CD_ID;
            mov.MOEP_DS_JUSTIFICATIVA = item.MOEP_DS_JUSTIFICATIVA;
            mov.MOEP_DT_MOVIMENTO = item.MOEP_DT_MOVIMENTO;
            mov.MOEP_IN_ATIVO = 0;
            mov.MOEP_IN_CHAVE_ORIGEM = item.MOEP_IN_CHAVE_ORIGEM;
            mov.MOEP_IN_ORIGEM = item.MOEP_IN_ORIGEM;
            mov.MOEP_IN_TIPO_MOVIMENTO = item.MOEP_IN_TIPO_MOVIMENTO;
            mov.MOEP_QN_ANTES = item.MOEP_QN_ANTES;
            mov.MOEP_QN_DEPOIS = item.MOEP_QN_DEPOIS;
            mov.MOEP_QN_QUANTIDADE = item.MOEP_QN_QUANTIDADE;
            mov.PROD_CD_ID = item.PROD_CD_ID;
            mov.USUA_CD_ID = item.USUA_CD_ID;
            mov.MOEP_IN_OPERACAO = item.MOEP_IN_OPERACAO;

            //Monta Estoque filial
            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial(item.PROD_CD_ID, (Int32)item.FILI_CD_ID);
            if (pef != null)
            {
                PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();
                pef1.FILI_CD_ID = pef.FILI_CD_ID;
                pef1.PREF_CD_ID = pef.PREF_CD_ID;
                pef1.PREF_DS_JUSTIFICATIVA = pef.PREF_DS_JUSTIFICATIVA;
                pef1.PREF_DT_ULTIMO_MOVIMENTO = pef.PREF_DT_ULTIMO_MOVIMENTO;
                pef1.PREF_IN_ATIVO = pef.PREF_IN_ATIVO;
                pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;
                pef1.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE;
                pef1.PREF_QN_QUANTIDADE_ALTERADA = pef.PREF_QN_QUANTIDADE_ALTERADA;
                pef1.PROD_CD_ID = pef.PROD_CD_ID;

                //Efetua Operações
                if (mov.MOEP_IN_TIPO_MOVIMENTO == 1 || mov.MOEP_IN_TIPO_MOVIMENTO == 4)
                {
                    pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE - (Int32)mov.MOEP_QN_QUANTIDADE;
                    Int32 ve = pefApp.ValidateEdit(pef1, pef, usuario);
                }
                else
                {
                    pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE + (Int32)mov.MOEP_QN_QUANTIDADE;
                    Int32 vs = pefApp.ValidateEdit(pef1, pef, usuario);
                }
            }
            Int32 volta = moepApp.ValidateDelete(mov, usuario);
            SessionMocks.flagMovmtAvulsa = null;
            SessionMocks.listaMovimentoProduto = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult ReativarMovimentoProduto(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = SessionMocks.UserCredentials;

            // Monta movimento
            var item = moepApp.GetById(id);
            MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
            mov.ASSI_CD_ID = item.ASSI_CD_ID;
            mov.FILI_CD_ID = item.FILI_CD_ID;
            mov.MATR_CD_ID = item.MATR_CD_ID;
            mov.MOEP_CD_ID = item.MOEP_CD_ID;
            mov.MOEP_DS_JUSTIFICATIVA = item.MOEP_DS_JUSTIFICATIVA;
            mov.MOEP_DT_MOVIMENTO = item.MOEP_DT_MOVIMENTO;
            mov.MOEP_IN_ATIVO = 1;
            mov.MOEP_IN_CHAVE_ORIGEM = item.MOEP_IN_CHAVE_ORIGEM;
            mov.MOEP_IN_ORIGEM = item.MOEP_IN_ORIGEM;
            mov.MOEP_IN_TIPO_MOVIMENTO = item.MOEP_IN_TIPO_MOVIMENTO;
            mov.MOEP_QN_ANTES = item.MOEP_QN_ANTES;
            mov.MOEP_QN_DEPOIS = item.MOEP_QN_DEPOIS;
            mov.MOEP_QN_QUANTIDADE = item.MOEP_QN_QUANTIDADE;
            mov.PROD_CD_ID = item.PROD_CD_ID;
            mov.USUA_CD_ID = item.USUA_CD_ID;
            mov.MOEP_IN_OPERACAO = item.MOEP_IN_OPERACAO;

            //Monta Estoque filial
            var pef = pefApp.GetByProdFilial(item.PROD_CD_ID, (Int32)item.FILI_CD_ID);
            PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();
            pef1.FILI_CD_ID = pef.FILI_CD_ID;
            pef1.PREF_CD_ID = pef.PREF_CD_ID;
            pef1.PREF_DS_JUSTIFICATIVA = pef.PREF_DS_JUSTIFICATIVA;
            pef1.PREF_DT_ULTIMO_MOVIMENTO = pef.PREF_DT_ULTIMO_MOVIMENTO;
            pef1.PREF_IN_ATIVO = pef.PREF_IN_ATIVO;
            pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;
            pef1.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE;
            pef1.PREF_QN_QUANTIDADE_ALTERADA = pef.PREF_QN_QUANTIDADE_ALTERADA;
            pef1.PROD_CD_ID = pef.PROD_CD_ID;

            //Executa Operações
            if (mov.MOEP_IN_TIPO_MOVIMENTO == 1 || mov.MOEP_IN_TIPO_MOVIMENTO == 4)
            {
                pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE + (Int32)mov.MOEP_QN_QUANTIDADE;
                Int32 ve = pefApp.ValidateEdit(pef1, pef, usuario);
            }
            else
            {
                pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE - (Int32)mov.MOEP_QN_QUANTIDADE;
                Int32 vs = pefApp.ValidateEdit(pef1, pef, usuario);
            }
            item.MOEP_IN_ATIVO = 0;
            Int32 volta = moepApp.ValidateReativar(mov, usuario);
            SessionMocks.flagMovmtAvulsa = null;
            SessionMocks.listaMovimentoProduto = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        //INSUMO
        [HttpGet]
        public ActionResult ExcluirMovimentoInsumo(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = SessionMocks.UserCredentials;

            // Monta movimento
            var item = moemApp.GetById(id);
            MOVIMENTO_ESTOQUE_MATERIA_PRIMA mov = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
            mov.ASSI_CD_ID = item.ASSI_CD_ID;
            mov.FILI_CD_ID = item.FILI_CD_ID;
            mov.MATR_CD_ID = item.MATR_CD_ID;
            mov.MOEM_CD_ID = item.MOEM_CD_ID;
            mov.MOEM_DS_JUSTIFICATIVA = item.MOEM_DS_JUSTIFICATIVA;
            mov.MOEM_DT_MOVIMENTO = item.MOEM_DT_MOVIMENTO;
            mov.MOEM_IN_ATIVO = 0;
            mov.MOEM_IN_CHAVE_ORIGEM = item.MOEM_IN_CHAVE_ORIGEM;
            mov.MOEM_NM_ORIGEM = item.MOEM_NM_ORIGEM;
            mov.MOEM_IN_TIPO_MOVIMENTO = item.MOEM_IN_TIPO_MOVIMENTO;
            mov.MOEM_QN_ANTES = item.MOEM_QN_ANTES;
            mov.MOEM_QN_DEPOIS = item.MOEM_QN_DEPOIS;
            mov.MOEM_QN_QUANTIDADE = item.MOEM_QN_QUANTIDADE;
            mov.MAPR_CD_ID = item.MAPR_CD_ID;
            mov.USUA_CD_ID = item.USUA_CD_ID;
            mov.MOEM_IN_OPERACAO = item.MOEM_IN_OPERACAO;

            //Monta Estoque filial
            var mef = mefApp.GetByInsFilial(item.MAPR_CD_ID, (Int32)item.FILI_CD_ID);
            if (mef != null)
            {
                MATERIA_PRIMA_ESTOQUE_FILIAL mef1 = new MATERIA_PRIMA_ESTOQUE_FILIAL();
                mef1.FILI_CD_ID = mef.FILI_CD_ID;
                mef1.MPFE_CD_ID = mef.MPFE_CD_ID;
                mef1.MPFE_DS_JUSTIFICATIVA = mef.MPFE_DS_JUSTIFICATIVA;
                mef1.MPFE_DT_ULTIMO_MOVIMENTO = mef.MPFE_DT_ULTIMO_MOVIMENTO;
                mef1.MPFE_IN_ATIVO = mef.MPFE_IN_ATIVO;
                mef1.MPFE_QN_ESTOQUE = mef.MPFE_QN_ESTOQUE;
                mef1.MPFE_QN_QUANTIDADE_ALTERADA = mef.MPFE_QN_QUANTIDADE_ALTERADA;
                mef1.MAPR_CD_ID = mef.MAPR_CD_ID;

                //Executa Operações
                if (mov.MOEM_IN_TIPO_MOVIMENTO == 1 || mov.MOEM_IN_TIPO_MOVIMENTO == 4)
                {
                    mef1.MPFE_QN_ESTOQUE = mef1.MPFE_QN_ESTOQUE - (Int32)mov.MOEM_QN_QUANTIDADE;
                    Int32 ve = mefApp.ValidateEdit(mef1, mef, usuario);
                }
                else
                {
                    mef1.MPFE_QN_ESTOQUE = mef1.MPFE_QN_ESTOQUE + (Int32)mov.MOEM_QN_QUANTIDADE;
                    Int32 vs = mefApp.ValidateEdit(mef1, mef, usuario);
                }
            }
            Int32 volta = moemApp.ValidateDelete(mov, usuario);
            SessionMocks.flagMovmtAvulsa = null;
            SessionMocks.listaMovimentoInsumo = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult ReativarMovimentoInsumo(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = SessionMocks.UserCredentials;

            // Monta movimento
            var item = moemApp.GetById(id);
            MOVIMENTO_ESTOQUE_MATERIA_PRIMA mov = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
            mov.ASSI_CD_ID = item.ASSI_CD_ID;
            mov.FILI_CD_ID = item.FILI_CD_ID;
            mov.MATR_CD_ID = item.MATR_CD_ID;
            mov.MOEM_CD_ID = item.MOEM_CD_ID;
            mov.MOEM_DS_JUSTIFICATIVA = item.MOEM_DS_JUSTIFICATIVA;
            mov.MOEM_DT_MOVIMENTO = item.MOEM_DT_MOVIMENTO;
            mov.MOEM_IN_ATIVO = 1;
            mov.MOEM_IN_CHAVE_ORIGEM = item.MOEM_IN_CHAVE_ORIGEM;
            mov.MOEM_NM_ORIGEM = item.MOEM_NM_ORIGEM;
            mov.MOEM_IN_TIPO_MOVIMENTO = item.MOEM_IN_TIPO_MOVIMENTO;
            mov.MOEM_QN_ANTES = item.MOEM_QN_ANTES;
            mov.MOEM_QN_DEPOIS = item.MOEM_QN_DEPOIS;
            mov.MOEM_QN_QUANTIDADE = item.MOEM_QN_QUANTIDADE;
            mov.MAPR_CD_ID = item.MAPR_CD_ID;
            mov.USUA_CD_ID = item.USUA_CD_ID;
            mov.MOEM_IN_OPERACAO = item.MOEM_IN_OPERACAO;

            //Monta Estoque filial
            var mef = mefApp.GetByInsFilial(item.MAPR_CD_ID, (Int32)item.FILI_CD_ID);
            MATERIA_PRIMA_ESTOQUE_FILIAL mef1 = new MATERIA_PRIMA_ESTOQUE_FILIAL();
            mef1.FILI_CD_ID = mef.FILI_CD_ID;
            mef1.MPFE_CD_ID = mef.MPFE_CD_ID;
            mef1.MPFE_DS_JUSTIFICATIVA = mef.MPFE_DS_JUSTIFICATIVA;
            mef1.MPFE_DT_ULTIMO_MOVIMENTO = mef.MPFE_DT_ULTIMO_MOVIMENTO;
            mef1.MPFE_IN_ATIVO = mef.MPFE_IN_ATIVO;
            mef1.MPFE_QN_ESTOQUE = mef.MPFE_QN_ESTOQUE;
            mef1.MPFE_QN_QUANTIDADE_ALTERADA = mef.MPFE_QN_QUANTIDADE_ALTERADA;
            mef1.MAPR_CD_ID = mef.MAPR_CD_ID;

            //Executa Operações
            if (mov.MOEM_IN_TIPO_MOVIMENTO == 1 || mov.MOEM_IN_TIPO_MOVIMENTO == 4)
            {
                mef1.MPFE_QN_ESTOQUE = mef1.MPFE_QN_ESTOQUE + (Int32)mov.MOEM_QN_QUANTIDADE;
                Int32 ve = mefApp.ValidateEdit(mef1, mef, usuario);
            }
            else
            {
                mef1.MPFE_QN_ESTOQUE = mef1.MPFE_QN_ESTOQUE - (Int32)mov.MOEM_QN_QUANTIDADE;
                Int32 vs = mefApp.ValidateEdit(mef1, mef, usuario);
            }
            Int32 volta = moemApp.ValidateReativar(item, usuario);
            SessionMocks.flagMovmtAvulsa = null;
            SessionMocks.listaMovimentoInsumo = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult IncluirMovimentacaoAvulsa()
        {
            
            var usuario = SessionMocks.UserCredentials;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            ViewBag.Title = "Movimentações Avulsas - Lançamento";
            List<SelectListItem> prodIns = new List<SelectListItem>();
            prodIns.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            prodIns.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                lista.Add(new SelectListItem() { Text = "Zeramento de Estoque", Value = "3" });
                lista.Add(new SelectListItem() { Text = "Transferência entre Filiais", Value = "4" });
            }
            lista.Add(new SelectListItem() { Text = "Compra Expressa", Value = "5" });
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.ProdutoInsumo = new SelectList(prodIns, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.ListaProdutos = new SelectList(prodApp.GetAllItens().Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.ListaInsumos = new SelectList(insApp.GetAllItens().OrderBy(x => x.MAPR_NM_NOME).ToList<MATERIA_PRIMA>(), "MAPR_CD_ID", "MAPR_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens().OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");

            MovimentacaoAvulsaViewModel vm = new MovimentacaoAvulsaViewModel();
            vm.MOVMT_DT_MOVIMENTO = DateTime.Now;
            if (Session["MovAvulsaVM"] != null)
            {
                vm = (MovimentacaoAvulsaViewModel)Session["MovAvulsaVM"];
                Session["MovAvulsaVM"] = null;
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirMovimentacaoAvulsa(MovimentacaoAvulsaViewModel vm)
        {
            var usuario = SessionMocks.UserCredentials;
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            ViewBag.Title = "Lançamentos - Produtos";
            List<SelectListItem> prodIns = new List<SelectListItem>();
            prodIns.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            prodIns.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                lista.Add(new SelectListItem() { Text = "Zeramento de Estoque", Value = "3" });
                lista.Add(new SelectListItem() { Text = "Transferência entre Filiais", Value = "4" });
            }
            ViewBag.ListaMovimentoProd = SessionMocks.listaMovimentoProduto;
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.ProdutoInsumo = new SelectList(prodIns, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.ListaProdutos = new SelectList(prodApp.GetAllItens().Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.ListaInsumos = new SelectList(insApp.GetAllItens(), "MAPR_CD_ID", "MAPR_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens().OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");

            var listaMvmtProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            var listaMvmtIns = new List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA>();

            if (ModelState.IsValid)
            {
                if (vm.REGISTROS == null)
                {
                    ModelState.AddModelError("", "Nenhum registro adicionado para inclusão");
                    return View(vm);
                }

                if (usuario.PERF_CD_ID != 1 && usuario.PERF_CD_ID != 2)
                {
                    vm.FILI_CD_ID = SessionMocks.idFilial;
                }

                if (vm.ProdutoInsumo == 1)
                {
                    for (var i = 0; i < vm.REGISTROS.Count(); i++)
                    {
                        try
                        {
                            Int32 qnAntes = 0;
                            MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial(vm.REGISTROS[i], vm.FILI_CD_ID);

                            if (pef == null)
                            {
                                pef = new PRODUTO_ESTOQUE_FILIAL();
                                pef.FILI_CD_ID = vm.FILI_CD_ID;
                                pef.PROD_CD_ID = vm.REGISTROS[i];
                                pef.PREF_QN_QUANTIDADE_ALTERADA = 0;
                                pef.PREF_QN_ESTOQUE = 0;
                                pef.PREF_IN_ATIVO = 1;
                                pef.PREF_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                Int32 v = pefApp.ValidateCreate(pef, usuario);
                                pef = pefApp.GetByProdFilial(vm.REGISTROS[i], vm.FILI_CD_ID);
                            }

                            PRODUTO_ESTOQUE_FILIAL pefAntes = pef;
                            qnAntes = pef.PREF_QN_ESTOQUE == null ? 0 : (Int32)pef.PREF_QN_ESTOQUE;

                            if (vm.MOVMT_IN_OPERACAO == 1)
                            {
                                if (vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA == null)
                                {
                                    ModelState.AddModelError("", "Campo TIPO DE MOVIMENTAÇÃO obrigatorio");
                                    return View(vm);
                                }

                                pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pef.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE + pef.PREF_QN_QUANTIDADE_ALTERADA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 2)
                            {
                                if (vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA == null)
                                {
                                    ModelState.AddModelError("", "Campo TIPO DE MOVIMENTAÇÃO obrigatorio");
                                    return View(vm);
                                }
                                pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pef.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE - pef.PREF_QN_QUANTIDADE_ALTERADA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 3)
                            {
                                pef.PREF_QN_QUANTIDADE_ALTERADA = pef.PREF_QN_ESTOQUE;
                                if (pef.PREF_QN_ESTOQUE < 0)
                                {
                                    vm.MOVMT_IN_OPERACAO = 1;
                                }
                                else
                                {
                                    vm.MOVMT_IN_OPERACAO = 2;
                                }
                                pef.PREF_QN_ESTOQUE = 0;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 4)
                            {
                                PRODUTO_ESTOQUE_FILIAL pefDestino = pefApp.GetByProdFilial(vm.REGISTROS[i], (Int32)vm.FILI_DESTINO_CD_ID);

                                if (pefDestino == null)
                                {
                                    pefDestino = new PRODUTO_ESTOQUE_FILIAL();
                                    pefDestino.FILI_CD_ID = vm.FILI_CD_ID;
                                    pefDestino.PROD_CD_ID = vm.REGISTROS[i];
                                    pefDestino.PREF_QN_ESTOQUE = 0;
                                    pefDestino.PREF_QN_QUANTIDADE_ALTERADA = 0;
                                    pefDestino.PREF_IN_ATIVO = 1;
                                    pefDestino.PREF_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                                    pefDestino.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                    Int32 v = pefApp.ValidateCreate(pefDestino, usuario);
                                    pefDestino = pefApp.CheckExist(pefDestino);
                                }
                                PRODUTO_ESTOQUE_FILIAL pefDestinoAntes = pefDestino;
                                qnAntes = pefDestino.PREF_QN_ESTOQUE == null ? 0 : (Int32)pefDestino.PREF_QN_ESTOQUE;
                                pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pef.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE - pef.PREF_QN_QUANTIDADE_ALTERADA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                pefDestino.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pefDestino.PREF_QN_ESTOQUE = pefDestino.PREF_QN_ESTOQUE + pefDestino.PREF_QN_QUANTIDADE_ALTERADA;
                                pefDestino.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                Int32 voltaPefDestino = pefApp.ValidateEdit(pefDestino, pefDestinoAntes, usuario);
                            }

                            mov.MOEP_QN_ANTES = qnAntes;
                            mov.MOEP_QN_DEPOIS = pef.PREF_QN_ESTOQUE;
                            mov.MOEP_IN_OPERACAO = vm.MOVMT_IN_OPERACAO;
                            if (vm.MOVMT_IN_OPERACAO == 1)
                            {
                                mov.MOEP_IN_TIPO_MOVIMENTO = (Int32)vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA;
                                mov.MOEP_IN_ORIGEM = GetTipoEntrada().Where(x => x.Value == vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA.ToString()).Select(x => x.Text).First();
                            }
                            else if (vm.MOVMT_IN_OPERACAO != 4)
                            {
                                mov.MOEP_IN_TIPO_MOVIMENTO = (Int32)vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA;
                                mov.MOEP_IN_ORIGEM = GetTipoSaida().Where(x => x.Value == vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA.ToString()).Select(x => x.Text).First();
                            }
                            else
                            {
                                mov.MOEP_IN_TIPO_MOVIMENTO = 4;
                                mov.MOEP_IN_ORIGEM = "Tranferência entre filiais";
                            }
                            mov.MOEP_IN_CHAVE_ORIGEM = 1;

                            mov.PROD_CD_ID = vm.REGISTROS[i];
                            mov.MOEP_QN_QUANTIDADE = vm.QUANTIDADE[i];
                            mov.MOEP_DT_MOVIMENTO = vm.MOVMT_DT_MOVIMENTO;
                            mov.FILI_CD_ID = vm.FILI_CD_ID;
                            mov.MOEP_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                            mov.MOEP_IN_ATIVO = 1;
                            mov.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            mov.USUA_CD_ID = usuario.USUA_CD_ID;
                            mov.MOEP_QN_ANTES = 0;
                            mov.MOEP_QN_ALTERADA = vm.QUANTIDADE[i];
                            mov.MOEP_QN_DEPOIS = vm.QUANTIDADE[i];
                            listaMvmtProd.Add(mov);

                            Int32 voltaPef = pefApp.ValidateEdit(pef, pefAntes, usuario);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = ex.Message;
                            return View();
                        }
                    }

                    Int32 volta = moepApp.ValidateCreateLista(listaMvmtProd);

                    foreach (var lp in listaMvmtProd)
                    {
                        lp.PRODUTO = prodApp.GetById(lp.PROD_CD_ID);
                        lp.FILIAL = filApp.GetById(lp.FILI_CD_ID);
                    }

                    ViewBag.flagProdIns = 1;
                    ViewBag.ListaMovimento = listaMvmtProd;
                    SessionMocks.listaMovimentoProduto = listaMvmtProd;

                    if (vm.btnVolta == null)
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaMovimentacaoAvulsa");
                    }
                }
                else
                {
                    for (var j = 0; j < vm.REGISTROS.Count(); j++)
                    {
                        try
                        {
                            MOVIMENTO_ESTOQUE_MATERIA_PRIMA mov = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
                            MATERIA_PRIMA_ESTOQUE_FILIAL pef = mefApp.GetByInsFilial((Int32)vm.REGISTROS[j], vm.FILI_CD_ID);

                            if (pef == null)
                            {
                                pef = new MATERIA_PRIMA_ESTOQUE_FILIAL();
                                pef.FILI_CD_ID = vm.FILI_CD_ID;
                                pef.MAPR_CD_ID = vm.REGISTROS[j];
                                pef.MPFE_IN_ATIVO = 1;
                                pef.MPFE_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                                pef.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                Int32 v = mefApp.ValidateCreate(pef, usuario);
                                pef = mefApp.GetByInsFilial(vm.REGISTROS[j], vm.FILI_CD_ID);
                            }

                            MATERIA_PRIMA_ESTOQUE_FILIAL pefAntes = pef;

                            if (vm.MOVMT_IN_OPERACAO == 1)
                            {
                                if (vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA == null)
                                {
                                    ModelState.AddModelError("", "Campo TIPO DE MOVIMENTAÇÃO obrigatorio");
                                    return View(vm);
                                }

                                pef.MPFE_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[j];
                                pef.MPFE_QN_ESTOQUE = pef.MPFE_QN_ESTOQUE + (Int32)pef.MPFE_QN_QUANTIDADE_ALTERADA;
                                pef.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 2)
                            {
                                if (vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA == null)
                                {
                                    ModelState.AddModelError("", "Campo TIPO DE MOVIMENTAÇÃO obrigatorio");
                                    return View(vm);
                                }

                                pef.MPFE_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[j];
                                pef.MPFE_QN_ESTOQUE = pef.MPFE_QN_ESTOQUE - (Int32)pef.MPFE_QN_QUANTIDADE_ALTERADA;
                                pef.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 3)
                            {
                                pef.MPFE_QN_QUANTIDADE_ALTERADA = pef.MPFE_QN_ESTOQUE;
                                if (pef.MPFE_QN_ESTOQUE < 0)
                                {
                                    vm.MOVMT_IN_OPERACAO = 1;
                                }
                                else
                                {
                                    vm.MOVMT_IN_OPERACAO = 2;
                                }
                                pef.MPFE_QN_ESTOQUE = 0;
                                pef.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 4)
                            {
                                MATERIA_PRIMA_ESTOQUE_FILIAL pefDestino = mefApp.GetByInsFilial(vm.REGISTROS[j], (Int32)vm.FILI_DESTINO_CD_ID);

                                if (pefDestino == null)
                                {
                                    pefDestino = new MATERIA_PRIMA_ESTOQUE_FILIAL();
                                    pefDestino.FILI_CD_ID = vm.FILI_CD_ID;
                                    pefDestino.MAPR_CD_ID = vm.REGISTROS[j];
                                    pefDestino.MPFE_IN_ATIVO = 1;
                                    pefDestino.MPFE_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                                    pefDestino.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                    Int32 v = mefApp.ValidateCreate(pefDestino, usuario);
                                    pefDestino = mefApp.GetByInsFilial(vm.REGISTROS[j], (Int32)vm.FILI_DESTINO_CD_ID);
                                }

                                MATERIA_PRIMA_ESTOQUE_FILIAL pefDestinoAntes = pefDestino;
                                pef.MPFE_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[j];
                                pef.MPFE_QN_ESTOQUE = pef.MPFE_QN_ESTOQUE - (Int32)pef.MPFE_QN_QUANTIDADE_ALTERADA;
                                pef.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                pefDestino.MPFE_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[j];
                                pefDestino.MPFE_QN_ESTOQUE = pefDestino.MPFE_QN_ESTOQUE - (Int32)pefDestino.MPFE_QN_QUANTIDADE_ALTERADA;
                                pefDestino.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                Int32 voltaPefDestino = mefApp.ValidateEdit(pefDestino, pefDestinoAntes, usuario);
                            }

                            mov.MOEM_IN_OPERACAO = vm.MOVMT_IN_OPERACAO;
                            if (vm.MOVMT_IN_OPERACAO == 1)
                            {
                                mov.MOEM_IN_TIPO_MOVIMENTO = (Int32)vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA;
                                mov.MOEM_NM_ORIGEM = GetTipoEntrada().Where(x => x.Value == vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA.ToString()).Select(x => x.Text).First();

                            }
                            else if (vm.MOVMT_IN_OPERACAO != 4)
                            {
                                mov.MOEM_IN_TIPO_MOVIMENTO = (Int32)vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA;
                                mov.MOEM_NM_ORIGEM = GetTipoSaida().Where(x => x.Value == vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA.ToString()).Select(x => x.Text).First();
                            }
                            else
                            {
                                mov.MOEM_IN_TIPO_MOVIMENTO = 4;
                                mov.MOEM_NM_ORIGEM = "Tranferência entre filiais";
                            }
                            mov.MOEM_IN_CHAVE_ORIGEM = 1;

                            mov.MAPR_CD_ID = vm.REGISTROS[j];
                            mov.MOEM_QN_QUANTIDADE = vm.QUANTIDADE[j];
                            mov.MOEM_DT_MOVIMENTO = vm.MOVMT_DT_MOVIMENTO;
                            mov.FILI_CD_ID = vm.FILI_CD_ID;
                            mov.MOEM_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                            mov.MOEM_IN_ATIVO = 1;
                            mov.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            mov.USUA_CD_ID = usuario.USUA_CD_ID;
                            mov.MOEM_QN_ANTES = 0;
                            mov.MOEM_QN_ALTERADA = vm.QUANTIDADE[j];
                            mov.MOEM_QN_DEPOIS = vm.QUANTIDADE[j];
                            listaMvmtIns.Add(mov);

                            Int32 voltaPef = mefApp.ValidateEdit(pef, pefAntes, usuario);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = ex.Message;
                            return View();
                        }
                    }

                    Int32 volta = moemApp.ValidateCreateLista(listaMvmtIns);

                    foreach (var li in listaMvmtIns)
                    {
                        li.MATERIA_PRIMA = insApp.GetById(li.MAPR_CD_ID);
                        li.FILIAL = filApp.GetById(li.FILI_CD_ID);
                    }

                    ViewBag.flagProdIns = 2;
                    ViewBag.ListaMovimento = listaMvmtIns;
                    SessionMocks.listaMovimentoInsumo = listaMvmtIns;

                    if (vm.btnVolta == null)
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaMovimentacaoAvulsa");
                    }
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult IncluirCompraExpressa(MovimentacaoAvulsaViewModel vm)
        {
            

            Session["MovAvulsaVM"] = vm;
            USUARIO usuario = new USUARIO();

            if (vm.ProdutoInsumoEx == 1)
            {
                if (vm.PROD_CD_ID == null)
                {
                    Session["MensCompraEx"] = 1;
                    return RedirectToAction("IncluirMovimentacaoAvulsa");
                }
                if (vm.PROD_CD_ID == null)
                {
                    Session["MensCompraEx"] = 2;
                    return RedirectToAction("IncluirMovimentacaoAvulsa");
                }

                PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial((Int32)vm.PROD_CD_ID, (Int32)vm.FILI_CD_ID_EX);
                PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();

                Int32 qnAntes = 0;

                if (pef == null)
                {
                    pef = new PRODUTO_ESTOQUE_FILIAL();
                    pef.FILI_CD_ID = vm.FILI_CD_ID_EX;
                    pef.PROD_CD_ID = (Int32)vm.PROD_CD_ID;
                    pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
                    pef.PREF_QN_ESTOQUE = vm.QTDE_PROD;
                    pef.PREF_IN_ATIVO = 1;
                    pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                    Int32 v = pefApp.ValidateCreate(pef, usuario);
                } 
                else
                {
                    pef1.PREF_CD_ID = pef.PREF_CD_ID;
                    pef1.FILI_CD_ID = pef.FILI_CD_ID;
                    pef1.PROD_CD_ID = pef.PROD_CD_ID;
                    pef1.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE + vm.QTDE_PROD;
                    pef1.PREF_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
                    pef1.PREF_IN_ATIVO = 1;
                    pef1.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                    pef1.PREF_DS_JUSTIFICATIVA = pef.PREF_DS_JUSTIFICATIVA;
                    pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;

                    Int32 v = pefApp.ValidateEdit(pef1, pef, usuario);

                    qnAntes = (Int32)pef.PREF_QN_ESTOQUE;
                }

                MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                mov.ASSI_CD_ID = SessionMocks.IdAssinante;
                mov.FILI_CD_ID = pef.FILI_CD_ID;
                mov.MOEP_DT_MOVIMENTO = DateTime.Now;
                mov.MOEP_IN_ATIVO = 1;
                mov.MOEP_IN_CHAVE_ORIGEM = 5;
                mov.MOEP_IN_OPERACAO = 1;
                mov.MOEP_IN_ORIGEM = "Compra Expressa";
                mov.MOEP_IN_TIPO_MOVIMENTO = 0;
                mov.MOEP_QN_ALTERADA = vm.QTDE_PROD;
                mov.MOEP_QN_ANTES = qnAntes;
                mov.MOEP_QN_DEPOIS = vm.QTDE_PROD;
                mov.MOEP_QN_QUANTIDADE = (Int32)vm.QTDE_PROD;
                mov.PROD_CD_ID = (Int32)vm.PROD_CD_ID;
                mov.USUA_CD_ID = usuario.USUA_CD_ID;

                Int32 volta = moepApp.ValidateCreate(mov, usuario);
            }
            else if (vm.ProdutoInsumoEx == 2)
            {
                if (vm.MAPR_CD_ID == null)
                {
                    Session["MensCompraEx"] = 3;
                    return RedirectToAction("IncluirMovimentacaoAvulsa");
                }
                if (vm.QTDE_MAPR == null)
                {
                    Session["MensCompraEx"] = 4;
                    return RedirectToAction("IncluirMovimentacaoAvulsa");
                }

                MATERIA_PRIMA_ESTOQUE_FILIAL mef = mefApp.GetByInsFilial((Int32)vm.MAPR_CD_ID, (Int32)vm.FILI_CD_ID_EX);
                MATERIA_PRIMA_ESTOQUE_FILIAL mef1 = new MATERIA_PRIMA_ESTOQUE_FILIAL();

                Int32 qnAntes = 0;

                if (mef == null)
                {
                    mef = new MATERIA_PRIMA_ESTOQUE_FILIAL();
                    mef.FILI_CD_ID = vm.FILI_CD_ID_EX;
                    mef.MAPR_CD_ID = (Int32)vm.PROD_CD_ID;
                    mef.MPFE_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
                    mef.MPFE_QN_ESTOQUE = (Int32)vm.QTDE_MAPR;
                    mef.MPFE_IN_ATIVO = 1;
                    mef.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                    Int32 v = mefApp.ValidateCreate(mef, usuario);
                }
                else
                {
                    mef1.MPFE_CD_ID = mef.MPFE_CD_ID;
                    mef1.FILI_CD_ID = mef.FILI_CD_ID;
                    mef1.MAPR_CD_ID = mef.MAPR_CD_ID;
                    mef1.MPFE_QN_ESTOQUE = mef.MPFE_QN_ESTOQUE + (Int32)vm.QTDE_MAPR;
                    mef1.MPFE_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
                    mef1.MPFE_IN_ATIVO = 1;
                    mef1.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                    mef1.MPFE_DS_JUSTIFICATIVA = mef.MPFE_DS_JUSTIFICATIVA;

                    Int32 v = mefApp.ValidateEdit(mef1, mef, usuario);

                    qnAntes = (Int32)mef.MPFE_QN_ESTOQUE;
                }

                MOVIMENTO_ESTOQUE_MATERIA_PRIMA mov = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
                mov.ASSI_CD_ID = SessionMocks.IdAssinante;
                mov.FILI_CD_ID = mef.FILI_CD_ID;
                mov.MOEM_DT_MOVIMENTO = DateTime.Now;
                mov.MOEM_IN_ATIVO = 1;
                mov.MOEM_IN_CHAVE_ORIGEM = 5;
                mov.MOEM_IN_OPERACAO = 1;
                mov.MOEM_NM_ORIGEM = "Compra Expressa";
                mov.MOEM_IN_TIPO_MOVIMENTO = 0;
                mov.MOEM_QN_ALTERADA = vm.QTDE_PROD;
                mov.MOEM_QN_ANTES = qnAntes;
                mov.MOEM_QN_DEPOIS = vm.QTDE_PROD;
                mov.MOEM_QN_QUANTIDADE = (Int32)vm.QTDE_PROD;
                mov.MAPR_CD_ID = (Int32)vm.PROD_CD_ID;
                mov.USUA_CD_ID = usuario.USUA_CD_ID;

                Int32 volta = moemApp.ValidateCreate(mov, usuario);
            }

            CONTA_PAGAR cp = new CONTA_PAGAR();
            cp.FORN_CD_ID = vm.FORN_CD_ID;
            cp.USUA_CD_ID = usuario.USUA_CD_ID;
            cp.CAPA_DT_LANCAMENTO = DateTime.Now;
            cp.CAPA_DT_COMPETENCIA = DateTime.Now;
            cp.CAPA_DT_VENCIMENTO = DateTime.Now.AddDays(30);

            cp.ASSI_CD_ID = SessionMocks.IdAssinante;
            cp.CAPA_IN_ATIVO = 1;
            cp.CAPA_IN_LIQUIDADA = 0;
            cp.CAPA_IN_PAGA_PARCIAL = 0;
            cp.CAPA_IN_PARCELADA = 0;
            cp.CAPA_IN_PARCELAS = 0;
            cp.CAPA_VL_SALDO = 0;
            cp.CAPA_IN_CHEQUE = 0;

            SessionMocks.contaPagar = cp;
            SessionMocks.voltaCompra = 2;

            return RedirectToAction("IncluirCP", "ContaPagar");
        }

        public ActionResult GerarRelatorioMovimentacaoAvulsaFiltro()
        {
            
            return RedirectToAction("GerarRelatorioMovimentacaoAvulsa", new { id = 1 });
        }

        [HttpGet]
        public ActionResult GerarRelatorioMovimentacaoAvulsa(Int32 id)
        {
            
            MovimentacaoAvulsaGridViewModel vm = (MovimentacaoAvulsaGridViewModel)Session["FiltroMovimentacaoAvulsa"];

            Int32 filtroProdIns = vm.ProdutoInsumo == null ? 1 : (Int32)vm.ProdutoInsumo;

            var listaMvmtProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            var listaMvmtIns = new List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA>();
            var filtroProd = new MOVIMENTO_ESTOQUE_PRODUTO();
            var filtroIns = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "MvmtAvulsaLista" + "_" + data + ".pdf";
            String titulo = "Movimentações Avulsas - Listagem";
            String operacao = String.Empty;
            String tipomvmt = String.Empty;
            String operacaoFiltro = String.Empty;
            String tipomvmtFiltro = String.Empty;
            if (SessionMocks.listaMovimentoProduto != null)
            {
                if (id == 1)
                {
                    listaMvmtProd = SessionMocks.listaMovimentoProduto;
                }
                else
                {
                    return RedirectToAction("MontarTelaMovimentacaoAvulsa");
                }
            }

            if (SessionMocks.listaMovimentoInsumo != null)
            {
                if (id == 1)
                {
                    listaMvmtIns = SessionMocks.listaMovimentoInsumo;
                }
                else
                {
                    return RedirectToAction("MontarTelaMovimentacaoAvulsa");
                }
            }
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

            cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
            table = new PdfPTable(new float[] { 100f, 100f, 150f, 80f, 40f, 100f, 100f, 40f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            String info = String.Empty;
            if (filtroProdIns == 1)
            {
                info = "Produtos selecionados pelos parametros de filtro abaixo";
            }
            else if (filtroProdIns == 2)
            {
                info = "Insumos selecionados pelos parametros de filtro abaixo";
            }

            cell = new PdfPCell(new Paragraph(info, meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Operação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tipo de Movimentação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data do Movimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Produto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Filial", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Antes", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Alterada", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Depois", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            if (filtroProdIns == 1)
            {
                foreach (var prod in listaMvmtProd)
                {
                    if (prod.MOEP_IN_TIPO_MOVIMENTO == 1)
                    {
                        operacao = "Entrada";
                    }
                    else if (prod.MOEP_IN_TIPO_MOVIMENTO == 2)
                    {
                        operacao = "Saída";
                    }
                    else if (prod.MOEP_IN_TIPO_MOVIMENTO == 3)
                    {
                        operacao = "Zeramento de Estoque";
                    }
                    else if (prod.MOEP_IN_TIPO_MOVIMENTO == 4)
                    {
                        operacao = "Transferência entre Filiais";
                    }
                    cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(operacao) ? "-" : operacao, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (prod.MOEP_IN_TIPO_MOVIMENTO == 1)
                    {
                        if (prod.MOEP_IN_CHAVE_ORIGEM == 1)
                        {
                            tipomvmt = "Devolução";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 2)
                        {
                            tipomvmt = "Retorno de conserto";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 2)
                        {
                            tipomvmt = "Reclassificação";
                        }
                        else
                        {
                            tipomvmt = "-";
                        }
                    }
                    else if (prod.MOEP_IN_TIPO_MOVIMENTO == 2)
                    {
                        if (prod.MOEP_IN_CHAVE_ORIGEM == 1)
                        {
                            tipomvmt = "Devolução do fornecedor";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 2)
                        {
                            tipomvmt = "Remessa para conserto";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 3)
                        {
                            tipomvmt = "Baixa de insumos para produção";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 4)
                        {
                            tipomvmt = "Reclassificação";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 5)
                        {
                            tipomvmt = "Perda ou roubo";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 6)
                        {
                            tipomvmt = "Descarte";
                        }
                        else if (prod.MOEP_IN_CHAVE_ORIGEM == 7)
                        {
                            tipomvmt = "Outras saídas";
                        }
                        else
                        {
                            tipomvmt = "-";
                        }
                    }
                    else
                    {
                        tipomvmt = "Tranferência entre filiais";
                    }
                    cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(tipomvmt) ? "-" : tipomvmt, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(prod.MOEP_DT_MOVIMENTO == null ? "-" : prod.MOEP_DT_MOVIMENTO.ToString("dd/MM/yyyy"), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(prod.PRODUTO.PROD_NM_NOME) ? "-" : prod.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (prod.FILIAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(prod.FILIAL.FILI_NM_NOME, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    cell = new PdfPCell(new Paragraph(prod.MOEP_QN_ANTES.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(prod.MOEP_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(prod.MOEP_QN_DEPOIS.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            else if (filtroProdIns == 2)
            {
                foreach (var ins in listaMvmtIns)
                {
                    if (ins.MOEM_IN_TIPO_MOVIMENTO == 1)
                    {
                        operacao = "Entrada";
                    }
                    else if (ins.MOEM_IN_TIPO_MOVIMENTO == 2)
                    {
                        operacao = "Saída";
                    }
                    else if (ins.MOEM_IN_TIPO_MOVIMENTO == 3)
                    {
                        operacao = "Zeramento de Estoque";
                    }
                    else if (ins.MOEM_IN_TIPO_MOVIMENTO == 4)
                    {
                        operacao = "Transferência entre Filiais";
                    }
                    cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(operacao) ? "-" : operacao, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (ins.MOEM_IN_TIPO_MOVIMENTO == 1)
                    {
                        if (ins.MOEM_IN_CHAVE_ORIGEM == 1)
                        {
                            tipomvmt = "Devolução";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 2)
                        {
                            tipomvmt = "Retorno de conserto";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 2)
                        {
                            tipomvmt = "Reclassificação";
                        }
                        else
                        {
                            tipomvmt = "-";
                        }
                    }
                    else if (ins.MOEM_IN_TIPO_MOVIMENTO == 2)
                    {
                        if (ins.MOEM_IN_CHAVE_ORIGEM == 1)
                        {
                            tipomvmt = "Devolução do fornecedor";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 2)
                        {
                            tipomvmt = "Remessa para conserto";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 3)
                        {
                            tipomvmt = "Baixa de insumos para produção";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 4)
                        {
                            tipomvmt = "Reclassificação";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 5)
                        {
                            tipomvmt = "Perda ou roubo";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 6)
                        {
                            tipomvmt = "Descarte";
                        }
                        else if (ins.MOEM_IN_CHAVE_ORIGEM == 7)
                        {
                            tipomvmt = "Outras saídas";
                        }
                        else
                        {
                            tipomvmt = "-";
                        }
                    }
                    else
                    {
                        tipomvmt = "Tranferência entre filiais";
                    }
                    cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(tipomvmt) ? "-" : tipomvmt, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ins.MOEM_DT_MOVIMENTO == null ? "-" : ins.MOEM_DT_MOVIMENTO.ToString("dd/MM/yyyy"), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(ins.MATERIA_PRIMA.MAPR_NM_NOME) ? "-" : ins.MATERIA_PRIMA.MAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (ins.FILIAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(ins.FILIAL.FILI_NM_NOME, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    cell = new PdfPCell(new Paragraph(ins.MOEM_QN_ANTES.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ins.MOEM_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ins.MOEM_QN_DEPOIS.ToString(), meuFont))
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

            if (vm.MOVMT_IN_TIPO_MOVIMENTO != null)
            {
                parametros += "Operacao: " + operacaoFiltro;
                ja = 1;
            }
            if (vm.MOVMT_DT_MOVIMENTO_INICIAL != DateTime.MinValue && vm.MOVMT_DT_MOVIMENTO_INICIAL != null)
            {
                if (ja == 0)
                {
                    parametros += "Data Inicial: " + vm.MOVMT_DT_MOVIMENTO_INICIAL.Value.ToString("dd/MM/yyyy");
                    ja = 1;
                }
                else
                {
                    parametros += "e Data Inicial: " + vm.MOVMT_DT_MOVIMENTO_INICIAL.Value.ToString("dd/MM/yyyy");
                }
            }
            if (vm.MOVMT_DT_MOVIMENTO_FINAL != DateTime.MinValue && vm.MOVMT_DT_MOVIMENTO_FINAL != null)
            {
                if (ja == 0)
                {
                    parametros += "Data Final: " + vm.MOVMT_DT_MOVIMENTO_FINAL.Value.ToString("dd/MM/yyyy");
                    ja = 1;
                }
                else
                {
                    parametros += "e Data Final: " + vm.MOVMT_DT_MOVIMENTO_FINAL.Value.ToString("dd/MM/yyyy");
                }
            }
            if (vm.FILI_CD_ID != null)
            {
                var nmefilial = filApp.GetById(vm.FILI_CD_ID).FILI_NM_NOME;
                if (ja == 0)
                {
                    parametros += "Filial: " + nmefilial;
                    ja = 1;
                }
                else
                {
                    parametros += "e Filial: " + nmefilial;
                }
            }
            else
            {
                if (ja == 0)
                {
                    parametros += "Filial: Sem Filial";
                    ja = 1;
                }
                else
                {
                    parametros += "e Filial: Sem Filial";
                }
            }
            if (vm.MOVMT_IN_CHAVE_ORIGEM != null)
            {
                if (ja == 0)
                {
                    parametros += "Tipo de Movimento: " + tipomvmtFiltro;
                    ja = 1;
                }
                else
                {
                    parametros += "e Tipo de Movimento: " + tipomvmtFiltro;
                }
            }
            if (ja == 0)
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

            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }
    }
}