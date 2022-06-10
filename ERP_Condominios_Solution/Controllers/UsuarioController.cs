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
using System.Data.Entity;

namespace ERP_Condominios_Solution.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly INotificacaoAppService notiApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUnidadeAppService uniApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public UsuarioController(IUsuarioAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps, IConfiguracaoAppService confApps, IUnidadeAppService uniApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notiApps;
            confApp = confApps;
            uniApp = uniApps;
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

        [HttpGet]
        public ActionResult MontarTelaUsuario()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");

            // Carrega listas
            if ((List<USUARIO>)Session["ListaUsuario"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaUsuario"] = listaMaster;
                Session["FiltroUsuario"] = null;
            }
            List<USUARIO> listaUsu = (List<USUARIO>)Session["ListaUsuario"];
            ViewBag.Listas = listaUsu;
            ViewBag.Usuarios = listaUsu.Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            ViewBag.UsuariosBloqueados = listaUsu.Where(p => p.USUA_IN_BLOQUEADO == 1).ToList().Count;
            ViewBag.UsuariosHoje = listaUsu.Where(p => p.USUA_IN_BLOQUEADO == 0 && p.USUA_DT_BLOQUEADO == DateTime.Today.Date).ToList().Count;
            ViewBag.Title = "Usuários";

            // Recupera numero de usuarios do assinante
            Session["NumeroUsuarios"] = listaUsu.Count;
            Int32 usuariosPossiveis = 1000;
            ViewBag.UsuariosPossiveis = usuariosPossiveis;

            // Mensagens
            if (Session["MensUsuario"] != null)
            {
                if ((Int32)Session["MensUsuario"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUsuario"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUsuario"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUsuario"] == 5)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0110", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUsuario"] == 6)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0111", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUsuario"] == 9)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUsuario"] == 50)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objeto = new USUARIO();
            return View(objeto);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaUsuario");
        }

        [HttpPost]
        public ActionResult FiltrarUsuario(USUARIO item)
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
                Int32 volta = baseApp.ExecuteFilter(item.PERF_CD_ID, item.CARG_CD_ID, item.USUA_NM_NOME, item.USUA_NM_LOGIN, item.USUA_NM_EMAIL, idAss, out listaObj);
                Session["FiltroUsuario"] = item;

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensUsuario"] = 1;
                }

                // Sucesso
                Session["MensUsuario"] = 0;
                listaMaster = listaObj;
                Session["ListaUsuario"] = listaObj;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaUsuario");
            }
        }

        public ActionResult RetirarFiltroUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaUsuario"] = null;
            return RedirectToAction("MontarTelaUsuario");
        }

        public ActionResult MostrarTudoUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            listaMaster = baseApp.GetAllUsuarios(idAss);
            Session["ListaUsuario"] = listaMaster;
            return RedirectToAction("MontarTelaUsuario");
        }

        [HttpGet]
        public ActionResult VerAnexoUsuario(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadUsuario(Int32 id)
        {
            USUARIO_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.USAN_AQ_ARQUIVO;
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

        public ActionResult VoltarAnexoUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idUsu = (Int32)Session["IdUsuario"];
            return RedirectToAction("EditarUsuario", new { id = idUsu });
        }

        public ActionResult EditarPerfilUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaUsu"] = 2;
            Int32 idUsu = (Int32)Session["IdUsuario"];
            return RedirectToAction("EditarUsuario", new { id = idUsu });
        }

        [HttpGet]
        public ActionResult VerUsuario(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["IdUsuario"] = id;
            USUARIO item = baseApp.GetItemById(id);
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult IncluirUsuario()
        {
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensUsuario"] = 2;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss).OrderBy(p => p.CARG_NM_NOME), "CARG_CD_ID", "CARG_NM_NOME");
            ViewBag.Unidades = new SelectList(uniApp.GetAllItens(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_EXIBE");

            // Prepara view
            USUARIO item = new USUARIO();
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            vm.USUA_DT_CADASTRO = DateTime.Today.Date;
            vm.USUA_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.USUA_IN_BLOQUEADO = 0;
            vm.USUA_IN_LOGADO = 0;
            vm.USUA_IN_LOGIN_PROVISORIO = 0;
            vm.USUA_IN_PROVISORIO = 0;
            vm.USUA_IN_SISTEMA = 0;
            vm.USUA_NR_ACESSOS = 0;
            vm.USUA_NR_FALHAS = 0;
            vm.CAUS_CD_ID = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirUsuario(UsuarioViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");
            ViewBag.Unidades = new SelectList(uniApp.GetAllItens(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUsuario"] = 3;
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    if (volta == 2)
                    {
                        Session["MensUsuario"] = 4;
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    if (volta == 3)
                    {
                        Session["MensUsuario"] = 5;
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    if (volta == 4 )
                    {
                        Session["MensUsuario"] = 6;
                        return RedirectToAction("MontarTelaUsuario");
                    }

                    // Carrega foto e processa alteracao
                    item.USUA_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                    volta = baseApp.ValidateEdit(item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<USUARIO>();
                    Session["ListaUsuario"] = null;
                    Session["IdUsuario"] = item.USUA_CD_ID;

                    if (Session["FileQueueUsuario"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueUsuario"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueUsuario(file);
                            }
                            else
                            {
                                UploadFotoQueueUsuario(file);
                            }
                        }

                        Session["FileQueueUsuario"] = null;
                    }

                    return RedirectToAction("MontarTelaUsuario");
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
        public ActionResult EditarUsuario(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensUsuario"] = 2;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");
            ViewBag.Unidades = new SelectList(uniApp.GetAllItens(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_EXIBE");
            ViewBag.UsuarioLogado = usuario;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            USUARIO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Usuario"] = item;
            Session["IdUsuario"] = id;
            Session["VoltaUsu"] = 1;
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarUsuario(UsuarioViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");
            ViewBag.Unidades = new SelectList(uniApp.GetAllItens(idAss).OrderBy(x => x.UNID_NM_EXIBE).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUsuario"] = 4;
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    if (volta == 2)
                    {
                        Session["MensUsuario"] = 5;
                        return RedirectToAction("MontarTelaUsuario");
                    }

                    // Mensagens
                    if (Session["MensUsuario"] != null)
                    {
                        if ((Int32)Session["MensUsuario"] == 10)
                        {
                            ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                        }
                        if ((Int32)Session["MensUsuario"] == 11)
                        {
                            ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                        }
                    }

                    // Sucesso
                    listaMaster = new List<USUARIO>();
                    Session["ListaUsuario"] = null;
                    Session["MensUsuario"] = 0;
                    if ((Int32)Session["VoltaUsu"] == 1)
                    {
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    else
                    {
                        return RedirectToAction("CarregarBase", "BaseAdmin");
                    }
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
        public ActionResult BloquearUsuario(Int32 id)
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

                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensUsuario"] = 2;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            USUARIO item = baseApp.GetItemById(id);
            objetoAntes = (USUARIO)Session["Usuario"];

            USUARIO block = new USUARIO();
            block.USUA_CD_ID = item.USUA_CD_ID;
            block.ASSI_CD_ID = item.ASSI_CD_ID;
            block.PERF_CD_ID = item.PERF_CD_ID;
            block.CARG_CD_ID = item.CARG_CD_ID;
            block.USUA_NM_NOME = item.USUA_NM_NOME;
            block.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            block.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            block.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            block.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            block.USUA_NM_SENHA = item.USUA_NM_SENHA;
            block.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            block.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            block.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            block.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            block.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
            block.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
            block.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            block.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            block.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            block.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            block.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            block.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            block.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            block.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            block.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            block.USUA_IN_BLOQUEADO = 1;
            block.USUA_DT_BLOQUEADO = DateTime.Today;

            Int32 volta = baseApp.ValidateBloqueio(block, usuario);
            listaMaster = new List<USUARIO>();
            Session["ListaUsuario"] = null;
            if (Session["FiltroUsuario"] != null)
            {
                FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
            }
            return RedirectToAction("MontarTelaUsuario");
        }

        [HttpGet]
        public ActionResult DesbloquearUsuario(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensUsuario"] = 2;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            USUARIO item = baseApp.GetItemById(id);
            objetoAntes = (USUARIO)Session["Usuario"];

            USUARIO unblock = new USUARIO();
            unblock.USUA_CD_ID = item.USUA_CD_ID;
            unblock.ASSI_CD_ID = item.ASSI_CD_ID;
            unblock.PERF_CD_ID = item.PERF_CD_ID;
            unblock.CARG_CD_ID = item.CARG_CD_ID;
            unblock.USUA_NM_NOME = item.USUA_NM_NOME;
            unblock.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            unblock.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            unblock.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            unblock.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            unblock.USUA_NM_SENHA = item.USUA_NM_SENHA;
            unblock.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            unblock.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            unblock.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            unblock.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            unblock.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
            unblock.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
            unblock.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            unblock.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            unblock.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            unblock.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            unblock.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            unblock.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            unblock.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            unblock.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            unblock.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            unblock.USUA_IN_BLOQUEADO = 0;
            unblock.USUA_DT_BLOQUEADO = null;

            Int32 volta = baseApp.ValidateDesbloqueio(unblock, usuario);
            listaMaster = new List<USUARIO>();
            Session["ListaUsuario"] = null;
            if (Session["FiltroUsuario"] != null)
            {
                FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
            }
            return RedirectToAction("MontarTelaUsuario");
        }

        [HttpGet]
        public ActionResult DesativarUsuario(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensUsuario"] = 2;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            USUARIO item = baseApp.GetItemById(id);
            objetoAntes = (USUARIO)Session["Usuario"];

            USUARIO dis = new USUARIO();
            dis.USUA_CD_ID = item.USUA_CD_ID;
            dis.ASSI_CD_ID = item.ASSI_CD_ID;
            dis.PERF_CD_ID = item.PERF_CD_ID;
            dis.CARG_CD_ID = item.CARG_CD_ID;
            dis.USUA_NM_NOME = item.USUA_NM_NOME;
            dis.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            dis.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            dis.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            dis.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            dis.USUA_NM_SENHA = item.USUA_NM_SENHA;
            dis.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            dis.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            dis.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            dis.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            dis.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            dis.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            dis.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            dis.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            dis.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            dis.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            dis.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            dis.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            dis.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            dis.USUA_IN_ATIVO = 0;
            dis.USUA_DT_ALTERACAO = DateTime.Today;

            Int32 volta = baseApp.ValidateDelete(dis, usuario);
            listaMaster = new List<USUARIO>();
            Session["ListaUsuario"] = null;
            if (Session["FiltroUsuario"] != null)
            {
                FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
            }
            return RedirectToAction("MontarTelaUsuario");
        }

        [HttpGet]
        public ActionResult ReativarUsuario(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensUsuario"] = 2;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            //Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumUsuarios"] <= num)
            {
                Session["MensUsuario"] = 50;
                return RedirectToAction("MontarTelaUsuario", "Usuario");
            }

            // Executar
            USUARIO item = baseApp.GetItemById(id);
            objetoAntes = (USUARIO)Session["Usuario"];

            USUARIO en = new USUARIO();
            en.USUA_CD_ID = item.USUA_CD_ID;
            en.ASSI_CD_ID = item.ASSI_CD_ID;
            en.PERF_CD_ID = item.PERF_CD_ID;
            en.CARG_CD_ID = item.CARG_CD_ID;
            en.USUA_NM_NOME = item.USUA_NM_NOME;
            en.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            en.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            en.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            en.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            en.USUA_NM_SENHA = item.USUA_NM_SENHA;
            en.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            en.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            en.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            en.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            en.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            en.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            en.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            en.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            en.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            en.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            en.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            en.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            en.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            en.USUA_IN_ATIVO = 1;
            en.USUA_DT_ALTERACAO = DateTime.Today;

            Int32 volta = baseApp.ValidateReativar(en, usuario);
            listaMaster = new List<USUARIO>();
            Session["ListaUsuario"] = null;
            if (Session["FiltroUsuario"] != null)
            {
                FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
            }
            return RedirectToAction("MontarTelaUsuario");
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

            Session["FileQueueUsuario"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueUsuario(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdUsuario"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensUsuario"] = 10;
                return RedirectToAction("VoltarAnexoUsuario");
            }

            USUARIO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensUsuario"] = 11;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            USUARIO_ANEXO foto = new USUARIO_ANEXO();
            foto.USAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.USAN_DT_ANEXO = DateTime.Today;
            foto.USAN_IN_ATIVO = 1;
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
            foto.USAN_IN_TIPO = tipo;
            foto.USAN_NM_TITULO = fileName;
            foto.USUA_CD_ID = item.USUA_CD_ID;

            item.USUARIO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoUsuario");
        }

       [HttpPost]
        public ActionResult UploadFileUsuario(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdUsuario"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensUsuario"] = 10;
                return RedirectToAction("VoltarAnexoUsuario");
            }

            USUARIO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensUsuario"] = 11;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            USUARIO_ANEXO foto = new USUARIO_ANEXO();
            foto.USAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.USAN_DT_ANEXO = DateTime.Today;
            foto.USAN_IN_ATIVO = 1;
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
            foto.USAN_IN_TIPO = tipo;
            foto.USAN_NM_TITULO = fileName;
            foto.USUA_CD_ID = item.USUA_CD_ID;

            item.USUARIO_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoUsuario");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueUsuario(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idUsu = (Int32)Session["IdUsuario"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensUsuario"] = 10;
                return RedirectToAction("VoltarAnexoUsuario");
            }

            USUARIO item = baseApp.GetById(idUsu);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensUsuario"] = 11;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
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
                item.USUA_AQ_FOTO = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            return RedirectToAction("VoltarAnexoUsuario");
        }

        [HttpPost]
        public ActionResult UploadFotoUsuario(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdUsuario"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensUsuario"] = 10;
                return RedirectToAction("VoltarAnexoUsuario");
            }

            USUARIO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensUsuario"] = 11;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
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
                item.USUA_AQ_FOTO = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            return RedirectToAction("VoltarAnexoUsuario");
        }

        public ActionResult GerarRelatorioLista()
        {            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "UsuarioLista" + "_" + data + ".pdf";
            List<USUARIO> lista = (List<USUARIO>)Session["ListaUsuario"];
            USUARIO filtro = (USUARIO)Session["FiltroUsuario"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Usuários - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 120f, 120f, 60f, 80f, 50f, 60f, 60f, 80f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Usuários selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome", meuFont))
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
            cell = new PdfPCell(new Paragraph("Login", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cargo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Perfil", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Bloqueado", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Acessos", meuFont))
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

            foreach (USUARIO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.USUA_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.USUA_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.USUA_NM_LOGIN, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARGO.CARG_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PERFIL.PERF_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.USUA_IN_BLOQUEADO == 1 ? "Sim" : "Não", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.USUA_NR_ACESSOS.Value.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.USUA_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.USUA_AQ_FOTO));
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
                if (filtro.USUA_NM_NOME != null)
                {
                    parametros += "Nome: " + filtro.USUA_NM_NOME;
                    ja = 1;
                }
                if (filtro.USUA_NM_LOGIN != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Login: " + filtro.USUA_NM_LOGIN;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Login: " + filtro.USUA_NM_LOGIN;
                    }
                }
                if (filtro.USUA_NM_EMAIL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "E-Mail: " + filtro.USUA_NM_EMAIL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e E-Mail: " + filtro.USUA_NM_EMAIL;
                    }
                }
                if (filtro.PERF_CD_ID > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Perfil: " + filtro.PERFIL.PERF_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Perfil: " + filtro.PERFIL.PERF_NM_NOME;
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

            return RedirectToAction("MontarTelaUsuario");
        }

        public ActionResult GerarRelatorioDetalhe()
        {            
            // Prepara geração
            USUARIO aten = baseApp.GetItemById((Int32)Session["IdUsuario"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Usuario_" + aten.USUA_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font meuFontGreen = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.GREEN);

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
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Usuário - Detalhes", meuFont2))
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
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            
            cell = new PdfPCell();
            cell.Border = 0;
            cell.Colspan = 1;
            image = Image.GetInstance(Server.MapPath(aten.USUA_AQ_FOTO));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.USUA_NM_NOME, meuFontGreen));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("E-Mail: " + aten.USUA_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Login: " + aten.USUA_NM_LOGIN, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cargo: " + aten.CARGO.CARG_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Perfil: " + aten.PERFIL.PERF_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Acessos
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados de Acesso", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.USUA_IN_BLOQUEADO == 1)
            {
                cell = new PdfPCell(new Paragraph("Bloqueado: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Bloqueado: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.USUA_DT_BLOQUEADO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Bloqueio: " + aten.USUA_DT_BLOQUEADO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Bloqueio: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.USUA_IN_PROVISORIO == 1)
            {
                cell = new PdfPCell(new Paragraph("Senha Provisória: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Senha Provisória: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.USUA_IN_LOGIN_PROVISORIO == 1)
            {
                cell = new PdfPCell(new Paragraph("Login Provisório: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Login Provisório: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.USUA_DT_ALTERACAO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Alteração: " + aten.USUA_DT_ALTERACAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Alteração: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.USUA_DT_TROCA_SENHA != null)
            {
                cell = new PdfPCell(new Paragraph("Data Alteração de Senha: " + aten.USUA_DT_TROCA_SENHA.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Alteração de Senha: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Acessos: " + CrossCutting.Formatters.DecimalFormatter(aten.USUA_NR_ACESSOS.Value), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.USUA_DT_ACESSO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Último Acesso: " + aten.USUA_DT_ACESSO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Último Acesso: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("Falhas de Login: " + CrossCutting.Formatters.DecimalFormatter(aten.USUA_NR_FALHAS.Value), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.USUA_DT_ULTIMA_FALHA != null)
            {
                cell = new PdfPCell(new Paragraph("Data Última Falha: " + aten.USUA_DT_ULTIMA_FALHA.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Última Falha: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.USUA_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoUsuario");
        }

        [HttpGet]
        public ActionResult TrocarSenha(Int32 id)
        {
            
            // Prepara view
            return RedirectToAction("TrocarSenha", "ControleAcesso");
        }
    }
}