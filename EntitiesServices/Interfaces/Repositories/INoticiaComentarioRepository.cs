using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INoticiaComentarioRepository : IRepositoryBase<NOTICIA_COMENTARIO>
    {
        List<NOTICIA_COMENTARIO> GetAllItens();
        NOTICIA_COMENTARIO GetItemById(Int32 id);
    }
}
