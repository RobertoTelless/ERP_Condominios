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
using Canducci.Zip;

namespace ERP_Condominios_Solution.Controllers
{
    public class AutorizacaoController : Controller
    {
        private readonly IAutorizacaoAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        AUTORIZACAO_ACESSO objetoForn = new AUTORIZACAO_ACESSO();
        AUTORIZACAO_ACESSO objetoFornAntes = new AUTORIZACAO_ACESSO();
        List<AUTORIZACAO_ACESSO> listaMasterForn = new List<AUTORIZACAO_ACESSO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public AutorizacaoController(IAutorizacaoAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            confApp = confApps;
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

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaAutorizacao()
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
                    Session["MensAutorizacao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaAutorizacao"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    listaMasterForn = listaMasterForn.Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
                }
                Session["ListaAutorizacao"] = listaMasterForn;
            }

            ViewBag.Listas = (List<AUTORIZACAO_ACESSO>)Session["ListaAutorizacao"];
            ViewBag.Title = "Autorizacao";
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            Session["IncluirAut"] = 0;

            // Indicadores
            ViewBag.Autorizacoes = ((List<AUTORIZACAO_ACESSO>)Session["ListaAutorizacao"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Autorização", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Restrição", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

            if (Session["MensAutorizacao"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAutorizacao"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAutorizacao"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAutorizacao"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAutorizacao"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new AUTORIZACAO_ACESSO();
            objetoForn.AUAC_IN_ATIVO = 1;
            Session["MensAutorizacao"] = 0;
            Session["VoltaAutorizacao"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroAutorizacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaAutorizacao"] = null;
            Session["FiltroAutorizacao"] = null;
            return RedirectToAction("MontarTelaAutorizacao");
        }

        public ActionResult MostrarTudoAutorizacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                listaMasterForn = listaMasterForn.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList(); 
            }
            Session["FiltroAutorizacao"] = null;
            Session["ListaAutorizacao"] = listaMasterForn;
            return RedirectToAction("MontarTelaAutorizacao");
        }

        [HttpPost]
        public ActionResult FiltrarAutorizacao(AUTORIZACAO_ACESSO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<AUTORIZACAO_ACESSO> listaObj = new List<AUTORIZACAO_ACESSO>();
                Session["FiltroAutorizacao"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.UNID_CD_ID, item.AUAC_NM_VISITANTE, item.AUAC_NR_DOCUMENTO, item.AUAC_NM_EMPRESA, item.AUAC_IN_TIPO, item.AUAC_DT_INICIO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAutorizacao"] = 1;
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    return RedirectToAction("MontarTelaAutorizacao");
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaAutorizacao"] = listaObj;
                return RedirectToAction("MontarTelaAutorizacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaAutorizacao");
            }
        }

        public ActionResult VoltarBaseAutorizacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaAutorizacao"] == 1)
            {
                return RedirectToAction("Morador", "Morador");
            }
            return RedirectToAction("MontarTelaAutorizacao");
        }

        [HttpGet]
        public ActionResult IncluirAutorizacao()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensAutorizacao"] = 2;
                    return RedirectToAction("MontarTelaAutorizacao", "Autorizacao");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            }
            else
            {
                ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            }
            ViewBag.Graus = new SelectList(fornApp.GetAllGraus(idAss).OrderBy(x => x.GRPA_NM_NOME), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Docs = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIDO_NM_NOME), "TIDO_CD_ID", "TIDO_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Autorização", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Restrição", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> aviso = new List<SelectListItem>();
            aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aviso.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
            List<SelectListItem> perm = new List<SelectListItem>();
            perm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            perm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Permanente = new SelectList(perm, "Value", "Text");
            Session["VoltaProp"] = 4;

