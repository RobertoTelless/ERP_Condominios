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
    public class ListaConvidadoController : Controller
    {
        private readonly IListaConvidadoAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        LISTA_CONVIDADO objetoForn = new LISTA_CONVIDADO();
        LISTA_CONVIDADO objetoFornAntes = new LISTA_CONVIDADO();
        List<LISTA_CONVIDADO> listaMasterForn = new List<LISTA_CONVIDADO>();
        String extensao;

        public ListaConvidadoController(IListaConvidadoAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps)
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
        public ActionResult MontarTelaLista()
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
                    Session["MensLista"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaLista"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaLista"] = listaMasterForn;
            }
            if (((List<FORNECEDOR>)Session["ListaLista"]).Count == 0)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaLista"] = listaMasterForn;
            }
            ViewBag.Listas = (List<LISTA_CONVIDADO>)Session["ListaLista"];
            ViewBag.Title = "Lista de Convidados";
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NR_NUMERO), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Reservas = new SelectList(fornApp.GetAllReservas(idAss).OrderBy(x => x.RESE_NM_NOME), "RESE_CD_ID", "RESE_NM_NOME");
            Session["IncluirLista"] = 0;

            // Indicadores
            ViewBag.Listas = ((List<LISTA_CONVIDADO>)Session["ListaLista"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensLista"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensLista"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensLista"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensLista"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensLista"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new LISTA_CONVIDADO();
            objetoForn.LICO_IN_ATIVO = 1;
            Session["MensLista"] = 0;
            Session["VoltaLista"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaLista"] = null;
            Session["FiltroLista"] = null;
            return RedirectToAction("MontarTelaLista");
        }

        public ActionResult MostrarTudoLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroLista"] = null;
            Session["ListaLista"] = listaMasterForn;
            return RedirectToAction("MontarTelaLista");
        }

        [HttpPost]
        public ActionResult FiltrarLista(LISTA_CONVIDADO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LISTA_CONVIDADO> listaObj = new List<LISTA_CONVIDADO>();
                Session["FiltroLista"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.LICO_NM_LISTA, item.LICO_DT_EVENTO, item.UNID_CD_ID, item.RESE_CD_ID, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensLista"] = 1;
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    return RedirectToAction("MontarTelaLista");
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaLista"] = listaObj;
                return RedirectToAction("MontarTelaLista");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaLista");
            }
        }

        public ActionResult VoltarBaseLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaLista");
        }

        [HttpGet]
        public ActionResult IncluirLista()
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
                    Session["MensLista"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Reservas = new SelectList(fornApp.GetAllReservas(idAss).OrderBy(x => x.RESE_NM_NOME), "RESE_CD_ID", "RESE_NM_NOME");
            Session["VoltaProp"] = 4;

            // Prepara view
            LISTA_CONVIDADO item = new LISTA_CONVIDADO();
            ListaConvidadoViewModel vm = Mapper.Map<LISTA_CONVIDADO, ListaConvidadoViewModel>(item);
            vm.LICO_DT_CADASTRO = DateTime.Today.Date;
            vm.LICO_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirLista(ListaConvidadoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Reservas = new SelectList(fornApp.GetAllReservas(idAss).OrderBy(x => x.RESE_NM_NOME), "RESE_CD_ID", "RESE_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    LISTA_CONVIDADO item = Mapper.Map<ListaConvidadoViewModel, LISTA_CONVIDADO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensLista"] = 3;
                        return RedirectToAction("MontarTelaLista", "ListaConvidado");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/ListaConvidado/" + item.LICO_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterForn = new List<LISTA_CONVIDADO>();
                    Session["ListaLista"] = null;
                    Session["IncluirLista"] = 1;
                    Session["Listas"] = fornApp.GetAllItens(idAss);

                    Session["IdVolta"] = item.LICO_CD_ID;
                    if (Session["FileQueueLista"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueLista"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueLista(file);
                            }
                        }

                        Session["FileQueueLista"] = null;
                    }

                    if ((Int32)Session["VoltaLista"] == 2)
                    {
                        return RedirectToAction("IncluirLista");
                    }
                    return RedirectToAction("MontarTelaLista");
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
        public ActionResult EditarLista(Int32 id)
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
                    Session["MensLista"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Reservas = new SelectList(fornApp.GetAllReservas(idAss).OrderBy(x => x.RESE_NM_NOME), "RESE_CD_ID", "RESE_NM_NOME");
            ViewBag.Incluir = (Int32)Session["IncluirLista"];

            if ((Int32)Session["MensLista"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensLista"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            LISTA_CONVIDADO item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Lista"] = item;
            Session["IdVolta"] = id;
            Session["IdLista"] = id;
            ListaConvidadoViewModel vm = Mapper.Map<LISTA_CONVIDADO, ListaConvidadoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarLista(ListaConvidadoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Unidades = new SelectList(fornApp.GetAllUnidades(idAss).OrderBy(x => x.UNID_NM_EXIBE), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Reservas = new SelectList(fornApp.GetAllReservas(idAss).OrderBy(x => x.RESE_NM_NOME), "RESE_CD_ID", "RESE_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    LISTA_CONVIDADO item = Mapper.Map<ListaConvidadoViewModel, LISTA_CONVIDADO>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<LISTA_CONVIDADO>();
                    Session["ListaLista"] = null;
                    Session["IncluirLista"] = 0;
                    return RedirectToAction("MontarTelaLista");
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
        public ActionResult VerLista(Int32 id)
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
                    Session["MensLista"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirLista"];

            LISTA_CONVIDADO item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Lista"] = item;
            Session["IdVolta"] = id;
            Session["IdLista"] = id;
            ListaConvidadoViewModel vm = Mapper.Map<LISTA_CONVIDADO, ListaConvidadoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirLista(Int32 id)
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
                    Session["MensLista"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            LISTA_CONVIDADO item = fornApp.GetItemById(id);
            ListaConvidadoViewModel vm = Mapper.Map<LISTA_CONVIDADO, ListaConvidadoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirLista(ListaConvidadoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LISTA_CONVIDADO item = Mapper.Map<ListaConvidadoViewModel, LISTA_CONVIDADO>(vm);
                Int32 volta = fornApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensLista"] = 4;
                    return RedirectToAction("MontarTelaLista", "ListaConvidado");
                }

                // Sucesso
                listaMasterForn = new List<LISTA_CONVIDADO>();
                Session["ListaLista"] = null;
                return RedirectToAction("MontarTelaLista");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult ReativarLista(Int32 id)
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
                    Session["MensLista"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            LISTA_CONVIDADO item = fornApp.GetItemById(id);
            ListaConvidadoViewModel vm = Mapper.Map<LISTA_CONVIDADO, ListaConvidadoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarLista(ListaConvidadoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LISTA_CONVIDADO item = Mapper.Map<ListaConvidadoViewModel, LISTA_CONVIDADO>(vm);
                Int32 volta = fornApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterForn = new List<LISTA_CONVIDADO>();
                Session["ListaLista"] = null;
                return RedirectToAction("MontarTelaLista");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objetoForn);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoLista(Int32 id)
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
                    Session["MensLista"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            LISTA_CONVIDADO_ANEXO item = fornApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarLista", new { id = (Int32)Session["IdLista"] });
        }

        public FileResult DownloadLista(Int32 id)
        {
            LISTA_CONVIDADO_ANEXO item = fornApp.GetAnexoById(id);
            String arquivo = item.LCAN_AQ_ARQUIVO;
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
            Session["FileQueueLista"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueLista(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdLista"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensLista"] = 5;
                return RedirectToAction("VoltarAnexoLista");
            }

            LISTA_CONVIDADO item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensLista"] = 6;
                return RedirectToAction("VoltarAnexoLista");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/ListaConvidado/" + item.LICO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            LISTA_CONVIDADO_ANEXO foto = new LISTA_CONVIDADO_ANEXO();
            foto.LCAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.LCAN_DT_ANEXO = DateTime.Today;
            foto.LCAN_IN_ATIVO = 1;
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
            foto.LCAN_IN_TIPO = tipo;
            foto.LCAN_NM_TITULO = fileName;
            foto.LICO_CD_ID = item.LICO_CD_ID;

            item.LISTA_CONVIDADO_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoLista");
        }

        [HttpPost]
        public ActionResult UploadFileLista(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdLista"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensLista"] = 5;
                return RedirectToAction("VoltarAnexoLista");
            }

            LISTA_CONVIDADO item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensLista"] = 6;
                return RedirectToAction("VoltarAnexoLista");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/ListaConvidado/" + item.LICO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            LISTA_CONVIDADO_ANEXO foto = new LISTA_CONVIDADO_ANEXO();
            foto.LCAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.LCAN_DT_ANEXO = DateTime.Today;
            foto.LCAN_IN_ATIVO = 1;
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
            foto.LCAN_IN_TIPO = tipo;
            foto.LCAN_NM_TITULO = fileName;
            foto.LICO_CD_ID = item.LICO_CD_ID;

            item.LISTA_CONVIDADO_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoLista");
        }

        [HttpGet]
        public ActionResult EditarConvidado(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CONVIDADO item = fornApp.GetConvidadoById(id);
            objetoFornAntes = (LISTA_CONVIDADO)Session["Lista"];
            ConvidadoViewModel vm = Mapper.Map<CONVIDADO, ConvidadoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarConvidado(ConvidadoViewModel vm)
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
                    CONVIDADO item = Mapper.Map<ConvidadoViewModel, CONVIDADO>(vm);
                    Int32 volta = fornApp.ValidateEditConvidado(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoLista");
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
        public ActionResult ExcluirConvidado(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CONVIDADO item = fornApp.GetConvidadoById(id);
            objetoFornAntes = (LISTA_CONVIDADO)Session["Lista"];
            item.CONV_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateEditConvidado(item);
            return RedirectToAction("VoltarAnexoLista");
        }

        [HttpGet]
        public ActionResult ReativarConvidado(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CONVIDADO item = fornApp.GetConvidadoById(id);
            objetoFornAntes = (LISTA_CONVIDADO)Session["Lista"];
            item.CONV_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateEditConvidado(item);
            return RedirectToAction("VoltarAnexoLista");
        }

        [HttpGet]
        public ActionResult IncluirConvidado()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONVIDADO item = new CONVIDADO();
            ConvidadoViewModel vm = Mapper.Map<CONVIDADO, ConvidadoViewModel>(item);
            vm.LICO_CD_ID = (Int32)Session["IdLista"];
            vm.CONV_IN_ATIVO = 1;
            vm.CONV_DT_CADASTRO = DateTime.Today.Date;
            vm.CONV_IN_CHEGOU = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirConvidado(ConvidadoViewModel vm)
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
                    CONVIDADO item = Mapper.Map<ConvidadoViewModel, CONVIDADO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreateConvidado(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoLista");
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