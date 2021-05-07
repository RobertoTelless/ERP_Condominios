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
using CrossCutting;

namespace DataServices.Repositories
{
    public class FornecedorCnpjRepository : RepositoryBase<FORNECEDOR_QUADRO_SOCIETARIO>, IFornecedorCnpjRepository
    {
        public FORNECEDOR_QUADRO_SOCIETARIO CheckExist(FORNECEDOR_QUADRO_SOCIETARIO fqs)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FORNECEDOR_QUADRO_SOCIETARIO> query = Db.FORNECEDOR_QUADRO_SOCIETARIO;
            query = query.Where(p => p.FORN_CD_ID == fqs.FORN_CD_ID);
            return query.FirstOrDefault();
        }

        public List<FORNECEDOR_QUADRO_SOCIETARIO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FORNECEDOR_QUADRO_SOCIETARIO> query = Db.FORNECEDOR_QUADRO_SOCIETARIO;
            return query.ToList();
        }

        public List<FORNECEDOR_QUADRO_SOCIETARIO> GetByFornecedor(FORNECEDOR fornecedor)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FORNECEDOR_QUADRO_SOCIETARIO> query = Db.FORNECEDOR_QUADRO_SOCIETARIO;
            query = query.Where(p => p.FORN_CD_ID == fornecedor.FORN_CD_ID);
            return query.ToList();
        }
    }
}