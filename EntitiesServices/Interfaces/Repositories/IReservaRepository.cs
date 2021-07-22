using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IReservaRepository : IRepositoryBase<RESERVA>
    {
        RESERVA CheckExist(RESERVA item, Int32 idAss);
        RESERVA GetItemById(Int32 id);
        List<RESERVA> GetAllItens(Int32 idAss);
        List<RESERVA> GetAllItensAdm(Int32 idAss);
        List<RESERVA> GetByUnidade(Int32 idUnid);
        List<RESERVA> ExecuteFilter(String nome, DateTime? data, Int32? finalidade, Int32? ambiente, Int32? unidade, Int32 idAss);
    }
}
