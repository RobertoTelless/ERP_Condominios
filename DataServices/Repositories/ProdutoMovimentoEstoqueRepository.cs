using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoMovimentoEstoqueRepository : RepositoryBase<MOVIMENTO_ESTOQUE_PRODUTO>, IProdutoMovimentoEstoqueRepository
    {
        public MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(p => p.MOEP_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens()
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            return query.ToList();
        }
    }
}
