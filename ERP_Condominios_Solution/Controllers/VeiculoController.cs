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
    public class VeiculoController : Controller
    {
        private readonly IVeiculoAppService baseApp;
        private readonly ILogAppService logApp;

        private String msg;
        private Exception exception;
        VEICULO objeto = new VEICULO();
        VEICULO objetoAntes = new VEICULO();
        List<VEICULO> listaMaster = new List<VEICULO>();
        String extensao;

        public VeiculoController(IVeiculoAppService baseApps, ILogAppService logApps)
        {
            baseApp = baseApps; ;
            logApp = logApps;
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
        public ActionResult MontarTelaVeiculo()
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
                    Session["MensVeiculo"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<VEICULO>)Session["ListaVeiculo"] == null)
            {
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    listaMaster = baseApp.GetByUnidade(usuario.UNID_CD_ID.Value);
                }
                else if (usuario.PERFIL.PERF_SG_SIGLA == "SIN" || usuario.PERFIL.PERF_SG_SIGLA == "POR")
                {
                    listaMaster = baseApp.GetAllItens(idAss);
                }
                Session["ListaVeiculo"] = listaMaster;
                Session["FiltroVeiculo"] = null;
            }
            ViewBag.Listas = (List<VEICULO>)Session["ListaVeiculo"];
            ViewBag.Title = "Veiculos";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Vagas = new SelectList(baseApp.GetAllVagas(idAss), "VAGA_CD_ID", "VAGA_NM_NOME");

            // Indicadores
            ViewBag.Veics = ((List<VEICULO>)Session["ListaVeiculo"]).Count;

            // Mensagem
            if ((Int32)Session["MensVeiculo"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensVeiculo"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensVeiculo"] == 3)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0023", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensVeiculo"] == 4)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0025", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["MensVeiculo"] = 0;
            Session["VoltaVeiculo"] = 1;
            objeto = new VEICULO();
            return View(objeto);
        }

        public ActionResult RetirarFiltroVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaVeiculo"] = null;
            Session["FiltroVeiculo"] = null;
            listaMaster = new List<VEICULO>();
            return RedirectToAction("MontarTelaVeiculo");
        }

        public ActionResult MostrarTudoVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaVeiculo"] = listaMaster;
            return RedirectToAction("MontarTelaVeiculo");
        }

        [HttpPost]
        public ActionResult FiltrarVeiculo(VEICULO item)
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

                    List<VEICULO> listaObj = new List<VEICULO>();
                    Int32 volta = baseApp.ExecuteFilter(item.VEIC_NM_PLACA, item.VEIC_NM_MARCA, item.UNID_CD_ID, item.TIVE_CD_ID, item.VAGA_CD_ID, idAss, out listaObj);
                    Session["FiltroVeiculo"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensVeiculo"] = 1;
                        ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        return RedirectToAction("MontarTelaVeiculo");
                    }

                    // Sucesso
                    Session["MensVeiculo"] = 0;
                    listaMaster = listaObj;
                    Session["ListaVeiculo"] = listaObj;
                    return RedirectToAction("MontarTelaVeiculo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaVeiculo");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaVeiculo");
            }
        }

        public ActionResult VoltarBaseVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaVeiculo");
        }

        [HttpGet]
        public ActionResult IncluirVeiculo()
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
                    Session["MensVeiculo"] = 2;
                    return RedirectToAction("MontarTelaVeiculo", "Veiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");

            // Prepara view
            VEICULO item = new VEICULO();
            VeiculoViewModel vm = Mapper.Map<VEICULO, VeiculoViewModel>(item);
            vm.VEIC_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.VEIC_DT_CADASTRO = DateTime.Today.Date;
            vm.VEIC_IN_CONFIRMA_VAGA = 0;
            if (usuario.PERFIL.PERF_SG_SIGLA == "MOR")
            {
                vm.UNID_CD_ID = usuario.UNID_CD_ID.Value;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirVeiculo(VeiculoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    VEICULO item = Mapper.Map<VeiculoViewModel, VEICULO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensVeiculo"] = 3;
                        return RedirectToAction("MontarTelaVeiculo", "Veiculo");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Veiculo/" + item.VEIC_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Anexos
                    if (Session["FileQueueVeiculo"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueVeiculo"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueVeiculo(file);
                            }
                        }
                        Session["FileQueueVeiculo"] = null;
                    }

                    // Sucesso
                    listaMaster = new List<VEICULO>();
                    Session["ListaVeiculo"] = null;
                    Session["VoltaVeiculo"] = 1;
                    Session["IdVeiculoVolta"] = item.UNID_CD_ID;
                    Session["Veiculo"] = item;
                    Session["MensVeiculo"] = 0;
                    return RedirectToAction("MontarTelaVeiculo");
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
        public ActionResult EditarVeiculo(Int32 id)
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
                    Session["MensVeiculo"] = 2;
                    return RedirectToAction("MontarTelaVeiculo", "Veiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");

            if ((Int32)Session["MensVeiculo"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensVeiculo"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            VEICULO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Veiculo"] = item;
            Session["IdVolta"] = id;
            Session["IdVeiculo"] = id;
            VeiculoViewModel vm = Mapper.Map<VEICULO, VeiculoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarVeiculo(VeiculoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    VEICULO item = Mapper.Map<VeiculoViewModel, VEICULO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<VEICULO>();
                    Session["ListaVeiculo"] = null;
                    Session["MensVeiculo"] = 0;
                    return RedirectToAction("MontarTelaVeiculo");
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
        public ActionResult VerVeiculo(Int32 id)
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
                    Session["MensVeiculo"] = 2;
                    return RedirectToAction("MontarTelaVeiculo", "Veiculo");
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
            VEICULO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Veiculo"] = item;
            Session["IdVolta"] = id;
            Session["IdVeiculo"] = id;
            VeiculoViewModel vm = Mapper.Map<VEICULO, VeiculoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirVeiculo(Int32 id)
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
                    Session["MensVeiculo"] = 2;
                    return RedirectToAction("MontarTelaVeiculo", "Veiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            VEICULO item = baseApp.GetItemById(id);
            objetoAntes = (VEICULO)Session["Veiculo"];
            item.VEIC_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensVeiculo"] = 4;
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0018", CultureInfo.CurrentCulture));
                return RedirectToAction("MontarTelaVeiculo");
            }
            listaMaster = new List<VEICULO>();
            Session["ListaVeiculo"] = null;
            Session["FiltroVeiculo"] = null;
            return RedirectToAction("MontarTelaVeiculo");
        }

        [HttpGet]
        public ActionResult ReativarVeiculo(Int32 id)
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
                    Session["MensVeiculo"] = 2;
                    return RedirectToAction("MontarTelaVeiculo", "Veiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            VEICULO item = baseApp.GetItemById(id);
            objetoAntes = (VEICULO)Session["Veiculo"];
            item.VEIC_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<VEICULO>();
            Session["ListaVeiculo"] = null;
            Session["FiltroVeiculo"] = null;
            return RedirectToAction("MontarTelaVeiculo");
        }

        [HttpGet]
        public ActionResult VerAnexoVeiculo(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            VEICULO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idVeic = (Int32)Session["IdVeiculo"];
            return RedirectToAction("EditarVeiculo", new { id = idVeic });
        }

        [HttpPost]
        public ActionResult UploadFileVeiculo(HttpPostedFileBase file)
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
                Session["MensVeiculo"] = 5;
                return RedirectToAction("VoltarAnexoVeiculo");
            }

            VEICULO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensVeiculo"] = 6;
                return RedirectToAction("VoltarAnexoVeiculo");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Veiculo/" + item.VEIC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            VEICULO_ANEXO foto = new VEICULO_ANEXO();
            foto.VEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.VEAN_DT_ANEXO = DateTime.Today;
            foto.VEAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.VEAN_IN_TIPO = tipo;
            foto.VEAN_NM_TITULO = fileName;
            foto.VEIC_CD_ID = item.VEIC_CD_ID;

            item.VEICULO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoVeiculo");
        }

        public FileResult DownloadVeiculo(Int32 id)
        {
            VEICULO_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.VEAN_AQ_ARQUIVO;
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
            Session["FileQueueVeiculo"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueVeiculo(FileQueue file)
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
                Session["MensVeiculo"] = 5;
                return RedirectToAction("VoltarAnexoVeiculo");
            }

            VEICULO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensVeiculo"] = 6;
                return RedirectToAction("VoltarAnexoVeiculo");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Veiculo/" + item.VEIC_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            VEICULO_ANEXO foto = new VEICULO_ANEXO();
            foto.VEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.VEAN_DT_ANEXO = DateTime.Today;
            foto.VEAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.VEAN_IN_TIPO = tipo;
            foto.VEAN_NM_TITULO = fileName;
            foto.VEIC_CD_ID = item.VEIC_CD_ID;

            item.VEICULO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoVeiculo");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueVeiculo(FileQueue file)
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
                Session["MensVeiculo"] = 5;
                return RedirectToAction("VoltarAnexoVeiculo");
            }

            VEICULO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensVeiculo"] = 6;
                return RedirectToAction("VoltarAnexoVeiculo");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Veiculo/" + item.VEIC_CD_ID.ToString() + "/Anexos/";
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
                item.VEIC_AQ_FOTO = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            else
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensVeiculo"] = 6;
                return RedirectToAction("VoltarAnexoVeiculo");
            }
            return RedirectToAction("VoltarAnexoVeiculo");
        }

        [HttpPost]
        public ActionResult UploadFotoVeiculo(HttpPostedFileBase file)
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
                Session["MensVeiculo"] = 5;
                return RedirectToAction("VoltarAnexoVeiculo");
            }

            VEICULO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensVeiculo"] = 6;
                return RedirectToAction("VoltarAnexoVeiculo");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Veiculo/" + item.VEIC_CD_ID.ToString() + "/Anexos/";
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
                item.VEIC_AQ_FOTO = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            else
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensVeiculo"] = 6;
                return RedirectToAction("VoltarAnexoVeiculo");
            }
            return RedirectToAction("VoltarAnexoVeiculo");
        }

        [HttpGet]
        public ActionResult GerarNotificacaoVeiculo()
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
                    Session["MensVeiculo"] = 2;
                    return RedirectToAction("MontarTelaVeiculo", "Veiculo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 unidade = (Int32)Session["IdUnidade"];
            List<USUARIO> lista = baseApp.GetAllUsuarios(idAss);

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
            vm.NOTI_NM_TITULO = "Notificação para Morador - Veículo";
            return View(vm);
        }

        [HttpPost]
        public ActionResult GerarNotificacaoVeiculo(NotificacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            VEICULO veiculo = (VEICULO)Session["Veiculo"];
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
                    Int32 volta = baseApp.GerarNotificacao(item, usuarioLogado, veiculo, "NOTIVEIC");

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<VEICULO>();
                    return RedirectToAction("VoltarBaseVeiculo");
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