using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoAnexoRepository : RepositoryBase<PRODUTO_ANEXO>, IProdutoAnexoRepository
    {
        public List<PRODUTO_ANEXO> GetAllItens()
        {
            return Db.PRODUTO_ANEXO.ToList();
        }

        public PRODUTO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_ANEXO> query = Db.PRODUTO_ANEXO.Where(p => p.PRAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
