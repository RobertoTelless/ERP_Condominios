using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class TipoContaRepository : RepositoryBase<TIPO_CONTA>, ITipoContaRepository
    {
        public TIPO_CONTA GetItemById(Int32 id)
        {
            IQueryable<TIPO_CONTA> query = Db.TIPO_CONTA;
            query = query.Where(p => p.TICO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_CONTA> GetAllItens()
        {
            IQueryable<TIPO_CONTA> query = Db.TIPO_CONTA;
            return query.ToList();
        }
    }
}
 