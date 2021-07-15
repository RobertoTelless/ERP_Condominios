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
    public class BancoAppService : AppServiceBase<BANCO>, IBancoAppService
    {
        private readonly IBancoService _baseService;

        public BancoAppService(IBancoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public BANCO CheckExist(BANCO conta, Int32 idAss)
        {
            BANCO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<BANCO> GetAllItens(Int32 idAss)
        {
            List<BANCO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<BANCO> GetAllItensAdm(Int32 idAss)
        {
            List<BANCO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public BANCO GetItemById(Int32 id)
        {
            BANCO item = _baseService.GetItemById(id);
            return item;
        }

        public BANCO GetByCodigo(String codigo)
        {
            BANCO item = _baseService.GetByCodigo(codigo);
            return item;
        }

        public Int32 ExecuteFilter(String codigo, String nome, Int32 idAss, out List<BANCO> objeto)
        {
            try
            {
                objeto = new List<BANCO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(codigo, nome, idAss);
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

        public Int32 ValidateCreate(BANCO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.BANC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddBANC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<BANCO>(item)
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

        public Int32 ValidateEdit(BANCO item, BANCO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditBANC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<BANCO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<BANCO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(BANCO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CONTA_BANCO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.BANC_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelBANC",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<BANCO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(BANCO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.BANC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatBANC",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<BANCO>(item)
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
