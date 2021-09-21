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
    public class ReservaAnexoRepository : RepositoryBase<RESERVA_ANEXO>, IReservaAnexoRepository
    {
        public List<RESERVA_ANEXO> GetAllItens(Int32 idAss)
        {
            return Db.RESERVA_ANEXO.ToList();
        }

        public RESERVA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<RESERVA_ANEXO> query = Db.RESERVA_ANEXO.Where(p => p.REAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 