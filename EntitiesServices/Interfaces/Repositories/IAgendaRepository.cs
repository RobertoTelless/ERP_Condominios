using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAgendaRepository : IRepositoryBase<AGENDA>
    {
        List<AGENDA> GetByDate(DateTime data);
        List<AGENDA> GetByUser(Int32 id);
        AGENDA GetItemById(Int32 id);
        List<AGENDA> GetAllItens();
        List<AGENDA> GetAllItensAdm();
        List<AGENDA> ExecuteFilter(DateTime? data, Int32? cat, String titulo, String descricao);
    }
}

