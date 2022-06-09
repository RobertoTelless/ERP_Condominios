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
        List<AGENDA> GetByDate(DateTime data, Int32 idAss);
        List<AGENDA> GetByUser(Int32 id, Int32 idAss);
        AGENDA GetItemById(Int32 id);
        List<AGENDA> GetAllItens(Int32 idAss);
        List<AGENDA> GetAllItensAdm(Int32 idAss);
        List<AGENDA> ExecuteFilter(DateTime? data, Int32? cat, String titulo, String descricao, Int32 idAss, Int32 idUser, Int32 corp);
    }
}

