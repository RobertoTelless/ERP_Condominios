using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITarefaAnexoRepository : IRepositoryBase<TAREFA_ANEXO>
    {
        List<TAREFA_ANEXO> GetAllItens();
        TAREFA_ANEXO GetItemById(Int32 id);
    }
}
