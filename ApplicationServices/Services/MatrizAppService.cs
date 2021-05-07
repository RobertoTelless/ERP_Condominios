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
    public class MatrizAppService : AppServiceBase<MATRIZ>, IMatrizAppService
    {
        private readonly IMatrizService _baseService;

        public MatrizAppService(IMatrizService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<MATRIZ> GetAllItens()
        {
            List<MATRIZ> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<TIPO_PESSOA> GetAllTipoPessoa()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllTipoPessoa();
            return lista;
        }

        public List<UF> GetAllUF()
        {
            return _baseService.GetAllUF();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<MATRIZ> GetAllItensAdm()
        {
            List<MATRIZ> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public MATRIZ GetItemById(Int32 id)
        {
            MATRIZ item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(MATRIZ item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.MATR_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    LOG_NM_OPERACAO = "AddMATR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MATRIZ>(item)
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

        public Int32 ValidateEdit(MATRIZ item, MATRIZ itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                //LOG log = new LOG
                //{
                //    LOG_DT_DATA = DateTime.Now,
                //    USUA_CD_ID = usuario.USUA_CD_ID,
                //    ASSI_CD_ID = SessionMocks.IdAssinante,
                //    LOG_NM_OPERACAO = "EditMATR",
                //    LOG_IN_ATIVO = 1,
                //    LOG_TX_REGISTRO = Serialization.SerializeJSON<MATRIZ>(item),
                //    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<MATRIZ>(itemAntes)
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
