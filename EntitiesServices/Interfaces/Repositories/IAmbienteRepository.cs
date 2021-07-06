using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAmbienteRepository : IRepositoryBase<AMBIENTE>
    {
        AMBIENTE CheckExist(AMBIENTE item, Int32 idAss);
        AMBIENTE GetItemById(Int32 id);
        List<AMBIENTE> GetAllItens(Int32 idAss);
        List<AMBIENTE> GetAllItensAdm(Int32 idAss);
        List<AMBIENTE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss);
    }
}
