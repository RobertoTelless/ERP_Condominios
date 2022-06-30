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
    public class AutorizacaoAppService : AppServiceBase<AUTORIZACAO_ACESSO>, IAutorizacaoAppService
    {
        private readonly IAutorizacaoService _baseService;

        public AutorizacaoAppService(IAutorizacaoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<AUTORIZACAO_ACESSO> GetAllItens(Int32 idAss)
        {
            List<AUTORIZACAO_ACESSO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<AUTORIZACAO_ACESSO> GetAllItensAdm(Int32 idAss)
        {
            List<AUTORIZACAO_ACESSO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public AUTORIZACAO_ACESSO GetItemById(Int32 id)
        {
            AUTORIZACAO_ACESSO item = _baseService.GetItemById(id);
            return item;
        }

        public AUTORIZACAO_ACESSO CheckExist(AUTORIZACAO_ACESSO conta, Int32 idAss)
        {
            AUTORIZACAO_ACESSO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_DOCUMENTO> GetAllTipos(Int32 idAss)
        {
            List<TIPO_DOCUMENTO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss)
        {
            List<GRAU_PARENTESCO> lista = _baseService.GetAllGraus(idAss);
            return lista;
        }

        public AUTORIZACAO_ACESSO_ANEXO GetAnexoById(Int32 id)
        {
            AUTORIZACAO_ACESSO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? unid, String nome, String documento, String empresa, Int32? tipo, DateTime? data, Int32 idAss, out List<AUTORIZACAO_ACESSO> objeto)
        {
            try
            {
                objeto = new List<AUTORIZACAO_ACESSO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(unid, nome, documento, empresa, tipo, data, idAss);
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

        public Int32 ValidateCreate(AUTORIZACAO_ACESSO item, USUARIO usuario)
        {
            try
            {
                // Criticas
                if (String.IsNullOrEmpty(item.AUAC_NR_DOCUMENTO))
                {
                    item.AUAC_NR_DOCUMENTO = "-";
                }              
                
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.AUAC_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddAUAC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AUTORIZACAO_ACESSO>(item)
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

        public Int32 ValidateEdit(AUTORIZACAO_ACESSO item, AUTORIZACAO_ACESSO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditAUAC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<AUTORIZACAO_ACESSO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<AUTORIZACAO_ACESSO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(AUTORIZACAO_ACESSO item, AUTORIZACAO_ACESSO itemAntes)
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

        public Int32 ValidateDelete(AUTORIZACAO_ACESSO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.ENTRADA_SAIDA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.AUAC_IN_ATIVO = 0;
                item.ASSINANTE = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelAUAC",
                    LOG_TX_REGISTRO = "Autorização: " + item.AUAC_CD_ID.ToString() + "|" + item.UNID_CD_ID + "|" + item.AUAC_DT_INICIO.Value.ToShortDateString() + "|" + item.AUAC_NM_VISITANTE
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(AUTORIZACAO_ACESSO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.AUAC_IN_ATIVO = 1;
                item.ASSINANTE = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatAUAC",
                    LOG_TX_REGISTRO = "Autorização: " + item.AUAC_CD_ID.ToString() + "|" + item.UNID_CD_ID + "|" + item.AUAC_DT_INICIO.Value.ToShortDateString() + "|" + item.AUAC_NM_VISITANTE
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
    }
}
