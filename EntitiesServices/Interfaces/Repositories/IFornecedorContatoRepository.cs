using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFornecedorContatoRepository : IRepositoryBase<FORNECEDOR_CONTATO>
    {
        List<FORNECEDOR_CONTATO> GetAllItens();
        FORNECEDOR_CONTATO GetItemById(Int32 id);
    }
}
