using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaFornecedorRepository : IRepositoryBase<CATEGORIA_FORNECEDOR>
    {
        List<CATEGORIA_FORNECEDOR> GetAllItens(Int32 idAss);
        CATEGORIA_FORNECEDOR GetItemById(Int32 id);
        List<CATEGORIA_FORNECEDOR> GetAllItensAdm(Int32 idAss);
    }
}
