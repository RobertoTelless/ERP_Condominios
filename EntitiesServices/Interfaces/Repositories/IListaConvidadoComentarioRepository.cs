using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IListaConvidadoComentarioRepository : IRepositoryBase<LISTA_CONVIDADO_COMENTARIO>
    {
        List<LISTA_CONVIDADO_COMENTARIO> GetAllItens();
        LISTA_CONVIDADO_COMENTARIO GetItemById(Int32 id);
    }
}
