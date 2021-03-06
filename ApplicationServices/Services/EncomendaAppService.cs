using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;


namespace ApplicationServices.Services
{
    public class EncomendaAppService : AppServiceBase<ENCOMENDA>, IEncomendaAppService
    {
        private readonly IEncomendaService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly ITemplateService _temService;
        private readonly IConfiguracaoService _confService;
        private readonly IUsuarioAppService _usuService;
        private readonly IUnidadeService _uniService;

        public EncomendaAppService(IEncomendaService baseService, INotificacaoService notiService, ITemplateService temService, IConfiguracaoService confService, IUsuarioAppService usuService, IUnidadeService uniService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _temService = temService;
            _confService = confService;
            _usuService = usuService;
            _uniService = uniService;
        }

        public List<ENCOMENDA> GetAllItens(Int32 idAss)
        {
            List<ENCOMENDA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<ENCOMENDA> GetAllItensAdm(Int32 idAss)
        {
            List<ENCOMENDA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<ENCOMENDA> GetByUnidade(Int32 idUnid)
        {
            return _baseService.GetByUnidade(idUnid);
        }

        public List<ENCOMENDA> GetItemByData(DateTime data, Int32 idAss)
        {
            return _baseService.GetByData(data, idAss);
        }

        public ENCOMENDA GetItemById(Int32 id)
        {
            ENCOMENDA item = _baseService.GetItemById(id);
            return item;
        }

        public List<FORMA_ENTREGA> GetAllFormas(Int32 idAss)
        {
            List<FORMA_ENTREGA> lista = _baseService.GetAllFormas(idAss);
            return lista;
        }

        public List<TIPO_ENCOMENDA> GetAllTipos(Int32 idAss)
        {
            List<TIPO_ENCOMENDA> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllUnidades(idAss);
            return lista;
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            List<USUARIO> lista = _baseService.GetAllUsuarios(idAss);
            return lista;
        }

        public ENCOMENDA_ANEXO GetAnexoById(Int32 id)
        {
            ENCOMENDA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public ENCOMENDA_COMENTARIO GetComentarioById(Int32 id)
        {
            ENCOMENDA_COMENTARIO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? unid, Int32? forma, Int32? tipo, DateTime? data, Int32? status, Int32 idAss, out List<ENCOMENDA> objeto)
        {
            try
            {
                objeto = new List<ENCOMENDA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(unid, forma, tipo, data, status, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateCreate(ENCOMENDA item, USUARIO usuario)
        {
            try
            {

                // Verifica existencia pr??via
                USUARIO usu = null;
                if (item.USUARIO != null)
                {
                    usu = item.USUARIO;
                }
                else
                {
                    USUARIO usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.UNID_CD_ID == item.UNID_CD_ID & p.USUA_IN_RESPONSAVEL == 1).ToList().FirstOrDefault();
                    if (usua == null)
                    {
                        usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.UNID_CD_ID == item.UNID_CD_ID).ToList().FirstOrDefault();
                    }
                    usu = usua;       
                }
                String unid = usu.UNIDADE.UNID_NM_EXIBE;

                // Completa objeto
                item.ENCO_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.ENCO_CD_CODIGO = Cryptography.GenerateRandomPassword(6);
                item.ENCO_IN_CONFIRMADO = 0;
                item.ENCO_IN_STATUS = 1;
                item.USUA_CD_ID = usu.USUA_CD_ID;

                //Verifica Campos
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddENCO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENCOMENDA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera Notifica????es
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.CANO_CD_ID = 1;
                noti.NOTI_DT_EMISSAO = DateTime.Today.Date;
                noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTI_IN_ATIVO = 1;
                noti.NOTI_IN_NIVEL = 1;
                noti.NOTI_IN_ORIGEM = 1;
                noti.NOTI_IN_STATUS = 1;
                noti.NOTI_IN_VISTA = 0;
                noti.NOTI_NM_TITULO = "NOTIFICA????O - ENCOMENDA";
                noti.NOTI_TX_TEXTO = "Unidade: " + unid + ". Uma encomenda chegou para essa unidade em " + item.ENCO_DT_CHEGADA.Value.ToShortDateString() + ". Est?? dispon??vel para entrega na portaria. C??digo: " + item.ENCO_CD_CODIGO + ".";
                noti.USUA_CD_ID = usu.USUA_CD_ID;
                Int32 volta1 = GerarNotificacao(noti, usu, item, unid, "NOTIENCO");
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ENCOMENDA item, ENCOMENDA itemAntes, USUARIO usuario)
        {
            try
            {
                // Criticas
                if (item.ENCO_IN_STATUS == 2)
                {
                    if (item.ENCO_DT_ENTREGA == null || item.ENCO_NM_PESSOA == null)
                    {
                        return 1;
                    }
                }
                if (item.ENCO_IN_STATUS == 4)
                {
                    if (item.ENCO_DT_DEVOLUCAO == null || item.ENCO_DS_JUSTIFICATIVA == null)
                    {
                        return 2;
                    }
                }
                if (item.ENCO_IN_STATUS == 3)
                {
                    if (item.ENCO_DT_RECUSA == null || item.ENCO_DS_JUSTIFICATIVA_RECUSA == null)
                    {
                        return 3;
                    }
                }

                // Prepara????o
                String unid = _uniService.GetItemById(item.UNID_CD_ID.Value).UNID_NM_EXIBE;
                USUARIO usu = null;
                if (item.USUARIO != null)
                {
                    usu = item.USUARIO;
                }
                else
                {
                    USUARIO usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.UNID_CD_ID == item.UNID_CD_ID & p.USUA_IN_RESPONSAVEL == 1).ToList().FirstOrDefault();
                    if (usua == null)
                    {
                        usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.UNID_CD_ID == item.UNID_CD_ID).ToList().FirstOrDefault();
                    }
                    usu = usua;
                }

                // Completa objeto
                item.ENCO_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                //Verifica Campos
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditENCO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENCOMENDA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<ENCOMENDA>(itemAntes)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                // Gera Notifica????es
                if (item.ENCO_IN_STATUS == 2 || item.ENCO_IN_STATUS == 3)
                {
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.CANO_CD_ID = 1;
                    noti.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_ORIGEM = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.NOTI_IN_VISTA = 0;
                    if (item.ENCO_IN_STATUS == 2)
                    {
                        noti.NOTI_NM_TITULO = "NOTIFICA????O - ENCOMENDA - ENTREGA";
                        noti.NOTI_TX_TEXTO = "Unidade: " + unid + ". A encomenda recebida em " + item.ENCO_DT_CHEGADA.Value.ToShortDateString() + " e com c??digo: " + item.ENCO_CD_CODIGO + ", foi entregue em " + item.ENCO_DT_ENTREGA.Value.ToShortDateString();
                    }
                    else
                    {
                        noti.NOTI_NM_TITULO = "NOTIFICA????O - ENCOMENDA - RECUSA";
                        noti.NOTI_TX_TEXTO = "Unidade: " + unid + ". A encomenda recebida em " + item.ENCO_DT_CHEGADA.Value.ToShortDateString() + " e com c??digo: " + item.ENCO_CD_CODIGO + ", foi recusada em " + item.ENCO_DT_RECUSA.Value.ToShortDateString();
                    }


                    noti.USUA_CD_ID = usu.USUA_CD_ID;
                    Int32 volta1 = GerarNotificacao(noti, usu, item, unid, "NOTIENCO");
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ENCOMENDA item, ENCOMENDA itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(ENCOMENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ENCO_IN_ATIVO = 0;
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleENCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENCOMENDA>(item),
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ENCOMENDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ENCO_IN_ATIVO = 1;
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.UNIDADE != null)
                {
                    item.UNIDADE = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatENCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENCOMENDA>(item),
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, ENCOMENDA entrada, String unid, String template)
        {
            try
            {
                // Verifica????o

                // Completa objeto

                // Monta Log

                // Gera Notificacoes
                Int32 volta = 0;
                if (item.USUA_CD_ID != 0)
                {
                    // Gera Notifica????o
                    volta = _notiService.Create(item);

                    // Recupera template e-mail
                    String header = _temService.GetByCode(template, usuario.ASSI_CD_ID).TEMP_TX_CABECALHO;
                    String body = _temService.GetByCode(template, usuario.ASSI_CD_ID).TEMP_TX_CORPO;
                    String footer = _temService.GetByCode(template, usuario.ASSI_CD_ID).TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    String frase = String.Empty;
                    footer = footer.Replace("{Codigo}", entrada.ENCO_CD_CODIGO);
                    footer = footer.Replace("{Unidade}", unid);
                    footer = footer.Replace("{Data}", entrada.ENCO_DT_CHEGADA.Value.ToShortDateString());
                    body = body.Replace("{Texto}", item.NOTI_TX_TEXTO);
                    header = header.Replace("{Nome}", usuario.USUA_NM_NOME);
                    body = body.Replace("{Condominio}", usuario.ASSINANTE.ASSI_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "NOTIFICA????O - ENCOMENDA";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = usuario.USUA_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    // Envia mensagem
                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);

                    // Envia SMS
                    String voltaSMS = ValidateCreateMensagem(usuario, entrada, item);
                    return volta;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public String ValidateCreateMensagem(USUARIO usuario, ENCOMENDA enco, NOTIFICACAO noti)
        {
            try
            {
                // Criticas
                if (usuario.USUA_NR_CELULAR == null)
                {
                    return "1";
                }

                // Monta token
                CONFIGURACAO conf = _confService.GetItemById(1);
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Monta routing
                String routing = "1";

                // Monta texto
                String msg = noti.NOTI_TX_TEXTO.Replace(System.Environment.NewLine, " ");
                if (msg.Length > 750)
                {
                    msg = msg.Substring(0, 750);
                }
                String body = _temService.GetByCode("ENCOSMS", usuario.ASSI_CD_ID).TEMP_TX_CORPO;
                String header = _temService.GetByCode("ENCOSMS", usuario.ASSI_CD_ID).TEMP_TX_CABECALHO;
                header = header.Replace("{Nome}", usuario.USUA_NM_NOME);
                body = body.Replace("{Texto}", msg);
                body = body.Replace("{Condominio}", usuario.ASSINANTE.ASSI_NM_NOME);
                String texto = header + body;

                // inicia processo
                List<String> resposta = new List<string>();
                WebRequest request = WebRequest.Create("https://api.smsfire.com.br/v1/sms/send");
                request.Headers["Authorization"] = auth;
                request.Method = "POST";
                request.ContentType = "application/json";

                // Monta destinatarios
                String listaDest = "55" + Regex.Replace(usuario.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();

                // Processa lista
                String responseFromServer = null;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    String campanha = "ERP_Condominio";
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

                // Sa??da
                reader.Close();
                response.Close();
                return responseFromServer;
            }
            catch (Exception ex)
            {
                return "3";
            }
        }
    }
}
