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
    public class FormaPagamentoAppService : AppServiceBase<FORMA_PAGAMENTO>, IFormaPagamentoAppService
    {
        private readonly IFormaPagamentoService _baseService;

        public FormaPagamentoAppService(IFormaPagamentoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<FORMA_PAGAMENTO> GetAllItens(Int32 tipo)
        {
            List<FORMA_PAGAMENTO> lista = _baseService.GetAllItens(tipo);
            return lista;
        }

        public List<FORMA_PAGAMENTO> GetAllItens()
        {
            List<FORMA_PAGAMENTO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<FORMA_PAGAMENTO> GetAllItensAdm()
        {
            List<FORMA_PAGAMENTO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public FORMA_PAGAMENTO GetItemById(Int32 id)
        {
            FORMA_PAGAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(FORMA_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.FOPA_IN_ATIVO = 1;
                item.ASSI_CD_ID = SessionMocks.IdAssinante;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddFOPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORMA_PAGAMENTO>(item)
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

        public Int32 ValidateEdit(FORMA_PAGAMENTO item, FORMA_PAGAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                item.CONTA_BANCO = null;
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditFOPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORMA_PAGAMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<FORMA_PAGAMENTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(FORMA_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CONTA_PAGAR.Count > 0)
                {
                    return 1;
                }
                if (item.CONTA_RECEBER.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.FOPA_IN_ATIVO = 0;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "DelFOPA",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORMA_PAGAMENTO>(item)
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FORMA_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FOPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatFOPA",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORMA_PAGAMENTO>(item)
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
