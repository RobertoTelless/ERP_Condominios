using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITarefaService : IServiceBase<TAREFA>
    {
        Int32 Create(TAREFA tarefa, LOG log);
        Int32 Create(TAREFA tarefa);
        Int32 Edit(TAREFA tarefa, LOG log);
        Int32 Edit(TAREFA tarefa);
        Int32 Delete(TAREFA tarefa, LOG log);

        TAREFA CheckExist(TAREFA tarefa, Int32 idAss);
        TAREFA GetItemById(Int32 id);
        List<TAREFA> GetByDate(DateTime data, Int32 idAss);
        List<TAREFA> GetByUser(Int32 user);
        List<TAREFA> GetTarefaStatus(Int32 user, Int32 tipo);
        List<TAREFA> GetAllItens(Int32 idAss);
        List<TAREFA> GetAllItensAdm(Int32 idAss);
        List<USUARIO> GetAllUsers(Int32 idAss);
        List<TIPO_TAREFA> GetAllTipos();
        TAREFA_ANEXO GetAnexoById(Int32 id);
        USUARIO GetUserById(Int32 id);
        List<PERIODICIDADE_TAREFA> GetAllPeriodicidade();
        List<TAREFA> ExecuteFilter(Int32? tipoId, String titulo, DateTime? data, Int32 encerrado, Int32 prioridade, Int32? usuario, Int32 idAss);
    }
}
