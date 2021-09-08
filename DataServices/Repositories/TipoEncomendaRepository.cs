using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class TipoEncomendaRepository : RepositoryBase<TIPO_ENCOMENDA>, ITipoEncomendaRepository
    {
        public TIPO_ENCOMENDA GetItemById(Int32 id)
        {
            IQueryable<TIPO_ENCOMENDA> query = Db.TIPO_ENCOMENDA;
            query = query.Where(p => p.TIEN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_ENCOMENDA> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_ENCOMENDA> query = Db.TIPO_ENCOMENDA;
            return query.ToList();
        }
    }
}
 