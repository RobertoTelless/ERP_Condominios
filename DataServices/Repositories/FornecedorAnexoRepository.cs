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
    public class FornecedorAnexoRepository : RepositoryBase<FORNECEDOR_ANEXO>, IFornecedorAnexoRepository
    {
        public List<FORNECEDOR_ANEXO> GetAllItens()
        {
            return Db.FORNECEDOR_ANEXO.ToList();
        }

        public FORNECEDOR_ANEXO GetItemById(Int32 id)
        {
            IQueryable<FORNECEDOR_ANEXO> query = Db.FORNECEDOR_ANEXO.Where(p => p.FOAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 