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
    public class UnidadeAppService : AppServiceBase<UNIDADE>, IUnidadeAppService
    {
        private readonly IUnidadeService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly ITemplateService _temService;
        private readonly IConfiguracaoService _confService;
        private readonly IUsuarioService _usuService;

        public UnidadeAppService(IUnidadeService baseService, INotificacaoService notiService, ITemplateService temService, IConfiguracaoService confService, IUsuarioService usuService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _temService = temService;
            _confService = confService;
            _usuService = usuService;
        }

        public UNIDADE CheckExist(UNIDADE unid, Int32 idAss)
        {
            UNIDADE item = _baseService.CheckExist(unid, idAss);
            return item;
        }

        public List<UNIDADE> GetAllItens(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllItensAdm(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public UNIDADE GetItemById(Int32 id)
        {
            UNIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public TORRE GetTorreById(Int32 id)
        {
            TORRE item = _baseService.GetTorreById(id);
            return item;
        }

        public UNIDADE_ANEXO GetAnexoById(Int32 id)
        {
            UNIDADE_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public List<TIPO_UNIDADE> GetAllTipos(Int32 idAss)
        {
            List<TIPO_UNIDADE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<TORRE> GetAllTorres(Int32 idAss)
        {
            List<TORRE> lista = _baseService.GetAllTorres(idAss);
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

        public Int32 ExecuteFilter(String numero, Int32? torre, Int32? idTipo, Int32? alugada, Int32 idAss, out List<UNIDADE> objeto)
        {
            try
            {
                objeto = new List<UNIDADE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(numero, torre, idTipo, alugada, idAss);
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


        public Int32 ValidateCreate(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.UNID_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                if (item.TORR_CD_ID != 0)
                {
                    TORRE torre = _baseService.GetTorreById(item.TORR_CD_ID);
                    item.UNID_NM_NOME_TORRE = torre.TORR_NM_NOME;
                }
                else
                {
                    item.UNID_NM_NOME_TORRE = "Única";
                }

                //Verifica Campos
                if (item.TIPO_UNIDADE != null)
                {
                    item.TIPO_UNIDADE = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.ENCOMENDA != null)
                {
                    item.ENCOMENDA = null;
                }
                if (item.AMBIENTE_CHAVE != null)
                {
                    item.AMBIENTE_CHAVE = null;
                }
                if (item.AUTORIZACAO_ACESSO != null)
                {
                    item.AUTORIZACAO_ACESSO = null;
                }
                if (item.ENTRADA_SAIDA != null)
                {
                    item.ENTRADA_SAIDA = null;
                }
                if (item.LISTA_CONVIDADO != null)
                {
                    item.LISTA_CONVIDADO = null;
                }
                if (item.OCORRENCIA != null)
                {
                    item.OCORRENCIA = null;
                }
                if (item.RESERVA != null)
                {
                    item.RESERVA = null;
                }
                if (item.SOLICITACAO_MUDANCA != null)
                {
                    item.SOLICITACAO_MUDANCA = null;
                }
                if (item.VAGA != null)
                {
                    item.VAGA = null;
                }
                if (item.CONTROLE_VEICULO != null)
                {
                    item.CONTROLE_VEICULO = null;
                }
                if (item.VEICULO != null)
                {
                    item.VEICULO = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddUNID",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<UNIDADE>(item)
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

        public Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.UNID_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                if (item.TORR_CD_ID != 0)
                {
                    TORRE torre = _baseService.GetTorreById(item.TORR_CD_ID);
                    item.UNID_NM_NOME_TORRE = torre.TORR_NM_NOME;
                }
                else
                {
                    item.UNID_NM_NOME_TORRE = "Única";
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditUNID",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<UNIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<UNIDADE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes)
        {
            try
            {
                // Persiste
                if (item.TORR_CD_ID != 0)
                {
                    TORRE torre = _baseService.GetTorreById(item.TORR_CD_ID);
                    item.UNID_NM_NOME_TORRE = torre.TORR_NM_NOME;
                }
                else
                {
                    item.UNID_NM_NOME_TORRE = "Única";
                }
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.USUARIO.Count > 0)
                {
                    return 1;
                }
                if (item.AMBIENTE_CHAVE.Count > 0)
                {
                    return 1;
                }
                if (item.AUTORIZACAO_ACESSO.Count > 0)
                {
                    return 1;
                }
                if (item.ENCOMENDA.Count > 0)
                {
                    return 1;
                }
                if (item.ENTRADA_SAIDA.Count > 0)
                {
                    return 1;
                }
                if (item.LISTA_CONVIDADO.Count > 0)
                {
                    return 1;
                }
                if (item.OCORRENCIA.Count > 0)
                {
                    return 1;
                }
                if (item.RESERVA.Count > 0)
                {
                    return 1;
                }
                if (item.SOLICITACAO_MUDANCA.Count > 0)
                {
                    return 1;
                }
                if (item.VAGA.Count > 0)
                {
                    return 1;
                }
                if (item.CONTROLE_VEICULO.Count > 0)
                {
                    return 1;
                }
                if (item.VEICULO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.UNID_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleUNID",
                    LOG_TX_REGISTRO = "Unidade: " + item.UNID_NM_EXIBE
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.UNID_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatUNID",
                    LOG_TX_REGISTRO = "Unidade: " + item.UNID_NM_EXIBE
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario)
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
                    String header = _temService.GetByCode("NOTIUNID").TEMP_TX_CABECALHO;
                    String body = _temService.GetByCode("NOTIUNID").TEMP_TX_CORPO;
                    String data = _temService.GetByCode("NOTIUNID").TEMP_TX_DADOS;

                    // Prepara corpo do e-mail  
                    USUARIO usu = _usuService.GetItemById(item.USUA_CD_ID);
                    String frase = String.Empty;
                    body = body.Replace("{Texto}", item.NOTI_TX_TEXTO);
                    data = data.Replace("{Usuario}", usuario.USUA_NM_NOME);
                    data = data.Replace("{Data}", DateTime.Today.Date.ToLongDateString());
                    header = header.Replace("{Nome}", usu.USUA_NM_NOME);

                    // Concatena
                    String emailBody = header + body + data;
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "NOTIFICAÇÃO";
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

                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
