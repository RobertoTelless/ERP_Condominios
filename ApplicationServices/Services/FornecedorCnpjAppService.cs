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
    public class FornecedorCnpjAppService : AppServiceBase<FORNECEDOR_QUADRO_SOCIETARIO>, IFornecedorCnpjAppService
    {
        private readonly IFornecedorCnpjService _baseService;

        public FornecedorCnpjAppService(IFornecedorCnpjService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public FORNECEDOR_QUADRO_SOCIETARIO CheckExist(FORNECEDOR_QUADRO_SOCIETARIO fqs, Int32 idAss)
        {
            FORNECEDOR_QUADRO_SOCIETARIO item = _baseService.CheckExist(fqs, idAss);
            return item;
        }

        public List<FORNECEDOR_QUADRO_SOCIETARIO> GetAllItens(Int32 idAss)
        {
            List<FORNECEDOR_QUADRO_SOCIETARIO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<FORNECEDOR_QUADRO_SOCIETARIO> GetByFornecedor(FORNECEDOR fornecedor)
        {
            List<FORNECEDOR_QUADRO_SOCIETARIO> lista = _baseService.GetByFornecedor(fornecedor);
            return lista;
        }

        public Int32 ValidateCreate(FORNECEDOR_QUADRO_SOCIETARIO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.FOQS_IN_ATIVO = 1;

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