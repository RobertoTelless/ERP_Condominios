using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEncomendaComentarioRepository : IRepositoryBase<ENCOMENDA_COMENTARIO>
    {
        List<ENCOMENDA_COMENTARIO> GetAllItens();
        ENCOMENDA_COMENTARIO GetItemById(Int32 id);
    }
}
