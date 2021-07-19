using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CategoriaProdutoRepository : RepositoryBase<CATEGORIA_PRODUTO>, ICategoriaProdutoRepository
    {
        public CATEGORIA_PRODUTO CheckExist(CATEGORIA_PRODUTO conta, Int32 idAss)
        {
            IQueryable<CATEGORIA_PRODUTO> query = Db.CATEGORIA_PRODUTO;
            query = query.Where(p => p.CAPR_NM_NOME == conta.CAPR_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CATEGORIA_PRODUTO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_PRODUTO> query = Db.CATEGORIA_PRODUTO;
            query = query.Where(p => p.CAPR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_PRODUTO> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_PRODUTO> query = Db.CATEGORIA_PRODUTO.Where(p => p.CAPR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_PRODUTO> query = Db.CATEGORIA_PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
