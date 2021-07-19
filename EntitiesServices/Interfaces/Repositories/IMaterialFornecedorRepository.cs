using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMaterialFornecedorRepository : IRepositoryBase<MATERIAL_FORNECEDOR>
    {
        List<MATERIAL_FORNECEDOR> GetAllItens();
        MATERIAL_FORNECEDOR GetItemById(Int32 id);
        MATERIAL_FORNECEDOR GetByProdForn(Int32 forn, Int32 prod);
    }
}
