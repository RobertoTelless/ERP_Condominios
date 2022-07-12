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
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class AmbienteController : Controller
    {
        private readonly IAmbienteAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;

        private String msg;
        private Exception exception;
        AMBIENTE objetoForn = new AMBIENTE();
        AMBIENTE objetoFornAntes = new AMBIENTE();
        List<AMBIENTE> listaMasterForn = new List<AMBIENTE>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public AmbienteController(IAmbienteAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps, IUsuarioAppService usuApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            confApp = confApps;
            usuApp = usuApps;
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
        public ActionResult MontarTelaAmbiente()
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

            // Carrega listas
            if (Session["ListaAmbiente"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaAmbiente"] = listaMasterForn;
            }
            ViewBag.Listas = (List<AMBIENTE>)Session["ListaAmbiente"];
            ViewBag.Title = "Ambientes";
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIAM_NM_NOME), "TIAM_CD_ID", "TIAM_NM_NOME");
            Session["IncluirAmb"] = 0;

            // Indicadores
            ViewBag.Ambientes = ((List<AMBIENTE>)Session["ListaAmbiente"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> gratuito = new List<SelectListItem>();
            gratuito.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            gratuito.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Gratuito = new SelectList(gratuito, "Value", "Text");

            if (Session["MensAmbiente"] != null)
            {
                // Mensagem
                //if ((Int32)Session["MensAmbiente"] == 1)
                //{
                //    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                //}
                if ((Int32)Session["MensAmbiente"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAmbiente"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0029", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAmbiente"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0030", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new AMBIENTE();
            objetoForn.AMBI_IN_ATIVO = 1;
            Session["MensAmbiente"] = 0;
            Session["VoltaAmbiente"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroAmbiente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaAmbiente"] = null;
            Session["FiltroAmbiente"] = null;
            return RedirectToAction("MontarTelaAmbiente");
        }

        public ActionResult MostrarTudoAmbiente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroAmbiente"] = null;
            Session["ListaAmbiente"] = listaMasterForn;
            return RedirectToAction("MontarTelaAmbiente");
        }

        [HttpPost]
        public ActionResult FiltrarAmbiente(AMBIENTE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<AMBIENTE> listaObj = new List<AMBIENTE>();
                Session["FiltroAmbiente"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.TIAM_CD_ID, item.AMBI_NM_AMBIENTE, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAmbiente"] = 1;
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaAmbiente"] = listaObj;
                return RedirectToAction("MontarTelaAmbiente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaAmbiente");
            }
        }

        public ActionResult VoltarBaseAmbiente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaAmbiente"] = null;
            return RedirectToAction("MontarTelaAmbiente");
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

        [HttpGet]
        public ActionResult IncluirAmbiente()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensAmbiente"] = 2;
                    return RedirectToAction("MontarTelaAmbiente", "Ambiente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIAM_NM_NOME), "TIAM_CD_ID", "TIAM_NM_NOME");
            List<SelectListItem> gratuito = new List<SelectListItem>();
            gratuito.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            gratuito.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Gratuito = new SelectList(gratuito, "Value", "Text");
            List<SelectListItem> chave = new List<SelectListItem>();
            chave.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            chave.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Chave = new SelectList(chave, "Value", "Text");
            Session["VoltaProp"] = 4;

            // Prepara view
            AMBIENTE item = new AMBIENTE();
            AmbienteViewModel vm = Mapper.Map<AMBIENTE, AmbienteViewModel>(item);
            vm.AMBI_DT_CADASTRO = DateTime.Today;
            vm.AMBI_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirAmbiente(AmbienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIAM_NM_NOME), "TIAM_CD_ID", "TIAM_NM_NOME");
            List<SelectListItem> gratuito = new List<SelectListItem>();
            gratuito.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            gratuito.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Gratuito = new SelectList(gratuito, "Value", "Text");
            List<SelectListItem> chave = new List<SelectListItem>();
            chave.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            chave.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Chave = new SelectList(chave, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    AMBIENTE item = Mapper.Map<AmbienteViewModel, AMBIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAmbiente"] = 3;
                        return RedirectToAction("MontarTelaAmbiente", "Ambiente");
                    }

                    // Carrega foto e processa alteracao
                    if (item.AMBI_AQ_FOTO == null)
                    {
                        item.AMBI_AQ_FOTO = "~/Imagens/Base/FotoBase.jpg";
                        volta = fornApp.ValidateEdit(item, item, usuarioLogado);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterForn = new List<AMBIENTE>();
                    Session["ListaAmbiente"] = null;
                    Session["IncluirAmb"] = 1;
                    Session["Ambientes"] = fornApp.GetAllItens(idAss);
                    Session["IdVolta"] = item.AMBI_CD_ID;
                    if (Session["FileQueueAmbiente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueAmbiente"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueAmbiente(file);
                            }
                            else
                            {
                                UploadFotoQueueAmbiente(file);
                            }
                        }

                        Session["FileQueueAmbiente"] = null;
                    }
                    if ((Int32)Session["VoltaAmbiente"] == 2)
                    {
                        return RedirectToAction("IncluirAmbiente");
                    }
                    return RedirectToAction("MontarTelaAmbiente");
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
        public ActionResult EditarAmbiente(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    Session["MensAmbiente"] = 2;
                    return RedirectToAction("MontarTelaAmbiente", "Ambiente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIAM_NM_NOME), "TIAM_CD_ID", "TIAM_NM_NOME");
            List<SelectListItem> gratuito = new List<SelectListItem>();
            gratuito.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            gratuito.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Gratuito = new SelectList(gratuito, "Value", "Text");
            List<SelectListItem> chave = new List<SelectListItem>();
            chave.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            chave.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Chave = new SelectList(chave, "Value", "Text");
            ViewBag.Incluir = (Int32)Session["IncluirAmb"];

            if ((Int32)Session["MensAmbiente"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensAmbiente"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensAmbiente"] == 7)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0031", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensAmbiente"] == 8)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0032", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensAmbiente"] == 9)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0033", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensAmbiente"] == 10)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0064", CultureInfo.CurrentCulture));
            }

            //Recupera ambiente
            AMBIENTE item = fornApp.GetItemById(id);

            // Verifica chave
            Int32 chaveUso = item.AMBIENTE_CHAVE.Where(p => p.AMCH_DT_DEVOLUCAO == null).ToList().Count;
            Session["ChaveUsada"] = chaveUso;

            // Prepara view
            objetoFornAntes = item;
            Session["Ambiente"] = item;
            Session["IdVolta"] = id;
            Session["IdAmbiente"] = id;
            AmbienteViewModel vm = Mapper.Map<AMBIENTE, AmbienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarAmbiente(AmbienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.TIAM_NM_NOME), "TIAM_CD_ID", "TIAM_NM_NOME");
            List<SelectListItem> gratuito = new List<SelectListItem>();
            gratuito.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            gratuito.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Gratuito = new SelectList(gratuito, "Value", "Text");
            List<SelectListItem> chave = new List<SelectListItem>();
            chave.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            chave.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Chave = new SelectList(chave, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    AMBIENTE item = Mapper.Map<AmbienteViewModel, AMBIENTE>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<AMBIENTE>();
                    Session["ListaAmbiente"] = null;
                    Session["IncluirAmb"] = 0;
                    return RedirectToAction("MontarTelaAmbiente");
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
        public ActionResult VerAmbiente(Int32 id)
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
            ViewBag.Incluir = (Int32)Session["IncluirAmb"];

            AMBIENTE item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Ambiente"] = item;
            Session["IdVolta"] = id;
            Session["IdAmbiente"] = id;
            AmbienteViewModel vm = Mapper.Map<AMBIENTE, AmbienteViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirAmbiente(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
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
            AMBIENTE item = fornApp.GetItemById(id);
            AmbienteViewModel vm = Mapper.Map<AMBIENTE, AmbienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirAmbiente(AmbienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                AMBIENTE item = Mapper.Map<AmbienteViewModel, AMBIENTE>(vm);
                Int32 volta = fornApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensAmbiente"] = 4;
                    return RedirectToAction("MontarTelaAmbiente", "Ambiente");
                }

                // Sucesso
                listaMasterForn = new List<AMBIENTE>();
                Session["ListaAmbiente"] = null;
                return RedirectToAction("MontarTelaAmbiente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult ReativarAmbiente(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
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
            AMBIENTE item = fornApp.GetItemById(id);
            AmbienteViewModel vm = Mapper.Map<AMBIENTE, AmbienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarAmbiente(AmbienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                AMBIENTE item = Mapper.Map<AmbienteViewModel, AMBIENTE>(vm);
                Int32 volta = fornApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<AMBIENTE>();
                Session["ListaAmbiente"] = null;
                return RedirectToAction("MontarTelaAmbiente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoAmbiente(Int32 id)
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
            AMBIENTE_IMAGEM item = fornApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoAmbiente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarAmbiente", new { id = (Int32)Session["IdVolta"] });
        }

        public FileResult DownloadAmbiente(Int32 id)
        {
            AMBIENTE_IMAGEM item = fornApp.GetAnexoById(id);
            String arquivo = item.AMIM_NM_ARQUIVO;
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

            Session["FileQueueAmbiente"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueAmbiente(FileQueue file)
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
                Session["MensAmbiente"] = 5;
                return RedirectToAction("VoltarAnexoAmbiente");
            }

            AMBIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAmbiente"] = 6;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            AMBIENTE_IMAGEM foto = new AMBIENTE_IMAGEM();
            foto.AMIM_NM_ARQUIVO = "~" + caminho + fileName;
            foto.AMIM_DT_CADASTRO = DateTime.Today;
            foto.AMIM_IN_ATIVO = 1;
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
            foto.AMIM_IN_TIPO = tipo;
            foto.AMIN_NM_TITULO = fileName;
            foto.AMBI_CD_ID = item.AMBI_CD_ID;

            item.AMBIENTE_IMAGEM.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpPost]
        public ActionResult UploadFileAmbiente(HttpPostedFileBase file)
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
                Session["MensAmbiente"] = 5;
                return RedirectToAction("VoltarAnexoAmbiente");
            }

            AMBIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAmbiente"] = 6;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            AMBIENTE_IMAGEM foto = new AMBIENTE_IMAGEM();
            foto.AMIM_NM_ARQUIVO = "~" + caminho + fileName;
            foto.AMIM_DT_CADASTRO = DateTime.Today;
            foto.AMIM_IN_ATIVO = 1;
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
            foto.AMIM_IN_TIPO = tipo;
            foto.AMIN_NM_TITULO = fileName;
            foto.AMBI_CD_ID = item.AMBI_CD_ID;

            item.AMBIENTE_IMAGEM.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueAmbiente(FileQueue file)
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
                Session["MensAmbiente"] = 5;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            AMBIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAmbiente"] = 6;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.AMBI_AQ_FOTO = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            listaMasterForn = new List<AMBIENTE>();
            Session["ListaAmbiente"] = null;
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpPost]
        public ActionResult UploadFotoAmbiente(HttpPostedFileBase file)
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
                Session["MensAmbiente"] = 5;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            AMBIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAmbiente"] = 6;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.AMBI_AQ_FOTO = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            listaMasterForn = new List<AMBIENTE>();
            Session["ListaAmbiente"] = null;
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpPost]
        public ActionResult UploadNormaUso(HttpPostedFileBase file)
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
                Session["MensAmbiente"] = 5;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            AMBIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAmbiente"] = 6;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.AMBI_AQ_NORMAS_USO = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            listaMasterForn = new List<AMBIENTE>();
            Session["ListaAmbiente"] = null;
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpPost]
        public ActionResult UploadListaMaterial(HttpPostedFileBase file)
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
                Session["MensAmbiente"] = 5;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            AMBIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensAmbiente"] = 6;
                return RedirectToAction("VoltarAnexoAmbiente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ambiente/" + item.AMBI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.AMBI_AQ_LISTA = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            listaMasterForn = new List<AMBIENTE>();
            Session["ListaAmbiente"] = null;
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpGet]
        public ActionResult EditarAmbienteCusto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            AMBIENTE_CUSTO item = fornApp.GetAmbienteCustoById(id);
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            AmbienteCustoViewModel vm = Mapper.Map<AMBIENTE_CUSTO, AmbienteCustoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAmbienteCusto(AmbienteCustoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    AMBIENTE_CUSTO item = Mapper.Map<AmbienteCustoViewModel, AMBIENTE_CUSTO>(vm);
                    Int32 volta = fornApp.ValidateEditAmbienteCusto(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoAmbiente");
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
        public ActionResult ExcluirAmbienteCusto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            AMBIENTE_CUSTO item = fornApp.GetAmbienteCustoById(id);
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            item.AMCU_NM_ATIVO = 0;
            Int32 volta = fornApp.ValidateEditAmbienteCusto(item);
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpGet]
        public ActionResult ReativarAmbienteCusto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            AMBIENTE_CUSTO item = fornApp.GetAmbienteCustoById(id);
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            item.AMCU_NM_ATIVO = 1;
            Int32 volta = fornApp.ValidateEditAmbienteCusto(item);
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpGet]
        public ActionResult IncluirAmbienteCusto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            AMBIENTE_CUSTO item = new AMBIENTE_CUSTO();
            AmbienteCustoViewModel vm = Mapper.Map<AMBIENTE_CUSTO, AmbienteCustoViewModel>(item);
            vm.AMBI_CD_ID = (Int32)Session["IdVolta"];
            vm.AMCU_NM_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAmbienteCusto(AmbienteCustoViewModel vm)
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
                    AMBIENTE_CUSTO item = Mapper.Map<AmbienteCustoViewModel, AMBIENTE_CUSTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreateAmbienteCusto(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoAmbiente");
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
        public ActionResult EditarAmbienteChave(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Verifica
            AMBIENTE_CHAVE ch = fornApp.GetAmbienteChaveById(id);
            if (ch.AMCH_DT_DEVOLUCAO != null)
            {
                Session["MensAmbiente"] = 8;
                return RedirectToAction("VoltarAnexoAmbiente");
            }

            // Prepara view
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            AmbienteChaveViewModel vm = Mapper.Map<AMBIENTE_CHAVE, AmbienteChaveViewModel>(ch);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAmbienteChave(AmbienteChaveViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    AMBIENTE_CHAVE item = Mapper.Map<AmbienteChaveViewModel, AMBIENTE_CHAVE>(vm);
                    Int32 volta = fornApp.ValidateEditAmbienteChave(item, usuarioLogado);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoAmbiente");
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
        public ActionResult DevolverAmbienteChave(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Verifica
            AMBIENTE_CHAVE ch = fornApp.GetAmbienteChaveById(id);
            if (ch.AMCH_DT_DEVOLUCAO != null)
            {
                Session["MensAmbiente"] = 8;
                return RedirectToAction("VoltarAnexoAmbiente");
            }

            // Prepara view
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            AmbienteChaveViewModel vm = Mapper.Map<AMBIENTE_CHAVE, AmbienteChaveViewModel>(ch);
            vm.AMCH_DT_DEVOLUCAO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DevolverAmbienteChave(AmbienteChaveViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    AMBIENTE_CHAVE item = Mapper.Map<AmbienteChaveViewModel, AMBIENTE_CHAVE>(vm);
                    if (item.AMCH_DT_DEVOLUCAO == null)
                    {
                        ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    Int32 volta = fornApp.ValidateEditAmbienteChave(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAmbiente"] = 10;
                        return RedirectToAction("VoltarAnexoAmbiente", "Ambiente");
                    }
                    Session["MensAmbiente"] = 0;
                    return RedirectToAction("VoltarAnexoAmbiente");
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
        public ActionResult ExcluirAmbienteChave(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            AMBIENTE_CHAVE item = fornApp.GetAmbienteChaveById(id);
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            item.AMCH_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateEditAmbienteChave(item, usuarioLogado);
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpGet]
        public ActionResult ReativarAmbienteChave(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            AMBIENTE_CHAVE item = fornApp.GetAmbienteChaveById(id);
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            item.AMCH_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateEditAmbienteChave(item, usuarioLogado);
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpGet]
        public ActionResult IncluirAmbienteChave()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica chave
            AMBIENTE amb = (AMBIENTE)Session["Ambiente"];
            if (amb.AMBIENTE_CHAVE.Where(p => p.AMCH_DT_DEVOLUCAO == null).ToList().Count > 0)
            {
                Session["MensAmbiente"] = 7;
                return RedirectToAction("VoltarAnexoAmbiente");
            }

            // Listas
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList(), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME).ToList(), "USUA_CD_ID", "USUA_NM_NOME");

            // Prepara view
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            AMBIENTE_CHAVE item = new AMBIENTE_CHAVE();
            AmbienteChaveViewModel vm = Mapper.Map<AMBIENTE_CHAVE, AmbienteChaveViewModel>(item);
            vm.AMBI_CD_ID = (Int32)Session["IdVolta"];
            vm.ASSI_CD_ID = (Int32)Session["IdAssinante"];
            vm.AMCH_DT_ENTREGA = DateTime.Today.Date;
            if (amb.AMBI_NR_DIAS_CHAVE > 0)
            {
                Double dias = Convert.ToDouble(amb.AMBI_NR_DIAS_CHAVE);
                vm.AMCH_DT_PREVISTA = DateTime.Today.Date.AddDays(dias);
            }
            vm.AMCH_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAmbienteChave(AmbienteChaveViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList(), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME).ToList(), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    AMBIENTE_CHAVE item = Mapper.Map<AmbienteChaveViewModel, AMBIENTE_CHAVE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreateAmbienteChave(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAmbiente"] = 9;
                        return RedirectToAction("VoltarAnexoAmbiente");
                    }

                    Session["MensAmbiente"] = 0;
                    return RedirectToAction("VoltarAnexoAmbiente");
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
        public ActionResult EnviarEMailAmbiente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO cont = usuApp.GetItemById(id);
            Session["Usuario"] = cont;
            ViewBag.Usuario = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.USUA_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.USUA_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailAmbiente(MensagemViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailAmbiente(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseAmbiente");
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailAmbiente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.USUA_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
            }
            String body = str.ToString();
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            mensagem.ASSUNTO = "Devolução de Chave - Aviso";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_DESTINO = cont.USUA_NM_EMAIL;
            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;

            // Envia mensagem
            try
            {
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
            }
            catch (Exception ex)
            {
                String erro = ex.Message;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSAmbiente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO item = usuApp.GetItemById(id);
            Session["Usuario"] = item;
            ViewBag.Usuario = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.USUA_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.USUA_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSAmbiente(MensagemViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSAmbiente(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseAmbiente");
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSAmbiente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.USUA_NM_NOME;

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = assi.ASSI_NM_NOME;

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Monta token
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = cab + vm.MENS_TX_SMS + rod;

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(vm.MENS_TX_SMS);
            if (!String.IsNullOrEmpty(vm.LINK))
            {
                if (!vm.LINK.Contains("www."))
                {
                    vm.LINK = "www." + vm.LINK;
                }
                if (!vm.LINK.Contains("http://"))
                {
                    vm.LINK = "http://" + vm.LINK;
                }
                str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                texto += "  " + vm.LINK;
            }
            String body = str.ToString();
            String smsBody = body;
            String erro = null;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(cont.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }
            return 0;
        }

        public ActionResult GerarRelatorioLista()
        {
            // Prepara geração
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "AmbientesLista" + "_" + data + ".pdf";
            List<AMBIENTE> lista = (List<AMBIENTE>)Session["ListaAmbiente"];
            AMBIENTE filtro = (AMBIENTE)Session["FiltroAmbiente"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/Favicon_ERP_Condominio.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Ambientes - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 80f, 50f, 50f, 50f, 50f, 50f, 50f, 60f, 50f, 50f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Ambientes selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 10;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tipo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Lotação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Gratuito", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tem Chave", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Início", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Término", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Chaves", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Reservado", meuFont))
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

            foreach (AMBIENTE item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.AMBI_NM_AMBIENTE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TIPO_AMBIENTE.TIAM_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.AMBI_NR_LOTACAO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.AMBI_IN_GRATUITO == 1)
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
                if (item.AMBI_IN_CHAVE == 1)
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
                cell = new PdfPCell(new Paragraph(item.AMBI_HR_INICIO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.AMBI_HR_FINAL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.AMBI_IN_CHAVE == 1)
                {
                    if (item.AMBIENTE_CHAVE.Where(p => p.AMCH_DT_DEVOLUCAO == null).ToList().Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph("Entregue", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Disponível", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
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
                if (item.RESERVA.Where(p => p.AMBI_CD_ID == item.AMBI_CD_ID & p.RESE_IN_STATUS == 1).ToList().Count > 0)
                {
                    cell = new PdfPCell(new Paragraph("Reservado", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Disponível", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (System.IO.File.Exists(Server.MapPath(item.AMBI_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.AMBI_AQ_FOTO));
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
                if (filtro.TIAM_CD_ID > 0)
                {
                    parametros += "Tipo: " + filtro.TIAM_CD_ID;
                    ja = 1;
                }
                if (filtro.AMBI_NM_AMBIENTE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.AMBI_NM_AMBIENTE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.AMBI_NM_AMBIENTE;
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

            return RedirectToAction("MontarTelaAmbiente");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            AMBIENTE aten = fornApp.GetItemById((Int32)Session["IdAmbiente"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Ambiente_" + aten.AMBI_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            String foto = String.Empty;

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
            Image image = Image.GetInstance(Server.MapPath("~/Images/Favicon_ERP_Condominio.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Ambiente - Detalhes", meuFont2))
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
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (System.IO.File.Exists(Server.MapPath(aten.AMBI_AQ_FOTO)))
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 5;
                image = Image.GetInstance(Server.MapPath(aten.AMBI_AQ_FOTO));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Tipo: " + aten.TIPO_AMBIENTE.TIAM_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome: " + aten.AMBI_NM_AMBIENTE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Lotação: " + aten.AMBI_NR_LOTACAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Início: " + aten.AMBI_HR_INICIO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Término: " + aten.AMBI_HR_FINAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            if (aten.AMBI_IN_CHAVE == 1)
            {
                cell = new PdfPCell(new Paragraph("Tem chave: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Tem chave: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.AMBI_IN_GRATUITO == 1)
            {
                cell = new PdfPCell(new Paragraph("Gratuito: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Gratuito: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("Dias de Reserva: " + aten.AMBI_NR_DIAS_CHAVE.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            // Lista de Reservas
            if (aten.RESERVA.Count > 0)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 60f, 100f, 60f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Reservas", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Finalidade", meuFont))
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
                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (RESERVA item in aten.RESERVA)
                {
                    cell = new PdfPCell(new Paragraph(item.FINALIDADE_RESERVA.FIRE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.RESE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.RESE_DT_EVENTO.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.RESE_HR_INICIO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.RESE_HR_FINAL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Lista de Preços
            if (aten.AMBIENTE_CUSTO.Count > 0)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 80f, 60f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Preços", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor Base (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor Reduzido (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor Hora (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (AMBIENTE_CUSTO item in aten.AMBIENTE_CUSTO)
                {
                    cell = new PdfPCell(new Paragraph(item.AMCU_DS_DESCRICAO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.AMCU_VL_BASE), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.AMCU_VL_REDUZIDO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.AMCU_VL_HORA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }


            // Chaves
            if (aten.AMBIENTE_CHAVE.Count > 0)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f, 60f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Chaves", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 7;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Entrega", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Unidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
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
                cell = new PdfPCell(new Paragraph("Prevista", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Devolução", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);


                foreach (AMBIENTE_CHAVE item in aten.AMBIENTE_CHAVE)
                {
                    cell = new PdfPCell(new Paragraph(item.AMCH_DT_ENTREGA.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.UNIDADE.UNID_NM_EXIBE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUARIO.USUA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUARIO.USUA_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUARIO.USUA_NR_CELULAR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.AMCH_DT_PREVISTA.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.AMCH_DT_DEVOLUCAO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.AMCH_DT_DEVOLUCAO.Value.ToShortDateString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);
            }

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("VoltarAnexoAmbiente");
        }

    }
}