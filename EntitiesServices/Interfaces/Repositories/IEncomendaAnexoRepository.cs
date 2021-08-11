using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEncomendaAnexoRepository : IRepositoryBase<ENCOMENDA_ANEXO>
    {
        List<ENCOMENDA_ANEXO> GetAllItens();
        ENCOMENDA_ANEXO GetItemById(Int32 id);
    }
}
