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
    public class PatrimonioAppService : AppServiceBase<PATRIMONIO>, IPatrimonioAppService
    {
        private readonly IPatrimonioService _baseService;

        public PatrimonioAppService(IPatrimonioService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<PATRIMONIO> GetAllItens()
        {
            List<PATRIMONIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PATRIMONIO> CalcularDepreciados()
        {
            List<PATRIMONIO> lista = _baseService.CalcularDepreciados();
            return lista;
        }

        public List<PATRIMONIO> CalcularBaixados()
        {
            List<PATRIMONIO> lista = _baseService.CalcularBaixados();
            return lista;
        }

        public List<PATRIMONIO> GetAllItensAdm()
        {
            List<PATRIMONIO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public PATRIMONIO GetItemById(Int32 id)
        {
            PATRIMONIO item = _baseService.GetItemById(id);
            return item;
        }

        public PATRIMONIO GetByNumero(String numero)
        {
            PATRIMONIO item = _baseService.GetByNumero(numero);
            return item;
        }

        public PATRIMONIO CheckExist(PATRIMONIO conta)
        {
            PATRIMONIO item = _baseService.CheckExist(conta);
            return item;
        }

        public List<CATEGORIA_PATRIMONIO> GetAllTipos()
        {
            List<CATEGORIA_PATRIMONIO> lista = _baseService.GetAllTipos();
            return lista;
        }

        public PATRIMONIO_ANEXO GetAnexoById(Int32 id)
        {
            PATRIMONIO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 CalcularDiasDepreciacao(PATRIMONIO item)
        {
            Int32 totalDias = 0;
            if (item.PATR_DT_COMPRA != null & item.PATR_NR_VIDA_UTIL != null)
            {
                if (item.PATR_DT_COMPRA != DateTime.MinValue & item.PATR_NR_VIDA_UTIL > 0)
                {
                    Int32 dias = item.PATR_NR_VIDA_UTIL.Value * 30;
                    DateTime dataLimite = item.PATR_DT_COMPRA.Value.AddDays(dias);
                    totalDias = dataLimite.Subtract(DateTime.Today).Days;
                }
            }
            return totalDias;
        }

        public Int32 ExecuteFilter(Int32? catId, String nome, String numero, Int32? filial, out List<PATRIMONIO> objeto)
        {
            try
            {
                objeto = new List<PATRIMONIO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, nome, numero, filial);
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

        public Int32 ValidateCreate(PATRIMONIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PATR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPATR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PATRIMONIO>(item)
                };

                // Persiste patrimonio
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PATRIMONIO item, PATRIMONIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPATR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PATRIMONIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PATRIMONIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PATRIMONIO item, PATRIMONIO itemAntes)
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

        public Int32 ValidateDelete(PATRIMONIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PATR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPATR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PATRIMONIO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PATRIMONIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PATR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPATR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PATRIMONIO>(item)
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
