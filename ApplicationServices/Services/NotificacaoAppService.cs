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

namespace ApplicationServices.Services
{
    public class NotificacaoAppService : AppServiceBase<NOTIFICACAO>, INotificacaoAppService
    {
        private readonly INotificacaoService _baseService;

        public NotificacaoAppService(INotificacaoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<NOTIFICACAO> GetAllItens(Int32 idAss)
        {
            List<NOTIFICACAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<NOTIFICACAO> GetAllItensAdm(Int32 idAss)
        {
            List<NOTIFICACAO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCategorias(Int32 idAss)
        {
            List<CATEGORIA_NOTIFICACAO> lista = _baseService.GetAllCategorias(idAss);
            return lista;
        }

        public NOTIFICACAO_ANEXO GetAnexoById(Int32 id)
        {
            NOTIFICACAO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public NOTIFICACAO GetItemById(Int32 id)
        {
            NOTIFICACAO item = _baseService.GetItemById(id);
            return item;
        }

        public List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss)
        {
            List<NOTIFICACAO> lista = _baseService.GetAllItensUser(id, idAss);
            return lista;
        }

        public List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss)
        {
            List<NOTIFICACAO> lista = _baseService.GetNotificacaoNovas(id, idAss);
            return lista;
        }

        public Int32 ExecuteFilter(String titulo, DateTime? data, String texto, Int32 idAss, out List<NOTIFICACAO> objeto)
        {
            try
            {
                objeto = new List<NOTIFICACAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(titulo, data, texto, idAss);
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

        public Int32 ValidateCreate(NOTIFICACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.NOTI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddNOTI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTIFICACAO>(item)
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

        public Int32 ValidateEdit(NOTIFICACAO item, NOTIFICACAO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditNOTI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTIFICACAO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<NOTIFICACAO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(NOTIFICACAO item, NOTIFICACAO itemAntes)
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

        public Int32 ValidateDelete(NOTIFICACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.NOTI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelNOTI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTIFICACAO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(NOTIFICACAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.NOTI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatNOTI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTIFICACAO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
