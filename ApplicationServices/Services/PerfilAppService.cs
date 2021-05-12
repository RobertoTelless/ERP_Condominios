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
    public class PerfilAppService : AppServiceBase<PERFIL>, IPerfilAppService
    {
        private readonly IPerfilService _perfilService;

        public PerfilAppService(IPerfilService perfilService): base(perfilService)
        {
            _perfilService = perfilService;
        }

        public List<PERFIL> GetAllItens()
        {
            List<PERFIL> lista = _perfilService.GetAll().ToList();
            return lista;
        }

        public PERFIL GetByID(Int32 id)
        {
            PERFIL perfil = _perfilService.GetById(id);
            return perfil;
        }

        public Int32 ValidateCreate(PERFIL perfil, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_perfilService.GetByName(perfil.PERF_NM_NOME) != null)
                {
                    return 1;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddPERF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERFIL>(perfil)
                };

                // Persiste
                Int32 volta = _perfilService.Create(perfil, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PERFIL perfil, PERFIL perfilAntes, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_perfilService.GetByName(perfil.PERF_NM_NOME) != null)
                {
                    return 1;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditPERF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERFIL>(perfil),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PERFIL>(perfilAntes)
                };

                // Persiste
                return _perfilService.Edit(perfil, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PERFIL perfil, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (_perfilService.GetUserProfile(perfil) != null)
                {
                    return 1;
                }

                                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPERF",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERFIL>(perfil)
                };

                // Persiste
                return _perfilService.Delete(perfil, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
