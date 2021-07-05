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
    public class TelefoneController : Controller
    {
        private readonly ITelefoneAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly ICategoriaTelefoneAppService cfApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        TELEFONE objetoForn = new TELEFONE();
        TELEFONE objetoFornAntes = new TELEFONE();
        List<TELEFONE> listaMasterForn = new List<TELEFONE>();
        String extensao;

        public TelefoneController(ITelefoneAppService fornApps, ILogAppService logApps, ICategoriaTelefoneAppService cfApps, IConfiguracaoAppService confApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            cfApp = cfApps;
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

        public ActionResult EnviarSmsTelefone(Int32 id, String mensagem)
        {
            try
            {
                TELEFONE forn = fornApp.GetById(id);
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica existencia prévia
                if (forn == null)
                {
                    Session["MensSMSForn"] = 1;
                    return RedirectToAction("MontarTelaTelefone");
                }

                // Criticas
                if (forn.TELE_NR_CELULAR == null)
                {
                    Session["MensSMSForn"] = 2;
                    return RedirectToAction("MontarTelaTelefone");
                }

                // Monta token
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Monta routing
                String routing = "1";

                // Monta texto
                String texto = String.Empty;
                //texto = texto.Replace("{Cliente}", clie.CLIE_NM_NOME);

                // inicia processo
                List<String> resposta = new List<string>();
                WebRequest request = WebRequest.Create("https://api.smsfire.com.br/v1/sms/send");
                request.Headers["Authorization"] = auth;
                request.Method = "POST";
                request.ContentType = "application/json";

                // Monta destinatarios
                String listaDest = "55" + Regex.Replace(forn.TELE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();

                // Processa lista
                String responseFromServer = null;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    String campanha = "ERP";

                    String json = null;
                    json = "{\"to\":[\"" + listaDest + "\"]," +
                            "\"from\":\"SMSFire\", " +
                            "\"campaignName\":\"" + campanha + "\", " +
                            "\"text\":\"" + texto + "\"} ";

                    streamWriter.Write(json);
                    streamWriter.Close();
                    streamWriter.Dispose();
                }

                WebResponse response = request.GetResponse();
                resposta.Add(response.ToString());

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                resposta.Add(responseFromServer);

                // Saída
                reader.Close();
                response.Close();
                Session["MensSMSForn"] = 200;
                return RedirectToAction("MontarTelaTelefone");
            }
            catch (Exception ex)
            {
                Session["MensSMSForn"] = 3;
                Session["MensSMSFornErro"] = ex.Message;
                return RedirectToAction("MontarTelaTelefone");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTelefone()
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

            // Carrega listas
            if (Session["ListaTelefone"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaTelefone"] = listaMasterForn;
            }
            if (((List<TELEFONE>)Session["ListaTelefone"]).Count == 0)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaTelefone"] = listaMasterForn;
            }
            ViewBag.Listas = (List<TELEFONE>)Session["ListaTelefone"];
            ViewBag.Title = "Telefones";
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["IncluirTel"] = 0;

            // Indicadores
            ViewBag.Telefones = ((List<TELEFONE>)Session["ListaTelefone"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");

            if (Session["MensTelefone"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensTelefone"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTelefone"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTelefone"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0028", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new TELEFONE();
            objetoForn.TELE_IN_ATIVO = 1;
            Session["MensTelefone"] = 0;
            Session["VoltaTelefone"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroTelefone()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaTelefone"] = null;
            Session["FiltroTelefone"] = null;
            if ((Int32)Session["VoltaTelefone"] == 2)
            {
                return RedirectToAction("VerCardsTelefone");
            }
            return RedirectToAction("MontarTelaTelefone");
        }

        public ActionResult MostrarTudoTelefone()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroTelefone"] = null;
            Session["ListaTelefone"] = listaMasterForn;
            if ((Int32)Session["VoltaTelefone"] == 2)
            {
                return RedirectToAction("VerCardsTelefone");
            }
            return RedirectToAction("MontarTelaTelefone");
        }

        [HttpPost]
        public ActionResult FiltrarTelefone(TELEFONE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TELEFONE> listaObj = new List<TELEFONE>();
                Session["FiltroTelefone"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.CATE_CD_ID, item.TELE_NM_NOME, item.TELE_NR_TELEFONE, item.TELE_NM_CIDADE, item.UF_CD_ID, item.TELE_NR_CELULAR, item.TELE_NM_EMAIL, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensTelefone"] = 1;
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    return RedirectToAction("MontarTelaTelefone");
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaTelefone"] = listaObj;
                if ((Int32)Session["VoltaTelefone"] == 2)
                {
                    return RedirectToAction("VerCardsTelefone");
                }
                return RedirectToAction("MontarTelaTelefone");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaTelefone");
            }
        }

        public ActionResult VoltarBaseTelefone()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaTelefone"] == 2)
            {
                return RedirectToAction("VerCardsTelefone");
            }
            return RedirectToAction("MontarTelaTelefone");
        }

        [HttpGet]
        public ActionResult IncluirTelefone()
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
                    Session["MensTelefone"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["VoltaProp"] = 4;

            // Prepara view
            TELEFONE item = new TELEFONE();
            TelefoneViewModel vm = Mapper.Map<TELEFONE, TelefoneViewModel>(item);
            vm.TELE_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirTelefone(TelefoneViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TELEFONE item = Mapper.Map<TelefoneViewModel, TELEFONE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTelefone"] = 3;
                        return RedirectToAction("MontarTelaTelefone", "Telefone");
                    }

                    // Sucesso
                    listaMasterForn = new List<TELEFONE>();
                    Session["ListaTelefone"] = null;
                    Session["IncluirTele"] = 1;
                    Session["Telefones"] = fornApp.GetAllItens(idAss);
                    Session["IdVolta"] = item.TELE_CD_ID;
                    if ((Int32)Session["VoltaTelefone"] == 2)
                    {
                        return RedirectToAction("IncluirTelefone");
                    }
                    return RedirectToAction("MontarTelaTelefone");
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