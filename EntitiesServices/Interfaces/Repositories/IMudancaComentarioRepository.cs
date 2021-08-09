using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMudancaComentarioRepository : IRepositoryBase<SOLICITACAO_MUDANCA_COMENTARIO>
    {
        List<SOLICITACAO_MUDANCA_COMENTARIO> GetAllItens();
        SOLICITACAO_MUDANCA_COMENTARIO GetItemById(Int32 id);
    }
}
