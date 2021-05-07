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
    public class UnidadeAppService : AppServiceBase<UNIDADE>, IUnidadeAppService
    {
        private readonly IUnidadeService _baseService;

        public UnidadeAppService(IUnidadeService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<UNIDADE> GetAllItens()
        {
            List<UNIDADE> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<UNIDADE> GetAllItensAdm()
        {
            List<UNIDADE> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public UNIDADE GetItemById(Int32 id)
        {
            UNIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.UNID_IN_ATIVO = 1;
                item.ASSI_CD_ID = SessionMocks.IdAssinante;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddUNID",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<UNIDADE>(item)
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

        public Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditUNID",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<UNIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<UNIDADE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes)
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

        public Int32 ValidateDelete(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                //if (item.USUARIO.Count > 0)
                //{
                //    return 1;
                //}

                // Acerta campos
                item.UNID_IN_ATIVO = 0;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "DeleUNID",
                //    LOG_TX_REGISTRO = "Unidade: " + item.UNID_NM_NOME
                //};

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.UNID_IN_ATIVO = 1;

                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    LOG_IN_ATIVO = 1,
                //    LOG_NM_OPERACAO = "ReatUNID",
                //    LOG_TX_REGISTRO = "Unidade: " + item.UNID_NM_NOME
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
