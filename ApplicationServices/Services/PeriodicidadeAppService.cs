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
    public class PeriodicidadeAppService : AppServiceBase<PERIODICIDADE>, IPeriodicidadeAppService
    {
        private readonly IPeriodicidadeService _baseService;

        public PeriodicidadeAppService(IPeriodicidadeService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<PERIODICIDADE> GetAllItens()
        {
            List<PERIODICIDADE> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PERIODICIDADE> GetAllItensAdm()
        {
            List<PERIODICIDADE> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public PERIODICIDADE GetItemById(Int32 id)
        {
            PERIODICIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(PERIODICIDADE item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.PERI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPERI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE>(item)
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

        public Int32 ValidateEdit(PERIODICIDADE item, PERIODICIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPERI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PERIODICIDADE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PERIODICIDADE item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CONTA_PAGAR.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.PERI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPERI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PERIODICIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PERI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPERI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE>(item)
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
