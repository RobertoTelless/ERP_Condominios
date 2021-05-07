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
    public class CategoriaFornecedorAppService : AppServiceBase<CATEGORIA_FORNECEDOR>, ICategoriaFornecedorAppService
    {
        private readonly ICategoriaFornecedorService _baseService;

        public CategoriaFornecedorAppService(ICategoriaFornecedorService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<CATEGORIA_FORNECEDOR> GetAllItens()
        {
            List<CATEGORIA_FORNECEDOR> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<CATEGORIA_FORNECEDOR> GetAllItensAdm()
        {
            List<CATEGORIA_FORNECEDOR> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public CATEGORIA_FORNECEDOR GetItemById(Int32 id)
        {
            CATEGORIA_FORNECEDOR item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_FORNECEDOR item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.ASSI_CD_ID = SessionMocks.IdAssinante;
                item.CAFO_IN_ATIVO = 1;

                //// Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    LOG_NM_OPERACAO = "AddCAFO",
                //    LOG_IN_ATIVO = 1,
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_FORNECEDOR>(item)
                //};

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_FORNECEDOR item, CATEGORIA_FORNECEDOR itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCAFO",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_FORNECEDOR>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_FORNECEDOR>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CATEGORIA_FORNECEDOR item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.FORNECEDOR.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CAFO_IN_ATIVO = 0;

                //// Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "DelCAFO",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_FORNECEDOR>(item)
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_FORNECEDOR item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CAFO_IN_ATIVO = 1;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "ReatCAFO",
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_FORNECEDOR>(item)
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
