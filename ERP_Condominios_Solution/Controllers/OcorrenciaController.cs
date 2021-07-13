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
    public class OcorrenciaController : Controller
    {
        private readonly IOcorrenciaAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private String msg;
        private Exception exception;
        OCORRENCIA objetoForn = new OCORRENCIA();
        OCORRENCIA objetoFornAntes = new OCORRENCIA();
        List<OCORRENCIA> listaMasterForn = new List<OCORRENCIA>();
        String extensao;

        public OcorrenciaController(IOcorrenciaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps)
        {
            fornApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
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
        public ActionResult MontarTelaOcorrencia()
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
                    Session["MensOcorrencia"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaOcorrencia"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaOcorrencia"] = listaMasterForn;
            }
            if (((List<OCORRENCIA>)Session["ListaOcorrencia"]).Count == 0)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaOcorrencia"] = listaMasterForn;
            }
            ViewBag.Listas = (List<OCORRENCIA>)Session["ListaOcorrencia"];
            ViewBag.Title = "Ocorrencias";
            ViewBag.Cats = new SelectList(fornApp.GetAllCategorias(idAss).OrderBy(x => x.CAOC_NM_NOME), "CAOC_CD_ID", "CAOC_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            Session["IncluirOco"] = 0;

            // Indicadores
            ViewBag.Ocorrencias = ((List<OCORRENCIA>)Session["ListaOcorrencia"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensOcorrencia"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensOcorrencia"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensOcorrencia"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new OCORRENCIA();
            objetoForn.OCOR_IN_ATIVO = 1;
            Session["MensOcorrencia"] = 0;
            Session["VoltaOcorrencia"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroOcorrencia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaOcorrencia"] = null;
            Session["FiltroOcorrencia"] = null;
            return RedirectToAction("MontarTelaOcorrencia");
        }

        public ActionResult MostrarTudoOcorrencia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroOcorrencia"] = null;
            Session["ListaOcorrencia"] = listaMasterForn;
            return RedirectToAction("MontarTelaOcorrencia");
        }

        [HttpPost]
        public ActionResult FiltrarOcorrencia(OCORRENCIA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<OCORRENCIA> listaObj = new List<OCORRENCIA>();
                Session["FiltroOcorrencia"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.UNID_CD_ID, item.USUA_CD_ID, item.CAOC_CD_ID, item.OCOR_NM_TITULO, item.OCOR_DT_OCORRENCIA, item.OCOR_TX_TEXTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensOcorrencia"] = 1;
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    return RedirectToAction("MontarTelaOcorrencia");
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaOcorrencia"] = listaObj;
                return RedirectToAction("MontarTelaOcorrencia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaOcorrencia");
            }
        }

        public ActionResult VoltarBaseOcorrencia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaOcorrencia");
        }

        [HttpGet]
        public ActionResult IncluirOcorrencia()
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
                    Session["MensOcorrencia"] = 2;
                    return RedirectToAction("MontarTelaOcorrencia", "Ambiente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(fornApp.GetAllCategorias(idAss).OrderBy(x => x.CAOC_NM_NOME), "CAOC_CD_ID", "CAOC_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            Session["VoltaProp"] = 4;

            // Prepara view
            OCORRENCIA item = new OCORRENCIA();
            OcorrenciaViewModel vm = Mapper.Map<OCORRENCIA, OcorrenciaViewModel>(item);
            vm.OCOR_DT_CADASTRO = DateTime.Today.Date;
            vm.OCOR_IN_ATIVO = 1;
            vm.OCOR_DT_OCORRENCIA = DateTime.Today.Date;
            vm.OCOR_IN_STATUS = 1;
            vm.UNID_CD_ID = usuario.UNID_CD_ID;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirOcorrencia(OcorrenciaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(fornApp.GetAllCategorias(idAss).OrderBy(x => x.CAOC_NM_NOME), "CAOC_CD_ID", "CAOC_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    OCORRENCIA item = Mapper.Map<OcorrenciaViewModel, OCORRENCIA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno


                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ocorrencias/" + item.OCOR_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterForn = new List<OCORRENCIA>();
                    Session["ListaOcorrencia"] = null;
                    Session["IncluirOco"] = 1;
                    Session["Ocorrencias"] = fornApp.GetAllItens(idAss);
                    Session["IdVolta"] = item.OCOR_CD_ID;
                    if (Session["FileQueueOcorrencia"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueOcorrencia"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueOcorrencia(file);
                            }
                            else
                            {
                                //UploadFotoQueueOcorrencia(file);
                            }
                        }

                        Session["FileQueueOcorrencia"] = null;
                    }
                    if ((Int32)Session["VoltaOcorrencia"] == 2)
                    {
                        return RedirectToAction("IncluirOcorrencia");
                    }
                    return RedirectToAction("MontarTelaOcorrencia");
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
        public ActionResult EditarOcorrencia(Int32 id)
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
                    Session["MensOcorrencia"] = 2;
                    return RedirectToAction("MontarTelaOcorrencia", "Ocorrencia");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Cats = new SelectList(fornApp.GetAllCategorias(idAss).OrderBy(x => x.CAOC_NM_NOME), "CAOC_CD_ID", "CAOC_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Incluir = (Int32)Session["IncluirOco"];

            if ((Int32)Session["MensOcorrencia"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensOcorrencia"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            OCORRENCIA item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Ocorrencia"] = item;
            Session["IdVolta"] = id;
            Session["IdOcorrencia"] = id;
            OcorrenciaViewModel vm = Mapper.Map<OCORRENCIA, OcorrenciaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarOcorrencia(OcorrenciaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(fornApp.GetAllCategorias(idAss).OrderBy(x => x.CAOC_NM_NOME), "CAOC_CD_ID", "CAOC_NM_NOME");
            ViewBag.Unids = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Usus = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    OCORRENCIA item = Mapper.Map<OcorrenciaViewModel, OCORRENCIA>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<OCORRENCIA>();
                    Session["ListaOcorrencia"] = null;
                    Session["IncluirOco"] = 0;
                    return RedirectToAction("MontarTelaOcorrencia");
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
        public ActionResult VerOcorrencia(Int32 id)
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
                    Session["MensOcorrencia"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirOco"];

            OCORRENCIA item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Ocorrencia"] = item;
            Session["IdVolta"] = id;
            Session["IdOcorrencia"] = id;
            OcorrenciaViewModel vm = Mapper.Map<OCORRENCIA, OcorrenciaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirOcorrencia(Int32 id)
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
                    Session["MensOcorrencia"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            OCORRENCIA item = fornApp.GetItemById(id);
            OcorrenciaViewModel vm = Mapper.Map<OCORRENCIA, OcorrenciaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirOcorrencia(OcorrenciaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                OCORRENCIA item = Mapper.Map<OcorrenciaViewModel, OCORRENCIA>(vm);
                Int32 volta = fornApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<OCORRENCIA>();
                Session["ListaOcorrencia"] = null;
                return RedirectToAction("MontarTelaOcorrencia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult ReativarOcorrencia(Int32 id)
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
                    Session["MensOcorrencia"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            OCORRENCIA item = fornApp.GetItemById(id);
            OcorrenciaViewModel vm = Mapper.Map<OCORRENCIA, OcorrenciaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarOcorrencia(OcorrenciaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                OCORRENCIA item = Mapper.Map<OcorrenciaViewModel, OCORRENCIA>(vm);
                Int32 volta = fornApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<OCORRENCIA>();
                Session["ListaOcorrencia"] = null;
                return RedirectToAction("MontarTelaOcorrencia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoOcorrencia(Int32 id)
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
                    Session["MensOcorrencia"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            OCORRENCIA_ANEXO item = fornApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoOcorrencia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarOcorrencia", new { id = (Int32)Session["IdOcorrencia"] });
        }

        public FileResult DownloadOcorrencia(Int32 id)
        {
            OCORRENCIA_ANEXO item = fornApp.GetAnexoById(id);
            String arquivo = item.OCAN_AQ_ARQUIVO;
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

            Session["FileQueueOcorrencia"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueOcorrencia(FileQueue file)
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
                Session["MensOcorrencia"] = 5;
                return RedirectToAction("VoltarAnexoOcorrencia");
            }

            OCORRENCIA item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensOcorrencia"] = 6;
                return RedirectToAction("VoltarAnexoOcorrencia");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ocorrencias/" + item.OCOR_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            OCORRENCIA_ANEXO foto = new OCORRENCIA_ANEXO();
            foto.OCAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.OCAN_DT_ANEXO = DateTime.Today;
            foto.OCAN_IN_ATIVO = 1;
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
            foto.OCAN_IN_TIPO = tipo;
            foto.OCAN_NM_TITULO = fileName;
            foto.OCOR_CD_ID = item.OCOR_CD_ID;

            item.OCORRENCIA_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoOcorrencia");
        }

        [HttpPost]
        public ActionResult UploadFileOcorrencia(HttpPostedFileBase file)
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
                Session["MensOcorrencia"] = 5;
                return RedirectToAction("VoltarAnexoOcorrencia");
            }

            OCORRENCIA item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensOcorrencia"] = 6;
                return RedirectToAction("VoltarAnexoOcorrencia");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Ocorrencias/" + item.OCOR_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            OCORRENCIA_ANEXO foto = new OCORRENCIA_ANEXO();
            foto.OCAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.OCAN_DT_ANEXO = DateTime.Today;
            foto.OCAN_IN_ATIVO = 1;
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
            foto.OCAN_IN_TIPO = tipo;
            foto.OCAN_NM_TITULO = fileName;
            foto.OCOR_CD_ID = item.OCOR_CD_ID;

            item.OCORRENCIA_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoOcorrencia");
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