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
using EntitiesServices.WorkClasses;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class ContaReceberController : Controller
    {
        private readonly IContaReceberAppService crApp;
        private readonly IClienteAppService cliApp;
        private readonly ILogAppService logApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IContaPagarAppService cpApp;
        private readonly IFornecedorAppService forApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly IFormaPagamentoAppService fpApp;
        private readonly IPeriodicidadeAppService perApp;
        private readonly IContaReceberParcelaAppService pcApp;
        private readonly IContaPagarParcelaAppService ppApp;
        private readonly IContaReceberRateioAppService ratApp;

        private String msg;
        private Exception exception;
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao = String.Empty;
        CONTA_RECEBER objetoCR = new CONTA_RECEBER();
        CONTA_RECEBER objetoCRAntes = new CONTA_RECEBER();
        List<CONTA_RECEBER> listaCRMaster = new List<CONTA_RECEBER>();
        CONTA_PAGAR objetoCP = new CONTA_PAGAR();
        CONTA_PAGAR objetoCPAntes = new CONTA_PAGAR();
        List<CONTA_PAGAR> listaCPMaster = new List<CONTA_PAGAR>();
        CONTA_RECEBER_PARCELA objetoCRP = new CONTA_RECEBER_PARCELA();
        CONTA_RECEBER_PARCELA objetoCRPAntes = new CONTA_RECEBER_PARCELA();
        List<CONTA_RECEBER_PARCELA> listaCRPMaster = new List<CONTA_RECEBER_PARCELA>();
        CONTA_PAGAR_PARCELA objetoCPP = new CONTA_PAGAR_PARCELA();
        CONTA_PAGAR_PARCELA objetoCPPAntes = new CONTA_PAGAR_PARCELA();
        List<CONTA_PAGAR_PARCELA> listaCPPMaster = new List<CONTA_PAGAR_PARCELA>();
        CONTA_BANCO contaPadrao = new CONTA_BANCO();

        public ContaReceberController(IContaReceberAppService crApps, ILogAppService logApps, IMatrizAppService matrizApps, IClienteAppService cliApps, IContaPagarAppService cpApps, IFornecedorAppService forApps, ICentroCustoAppService ccApps, IContaBancariaAppService cbApps, IFormaPagamentoAppService fpApps, IPeriodicidadeAppService perApps, IContaReceberParcelaAppService pcApps, IContaPagarParcelaAppService ppApps, IContaReceberRateioAppService ratApps)
        {
            logApp = logApps;
            matrizApp = matrizApps;
            crApp = crApps;
            cliApp = cliApps;
            cpApp = cpApps;
            forApp = forApps;
            ccApp = ccApps;
            cbApp = cbApps;
            fpApp = fpApps;
            perApp = perApps;
            pcApp = pcApps;
            ppApp = ppApps;
            ratApp = ratApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult Voltar()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult IncluirCliente()
        {
            
            SessionMocks.ClienteToCr = true;
            return RedirectToAction("IncluirCliente", "Cliente");
        }

        public ActionResult VoltarDashboard()
        {
            
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            listaCPMaster = new List<CONTA_PAGAR>();
            SessionMocks.listaCP = null;
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpPost]
        public void Liquidar()
        {
            SessionMocks.liquidaCR = 1;
            SessionMocks.parcialCR = 0;
        }

        [HttpPost]
        public void Parcial()
        {
            SessionMocks.parcialCR = 1;
            SessionMocks.liquidaCR = 0;
        }

        [HttpPost]
        public void Parcelamento()
        {
            SessionMocks.parcialCR = 0;
            SessionMocks.liquidaCR = 0;
        }

        public JsonResult GetRateio()
        {
            CONTA_RECEBER item = crApp.GetItemById(SessionMocks.idVolta);
            List<Hashtable> result = new List<Hashtable>();

            if (item.CONTA_RECEBER_RATEIO != null && item.CONTA_RECEBER_RATEIO.Count > 0)
            {
                List<Int32> lstCC = item.CONTA_RECEBER_RATEIO.Select(x => x.CECU_CD_ID).ToList<Int32>();

                foreach (var i in lstCC)
                {
                    Hashtable id = new Hashtable();
                    id.Add("id", i);
                    result.Add(id);
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();

            SessionMocks.Clientes = cliApp.GetAllItens();

            if (nome != null)
            {
                List<CLIENTE> lstCliente = SessionMocks.Clientes.Where(x => x.CLIE_NM_NOME != null && x.CLIE_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = SessionMocks.Clientes.Where(x => x.CLIE_NM_RAZAO != null).ToList<CLIENTE>();
                    lstCliente = lstCliente.Where(x => x.CLIE_NM_RAZAO.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.CLIE_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.CLIE_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.CLIE_NM_NOME + " (" + item.CLIE_NM_RAZAO + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }

            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaCR()
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            usuario = SessionMocks.UserCredentials;

            // Carrega listas
            if (SessionMocks.listaCR == null || SessionMocks.listaCR.Count == 0)
            {
                listaCRMaster = crApp.GetVencimentoAtual();
                SessionMocks.listaCR = listaCRMaster;
            }
            ViewBag.Listas = SessionMocks.listaCR;
            ViewBag.Title = "Contas a Receberxxx";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            SessionMocks.Clientes = cliApp.GetAllItens();
            ViewBag.Clientes = new SelectList(SessionMocks.Clientes, "CLIE_CD_ID", "CLIE_NM_NOME");

            // Indicadores
            Decimal aReceberDia = (Decimal)crApp.GetVencimentoAtual().Where(x => x.CARE_IN_ATIVO == 1 && x.CARE_IN_LIQUIDADA == 0 && x.CARE_DT_VENCIMENTO.Value.Day == DateTime.Now.Day && (x.CONTA_RECEBER_PARCELA == null || x.CONTA_RECEBER_PARCELA.Count == 0)).Sum(x => x.CARE_VL_SALDO);
            aReceberDia += (Decimal)crApp.GetAllItens().Where(x => x.CARE_IN_ATIVO == 1 && x.CARE_IN_LIQUIDADA == 0 && x.CARE_DT_VENCIMENTO.Value.Day == DateTime.Now.Day && x.CONTA_RECEBER_PARCELA != null).SelectMany(x => x.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Day == DateTime.Now.Day).Sum(x => x.CRPA_VL_VALOR);
            ViewBag.CRS = aReceberDia;
            ViewBag.Recebido = crApp.GetAllItens().Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_IN_LIQUIDADA == 1 && p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month).Sum(p => p.CARE_VL_VALOR_LIQUIDADO).Value;
            Decimal sumReceber = crApp.GetAllItens().Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumReceber += (Decimal)crApp.GetAllItens().Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.AReceber = sumReceber;
            Decimal sumAtraso = crApp.GetAllItens().Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumAtraso += (Decimal)crApp.GetAllItens().Where(p => p.CARE_IN_ATIVO == 1 && p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.Atrasos = sumAtraso;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipoFiltro = new List<SelectListItem>();
            tipoFiltro.Add(new SelectListItem() { Text = "Somente em Aberto", Value = "1" });
            tipoFiltro.Add(new SelectListItem() { Text = "Somente Fechados", Value = "2" });
            ViewBag.Filtro = new SelectList(tipoFiltro, "Value", "Text");

            SessionMocks.ContasBancarias = cbApp.GetAllItens();
            if ((Int32)Session["ErroSoma"] == 2)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0083", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["ErroSoma"] == 3)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0017", CultureInfo.CurrentCulture));
            }
            Session["ErroSoma"] = 0;
            
            if (Session["MensCR"] != null && (Int32)Session["MensCR"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }
            else if (SessionMocks.mensVencimentoCR == 1)
            {
                SessionMocks.mensVencimentoCR = 0;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0120", CultureInfo.CurrentCulture));
            }

            if (SessionMocks.voltaCR != 0)
            {
                ViewBag.volta = SessionMocks.voltaCR;
                SessionMocks.voltaCR = 0;
            }
            // Abre view
            objetoCR = new CONTA_RECEBER();
            //objetoCR.CARE_DT_LANCAMENTO = DateTime.Now.Date;
            return View(objetoCR);
        }

        public ActionResult RetirarFiltroCR()
        {
            
            SessionMocks.listaCR = null;
            SessionMocks.filtroCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        public ActionResult MostrarTudoCR()
        {
            
            listaCRMaster = crApp.GetAllItensAdm();
            SessionMocks.filtroCR = null;
            SessionMocks.listaCR = listaCRMaster;
            return RedirectToAction("MontarTelaCR");
        }

        public ActionResult MostrarAtivosCR()
        {
            
            listaCRMaster = crApp.GetAllItensAdm().Where(x => x.CARE_IN_ATIVO == 1).ToList();
            SessionMocks.filtroCR = null;
            SessionMocks.listaCR = listaCRMaster;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpPost]
        public ActionResult FiltrarCR(CONTA_RECEBER item, DateTime? CARE_DT_VENCIMENTO_FINAL)
        {
            
            try
            {
                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                SessionMocks.filtroCR = item;
                if (CARE_DT_VENCIMENTO_FINAL != null)
                {
                    Session["vencFinal"] = CARE_DT_VENCIMENTO_FINAL.Value.ToShortDateString();
                }
                Int32 volta = crApp.ExecuteFilter(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, CARE_DT_VENCIMENTO_FINAL, item.CARE_DS_DESCRICAO, item.CARE_IN_ABERTOS, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCR"] = 1;
                }

                // Sucesso
                listaCRMaster = listaObj;
                SessionMocks.listaCR = listaObj;
                return RedirectToAction("MontarTelaCR");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCR");
            }
        }

        public ActionResult VoltarBaseCR()
        {
            
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            if (SessionMocks.filtroCR != null)
            {
                if (Session["vencFinal"] == null)
                {
                    FiltrarCR(SessionMocks.filtroCR, null);
                }
                else
                {
                    FiltrarCR(SessionMocks.filtroCR, (DateTime)Session["vencFinal"]);
                }
            }
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult VerRecebimentosMes()
        {
            
            if (SessionMocks.listaCRRecebimentoMes == null || SessionMocks.listaCRRecebimentoMes.Count == 0)
            {
                SessionMocks.listaCRRecebimentoMes = crApp.GetAllItens();
            }
            ViewBag.ListaCR = SessionMocks.listaCRRecebimentoMes.Where(p => p.CARE_IN_LIQUIDADA == 1 && p.CARE_DT_DATA_LIQUIDACAO != null && p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month).ToList();
            ViewBag.LR = crApp.GetAllItens().Count;
            ViewBag.Valor = crApp.GetAllItens().Sum(x => x.CARE_VL_VALOR_LIQUIDADO);
            ViewBag.CC = new SelectList(ccApp.GetAllItens().Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");

            if (Session["MensRecebimentoMes"] != null && (Int32)Session["MensRecebimentoMes"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            return View();
        }

        [HttpPost]
        public ActionResult FiltrarRecebimentoMes(CONTA_RECEBER item)
        {
            
            try
            {
                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                SessionMocks.filtroCRRecebimentoMes = item;
                Int32 volta = crApp.ExecuteFilterRecebimentoMes(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DS_DESCRICAO, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, item.CARE_DT_DATA_LIQUIDACAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensRecebimentoMes"] = 1;
                }

                // Sucesso
                SessionMocks.listaCRRecebimentoMes = listaObj;
                return RedirectToAction("VerRecebimentosMes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerRecebimentosMes");
            }
        }

        [HttpGet]
        public ActionResult RetirarFiltroRecMes()
        {
            
            SessionMocks.listaCRRecebimentoMes = new List<CONTA_RECEBER>();
            return RedirectToAction("VerRecebimentosMes");
        }

        public ActionResult GerarRelatorioListaRecMes()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "RecebimentoMesLista" + "_" + data + ".pdf";
            List<CONTA_RECEBER> lista = SessionMocks.listaCRRecebimentoMes.Where(p => p.CARE_IN_LIQUIDADA == 1 && p.CARE_DT_DATA_LIQUIDACAO != null && p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month).ToList();
            CONTA_RECEBER filtro = SessionMocks.filtroCRRecebimentoMes;
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

            cell = new PdfPCell(new Paragraph("Recebimentos do Mês - Listagem", meuFont2))
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
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
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
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Liquidada", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
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
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
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
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
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
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_IN_LIQUIDADA == 1)
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
                if (item.CARE_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_NR_ATRASO.Value.ToString(), meuFont))
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
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CECU_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_DATA_LIQUIDACAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Liquidação: " + filtro.CARE_DT_DATA_LIQUIDACAO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Liquidação: " + filtro.CARE_DT_DATA_LIQUIDACAO.Value.ToShortDateString();
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

            return RedirectToAction("VerRecebimentosMes");
        }

        [HttpGet]
        public ActionResult VerAReceberMes()
        {
            
            if (SessionMocks.listaCRReceberMes == null || SessionMocks.listaCRReceberMes.Count == 0)
            {
                SessionMocks.listaCRReceberMes = crApp.GetAllItens();
            }
            ViewBag.ListaCR = SessionMocks.listaCRReceberMes.Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month).ToList();
            ViewBag.LR = crApp.GetAllItens().Count(x => x.CARE_IN_LIQUIDADA == 0 && x.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month);
            Decimal sumReceber = crApp.GetAllItens().Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumReceber += (Decimal)crApp.GetAllItens().Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && (p.CONTA_RECEBER_PARCELA != null || p.CONTA_RECEBER_PARCELA.Count > 0)).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.Valor = sumReceber;
            ViewBag.CC = new SelectList(ccApp.GetAllItens().Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");

            if (Session["MensAReceberMes"] != null && (Int32)Session["MensAReceberMes"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            return View();
        }

        [HttpPost]
        public ActionResult FiltrarAReceberMes(CONTA_RECEBER item)
        {
            
            try
            {
                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                SessionMocks.filtroCRReceberMes = item;
                Int32 volta = crApp.ExecuteFilterAReceberMes(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DS_DESCRICAO, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAReceberMes"] = 1;
                }

                // Sucesso
                SessionMocks.listaCRReceberMes = listaObj;
                return RedirectToAction("VerAReceberMes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerAReceberMes");
            }
        }

        [HttpGet]
        public ActionResult RetirarFiltroARecMes()
        {
            
            SessionMocks.listaCR = new List<CONTA_RECEBER>();
            return RedirectToAction("VerAReceberMes");
        }

        public ActionResult GerarRelatorioListaARecMes()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AReceberMesLista" + "_" + data + ".pdf";
            List<CONTA_RECEBER> lista = SessionMocks.listaCRReceberMes.Where(p => p.CARE_IN_LIQUIDADA == 0 && p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month).ToList();
            CONTA_RECEBER filtro = SessionMocks.filtroCRReceberMes;
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

            cell = new PdfPCell(new Paragraph("A Receber no Mês - Listagem", meuFont2))
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
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
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
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
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
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
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
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
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
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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
                if (item.CARE_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_NR_ATRASO.Value.ToString(), meuFont))
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
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CECU_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
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

            return RedirectToAction("VerAReceberMes");
        }

        [HttpGet]
        public ActionResult VerLancamentosAtraso()
        {
            
            if (SessionMocks.listaCRLancAtraso == null || SessionMocks.listaCRLancAtraso.Count == 0)
            {
                SessionMocks.listaCRLancAtraso = crApp.GetAllItens();
            }
            ViewBag.ListaCR = SessionMocks.listaCRLancAtraso.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date).ToList();
            ViewBag.LR = SessionMocks.listaCR.Count;
            Decimal sumAtraso = SessionMocks.listaCR.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumAtraso += (Decimal)SessionMocks.listaCR.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date && p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null && x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.Valor = sumAtraso;
            ViewBag.CC = new SelectList(ccApp.GetAllItens().Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");

            if (Session["MensCRAtraso"] != null && (Int32)Session["MensCRAtraso"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            return View();
        }

        [HttpPost]
        public ActionResult FiltrarLancAtraso(CONTA_RECEBER item)
        {
            
            try
            {
                // Executa a operação
                List<CONTA_RECEBER> listaObj = new List<CONTA_RECEBER>();
                SessionMocks.filtroCRLancAtraso = item;
                Int32 volta = crApp.ExecuteFilterCRAtrasos(item.CLIE_CD_ID, item.CECU_CD_ID, item.CARE_DS_DESCRICAO, item.CARE_DT_LANCAMENTO, item.CARE_DT_VENCIMENTO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCRAtraso"] = 1;
                }

                // Sucesso
                SessionMocks.listaCRLancAtraso = listaObj;
                return RedirectToAction("VerLancamentosAtraso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerLancamentosAtraso");
            }
        }

        [HttpGet]
        public ActionResult RetirarFiltroLancAtraso()
        {
            
            SessionMocks.listaCRLancAtraso = new List<CONTA_RECEBER>();
            return RedirectToAction("VerLancamentosAtraso");
        }

        public ActionResult GerarRelatorioListaLancAtraso()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "LancAtrasoLista" + "_" + data + ".pdf";
            List<CONTA_RECEBER> lista = SessionMocks.listaCRLancAtraso.Where(p => p.CARE_NR_ATRASO > 0 && p.CARE_DT_VENCIMENTO < DateTime.Today.Date).ToList();
            CONTA_RECEBER filtro = SessionMocks.filtroCRLancAtraso;
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

            cell = new PdfPCell(new Paragraph("Lançamentos em Atraso - Listagem", meuFont2))
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
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
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
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
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
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
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
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
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
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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
                if (item.CARE_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_NR_ATRASO.Value.ToString(), meuFont))
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
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CECU_CD_ID != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Plano de Contas: " + ccApp.GetItemById(filtro.CECU_CD_ID.Value);
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento: " + filtro.CARE_DT_VENCIMENTO.Value.ToShortDateString();
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

            return RedirectToAction("VerLancamentosAtraso");
        }

        [HttpGet]
        public ActionResult VerCR(Int32 id)
        {
            
            // Prepara view
            CONTA_RECEBER item = crApp.GetItemById(id);
            SessionMocks.contaReceber = item;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            SessionMocks.idVolta = id;
            SessionMocks.idCRVolta = 1;
            return View(vm);
        }


        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();

            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }

                queue.Add(f);
            }

            Session["FileQueueCR"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueLancamentoCR(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                Session["ErroSoma"] = 4;
                return RedirectToAction("VoltarAnexoCR");
            }

            CONTA_RECEBER item = crApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["ErroSoma"] = 5;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoCR");
            }

            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/ContaReceber/" + item.CARE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_RECEBER_ANEXO foto = new CONTA_RECEBER_ANEXO();
            foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRAN_DT_ANEXO = DateTime.Today;
            foto.CRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CRAN_IN_TIPO = tipo;
            foto.CRAN_NM_TITULO = fileName;
            foto.CARE_CD_ID = item.CARE_CD_ID;

            item.CONTA_RECEBER_ANEXO.Add(foto);
            objetoCRAntes = item;
            Int32 volta = crApp.ValidateEdit(item, objetoCRAntes, usu);
            if (SessionMocks.idCRVolta == 1)
            {
                return RedirectToAction("VerCR");
            }
            return RedirectToAction("VoltarAnexoCR");
        }

        [HttpPost]
        public ActionResult UploadFileLancamentoCR(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                Session["ErroSoma"] = 4;
                return RedirectToAction("VoltarAnexoCR");
            }

            CONTA_RECEBER item = crApp.GetById(SessionMocks.idVolta);

            CONTA_RECEBER itemAnx = new CONTA_RECEBER
            {
                CONTA_RECEBER_ANEXO = item.CONTA_RECEBER_ANEXO,
                CONTA_RECEBER_PARCELA = item.CONTA_RECEBER_PARCELA,
                CONTA_RECEBER_RATEIO = item.CONTA_RECEBER_RATEIO,
                CONTA_RECEBER_TAG = item.CONTA_RECEBER_TAG,
                CARE_CD_ID = item.CARE_CD_ID,
                ASSI_CD_ID = item.ASSI_CD_ID,
                MATR_CD_ID = item.MATR_CD_ID,
                FILI_CD_ID = item.FILI_CD_ID,
                USUA_CD_ID = item.USUA_CD_ID,
                CLIE_CD_ID = item.CLIE_CD_ID,
                TIFA_CD_ID = item.TIFA_CD_ID,
                PEVE_CD_ID = item.PEVE_CD_ID,
                COBA_CD_ID = item.COBA_CD_ID,
                PLCO_CD_ID = item.PLCO_CD_ID,
                CARE_DT_LANCAMENTO = item.CARE_DT_LANCAMENTO,
                CARE_VL_VALOR = item.CARE_VL_VALOR,
                CARE_DS_DESCRICAO = item.CARE_DS_DESCRICAO,
                CARE_IN_TIPO_LANCAMENTO = item.CARE_IN_TIPO_LANCAMENTO,
                CARE_NM_FAVORECIDO = item.CARE_NM_FAVORECIDO,
                CARE_NM_FORMA_PAGAMENTO = item.CARE_NM_FORMA_PAGAMENTO,
                CARE_IN_LIQUIDADA = item.CARE_IN_LIQUIDADA,
                CARE_IN_ATIVO = item.CARE_IN_ATIVO,
                CARE_DT_DATA_LIQUIDACAO = item.CARE_DT_DATA_LIQUIDACAO,
                CARE_VL_VALOR_LIQUIDADO = item.CARE_VL_VALOR_LIQUIDADO,
                CARE_DT_VENCIMENTO = item.CARE_DT_VENCIMENTO,
                CARE_NR_ATRASO = item.CARE_NR_ATRASO,
                FOPA_CD_ID = item.FOPA_CD_ID,
                CARE_TX_OBSERVACOES = item.CARE_TX_OBSERVACOES,
                CARE_IN_PARCELADA = item.CARE_IN_PARCELADA,
                CARE_IN_PARCELAS = item.CARE_IN_PARCELAS,
                CARE_DT_INICIO_PARCELA = item.CARE_DT_INICIO_PARCELA,
                PERI_CD_ID = item.PERI_CD_ID,
                CARE_VL_PARCELADO = item.CARE_VL_PARCELADO,
                CARE_NR_DOCUMENTO = item.CARE_NR_DOCUMENTO,
                CARE_DT_COMPETENCIA = item.CARE_DT_COMPETENCIA,
                CARE_VL_DESCONTO = item.CARE_VL_DESCONTO,
                CARE_VL_JUROS = item.CARE_VL_JUROS,
                CARE_VL_TAXAS = item.CARE_VL_TAXAS,
                CECU_CD_ID = item.CECU_CD_ID,
                CARE_VL_SALDO = item.CARE_VL_SALDO,
                CARE_IN_PAGA_PARCIAL = item.CARE_IN_PAGA_PARCIAL,
                CARE_VL_PARCIAL = item.CARE_VL_PARCIAL,
                TITA_CD_ID = item.TITA_CD_ID,
                PEVE_DT_PREVISTA = item.PEVE_DT_PREVISTA,
                CARE_IN_ABERTOS = item.CARE_IN_ABERTOS
            };

            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["ErroSoma"] = 5;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoCR");
            }

            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/ContaReceber/" + itemAnx.CARE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_RECEBER_ANEXO foto = new CONTA_RECEBER_ANEXO();
            foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRAN_DT_ANEXO = DateTime.Today;
            foto.CRAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CRAN_IN_TIPO = tipo;
            foto.CRAN_NM_TITULO = fileName;
            foto.CARE_CD_ID = itemAnx.CARE_CD_ID;

            itemAnx.CONTA_RECEBER_ANEXO.Add(foto);
            objetoCRAntes = item;
            Int32 volta = crApp.ValidateEdit(itemAnx, objetoCRAntes, usu);
            if (SessionMocks.idCRVolta == 1)
            {
                return RedirectToAction("VerCR");
            }
            return RedirectToAction("VoltarAnexoCR");
        }

        [HttpGet]
        public ActionResult VerAnexoLancamentoCR(Int32 id)
        {
            
            // Prepara view
            CONTA_RECEBER_ANEXO item = crApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadLancamentoCR(Int32 id)
        {
            CONTA_RECEBER_ANEXO item = crApp.GetAnexoById(id);
            String arquivo = item.CRAN_AQ_ARQUIVO;
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

        [HttpGet]
        public ActionResult SlideShowCR()
        {
            
            // Prepara view
            CONTA_RECEBER item = crApp.GetItemById(SessionMocks.idVolta);
            objetoCRAntes = item;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirCR(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = SessionMocks.contaReceber;
            item.CARE_IN_ATIVO = 0;
            Int32 volta = crApp.ValidateDelete(item, usu);
            if (volta == 1)
            {
                Session["ErroSoma"] = 2;
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0083", CultureInfo.CurrentCulture));
            }
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult ReativarCR(Int32 id)
        {
            
            USUARIO usu = SessionMocks.UserCredentials;
            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = SessionMocks.contaReceber;
            item.CARE_IN_ATIVO = 1;
            Int32 volta = crApp.ValidateReativar(item, usu);
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult VerParcelaCR(Int32 id)
        {
            
            // Prepara view
            CONTA_RECEBER_PARCELA item = crApp.GetParcelaById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoVerCR()
        {
            
            return RedirectToAction("VerCR", new { id = SessionMocks.idVolta });
        }

        public ActionResult VoltarAnexoCR()
        {
            
            return RedirectToAction("EditarCR", new { id = SessionMocks.idVolta });
        }

        [HttpGet]
        public ActionResult IncluirCR()
        {
            
            // Prepara listas
            ViewBag.Clientes = new SelectList(SessionMocks.Clientes, "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens().Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(SessionMocks.ContasBancarias, "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(2), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            CONTA_RECEBER item = new CONTA_RECEBER();
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.CARE_DT_LANCAMENTO = DateTime.Today.Date;
            vm.CARE_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.FILI_CD_ID = 1;
            vm.CARE_DT_COMPETENCIA = DateTime.Today.Date;
            vm.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            vm.CARE_IN_LIQUIDADA = 0;
            vm.CARE_IN_PAGA_PARCIAL = 0;
            vm.CARE_IN_PARCELADA = 0;
            vm.CARE_IN_PARCELAS = 0;
            vm.CARE_VL_SALDO = 0;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCR(ContaReceberViewModel vm)
        {
            
            ViewBag.Clientes = new SelectList(SessionMocks.Clientes, "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens().Where(x => x.CECU_IN_TIPO == 1).OrderBy(x => x.CECU_NM_NOME).ToList<CENTRO_CUSTO>(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(SessionMocks.ContasBancarias, "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(2), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            List<SelectListItem> tipoRec = new List<SelectListItem>();
            tipoRec.Add(new SelectListItem() { Text = "Recebimento Recorrente", Value = "1" });
            tipoRec.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoRec, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    Int32 recorrencia = vm.CARE_IN_RECORRENTE;
                    DateTime? data = vm.CARE_DT_INICIO_RECORRENCIA;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = crApp.ValidateCreate(item, recorrencia, data, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0095", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0096", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/ContaReceber/" + item.CARE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;

                    SessionMocks.idVolta = item.CARE_CD_ID;

                    if (Session["FileQueueCR"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCR"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueLancamentoCR(file);
                        }

                        Session["FileQueueCR"] = null;
                    }

                    return RedirectToAction("MontarTelaCR");
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
        public ActionResult EditarCR(Int32 id)
        {
            
            // Prepara view
            ViewBag.Clientes = new SelectList(SessionMocks.Clientes, "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllReceitas(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(SessionMocks.ContasBancarias, "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(2), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 0;

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            SessionMocks.contaReceber = item;
            SessionMocks.idVolta = id;
            SessionMocks.idCRVolta = 2;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            if (vm.CARE_IN_PAGA_PARCIAL == 1)
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_SALDO;
            }
            else
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_VALOR;
            }
            if (Session["ErroSoma"] != null && (Int32)Session["ErroSoma"] == 1)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                SessionMocks.idVoltaTab = 3;
            }
            if (Session["ErroSoma"] != null && (Int32)Session["ErroSoma"] == 3)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0086", CultureInfo.CurrentCulture));
                SessionMocks.idVoltaTab = 1;
            }
            if (Session["ErroSoma"] != null && (Int32)Session["ErroSoma"] == 4)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                SessionMocks.idVoltaTab = 1;
            }
            Session["ErroSoma"] = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCR(ContaReceberViewModel vm)
        {
            
            var vmVolta = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(SessionMocks.contaReceber);

            ViewBag.Clientes = new SelectList(SessionMocks.Clientes, "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllReceitas(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(SessionMocks.ContasBancarias, "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(2), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    if (vm.CARE_DT_DATA_LIQUIDACAO == null)
                    {
                        vm.CARE_DT_DATA_LIQUIDACAO = DateTime.Now.Date;
                    }
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    Int32 volta = crApp.ValidateEdit(item, SessionMocks.contaReceber, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0049", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 3)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0050", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 4)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 5)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 6)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 7)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 8)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 9)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }
                    if (volta == 10)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture));
                        return View(vmVolta);
                    }

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;
                    SessionMocks.voltaCR = item.CARE_CD_ID;
                    if (SessionMocks.filtroCR != null)
                    {
                        if (Session["vencFinal"] == null)
                        {
                            FiltrarCR(SessionMocks.filtroCR, null);
                        }
                        else
                        {
                            FiltrarCR(SessionMocks.filtroCR, (DateTime)Session["vencFinal"]);
                        }
                    }
                    return RedirectToAction("MontarTelaCR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vmVolta);
                }
            }
            else
            {
                return View(vmVolta);
            }
        }

        [HttpGet]
        public ActionResult ParcelarCR(Int32 id)
        {
            
            // Prepara view
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            SessionMocks.contaReceber = item;
            SessionMocks.idVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_DT_INICIO_PARCELA = DateTime.Today.Date;
            vm.CARE_IN_PARCELADA = 1;
            vm.CARE_IN_PARCELAS = 2;
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParcelarCR(ContaReceberViewModel vm)
        {
            
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
            {
                    // Processa parcelas
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    DateTime dataParcela = item.CARE_DT_INICIO_PARCELA.Value;
                    PERIODICIDADE period = perApp.GetItemById(item.PERI_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= item.CARE_IN_PARCELAS; i++)
                    {
                        CONTA_RECEBER_PARCELA parc = new CONTA_RECEBER_PARCELA();
                        parc.CARE_CD_ID = item.CARE_CD_ID;
                        parc.CRPA_DT_QUITACAO = null;
                        parc.CRPA_DT_VENCIMENTO = dataParcela;
                        parc.CRPA_IN_ATIVO = 1;
                        parc.CRPA_IN_QUITADA = 0;
                        parc.CRPA_NR_PARCELA = i.ToString() + "/" + item.CARE_IN_PARCELAS.Value.ToString();
                        parc.CRPA_VL_RECEBIDO = 0;
                        parc.CRPA_VL_VALOR = item.CARE_VL_PARCELADO;
                        item.CONTA_RECEBER_PARCELA.Add(parc);
                        dataParcela = dataParcela.AddDays(period.PERI_NR_DIAS);
                    }

                    item.CARE_IN_PARCELADA = 1;
                    objetoCRAntes = item;
                    Int32 volta = crApp.ValidateEdit(item, objetoCRAntes, usuarioLogado);
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;
                    return RedirectToAction("MontarTelaCR");
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

        public ActionResult DuplicarCR()
        {
            
            // Monta novo lançamento
            CONTA_RECEBER item = crApp.GetItemById(SessionMocks.idVolta);
            CONTA_RECEBER novo = new CONTA_RECEBER();
            novo.ASSI_CD_ID = SessionMocks.IdAssinante;
            novo.CARE_DS_DESCRICAO = "Lançamento Duplicado - " + item.CARE_DS_DESCRICAO;
            novo.CARE_DT_COMPETENCIA = item.CARE_DT_COMPETENCIA;
            novo.CARE_DT_LANCAMENTO = DateTime.Today.Date;
            novo.CARE_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            novo.CARE_IN_ATIVO = 1;
            novo.CARE_IN_LIQUIDADA = 0;
            novo.CARE_IN_PAGA_PARCIAL = 0;
            novo.CARE_IN_PARCELADA = 0;
            novo.CARE_IN_PARCELAS = 0;
            novo.CARE_IN_TIPO_LANCAMENTO = 0;
            novo.CARE_NM_FAVORECIDO = item.CARE_NM_FAVORECIDO;
            novo.CARE_NR_DOCUMENTO = item.CARE_NR_DOCUMENTO;
            novo.CARE_TX_OBSERVACOES = item.CARE_TX_OBSERVACOES;
            novo.CARE_VL_DESCONTO = 0;
            novo.CARE_VL_JUROS = 0;
            novo.CARE_VL_PARCELADO = 0;
            novo.CARE_VL_PARCIAL = 0;
            novo.CARE_VL_SALDO = 0;
            novo.CARE_VL_TAXAS = 0;
            novo.CARE_VL_VALOR = item.CARE_VL_VALOR;
            novo.CARE_VL_VALOR_LIQUIDADO = 0;
            novo.CECU_CD_ID = item.CECU_CD_ID;
            novo.CLIE_CD_ID = item.CLIE_CD_ID;
            novo.COBA_CD_ID = item.COBA_CD_ID;
            novo.FILI_CD_ID = item.FILI_CD_ID;
            novo.FOPA_CD_ID = item.FOPA_CD_ID;
            novo.MATR_CD_ID = item.MATR_CD_ID;
            novo.PERI_CD_ID = item.PERI_CD_ID;
            novo.PEVE_CD_ID = item.PEVE_CD_ID;
            novo.PLCO_CD_ID = item.PLCO_CD_ID;
            novo.TIFA_CD_ID = item.TIFA_CD_ID;
            novo.USUA_CD_ID = item.USUA_CD_ID;

            // Grava
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            Int32 volta = crApp.ValidateCreate(novo, 0, null, usuarioLogado);

            // Cria pastas
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/ContaReceber/" + novo.CARE_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));

            // Sucesso
            listaCRMaster = new List<CONTA_RECEBER>();
            SessionMocks.listaCR = null;
            return RedirectToAction("MontarTelaCR");
        }

        [HttpGet]
        public ActionResult LiquidarParcelaCR(Int32 id)
        {
            
            // Prepara view
            CONTA_RECEBER_PARCELA item = pcApp.GetItemById(id);
            objetoCRPAntes = item;
            SessionMocks.contaReceberParcela = item;
            SessionMocks.idVoltaCRP = id;
            SessionMocks.liquidaCR = 0;
            SessionMocks.parcialCR = 0;
            ContaReceberParcelaViewModel vm = Mapper.Map<CONTA_RECEBER_PARCELA, ContaReceberParcelaViewModel>(item);
            vm.CRPA_VL_DESCONTO = 0;
            vm.CRPA_VL_JUROS = 0;
            vm.CRPA_VL_TAXAS = 0;
            vm.CRPA_VL_RECEBIDO = vm.CRPA_VL_VALOR;
            vm.CRPA_DT_QUITACAO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarParcelaCR(ContaReceberParcelaViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER_PARCELA item = Mapper.Map<ContaReceberParcelaViewModel, CONTA_RECEBER_PARCELA>(vm);
                    Int32 volta = pcApp.ValidateEdit(item, SessionMocks.contaReceberParcela, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0119", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Acerta saldo
                    CONTA_RECEBER rec = crApp.GetItemById(SessionMocks.idVolta);
                    CONTA_RECEBER recEdit = new CONTA_RECEBER {
                        CONTA_RECEBER_ANEXO = rec.CONTA_RECEBER_ANEXO,
                        CONTA_RECEBER_PARCELA = rec.CONTA_RECEBER_PARCELA,
                        CONTA_RECEBER_RATEIO = rec.CONTA_RECEBER_RATEIO,
                        CONTA_RECEBER_TAG = rec.CONTA_RECEBER_TAG,
                        CARE_CD_ID = rec.CARE_CD_ID,
                        ASSI_CD_ID = rec.ASSI_CD_ID,
                        MATR_CD_ID = rec.MATR_CD_ID,
                        FILI_CD_ID = rec.FILI_CD_ID,
                        USUA_CD_ID = rec.USUA_CD_ID,
                        CLIE_CD_ID = rec.CLIE_CD_ID,
                        TIFA_CD_ID = rec.TIFA_CD_ID,
                        PEVE_CD_ID = rec.PEVE_CD_ID,
                        COBA_CD_ID = rec.COBA_CD_ID,
                        PLCO_CD_ID = rec.PLCO_CD_ID,
                        CARE_DT_LANCAMENTO = rec.CARE_DT_LANCAMENTO,
                        CARE_VL_VALOR = rec.CARE_VL_VALOR,
                        CARE_DS_DESCRICAO = rec.CARE_DS_DESCRICAO,
                        CARE_IN_TIPO_LANCAMENTO = rec.CARE_IN_TIPO_LANCAMENTO,
                        CARE_NM_FAVORECIDO = rec.CARE_NM_FAVORECIDO,
                        CARE_NM_FORMA_PAGAMENTO = rec.CARE_NM_FORMA_PAGAMENTO,
                        CARE_IN_LIQUIDADA = rec.CARE_IN_LIQUIDADA,
                        CARE_IN_ATIVO = rec.CARE_IN_ATIVO,
                        CARE_DT_DATA_LIQUIDACAO = rec.CARE_DT_DATA_LIQUIDACAO,
                        CARE_VL_VALOR_LIQUIDADO = rec.CARE_VL_VALOR_LIQUIDADO,
                        CARE_DT_VENCIMENTO = rec.CARE_DT_VENCIMENTO,
                        CARE_NR_ATRASO = rec.CARE_NR_ATRASO,
                        CARE_TX_OBSERVACOES = rec.CARE_TX_OBSERVACOES,
                        CARE_IN_PARCELADA = rec.CARE_IN_PARCELADA,
                        CARE_IN_PARCELAS = rec.CARE_IN_PARCELAS,
                        CARE_DT_INICIO_PARCELA = rec.CARE_DT_INICIO_PARCELA,
                        PERI_CD_ID = rec.PERI_CD_ID,
                        CARE_VL_PARCELADO = rec.CARE_VL_PARCELADO,
                        CARE_NR_DOCUMENTO = rec.CARE_NR_DOCUMENTO,
                        CARE_DT_COMPETENCIA = rec.CARE_DT_COMPETENCIA,
                        CARE_VL_DESCONTO = rec.CARE_VL_DESCONTO,
                        CARE_VL_JUROS = rec.CARE_VL_JUROS,
                        CARE_VL_TAXAS = rec.CARE_VL_TAXAS,
                        CECU_CD_ID = rec.CECU_CD_ID,
                        CARE_VL_SALDO = rec.CARE_VL_SALDO,
                        CARE_IN_PAGA_PARCIAL = rec.CARE_IN_PAGA_PARCIAL,
                        CARE_VL_PARCIAL = rec.CARE_VL_PARCIAL,
                        TITA_CD_ID = rec.TITA_CD_ID,
                        PEVE_DT_PREVISTA = rec.PEVE_DT_PREVISTA,
                        CARE_IN_ABERTOS = rec.CARE_IN_ABERTOS,
                        FOPA_CD_ID = rec.FOPA_CD_ID
                    };
                    recEdit.CARE_VL_SALDO = recEdit.CARE_VL_SALDO - item.CRPA_VL_RECEBIDO;                    
                    
                    // Verifica se liquidou todas
                    List<CONTA_RECEBER_PARCELA> lista = recEdit.CONTA_RECEBER_PARCELA.Where(p => p.CRPA_IN_QUITADA == 0).ToList<CONTA_RECEBER_PARCELA>();
                    if (lista.Count == 0)
                    {
                        recEdit.CARE_IN_LIQUIDADA = 1;
                        recEdit.CARE_DT_DATA_LIQUIDACAO = DateTime.Today.Date;
                        recEdit.CARE_VL_VALOR_LIQUIDADO = recEdit.CONTA_RECEBER_PARCELA.Sum(p => p.CRPA_VL_RECEBIDO);
                        recEdit.CARE_VL_SALDO = 0;
                    }

                    recEdit.CARE_VL_VALOR_LIQUIDADO = recEdit.CONTA_RECEBER_PARCELA.Where(p => p.CRPA_IN_QUITADA == 1).Sum(p => p.CRPA_VL_VALOR);

                    volta = crApp.ValidateEdit(recEdit, rec, usuarioLogado);

                    // Sucesso
                    listaCRPMaster = new List<CONTA_RECEBER_PARCELA>();
                    SessionMocks.listaCRP = null;
                    return RedirectToAction("VoltarAnexoCR");
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
        public ActionResult LiquidarCR(Int32 id)
        {
            
            // Prepara view
            ViewBag.Clientes = new SelectList(SessionMocks.Clientes, "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllReceitas(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(SessionMocks.ContasBancarias, "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 1;
            SessionMocks.liquidaCR = 1;

            CONTA_RECEBER item = crApp.GetItemById(id);
            objetoCRAntes = item;
            SessionMocks.contaReceber = item;
            SessionMocks.idVolta = id;
            ContaReceberViewModel vm = Mapper.Map<CONTA_RECEBER, ContaReceberViewModel>(item);
            vm.CARE_VL_PARCELADO = vm.CARE_VL_VALOR;
            if (vm.CARE_IN_PAGA_PARCIAL == 1)
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_SALDO;
            }
            else
            {
                vm.CARE_VL_VALOR_LIQUIDADO = vm.CARE_VL_VALOR;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LiquidarCR(ContaReceberViewModel vm)
        {
            
            ViewBag.Clientes = new SelectList(SessionMocks.Clientes, "CLIE_CD_ID", "CLIE_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllReceitas(), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Contas = new SelectList(SessionMocks.ContasBancarias, "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(SessionMocks.Formas, "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Periodicidade = new SelectList(SessionMocks.Periodicidades, "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Tags = new SelectList(crApp.GetAllTags(), "TITA_CD_ID", "TITA_NM_NOME");
            ViewBag.Liquida = 1;
            SessionMocks.liquidaCR = 1;
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    CONTA_RECEBER item = Mapper.Map<ContaReceberViewModel, CONTA_RECEBER>(vm);
                    Int32 volta = crApp.ValidateEdit(item, SessionMocks.contaReceber, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0049", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0050", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 7)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 8)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0057", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaCRMaster = new List<CONTA_RECEBER>();
                    SessionMocks.listaCR = null;
                    return RedirectToAction("MontarTelaCR");
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

        public ActionResult IncluirRateioCC(ContaReceberViewModel vm)
        {
            
            try
            {
                // Executa a operação
                Int32? cc = vm.CECU_CD_RATEIO;
                Int32? perc = vm.CARE_VL_PERCENTUAL;
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                CONTA_RECEBER item = crApp.GetItemById(vm.CARE_CD_ID);
                Int32 volta = crApp.IncluirRateioCC(item, cc, perc, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["ErroSoma"] = 1;
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                    return RedirectToAction("VoltarAnexoCR");
                }

                // Sucesso
                SessionMocks.idVoltaTab = 2;
                return RedirectToAction("VoltarAnexoCR");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VoltarAnexoCR");
            }
        }

        [HttpGet]
        public ActionResult ExcluirRateio(Int32 id)
        {
            
            // Verifica se tem usuario logado
            CONTA_RECEBER cp = SessionMocks.contaReceber;
            CONTA_RECEBER_RATEIO rl = ratApp.GetItemById(id);
            Int32 volta = ratApp.ValidateDelete(rl);
            SessionMocks.idVoltaTab = 2;
            return RedirectToAction("VoltarAnexoCR");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ContaReceberLista" + "_" + data + ".pdf";
            List<CONTA_RECEBER> lista = SessionMocks.listaCR;
            CONTA_RECEBER filtro = SessionMocks.filtroCR;
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

            cell = new PdfPCell(new Paragraph("Contas a Receber - Listagem", meuFont2))
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
            table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Receber selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
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
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Saldo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_RECEBER item in lista)
            {
                if (item.CLIENTE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
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
                if (item.CENTRO_CUSTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CENTRO_CUSTO.CECU_NM_NOME, meuFont))
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
                if (item.CARE_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CARE_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARE_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CARE_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CARE_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CARE_VL_SALDO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
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
                if (filtro.CLIE_CD_ID != null)
                {
                    parametros += "Cliente: " + cliApp.GetItemById(filtro.CLIE_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CARE_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data da Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data da Emissão: " + filtro.CARE_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CARE_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento Inicial: " + filtro.CARE_DT_VENCIMENTO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento Inicial: " + filtro.CARE_DT_VENCIMENTO;
                    }
                }
                if (Session["vencFinal"] != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento Final: " + (String)Session["vencFinal"];
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento Final: " + (String)Session["vencFinal"];
                    }
                }
                if (filtro.CARE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição (Histórico): " + filtro.CARE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição (Histórico): " + filtro.CARE_DS_DESCRICAO;
                    }
                }
                if (filtro.CARE_IN_ABERTOS != null)
                {
                    String af = filtro.CARE_IN_ABERTOS == 1 ? "Abertos" : "Fechados";
                    if (ja == 0)
                    {
                        parametros += "Abertos/Fechados: " + af;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Abertos/Fechados: " + af;
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

            return RedirectToAction("MontarTelaCR");
        }
    }
}