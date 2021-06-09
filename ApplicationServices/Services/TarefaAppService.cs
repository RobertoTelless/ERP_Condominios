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

        public List<TAREFA> GetAllItens(Int32 idAss)
        {
            List<TAREFA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TAREFA> GetAllItensAdm(Int32 idAss)
        {
            List<TAREFA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<TAREFA> GetTarefaStatus(Int32 user, Int32 tipo)
        {
            List<TAREFA> lista = _baseService.GetTarefaStatus(user, tipo);
            return lista;
        }

        public List<TAREFA> GetByDate(DateTime data, Int32 idAss)
        {
            List<TAREFA> lista = _baseService.GetByDate(data, idAss);
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

        public TAREFA CheckExist(TAREFA tarefa, Int32 idAss)
        {
            TAREFA item = _baseService.CheckExist(tarefa, idAss);
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

        public Int32 ExecuteFilter(Int32? tipoId, String titulo, DateTime? data, Int32 encerradas, Int32 prioridade, Int32? usuario, Int32 idAss, out List<TAREFA> objeto)
        {
            try
            {
                objeto = new List<TAREFA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipoId, titulo, data, encerradas, prioridade, usuario, idAss);
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
                //Verifica Campos
                if (item.TIPO_TAREFA != null)
                {
                    item.TIPO_TAREFA = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.PERIODICIDADE_TAREFA != null)
                {
                    item.PERIODICIDADE_TAREFA = null;
                }

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TARE_IN_ATIVO = 1;
                item.USUA_CD_ID = usuario.USUA_CD_ID;
                item.TARE_IN_STATUS = 1;
                item.TARE_IN_AVISA = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item)
                };
                
                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera Notificações e tarefas compartilhadas

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
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TAREFA>(itemAntes)
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
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

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTARE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item)
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);
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

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTARE",
                    //LOG_TX_REGISTRO = Serialization.SerializeJSON<TAREFA>(item)
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
