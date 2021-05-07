using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFornecedorCnpjAppService : IAppServiceBase<FORNECEDOR_QUADRO_SOCIETARIO>
    {
        List<FORNECEDOR_QUADRO_SOCIETARIO> GetAllItens();
        List<FORNECEDOR_QUADRO_SOCIETARIO> GetByFornecedor(FORNECEDOR fornecedor);
        Int32 ValidateCreate(FORNECEDOR_QUADRO_SOCIETARIO item, USUARIO usuario);
    }
}