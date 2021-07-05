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
    public class CategoriaTelefoneAppService : AppServiceBase<CATEGORIA_TELEFONE>, ICategoriaTelefoneAppService
    {
        private readonly ICategoriaTelefoneService _baseService;

        public CategoriaTelefoneAppService(ICategoriaTelefoneService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<CATEGORIA_TELEFONE> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_TELEFONE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_TELEFONE> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_TELEFONE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_TELEFONE GetItemById(Int32 id)
        {
            CATEGORIA_TELEFONE item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_TELEFONE item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.CATE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCATE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_TELEFONE>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CATEGORIA_TELEFONE item, CATEGORIA_TELEFONE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCATE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_TELEFONE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CATEGORIA_TELEFONE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CATEGORIA_TELEFONE item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.TELEFONE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CATE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCATE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_TELEFONE>(item)
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_TELEFONE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CATE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCATE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CATEGORIA_TELEFONE>(item)
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
