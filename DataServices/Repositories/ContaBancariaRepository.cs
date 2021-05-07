using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class ContaBancariaRepository : RepositoryBase<CONTA_BANCO>, IContaBancariaRepository
    {
        public CONTA_BANCO CheckExist(CONTA_BANCO conta)
        {
            Int32? idCond = SessionMocks.IdAssinante;
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO;
            query = query.Where(p => p.COBA_NR_AGENCIA == conta.COBA_NR_AGENCIA);
            query = query.Where(p => p.COBA_NR_CONTA == conta.COBA_NR_CONTA);
            query = query.Where(p => p.ASSI_CD_ID == idCond);
            return query.FirstOrDefault();
        }

        public CONTA_BANCO GetItemById(Int32 id)
        {
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO;
            query = query.Where(p => p.COBA_CD_ID == id);
            query = query.Include(p => p.CONTA_PAGAR);
            query = query.Include(p => p.CONTA_RECEBER);
            query = query.Include(p => p.BANCO);
            query = query.Include(p => p.TIPO_CONTA);
            query = query.Include(p => p.CONTA_BANCO_CONTATO);
            query = query.Include(p => p.CONTA_BANCO_LANCAMENTO);
            return query.FirstOrDefault();
        }

        public CONTA_BANCO GetContaPadrao()
        {
            Int32? idCond = SessionMocks.IdAssinante;
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO.Where(p => p.COBA_IN_ATIVO == 1);
            query = query.Where(p => p.COBA_IN_CONTA_PADRAO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idCond);
            return query.FirstOrDefault();
        }

        public List<CONTA_BANCO> GetAllItens()
        {
            Int32? idCond = SessionMocks.IdAssinante;
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO.Where(p => p.COBA_IN_ATIVO == 1);
            query = query.Include(p => p.CONTA_PAGAR);
            query = query.Include(p => p.CONTA_RECEBER);
            query = query.Include(p => p.BANCO);
            query = query.Include(p => p.TIPO_CONTA);
            query = query.Where(p => p.ASSI_CD_ID == idCond);
            return query.ToList();
        }

        public Decimal GetTotalContas()
        {
            Int32? idCond = SessionMocks.IdAssinante;
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO.Where(p => p.COBA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idCond);
            return query.Sum(p => p.COBA_VL_SALDO_ATUAL).Value;
        }

        public List<CONTA_BANCO> GetAllItensAdm()
        {
            Int32? idCond = SessionMocks.IdAssinante;
            IQueryable<CONTA_BANCO> query = Db.CONTA_BANCO;
            query = query.Include(p => p.CONTA_PAGAR);
            query = query.Include(p => p.CONTA_RECEBER);
            query = query.Include(p => p.BANCO);
            query = query.Include(p => p.TIPO_CONTA);
            query = query.Where(p => p.ASSI_CD_ID == idCond);
            return query.ToList();
        }
    }
}
 