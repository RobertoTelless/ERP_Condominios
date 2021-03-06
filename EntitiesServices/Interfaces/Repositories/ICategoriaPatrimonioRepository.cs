using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaPatrimonioRepository : IRepositoryBase<CATEGORIA_PATRIMONIO>
    {
        List<CATEGORIA_PATRIMONIO> GetAllItens();
        CATEGORIA_PATRIMONIO GetItemById(Int32 id);
        List<CATEGORIA_PATRIMONIO> GetAllItensAdm();
    }
}
