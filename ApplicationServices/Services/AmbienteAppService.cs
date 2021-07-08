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
    public class AmbienteAppService : AppServiceBase<AMBIENTE>, IAmbienteAppService
    {
        private readonly IAmbienteService _baseService;

        public AmbienteAppService(IAmbienteService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<AMBIENTE> GetAllItens(Int32 idAss)
        {
            List<AMBIENTE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<AMBIENTE> GetAllItensAdm(Int32 idAss)
        {
            List<AMBIENTE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public AMBIENTE GetItemById(Int32 id)
        {
            AMBIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public AMBIENTE CheckExist(AMBIENTE conta, Int32 idAss)
        {
            AMBIENTE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_AMBIENTE> GetAllTipos(Int32 idAss)
        {
            List<TIPO_AMBIENTE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public AMBIENTE_IMAGEM GetAnexoById(Int32 id)
        {
            AMBIENTE_IMAGEM lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? tipo, String nome, Int32 idAss, out List<AMBIENTE> objeto)
        {
            try
            {
                objeto = new List<AMBIENTE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, nome, idAss);
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

        public Int32 ValidateCreate(AMBIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.AMBI_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddAMBI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item)
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

        public Int32 ValidateEdit(AMBIENTE item, AMBIENTE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditAMBI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<AMBIENTE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(AMBIENTE item, AMBIENTE itemAntes)
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

        public Int32 ValidateDelete(AMBIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.RESERVA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.AMBI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelAMBI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(AMBIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.AMBI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatAMBI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AMBIENTE>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
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

        public AMBIENTE_CUSTO GetAmbienteCustoById(Int32 id)
        {
            AMBIENTE_CUSTO lista = _baseService.GetAmbienteCustoById(id);
            return lista;
        }

        public Int32 ValidateEditAmbienteCusto(AMBIENTE_CUSTO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAmbienteCusto(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAmbienteCusto(AMBIENTE_CUSTO item)
        {
            try
            {
                // Persiste
                item.AMCU_NM_ATIVO = 1;
                Int32 volta = _baseService.CreateAmbienteCusto(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AMBIENTE_CHAVE GetAmbienteChaveById(Int32 id)
        {
            AMBIENTE_CHAVE lista = _baseService.GetAmbienteChaveById(id);
            return lista;
        }

        public Int32 ValidateEditAmbienteChave(AMBIENTE_CHAVE item)
        {
            try
            {
                // Persiste
                return _baseService.EditAmbienteChave(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAmbienteChave(AMBIENTE_CHAVE item)
        {
            try
            {
                // Critica
                if (item.AMCH_DT_ENTREGA > DateTime.Today.Date)
                {
                    return 1;
                }

                // Persiste
                item.AMCH_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateAmbienteChave(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
