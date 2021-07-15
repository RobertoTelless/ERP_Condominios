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
    public class ContaBancariaLancamentoRepository : RepositoryBase<CONTA_BANCO_LANCAMENTO>, IContaBancariaLancamentoRepository
    {
        public List<CONTA_BANCO_LANCAMENTO> GetAllItens(Int32 idAss)
        {
            return Db.CONTA_BANCO_LANCAMENTO.ToList();
        }

        public List<CONTA_BANCO_LANCAMENTO> ExecuteFilter(Int32 conta, DateTime? data, Int32? tipo, String desc)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.CBLA_IN_ATIVO == 1);
            query = query.Where(p => p.COBA_CD_ID == conta);

            if (tipo != null)
            {
                query = query.Where(x => x.CBLA_IN_TIPO == tipo);
            }
            if (desc != null)
            {
                query = query.Where(x => x.CBLA_DS_DESCRICAO == desc);
            }

            List<CONTA_BANCO_LANCAMENTO> lista = query.ToList<CONTA_BANCO_LANCAMENTO>();

            if (data != null)
            {
                lista = lista.Where(x => x.CBLA_DT_LANCAMENTO == data).ToList<CONTA_BANCO_LANCAMENTO>();
            }

            return lista;
        }

        public CONTA_BANCO_LANCAMENTO GetItemById(Int32 id)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.CBLA_CD_ID == id);
            query = query.Include(p => p.CONTA_BANCO);
            return query.FirstOrDefault();
        }

        public Decimal GetTotalReceita(Int32 conta)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.CBLA_IN_TIPO.Value == 1);
            query = query.Where(p => p.COBA_CD_ID == conta);
            return query.Sum(p => p.CBLA_VL_VALOR).Value;
        }

        public Decimal GetTotalDespesa(Int32 conta)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.CBLA_IN_TIPO.Value == 2);
            query = query.Where(p => p.COBA_CD_ID == conta);
            return query.Sum(p => p.CBLA_VL_VALOR).Value;
        }

        public Decimal GetTotalReceitaMes(Int32 conta, Int32 mes)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.CBLA_IN_TIPO.Value == 1);
            query = query.Where(p => p.COBA_CD_ID == conta);
            query = query.Where(p => DbFunctions.TruncateTime(p.CBLA_DT_LANCAMENTO).Value.Month == mes);
            return query.Sum(p => p.CBLA_VL_VALOR).Value;
        }

        public Decimal GetTotalDespesaMes(Int32 conta, Int32 mes)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.CBLA_IN_TIPO.Value == 2);
            query = query.Where(p => p.COBA_CD_ID == conta);
            query = query.Where(p => DbFunctions.TruncateTime(p.CBLA_DT_LANCAMENTO).Value.Month == mes);
            return query.Sum(p => p.CBLA_VL_VALOR).Value;
        }

        public List<CONTA_BANCO_LANCAMENTO> GetLancamentosMes(Int32 conta, Int32 mes)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.COBA_CD_ID == conta);
            query = query.Where(p => DbFunctions.TruncateTime(p.CBLA_DT_LANCAMENTO).Value.Month == mes);
            return query.ToList();
        }

        public List<CONTA_BANCO_LANCAMENTO> GetLancamentosDia(Int32 conta, DateTime data)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.COBA_CD_ID == conta);
            query = query.Where(p => DbFunctions.TruncateTime(p.CBLA_DT_LANCAMENTO).Value == data);
            return query.ToList();
        }

        public List<CONTA_BANCO_LANCAMENTO> GetLancamentosFaixa(Int32 conta, DateTime inicio, DateTime final)
        {
            IQueryable<CONTA_BANCO_LANCAMENTO> query = Db.CONTA_BANCO_LANCAMENTO.Where(p => p.COBA_CD_ID == conta);
            query = query.Where(p => DbFunctions.TruncateTime(p.CBLA_DT_LANCAMENTO).Value >= inicio);
            query = query.Where(p => DbFunctions.TruncateTime(p.CBLA_DT_LANCAMENTO).Value <= final);
            return query.ToList();
        }
    }
}
 