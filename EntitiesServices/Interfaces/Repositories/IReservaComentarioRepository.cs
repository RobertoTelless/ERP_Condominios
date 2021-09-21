using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IReservaComentarioRepository : IRepositoryBase<RESERVA_COMENTARIO>
    {
        List<RESERVA_COMENTARIO> GetAllItens();
        RESERVA_COMENTARIO GetItemById(Int32 id);
    }
}
