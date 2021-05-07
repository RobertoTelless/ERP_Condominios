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

namespace ApplicationServices.Services
{
    public class TarefaAppService : AppServiceBase<TAREFA>, ITarefaAppService
    {
        private readonly ITarefaService _baseService;
        private readonly INotificacaoService _notiService;

        public TarefaAppService(ITarefaService baseService, INotificacaoService notiService): base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
        }

        public List<TAREFA> GetAllItens()
        {
            List<TAREFA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<TAREFA> GetAllItensAdm()
        {
            List<TAREFA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public List<TAREFA> GetTarefaStatus(Int32 user, Int32 tipo)
        {
            List<TAREFA> lista = _baseService.GetTarefaStatus(user, tipo);
            return lista;
        }

        public List<TAREFA> GetByDate(DateTime data)
        {
            List<TAREFA> lista = _baseService.GetByDate(data);
            return lista;
        }

        public List<TAREFA> GetByUser(Int32 user)
        {
            List<TAREFA> lista = _baseService.GetByUser(user);
            return lista;
        }

        public TAREFA GetItemById(Int32 id)
        {
            TAREFA item = _baseService.GetItemById(id);
            return item;
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _baseService.GetUserById(id);
            return item;
        }

        public TAREFA CheckExist(TAREFA tarefa)
        {
            TAREFA item = _baseService.CheckExist(tarefa);
            return item;
        }

        public List<TIPO_TAREFA> GetAllTipos()
        {
            List<TIPO_TAREFA> lista = _baseService.GetAllTipos();
            return lista;
        }

        public TAREFA_ANEXO GetAnexoById(Int32 id)
        {
            TAREFA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public List<PERIODICIDADE_TAREFA> GetAllPeriodicidade()
        {
            return _baseService.GetAllPeriodicidade();
        }

        public Int32 ExecuteFilter(Int32? tipoId, String titulo, DateTime? data, Int32 encerradas, Int32 prioridade, Int32? usuario, out List<TAREFA> objeto)
        {
            try
            {
                objeto = new List<TAREFA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipoId, titulo, data, encerradas, prioridade, usuario);
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

        public Int32 ValidateCreate(TAREFA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Verifica compartilhamento
                if (item.TARE_CD_USUA_1 == item.USUA_CD_ID || item.TARE_CD_USUA_2 == item.USUA_CD_ID || item.TARE_CD_USUA_3 == item.USUA_CD_ID )
                {
                    return 2;
                }

                // Completa objeto
                item.TARE_IN_ATIVO = 1;
                item.USUA_CD_ID = SessionMocks.UserCredentials.USUA_CD_ID;
                item.TARE_IN_STATUS = 1;
                item.TARE_IN_AVISA = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_NM_OPERACAO = "AddTARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item)
                };
                
                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera Notificações e tarefas compartilhadas
                if (item.TARE_CD_USUA_1 != null || item.TARE_CD_USUA_1 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_1.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_2 != null || item.TARE_CD_USUA_2 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_2.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_3 != null || item.TARE_CD_USUA_3 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_3.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TAREFA item, TAREFA itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.TARE_DT_REALIZADA < item.TARE_DT_CADASTRO)
                {
                    return 1;
                }
                if (item.TARE_DT_REALIZADA > DateTime.Today.Date)
                {
                    return 2;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_NM_OPERACAO = "EditTARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TAREFA>(itemAntes)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                // Gera Notificações e tarefas compartilhadas
                if (item.TARE_CD_USUA_1 != null || item.TARE_CD_USUA_1 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_1.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você e foi alterada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_2 != null || item.TARE_CD_USUA_2 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_2.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você e foi alterada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_3 != null || item.TARE_CD_USUA_3 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_3.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você e foi alterada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TAREFA item, TAREFA itemAntes)
        {
            try
            {
                // Verificação
                if (item.TARE_DT_REALIZADA < item.TARE_DT_CADASTRO)
                {
                    return 1;
                }
                if (item.TARE_DT_REALIZADA > DateTime.Today.Date)
                {
                    return 2;
                }

                // Persiste
                Int32 volta = _baseService.Edit(item);

                // Gera Notificações e tarefas compartilhadas
                if (item.TARE_CD_USUA_1 != null || item.TARE_CD_USUA_1 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_1.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você e foi alterada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_2 != null || item.TARE_CD_USUA_2 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_2.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você e foi alterada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_3 != null || item.TARE_CD_USUA_3 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_3.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + " foi compartilhada com você e foi alterada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TAREFA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                // Acerta campos
                item.TARE_IN_ATIVO = 0;

                //Evita erros de serialização
                if (item.TAREFA_ACOMPANHAMENTO != null)
                {
                    item.TAREFA_ACOMPANHAMENTO = null;
                }
                if (item.TAREFA_ANEXO != null)
                {
                    item.TAREFA_ANEXO = null;
                }
                if (item.TAREFA_NOTIFICACAO != null)
                {
                    item.TAREFA_NOTIFICACAO = null;
                }
                if (item.TIPO_TAREFA != null)
                {
                    item.TIPO_TAREFA = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.USUARIO1 != null)
                {
                    item.USUARIO1 = null;
                }

                if (item.USUARIO2 != null)
                {
                    item.USUARIO2 = null;
                }

                if (item.USUARIO3 != null)
                {
                    item.USUARIO3 = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTARE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item)
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);

                // Gera Notificações e tarefas compartilhadas
                if (item.TARE_CD_USUA_1 != null || item.TARE_CD_USUA_1 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_1.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + ", compartilhada com você,  foi excluída";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_2 != null || item.TARE_CD_USUA_2 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_2.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + ", compartilhada com você,  foi excluída";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_3 != null || item.TARE_CD_USUA_3 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_3.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + ", compartilhada com você,  foi excluída";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TAREFA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TARE_IN_ATIVO = 1;

                //Evita erros de serialização
                if (item.TAREFA_ACOMPANHAMENTO != null)
                {
                    item.TAREFA_ACOMPANHAMENTO = null;
                }

                if (item.TAREFA_ANEXO != null)
                {
                    item.TAREFA_ANEXO = null;
                }
                if (item.TAREFA_NOTIFICACAO != null)
                {
                    item.TAREFA_NOTIFICACAO = null;
                }

                if (item.TIPO_TAREFA != null)
                {
                    item.TIPO_TAREFA = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.USUARIO1 != null)
                {
                    item.USUARIO1 = null;
                }

                if (item.USUARIO2 != null)
                {
                    item.USUARIO2 = null;
                }

                if (item.USUARIO3 != null)
                {
                    item.USUARIO3 = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTARE",
                    //LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item)
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);

                // Gera Notificações e tarefas compartilhadas
                if (item.TARE_CD_USUA_1 != null || item.TARE_CD_USUA_1 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_1.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + ", compartilhada com você,  foi reativada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_2 != null || item.TARE_CD_USUA_2 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_2.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + ", compartilhada com você,  foi reativada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
                }
                if (item.TARE_CD_USUA_3 != null || item.TARE_CD_USUA_3 > 0)
                {

                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.CANO_CD_ID = 1;
                    noti.ASSI_CD_ID = SessionMocks.IdAssinante;
                    noti.NOTI_DT_DATA = DateTime.Today;
                    noti.NOTI_DT_EMISSAO = DateTime.Today;
                    noti.NOTI_IN_ATIVO = 1;
                    noti.NOTI_IN_ENVIADA = 1;
                    noti.NOTI_IN_STATUS = 1;
                    noti.USUA_CD_ID = item.TARE_CD_USUA_3.Value;
                    noti.NOTI_DT_VALIDADE = DateTime.Today.AddDays(30);
                    noti.NOTI_IN_NIVEL = 1;
                    noti.NOTI_IN_VISTA = 0;
                    noti.NOTI_NM_TITULO = "Aviso de Tarefa";
                    noti.NOTI_TX_TEXTO = "A tarefa " + item.TARE_NM_TITULO + ", compartilhada com você,  foi reativada";

                    // Persiste notificação 
                    Int32 volta1 = _notiService.Create(noti);
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
