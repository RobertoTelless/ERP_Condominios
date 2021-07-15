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
    public class BancoRepository : RepositoryBase<BANCO>, IBancoRepository
    {
        public BANCO CheckExist(BANCO conta, Int32 idAss)
        {
            IQueryable<BANCO> query = Db.BANCO;
            query = query.Where(p => p.BANC_SG_CODIGO == conta.BANC_SG_CODIGO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public BANCO GetByCodigo(String codigo)
        {
            IQueryable<BANCO> query = Db.BANCO.Where(p => p.BANC_IN_ATIVO == 1);
            query = query.Where(p => p.BANC_SG_CODIGO == codigo);
            query = query.Include(p => p.CONTA_BANCO);
            return query.FirstOrDefault();
        }

        public BANCO GetItemById(Int32 id)
        {
            IQueryable<BANCO> query = Db.BANCO;
            query = query.Where(p => p.BANC_CD_ID == id);
            query = query.Include(p => p.CONTA_BANCO);
            return query.FirstOrDefault();
        }

        public List<BANCO> GetAllItens(Int32 idAss)
        {
            IQueryable<BANCO> query = Db.BANCO.Where(p => p.BANC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.CONTA_BANCO);
            return query.ToList();
        }

        public List<BANCO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<BANCO> query = Db.BANCO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.CONTA_BANCO);
            return query.ToList();
        }

        public List<BANCO> ExecuteFilter(String codigo, String nome, Int32 idAss)
        {
            List<BANCO> lista = new List<BANCO>();
            IQueryable<BANCO> query = Db.BANCO;
            if (!String.IsNullOrEmpty(codigo))
            {
                query = query.Where(p => p.BANC_SG_CODIGO == codigo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.BANC_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.BANC_NM_NOME);
                lista = query.ToList<BANCO>();
            }
            return lista;
        }
    }
}
 