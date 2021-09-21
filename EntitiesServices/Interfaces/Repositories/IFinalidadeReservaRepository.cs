using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFinalidadeReservaRepository : IRepositoryBase<FINALIDADE_RESERVA>
    {
        List<FINALIDADE_RESERVA> GetAllItens(Int32 idAss);
        FINALIDADE_RESERVA GetItemById(Int32 id);
    }
}
