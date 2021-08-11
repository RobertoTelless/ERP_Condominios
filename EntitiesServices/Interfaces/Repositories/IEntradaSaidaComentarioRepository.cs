using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEntradaSaidaComentarioRepository : IRepositoryBase<ENTRADA_SAIDA_COMENTARIO>
    {
        List<ENTRADA_SAIDA_COMENTARIO> GetAllItens();
        ENTRADA_SAIDA_COMENTARIO GetItemById(Int32 id);
    }
}
