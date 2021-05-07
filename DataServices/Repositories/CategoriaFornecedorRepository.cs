using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class CategoriaFornecedorRepository : RepositoryBase<CATEGORIA_FORNECEDOR>, ICategoriaFornecedorRepository
    {
        public CATEGORIA_FORNECEDOR GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_FORNECEDOR> query = Db.CATEGORIA_FORNECEDOR;
            query = query.Where(p => p.CAFO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_FORNECEDOR> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_FORNECEDOR> query = Db.CATEGORIA_FORNECEDOR.Where(p => p.CAFO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CATEGORIA_FORNECEDOR> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<CATEGORIA_FORNECEDOR> query = Db.CATEGORIA_FORNECEDOR;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 