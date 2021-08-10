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

        private String msg;
        private Exception exception;
        SOLICITACAO_MUDANCA objeto = new SOLICITACAO_MUDANCA();
        SOLICITACAO_MUDANCA objetoAntes = new SOLICITACAO_MUDANCA();
        List<SOLICITACAO_MUDANCA> listaMaster = new List<SOLICITACAO_MUDANCA>();
        String extensao;

        public MudancaController(IMudancaAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps)
        {
            baseApp = baseApps; ;
            logApp = logApps;
            notiApp = notiApps;
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
            if ((Int32)Session["MensMudanca"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
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
                        ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        return RedirectToAction("MontarTelaMudanca");
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
            if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                vm.UNID_CD_ID = usuario.UNID_CD_ID.Value;
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

            SOLICITACAO_MUDANCA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Mudanca"] = item;
            Session["IdVolta"] = id;
            Session["IdMudanca"] = id;
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SOLICITACAO_MUDANCA item = Mapper.Map<MudancaViewModel, SOLICITACAO_MUDANCA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
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
                        not.NOTI_NM_TITULO = "Notificação para Morador - Aprovação de Mudança";
                        not.USUA_CD_ID = item.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi aprovada em " + item.SOMU_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
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
                            not.NOTI_NM_TITULO = "Notificação para Portaria - Aprovação de Mudança";
                            not.USUA_CD_ID = usu.USUA_CD_ID;
                            not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi aprovada em " + item.SOMU_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                            Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                        }

                    }
                    else if (item.SOMU_IN_STATUS == 3)
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
                        not.NOTI_NM_TITULO = "Notificação para Morador - Reprovação de Mudança";
                        not.USUA_CD_ID = item.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " NÃO foi aprovada em " + item.SOMU_DT_VETADA.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                        Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);
                    }
                    else if (item.SOMU_IN_STATUS == 4)
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
                        not.NOTI_NM_TITULO = "Notificação para Síndico - Execução/Encerramento de Mudança";
                        not.USUA_CD_ID = sind.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi executada/encerrada em " + item.SOMU_DT_EXECUCAO_INICIO.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                        Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                    }
                    else if (item.SOMU_IN_STATUS == 5)
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
                        not.NOTI_NM_TITULO = "Notificação para Síndico - Cancelamento de Mudança";
                        not.USUA_CD_ID = sind.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.SOMU_DT_SUSPENSA.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
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
                            not.NOTI_NM_TITULO = "Notificação para Portaria - Cancelamento de Mudança";
                            not.USUA_CD_ID = usu.USUA_CD_ID;
                            not.NOTI_TX_TEXTO = "A solicitação de mudança " + item.SOMU_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.SOMU_DT_SUSPENSA.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                            Int32 volta3 = notiApp.ValidateCreate(not, usuarioLogado);
                        }
                    }

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





    }
}