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
    public class ReservaAppService : AppServiceBase<RESERVA>, IReservaAppService
    {
        private readonly IReservaService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly ITemplateService _temService;
        private readonly IConfiguracaoService _confService;
        private readonly IUsuarioAppService _usuService;
        private readonly IAmbienteService _ambService;

        public ReservaAppService(IReservaService baseService, INotificacaoService notiService, ITemplateService temService, IConfiguracaoService confService, IUsuarioAppService usuService, IAmbienteService ambService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _temService = temService;
            _confService = confService;
            _usuService = usuService;
            _ambService = ambService;
        }

        public RESERVA CheckExist(RESERVA unid, Int32 idAss)
        {
            RESERVA item = _baseService.CheckExist(unid, idAss);
            return item;
        }

        public List<RESERVA> GetAllItens(Int32 idAss)
        {
            List<RESERVA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<RESERVA> GetAllItensAdm(Int32 idAss)
        {
            List<RESERVA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss)
        {
            List<CATEGORIA_NOTIFICACAO> lista = _baseService.GetAllCatNotificacao(idAss);
            return lista;
        }

        public List<RESERVA> GetByUnidade(Int32 idUnid)
        {
            return _baseService.GetByUnidade(idUnid);
        }

        public List<RESERVA> GetItemByData(DateTime data, Int32 idAss)
        {
            return _baseService.GetByData(data, idAss);
        }

        public RESERVA GetItemById(Int32 id)
        {
            RESERVA item = _baseService.GetItemById(id);
            return item;
        }

        public List<FINALIDADE_RESERVA> GetAllFinalidades(Int32 idAss)
        {
            List<FINALIDADE_RESERVA> lista = _baseService.GetAllFinalidades(idAss);
            return lista;
        }

        public List<AMBIENTE> GetAllAmbientes(Int32 idAss)
        {
            List<AMBIENTE> lista = _baseService.GetAllAmbientes(idAss);
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

        public RESERVA_ANEXO GetAnexoById(Int32 id)
        {
            RESERVA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public RESERVA_COMENTARIO GetComentarioById(Int32 id)
        {
            RESERVA_COMENTARIO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(String nome, DateTime? data, Int32? finalidade, Int32? ambiente, Int32? unidade, Int32? status, Int32 idAss, out List<RESERVA> objeto)
        {
            try
            {
                objeto = new List<RESERVA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, data, finalidade, ambiente, unidade, status, idAss);
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


        public Int32 ValidateCreate(RESERVA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Verifica limite de convidados
                AMBIENTE ambi = _ambService.GetItemById(item.AMBI_CD_ID);
                if (item.RESE_NR_CONVIDADOS > ambi.AMBI_NR_LOTACAO)
                {
                    return 2;
                }

                // Verifica horario livre
                List<RESERVA> lista = _baseService.GetAllItens(usuario.ASSI_CD_ID);
                lista = lista.Where(p => p.AMBI_CD_ID == ambi.AMBI_CD_ID & p.RESE_DT_EVENTO == item.RESE_DT_EVENTO).ToList();
                foreach (RESERVA res in lista)
                {
                    if (res.RESE_HR_INICIO >= item.RESE_HR_INICIO & res.RESE_HR_FINAL <= item.RESE_HR_FINAL)
                    {

                    }
                }





                // Completa objeto
                item.RESE_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.RESE_DT_CADASTRO = DateTime.Today.Date;
                item.RESE_IN_ACEITA = 0;
                item.RESE_IN_STATUS = 1;
                item.RESE_IN_BOLETO = 0;
                item.RESE_IN_CONFIRMADA = 0;
                item.RESE_IN_LEMBRAR = 0;
                item.RESE_IN_PAGA = 0;

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
                if (item.AMBIENTE != null)
                {
                    item.AMBIENTE = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddRESE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<RESERVA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera Notificações
                USUARIO usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.PERFIL.PERF_SG_SIGLA == "SIN").ToList().FirstOrDefault();
                if (usua == null)
                {
                    usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.PERFIL.PERF_SG_SIGLA == "ADM").ToList().FirstOrDefault();
                }
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
                noti.NOTI_NM_TITULO = "NOTIFICAÇÃO - RESERVA";
                noti.NOTI_TX_TEXTO = "A Unidade: " + usuario.UNIDADE.UNID_NR_NUMERO.ToString() + " abriu uma solicitação de reserva para " + item.RESE_DT_EVENTO.ToShortDateString() + " no ambiente " + ambi.AMBI_NM_AMBIENTE + ".";
                noti.USUA_CD_ID = usua.USUA_CD_ID;
                Int32 volta1 = GerarNotificacao(noti, usua, item, "NOTIRESE");
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(RESERVA item, RESERVA itemAntes, USUARIO usuario)
        {
            try
            {
                // Criticas
                if (item.RESE_IN_STATUS == 2)
                {
                    if (item.RESE_DT_APROVACAO == null)
                    {
                        return 3;
                    }
                    if (item.RESE_DT_APROVACAO.Value.Date > DateTime.Today.Date)
                    {
                        return 1;
                    }
                    if (item.RESE_DT_APROVACAO.Value.Date > item.RESE_DT_EVENTO.Date)
                    {
                        return 2;
                    }
                }
                if (item.RESE_IN_STATUS == 3)
                {
                    if (item.RESE_DT_VETADA == null)
                    {
                        return 4;
                    }
                    if (item.RESE_DT_VETADA.Value.Date > DateTime.Today.Date)
                    {
                        return 5;
                    }
                    if (item.RESE_DT_VETADA.Value.Date > item.RESE_DT_EVENTO.Date)
                    {
                        return 6;
                    }
                }
                if (item.RESE_IN_STATUS == 4)
                {
                    if (item.RESE_DT_CONFIRMACAO == null)
                    {
                        return 7;
                    }
                    if (item.RESE_DT_CONFIRMACAO.Value.Date > DateTime.Today.Date)
                    {
                        return 8;
                    }
                    if (item.RESE_DT_CONFIRMACAO.Value.Date > item.RESE_DT_EVENTO.Date)
                    {
                        return 9;
                    }
                }

                // Preparação

                // Completa objeto
                item.RESE_IN_ATIVO = 1;
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
                    LOG_NM_OPERACAO = "EditRESE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<RESERVA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<RESERVA>(itemAntes)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                // Gera Notificações
                if (item.RESE_IN_STATUS > 1 & item.RESE_IN_STATUS != itemAntes.RESE_IN_STATUS)
                {
                    USUARIO usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID & p.USUA_IN_RESPONSAVEL == 1).ToList().FirstOrDefault();
                    if (usua == null)
                    {
                        usua = _usuService.GetAllUsuarios(usuario.ASSI_CD_ID).Where(p => p.UNID_CD_ID == usuario.UNID_CD_ID).ToList().FirstOrDefault();
                    }

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
                    noti.NOTI_NM_TITULO = "NOTIFICAÇÃO - RESERVA";
                    if (item.RESE_IN_STATUS == 2)
                    {
                        noti.NOTI_TX_TEXTO = "A reserva solicitada pela unidade " + item.UNIDADE.UNID_NR_NUMERO.ToString() + " para " + item.RESE_DT_EVENTO.ToShortDateString() + " no ambiente " + item.AMBIENTE.AMBI_NM_AMBIENTE + " foi APROVADA. O boleto para pagamento será enviado.";
                    }
                    else if (item.RESE_IN_STATUS == 3)
                    {
                        noti.NOTI_TX_TEXTO = "A reserva solicitada pela unidade " + item.UNIDADE.UNID_NR_NUMERO.ToString() + " para " + item.RESE_DT_EVENTO.ToShortDateString() + " no ambiente " + item.AMBIENTE.AMBI_NM_AMBIENTE + " foi VETADA.";
                    }
                    else if (item.RESE_IN_STATUS == 4)
                    {
                        noti.NOTI_TX_TEXTO = "A reserva solicitada pela unidade " + item.UNIDADE.UNID_NR_NUMERO.ToString() + " para " + item.RESE_DT_EVENTO.ToShortDateString() + " no ambiente " + item.AMBIENTE.AMBI_NM_AMBIENTE + " foi CONFIRMADA.";
                    }
                    noti.USUA_CD_ID = usua.USUA_CD_ID;
                    Int32 volta1 = GerarNotificacao(noti, usua, item, "NOTIRESE");
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(RESERVA item, RESERVA itemAntes)
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

        public Int32 ValidateDelete(RESERVA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.RESE_IN_STATUS != 1)
                {
                    return 1;
                }

                // Acerta campos
                item.RESE_IN_ATIVO = 0;
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
                    LOG_NM_OPERACAO = "DeleRESE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<RESERVA>(item),
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(RESERVA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.RESE_IN_ATIVO = 1;
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
                    LOG_NM_OPERACAO = "ReatRESE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<RESERVA>(item),
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, RESERVA entrada, String template)
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
                    footer = footer.Replace("{Ambiente}", entrada.AMBIENTE.AMBI_NM_AMBIENTE);
                    footer = footer.Replace("{Unidade}", entrada.UNIDADE.UNID_NM_EXIBE);
                    footer = footer.Replace("{Data}", entrada.RESE_DT_EVENTO.ToShortDateString());
                    body = body.Replace("{Texto}", item.NOTI_TX_TEXTO);
                    header = header.Replace("{Nome}", usuario.USUA_NM_NOME);
                    body = body.Replace("{Condominio}", usuario.ASSINANTE.ASSI_NM_NOME);

                    // Concatena
                    String emailBody = header + body;
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "NOTIFICAÇÃO - RESERVA";
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

        public String ValidateCreateMensagem(USUARIO usuario, RESERVA enco, NOTIFICACAO noti)
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
                String body = _temService.GetByCode("RESESMS").TEMP_TX_CORPO;
                String header = _temService.GetByCode("RESESMS").TEMP_TX_CABECALHO;
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
