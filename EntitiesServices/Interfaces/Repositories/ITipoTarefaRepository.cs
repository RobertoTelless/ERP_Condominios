using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoTarefaRepository : IRepositoryBase<TIPO_TAREFA>
    {
        List<TIPO_TAREFA> GetAllItens();
        TIPO_TAREFA GetItemById(Int32 id);
    }
}
