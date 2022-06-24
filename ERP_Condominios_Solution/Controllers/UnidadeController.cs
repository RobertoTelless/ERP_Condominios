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
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class UnidadeController : Controller
    {
        private readonly IUnidadeAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        UNIDADE objeto = new UNIDADE();
        UNIDADE objetoAntes = new UNIDADE();
        List<UNIDADE> listaMaster = new List<UNIDADE>();
        String extensao;

        public UnidadeController(IUnidadeAppService baseApps, ILogAppService logApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps; ;
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

        [HttpGet]
        public ActionResult MontarTelaUnidade()
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
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<UNIDADE>)Session["ListaUnidade"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaUnidade"] = listaMaster;
                Session["FiltroUnidade"] = null;
            }
            ViewBag.Listas = (List<UNIDADE>)Session["ListaUnidade"];
            ViewBag.Title = "Unidades";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIUN_CD_ID", "TIUN_NM_NOME");
            ViewBag.Torres = new SelectList(baseApp.GetAllTorres(idAss), "TORR_CD_ID", "TORR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            status.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Alugada = new SelectList(status, "Value", "Text");

            // Indicadores
            ViewBag.Unids = ((List<UNIDADE>)Session["ListaUnidade"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagem
            //if ((Int32)Session["MensUnidade"] == 1)
            //{
            //    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            //}
            if ((Int32)Session["MensUnidade"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 3)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0017", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 4)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 8)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 9)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0049", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 10)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0050", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 11)
            {
                String msg = ERP_Condominios_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture) + " - " + (String)Session["MensSMSError"];
                ModelState.AddModelError("", msg);
            }
            if ((Int32)Session["MensUnidade"] == 12)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensUnidade"] = 0;
            objeto = new UNIDADE();
            return View(objeto);
        }

        public ActionResult RetirarFiltroUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaUnidade"] = null;
            Session["FiltroUnidade"] = null;
            listaMaster = new List<UNIDADE>();
            return RedirectToAction("MontarTelaUnidade");
        }

        public ActionResult MostrarTudoUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaUnidade"] = listaMaster;
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpPost]
        public ActionResult FiltrarUnidade(UNIDADE item)
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

                    List<UNIDADE> listaObj = new List<UNIDADE>();
                    Int32 volta = baseApp.ExecuteFilter(item.UNID_NR_NUMERO, item.TORR_CD_ID, item.TIUN_CD_ID, item.UNID_IN_ALUGADA, idAss, out listaObj);
                    Session["FiltroUnidade"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUnidade"] = 1;
                    }

                    // Sucesso
                    Session["MensUnidade"] = 0;
                    listaMaster = listaObj;
                    Session["ListaUnidade"] = listaObj;
                    return RedirectToAction("MontarTelaUnidade");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaUnidade");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaUnidade");
            }
        }

        public ActionResult VoltarBaseUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaUnidade");
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
        public ActionResult IncluirUnidade()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade", "Unidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIUN_CD_ID", "TIUN_NM_NOME");
            ViewBag.Torres = new SelectList(baseApp.GetAllTorres(idAss), "TORR_CD_ID", "TORR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            status.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Alugada = new SelectList(status, "Value", "Text");

            // Prepara view
            UNIDADE item = new UNIDADE();
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            vm.UNID_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirUnidade(UnidadeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIUN_CD_ID", "TIUN_NM_NOME");
            ViewBag.Torres = new SelectList(baseApp.GetAllTorres(idAss), "TORR_CD_ID", "TORR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            status.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Alugada = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUnidade"] = 3;
                        return RedirectToAction("MontarTelaUnidade", "Unidade");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Unidade/" + item.UNID_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Anexos
                    if (Session["FileQueueUnidade"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueUnidade"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueUnidade(file);
                            }
                        }
                        Session["FileQueueUnidade"] = null;
                    }

                    // Sucesso
                    listaMaster = new List<UNIDADE>();
                    Session["ListaUnidade"] = null;
                    Session["VoltaUnidade"] = 1;
                    Session["IdUnidadeVolta"] = item.UNID_CD_ID;
                    Session["Unidade"] = item;
                    Session["MensUnidade"] = 0;
                    return RedirectToAction("MontarTelaUnidade");
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
        public ActionResult EditarUnidade(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade", "Unidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIUN_CD_ID", "TIUN_NM_NOME");
            ViewBag.Torres = new SelectList(baseApp.GetAllTorres(idAss), "TORR_CD_ID", "TORR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            status.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Alugada = new SelectList(status, "Value", "Text");

            if ((Int32)Session["MensUnidade"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensUnidade"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            UNIDADE item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Unidade"] = item;
            Session["IdVolta"] = id;
            Session["IdUnidade"] = id;
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarUnidade(UnidadeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIUN_CD_ID", "TIUN_NM_NOME");
            ViewBag.Torres = new SelectList(baseApp.GetAllTorres(idAss), "TORR_CD_ID", "TORR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            status.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Alugada = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<UNIDADE>();
                    Session["ListaUnidade"] = null;
                    Session["MensUnidade"] = 0;
                    return RedirectToAction("MontarTelaUnidade");
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
        public ActionResult VerUnidade(Int32 id)
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
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade", "Unidade");
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
            UNIDADE item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Unidade"] = item;
            Session["IdVolta"] = id;
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirUnidade(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade", "Unidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            UNIDADE item = baseApp.GetItemById(id);
            objetoAntes = (UNIDADE)Session["Unidade"];
            item.UNID_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensUnidade"] = 4;
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture));
                return RedirectToAction("MontarTelaUnidade");
            }
            listaMaster = new List<UNIDADE>();
            Session["ListaUnidade"] = null;
            Session["FiltroUnidade"] = null;
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult ReativarUnidade(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade", "Unidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            UNIDADE item = baseApp.GetItemById(id);
            objetoAntes = (UNIDADE)Session["Unidade"];
            item.UNID_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<UNIDADE>();
            Session["ListaUnidade"] = null;
            Session["FiltroUnidade"] = null;
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult VerAnexoUnidade(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            UNIDADE_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idUnid = (Int32)Session["IdVolta"];
            return RedirectToAction("EditarUnidade", new { id = idUnid });
        }

        [HttpPost]
        public ActionResult UploadFileUnidade(HttpPostedFileBase file)
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
                Session["MensUnidade"] = 5;
                return RedirectToAction("VoltarAnexoUnidade");
            }

            UNIDADE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensUnidade"] = 6;
                return RedirectToAction("VoltarAnexoUnidade");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Unidade/" + item.UNID_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            UNIDADE_ANEXO foto = new UNIDADE_ANEXO();
            foto.UNAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.UNAN_DT_ANEXO = DateTime.Today;
            foto.UNAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.UNAN_IN_TIPO = tipo;
            foto.UNAN_NM_TITULO = fileName;
            foto.UNID_CD_ID = item.UNID_CD_ID;

            item.UNIDADE_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoUnidade");
        }

        public FileResult DownloadUnidade(Int32 id)
        {
            UNIDADE_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.UNAN_AQ_ARQUIVO;
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
            Session["FileQueueUnidade"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueUnidade(FileQueue file)
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
                Session["MensUnidade"] = 5;
                return RedirectToAction("VoltarAnexoUnidade");
            }

            UNIDADE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensUnidade"] = 6;
                return RedirectToAction("VoltarAnexoUnidade");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Unidade/" + item.UNID_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            UNIDADE_ANEXO foto = new UNIDADE_ANEXO();
            foto.UNAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.UNAN_DT_ANEXO = DateTime.Today;
            foto.UNAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.UNAN_IN_TIPO = tipo;
            foto.UNAN_NM_TITULO = fileName;
            foto.UNID_CD_ID = item.UNID_CD_ID;

            item.UNIDADE_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoUnidade");
        }

        [HttpGet]
        public ActionResult MontarTelaMorador()
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
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade", "Unidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 idUnidade = (Int32)Session["IdUnidade"];
            UNIDADE unidade = (UNIDADE)Session["Unidade"];

            // Prepara view
            List<USUARIO> lista = unidade.USUARIO.ToList();
            Session["ListaUsuario"] = lista;
            return RedirectToAction("MontarTelaUsuario", "Usuario");
        }

        [HttpGet]
        public ActionResult GerarNotificacaoUnidade()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensUnidade"] = 2;
                    return RedirectToAction("MontarTelaUnidade", "Unidade");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 unidade = (Int32)Session["IdUnidade"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss).Where(p => p.UNID_CD_ID == unidade).ToList();

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
            vm.NOTI_NM_TITULO = "Notificação para Morador";
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoUnidade(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 unidade = (Int32)Session["IdUnidade"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss).Where(p => p.UNID_CD_ID == unidade).ToList();
            ViewBag.Cats = new SelectList(baseApp.GetAllCatNotificacao(idAss), "CANO_CD_ID", "CANO_NM_NOME");
            ViewBag.Usuarios = new SelectList(lista, "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTIFICACAO item = Mapper.Map<NotificacaoViewModel, NOTIFICACAO>(vm);
                    Int32 volta = baseApp.GerarNotificacao(item, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<UNIDADE>();
                    return RedirectToAction("VoltarBaseUnidade");
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

        public ActionResult EnviarSmsUnidade(Int32 id, String mensagem)
        {
            try
            {
                UNIDADE unid = baseApp.GetById(id);
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica existencia prévia
                if (unid == null)
                {
                    Session["MensUnidade"] = 8;
                    return RedirectToAction("MontarTelaUnidade");
                }

                //Recupera responsavel
                USUARIO resp = unid.USUARIO.Where(p => p.USUA_IN_RESPONSAVEL == 1).FirstOrDefault();
                if (resp == null)
                {
                    Session["MensUnidade"] = 9;
                    return RedirectToAction("MontarTelaUnidade");
                }

                // Criticas
                if (resp.USUA_NR_CELULAR == null)
                {
                    Session["MensUnidade"] = 10;
                    return RedirectToAction("MontarTelaUnidade");
                }

                // Processa SMS
                CONFIGURACAO conf = confApp.GetItemById(idAss);

                // Monta token
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = mensagem;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(mensagem);
                String body = str.ToString();
                String smsBody = body;
                String erro = null;

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                try
                {
                    String listaDest = "55" + Regex.Replace(resp.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String customId = Cryptography.GenerateRandomPassword(8);
                    String data = String.Empty;
                    String json = String.Empty;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPCondominio\"}]}");
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
                return RedirectToAction("MontarTelaUnidade");
            }
            catch (Exception ex)
            {
                Session["MensUnidade"] = 11;
                Session["MensSMSErro"] = ex.Message;
                return RedirectToAction("MontarTelaUnidade");
            }
        }

        [HttpGet]
        public ActionResult EnviarSMSUnidadeForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            UNIDADE item = (UNIDADE)Session["Unidade"];

            //Recupera responsavel
            USUARIO resp = item.USUARIO.Where(p => p.USUA_IN_RESPONSAVEL == 1).FirstOrDefault();
            if (resp == null)
            {
                Session["MensUnidade"] = 9;
                return RedirectToAction("VoltarAnexoUnidade");
            }

            // Criticas
            if (resp.USUA_NR_CELULAR == null)
            {
                Session["MensUnidade"] = 10;
                return RedirectToAction("VoltarAnexoUnidade");
            }

            Session["Unidade"] = item;
            Session["Responsavel"] = resp;
            ViewBag.Unidade = item;
            ViewBag.Responsavel = resp;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.UNID_NM_EXIBE;
            mens.ID = item.UNID_CD_ID;
            mens.MODELO = resp.USUA_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSUnidadeForm(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioSMSUnidade(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarAnexoUnidade");
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
        public Int32 ProcessaEnvioSMSUnidade(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Responsavel"];

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Monta token
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = vm.MENS_TX_SMS;

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

        [HttpGet]
        public ActionResult EnviarEMailUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            UNIDADE item = (UNIDADE)Session["Unidade"];

            //Recupera responsavel
            USUARIO resp = item.USUARIO.Where(p => p.USUA_IN_RESPONSAVEL == 1).FirstOrDefault();
            if (resp == null)
            {
                Session["MensUnidade"] = 9;
                return RedirectToAction("VoltarAnexoUnidade");
            }

            // Criticas
            if (resp.USUA_NR_CELULAR == null)
            {
                Session["MensUnidade"] = 10;
                return RedirectToAction("VoltarAnexoUnidade");
            }

            Session["Unidade"] = item;
            Session["Responsavel"] = resp;
            ViewBag.Unidade = item;
            ViewBag.Responsavel = resp;

            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = resp.USUA_NM_NOME;
            mens.ID = item.UNID_CD_ID;
            mens.MODELO = resp.USUA_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailUnidade(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioEMailUnidade(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarAnexoUnidade");
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
        public Int32 ProcessaEnvioEMailUnidade(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contato
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Responsavel"];

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
            mensagem.ASSUNTO = "Contato";
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

    }
}