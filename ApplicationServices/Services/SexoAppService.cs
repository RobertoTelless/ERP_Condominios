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
    public class SexoAppService : AppServiceBase<SEXO>, ISexoAppService
    {
        private readonly Isex _baseService;

        public RegimeTributarioAppService(IRegimeTributarioService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<REGIME_TRIBUTARIO> GetAllItens()
        {
            List<REGIME_TRIBUTARIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<REGIME_TRIBUTARIO> GetAllItensAdm()
        {
            List<REGIME_TRIBUTARIO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public REGIME_TRIBUTARIO GetItemById(Int32 id)
        {
            REGIME_TRIBUTARIO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(REGIME_TRIBUTARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.RETR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddRETR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<REGIME_TRIBUTARIO>(item)
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

        public Int32 ValidateEdit(REGIME_TRIBUTARIO item, REGIME_TRIBUTARIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditRETR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<REGIME_TRIBUTARIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<REGIME_TRIBUTARIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(REGIME_TRIBUTARIO item, REGIME_TRIBUTARIO itemAntes)
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

        public Int32 ValidateDelete(REGIME_TRIBUTARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CLIENTE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.RETR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatRETR",
                    LOG_TX_REGISTRO = "Categoria: " + item.RETR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(REGIME_TRIBUTARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.RETR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatRETR",
                    LOG_TX_REGISTRO = "Categoria: " + item.RETR_NM_NOME
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
