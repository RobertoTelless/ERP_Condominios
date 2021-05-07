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
    public class SubgrupoAppService : AppServiceBase<SUBGRUPO>, ISubgrupoAppService
    {
        private readonly ISubgrupoService _baseService;

        public SubgrupoAppService(ISubgrupoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<SUBGRUPO> GetAllItens()
        {
            List<SUBGRUPO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<SUBGRUPO> GetAllItensAdm()
        {
            List<SUBGRUPO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public SUBGRUPO GetItemById(Int32 id)
        {
            SUBGRUPO item = _baseService.GetItemById(id);
            return item;
        }

        public List<GRUPO> GetAllGrupos()
        {
            List<GRUPO> lista = _baseService.GetAllGrupos();
            return lista;
        }

        public SUBGRUPO CheckExist(SUBGRUPO obj)
        {
            SUBGRUPO item = _baseService.CheckExist(obj);
            return item;
        }


        public Int32 ValidateCreate(SUBGRUPO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.SUBG_IN_ATIVO = 1;
                item.ASSI_CD_ID = SessionMocks.IdAssinante;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddSUBG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SUBGRUPO>(item)
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

        public Int32 ValidateEdit(SUBGRUPO item, SUBGRUPO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditSUBG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<SUBGRUPO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<SUBGRUPO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(SUBGRUPO item, SUBGRUPO itemAntes)
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

        public Int32 ValidateDelete(SUBGRUPO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.CENTRO_CUSTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.SUBG_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleSUBG",
                    LOG_TX_REGISTRO = "SubGrupo: " + item.SUBG_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(SUBGRUPO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.SUBG_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatSUBG",
                    LOG_TX_REGISTRO = "SubGrupo: " + item.SUBG_NM_NOME
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
