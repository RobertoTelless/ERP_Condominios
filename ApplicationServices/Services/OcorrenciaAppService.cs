using System;
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
    public class OcorrenciaAppService : AppServiceBase<OCORRENCIA>, IOcorrenciaAppService
    {
        private readonly IOcorrenciaService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly IUnidadeService _uniService;
        private readonly ITemplateService _temService;
        private readonly IConfiguracaoService _confService;

        public OcorrenciaAppService(IOcorrenciaService baseService, INotificacaoService notiService, IUnidadeService uniService, ITemplateService temService, IConfiguracaoService confService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _uniService = uniService;
            _temService = temService;
            _confService = confService;
        }

        public List<OCORRENCIA> GetAllItens(Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<OCORRENCIA> GetAllItensAdm(Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CATEGORIA_OCORRENCIA> GetAllCategorias(Int32 idAss)
        {
            List<CATEGORIA_OCORRENCIA> lista = _baseService.GetAllCategorias(idAss);
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

        public OCORRENCIA_ANEXO GetAnexoById(Int32 id)
        {
            OCORRENCIA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public OCORRENCIA GetItemById(Int32 id)
        {
            OCORRENCIA item = _baseService.GetItemById(id);
            return item;
        }

        public List<OCORRENCIA> GetAllItensUser(Int32 id, Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItensUser(id, idAss);
            return lista;
        }

        public List<OCORRENCIA> GetAllItensUnidade(Int32 id, Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItensUnidade(id, idAss);
            return lista;
        }

        public List<OCORRENCIA> GetOcorrenciasNovas(Int32 id, Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetOcorrenciasNovas(id, idAss);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? unidade, Int32? usuario, Int32? cat,String titulo, DateTime? data, String texto, Int32 idAss, out List<OCORRENCIA> objeto)
        {
            try
            {
                objeto = new List<OCORRENCIA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(unidade, usuario, cat, titulo, data, texto, idAss);
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

        public Int32 ValidateCreate(OCORRENCIA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.OCOR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddOCOR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera Notificação
                if (item.UNID_CD_ID != null)
                {
                    UNIDADE unid = _uniService.GetItemById(item.UNID_CD_ID.Value);
                    foreach (USUARIO usu in unid.USUARIO)
                    {
                        NOTIFICACAO noti = new NOTIFICACAO();
                        noti.CANO_CD_ID = 5;
                        noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        noti.NOTI_DT_EMISSAO = DateTime.Today;
                        noti.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                        noti.NOTI_IN_VISTA = 0;
                        noti.NOTI_NM_TITULO = "Registro de Ocorrência";
                        noti.NOTI_IN_ATIVO = 1;
                        noti.NOTI_TX_TEXTO = "ATENÇÃO: Foi registrada uma ocorrência para sua unidade em " + DateTime.Today.Date.ToLongDateString() + ". Favor tomar conhecimento";
                        noti.USUA_CD_ID = usu.USUA_CD_ID;
                        noti.NOTI_IN_STATUS = 1;
                        noti.NOTI_IN_NIVEL = 1;
                        Int32 volta1 = _notiService.Create(noti);
                    }
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(OCORRENCIA item, OCORRENCIA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditOCOR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<OCORRENCIA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(OCORRENCIA item, OCORRENCIA itemAntes)
        {
            try
            {

                // Persiste
                item.USUARIO = null;
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(OCORRENCIA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.OCOR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelOCOR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(OCORRENCIA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.OCOR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatOCOR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, OCORRENCIA ocorrencia, String template)
        {
            try
            {
                // Verificação

                // Completa objeto

                // Monta Log

                // Gera Notificacoes
                Int32 volta = 0;
                if (item.USUA_CD_ID != 0)
                {
                    // Gera Notificação
                    volta = _notiService.Create(item);

                    // Recupera template e-mail
                    String header = _temService.GetByCode(template).TEMP_TX_CABECALHO;
                    String body = _temService.GetByCode(template).TEMP_TX_CORPO;
                    String footer = _temService.GetByCode(template).TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    String frase = String.Empty;
                    body = body.Replace("{data}", ocorrencia.OCOR_DT_OCORRENCIA.ToShortDateString());
                    body = body.Replace("{titulo}", ocorrencia.OCOR_NM_TITULO);
                    body = body.Replace("{texto}", ocorrencia.OCOR_TX_TEXTO);
                    header = header.Replace("{unidade}", ocorrencia.UNIDADE.UNID_NM_EXIBE);;

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "NOTIFICAÇÃO - OCORRÊNCIAS";
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
                    String voltaSMS = ValidateCreateMensagem(usuario);
                    return volta;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public String ValidateCreateMensagem(USUARIO usuario)
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
                String texto = _temService.GetByCode("OCORSMS").TEMP_TX_CORPO;
                texto = usuario.USUA_NM_NOME;

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

                // Saída
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
