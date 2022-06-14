using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using EntitiesServices.WorkClasses;

namespace ERP_Condominios_Solution.Controllers
{
    public class EncomendaController : Controller
    {
        private readonly IEncomendaAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUnidadeAppService uniApp;
        private String msg;
        private Exception exception;
        ENCOMENDA objetoForn = new ENCOMENDA();
        ENCOMENDA objetoFornAntes = new ENCOMENDA();
        List<ENCOMENDA> listaMasterForn = new List<ENCOMENDA>();
        String extensao;

        public EncomendaController(IEncomendaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IUnidadeAppService uniApps)
        {
            fornApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
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
        public ActionResult MontarTelaEncomenda()
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
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaEncomenda"] == null)
            {
                listaMasterForn = fornApp.GetItemByData(DateTime.Today.Date, idAss);
                Session["ListaEncomenda"] = listaMasterForn;
            }

            ViewBag.Listas = (List<ENCOMENDA>)Session["ListaEncomenda"];
            ViewBag.Title = "Encomendas";
            ViewBag.Formas = new SelectList(fornApp.GetAllFormas(idAss).OrderBy(x => x.FOEN_NM_NOME), "FOEN_CD_ID", "FOEN_NM_NOME");
            ViewBag.Tipos = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIEN_NM_NOME), "TIEN_CD_ID", "TIEN_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Recebida", Value = "1" });
            status.Add(new SelectListItem() { Text = "Entregue", Value = "2" });
            status.Add(new SelectListItem() { Text = "Recusada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Devolvida", Value = "4" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            Session["IncluirEnc"] = 0;

            // Indicadores
            ViewBag.Encomendas = ((List<ENCOMENDA>)Session["ListaEncomenda"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensEncomenda"] != null)
            {
                // Mensagem
                //if ((Int32)Session["MensOcorrencia"] == 1)
                //{
                //    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                //}
                if ((Int32)Session["MensEncomenda"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new ENCOMENDA();
            objetoForn.ENCO_IN_ATIVO = 1;
            objetoForn.ENCO_DT_CHEGADA = DateTime.Today.Date;
            Session["MensEncomenda"] = 0;
            Session["VoltaEncomenda"] = 1;
            return View(objetoForn);
        }

        [HttpGet]
        public ActionResult MontarTelaEncomendaNaoEntregue()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaEncomenda"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss).Where(p => p.ENCO_IN_STATUS == 1).ToList();
                Session["ListaEncomenda"] = listaMasterForn;
            }

            ViewBag.Listas = (List<ENCOMENDA>)Session["ListaEncomenda"];
            ViewBag.Title = "Encomendas";
            ViewBag.Formas = new SelectList(fornApp.GetAllFormas(idAss).OrderBy(x => x.FOEN_NM_NOME), "FOEN_CD_ID", "FOEN_NM_NOME");
            ViewBag.Tipos = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIEN_NM_NOME), "TIEN_CD_ID", "TIEN_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            Session["IncluirEnc"] = 0;

            // Indicadores
            ViewBag.Encomendas = ((List<ENCOMENDA>)Session["ListaEncomenda"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensEncomenda"] != null)
            {
                // Mensagem
                //if ((Int32)Session["MensOcorrencia"] == 1)
                //{
                //    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                //}
                if ((Int32)Session["MensEncomenda"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new ENCOMENDA();
            objetoForn.ENCO_IN_ATIVO = 1;
            objetoForn.ENCO_DT_CHEGADA = DateTime.Today.Date;
            Session["MensEncomenda"] = 0;
            Session["VoltaEncomenda"] = 3;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaEncomenda"] = null;
            Session["FiltroEncomenda"] = null;
            return RedirectToAction("MontarTelaEncomenda");
        }

        public ActionResult RetirarFiltroEncomendaNaoEntregue()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaEncomenda"] = null;
            Session["FiltroEncomenda"] = null;
            return RedirectToAction("MontarTelaEncomendaNaoEntregue");
        }

        public ActionResult VerTodasEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItens(idAss);
            Session["ListaEncomenda"] = listaMasterForn;
            Session["FiltroEncomenda"] = null;
            return RedirectToAction("MontarTelaEncomenda");
        }

        public ActionResult MostrarTudoEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroEncomenda"] = null;
            Session["ListaEncomenda"] = listaMasterForn;
            return RedirectToAction("MontarTelaEncomenda");
        }

        [HttpPost]
        public ActionResult FiltrarEncomenda(ENCOMENDA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ENCOMENDA> listaObj = new List<ENCOMENDA>();
                Session["FiltroEncomenda"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.UNID_CD_ID, item.FOEN_CD_ID, item.TIEN_CD_ID, item.ENCO_DT_CHEGADA, item.ENCO_IN_STATUS, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEncomenda"] = 1;
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaEncomenda"] = listaObj;
                return RedirectToAction("MontarTelaEncomenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEncomenda");
            }
        }

        [HttpPost]
        public ActionResult FiltrarEncomendaNaoEntregue(ENCOMENDA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ENCOMENDA> listaObj = new List<ENCOMENDA>();
                Session["FiltroEncomenda"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.UNID_CD_ID, item.FOEN_CD_ID, item.TIEN_CD_ID, item.ENCO_DT_CHEGADA, item.ENCO_IN_STATUS, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEncomenda"] = 1;
                }

                // Sucesso
                listaMasterForn = listaObj.Where(p => p.ENCO_IN_STATUS == 1).ToList();
                Session["ListaEncomenda"] = listaObj;
                return RedirectToAction("MontarTelaEncomendaNaoEntregue");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEncomendaNaoEntregue");
            }
        }

        public ActionResult VoltarBaseEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaEncomenda"] == 2)
            {
                return RedirectToAction("MontarTelaMorador", "Morador");
            }
            if ((Int32)Session["VoltaEncomenda"] == 3)
            {
                return RedirectToAction("MontarTelaEncomendaNaoEntregue");
            }
            return RedirectToAction("MontarTelaEncomenda");
        }

        [HttpGet]
        public ActionResult IncluirEncomenda()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIEN_NM_NOME).ToList<TIPO_ENCOMENDA>(), "TIEN_CD_ID", "TIEN_NM_NOME");
            ViewBag.Formas = new SelectList(fornApp.GetAllFormas(idAss).OrderBy(x => x.FOEN_NM_NOME).ToList<FORMA_ENTREGA>(), "FOEN_CD_ID", "FOEN_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");

            // Prepara view
            ENCOMENDA item = new ENCOMENDA();
            EncomendaViewModel vm = Mapper.Map<ENCOMENDA, EncomendaViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.ENCO_DT_CHEGADA = DateTime.Now;
            vm.ENCO_IN_ATIVO = 1;
            vm.ENCO_IN_CONFIRMADO = 0;
            vm.ENCO_IN_STATUS = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirEncomenda(EncomendaViewModel vm)
        {
            Hashtable result = new Hashtable();
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIEN_NM_NOME).ToList<TIPO_ENCOMENDA>(), "TIEN_CD_ID", "TIEN_NM_NOME");
            ViewBag.Formas = new SelectList(fornApp.GetAllFormas(idAss).OrderBy(x => x.FOEN_NM_NOME).ToList<FORMA_ENTREGA>(), "FOEN_CD_ID", "FOEN_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ENCOMENDA item = Mapper.Map<EncomendaViewModel, ENCOMENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEncomenda"] = 3;
                        return RedirectToAction("MontarTelaEncomenda", "Encomenda");
                    }

                    // Carrega foto e processa alteracao
                    item.ENCO_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                    volta = fornApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Encomenda/" + item.ENCO_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Encomenda/" + item.ENCO_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.ENCO_CD_ID;
                    if (Session["FileQueueEncomenda"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueEncomenda"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueEncomenda(file);
                            }
                            else
                            {
                                UploadFotoQueueEncomenda(file);
                            }
                        }

                        Session["FileQueueEncomenda"] = null;
                    }

                    vm.ENCO_CD_ID = item.ENCO_CD_ID;
                    Session["IdEncomenda"] = item.ENCO_CD_ID;

                    // Sucesso
                    listaMasterForn = new List<ENCOMENDA>();
                    Session["ListaEncomenda"] = null;
                    if ((Int32)Session["VoltaEncomenda"] == 2)
                    {
                        return RedirectToAction("VerEncomendaDiferente");
                    }
                    return RedirectToAction("MontarTelaEncomenda");
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
        public ActionResult EditarEncomenda(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("MontarTelaEncomenda", "Encomenda");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirEnc"];

            if ((Int32)Session["MensEncomenda"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensEncomenda"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensEncomenda"] == 7)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensEncomenda"] == 8)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0077", CultureInfo.CurrentCulture));
            }

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            ENCOMENDA item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            List<SelectListItem> status = new List<SelectListItem>();
            if (item.ENCO_IN_STATUS == 1)
            {
                status.Add(new SelectListItem() { Text = "Entregar", Value = "2" });
                status.Add(new SelectListItem() { Text = "Recusar", Value = "3" });
            }
            else if (item.ENCO_IN_STATUS == 2)
            {
                status.Add(new SelectListItem() { Text = "Devolver", Value = "4" });
            }
            ViewBag.Status = new SelectList(status, "Value", "Text");

            Session["Encomenda"] = item;
            Session["IdVolta"] = id;
            Session["IdEncomenda"] = id;
            EncomendaViewModel vm = Mapper.Map<ENCOMENDA, EncomendaViewModel>(item);
            if (vm.ENCO_IN_STATUS == 1)
            {
                vm.ENCO_IN_STATUS_TROCA = 2;
            }
            if (vm.ENCO_IN_STATUS == 2)
            {
                vm.ENCO_IN_STATUS_TROCA = 4;
            }
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarEncomenda(EncomendaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<SelectListItem> status = new List<SelectListItem>();
            if (vm.ENCO_IN_STATUS == 1)
            {
                status.Add(new SelectListItem() { Text = "Entregar", Value = "2" });
                status.Add(new SelectListItem() { Text = "Recusar", Value = "3" });
            }
            else if (vm.ENCO_IN_STATUS == 2)
            {
                status.Add(new SelectListItem() { Text = "Devolver", Value = "4" });
            }

            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Acerta status
                    if (vm.ENCO_IN_STATUS_TROCA > 0)
                    {
                        vm.ENCO_IN_STATUS = vm.ENCO_IN_STATUS_TROCA;
                    }                  

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ENCOMENDA item = Mapper.Map<EncomendaViewModel, ENCOMENDA>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEncomenda"] = 7;
                        return RedirectToAction("EditarEncomenda", "Encomenda");
                    }
                    if (volta == 2)
                    {
                        Session["MensEncomenda"] = 8;
                        return RedirectToAction("EditarEncomenda", "Encomenda");
                    }

                    // Sucesso
                    Session["MensEncomenda"] = 0;
                    listaMasterForn = new List<ENCOMENDA>();
                    Session["ListaEncomenda"] = null;
                    Session["IncluirEnc"] = 0;
                    return RedirectToAction("MontarTelaEncomenda");
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
        public ActionResult VerEncomenda(Int32 id)
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
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirEnc"];

            ENCOMENDA item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Encomenda"] = item;
            Session["IdVolta"] = id;
            Session["IdEncomenda"] = id;
            Session["VoltaEncomenda"] = 1;
            EncomendaViewModel vm = Mapper.Map<ENCOMENDA, EncomendaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirEncomenda(Int32 id)
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
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ENCOMENDA item = fornApp.GetItemById(id);
            EncomendaViewModel vm = Mapper.Map<ENCOMENDA, EncomendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirEncomenda(EncomendaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                ENCOMENDA item = Mapper.Map<EncomendaViewModel, ENCOMENDA>(vm);
                Int32 volta = fornApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<ENCOMENDA>();
                Session["ListaEncomenda"] = null;
                return RedirectToAction("MontarTelaEncomenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult ReativarEncomenda(Int32 id)
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
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ENCOMENDA item = fornApp.GetItemById(id);
            EncomendaViewModel vm = Mapper.Map<ENCOMENDA, EncomendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarEncomenda(EncomendaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                ENCOMENDA item = Mapper.Map<EncomendaViewModel, ENCOMENDA>(vm);
                Int32 volta = fornApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<ENCOMENDA>();
                Session["ListaEncomenda"] = null;
                return RedirectToAction("MontarTelaEncomenda");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoEncomenda(Int32 id)
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
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ENCOMENDA_ANEXO item = fornApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarEncomenda", new { id = (Int32)Session["IdEncomenda"] });
        }

        public FileResult DownloadEncomenda(Int32 id)
        {
            ENCOMENDA_ANEXO item = fornApp.GetAnexoById(id);
            String arquivo = item.ENAN_AQ_ARQUIVO;
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
            Session["FileQueueEncomenda"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueEncomenda(FileQueue file)
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
                Session["MensEncomenda"] = 5;
                return RedirectToAction("VoltarAnexoEncomenda");
            }

            ENCOMENDA item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensEncomenda"] = 6;
                return RedirectToAction("VoltarAnexoEncomenda");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Encomenda/" + item.ENCO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            ENCOMENDA_ANEXO foto = new ENCOMENDA_ANEXO();
            foto.ENAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.ENAN_DT_ANEXO = DateTime.Today;
            foto.ENAN_IN_ATIVO = 1;
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
            foto.ENAN_IN_TIPO = tipo;
            foto.ENAN_NM_TITULO = fileName;
            foto.ENCO_CD_ID = item.ENCO_CD_ID;

            item.ENCOMENDA_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoEncomenda");
        }

       [HttpPost]
        public ActionResult UploadFileEncomenda(HttpPostedFileBase file)
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
                Session["MensEncomenda"] = 5;
                return RedirectToAction("VoltarAnexoEncomenda");
            }

            ENCOMENDA item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensEncomenda"] = 6;
                return RedirectToAction("VoltarAnexoEncomenda");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Encomenda/" + item.ENCO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            ENCOMENDA_ANEXO foto = new ENCOMENDA_ANEXO();
            foto.ENAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.ENAN_DT_ANEXO = DateTime.Today;
            foto.ENAN_IN_ATIVO = 1;
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
            foto.ENAN_IN_TIPO = tipo;
            foto.ENAN_NM_TITULO = fileName;
            foto.ENCO_CD_ID = item.ENCO_CD_ID;

            item.ENCOMENDA_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoEncomenda");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueEncomenda(FileQueue file)
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
                Session["MensEncomenda"] = 5;
                return RedirectToAction("VoltarAnexoEncomenda");
            }

            ENCOMENDA item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensEncomenda"] = 6;
                return RedirectToAction("VoltarAnexoEncomenda");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Encomenda/" + item.ENCO_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                item.ENCO_AQ_FOTO = "~" + caminho + fileName;
                objetoForn = item;
                Int32 volta = fornApp.ValidateEdit(item, objetoForn);
            }
            return RedirectToAction("VoltarAnexoEncomenda");
        }

        [HttpPost]
        public ActionResult UploadFotoEncomenda(HttpPostedFileBase file)
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
                Session["MensEncomenda"] = 5;
                return RedirectToAction("VoltarAnexoEncomenda");
            }

            ENCOMENDA item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensEncomenda"] = 6;
                return RedirectToAction("VoltarAnexoEncomenda");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Encomenda/" + item.ENCO_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                file.SaveAs(path);

                // Gravar registro
                item.ENCO_AQ_FOTO = "~" + caminho + fileName;
                objetoForn = item;
                Int32 volta = fornApp.ValidateEdit(item, objetoForn);
            }
            return RedirectToAction("VoltarAnexoEncomenda");
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

        [HttpGet]
        public ActionResult GerarNotificacaoEncomenda()
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
                    Session["MensEncomenda"] = 2;
                    return RedirectToAction("MontarTelaEncomenda", "Encomenda");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 unidade = (Int32)Session["IdUnidade"];
            List<USUARIO> lista = fornApp.GetAllUsuarios(idAss);

            // Prepara view
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
            vm.CANO_CD_ID = 5;
            vm.NOTI_NM_TITULO = "Notificação de Encomenda";
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoEncomenda(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ENCOMENDA veiculo = (ENCOMENDA)Session["Encomenda"];
            String unid = veiculo.UNIDADE.UNID_NM_EXIBE;
            List<USUARIO> lista = fornApp.GetAllUsuarios(idAss);
            ViewBag.Usuarios = new SelectList(lista, "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    Int32 volta = fornApp.GerarNotificacao(item, usuarioLogado, veiculo, unid, "NOTIOCOR");

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<ENCOMENDA>();
                    return RedirectToAction("VoltarBaseEncomenda");
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

        public ActionResult IncluirComentarioEncomenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ENCOMENDA item = fornApp.GetItemById((Int32)Session["IdEncomenda"]);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ENCOMENDA_COMENTARIO coment = new ENCOMENDA_COMENTARIO();
            EncomendaComentarioViewModel vm = Mapper.Map<ENCOMENDA_COMENTARIO, EncomendaComentarioViewModel>(coment);
            vm.ECOM_DT_COMENTARIO = DateTime.Now;
            vm.ECOM_IN_ATIVO = 1;
            vm.ENCO_CD_ID = item.ENCO_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirComentarioEncomenda(EncomendaComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ENCOMENDA_COMENTARIO item = Mapper.Map<EncomendaComentarioViewModel, ENCOMENDA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ENCOMENDA not = fornApp.GetItemById((Int32)Session["IdEncomenda"]);

                    item.USUARIO = null;
                    not.ENCOMENDA_COMENTARIO.Add(item);
                    objetoFornAntes = not;
                    Int32 volta = fornApp.ValidateEdit(not, objetoFornAntes);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("EditarEncomenda", new { id = (Int32)Session["IdEncomenda"] });
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