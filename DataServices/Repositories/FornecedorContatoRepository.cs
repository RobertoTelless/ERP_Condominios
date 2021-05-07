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
    public class FornecedorContatoRepository : RepositoryBase<FORNECEDOR_CONTATO>, IFornecedorContatoRepository
    {
        public List<FORNECEDOR_CONTATO> GetAllItens()
        {
            return Db.FORNECEDOR_CONTATO.ToList();
        }

        public FORNECEDOR_CONTATO GetItemById(Int32 id)
        {
            IQueryable<FORNECEDOR_CONTATO> query = Db.FORNECEDOR_CONTATO.Where(p => p.FOCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 