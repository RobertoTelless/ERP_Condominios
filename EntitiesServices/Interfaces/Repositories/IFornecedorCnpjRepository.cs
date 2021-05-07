using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFornecedorCnpjRepository : IRepositoryBase<FORNECEDOR_QUADRO_SOCIETARIO>
    {
        FORNECEDOR_QUADRO_SOCIETARIO CheckExist(FORNECEDOR_QUADRO_SOCIETARIO fqs);
        List<FORNECEDOR_QUADRO_SOCIETARIO> GetAllItens();
        List<FORNECEDOR_QUADRO_SOCIETARIO> GetByFornecedor(FORNECEDOR fornecedor);

    }
}