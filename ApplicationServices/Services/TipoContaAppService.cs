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
    public class TipoContaAppService : AppServiceBase<TIPO_CONTA>, ITipoContaAppService
    {
        private readonly ITipoContaService _baseService;

        public TipoContaAppService(ITipoContaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_CONTA> GetAllItens()
        {
            List<TIPO_CONTA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<TIPO_CONTA> GetAllItensAdm()
        {
            List<TIPO_CONTA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public TIPO_CONTA GetItemById(Int32 id)
        {
            TIPO_CONTA item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(TIPO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddTICO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_CONTA>(item)
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

        public Int32 ValidateEdit(TIPO_CONTA item, TIPO_CONTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditTICO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_CONTA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_CONTA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TIPO_CONTA item, TIPO_CONTA itemAntes)
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

        public Int32 ValidateDelete(TIPO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                //if (item.USUARIO.Count > 0)
                //{
                //    return 1;
                //}

                // Acerta campos

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleTICO",
                    LOG_TX_REGISTRO = "Tipo de Conta: " + item.TICO_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTICO",
                    LOG_TX_REGISTRO = "Tipo de Conta: " + item.TICO_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
