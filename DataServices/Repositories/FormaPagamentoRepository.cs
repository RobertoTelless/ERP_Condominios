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
    public class FormaPagamentoRepository : RepositoryBase<FORMA_PAGAMENTO>, IFormaPagamentoRepository
    {
        public FORMA_PAGAMENTO GetItemById(Int32 id)
        {
            IQueryable<FORMA_PAGAMENTO> query = Db.FORMA_PAGAMENTO;
            query = query.Where(p => p.FOPA_CD_ID == id);
            query = query.Include(p => p.CONTA_BANCO);
            return query.FirstOrDefault();
        }

        public List<FORMA_PAGAMENTO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FORMA_PAGAMENTO> query = Db.FORMA_PAGAMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.CONTA_BANCO);
            return query.ToList();
        }

        public List<FORMA_PAGAMENTO> GetAllItens(Int32 tipo)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FORMA_PAGAMENTO> query = Db.FORMA_PAGAMENTO.Where(p => p.FOPA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            if (tipo == 1)
            {
                query = query.Where(p => p.FOPA_IN_TIPO == 1 || p.FOPA_IN_TIPO == 3);
            }
            else
            {
                query = query.Where(p => p.FOPA_IN_TIPO == 2 || p.FOPA_IN_TIPO == 3);
            }
            query = query.Include(p => p.CONTA_BANCO);
            return query.ToList();
        }

        public List<FORMA_PAGAMENTO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<FORMA_PAGAMENTO> query = Db.FORMA_PAGAMENTO.Where(p => p.FOPA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.CONTA_BANCO);
            return query.ToList();
        }

    }
}
 