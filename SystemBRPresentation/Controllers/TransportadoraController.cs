using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SystemBRPresentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using SystemBRPresentation.ViewModels;
using System.IO;
using ExternalServices;
using Canducci.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections;
using EntitiesServices.WorkClasses;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class TransportadoraController : Controller
    {
        private readonly IMatrizAppService matrizApp;
        private readonly ITransportadoraAppService tranApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        private String extensao;
        TRANSPORTADORA objetoTran = new TRANSPORTADORA();
        TRANSPORTADORA objetoTranAntes = new TRANSPORTADORA();
        List<TRANSPORTADORA> listaMasterTran = new List<TRANSPORTADORA>();

        public TransportadoraController(IMatrizAppService matrizApps, ITransportadoraAppService tranApps, IConfiguracaoAppService confApps)
        {
            matrizApp = matrizApps;
            tranApp = tranApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            TRANSPORTADORA item = new TRANSPORTADORA();
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            
            if (SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA == "ADM")
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult EnviarSmsCliente(Int32 id, String mensagem)
        {
            try
            {
                TRANSPORTADORA tran = tranApp.GetById(id);

                // Verifica existencia prévia
                if (tran == null)
                {
                    Session["MensSMSTrans"] = 1;
                    return RedirectToAction("MontarTelaTransportadora");
                }

                // Criticas
                if (tran.TRAN_NR_TELEFONES == null)
                {
                    Session["MensSMSTrans"] = 2;
                    return RedirectToAction("MontarTelaTransportadora");
                }

                // Monta token
                CONFIGURACAO conf = confApp.GetItemById(SessionMocks.IdAssinante);
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
                String listaDest = "55" + Regex.Replace(tran.TRAN_NR_TELEFONES, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();

                // Processa lista
                String responseFromServer = null;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    String campanha = "SystemBR";

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
                Session["MensSMSTrans"] = 200;
                return RedirectToAction("MontarTelaTransportadora");
            }
            catch (Exception ex)
            {
                Session["MensSMSTrans"] = 3;
                Session["MensSMSTransErro"] = ex.Message;
                return RedirectToAction("MontarTelaTransportadora");
            }
        }

        [HttpPost]
        public JsonResult PesquisaCNPJ(string cnpj)
        {
            var tran = new TRANSPORTADORA();

            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(cnpj, "[^0-9]", "");
            String json = String.Empty;

            WebRequest request = WebRequest.Create(url);
            request.Headers["Authorization"] = "df3c411d-bb44-41eb-9304-871c45d72978-cd751b62-ff3d-4421-a9d2-b97e01ca6d2b";

            try
            {
                WebResponse response = request.GetResponse();

                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }

                var jObject = JObject.Parse(json);

                CNPJ pesquisaCNPJ = new CNPJ();
                pesquisaCNPJ.RAZAO = jObject["name"] == null ? String.Empty : jObject["name"].ToString();
                pesquisaCNPJ.NOME = jObject["alias"] == null ? jObject["name"].ToString() : jObject["alias"].ToString();
                pesquisaCNPJ.CEP = jObject["address"]["zip"].ToString();
                pesquisaCNPJ.ENDERECO = jObject["address"]["street"].ToString();
                //matriz.numero = jObject["address"]["number"].ToString();
                pesquisaCNPJ.BAIRRO = jObject["address"]["neighborhood"].ToString();
                pesquisaCNPJ.CIDADE = jObject["address"]["city"].ToString();
                pesquisaCNPJ.UF = SessionMocks.UFs.Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                pesquisaCNPJ.INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                pesquisaCNPJ.TELEFONE = jObject["phone"].ToString();
                //matriz.CLIE_NR_TELEFONE_ADICIONAL = jObject["phone_alt"].ToString();
                pesquisaCNPJ.EMAIL = jObject["email"].ToString();

                return Json(pesquisaCNPJ);
            }
            catch (WebException ex)
            {
                var hash = new Hashtable();
                hash.Add("status", "ERROR");

                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "BadRequest")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "CNPJ inválido");
                    return Json(hash);
                }
                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "NotFound")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "O CNPJ consultado não está registrado na Receita Federal");
                    return Json(hash);
                }
                else
                {
                    hash.Add("public", 0);
                    return Json("message", ex.Message);
                }
            }
        }

        [HttpPost]
        public JsonResult PesquisaCEP(String cep)
        {

            // Chama servico ECT
            //Address end = ExternalServices.ECT_Services.GetAdressCEP(item.CLIE_NR_CEP_BUSCA);
            //Endereco end = ExternalServices.ECT_Services.GetAdressCEPService(item.CLIE_NR_CEP_BUSCA);
            TRANSPORTADORA cli = tranApp.GetItemById(SessionMocks.idVolta);

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

            hash.Add("endereco", end.Address);
            hash.Add("numero", end.Complement);
            hash.Add("bairro", end.District);
            hash.Add("cidade", end.City);
            hash.Add("uf", end.Uf);
            hash.Add("cep", cep);

            // Retorna
            SessionMocks.voltaCEP = 2;
            return Json(hash);
        }

        [HttpGet]
        public ActionResult MontarTelaTransportadora()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaTransportadora == null)
            {
                listaMasterTran = tranApp.GetAllItens();
                SessionMocks.listaTransportadora = listaMasterTran;
            }
            if (SessionMocks.listaTransportadora.Count == 0)
            {
                listaMasterTran = tranApp.GetAllItens();
                SessionMocks.listaTransportadora = listaMasterTran;
            }
            ViewBag.Listas = SessionMocks.listaTransportadora;
            ViewBag.Title = "Transportadorasxxx";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensTransportadora"] != null)
            {
                if ((Int32)Session["MensTransportadora"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensTransportadora"] = 0;
                }
            }

            // Abre view
            objetoTran = new TRANSPORTADORA();
            if (SessionMocks.filtroTransportadora != null)
            {
                objetoTran = SessionMocks.filtroTransportadora;
            }
            SessionMocks.voltaTransportadora = 1;
            return View(objetoTran);
        }

        public ActionResult RetirarFiltroTransportadora()
        {
            
            SessionMocks.listaTransportadora = null;
            SessionMocks.filtroTransportadora = null;
            return RedirectToAction("MontarTelaTransportadora");
        }

        public ActionResult MostrarTudoTransportadora()
        {
            
            listaMasterTran = tranApp.GetAllItensAdm();
            SessionMocks.filtroTransportadora = null;
            SessionMocks.listaTransportadora = listaMasterTran;
            return RedirectToAction("MontarTelaTransportadora");
        }

        [HttpPost]
        public ActionResult FiltrarTransportadora(TRANSPORTADORA item)
        {
            
            try
            {
                // Executa a operação
                List<TRANSPORTADORA> listaObj = new List<TRANSPORTADORA>();
                SessionMocks.filtroTransportadora = item;
                Int32 volta = tranApp.ExecuteFilter(item.TRAN_NM_NOME, item.TRAN_NR_CNPJ, item.TRAN_NM_EMAIL, item.TRAN_NM_CIDADE, item.TRAN_SG_UF, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensTransportadora"] = 1;
                }

                // Sucesso
                listaMasterTran = listaObj;
                SessionMocks.listaTransportadora = listaObj;
                return RedirectToAction("MontarTelaTransportadora");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaTransportadora");
            }
        }

        public ActionResult VoltarBaseTransportadora()
        {
            
            return RedirectToAction("MontarTelaTransportadora");
        }

        [HttpGet]
        public ActionResult IncluirTransportadora()
        {
            
            // Prepara listas
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.TipoVeiculos = new SelectList(tranApp.GetAllTipoVeiculo().OrderBy(x => x.TIVE_NM_NOME).ToList<TIPO_VEICULO>(), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.TipoTransportes = new SelectList(tranApp.GetAllTipoTransporte().OrderBy(x => x.TITR_NM_NOME).ToList<TIPO_TRANSPORTE>(), "TITR_CD_ID", "TITR_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            TRANSPORTADORA item = new TRANSPORTADORA();
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.TRAN_DT_CADASTRO = DateTime.Today;
            vm.TRAN_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.FILI_CD_ID = SessionMocks.idFilial;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirTransportadora(TransportadoraViewModel vm)
        {
            
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.TipoVeiculos = new SelectList(tranApp.GetAllTipoVeiculo(), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.TipoTransportes = new SelectList(tranApp.GetAllTipoTransporte(), "TITR_CD_ID", "TITR_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TRANSPORTADORA item = Mapper.Map<TransportadoraViewModel, TRANSPORTADORA>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = tranApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0113", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Carrega logo e processa alteracao
                    item.TRAN_AQ_LOGO = "~/Imagens/Base/FotoBase.jpg";
                    volta = tranApp.ValidateEdit(item, item, usuarioLogado);
                    
                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Transportadoras/" + item.TRAN_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Transportadoras/" + item.TRAN_CD_ID.ToString() + "/Logos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterTran = new List<TRANSPORTADORA>();
                    SessionMocks.listaTransportadora = null;
                    SessionMocks.idVolta = item.TRAN_CD_ID;

                    if (Session["FileQueueTrans"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueTrans"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueTransportadora(file);
                            }
                            else
                            {
                                UploadFotoQueueTransportadora(file);
                            }
                        }

                        Session["FileQueueTrans"] = null;
                    }

                    return RedirectToAction("MontarTelaTransportadora");
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
        public ActionResult VerTransportadora(Int32 id)
        {
            
            // Prepara view
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.TipoVeiculos = new SelectList(tranApp.GetAllTipoVeiculo().OrderBy(x => x.TIVE_NM_NOME).ToList<TIPO_VEICULO>(), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.TipoTransportes = new SelectList(tranApp.GetAllTipoTransporte().OrderBy(x => x.TITR_NM_NOME).ToList<TIPO_TRANSPORTE>(), "TITR_CD_ID", "TITR_NM_NOME");
            TRANSPORTADORA item = tranApp.GetItemById(id);
            objetoTranAntes = item;
            SessionMocks.transportadora = item;
            SessionMocks.idVolta = id;
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTransportadora(Int32 id)
        {
            
            // Prepara view
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.TipoVeiculos = new SelectList(tranApp.GetAllTipoVeiculo().OrderBy(x => x.TIVE_NM_NOME).ToList<TIPO_VEICULO>(), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.TipoTransportes = new SelectList(tranApp.GetAllTipoTransporte().OrderBy(x => x.TITR_NM_NOME).ToList<TIPO_TRANSPORTE>(), "TITR_CD_ID", "TITR_NM_NOME");
            TRANSPORTADORA item = tranApp.GetItemById(id);
            objetoTranAntes = item;
            SessionMocks.transportadora = item;
            SessionMocks.idVolta = id;
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarTransportadora(TransportadoraViewModel vm)
        {
            
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.TipoVeiculos = new SelectList(tranApp.GetAllTipoVeiculo().OrderBy(x => x.TIVE_NM_NOME).ToList<TIPO_VEICULO>(), "TIVE_CD_ID", "TIVE_NM_NOME");
            ViewBag.TipoTransportes = new SelectList(tranApp.GetAllTipoTransporte().OrderBy(x => x.TITR_NM_NOME).ToList<TIPO_TRANSPORTE>(), "TITR_CD_ID", "TITR_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    TRANSPORTADORA item = Mapper.Map<TransportadoraViewModel, TRANSPORTADORA>(vm);
                    Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTran = new List<TRANSPORTADORA>();
                    SessionMocks.listaTransportadora = null;
                    return RedirectToAction("MontarTelaTransportadora");
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
        public ActionResult ExcluirTransportadora(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usu = SessionMocks.UserCredentials;

            // Executar
            TRANSPORTADORA item = tranApp.GetItemById(id);
            TRANSPORTADORA ex = new TRANSPORTADORA();
            ex.ASSI_CD_ID = item.ASSI_CD_ID;
            ex.FILI_CD_ID = item.FILI_CD_ID;
            ex.MATR_CD_ID = item.MATR_CD_ID;
            ex.TITR_CD_ID = item.TITR_CD_ID;
            ex.TIVE_CD_ID = item.TIVE_CD_ID;
            ex.TRAN_CD_ID = item.TRAN_CD_ID;
            ex.TRAN_AQ_LOGO = item.TRAN_AQ_LOGO;
            ex.TRAN_DS_AREA_COBERTURA = item.TRAN_DS_AREA_COBERTURA;
            ex.TRAN_DS_INFORMACOES_GERAIS = item.TRAN_DS_INFORMACOES_GERAIS;
            ex.TRAN_DS_SEGURO = item.TRAN_DS_SEGURO;
            ex.TRAN_DS_TRANSPORTE_ESPECIAL = item.TRAN_DS_TRANSPORTE_ESPECIAL;
            ex.TRAN_DT_CADASTRO = item.TRAN_DT_CADASTRO;
            ex.TRAN_IN_ATIVO = 0;
            ex.TRAN_NM_BAIRRO = item.TRAN_NM_BAIRRO;
            ex.TRAN_NM_CIDADE = item.TRAN_NM_CIDADE;
            ex.TRAN_NM_CONTATOS = item.TRAN_NM_CONTATOS;
            ex.TRAN_NM_EMAIL = item.TRAN_NM_EMAIL;
            ex.TRAN_NM_ENDERECO = item.TRAN_NM_ENDERECO;
            ex.TRAN_SG_UF = item.TRAN_SG_UF;
            ex.TRAN_NR_CEP = item.TRAN_NR_CEP;
            ex.TRAN_DT_CADASTRO = item.TRAN_DT_CADASTRO;
            ex.TRAN_NR_INSCRICAO_ESTADUAL = item.TRAN_NR_INSCRICAO_ESTADUAL;
            ex.TRAN_NR_INSCRICAO_MUNICIPAL = item.TRAN_NR_INSCRICAO_MUNICIPAL;
            ex.TRAN_NM_WEBSITE = item.TRAN_NM_WEBSITE;
            ex.TRAN_TX_OBSERVACOES = item.TRAN_TX_OBSERVACOES;
            ex.TRAN_DS_AREA_COBERTURA = item.TRAN_DS_AREA_COBERTURA;
            ex.TRAN_NM_NOME = item.TRAN_NM_NOME;
            ex.TRAN_NM_RAZAO = item.TRAN_NM_RAZAO;
            ex.TRAN_NR_CNPJ = item.TRAN_NR_CNPJ;
            ex.TRAN_NR_TELEFONES = item.TRAN_NR_TELEFONES;
            objetoTranAntes = item;
            Int32 volta = tranApp.ValidateDelete(ex, usu);
            listaMasterTran = new List<TRANSPORTADORA>();
            SessionMocks.listaTransportadora = null;
            return RedirectToAction("MontarTelaTransportadora");
        }

        [HttpGet]
        public ActionResult ReativarTransportadora(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usu = SessionMocks.UserCredentials;
            // Executar
            TRANSPORTADORA item = tranApp.GetItemById(id);
            TRANSPORTADORA ex = new TRANSPORTADORA();
            ex.ASSI_CD_ID = item.ASSI_CD_ID;
            ex.FILI_CD_ID = item.FILI_CD_ID;
            ex.MATR_CD_ID = item.MATR_CD_ID;
            ex.TITR_CD_ID = item.TITR_CD_ID;
            ex.TIVE_CD_ID = item.TIVE_CD_ID;
            ex.TRAN_CD_ID = item.TRAN_CD_ID;
            ex.TRAN_AQ_LOGO = item.TRAN_AQ_LOGO;
            ex.TRAN_DS_AREA_COBERTURA = item.TRAN_DS_AREA_COBERTURA;
            ex.TRAN_DS_INFORMACOES_GERAIS = item.TRAN_DS_INFORMACOES_GERAIS;
            ex.TRAN_DS_SEGURO = item.TRAN_DS_SEGURO;
            ex.TRAN_DS_TRANSPORTE_ESPECIAL = item.TRAN_DS_TRANSPORTE_ESPECIAL;
            ex.TRAN_DT_CADASTRO = item.TRAN_DT_CADASTRO;
            ex.TRAN_IN_ATIVO = 1;
            ex.TRAN_NM_BAIRRO = item.TRAN_NM_BAIRRO;
            ex.TRAN_NM_CIDADE = item.TRAN_NM_CIDADE;
            ex.TRAN_NM_CONTATOS = item.TRAN_NM_CONTATOS;
            ex.TRAN_NM_EMAIL = item.TRAN_NM_EMAIL;
            ex.TRAN_NM_ENDERECO = item.TRAN_NM_ENDERECO;
            ex.TRAN_SG_UF = item.TRAN_SG_UF;
            ex.TRAN_NR_CEP = item.TRAN_NR_CEP;
            ex.TRAN_DT_CADASTRO = item.TRAN_DT_CADASTRO;
            ex.TRAN_NR_INSCRICAO_ESTADUAL = item.TRAN_NR_INSCRICAO_ESTADUAL;
            ex.TRAN_NR_INSCRICAO_MUNICIPAL = item.TRAN_NR_INSCRICAO_MUNICIPAL;
            ex.TRAN_NM_WEBSITE = item.TRAN_NM_WEBSITE;
            ex.TRAN_TX_OBSERVACOES = item.TRAN_TX_OBSERVACOES;
            ex.TRAN_DS_AREA_COBERTURA = item.TRAN_DS_AREA_COBERTURA;
            ex.TRAN_NM_NOME = item.TRAN_NM_NOME;
            ex.TRAN_NM_RAZAO = item.TRAN_NM_RAZAO;
            ex.TRAN_NR_CNPJ = item.TRAN_NR_CNPJ;
            ex.TRAN_NR_TELEFONES = item.TRAN_NR_TELEFONES;
            objetoTranAntes = item;
            Int32 volta = tranApp.ValidateReativar(ex, usu);
            listaMasterTran = new List<TRANSPORTADORA>();
            SessionMocks.listaTransportadora = null;
            return RedirectToAction("MontarTelaTransportadora");
        }

        public ActionResult VerCardsTransportadora()
        {
            
            // Carrega listas
            if (SessionMocks.listaTransportadora == null)
            {
                listaMasterTran = tranApp.GetAllItens();
                SessionMocks.listaTransportadora = listaMasterTran;
            }
            ViewBag.Listas = SessionMocks.listaTransportadora;
            ViewBag.Title = "Transportadoras";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Filiais = new SelectList(tranApp.GetAllFilial(), "FILI_CD_ID", "FILI_NM_NOME");

            // Indicadores
            ViewBag.Transportadoras = tranApp.GetAllItens().Count;

            // Abre view
            objetoTran = new TRANSPORTADORA();
            SessionMocks.voltaTransportadora = 2;
            if (SessionMocks.filtroTransportadora != null)
            {
                objetoTran = SessionMocks.filtroTransportadora;
            }
            return View(objetoTran);
        }

        [HttpGet]
        public ActionResult VerAnexoTransportadora(Int32 id)
        {
            
            // Prepara view
            TRANSPORTADORA_ANEXO item = tranApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoTransportadora()
        {
            
            return RedirectToAction("EditarTransportadora", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadTransportadora(Int32 id)
        {
            TRANSPORTADORA_ANEXO item = tranApp.GetAnexoById(id);
            String arquivo = item.TRAX_AQ_ARQUIVO;
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

            Session["FileQueueTrans"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueTransportadora(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }

            TRANSPORTADORA item = tranApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Transportadoras/" + item.TRAN_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            TRANSPORTADORA_ANEXO foto = new TRANSPORTADORA_ANEXO();
            foto.TRAX_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.TRAX_DT_ANEXO = DateTime.Today;
            foto.TRAX_IN_ATIVO = 1;
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
            foto.TRAX_IN_TIPO = tipo;
            foto.TRAX_NM_TITULO = fileName;
            foto.TRAN_CD_ID = item.TRAN_CD_ID;

            item.TRANSPORTADORA_ANEXO.Add(foto);
            objetoTranAntes = item;
            Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes);
            return RedirectToAction("VoltarAnexoTransportadora");
        }

        [HttpPost]
        public ActionResult UploadFileTransportadora(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }

            TRANSPORTADORA item = tranApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Transportadoras/" + item.TRAN_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            TRANSPORTADORA_ANEXO foto = new TRANSPORTADORA_ANEXO();
            foto.TRAX_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.TRAX_DT_ANEXO = DateTime.Today;
            foto.TRAX_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            foto.TRAX_IN_TIPO = tipo;
            foto.TRAX_NM_TITULO = fileName;
            foto.TRAN_CD_ID = item.TRAN_CD_ID;

            item.TRANSPORTADORA_ANEXO.Add(foto);
            objetoTranAntes = item;
            Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes);
            return RedirectToAction("VoltarAnexoTransportadora");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueTransportadora(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }

            TRANSPORTADORA item = tranApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Transportadoras/" + item.TRAN_CD_ID.ToString() + "/Logos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.TRAN_AQ_LOGO = "~" + caminho + fileName;
            objetoTranAntes = item;
            Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes);
            return RedirectToAction("VoltarAnexoTransportadora");
        }

        [HttpPost]
        public ActionResult UploadFotoTransportadora(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }

            TRANSPORTADORA item = tranApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoTransportadora");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Transportadoras/" + item.TRAN_CD_ID.ToString() + "/Logos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.TRAN_AQ_LOGO = "~" + caminho + fileName;
            objetoTranAntes = item;
            Int32 volta = tranApp.ValidateEdit(item, objetoTranAntes);
            return RedirectToAction("VoltarAnexoTransportadora");
        }

        [HttpGet]
        public ActionResult SlideShowTransportadora()
        {
            
            // Prepara view
            TRANSPORTADORA item = tranApp.GetItemById(SessionMocks.idVolta);
            objetoTranAntes = item;
            TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult BuscarCEPTransportadora()
        {
            
            // Prepara view
            if (SessionMocks.voltaCEP == 1)
            {
                TRANSPORTADORA item = SessionMocks.transportadora;
                TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(item);
                vm.TRAN_NR_CEP_BUSCA = String.Empty;
                return View(vm);
            }
            else
            {
                TransportadoraViewModel vm = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(SessionMocks.transportadora);
                vm.TRAN_NR_CEP_BUSCA = String.Empty;
                return View(vm);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult BuscarCEPTransportadora(TransportadoraViewModel vm)
        {
            
            try
            {
                // Atualiza cliente
                TRANSPORTADORA item = SessionMocks.transportadora;
                TRANSPORTADORA cli = new TRANSPORTADORA();
                cli.ASSI_CD_ID = item.ASSI_CD_ID;
                cli.ASSI_CD_ID = item.ASSI_CD_ID;
                cli.MATR_CD_ID = item.MATR_CD_ID;
                cli.FILI_CD_ID = item.FILI_CD_ID;
                cli.TRAN_AQ_LOGO = item.TRAN_AQ_LOGO;
                cli.TRAN_CD_ID = item.TRAN_CD_ID;
                cli.TRAN_DS_INFORMACOES_GERAIS = item.TRAN_DS_INFORMACOES_GERAIS;
                cli.TRAN_DT_CADASTRO = item.TRAN_DT_CADASTRO;
                cli.TRAN_IN_ATIVO = item.TRAN_IN_ATIVO;
                cli.TRAN_NM_BAIRRO = item.TRAN_NM_BAIRRO;
                cli.TRAN_NM_CIDADE = item.TRAN_NM_CIDADE;
                cli.TRAN_NM_CONTATOS = item.TRAN_NM_CONTATOS;
                cli.TRAN_NM_EMAIL = item.TRAN_NM_EMAIL;
                cli.TRAN_NM_ENDERECO = item.TRAN_NM_ENDERECO;
                cli.TRAN_NM_NOME = item.TRAN_NM_NOME;
                cli.TRAN_NM_RAZAO = item.TRAN_NM_RAZAO;
                cli.TRAN_NM_WEBSITE = item.TRAN_NM_WEBSITE;
                cli.TRAN_NR_CEP = item.TRAN_NR_CEP;
                cli.TRAN_NR_CNPJ = item.TRAN_NR_CNPJ;
                cli.TRAN_NR_INSCRICAO_ESTADUAL = item.TRAN_NR_INSCRICAO_ESTADUAL;
                cli.TRAN_NR_INSCRICAO_MUNICIPAL = item.TRAN_NR_INSCRICAO_MUNICIPAL;
                cli.TRAN_NR_TELEFONES = item.TRAN_NR_TELEFONES;
                cli.TRAN_SG_UF = item.TRAN_SG_UF;
                cli.TRAN_TX_OBSERVACOES = item.TRAN_TX_OBSERVACOES;

                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                Int32 volta = tranApp.ValidateEdit(cli, cli);

                // Verifica retorno

                // Sucesso
                listaMasterTran = new List<TRANSPORTADORA>();
                SessionMocks.listaTransportadora = null;
                return RedirectToAction("EditarTransportadora", new { id = SessionMocks.idVolta });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public ActionResult PesquisaCEP(TransportadoraViewModel itemVolta)
        {
            
            // Chama servico ECT
            TRANSPORTADORA cli = tranApp.GetItemById(SessionMocks.idVolta);
            TransportadoraViewModel item = Mapper.Map<TRANSPORTADORA, TransportadoraViewModel>(cli);

            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            String cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(itemVolta.TRAN_NR_CEP_BUSCA);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza            
            item.TRAN_NM_ENDERECO = end.Address + "/" + end.Complement;
            item.TRAN_NM_BAIRRO = end.District;
            item.TRAN_NM_CIDADE = end.City;
            item.TRAN_SG_UF = end.Uf;
            item.TRAN_NR_CEP = itemVolta.TRAN_NR_CEP_BUSCA;

            // Retorna
            SessionMocks.voltaCEP = 2;
            SessionMocks.transportadora = Mapper.Map<TransportadoraViewModel, TRANSPORTADORA>(item);
            return RedirectToAction("BuscarCEPTransportadora");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "TransportadoraLista" + "_" + data + ".pdf";
            List<TRANSPORTADORA> lista = SessionMocks.listaTransportadora;
            TRANSPORTADORA filtro = SessionMocks.filtroTransportadora;
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

            cell = new PdfPCell(new Paragraph("Transportadoras - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 120f, 60f, 120f, 120f, 80f, 50f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Transportadoras selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 6;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CNPJ", meuFont))
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
            cell = new PdfPCell(new Paragraph("Contatos", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("UF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (TRANSPORTADORA item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.TRAN_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TRAN_NR_CNPJ, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TRAN_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TRAN_NM_CONTATOS, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TRAN_NM_CIDADE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TRAN_SG_UF, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
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
                if (filtro.TRAN_NM_NOME != null)
                {
                    parametros += "Nome: " + filtro.TRAN_NM_NOME;
                    ja = 1;
                }
                if (filtro.TRAN_NR_CNPJ != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CNPJ: " + filtro.TRAN_NR_CNPJ;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e CNPJ: " + filtro.TRAN_NR_CNPJ;
                    }
                }
                if (filtro.TRAN_NM_EMAIL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "E-Mail: " + filtro.TRAN_NM_EMAIL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e E-Mail: " + filtro.TRAN_NM_EMAIL;
                    }
                }
                if (filtro.TRAN_NM_CONTATOS != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Contatos: " + filtro.TRAN_NM_CONTATOS;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Contatos: " + filtro.TRAN_NM_CONTATOS;
                    }
                }
                if (filtro.TRAN_NM_CIDADE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Cidade: " + filtro.TRAN_NM_CIDADE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Cidade: " + filtro.TRAN_NM_CIDADE;
                    }
                }
                if (filtro.TRAN_SG_UF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "UF: " + filtro.TRAN_SG_UF;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e UF: " + filtro.TRAN_SG_UF;
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

            return RedirectToAction("MontarTelaTransportadora");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            // Prepara geração
            TRANSPORTADORA tran = tranApp.GetById(SessionMocks.idVolta);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Transportadora" + tran.TRAN_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

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

            cell = new PdfPCell(new Paragraph("Transportadora - Detalhes", meuFont2))
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

            cell = new PdfPCell(new Paragraph("Logotipo da Transportadora", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            try
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                Image imagemCliente = Image.GetInstance(Server.MapPath(tran.TRAN_AQ_LOGO));
                imagemCliente.ScaleAbsolute(50, 50);
                cell.AddElement(imagemCliente);
                table.AddCell(cell);
            }
            catch (Exception ex)
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                Image imagemCliente = Image.GetInstance(Server.MapPath("~/Images/a8.jpg"));
                imagemCliente.ScaleAbsolute(50, 50);
                cell.AddElement(imagemCliente);
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Filial: " + tran.FILIAL.FILI_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.TIPO_VEICULO != null)
            {
                cell = new PdfPCell(new Paragraph("Tipo de Veículo: " + tran.TIPO_VEICULO.TIVE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (tran.TIPO_TRANSPORTE != null)
            {
                cell = new PdfPCell(new Paragraph("Tipo de Transporte: " + tran.TIPO_TRANSPORTE.TITR_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Nome: " + tran.TRAN_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Razão Social: " + tran.TRAN_NM_RAZAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("CNPJ: " + tran.TRAN_NR_CNPJ, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Inscrição Estadual: " + tran.TRAN_NR_INSCRICAO_ESTADUAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Inscrição Estadual: " + tran.TRAN_NR_INSCRICAO_ESTADUAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Inscrição Municipal: " + tran.TRAN_NR_INSCRICAO_MUNICIPAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Email: " + tran.TRAN_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Informações Gerais: " + tran.TRAN_DS_INFORMACOES_GERAIS, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Área de Cobertura: " + tran.TRAN_DS_AREA_COBERTURA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Transportes Especiais: " + tran.TRAN_DS_TRANSPORTE_ESPECIAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Seguros: " + tran.TRAN_DS_SEGURO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Endereços
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Endereço Principal", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço: " + tran.TRAN_NM_ENDERECO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Bairro: " + tran.TRAN_NM_BAIRRO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cidade: " + tran.TRAN_NM_CIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (tran.TRAN_SG_UF != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + tran.TRAN_SG_UF, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Contatos
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Contatos", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Email: " + tran.TRAN_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Website: " + tran.TRAN_NM_WEBSITE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Telefones: " + tran.TRAN_NR_TELEFONES, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Contatos: " + tran.TRAN_NM_CONTATOS, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + tran.TRAN_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
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

            return RedirectToAction("VoltarAnexoTransportadora");
        }

    }
}