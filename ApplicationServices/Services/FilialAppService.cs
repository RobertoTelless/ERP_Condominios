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
    public class FilialAppService : AppServiceBase<FILIAL>, IFilialAppService
    {
        private readonly IFilialService _baseService;

        public FilialAppService(IFilialService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<FILIAL> GetAllItens()
        {
            List<FILIAL> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<FILIAL> GetAllItensAdm()
        {
            List<FILIAL> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public List<UF> GetAllUF()
        {
            return _baseService.GetAllUF();
        }

        public FILIAL GetItemById(Int32 id)
        {
            FILIAL item = _baseService.GetItemById(id);
            return item;
        }

        public FILIAL CheckExist(FILIAL filial)
        {
            FILIAL item = _baseService.CheckExist(filial);
            return item;
        }

        public Int32 ValidateCreate(FILIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.FILI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_NM_OPERACAO = "AddFILI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FILIAL>(item)
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

        public Int32 ValidateEdit(FILIAL item, FILIAL itemAntes, USUARIO usuario)
        {
            try
            {
                //// Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_NM_OPERACAO = "EditFILI",
                //    LOG_IN_ATIVO = 1,
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<FILIAL>(item),
                //    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<FILIAL>(itemAntes)
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(FILIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CLIENTE != null)
                {
                    item.CLIENTE = null;
                }
                if (item.COLABORADOR != null)
                {
                    item.COLABORADOR = null;
                }
                if (item.CONTA_RECEBER != null)
                {
                    item.CONTA_RECEBER = null;
                }
                if (item.EQUIPAMENTO != null)
                {
                    item.EQUIPAMENTO = null;
                }
                if (item.FORNECEDOR != null)
                {
                    item.FORNECEDOR = null;
                }
                if (item.MATERIA_PRIMA != null)
                {
                    item.MATERIA_PRIMA = null;
                }
                if (item.MOVIMENTO_ESTOQUE_MATERIA_PRIMA != null)
                {
                    item.MOVIMENTO_ESTOQUE_MATERIA_PRIMA = null;
                }
                if (item.MOVIMENTO_ESTOQUE_PRODUTO != null)
                {
                    item.MOVIMENTO_ESTOQUE_PRODUTO = null;
                }
                if (item.PATRIMONIO != null)
                {
                    item.PATRIMONIO = null;
                }
                if (item.PEDIDO_COMPRA != null)
                {
                    item.PEDIDO_COMPRA = null;
                }
                if (item.PEDIDO_VENDA != null)
                {
                    item.PEDIDO_VENDA = null;
                }
                if (item.PRECO_PRODUTO != null)
                {
                    item.PRECO_PRODUTO = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }
                if (item.SERVICO != null)
                {
                    item.SERVICO = null;
                }
                if (item.TRANSPORTADORA != null)
                {
                    item.TRANSPORTADORA = null;
                }
                if (item.VALOR_COMISSAO != null)
                {
                    item.VALOR_COMISSAO = null;
                }

                // Acerta campos
                item.FILI_IN_ATIVO = 0;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "DelFILI",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<FILIAL>(item)
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FILIAL item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FILI_IN_ATIVO = 1;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "ReatFILI",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<FILIAL>(item)
                //};

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
