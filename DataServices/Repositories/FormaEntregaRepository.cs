using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class FormaEntregaRepository : RepositoryBase<FORMA_ENTREGA>, IFormaEntregaRepository
    {
        public FORMA_ENTREGA GetItemById(Int32 id)
        {
            IQueryable<FORMA_ENTREGA> query = Db.FORMA_ENTREGA;
            query = query.Where(p => p.FOEN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FORMA_ENTREGA> GetAllItens(Int32 idAss)
        {
            IQueryable<FORMA_ENTREGA> query = Db.FORMA_ENTREGA;
            return query.ToList();
        }
    }
}
 