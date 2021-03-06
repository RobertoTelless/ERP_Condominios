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
    public class EntradaSaidaAppService : AppServiceBase<ENTRADA_SAIDA>, IEntradaSaidaAppService
    {
        private readonly IEntradaSaidaService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly ITemplateService _temService;
        private readonly IConfiguracaoService _confService;

        public EntradaSaidaAppService(IEntradaSaidaService baseService, INotificacaoService notiService, ITemplateService temService, IConfiguracaoService confService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _temService = temService;
            _confService = confService;
        }

        public List<ENTRADA_SAIDA> GetAllItens(Int32 idAss)
        {
            List<ENTRADA_SAIDA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<ENTRADA_SAIDA> GetAllItensAdm(Int32 idAss)
        {
            List<ENTRADA_SAIDA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<ENTRADA_SAIDA> GetByUnidade(Int32 idUnid)
        {
            return _baseService.GetByUnidade(idUnid);
        }

        public List<ENTRADA_SAIDA> GetItemByData(DateTime data, Int32 idAss)
        {
            return _baseService.GetByData(data, idAss);
        }

        public ENTRADA_SAIDA GetItemById(Int32 id)
        {
            ENTRADA_SAIDA item = _baseService.GetItemById(id);
            return item;
        }

        public List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss)
        {
            List<GRAU_PARENTESCO> lista = _baseService.GetAllGraus(idAss);
            return lista;
        }

        public List<AUTORIZACAO_ACESSO> GetAllAutorizacoes(Int32 idAss)
        {
            List<AUTORIZACAO_ACESSO> lista = _baseService.GetAllAutorizacoes(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllUnidades(idAss);
            return lista;
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss)
        {
            List<CATEGORIA_NOTIFICACAO> lista = _baseService.GetAllCatNotificacao(idAss);
            return lista;
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            List<USUARIO> lista = _baseService.GetAllUsuarios(idAss);
            return lista;
        }

        public Int32 ExecuteFilter(String nome, String documento, Int32? unid, Int32? autorizacao, DateTime? dataEntrada, DateTime? dataSaida, Int32? status, Int32 idAss, out List<ENTRADA_SAIDA> objeto)
        {
            try
            {
                objeto = new List<ENTRADA_SAIDA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, documento, unid, autorizacao, dataEntrada, dataSaida, status, idAss);
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


        public Int32 ValidateCreate(ENTRADA_SAIDA item, USUARIO usuario)
        {
            try
            {

                // Verifica existencia pr??via

                // Completa objeto
                item.ENSA_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                //Verifica Campos
                if (item.GRAU_PARENTESCO != null)
                {
                    item.GRAU_PARENTESCO = null;
                }
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
                    LOG_NM_OPERACAO = "AddENSA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENTRADA_SAIDA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ENTRADA_SAIDA item, ENTRADA_SAIDA itemAntes, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.ENSA_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                //Verifica Campos
                if (item.GRAU_PARENTESCO != null)
                {
                    item.GRAU_PARENTESCO = null;
                }
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
                    LOG_NM_OPERACAO = "EditENSA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENTRADA_SAIDA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<ENTRADA_SAIDA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ENTRADA_SAIDA item, ENTRADA_SAIDA itemAntes)
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

        public Int32 ValidateDelete(ENTRADA_SAIDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ENSA_IN_ATIVO = 0;
                if (item.GRAU_PARENTESCO != null)
                {
                    item.GRAU_PARENTESCO = null;
                }
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
                    LOG_NM_OPERACAO = "DeleENSA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENTRADA_SAIDA>(item),
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(ENTRADA_SAIDA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ENSA_IN_ATIVO = 1;
                if (item.GRAU_PARENTESCO != null)
                {
                    item.GRAU_PARENTESCO = null;
                }
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
                    LOG_NM_OPERACAO = "ReatENSA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ENTRADA_SAIDA>(item),
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, ENTRADA_SAIDA entrada, String template)
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
                    footer = footer.Replace("{Visita}", entrada.ENSA_NM_NOME);
                    footer = footer.Replace("{Unidade}", usuario.UNIDADE.UNID_NM_EXIBE);
                    footer = footer.Replace("{Data}", item.NOTI_DT_EMISSAO.Value.ToShortDateString());
                    body = body.Replace("{Texto}", item.NOTI_TX_TEXTO);
                    header = header.Replace("{Nome}", usuario.USUA_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "NOTIFICA????O - ENTRADA DE VE??CULO";
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
                String texto = _temService.GetByCode("VEICSMS", usuario.ASSI_CD_ID).TEMP_TX_CORPO;

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
