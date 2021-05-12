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
    public class ConfiguracaoAppService : AppServiceBase<CONFIGURACAO>, IConfiguracaoAppService
    {
        private readonly IConfiguracaoService _baseService;

        public ConfiguracaoAppService(IConfiguracaoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public CONFIGURACAO GetItemById(Int32 id)
        {
            return _baseService.GetItemById(id);
        }

        public List<CONFIGURACAO> GetAllItems(Int32 idAss)
        {
            return _baseService.GetAllItems(idAss);
        }

        public Int32 ValidateEdit(CONFIGURACAO item, CONFIGURACAO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditCONF",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONFIGURACAO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONFIGURACAO>(itemAntes),
                    LOG_IN_ATIVO = 1
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(CONFIGURACAO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
