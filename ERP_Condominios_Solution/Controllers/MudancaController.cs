using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ERP_Condominios_Solution.Controllers
{
    public class MudancaController : Controller
    {
        private readonly IMudancaAppService baseApp;
        private readonly INotificacaoAppService notiApp;
        private readonly ILogAppService logApp;
        private readonly IUnidadeAppService uniApp;

        private String msg;
        private Exception exception;
        SOLICITACAO_MUDANCA objeto = new SOLICITACAO_MUDANCA();
        SOLICITACAO_MUDANCA objetoAntes = new SOLICITACAO_MUDANCA();
        List<SOLICITACAO_MUDANCA> listaMaster = new List<SOLICITACAO_MUDANCA>();
        String extensao;

        public MudancaController(IMudancaAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps, IUnidadeAppService uniApps)
        {
            baseApp = baseApps; ;
            logApp = logApps;
            notiApp = notiApps;
            uniApp = uniApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");

        }

        [HttpGet]
        public ActionResult MontarTelaMudanca()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<SOLICITACAO_MUDANCA>)Session["ListaMudanca"] == null)
            {
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    listaMaster = baseApp.GetByUnidade(usuario.UNID_CD_ID.Value);
                }
                else if (usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMaster = baseApp.GetAllItens(idAss);
                }
                else if (usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    listaMaster = baseApp.GetAllItens(idAss).Where(p => p.SOMU_IN_STATUS == 2).ToList();
                }
                Session["ListaMudanca"] = listaMaster;
                Session["FiltroMudanca"] = null;
            }

            ViewBag.Listas = (List<SOLICITACAO_MUDANCA>)Session["ListaMudanca"];
            ViewBag.Title = "Mudancas";
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
            status.Add(new SelectListItem() { Text = "Não Aprovada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Executada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> ent = new List<SelectListItem>();
            ent.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            ent.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.Entrada = new SelectList(ent, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Mudancas = ((List<SOLICITACAO_MUDANCA>)Session["ListaMudanca"]).Count;

            // Mensagem
            if ((Int32)Session["MensMudanca"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensMudanca"] == 3)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0060", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensMudanca"] == 4)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensMudanca"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0062", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensMudanca"] = 0;
            Session["VoltaMudanca"] = 1;
            objeto = new SOLICITACAO_MUDANCA();
            objeto.SOMU_DT_MUDANCA = DateTime.Today.Date;
            return View(objeto);
        }

        public ActionResult RetirarFiltroMudanca()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMudanca"] = null;
            Session["FiltroMudanca"] = null;
            listaMaster = new List<SOLICITACAO_MUDANCA>();
            return RedirectToAction("MontarTelaMudanca");
        }

        public ActionResult MostrarTudoMudanca()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaMudanca"] = listaMaster;
            return RedirectToAction("MontarTelaMudanca");
        }

        [HttpPost]
        public ActionResult FiltrarMudanca(SOLICITACAO_MUDANCA item)
        {
            try
            {
                try
                {
                    // Executa a operação
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Login", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    List<SOLICITACAO_MUDANCA> listaObj = new List<SOLICITACAO_MUDANCA>();
                    Int32 volta = baseApp.ExecuteFilter(item.SOMU_DT_MUDANCA, item.SOMU_IN_ENTRADA_SAIDA, item.SOMU_IN_STATUS, item.UNID_CD_ID, idAss, out listaObj);
                    Session["FiltroMudanca"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMudanca"] = 1;
                    }

                    // Sucesso
                    Session["MensMudanca"] = 0;
                    listaMaster = listaObj;
                    Session["ListaMudanca"] = listaObj;
                    return RedirectToAction("MontarTelaMudanca");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaMudanca");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMudanca");
            }
        }

        public ActionResult VoltarBaseMudanca()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMudanca"] = null;
            return RedirectToAction("MontarTelaMudanca");
        }

        [HttpGet]
        public ActionResult IncluirMudanca()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            List<SelectListItem> ent = new List<SelectListItem>();
            ent.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            ent.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.Entrada = new SelectList(ent, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            SOLICITACAO_MUDANCA item = new SOLICITACAO_MUDANCA();
            MudancaViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA, MudancaViewModel>(item);
            vm.SOMU_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.SOMU_DT_CADASTRO = DateTime.Today.Date;
            vm.SOMU_DT_CRIACAO = DateTime.Today.Date;
            vm.SOMU_IN_STATUS = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.SOMU_DT_MUDANCA = DateTime.Today.Date;
            if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                vm.UNID_CD_ID = usuario.UNID_CD_ID.Value;
                ViewBag.Unidade = usuario.UNIDADE.UNID_NM_EXIBE;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirMudanca(MudancaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            List<SelectListItem> ent = new List<SelectListItem>();
            ent.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            ent.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.Entrada = new SelectList(ent, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SOLICITACAO_MUDANCA item = Mapper.Map<MudancaViewModel, SOLICITACAO_MUDANCA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMudanca"] = 3;
                        return RedirectToAction("MontarTelaMudanca", "Mudanca");
                    }
                    if (volta == 2)
                    {
                        Session["MensMudanca"] = 5;
                        return RedirectToAction("MontarTelaMudanca", "Mudanca");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Mudanca/" + item.SOMU_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.SOMU_CD_ID;
                    Session["IdMudanca"] = item.SOMU_CD_ID;
                    if (Session["FileQueueMudanca"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueMudanca"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueMudanca(file);
                            }
                        }
                        Session["FileQueueMudanca"] = null;
                    }

                    vm.SOMU_CD_ID = item.SOMU_CD_ID;
                    Session["IdMudanca"] = item.SOMU_CD_ID;

                    // Sucesso
                    listaMaster = new List<SOLICITACAO_MUDANCA>();
                    Session["ListaMudanca"] = null;
                    Session["VoltaMudanca"] = 1;
                    Session["IdMudancaVolta"] = item.SOMU_CD_ID;
                    Session["Mudanca"] = item;
                    Session["MensMudanca"] = 0;
                    return RedirectToAction("MontarTelaMudanca");
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
        public ActionResult EditarMudanca(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            List<SelectListItem> ent = new List<SelectListItem>();
            ent.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            ent.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.Entrada = new SelectList(ent, "Value", "Text");

            // Prepara status
            SOLICITACAO_MUDANCA item = baseApp.GetItemById(id);
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;
            List<SelectListItem> status = new List<SelectListItem>();
            if (perfil == "ADM" || perfil == "SIN")
            {
                if (item.SOMU_IN_STATUS == 1)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Não Aprovada", Value = "3" });
                }
            }
            else if (perfil == "MOR")
            {
                if (item.SOMU_IN_STATUS == 1 || item.SOMU_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Cancelada", Value = "5" });
                }
            }
            else if (perfil == "POR")
            {
                if (item.SOMU_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Executada", Value = "4" });
                }
            }
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (item.SOMU_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.SOMU_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Executada/Encerrada";
            }
            else if (item.SOMU_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }

            // Monta view
            objetoAntes = item;
            Session["Mudanca"] = item;
            Session["IdVolta"] = id;
            Session["IdMudanca"] = id;
            if (item.SOMU_IN_ENTRADA_SAIDA == 1)
            {
                ViewBag.ES = "Entrada";
            }
            else
            {
                ViewBag.ES = "Saída";
            }
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            MudancaViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA, MudancaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMudanca(MudancaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            String perfil = usuarioLogado.PERFIL.PERF_SG_SIGLA;
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            List<SelectListItem> ent = new List<SelectListItem>();
            ent.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            ent.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.Entrada = new SelectList(ent, "Value", "Text");
            List<SelectListItem> status = new List<SelectListItem>();
            if (perfil == "ADM" || perfil == "SIN")
            {
                if (vm.SOMU_IN_STATUS == 1)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Não Aprovada", Value = "3" });
                }
            }
            else if (perfil == "MOR")
            {
                if (vm.SOMU_IN_STATUS == 1 || vm.SOMU_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Cancelada", Value = "5" });
                }
            }
            else if (perfil == "POR")
            {
                if (vm.SOMU_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Executada", Value = "4" });
                }
            }
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SOLICITACAO_MUDANCA item = Mapper.Map<MudancaViewModel, SOLICITACAO_MUDANCA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    //if (item.SOMU_IN_STATUS == 2)
                    //{
                    //    NOTIFICACAO not = new NOTIFICACAO();
                    //    not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    //    not.ASSI_CD_ID = idAss;
                    //    not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    //    not.NOTI_IN_ATIVO = 1;
                    //    not.NOTI_IN_NIVEL = 1;
                    //    not.NOTI_IN_ORIGEM = 1;
                    //    not.NOTI_IN_STATUS = 1;
                    //    not.NOTI_IN_VISTA = 0;
                    //    not.NOTI_NM_TITULO = "Notificação para Morador - Aprovação de Mudança";
                    //    not.USUA_CD_ID = item.USUA_CD_ID;
                    //    not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi aprovada em " + item.SOMU_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                    //    Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);

                    //    List<USUARIO> port = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 4).ToList();
                    //    foreach (USUARIO usu in port)
                    //    {
                    //        not = new NOTIFICACAO();
                    //        not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    //        not.ASSI_CD_ID = idAss;
                    //        not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    //        not.NOTI_IN_ATIVO = 1;
                    //        not.NOTI_IN_NIVEL = 1;
                    //        not.NOTI_IN_ORIGEM = 1;
                    //        not.NOTI_IN_STATUS = 1;
                    //        not.NOTI_IN_VISTA = 0;
                    //        not.NOTI_NM_TITULO = "Notificação para Portaria - Aprovação de Mudança";
                    //        not.USUA_CD_ID = usu.USUA_CD_ID;
                    //        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi aprovada em " + item.SOMU_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                    //        Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                    //    }

                    //}
                    //else if (item.SOMU_IN_STATUS == 3)
                    //{
                    //    NOTIFICACAO not = new NOTIFICACAO();
                    //    not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    //    not.ASSI_CD_ID = idAss;
                    //    not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    //    not.NOTI_IN_ATIVO = 1;
                    //    not.NOTI_IN_NIVEL = 1;
                    //    not.NOTI_IN_ORIGEM = 1;
                    //    not.NOTI_IN_STATUS = 1;
                    //    not.NOTI_IN_VISTA = 0;
                    //    not.NOTI_NM_TITULO = "Notificação para Morador - Reprovação de Mudança";
                    //    not.USUA_CD_ID = item.USUA_CD_ID;
                    //    not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " NÃO foi aprovada em " + item.SOMU_DT_VETADA.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                    //    Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);
                    //}
                    //else if (item.SOMU_IN_STATUS == 4)
                    //{
                    //    USUARIO sind = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 2).FirstOrDefault();
                    //    NOTIFICACAO not = new NOTIFICACAO();
                    //    not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    //    not.ASSI_CD_ID = idAss;
                    //    not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    //    not.NOTI_IN_ATIVO = 1;
                    //    not.NOTI_IN_NIVEL = 1;
                    //    not.NOTI_IN_ORIGEM = 1;
                    //    not.NOTI_IN_STATUS = 1;
                    //    not.NOTI_IN_VISTA = 0;
                    //    not.NOTI_NM_TITULO = "Notificação para Síndico - Execução/Encerramento de Mudança";
                    //    not.USUA_CD_ID = sind.USUA_CD_ID;
                    //    not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi executada/encerrada em " + item.SOMU_DT_EXECUCAO_INICIO.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                    //    Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                    //}
                    //else if (item.SOMU_IN_STATUS == 5)
                    //{
                    //    USUARIO sind = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 2).FirstOrDefault();
                    //    NOTIFICACAO not = new NOTIFICACAO();
                    //    not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    //    not.ASSI_CD_ID = idAss;
                    //    not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    //    not.NOTI_IN_ATIVO = 1;
                    //    not.NOTI_IN_NIVEL = 1;
                    //    not.NOTI_IN_ORIGEM = 1;
                    //    not.NOTI_IN_STATUS = 1;
                    //    not.NOTI_IN_VISTA = 0;
                    //    not.NOTI_NM_TITULO = "Notificação para Síndico - Cancelamento de Mudança";
                    //    not.USUA_CD_ID = sind.USUA_CD_ID;
                    //    not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.SOMU_DT_SUSPENSA.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                    //    Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);

                    //    List<USUARIO> port = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 4).ToList();
                    //    foreach (USUARIO usu in port)
                    //    {
                    //        not = new NOTIFICACAO();
                    //        not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    //        not.ASSI_CD_ID = idAss;
                    //        not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    //        not.NOTI_IN_ATIVO = 1;
                    //        not.NOTI_IN_NIVEL = 1;
                    //        not.NOTI_IN_ORIGEM = 1;
                    //        not.NOTI_IN_STATUS = 1;
                    //        not.NOTI_IN_VISTA = 0;
                    //        not.NOTI_NM_TITULO = "Notificação para Portaria - Cancelamento de Mudança";
                    //        not.USUA_CD_ID = usu.USUA_CD_ID;
                    //        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.SOMU_DT_SUSPENSA.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                    //        Int32 volta3 = notiApp.ValidateCreate(not, usuarioLogado);
                    //    }
                    //}

                    // Sucesso
                    listaMaster = new List<SOLICITACAO_MUDANCA>();
                    Session["ListaMudanca"] = null;
                    Session["MensMudanca"] = 0;
                    return RedirectToAction("MontarTelaMudanca");
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
        public ActionResult VerMudanca(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Unidade = usuario.UNID_CD_ID;

            // Prepara view
            SOLICITACAO_MUDANCA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Mudanca"] = item;
            Session["IdVolta"] = id;
            Session["IdMudanca"] = id;
            MudancaViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA, MudancaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult AprovarMudanca()
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN"  || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            SOLICITACAO_MUDANCA item = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.SOMU_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.SOMU_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Executada/Encerrada";
            }
            else if (item.SOMU_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }

            // Monta view
            objetoAntes = item;
            Session["Mudanca"] = item;
            if (item.SOMU_IN_ENTRADA_SAIDA == 1)
            {
                ViewBag.ES = "Entrada";
            }
            else
            {
                ViewBag.ES = "Saída";
            }
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Aprovada";
            MudancaViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA, MudancaViewModel>(item);
            vm.SOMU_DT_APROVACAO = DateTime.Now;
            //vm.SOMU_IN_STATUS = 2;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AprovarMudanca(MudancaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SOLICITACAO_MUDANCA item = Mapper.Map<MudancaViewModel, SOLICITACAO_MUDANCA>(vm);
                    item.SOMU_IN_STATUS = 2;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    UNIDADE unid = uniApp.GetItemById(item.UNID_CD_ID);
                    if (item.SOMU_IN_STATUS == 2)
                    {
                        NOTIFICACAO not = new NOTIFICACAO();
                        not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                        not.ASSI_CD_ID = idAss;
                        not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                        not.NOTI_IN_ATIVO = 1;
                        not.NOTI_IN_NIVEL = 1;
                        not.NOTI_IN_ORIGEM = 1;
                        not.NOTI_IN_STATUS = 1;
                        not.NOTI_IN_VISTA = 0;
                        not.CANO_CD_ID = 1;
                        not.NOTI_NM_TITULO = "Notificação para Morador - Aprovação de Mudança";
                        not.USUA_CD_ID = item.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + unid.UNID_NM_EXIBE + " foi aprovada em " + item.SOMU_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                        Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);

                        List<USUARIO> port = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 4).ToList();
                        foreach (USUARIO usu in port)
                        {
                            not = new NOTIFICACAO();
                            not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                            not.ASSI_CD_ID = idAss;
                            not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                            not.NOTI_IN_ATIVO = 1;
                            not.NOTI_IN_NIVEL = 1;
                            not.NOTI_IN_ORIGEM = 1;
                            not.NOTI_IN_STATUS = 1;
                            not.NOTI_IN_VISTA = 0;
                            not.CANO_CD_ID = 1;
                            not.NOTI_NM_TITULO = "Notificação para Portaria - Aprovação de Mudança";
                            not.USUA_CD_ID = usu.USUA_CD_ID;
                            not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + unid.UNID_NM_EXIBE + " foi aprovada em " + item.SOMU_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                            Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                        }

                    }

                    // Sucesso
                    Session["MensMudanca"] = 0;
                    return RedirectToAction("VoltarAnexoMudanca");
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
        public ActionResult ReprovarMudanca()
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN"  || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            SOLICITACAO_MUDANCA item = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.SOMU_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.SOMU_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Executada/Encerrada";
            }
            else if (item.SOMU_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }

            // Monta view
            objetoAntes = item;
            Session["Mudanca"] = item;
            if (item.SOMU_IN_ENTRADA_SAIDA == 1)
            {
                ViewBag.ES = "Entrada";
            }
            else
            {
                ViewBag.ES = "Saída";
            }
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Não Aprovada";
            MudancaViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA, MudancaViewModel>(item);
            vm.SOMU_DT_VETADA = DateTime.Now;
            //vm.SOMU_IN_STATUS = 3;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReprovarMudanca(MudancaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SOLICITACAO_MUDANCA item = Mapper.Map<MudancaViewModel, SOLICITACAO_MUDANCA>(vm);
                    item.SOMU_IN_STATUS = 3;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    UNIDADE unid = uniApp.GetItemById(item.UNID_CD_ID);
                    if (item.SOMU_IN_STATUS == 3)
                    {
                        NOTIFICACAO not = new NOTIFICACAO();
                        not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                        not.ASSI_CD_ID = idAss;
                        not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                        not.NOTI_IN_ATIVO = 1;
                        not.NOTI_IN_NIVEL = 1;
                        not.NOTI_IN_ORIGEM = 1;
                        not.NOTI_IN_STATUS = 1;
                        not.NOTI_IN_VISTA = 0;
                        not.CANO_CD_ID = 1;
                        not.NOTI_NM_TITULO = "Notificação para Morador - Reprovação de Mudança";
                        not.USUA_CD_ID = item.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + unid.UNID_NM_EXIBE + " NÃO foi aprovada em " + item.SOMU_DT_VETADA.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                        Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);
                    }

                    // Sucesso
                    Session["MensMudanca"] = 0;
                    return RedirectToAction("VoltarAnexoMudanca");
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
        public ActionResult CancelarMudanca()
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            SOLICITACAO_MUDANCA item = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.SOMU_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.SOMU_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Executada/Encerrada";
            }
            else if (item.SOMU_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }

            // Monta view
            objetoAntes = item;
            Session["Mudanca"] = item;
            if (item.SOMU_IN_ENTRADA_SAIDA == 1)
            {
                ViewBag.ES = "Entrada";
            }
            else
            {
                ViewBag.ES = "Saída";
            }
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Cancelada";
            MudancaViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA, MudancaViewModel>(item);
            vm.SOMU_DT_SUSPENSA = DateTime.Now;
            //vm.SOMU_IN_STATUS = 5;
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelarMudanca(MudancaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SOLICITACAO_MUDANCA item = Mapper.Map<MudancaViewModel, SOLICITACAO_MUDANCA>(vm);
                    item.SOMU_IN_STATUS = 5;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (item.SOMU_IN_STATUS == 5)
                    {
                        USUARIO sind = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 2).FirstOrDefault();
                        UNIDADE unid = uniApp.GetItemById(item.UNID_CD_ID);
                        NOTIFICACAO not = new NOTIFICACAO();
                        not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                        not.ASSI_CD_ID = idAss;
                        not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                        not.NOTI_IN_ATIVO = 1;
                        not.NOTI_IN_NIVEL = 1;
                        not.NOTI_IN_ORIGEM = 1;
                        not.NOTI_IN_STATUS = 1;
                        not.NOTI_IN_VISTA = 0;
                        not.CANO_CD_ID = 1;
                        not.NOTI_NM_TITULO = "Notificação para Síndico - Cancelamento de Mudança";
                        not.USUA_CD_ID = sind.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + unid.UNID_NM_EXIBE + " foi cancelada em " + item.SOMU_DT_SUSPENSA.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                        Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);

                        List<USUARIO> port = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 4).ToList();
                        foreach (USUARIO usu in port)
                        {
                            not = new NOTIFICACAO();
                            not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                            not.ASSI_CD_ID = idAss;
                            not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                            not.NOTI_IN_ATIVO = 1;
                            not.NOTI_IN_NIVEL = 1;
                            not.NOTI_IN_ORIGEM = 1;
                            not.NOTI_IN_STATUS = 1;
                            not.NOTI_IN_VISTA = 0;
                            not.CANO_CD_ID = 1;
                            not.NOTI_NM_TITULO = "Notificação para Portaria - Cancelamento de Mudança";
                            not.USUA_CD_ID = usu.USUA_CD_ID;
                            not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + unid.UNID_NM_EXIBE + " foi cancelada em " + item.SOMU_DT_SUSPENSA.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                            Int32 volta3 = notiApp.ValidateCreate(not, usuarioLogado);
                        }
                    }

                    // Sucesso
                    Session["MensMudanca"] = 0;
                    return RedirectToAction("VoltarAnexoMudanca");
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
        public ActionResult ExecutarMudanca()
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            SOLICITACAO_MUDANCA item = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.SOMU_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.SOMU_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.SOMU_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Executada/Encerrada";
            }
            else if (item.SOMU_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }

            // Monta view
            objetoAntes = item;
            Session["Mudanca"] = item;
            if (item.SOMU_IN_ENTRADA_SAIDA == 1)
            {
                ViewBag.ES = "Entrada";
            }
            else
            {
                ViewBag.ES = "Saída";
            }
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Executada";
            MudancaViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA, MudancaViewModel>(item);
            vm.SOMU_DT_EXECUCAO_FINAL = DateTime.Now;
            vm.SOMU_DT_EXECUCAO_INICIO = DateTime.Now;
            //vm.SOMU_IN_STATUS = 4;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExecutarMudanca(MudancaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SOLICITACAO_MUDANCA item = Mapper.Map<MudancaViewModel, SOLICITACAO_MUDANCA>(vm);
                    item.SOMU_IN_STATUS = 4;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    UNIDADE unid = uniApp.GetItemById(item.UNID_CD_ID);
                    if (item.SOMU_IN_STATUS == 4)
                    {
                        USUARIO sind = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 2).FirstOrDefault();
                        NOTIFICACAO not = new NOTIFICACAO();
                        not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                        not.ASSI_CD_ID = idAss;
                        not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                        not.NOTI_IN_ATIVO = 1;
                        not.NOTI_IN_NIVEL = 1;
                        not.NOTI_IN_ORIGEM = 1;
                        not.NOTI_IN_STATUS = 1;
                        not.NOTI_IN_VISTA = 0;
                        not.CANO_CD_ID = 1;
                        not.NOTI_NM_TITULO = "Notificação para Síndico - Execução/Encerramento de Mudança";
                        not.USUA_CD_ID = sind.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + unid.UNID_NM_EXIBE + " foi executada/encerrada em " + item.SOMU_DT_EXECUCAO_INICIO.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                        Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                    }

                    // Sucesso
                    Session["MensMudanca"] = 0;
                    return RedirectToAction("VoltarAnexoMudanca");
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
        public ActionResult ExcluirMudanca(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SOLICITACAO_MUDANCA item = baseApp.GetItemById(id);
            objetoAntes = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            item.SOMU_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMudanca"] = 4;
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
                return RedirectToAction("MontarTelaMudanca");
            }
            listaMaster = new List<SOLICITACAO_MUDANCA>();
            Session["ListaMudanca"] = null;
            Session["FiltroMudanca"] = null;
            return RedirectToAction("MontarTelaMudanca");
        }

        [HttpGet]
        public ActionResult ReativarMudanca(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SOLICITACAO_MUDANCA item = baseApp.GetItemById(id);
            objetoAntes = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            item.SOMU_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<SOLICITACAO_MUDANCA>();
            Session["ListaMudanca"] = null;
            Session["FiltroMudanca"] = null;
            return RedirectToAction("MontarTelaMudanca");
        }

        public ActionResult VoltarAnexoMudanca()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idVeic = (Int32)Session["IdMudanca"];
            return RedirectToAction("EditarMudanca", new { id = idVeic });
        }

        [HttpGet]
        public ActionResult GerarNotificacaoMudanca()
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensMudanca"] = 2;
                    return RedirectToAction("MontarTelaMudanca", "Mudanca");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            SOLICITACAO_MUDANCA mudanca = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss).Where(p => p.UNID_CD_ID == mudanca.UNID_CD_ID).ToList();

            // Prepara view
            ViewBag.Cats = new SelectList(baseApp.GetAllCatNotificacao(idAss), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usuarios = new SelectList(lista, "USUA_CD_ID", "USUA_NM_NOME");

            NOTIFICACAO item = new NOTIFICACAO();
            NotificacaoViewModel vm = Mapper.Map<NOTIFICACAO, NotificacaoViewModel>(item);
            vm.NOTI_DT_EMISSAO = DateTime.Today.Date;
            vm.ASSI_CD_ID = idAss;
            vm.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTI_IN_ATIVO = 1;
            vm.NOTI_IN_NIVEL = 1;
            vm.NOTI_IN_ORIGEM = 1;
            vm.NOTI_IN_STATUS = 1;
            vm.NOTI_IN_VISTA = 0;
            vm.NOTI_NM_TITULO = "Notificação para Morador - Mudança";
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoMudanca(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            SOLICITACAO_MUDANCA mudanca = (SOLICITACAO_MUDANCA)Session["Mudanca"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss);
            ViewBag.Cats = new SelectList(baseApp.GetAllCatNotificacao(idAss), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usuarios = new SelectList(lista, "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    Int32 volta = baseApp.GerarNotificacao(item, usuarioLogado, mudanca, "NOTIMUDA");

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<SOLICITACAO_MUDANCA>();
                    return RedirectToAction("VoltarBaseMudanca");
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
        public ActionResult IncluirComentarioMudanca()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdVolta"];
            SOLICITACAO_MUDANCA item = baseApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            SOLICITACAO_MUDANCA_COMENTARIO coment = new SOLICITACAO_MUDANCA_COMENTARIO();
            MudancaComentarioViewModel vm = Mapper.Map<SOLICITACAO_MUDANCA_COMENTARIO, MudancaComentarioViewModel>(coment);
            vm.SMCO_DT_COMENTARIO = DateTime.Now;
            vm.SMCO_IN_ATIVO = 1;
            vm.SOMU_CD_ID = item.SOMU_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentarioMudanca(MudancaComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdMudanca"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SOLICITACAO_MUDANCA_COMENTARIO item = Mapper.Map<MudancaComentarioViewModel, SOLICITACAO_MUDANCA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SOLICITACAO_MUDANCA not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.SOLICITACAO_MUDANCA_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarMudanca", new { id = idNot });
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

        [HttpPost]
        public ActionResult UploadFileMudanca(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdMudanca"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensMudanca"] = 5;
                return RedirectToAction("VoltarAnexoMudanca");
            }

            SOLICITACAO_MUDANCA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensMudanca"] = 6;
                return RedirectToAction("VoltarAnexoMudanca");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Mudanca/" + item.SOMU_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SOLICITACAO_MUDANCA_ANEXO foto = new SOLICITACAO_MUDANCA_ANEXO();
            foto.SOAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SOAN_DT_ANEXO = DateTime.Today;
            foto.SOAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.SOAN_IN_TIPO = tipo;
            foto.SOAN_NM_TITULO = fileName;
            foto.SOMU_CD_ID = item.SOMU_CD_ID;

            item.SOLICITACAO_MUDANCA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoMudanca");
        }

        public FileResult DownloadMudanca(Int32 id)
        {
            SOLICITACAO_MUDANCA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.SOAN_AQ_ARQUIVO;
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
            else if (arquivo.Contains(".jpeg"))
            {
                contentType = "image/jpeg";
            }
            return File(arquivo, contentType, nomeDownload);
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
            Session["FileQueueMudanca"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueMudanca(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdMudanca"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensMudanca"] = 5;
                return RedirectToAction("VoltarAnexoMudanca");
            }

            SOLICITACAO_MUDANCA item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensMudanca"] = 6;
                return RedirectToAction("VoltarAnexoMudanca");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Mudanca/" + item.SOMU_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SOLICITACAO_MUDANCA_ANEXO foto = new SOLICITACAO_MUDANCA_ANEXO();
            foto.SOAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SOAN_DT_ANEXO = DateTime.Today;
            foto.SOAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.SOAN_IN_TIPO = tipo;
            foto.SOAN_NM_TITULO = fileName;
            foto.SOMU_CD_ID = item.SOMU_CD_ID;

            item.SOLICITACAO_MUDANCA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoMudanca");
        }

        [HttpGet]
        public ActionResult VerAnexoMudanca(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            SOLICITACAO_MUDANCA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaUnidade"] == 1)
            {
                return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 2)
            {
                return RedirectToAction("CarregarPortaria", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 3)
            {
                return RedirectToAction("CarregarSindico", "BaseAdmin");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }



    }
}