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

namespace ApplicationServices.Services
{
    public class AmbienteAppService : AppServiceBase<AMBIENTE>, IAmbienteAppService
    {
        private readonly IAmbienteService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly ITemplateService _temService;
        private readonly IConfiguracaoService _confService;
        private readonly IUnidadeAppService _unidService;
        private readonly IUsuarioAppService _usuService;

        public AmbienteAppService(IAmbienteService baseService, INotificacaoService notiService, ITemplateService temService, IConfiguracaoService confService, IUnidadeAppService unidService, IUsuarioAppService usuService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _temService = temService;
            _confService = confService;
            _unidService = unidService;
            _usuService = usuService;
        }

        public List<AMBIENTE> GetAllItens(Int32 idAss)
        {
            List<AMBIENTE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<AMBIENTE> GetAllItensAdm(Int32 idAss)
        {
            List<AMBIENTE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public AMBIENTE GetItemById(Int32 id)
        {
            AMBIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public AMBIENTE CheckExist(AMBIENTE conta, Int32 idAss)
        {
            AMBIENTE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_AMBIENTE> GetAllTipos(Int32 idAss)
        {
            List<TIPO_AMBIENTE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public AMBIENTE_IMAGEM GetAnexoById(Int32 id)
        {
            AMBIENTE_IMAGEM lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? tipo, String nome, Int32 idAss, out List<AMBIENTE> objeto)
        {
            try
            {
                objeto = new List<AMBIENTE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, nome, idAss);
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

        public Int32 ValidateCreate(AMBIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.AMBI_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddAMBI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item)
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

        public Int32 ValidateEdit(AMBIENTE item, AMBIENTE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditAMBI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<AMBIENTE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(AMBIENTE item, AMBIENTE itemAntes)
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

        public Int32 ValidateDelete(AMBIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.RESERVA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.AMBI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelAMBI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(AMBIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.AMBI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatAMBI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
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

        public AMBIENTE_CUSTO GetAmbienteCustoById(Int32 id)
        {
            AMBIENTE_CUSTO lista = _baseService.GetAmbienteCustoById(id);
            return lista;
        }

        public Int32 ValidateEditAmbienteCusto(AMBIENTE_CUSTO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAmbienteCusto(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAmbienteCusto(AMBIENTE_CUSTO item)
        {
            try
            {
                // Persiste
                item.AMCU_NM_ATIVO = 1;
                Int32 volta = _baseService.CreateAmbienteCusto(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AMBIENTE_CHAVE GetAmbienteChaveById(Int32 id)
        {
            AMBIENTE_CHAVE lista = _baseService.GetAmbienteChaveById(id);
            return lista;
        }

        public Int32 ValidateEditAmbienteChave(AMBIENTE_CHAVE item, USUARIO usuario)
        {
            try
            {
                // Critica data
                if (item.AMCH_DT_DEVOLUCAO != null)
                {
                    if (item.AMCH_DT_DEVOLUCAO > DateTime.Today.Date)
                    {
                        return 1;
                    }
                }

                // Persiste
                Int32 volta1 = 0;

                // Verifica data e gera mensagens
                if (item.AMCH_DT_DEVOLUCAO > item.AMCH_DT_PREVISTA)
                {
                    // Persiste
                    item.AMCH_TX_OBSERVACOES = "Devolvida com Atraso";
                    volta1 = _baseService.EditAmbienteChave(item);

                    // Recupera informações
                    USUARIO usua = _usuService.GetItemById(item.USUA_CD_ID);
                    AMBIENTE amb = _baseService.GetItemById(item.AMBI_CD_ID);
                    UNIDADE unid = _unidService.GetItemById(item.UNID_CD_ID);

                    // Recupera template e-mail
                    String texto = "A devolução da chave do ambiente " + amb.AMBI_NM_AMBIENTE + " que estava sob sua responsabilidade foi devolvida após a data especificada. Isso poderá gerar multa. Você será contactado pela Administração";
                    String header = _temService.GetByCode("NOTICHAVE").TEMP_TX_CABECALHO;
                    String body = _temService.GetByCode("NOTICHAVE").TEMP_TX_CORPO;
                    String data = _temService.GetByCode("NOTICHAVE").TEMP_TX_DADOS;

                    body = body.Replace("{Texto}", texto);
                    body = body.Replace("{Condominio}", usuario.ASSINANTE.ASSI_NM_NOME);
                    data = data.Replace("{Ambiente}", amb.AMBI_NM_AMBIENTE);
                    data = data.Replace("{Unidade}", unid.UNID_NM_EXIBE);
                    data = data.Replace("{DataEntrega}", item.AMCH_DT_ENTREGA.ToLongDateString());
                    data = data.Replace("{DataPrevista}", item.AMCH_DT_DEVOLUCAO.Value.ToLongDateString());
                    data = data.Replace("{Unidade}", DateTime.Today.Date.ToLongDateString());
                    header = header.Replace("{Nome}", usua.USUA_NM_NOME);

                    // Gera Notificação para morador
                    NOTIFICACAO vm = new NOTIFICACAO();
                    vm.NOTI_DT_EMISSAO = DateTime.Today.Date;
                    vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    vm.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    vm.NOTI_IN_ATIVO = 1;
                    vm.NOTI_IN_NIVEL = 1;
                    vm.NOTI_IN_ORIGEM = 1;
                    vm.NOTI_IN_STATUS = 1;
                    vm.NOTI_IN_VISTA = 0;
                    vm.NOTI_NM_TITULO = "Notificação para Morador - Chave de Ambiente";
                    vm.CANO_CD_ID = 3;
                    vm.USUA_CD_ID = item.USUA_CD_ID;
                    vm.NOTI_TX_TEXTO = texto;
                    volta1 = _notiService.Create(vm);

                    // Concatena
                    String emailBody = header + body + data;
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                    Email mensagem = new Email();
                    mensagem.ASSUNTO = "Notificação para Morador - Chave de Ambiente";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_DESTINO = usua.USUA_NM_EMAIL;
                    mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                    mensagem.ENABLE_SSL = true;
                    mensagem.NOME_EMISSOR = "ERP Condomínios";
                    mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                    mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                    mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                    mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                    mensagem.NETWORK_CREDENTIAL = net;

                    // Envia mensagem
                    Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);

                    // Gera Notificação para sindico
                    texto = "A devolução da chave do ambiente " + amb.AMBI_NM_AMBIENTE + " que estava sob a responsabilidade do morador " + usua.USUA_NM_NOME + " da unidade " + unid.UNID_NM_EXIBE + " foi devolvida após a data especificada. Favor verificar a possível incidência de multa e contactar o morador responsável";
                    List<USUARIO> lista = _usuService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.PERF_CD_ID == 1 || p.PERF_CD_ID == 2).ToList();
                    foreach (USUARIO usu in lista)
                    {
                        vm = new NOTIFICACAO();
                        vm.NOTI_DT_EMISSAO = DateTime.Today.Date;
                        vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        vm.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                        vm.NOTI_IN_ATIVO = 1;
                        vm.NOTI_IN_NIVEL = 1;
                        vm.NOTI_IN_ORIGEM = 1;
                        vm.NOTI_IN_STATUS = 1;
                        vm.NOTI_IN_VISTA = 0;
                        vm.NOTI_NM_TITULO = "Notificação para Síndico - Chave de Ambiente";
                        vm.CANO_CD_ID = 3;
                        vm.USUA_CD_ID = usu.USUA_CD_ID;
                        vm.NOTI_TX_TEXTO = texto;
                        volta1 = _notiService.Create(vm);
                    }
                }
                else
                {
                    volta1 = _baseService.EditAmbienteChave(item);
                }
                return volta1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAmbienteChave(AMBIENTE_CHAVE item, USUARIO usuario)
        {
            try
            {
                // Critica
                if (item.AMCH_DT_ENTREGA > DateTime.Today.Date)
                {
                    return 1;
                }

                // Persiste
                item.AMCH_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateAmbienteChave(item);

                // Gera Notificação
                AMBIENTE amb = _baseService.GetItemById(item.AMBI_CD_ID);
                UNIDADE unid = _unidService.GetItemById(item.UNID_CD_ID);
                USUARIO usu = _usuService.GetItemById(item.USUA_CD_ID);
                NOTIFICACAO vm = new NOTIFICACAO();
                vm.CANO_CD_ID = 3;
                vm.USUA_CD_ID = item.USUA_CD_ID;
                vm.NOTI_DT_EMISSAO = DateTime.Today.Date;
                vm.ASSI_CD_ID = item.ASSI_CD_ID;
                vm.NOTI_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                vm.NOTI_IN_ATIVO = 1;
                vm.NOTI_IN_NIVEL = 1;
                vm.NOTI_IN_ORIGEM = 1;
                vm.NOTI_IN_STATUS = 1;
                vm.NOTI_IN_VISTA = 0;
                vm.NOTI_NM_TITULO = "Notificação de Entrega de Chave";
                vm.NOTI_TX_TEXTO = "A chave do ambiente " + amb.AMBI_NM_AMBIENTE + " foi entrege em " + item.AMCH_DT_ENTREGA.ToShortDateString() + " para a unidade " + unid.UNID_NM_EXIBE + " com devolução prevista para " + item.AMCH_DT_PREVISTA.ToShortDateString() + ".";
                volta = _notiService.Create(vm);

                // Recupera template e-mail
                String header = _temService.GetByCode("NOTICHAVE").TEMP_TX_CABECALHO;
                String body = _temService.GetByCode("NOTICHAVE").TEMP_TX_CORPO;
                String footer = _temService.GetByCode("NOTICHAVE").TEMP_TX_DADOS;

                // Prepara corpo do e-mail  
                String frase = String.Empty;
                footer = footer.Replace("{Ambiente}", amb.AMBI_NM_AMBIENTE);
                footer = footer.Replace("{Unidade}", unid.UNID_NM_EXIBE);
                footer = footer.Replace("{DataEntrega}", item.AMCH_DT_ENTREGA.ToShortDateString());
                footer = footer.Replace("{DataPrevista}", item.AMCH_DT_PREVISTA.ToShortDateString());
                body = body.Replace("{Texto}", "A chave do ambiente " + amb.AMBI_NM_AMBIENTE + " foi entrege em " + item.AMCH_DT_ENTREGA.ToShortDateString() + " para a unidade " + unid.UNID_NM_EXIBE + " com devolução prevista para " + item.AMCH_DT_PREVISTA.ToShortDateString() + ".");
                body = body.Replace("{Condominio}", usuario.ASSINANTE.ASSI_NM_NOME);
                header = header.Replace("{Nome}", usu.USUA_NM_NOME);

                // Concatenana 
                String emailBody = header + body + footer;
                CONFIGURACAO conf = _confService.GetItemById(item.ASSI_CD_ID);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                Email mensagem = new Email();
                mensagem.ASSUNTO = "NOTIFICAÇÃO - ENTREGA DE CHAVES";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_DESTINO = usu.USUA_NM_EMAIL;
                mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "ERP Condomínios";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.NETWORK_CREDENTIAL = net;

                // Envia mensagem
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
