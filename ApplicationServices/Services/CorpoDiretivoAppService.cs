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
    public class CorpoDiretivoAppService : AppServiceBase<CORPO_DIRETIVO>, ICorpoDiretivoAppService
    {
        private readonly ICorpoDiretivoService _baseService;
        private readonly IConfiguracaoService _confService;

        public CorpoDiretivoAppService(ICorpoDiretivoService baseService, IConfiguracaoService confService): base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<CORPO_DIRETIVO> GetAllItens(Int32 idAss)
        {
            List<CORPO_DIRETIVO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public CORPO_DIRETIVO CheckExist(CORPO_DIRETIVO obj, Int32 idAss)
        {
            CORPO_DIRETIVO item = _baseService.CheckExist(obj, idAss);
            return item;
        }

        public List<CORPO_DIRETIVO> GetAllItensAdm(Int32 idAss)
        {
            List<CORPO_DIRETIVO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CORPO_DIRETIVO GetItemById(Int32 id)
        {
            CORPO_DIRETIVO item = _baseService.GetItemById(id);
            return item;
        }

        public List<FUNCAO_CORPO_DIRETIVO> GetAllFuncoes(Int32 idAss)
        {
            List<FUNCAO_CORPO_DIRETIVO> lista = _baseService.GetAllFuncoes(idAss);
            return lista;
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            List<USUARIO> lista = _baseService.GetAllUsuarios(idAss);
            return lista;
        }

        public Int32 ValidateCreate(CORPO_DIRETIVO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                List<CORPO_DIRETIVO> lista = _baseService.GetAllItens(usuario.ASSI_CD_ID);
                List<CORPO_DIRETIVO> lista_proc = lista.Where(p => p.USUA_CD_ID == item.USUA_CD_ID & p.CODI_DT_FINAL == null).ToList();
                if (lista_proc.Count > 0)
                {
                    return 1;
                }

                if (item.FUCO_CD_ID != 4)
                {
                    // Verifica se cargo já está preenchido
                    lista_proc = lista.Where(p => p.FUCO_CD_ID == item.FUCO_CD_ID & p.CODI_DT_FINAL == null).ToList();
                    if (lista_proc.Count > 0)
                    {
                        return 2;
                    }
                }
                else
                {
                    // Verifica numero de conselheiros
                    lista_proc = lista.Where(p => p.FUCO_CD_ID == 4 & p.CODI_DT_FINAL == null).ToList();
                    CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);
                    if (lista_proc.Count >= conf.CONF_NR_NUMERO_CONSELHEIROS)
                    {
                        return 3;
                    }
                }

                // Completa objeto
                item.CODI_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCODI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CORPO_DIRETIVO>(item)
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

        public Int32 ValidateEdit(CORPO_DIRETIVO item, CORPO_DIRETIVO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCODI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CORPO_DIRETIVO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CORPO_DIRETIVO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CORPO_DIRETIVO item, CORPO_DIRETIVO itemAntes)
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

        public Int32 ValidateDelete(CORPO_DIRETIVO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CODI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleCODI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CORPO_DIRETIVO>(item),
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CORPO_DIRETIVO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CODI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCODI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CORPO_DIRETIVO>(item),
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
