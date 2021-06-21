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
using Correios.Net;
using Canducci.Zip;

namespace ERP_Condominios_Solution.Controllers
{
    public class CondominioController : Controller
    {
        private readonly IAssinanteAppService baseApp;
        private String msg;
        private Exception exception;
        ASSINANTE objeto = new ASSINANTE();
        ASSINANTE objetoAntes = new ASSINANTE();
        List<ASSINANTE> listaMaster = new List<ASSINANTE>();
        String extensao;

        public CondominioController(IAssinanteAppService baseApps)
        {
            baseApp = baseApps;
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
        public ActionResult MontarTelaCondominio(Int32? id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "MOR" || usuario.PERFIL.PERF_SG_SIGLA == "POR" || usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensCondominio"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega Dados
            if (Session["Condominio"] == null)
            {
                objeto = baseApp.GetItemById(usuario.ASSI_CD_ID);
                Session["Condominio"] = objeto;
            }
            ViewBag.Condominio = objeto;
            ViewBag.Title = "Condominio";

            // Indicadores
            ViewBag.Tipos = new SelectList(baseApp.GetAllTiposPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.TC = new SelectList(baseApp.GetAllTiposCondominio(), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Perfil = usuario.PERF_CD_ID;

            // Mensagem
            if (Session["MensCondominio"] != null)
            {
                if ((Int32)Session["MensCondominio"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensCondominio"] = 0;
                }
                if ((Int32)Session["MensCondominio"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensCondominio"] = 0;
                }
                if ((Int32)Session["MensCondominio"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0020", CultureInfo.CurrentCulture));
                    Session["MensCondominio"] = 0;
                }
            }

            // Abre view
            Session["MensCondominio"] = 0;
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(objeto);
            objetoAntes = objeto;
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaCondominio(AssinanteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTiposPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.TC = new SelectList(baseApp.GetAllTiposCondominio(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<ASSINANTE>();
                    Session["Condominio"] = null;
                    Session["MensCondominio"] = 0;
                    return RedirectToAction("MontarTelaCondominio");
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

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaCondominio");
        }

        [HttpPost]
        public JsonResult PesquisaCEP_Javascript(String cep, int tipoEnd)
        {
            // Chama servico ECT
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ASSINANTE item = baseApp.GetItemById(usuarioLogado.ASSI_CD_ID);

            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(cep);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza
            var hash = new Hashtable();
            hash.Add("ASSI_NM_ENDERECO", end.Address);
            hash.Add("ASSI_NR_NUMERO", end.Complement);
            hash.Add("ASSI_NM_BAIRRO", end.District);
            hash.Add("ASSI_NM_CIDADE", end.City);
            hash.Add("UF_CD_ID", baseApp.GetUFBySigla(end.Uf).UF_CD_ID);
            hash.Add("ASSI_NR_CEP", cep);

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        [HttpGet]
        public ActionResult VerAnexoCondominio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            ASSINANTE_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoCondominio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaCondominio");
        }

        public FileResult DownloadCondominio(Int32 id)
        {
            ASSINANTE_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.ASAN_AQ_ARQUIVO;
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
        public ActionResult UploadFileCondominio(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCondominio"] = 1;
                return RedirectToAction("VoltarAnexoCondominio");
            }

            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ASSINANTE item = baseApp.GetItemById(usuario.ASSI_CD_ID);
            var fileName = Path.GetFileName(file.FileName);

            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCondominio"] = 3;
                return RedirectToAction("VoltarAnexoCliente");
            }

            String caminho = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Condominio/" + item.ASSI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            ASSINANTE_ANEXO foto = new ASSINANTE_ANEXO();
            foto.ASAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.ASAN_DT_ANEXO = DateTime.Today;
            foto.ASAN_IN_ATIVO = 1;
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
            foto.ASAN_IN_TIPO = tipo;
            foto.ASAN_NM_TITULO = fileName;
            foto.ASSI_CD_ID = item.ASSI_CD_ID;

            item.ASSINANTE_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);
            return RedirectToAction("VoltarAnexoCondominio");
        }

        [HttpPost]
        public ActionResult UploadFotoCondominio(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCondominio"] = 1;
                return RedirectToAction("VoltarAnexoCondominio");
            }

            // Recupera arquivo
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ASSINANTE item = baseApp.GetItemById(usuario.ASSI_CD_ID);
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCondominio"] = 3;
                return RedirectToAction("VoltarAnexoCondominio");
            }
            String caminho = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Condominio/" + item.ASSI_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                file.SaveAs(path);

                // Gravar registro
                item.ASSI_AQ_FOTO = "~" + caminho + fileName;
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);
            }
            else
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0020", CultureInfo.CurrentCulture));
                Session["MensCondominio"] = 4;
                return RedirectToAction("VoltarAnexoCondominio");
            }
            return RedirectToAction("VoltarAnexoCondominio");
        }

        public ActionResult VerUnidade(Int32 idUnidade)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaUnidade"] = 2;
            return RedirectToAction("EditarUnidade", "Unidade", new { id = idUnidade });
        }

        public ActionResult VerVaga(Int32 idVaga)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaVaga"] = 2;
            return RedirectToAction("EditarVaga", "Vaga", new { id = idVaga });
        }

        public ActionResult VerEntradaSaida(Int32 idES)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaEntradaSaida"] = 2;
            return RedirectToAction("EditarEntradaSaida", "EntradaSaida", new { id = idES});
        }

        public ActionResult VerControleVeiculo(Int32 idCV)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaControleVeiculo"] = 2;
            return RedirectToAction("EditarControleVeiculo", "ControleVeiculo", new { id = idCV});
        }

    }
}