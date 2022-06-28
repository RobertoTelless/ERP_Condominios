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
    public class MoradorController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;
        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        String extensao;

        public MoradorController(IUsuarioAppService baseApps, ILogAppService logApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
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
        public ActionResult MontarTelaMorador()
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
                    Session["MensMorador"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega Morador
            USUARIO usu = baseApp.GetItemById(usuario.USUA_CD_ID);
            Session["Morador"] = usu;
            ViewBag.Morador = usu;
            ViewBag.Title = "Morador";

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            listaMaster = baseApp.GetAllItens(idAss).Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
            ViewBag.Moradores = listaMaster;

            // Abre view
            Session["MensMorador"] = 0;
            Session["VoltaMorador"] = 1;
            return View(usuario);
        }

        public ActionResult VoltarBaseMorador()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMorador"] = null;
            Session["FiltroMorador"] = null;
            if ((Int32)Session["VoltaMorador"] == 1)
            {
                return RedirectToAction("MontarTelaMorador");
            }
            return RedirectToAction("MontarTelaMoradores");
        }

        public ActionResult VoltarAnexoMorador()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdMorador"];
            return RedirectToAction("VerMorador", new { id = id });
        }

        [HttpGet]
        public ActionResult MontarTelaMoradores()
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
                    Session["MensMorador"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if ((List<USUARIO>)Session["ListaMorador"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss).Where(p => p.USUA_IN_MORADOR == 1).ToList();
                Session["ListaMorador"] = listaMaster;
                Session["FiltroMorador"] = null;
            }

            ViewBag.Listas = (List<USUARIO>)Session["ListaMorador"];
            ViewBag.Title = "Moradores";
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores

            // Mensagem
            if ((Int32)Session["MensMorador"] == 1)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensMorador"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["UsuarioMensagem"] = 2;
            Session["MensMorador"] = 0;
            Session["VoltaMorador"] = 2;
            objeto = new USUARIO();
            return View(objeto);
        }

        public ActionResult RetirarFiltroMorador()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMorador"] = null;
            Session["FiltroMorador"] = null;
            listaMaster = new List<USUARIO>();
            return RedirectToAction("MontarTelaMoradores");
        }

        [HttpPost]
        public ActionResult FiltrarMorador(USUARIO item)
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

                    List<USUARIO> listaObj = new List<USUARIO>();
                    Int32 volta = baseApp.ExecuteFilterMorador(item.USUA_NM_NOME, item.UNID_CD_ID, idAss, out listaObj);
                    Session["FiltroMorador"] = item;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMorador"] = 1;
                        ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        return RedirectToAction("MontarTelaMoradores");
                    }

                    // Sucesso
                    Session["MensMorador"] = 0;
                    listaMaster = listaObj;
                    Session["ListaMorador"] = listaObj;
                    return RedirectToAction("MontarTelaMoradores");
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

        [HttpGet]
        public ActionResult VerMorador(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "MOR")
                {
                    Session["MensMorador"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Unidade = usuario.UNID_CD_ID;

            // Recupera Moradores da unidade
            List<USUARIO> moradores = baseApp.GetAllItens(idAss).Where(p => p.USUA_IN_MORADOR == 1 & p.UNID_CD_ID == usuario.UNID_CD_ID).ToList();
            ViewBag.Moradores = moradores;

            // Mensagens
            if ((Int32)Session["MensMorador"] == 8)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0067", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensMorador"] == 10)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0068", CultureInfo.CurrentCulture));
            }

            // Prepara view
            USUARIO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Morador"] = item;
            Session["IdVolta"] = id;
            Session["IdMorador"] = id;
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult VerEncomenda(Int32 idEnc)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            return RedirectToAction("VerEncomenda", "Encomenda", new { id = idEnc });
        }

        [HttpGet]
        public ActionResult EnviarEMailMorador()
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EnviarEMailUsuario", "Usuario", new { id = (Int32)Session["IdMorador"] });
        }

        [HttpGet]
        public ActionResult EnviarSMSMorador()
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EnviarSMSUsuario", "Usuario", new { id = (Int32)Session["IdMorador"] });
        }

        [HttpGet]
        public ActionResult VerReserva(Int32 idRes)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            return RedirectToAction("VerReserva", "Reserva", new { id = idRes });
        }

        [HttpGet]
        public ActionResult VerMudanca(Int32 idMud)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            return RedirectToAction("VerMudanca", "Mudanca", new { id = idMud });
        }

        [HttpGet]
        public ActionResult VerAutorizacao(Int32 idAut)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["VoltaAutorizacao"] = 2;
            return RedirectToAction("VerAutorizacao", "Autorizacao", new { id = idAut });
        }

        [HttpGet]
        public ActionResult VerOcorrencia(Int32 idOcor)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["VoltaOcorrencia"] = 2;
            return RedirectToAction("VerOcorrencia", "Ocorrencia", new { id = idOcor });
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

        public ActionResult EnviarSmsMorador(Int32 id, String mensagem)
        {
            try
            {
                USUARIO resp = baseApp.GetById(id);
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica existencia prévia
                if (resp == null)
                {
                    Session["MensMorador"] = 8;
                    return RedirectToAction("VoltarAnexoMorador");
                }

                // Criticas
                if (resp.USUA_NR_CELULAR == null)
                {
                    Session["MensMorador"] = 10;
                    return RedirectToAction("VoltarAnexoMorador");
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
                return RedirectToAction("VoltarAnexoMorador");
            }
            catch (Exception ex)
            {
                Session["MensUnidade"] = 11;
                Session["MensSMSErro"] = ex.Message;
                return RedirectToAction("VoltarAnexoMorador");
            }
        }

    }
}