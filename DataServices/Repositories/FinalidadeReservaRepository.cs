using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class FinalidadeReservaRepository : RepositoryBase<FINALIDADE_RESERVA>, IFinalidadeReservaRepository
    {
        public FINALIDADE_RESERVA GetItemById(Int32 id)
        {
            IQueryable<FINALIDADE_RESERVA> query = Db.FINALIDADE_RESERVA;
            query = query.Where(p => p.FIRE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FINALIDADE_RESERVA> GetAllItens(Int32 idAss)
        {
            IQueryable<FINALIDADE_RESERVA> query = Db.FINALIDADE_RESERVA;
            return query.ToList();
        }
    }
}
 