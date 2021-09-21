using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ReservaComentarioRepository : RepositoryBase<RESERVA_COMENTARIO>, IReservaComentarioRepository
    {
        public List<RESERVA_COMENTARIO> GetAllItens()
        {
            return Db.RESERVA_COMENTARIO.ToList();
        }

        public RESERVA_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<RESERVA_COMENTARIO> query = Db.RESERVA_COMENTARIO.Where(p => p.RECO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 