            // Prepara view
            AUTORIZACAO_ACESSO item = new AUTORIZACAO_ACESSO();
            AutorizacaoViewModel vm = Mapper.Map<AUTORIZACAO_ACESSO, AutorizacaoViewModel>(item);
            vm.AUAC_DT_INICIO = DateTime.Today.Date;
            vm.AUAC_DT_LIMITE = DateTime.Today.Date.AddDays(30);
            if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                vm.UNID_CD_ID = usuario.UNID_CD_ID;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
            }
            vm.AUAC_IN_AVISO = 1;
            vm.AUAC_IN_PERMANENTE = 2;
            vm.AUAC_IN_TIPO = 1;
            vm.AUAC_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirAutorizacao(AutorizacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Graus = new SelectList(fornApp.GetAllGraus(idAss).OrderBy(x => x.GRPA_NM_NOME), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Docs = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIDO_NM_NOME), "TIDO_CD_ID", "TIDO_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Autorização", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Restrição", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> aviso = new List<SelectListItem>();
            aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aviso.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
            List<SelectListItem> perm = new List<SelectListItem>();
            perm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            perm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Permanente = new SelectList(perm, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    AUTORIZACAO_ACESSO item = Mapper.Map<AutorizacaoViewModel, AUTORIZACAO_ACESSO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAutorizacao"] = 3;
                        return RedirectToAction("MontarTelaAutorizacao", "Autorizacao");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Autorizacoes/" + item.AUAC_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterForn = new List<AUTORIZACAO_ACESSO>();
                    Session["ListaAutorizacao"] = null;
                    Session["IncluirAut"] = 1;
                    Session["Autorizacao"] = fornApp.GetAllItens(idAss);
                    Session["IdVolta"] = item.AUAC_CD_ID;
                    if (Session["FileQueueAutorizacao"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueAutorizacao"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueAutorizacao(file);
                            }
                        }

                        Session["FileQueueAutorizacao"] = null;
                    }
                    if ((Int32)Session["VoltaAutorizacao"] == 2)
                    {
                        return RedirectToAction("IncluirAutorizacao");
                    }
                    return RedirectToAction("MontarTelaAutorizacao");
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
        public ActionResult EditarAutorizacao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensAutorizacao"] = 2;
                    return RedirectToAction("MontarTelaAutorizacao", "Autorizacao");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Graus = new SelectList(fornApp.GetAllGraus(idAss).OrderBy(x => x.GRPA_NM_NOME), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Docs = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIDO_NM_NOME), "TIDO_CD_ID", "TIDO_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Autorização", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Restrição", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> aviso = new List<SelectListItem>();
            aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aviso.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
            List<SelectListItem> perm = new List<SelectListItem>();
            perm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            perm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Permanente = new SelectList(perm, "Value", "Text");
            ViewBag.Incluir = (Int32)Session["IncluirAut"];

            if ((Int32)Session["MensAutorizacao"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensAutorizacao"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            AUTORIZACAO_ACESSO item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Autorizacao"] = item;
            Session["IdVolta"] = id;
            Session["IdAutorizacao"] = id;
            AutorizacaoViewModel vm = Mapper.Map<AUTORIZACAO_ACESSO, AutorizacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAutorizacao(AutorizacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Graus = new SelectList(fornApp.GetAllGraus(idAss).OrderBy(x => x.GRPA_NM_NOME), "GRPA_CD_ID", "GRPA_NM_NOME");
            ViewBag.Docs = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIDO_NM_NOME), "TIDO_CD_ID", "TIDO_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Autorização", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Restrição", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> aviso = new List<SelectListItem>();
            aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aviso.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
            List<SelectListItem> perm = new List<SelectListItem>();
            perm.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            perm.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Permanente = new SelectList(perm, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    AUTORIZACAO_ACESSO item = Mapper.Map<AutorizacaoViewModel, AUTORIZACAO_ACESSO>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<AUTORIZACAO_ACESSO>();
                    Session["ListaAutorizacao"] = null;
                    Session["IncluirAut"] = 0;
                    return RedirectToAction("MontarTelaAutorizacao");
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
        public ActionResult VerAutorizacao(Int32 id)
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
                    Session["MensAmbiente"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirAut"];

            AUTORIZACAO_ACESSO item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Autorizacao"] = item;
            Session["IdVolta"] = id;
            Session["IdAutorizacao"] = id;
            Session["VoltaAutorizacao"] = 1;
            AutorizacaoViewModel vm = Mapper.Map<AUTORIZACAO_ACESSO, AutorizacaoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirAutorizacao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensAutorizacao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            AUTORIZACAO_ACESSO item = fornApp.GetItemById(id);
            AutorizacaoViewModel vm = Mapper.Map<AUTORIZACAO_ACESSO, AutorizacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirAutorizacao(AutorizacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                AUTORIZACAO_ACESSO item = Mapper.Map<AutorizacaoViewModel, AUTORIZACAO_ACESSO>(vm);
                Int32 volta = fornApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAutorizacao"] = 4;
                    return RedirectToAction("MontarTelaAutorizacao", "Autorizacao");
                }

                // Sucesso
                listaMasterForn = new List<AUTORIZACAO_ACESSO>();
                Session["ListaAutorizacao"] = null;
                return RedirectToAction("MontarTelaAutorizacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult ReativarAutorizacao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensAutorizacao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            AUTORIZACAO_ACESSO item = fornApp.GetItemById(id);
            AutorizacaoViewModel vm = Mapper.Map<AUTORIZACAO_ACESSO, AutorizacaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarAutorizacao(AutorizacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                AUTORIZACAO_ACESSO item = Mapper.Map<AutorizacaoViewModel, AUTORIZACAO_ACESSO>(vm);
                Int32 volta = fornApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<AUTORIZACAO_ACESSO>();
                Session["ListaAutorizacao"] = null;
                return RedirectToAction("MontarTelaAutorizacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoAutorizacao(Int32 id)
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
                    Session["MensAutorizacao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            AUTORIZACAO_ACESSO_ANEXO item = fornApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoAutorizacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarAutorizacao", new { id = (Int32)Session["IdAutorizacao"] });
        }

        public FileResult DownloadAutorizacao(Int32 id)
        {
            AUTORIZACAO_ACESSO_ANEXO item = fornApp.GetAnexoById(id);
            String arquivo = item.AAAN_AQ_ARQUIVO;
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

            Session["FileQueueAutorizacao"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueAutorizacao(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensAutorizacao"] = 5;
                return RedirectToAction("VoltarAnexoAutorizacao");
            }

            AUTORIZACAO_ACESSO item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAutorizacao"] = 6;
                return RedirectToAction("VoltarAnexoAutorizacao");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Autorizacoes/" + item.AUAC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            AUTORIZACAO_ACESSO_ANEXO foto = new AUTORIZACAO_ACESSO_ANEXO();
            foto.AAAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.AAAN_DT_ANEXO = DateTime.Today;
            foto.AAAN_IN_ATIVO = 1;
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
            foto.AAAN_IN_TIPO = tipo;
            foto.AAAN_NM_TITULO = fileName;
            foto.AUAC_CD_ID = item.AUAC_CD_ID;

            item.AUTORIZACAO_ACESSO_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoAutorizacao");
        }

        [HttpPost]
        public ActionResult UploadFileAutorizacao(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensAutorizacao"] = 5;
                return RedirectToAction("VoltarAnexoAutorizacao");
            }

            AUTORIZACAO_ACESSO item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAutorizacao"] = 6;
                return RedirectToAction("VoltarAnexoAutorizacao");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Autorizacoes/" + item.AUAC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            AUTORIZACAO_ACESSO_ANEXO foto = new AUTORIZACAO_ACESSO_ANEXO();
            foto.AAAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.AAAN_DT_ANEXO = DateTime.Today;
            foto.AAAN_IN_ATIVO = 1;
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
            foto.AAAN_IN_TIPO = tipo;
            foto.AAAN_NM_TITULO = fileName;
            foto.AUAC_CD_ID = item.AUAC_CD_ID;

            item.AUTORIZACAO_ACESSO_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoAutorizacao");
        }

        [HttpPost]
        public JsonResult FiltrarUsuarioUnidade(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaUsuarios = fornApp.GetAllUsuarios(idAss);

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaUsuarios = listaUsuarios.Where(x => x.UNID_CD_ID == id).ToList();
            }

            return Json(listaUsuarios.Select(x => new { x.USUA_CD_ID, x.USUA_NM_NOME }));
        }

        [HttpPost]
        public JsonResult FiltrarUnidadeUsuario(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaFiltrada = fornApp.GetAllUnidades(idAss);

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaFiltrada = listaFiltrada.Where(x => x.USUARIO.Any(s => s.USUA_CD_ID == id)).ToList();
            }

            return Json(listaFiltrada.Select(x => new { x.UNID_CD_ID, x.UNID_NM_EXIBE }));
        }

        public ActionResult VerEntradaSaida(Int32 idEntrada)
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            //return RedirectToAction("VerEntradaSaida", "EntradaSaida");
            return RedirectToAction("VerEntradaSaida", new { id = idEntrada });
        }

        public ActionResult IncluirEntradaSaida(Int32 id)
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            return RedirectToAction("IncluirEntradaSaida", "EntradaSaida");
        }

    }
}