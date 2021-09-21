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
    public class ReservaController : Controller
    {
        private readonly IReservaAppService baseApp;
        private readonly INotificacaoAppService notiApp;
        private readonly ILogAppService logApp;

        private String msg;
        private Exception exception;
        RESERVA objeto = new RESERVA();
        RESERVA objetoAntes = new RESERVA();
        List<RESERVA> listaMaster = new List<RESERVA>();
        String extensao;

        public ReservaController(IReservaAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps)
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
        public ActionResult MontarTelaReserva()
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<RESERVA>)Session["ListaReserva"] == null)
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
                    listaMaster = baseApp.GetAllItens(idAss).Where(p => p.RESE_IN_STATUS == 4).ToList();
                }
                Session["ListaReserva"] = listaMaster;
                Session["FiltroReserva"] = null;
            }

            ViewBag.Listas = (List<RESERVA>)Session["ListaReserva"];
            ViewBag.Title = "Reservas";

            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Finalidades = new SelectList(baseApp.GetAllFinalidades(idAss), "FIRE_CD_ID", "FIRE_NM_NOME");
            ViewBag.Ambientes = new SelectList(baseApp.GetAllAmbientes(idAss), "AMBI_CD_ID", "AMBI_NM_AMBIENTE");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
            status.Add(new SelectListItem() { Text = "Não Aprovada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Confirmada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Reservas = ((List<RESERVA>)Session["ListaReserva"]).Count;

            // Mensagem
            if ((Int32)Session["MensReserva"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensReserva"] == 3)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0078", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensReserva"] == 4)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0079", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensReserva"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0080", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensReserva"] = 0;
            Session["VoltaReserva"] = 1;
            objeto = new RESERVA();
            return View(objeto);
        }

        public ActionResult RetirarFiltroReserva()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaReserva"] = null;
            Session["FiltroReserva"] = null;
            listaMaster = new List<RESERVA>();
            return RedirectToAction("MontarTelaReserva");
        }

        public ActionResult MostrarTudoReserva()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaReserva"] = listaMaster;
            return RedirectToAction("MontarTelaReserva");
        }

        [HttpPost]
        public ActionResult FiltrarReserva(RESERVA item)
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

                    List<RESERVA> listaObj = new List<RESERVA>();
                    Int32 volta = baseApp.ExecuteFilter(item.RESE_NM_NOME, item.RESE_DT_EVENTO, item.FIRE_CD_ID, item.AMBI_CD_ID, item.UNID_CD_ID, item.RESE_IN_STATUS, idAss, out listaObj);
                    Session["FiltroReserva"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensReserva"] = 1;
                    }

                    // Sucesso
                    Session["MensReserva"] = 0;
                    listaMaster = listaObj;
                    Session["ListaReserva"] = listaObj;
                    return RedirectToAction("MontarTelaReserva");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaReserva");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaReserva");
            }
        }

        public ActionResult VoltarBaseReserva()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaReserva"] = null;
            return RedirectToAction("MontarTelaReserva");
        }

        [HttpGet]
        public ActionResult IncluirReserva()
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Finalidades = new SelectList(baseApp.GetAllFinalidades(idAss), "FIRE_CD_ID", "FIRE_NM_NOME");
            ViewBag.Ambientes = new SelectList(baseApp.GetAllAmbientes(idAss), "AMBI_CD_ID", "AMBI_NM_AMBIENTE");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            RESERVA item = new RESERVA();
            ReservaViewModel vm = Mapper.Map<RESERVA, ReservaViewModel>(item);
            vm.RESE_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.RESE_DT_CADASTRO = DateTime.Today.Date;
            vm.RESE_DT_EVENTO = DateTime.Today.Date;
            vm.RESE_IN_STATUS = 1;
            vm.RESE_IN_BOLETO = 0;
            vm.RESE_IN_CONFIRMADA = 0;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                vm.UNID_CD_ID = usuario.UNID_CD_ID.Value;
                ViewBag.Unidade = usuario.UNIDADE.UNID_NM_EXIBE;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirReserva(ReservaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Finalidades = new SelectList(baseApp.GetAllFinalidades(idAss), "FIRE_CD_ID", "FIRE_NM_NOME");
            ViewBag.Ambientes = new SelectList(baseApp.GetAllAmbientes(idAss), "AMBI_CD_ID", "AMBI_NM_AMBIENTE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    RESERVA item = Mapper.Map<ReservaViewModel, RESERVA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensReserva"] = 3;
                        return RedirectToAction("MontarTelaReserva", "Reserva");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Reserva/" + item.RESE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.RESE_CD_ID;
                    if (Session["FileQueueReserva"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueReserva"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueReserva(file);
                            }
                        }
                        Session["FileQueueReserva"] = null;
                    }

                    vm.RESE_CD_ID = item.RESE_CD_ID;
                    Session["IdReserva"] = item.RESE_CD_ID;

                    // Sucesso
                    listaMaster = new List<RESERVA>();
                    Session["ListaReserva"] = null;
                    Session["VoltaReserva"] = 1;
                    Session["IdReservaVolta"] = item.RESE_CD_ID;
                    Session["Reserva"] = item;
                    Session["MensReserva"] = 0;
                    return RedirectToAction("MontarTelaReserva");
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
        public ActionResult EditarReserva(Int32 id)
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Finalidades = new SelectList(baseApp.GetAllFinalidades(idAss), "FIRE_CD_ID", "FIRE_NM_NOME");

            // Prepara status
            RESERVA item = baseApp.GetItemById(id);
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;
            List<SelectListItem> status = new List<SelectListItem>();
            if (perfil == "ADM" || perfil == "SIN")
            {
                if (item.RESE_IN_STATUS == 1)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Não Aprovada", Value = "3" });
                    status.Add(new SelectListItem() { Text = "Confirmada", Value = "4" });
                }
            }
            else if (perfil == "MOR")
            {
                if (item.RESE_IN_STATUS == 1 || item.RESE_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Cancelada", Value = "5" });
                }
            }
            else if (perfil == "POR")
            {
                if (item.RESE_IN_STATUS == 4)
                {
                    status.Add(new SelectListItem() { Text = "Confirmada", Value = "4" });
                    status.Add(new SelectListItem() { Text = "Encerrada", Value = "6" });
                }
            }
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (item.RESE_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.RESE_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.RESE_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.RESE_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Confirmada";
            }
            else if (item.RESE_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }
            else if (item.RESE_IN_STATUS == 6)
            {
                ViewBag.NomeStatus = "Encerrada";
            }

            // Monta view
            objetoAntes = item;
            Session["Reserva"] = item;
            Session["IdVolta"] = id;
            Session["IdReserva"] = id;
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ReservaViewModel vm = Mapper.Map<RESERVA, ReservaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarReserva(ReservaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            String perfil = usuarioLogado.PERFIL.PERF_SG_SIGLA;
            ViewBag.Finalidades = new SelectList(baseApp.GetAllFinalidades(idAss), "FIRE_CD_ID", "FIRE_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            if (perfil == "ADM" || perfil == "SIN")
            {
                if (vm.RESE_IN_STATUS == 1)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Não Aprovada", Value = "3" });
                    status.Add(new SelectListItem() { Text = "Confirmada", Value = "4" });
                }
            }
            else if (perfil == "MOR")
            {
                if (vm.RESE_IN_STATUS == 1 || vm.RESE_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Em Aprovação", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Aprovada", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Cancelada", Value = "5" });
                }
            }
            else if (perfil == "POR")
            {
                if (vm.RESE_IN_STATUS == 4)
                {
                    status.Add(new SelectListItem() { Text = "Confirmada", Value = "4" });
                    status.Add(new SelectListItem() { Text = "Encerrada", Value = "6" });
                }
            }
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    RESERVA item = Mapper.Map<ReservaViewModel, RESERVA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    //if (item.RESE_IN_STATUS == 2)
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
                    //    not.NOTI_NM_TITULO = "Notificação para Morador - Aprovação de Reserva";
                    //    not.USUA_CD_ID = item.USUA_CD_ID.Value;
                    //    not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi aprovada em " + item.RESE_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                    //    Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);
                    //}
                    //else if (item.RESE_IN_STATUS == 3)
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
                    //    not.NOTI_NM_TITULO = "Notificação para Morador - Reprovação de Reserva";
                    //    not.USUA_CD_ID = item.USUA_CD_ID.Value;
                    //    not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " NÃO foi aprovada em " + item.RESE_DT_VETADA.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                    //    Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);
                    //}
                    //else if (item.RESE_IN_STATUS == 4)
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
                    //    not.NOTI_NM_TITULO = "Notificação para Morador - Confirmação de Reserva";
                    //    not.USUA_CD_ID = item.USUA_CD_ID.Value;
                    //    not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " NÃO foi confirmada em " + item.RESE_DT_CONFIRMACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
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
                    //        not.NOTI_NM_TITULO = "Notificação para Portaria - Confirmação de Reserva";
                    //        not.USUA_CD_ID = usu.USUA_CD_ID;
                    //        not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi confirmada em " + item.RESE_DT_CONFIRMACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                    //        Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                    //    }
                    //}
                    //else if (item.RESE_IN_STATUS == 5)
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
                    //    not.NOTI_NM_TITULO = "Notificação para Síndico - Cancelamento de Reserva";
                    //    not.USUA_CD_ID = sind.USUA_CD_ID;
                    //    not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.RESE_DT_CADASTRO.ToShortDateString() + ". Por favor consulte a solicitação.";
                    //    Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);

                    //    RESERVA antes = (RESERVA)Session["Reserva"];
                    //    if (antes.RESE_IN_STATUS == 4)
                    //    {
                    //        List<USUARIO> port = baseApp.GetAllUsuarios(idAss).Where(p => p.PERF_CD_ID == 4).ToList();
                    //        foreach (USUARIO usu in port)
                    //        {
                    //            not = new NOTIFICACAO();
                    //            not.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    //            not.ASSI_CD_ID = idAss;
                    //            not.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    //            not.NOTI_IN_ATIVO = 1;
                    //            not.NOTI_IN_NIVEL = 1;
                    //            not.NOTI_IN_ORIGEM = 1;
                    //            not.NOTI_IN_STATUS = 1;
                    //            not.NOTI_IN_VISTA = 0;
                    //            not.NOTI_NM_TITULO = "Notificação para Portaria - Cancelamento de Reserva";
                    //            not.USUA_CD_ID = usu.USUA_CD_ID;
                    //            not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.RESE_DT_CADASTRO.ToShortDateString() + ". Por favor consulte a solicitação.";
                    //            Int32 volta3 = notiApp.ValidateCreate(not, usuarioLogado);
                    //        }
                    //    }
                    //}

                    // Sucesso
                    listaMaster = new List<RESERVA>();
                    Session["ListaReserva"] = null;
                    Session["MensReserva"] = 0;
                    return RedirectToAction("MontarTelaReserva");
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
        public ActionResult VerReserva(Int32 id)
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
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
            RESERVA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Reserva"] = item;
            Session["IdVolta"] = id;
            Session["IdReserva"] = id;
            ReservaViewModel vm = Mapper.Map<RESERVA, ReservaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult AprovarReserva()
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            RESERVA item = (RESERVA)Session["Reserva"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.RESE_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.RESE_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.RESE_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.RESE_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Confirmada";
            }
            else if (item.RESE_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }
            else if (item.RESE_IN_STATUS == 6)
            {
                ViewBag.NomeStatus = "Encerrada";
            }

            // Monta view
            objetoAntes = item;
            Session["Reserva"] = item;
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Aprovada";
            ReservaViewModel vm = Mapper.Map<RESERVA, ReservaViewModel>(item);
            vm.RESE_DT_APROVACAO = DateTime.Now;
            vm.RESE_IN_STATUS = 2;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AprovarReserva(ReservaViewModel vm)
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
                    RESERVA item = Mapper.Map<ReservaViewModel, RESERVA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (item.RESE_IN_STATUS == 2)
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
                        not.NOTI_NM_TITULO = "Notificação para Morador - Aprovação de Reserva";
                        not.USUA_CD_ID = item.USUA_CD_ID.Value;
                        not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi aprovada em " + item.RESE_DT_APROVACAO.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                        Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);
                    }

                    // Sucesso
                    Session["MensReserva"] = 0;
                    return RedirectToAction("VoltarAnexoReserva");
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
        public ActionResult ReprovarReserva()
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            RESERVA item = (RESERVA)Session["Reserva"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.RESE_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.RESE_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.RESE_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.RESE_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Confirmada";
            }
            else if (item.RESE_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }
            else if (item.RESE_IN_STATUS == 6)
            {
                ViewBag.NomeStatus = "Encerrada";
            }

            // Monta view
            objetoAntes = item;
            Session["Reserva"] = item;
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Não Aprovada";
            ReservaViewModel vm = Mapper.Map<RESERVA, ReservaViewModel>(item);
            vm.RESE_DT_VETADA = DateTime.Now;
            vm.RESE_IN_STATUS = 3;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReprovarReserva(ReservaViewModel vm)
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
                    RESERVA item = Mapper.Map<ReservaViewModel, RESERVA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (item.RESE_IN_STATUS == 3)
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
                        not.NOTI_NM_TITULO = "Notificação para Morador - Reprovação de Reserva";
                        not.USUA_CD_ID = item.USUA_CD_ID.Value;
                        not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " NÃO foi aprovada em " + item.RESE_DT_VETADA.Value.ToShortDateString() + ". Por favor consulte a solicitação e/ou tome as providências necessárias.";
                        Int32 volta1 = notiApp.ValidateCreate(not, usuarioLogado);
                    }

                    // Sucesso
                    Session["MensReserva"] = 0;
                    return RedirectToAction("VoltarAnexoReserva");
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
        public ActionResult CancelarReserva()
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            RESERVA item = (RESERVA)Session["Reserva"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.RESE_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.RESE_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.RESE_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.RESE_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Confirmada";
            }
            else if (item.RESE_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }
            else if (item.RESE_IN_STATUS == 6)
            {
                ViewBag.NomeStatus = "Encerrada";
            }

            // Monta view
            objetoAntes = item;
            Session["Reserva"] = item;
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Cancelada";
            ReservaViewModel vm = Mapper.Map<RESERVA, ReservaViewModel>(item);
            vm.RESE_DT_CADASTRO = DateTime.Now;
            vm.RESE_IN_STATUS = 5;
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelarReserva(ReservaViewModel vm)
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
                    RESERVA item = Mapper.Map<ReservaViewModel, RESERVA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (item.RESE_IN_STATUS == 5)
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
                        not.NOTI_NM_TITULO = "Notificação para Síndico - Cancelamento de Reserva";
                        not.USUA_CD_ID = sind.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.RESE_DT_CADASTRO.ToShortDateString() + ". Por favor consulte a solicitação.";
                        Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);

                        if (objetoAntes.RESE_IN_STATUS == 4)
                        {
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
                                not.NOTI_NM_TITULO = "Notificação para Portaria - Cancelamento de Reserva";
                                not.USUA_CD_ID = usu.USUA_CD_ID;
                                not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi cancelada em " + item.RESE_DT_CADASTRO.ToShortDateString() + ". Por favor consulte a solicitação.";
                                Int32 volta3 = notiApp.ValidateCreate(not, usuarioLogado);
                            }
                        }
                    }

                    // Sucesso
                    Session["MensReserva"] = 0;
                    return RedirectToAction("VoltarAnexoReserva");
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
        public ActionResult ExecutarReserva()
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view

            // Prepara status
            RESERVA item = (RESERVA)Session["Reserva"];
            String perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Perfil = perfil;

            if (item.RESE_IN_STATUS == 1)
            {
                ViewBag.NomeStatus = "Em Aprovação";
            }
            else if (item.RESE_IN_STATUS == 2)
            {
                ViewBag.NomeStatus = "Aprovada";
            }
            else if (item.RESE_IN_STATUS == 3)
            {
                ViewBag.NomeStatus = "Não Aprovada";
            }
            else if (item.RESE_IN_STATUS == 4)
            {
                ViewBag.NomeStatus = "Confirmada";
            }
            else if (item.RESE_IN_STATUS == 5)
            {
                ViewBag.NomeStatus = "Cancelada";
            }
            else if (item.RESE_IN_STATUS == 6)
            {
                ViewBag.NomeStatus = "Encerrada";
            }

            // Monta view
            objetoAntes = item;
            Session["Reserva"] = item;
            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                Session["IdUnidade"] = usuario.UNID_CD_ID;
            }
            else
            {
                Session["IdUnidade"] = null;
            }
            ViewBag.NovoStatus = "Executada";
            ReservaViewModel vm = Mapper.Map<RESERVA, ReservaViewModel>(item);
            vm.RESE_DT_FINAL = DateTime.Now;
            vm.RESE_IN_STATUS = 6;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExecutarReserva(ReservaViewModel vm)
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
                    RESERVA item = Mapper.Map<ReservaViewModel, RESERVA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (item.RESE_IN_STATUS == 6)
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
                        not.NOTI_NM_TITULO = "Notificação para Síndico - Execução/Encerramento de Reserva";
                        not.USUA_CD_ID = sind.USUA_CD_ID;
                        not.NOTI_TX_TEXTO = "A solicitação de reserva " + item.RESE_CD_ID.ToString() + " aberta pela unidade " + item.UNIDADE.UNID_NM_EXIBE + " foi executada/encerrada em " + item.RESE_DT_FINAL.Value.ToShortDateString() + ". Por favor consulte a solicitação.";
                        Int32 volta2 = notiApp.ValidateCreate(not, usuarioLogado);
                    }

                    // Sucesso
                    Session["MensReserva"] = 0;
                    return RedirectToAction("VoltarAnexoReserva");
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
        public ActionResult ExcluirReserva(Int32 id)
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            RESERVA item = baseApp.GetItemById(id);
            objetoAntes = (RESERVA)Session["Reserva"];
            item.RESE_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensReserva"] = 4;
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0079", CultureInfo.CurrentCulture));
                return RedirectToAction("MontarTelaReserva");
            }
            listaMaster = new List<RESERVA>();
            Session["ListaReserva"] = null;
            Session["FiltroReserva"] = null;
            return RedirectToAction("MontarTelaReserva");
        }

        [HttpGet]
        public ActionResult ReativarReserva(Int32 id)
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            RESERVA item = baseApp.GetItemById(id);
            objetoAntes = (RESERVA)Session["Reserva"];
            item.RESE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<RESERVA>();
            Session["ListaReserva"] = null;
            Session["FiltroReserva"] = null;
            return RedirectToAction("MontarTelaReserva");
        }

        public ActionResult VoltarAnexoReserva()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idVeic = (Int32)Session["IdReserva"];
            return RedirectToAction("EditarReserva", new { id = idVeic });
        }

        [HttpGet]
        public ActionResult GerarNotificacaoReserva()
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
                    Session["MensReserva"] = 2;
                    return RedirectToAction("MontarTelaReserva", "Reserva");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            RESERVA mudanca = (RESERVA)Session["Reserva"];
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
            vm.NOTI_NM_TITULO = "Notificação para Morador - Reserva";
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoReserva(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            RESERVA mudanca = (RESERVA)Session["Reserva"];
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
                    listaMaster = new List<RESERVA>();
                    return RedirectToAction("VoltarBaseReserva");
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
        public ActionResult IncluirComentarioReserva()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdVolta"];
            RESERVA item = baseApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            RESERVA_COMENTARIO coment = new RESERVA_COMENTARIO();
            ReservaComentarioViewModel vm = Mapper.Map<RESERVA_COMENTARIO, ReservaComentarioViewModel>(coment);
            vm.RECO_DT_COMENTARIO = DateTime.Now;
            vm.RECO_IN_ATIVO = 1;
            vm.RESE_CD_ID = item.RESE_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentarioReserva(ReservaComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdReserva"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    RESERVA_COMENTARIO item = Mapper.Map<ReservaComentarioViewModel, RESERVA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    RESERVA not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.RESERVA_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarReserva", new { id = idNot });
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
        public ActionResult UploadFileReserva(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdReserva"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensReserva"] = 5;
                return RedirectToAction("VoltarAnexoReserva");
            }

            RESERVA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensReserva"] = 6;
                return RedirectToAction("VoltarAnexoReserva");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Reserva/" + item.RESE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            RESERVA_ANEXO foto = new RESERVA_ANEXO();
            foto.REAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.REAN_DT_ANEXO = DateTime.Today;
            foto.REAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.REAN_IN_TIPO = tipo;
            foto.REAN_NM_TITULO = fileName;
            foto.RESE_CD_ID = item.RESE_CD_ID;

            item.RESERVA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoReserva");
        }

        public FileResult DownloadReserva(Int32 id)
        {
            RESERVA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.REAN_AQ_ARQUIVO;
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
            Session["FileQueueReserva"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueReserva(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdReserva"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensReserva"] = 5;
                return RedirectToAction("VoltarAnexoReserva");
            }

            RESERVA item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensReserva"] = 6;
                return RedirectToAction("VoltarAnexoReserva");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Reserva/" + item.RESE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            RESERVA_ANEXO foto = new RESERVA_ANEXO();
            foto.REAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.REAN_DT_ANEXO = DateTime.Today;
            foto.REAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.REAN_IN_TIPO = tipo;
            foto.REAN_NM_TITULO = fileName;
            foto.RESE_CD_ID = item.RESE_CD_ID;

            item.RESERVA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoReserva");
        }

        [HttpGet]
        public ActionResult VerAnexoReserva(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            RESERVA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }
    }
}