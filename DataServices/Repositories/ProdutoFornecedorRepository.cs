using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoFornecedorRepository : RepositoryBase<PRODUTO_FORNECEDOR>, IProdutoFornecedorRepository
    {
        public List<PRODUTO_FORNECEDOR> GetAllItens()
        {
            return Db.PRODUTO_FORNECEDOR.ToList();
        }

        public PRODUTO_FORNECEDOR GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_FORNECEDOR> query = Db.PRODUTO_FORNECEDOR.Where(p => p.PRFO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public PRODUTO_FORNECEDOR GetByProdForn(Int32 forn, Int32 prod)
        {
            IQueryable<PRODUTO_FORNECEDOR> query = Db.PRODUTO_FORNECEDOR;
            query = query.Where(x => x.PROD_CD_ID == prod);
            query = query.Where(x => x.FORN_CD_ID == forn);
            return query.FirstOrDefault();
        }
    }
}
