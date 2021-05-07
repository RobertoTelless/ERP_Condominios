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
    public class PlanoContaAppService : AppServiceBase<PLANO_CONTA>, IPlanoContaAppService
    {
        private readonly IPlanoContaService _baseService;

        public PlanoContaAppService(IPlanoContaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<PLANO_CONTA> GetAllItens()
        {
            List<PLANO_CONTA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PLANO_CONTA> GetAllItensAdm()
        {
            List<PLANO_CONTA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public PLANO_CONTA GetItemById(Int32 id)
        {
            PLANO_CONTA item = _baseService.GetItemById(id);
            return item;
        }

        public PLANO_CONTA CheckExist(PLANO_CONTA obj)
        {
            PLANO_CONTA item = _baseService.CheckExist(obj);
            return item;
        }

        public Int32 ValidateCreate(PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.PLCO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPLCO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO_CONTA>(item)
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

        public Int32 ValidateEdit(PLANO_CONTA item, PLANO_CONTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPLCO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO_CONTA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PLANO_CONTA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                //if (item.CONTA_PAGAR.Count > 0 | item.CONTA_RECEBER.Count > 0 | item.CONTRATO.Count > 0)
                //{
                //    return 1;
                //}

                // Acerta campos
                item.PLCO_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPLCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO_CONTA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PLCO_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPLCO",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PLANO_CONTA>(item)
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
