using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFornecedorAnexoRepository : IRepositoryBase<FORNECEDOR_ANEXO>
    {
        List<FORNECEDOR_ANEXO> GetAllItens(Int32 idAss);
        FORNECEDOR_ANEXO GetItemById(Int32 id);
    }
}
