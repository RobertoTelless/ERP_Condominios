using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class ContaBancariaContatoRepository : RepositoryBase<CONTA_BANCO_CONTATO>, IContaBancariaContatoRepository
    {
        public List<CONTA_BANCO_CONTATO> GetAllItens()
        {
            return Db.CONTA_BANCO_CONTATO.ToList();
        }

        public CONTA_BANCO_CONTATO GetItemById(Int32 id)
        {
            IQueryable<CONTA_BANCO_CONTATO> query = Db.CONTA_BANCO_CONTATO.Where(p => p.CBCT_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 