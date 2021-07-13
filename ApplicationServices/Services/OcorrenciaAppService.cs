using System;
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
    public class OcorrenciaAppService : AppServiceBase<OCORRENCIA>, IOcorrenciaAppService
    {
        private readonly IOcorrenciaService _baseService;

        public OcorrenciaAppService(IOcorrenciaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<OCORRENCIA> GetAllItens(Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<OCORRENCIA> GetAllItensAdm(Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CATEGORIA_OCORRENCIA> GetAllCategorias(Int32 idAss)
        {
            List<CATEGORIA_OCORRENCIA> lista = _baseService.GetAllCategorias(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllUnidades(idAss);
            return lista;
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            List<USUARIO> lista = _baseService.GetAllUsuarios(idAss);
            return lista;
        }

        public OCORRENCIA_ANEXO GetAnexoById(Int32 id)
        {
            OCORRENCIA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public OCORRENCIA GetItemById(Int32 id)
        {
            OCORRENCIA item = _baseService.GetItemById(id);
            return item;
        }

        public List<OCORRENCIA> GetAllItensUser(Int32 id, Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItensUser(id, idAss);
            return lista;
        }

        public List<OCORRENCIA> GetAllItensUnidade(Int32 id, Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetAllItensUnidade(id, idAss);
            return lista;
        }

        public List<OCORRENCIA> GetOcorrenciasNovas(Int32 id, Int32 idAss)
        {
            List<OCORRENCIA> lista = _baseService.GetOcorrenciasNovas(id, idAss);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? unidade, Int32? usuario, Int32? cat,String titulo, DateTime? data, String texto, Int32 idAss, out List<OCORRENCIA> objeto)
        {
            try
            {
                objeto = new List<OCORRENCIA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(unidade, usuario, cat, titulo, data, texto, idAss);
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

        public Int32 ValidateCreate(OCORRENCIA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.OCOR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddOCOR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item)
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

        public Int32 ValidateEdit(OCORRENCIA item, OCORRENCIA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditOCOR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<OCORRENCIA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(OCORRENCIA item, OCORRENCIA itemAntes)
        {
            try
            {

                // Persiste
                item.USUARIO = null;
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(OCORRENCIA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.OCOR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelOCOR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(OCORRENCIA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.OCOR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatOCOR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<OCORRENCIA>(item)
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
