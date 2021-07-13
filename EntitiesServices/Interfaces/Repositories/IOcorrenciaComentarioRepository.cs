using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IOcorrenciaComentarioRepository : IRepositoryBase<OCORRENCIA_COMENTARIO>
    {
        List<OCORRENCIA_COMENTARIO> GetAllItens();
        OCORRENCIA_COMENTARIO GetItemById(Int32 id);
    }
}
