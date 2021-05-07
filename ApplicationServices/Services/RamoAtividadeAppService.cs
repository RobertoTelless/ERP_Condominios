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
    public class RamoAtividadeAppService : AppServiceBase<RAMO_ATIVIDADE>, IRamoAtividadeAppService
    {
        private readonly IRamoAtividadeService _baseService;

        public RamoAtividadeAppService(IRamoAtividadeService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<RAMO_ATIVIDADE> GetAllItens()
        {
            List<RAMO_ATIVIDADE> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<RAMO_ATIVIDADE> GetAllItensAdm()
        {
            List<RAMO_ATIVIDADE> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public RAMO_ATIVIDADE GetItemById(Int32 id)
        {
            RAMO_ATIVIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(RAMO_ATIVIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.RAAT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddRAAT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<RAMO_ATIVIDADE>(item)
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

        public Int32 ValidateEdit(RAMO_ATIVIDADE item, RAMO_ATIVIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditRAAT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<RAMO_ATIVIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<RAMO_ATIVIDADE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(RAMO_ATIVIDADE item, RAMO_ATIVIDADE itemAntes)
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

        public Int32 ValidateDelete(RAMO_ATIVIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.EMPREITEIRO.Count > 0 || item.RESPONSAVEL_TECNICO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.RAAT_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatRAAT",
                    LOG_TX_REGISTRO = "Ramo: " + item.RAAT_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(RAMO_ATIVIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.RAAT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatRAAT",
                    LOG_TX_REGISTRO = "Categoria: " + item.RAAT_NM_NOME
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
