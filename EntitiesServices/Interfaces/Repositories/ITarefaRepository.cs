using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITarefaRepository : IRepositoryBase<TAREFA>
    {
        TAREFA CheckExist(TAREFA item);
        List<TAREFA> GetByDate(DateTime data);
        List<TAREFA> GetByUser(Int32 user);
        List<TAREFA> GetTarefaStatus(Int32 user, Int32 tipo);
        TAREFA GetItemById(Int32 id);
        List<TAREFA> GetAllItens();
        List<TAREFA> GetAllItensAdm();
        List<PERIODICIDADE_TAREFA> GetAllPeriodicidade();
        List<TAREFA> ExecuteFilter(Int32? tipoId, String titulo, DateTime? data, Int32 encerrada, Int32 prioridade, Int32? usuario);
    }
}
