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
    public class AmbienteController : Controller
    {
        private readonly IAmbienteAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        AMBIENTE objetoForn = new AMBIENTE();
        AMBIENTE objetoFornAntes = new AMBIENTE();
        List<AMBIENTE> listaMasterForn = new List<AMBIENTE>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public AmbienteController(IAmbienteAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps)
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
            if (((List<AMBIENTE>)Session["ListaAmbiente"]).Count == 0)
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
                if ((Int32)Session["MensAmbiente"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
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
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    return RedirectToAction("MontarTelaAmbiente");
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
            return RedirectToAction("MontarTelaAmbiente");
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

            AMBIENTE item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Ambiente"] = item;
            Session["IdVolta"] = id;
            Session["IdAmbiente"] = id;
            AmbienteViewModel vm = Mapper.Map<AMBIENTE, AmbienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    Int32 volta = fornApp.ValidateEditAmbienteChave(item);

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
        public ActionResult ExcluirAmbienteChave(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            AMBIENTE_CHAVE item = fornApp.GetAmbienteChaveById(id);
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            item.AMCH_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateEditAmbienteChave(item);
            return RedirectToAction("VoltarAnexoAmbiente");
        }

        [HttpGet]
        public ActionResult ReativarAmbienteChave(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            AMBIENTE_CHAVE item = fornApp.GetAmbienteChaveById(id);
            objetoFornAntes = (AMBIENTE)Session["Ambiente"];
            item.AMCH_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateEditAmbienteChave(item);
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
            vm.AMCH_DT_ENTREGA = DateTime.Today.Date;
            vm.AMCH_DT_PREVISTA = DateTime.Today.Date.AddDays(5);
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
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    AMBIENTE_CHAVE item = Mapper.Map<AmbienteChaveViewModel, AMBIENTE_CHAVE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreateAmbienteChave(item);

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
    }
}