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
using System.Text;
using System.Net;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ERP_Condominios_Solution.Controllers
{
    public class CorpoDiretivoController : Controller
    {
        private readonly ICorpoDiretivoAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;
        private String msg;
        private Exception exception;
        CORPO_DIRETIVO objetoForn = new CORPO_DIRETIVO();
        CORPO_DIRETIVO objetoFornAntes = new CORPO_DIRETIVO();
        List<CORPO_DIRETIVO> listaMasterForn = new List<CORPO_DIRETIVO>();
        String extensao;


        public CorpoDiretivoController(ICorpoDiretivoAppService baseApps, ILogAppService logApps, IConfiguracaoAppService confApps, IUsuarioAppService usuApps)
        {
            fornApp = baseApps;
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

        [HttpGet]
        public ActionResult MontarTelaCorpo()
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
            if (Session["ListaCorpo"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                listaMasterForn = listaMasterForn.Where(p => p.CODI_DT_SAIDA_REAL == null).ToList();
                Session["ListaCorpo"] = listaMasterForn;
            }
            ViewBag.Listas = (List<CORPO_DIRETIVO>)Session["ListaCorpo"];
            ViewBag.Title = "Corpo Diretivo";

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCorpo"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCorpo"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0059", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCorpo"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCorpo"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0066", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new CORPO_DIRETIVO();
            Session["MensCorpo"] = 0;
            Session["VoltaCorpo"] = 1;
            return View(objetoForn);
        }

        [HttpGet]
        public ActionResult MontarTelaCorpoGrafica()
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
            if (Session["ListaCorpo"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                listaMasterForn = listaMasterForn.Where(p => p.CODI_DT_SAIDA_REAL == null).ToList();
                Session["ListaCorpo"] = listaMasterForn;
            }
            ViewBag.Listas = (List<CORPO_DIRETIVO>)Session["ListaCorpo"];
            ViewBag.Title = "Corpo Diretivo";

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCorpo"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCorpo"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0059", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCorpo"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new CORPO_DIRETIVO();
            Session["MensCorpo"] = 0;
            Session["VoltaCorpo"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroCorpo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCorpo"] = null;
            return RedirectToAction("MontarTelaCorpo");
        }

        public ActionResult MostrarTudoCorpo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["ListaCorpo"] = listaMasterForn;
            return RedirectToAction("MontarTelaCorpo");
        }

        public ActionResult MostrarInativosCorpo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItens(idAss);
            Session["ListaCorpo"] = listaMasterForn;
            return RedirectToAction("MontarTelaCorpo");
        }

        public ActionResult VoltarBaseCorpo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCorpo"] = null;
            return RedirectToAction("MontarTelaCorpo");
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
        public ActionResult IncluirCorpo()
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
                    Session["MensCorpo"] = 2;
                    return RedirectToAction("MontarTelaCorpo", "CorpoDiretivo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Funcoes = new SelectList(fornApp.GetAllFuncoes(idAss).OrderBy(x => x.FUCO_NM_NOME), "FUCO_CD_ID", "FUCO_NM_NOME");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).Where(p => p.USUA_IN_PROPRIETARIO == 1).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Unidade = usuario.UNID_CD_ID;
            ViewBag.Usuario = usuario.USUA_CD_ID;

            // Prepara view
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            CORPO_DIRETIVO item = new CORPO_DIRETIVO();
            CorpoDiretivoViewModel vm = Mapper.Map<CORPO_DIRETIVO, CorpoDiretivoViewModel>(item);
            vm.CODI_DT_CADASTRO = DateTime.Today.Date;
            vm.CODI_IN_ATIVO = 1;
            vm.CODI_DT_INICIO = DateTime.Today.Date;
            vm.CODI_DT_FINAL = DateTime.Today.Date.AddDays(Convert.ToDouble(conf.CONF_NR_CORPO_DIRETIVO_PERIODO));
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCorpo(CorpoDiretivoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Funcoes = new SelectList(fornApp.GetAllFuncoes(idAss).OrderBy(x => x.FUCO_NM_NOME), "FUCO_CD_ID", "FUCO_NM_NOME");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CORPO_DIRETIVO item = Mapper.Map<CorpoDiretivoViewModel, CORPO_DIRETIVO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCorpo"] = 1;
                        Session["ListaCorpo"] = null;
                        return RedirectToAction("MontarTelaCorpo", "CorpoDiretivo");
                    }
                    if (volta == 2)
                    {
                        Session["MensCorpo"] = 4;
                        Session["ListaCorpo"] = null;
                        return RedirectToAction("MontarTelaCorpo", "CorpoDiretivo");
                    }
                    if (volta == 3)
                    {
                        Session["MensCorpo"] = 3;
                        Session["ListaCorpo"] = null;
                        return RedirectToAction("MontarTelaCorpo", "CorpoDiretivo");
                    }

                    // Sucesso
                    listaMasterForn = new List<CORPO_DIRETIVO>();
                    Session["ListaCorpo"] = null;
                    Session["IdVolta"] = item.CODI_CD_ID;
                    return RedirectToAction("MontarTelaCorpo");
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
        public ActionResult EditarCorpo(Int32 id)
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
                    Session["MensCorpo"] = 2;
                    return RedirectToAction("MontarTelaCorpo", "CorpoDiretivo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Funcoes = new SelectList(fornApp.GetAllFuncoes(idAss).OrderBy(x => x.FUCO_NM_NOME), "FUCO_CD_ID", "FUCO_NM_NOME");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).Where(p => p.USUA_IN_PROPRIETARIO == 1).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            CORPO_DIRETIVO item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Corpo"] = item;
            Session["IdVolta"] = id;
            Session["IdCorpo"] = id;
            CorpoDiretivoViewModel vm = Mapper.Map<CORPO_DIRETIVO, CorpoDiretivoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarCorpo(CorpoDiretivoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Funcoes = new SelectList(fornApp.GetAllFuncoes(idAss).OrderBy(x => x.FUCO_NM_NOME), "FUCO_CD_ID", "FUCO_NM_NOME");
            ViewBag.Usuarios = new SelectList(fornApp.GetAllUsuarios(idAss).OrderBy(x => x.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CORPO_DIRETIVO item = Mapper.Map<CorpoDiretivoViewModel, CORPO_DIRETIVO>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<CORPO_DIRETIVO>();
                    Session["ListaCorpo"] = null;
                    return RedirectToAction("MontarTelaCorpo");
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
        public ActionResult VerCorpo(Int32 id)
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

            // Prepara view
            CORPO_DIRETIVO item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Corpo"] = item;
            Session["IdVolta"] = id;
            Session["IdCorpo"] = id;
            CorpoDiretivoViewModel vm = Mapper.Map<CORPO_DIRETIVO, CorpoDiretivoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirCorpo(Int32 id)
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
                    Session["MensCorpo"] = 2;
                    return RedirectToAction("MontarTelaCorpo", "CorpoDiretivo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CORPO_DIRETIVO item = fornApp.GetItemById(id);
            objetoFornAntes = (CORPO_DIRETIVO)Session["Corpo"];
            item.CODI_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateDelete(item, usuario);
            listaMasterForn = new List<CORPO_DIRETIVO>();
            Session["ListaCorpo"] = null;
            return RedirectToAction("MontarTelaCorpo");
        }

        [HttpGet]
        public ActionResult ReativarCorpo(Int32 id)
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
                    Session["MensCorpo"] = 2;
                    return RedirectToAction("MontarTelaCorpo", "CorpoDiretivo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CORPO_DIRETIVO item = fornApp.GetItemById(id);
            objetoFornAntes = (CORPO_DIRETIVO)Session["Corpo"];
            item.CODI_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateDelete(item, usuario);
            listaMasterForn = new List<CORPO_DIRETIVO>();
            Session["ListaCorpo"] = null;
            return RedirectToAction("MontarTelaCorpo");
        }

        [HttpGet]
        public ActionResult EnviarEMailCorpo(Int32 id)
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
        public ActionResult EnviarEMailCorpo(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioEMailCorpo(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseCorpo");
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
        public Int32 ProcessaEnvioEMailCorpo(MensagemViewModel vm, USUARIO usuario)
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
            mensagem.ASSUNTO = "Corpo Diretivo - Contato";
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
        public ActionResult EnviarSMSCorpo(Int32 id)
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
        public ActionResult EnviarSMSCorpo(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioSMSCorpo(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseCorpo");
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
        public Int32 ProcessaEnvioSMSCorpo(MensagemViewModel vm, USUARIO usuario)
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

    }
}