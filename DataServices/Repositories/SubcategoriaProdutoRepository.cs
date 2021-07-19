using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class SubcategoriaProdutoRepository : RepositoryBase<SUBCATEGORIA_PRODUTO>, ISubcategoriaProdutoRepository
    {
        public SUBCATEGORIA_PRODUTO CheckExist(SUBCATEGORIA_PRODUTO conta, Int32 idAss)
        {
            IQueryable<SUBCATEGORIA_PRODUTO> query = Db.SUBCATEGORIA_PRODUTO;
            query = query.Where(p => p.CAPR_CD_ID == conta.CAPR_CD_ID);
            query = query.Where(p => p.SCPR_NM_NOME == conta.SCPR_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public SUBCATEGORIA_PRODUTO GetItemById(Int32 id)
        {
            IQueryable<SUBCATEGORIA_PRODUTO> query = Db.SUBCATEGORIA_PRODUTO;
            query = query.Where(p => p.SCPR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllItens(Int32 idAss)
        {
            IQueryable<SUBCATEGORIA_PRODUTO> query = Db.SUBCATEGORIA_PRODUTO.Where(p => p.SCPR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<SUBCATEGORIA_PRODUTO> query = Db.SUBCATEGORIA_PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
