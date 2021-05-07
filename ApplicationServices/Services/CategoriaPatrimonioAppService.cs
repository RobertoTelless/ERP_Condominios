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
    public class CategoriaPatrimonioAppService : AppServiceBase<CATEGORIA_PATRIMONIO>, ICategoriaPatrimonioAppService
    {
        private readonly ICategoriaPatrimonioService _baseService;

        public CategoriaPatrimonioAppService(ICategoriaPatrimonioService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<CATEGORIA_PATRIMONIO> GetAllItens()
        {
            List<CATEGORIA_PATRIMONIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<CATEGORIA_PATRIMONIO> GetAllItensAdm()
        {
            List<CATEGORIA_PATRIMONIO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public CATEGORIA_PATRIMONIO GetItemById(Int32 id)
        {
            CATEGORIA_PATRIMONIO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_PATRIMONIO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.ASSI_CD_ID = SessionMocks.IdAssinante;
                item.CAPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCAPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_PATRIMONIO>(item)
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

        public Int32 ValidateEdit(CATEGORIA_PATRIMONIO item, CATEGORIA_PATRIMONIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCAPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_PATRIMONIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_PATRIMONIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CATEGORIA_PATRIMONIO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PATRIMONIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CAPA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCAPA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_PATRIMONIO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_PATRIMONIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCAPA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_PATRIMONIO>(item)
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
