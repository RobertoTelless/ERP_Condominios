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
    public class CentroCustoRepository : RepositoryBase<CENTRO_CUSTO>, ICentroCustoRepository
    {
        public CENTRO_CUSTO CheckExist(CENTRO_CUSTO conta, Int32 idAss)
        {
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO;
            query = query.Where(p => p.CECU_NM_NOME == conta.CECU_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CENTRO_CUSTO GetByNome(String nome, Int32 idAss)
        {
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.CECU_NM_NOME == nome);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CENTRO_CUSTO GetItemById(Int32 id)
        {
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO;
            query = query.Where(p => p.CECU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CENTRO_CUSTO> GetAllItens(Int32 idAss)
        {
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CENTRO_CUSTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CENTRO_CUSTO> GetAllDespesas(Int32 idAss)
        {
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.CECU_IN_TIPO == 2);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CENTRO_CUSTO> GetAllReceitas(Int32 idAss)
        {
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO.Where(p => p.CECU_IN_ATIVO == 1);
            query = query.Where(p => p.CECU_IN_TIPO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CENTRO_CUSTO> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss)
        {
            List<CENTRO_CUSTO> lista = new List<CENTRO_CUSTO>();
            IQueryable<CENTRO_CUSTO> query = Db.CENTRO_CUSTO;
            if (grupoId != null)
            {
                query = query.Where(p => p.GRUP_CD_ID == grupoId);
            }
            if (subGrupoId != null)
            {
                query = query.Where(p => p.SUBG_CD_ID == subGrupoId);
            }
            if (tipo != null)
            {
                query = query.Where(p => p.CECU_IN_TIPO == tipo);
            }
            if (movimento != null)
            {
                query = query.Where(p => p.CECU_IN_MOVTO == movimento);
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.CECU_NR_NUMERO == numero);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CECU_NM_NOME.Contains(nome));
            }
            if (query != null)
            {
                query = query.Where(p => p.CECU_IN_ATIVO == 1);
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CECU_NM_NOME);
                lista = query.ToList<CENTRO_CUSTO>();
            }
            return lista;
        }

    }
}
 