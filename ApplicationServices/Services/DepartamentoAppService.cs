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
    public class DepartamentoAppService : AppServiceBase<DEPARTAMENTO>, IDepartamentoAppService
    {
        private readonly IDepartamentoService _baseService;

        public DepartamentoAppService(IDepartamentoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<DEPARTAMENTO> GetAllItens()
        {
            List<DEPARTAMENTO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<DEPARTAMENTO> GetAllItensAdm()
        {
            List<DEPARTAMENTO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public DEPARTAMENTO GetItemById(Int32 id)
        {
            DEPARTAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(DEPARTAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.DEPT_IN_ATIVO = 1;
                item.ASSI_CD_ID = SessionMocks.IdAssinante;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddDEPT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<DEPARTAMENTO>(item)
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

        public Int32 ValidateEdit(DEPARTAMENTO item, DEPARTAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_NM_OPERACAO = "EditDEPT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<DEPARTAMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<DEPARTAMENTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(DEPARTAMENTO item, DEPARTAMENTO itemAntes)
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

        public Int32 ValidateDelete(DEPARTAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ATENDIMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.DEPT_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleDEPT",
                    LOG_TX_REGISTRO = "Departamento: " + item.DEPT_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(DEPARTAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.DEPT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatDEPT",
                    LOG_TX_REGISTRO = "Deparatmento: " + item.DEPT_NM_NOME
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
