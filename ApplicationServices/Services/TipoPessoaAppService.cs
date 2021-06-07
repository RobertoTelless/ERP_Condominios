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
    public class TipoPessoaAppService : AppServiceBase<TIPO_PESSOA>, ITipoPessoaAppService
    {
        private readonly ITipoPessoaService _baseService;

        public TipoPessoaAppService(ITipoPessoaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_PESSOA> GetAllItens()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<TIPO_PESSOA> GetAllItensAdm()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public TIPO_PESSOA GetItemById(Int32 id)
        {
            TIPO_PESSOA item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(TIPO_PESSOA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.TIPE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTIPE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_PESSOA>(item)
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

        public Int32 ValidateEdit(TIPO_PESSOA item, TIPO_PESSOA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTIPE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_PESSOA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_PESSOA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TIPO_PESSOA item, TIPO_PESSOA itemAntes)
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

        public Int32 ValidateDelete(TIPO_PESSOA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                //if (item.USUARIO.Count > 0)
                //{
                //    return 1;
                //}

                // Acerta campos
                item.TIPE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleTIPE",
                    LOG_TX_REGISTRO = "Tipo de Pessoa: " + item.TIPE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_PESSOA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIPE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTIPE",
                    LOG_TX_REGISTRO = "Tipo de Pessoa: " + item.TIPE_NM_NOME
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
