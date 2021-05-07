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
    public class AssinanteRepository : RepositoryBase<ASSINANTE>, IAssinanteRepository
    {
        public ASSINANTE CheckExist(ASSINANTE conta)
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            query = query.Where(p => p.ASSI_NM_NOME == conta.ASSI_NM_NOME);
            return query.FirstOrDefault();
        }

        public ASSINANTE GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            query = query.Where(p => p.ASSI_CD_ID == id);
            query = query.Include(p => p.USUARIO);
            return query.FirstOrDefault();
        }

        public List<ASSINANTE> GetAllItens()
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE.Where(p => p.ASSI_IN_ATIVO == 1);
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<ASSINANTE> GetAllItensAdm()
        {
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            query = query.Include(p => p.USUARIO);
            return query.ToList();
        }

        public List<ASSINANTE> ExecuteFilter(Int32 tipo, String nome)
        {
            List<ASSINANTE> lista = new List<ASSINANTE>();
            IQueryable<ASSINANTE> query = Db.ASSINANTE;
            if (tipo > 0)
            {
                query = query.Where(p => p.ASSI_IN_TIPO == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.ASSI_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.ASSI_NM_NOME);
                lista = query.ToList<ASSINANTE>();
            }
            return lista;
        }
    }
}
 