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
    public class VagaRepository : RepositoryBase<VAGA>, IVagaRepository
    {
        public VAGA CheckExist(VAGA tarefa, Int32 idAss)
        {
            IQueryable<VAGA> query = Db.VAGA;
            query = query.Where(p => p.VAGA_NR_NUMERO == tarefa.VAGA_NR_NUMERO);
            query = query.Where(p => p.VAGA_NR_ANDAR == tarefa.VAGA_NR_ANDAR);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public VAGA GetItemById(Int32 id)
        {
            IQueryable<VAGA> query = Db.VAGA;
            query = query.Where(p => p.VAGA_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.VEICULO);
            return query.FirstOrDefault();
        }

        public List<VAGA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<VAGA> query = Db.VAGA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VAGA> GetAllItens(Int32 idAss)
        {
            IQueryable<VAGA> query = Db.VAGA.Where(p => p.VAGA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<VAGA> ExecuteFilter(String numero, String andar, Int32? unid, Int32? idTipo, Int32 idAss)
        {
            List<VAGA> lista = new List<VAGA>();
            IQueryable<VAGA> query = Db.VAGA;
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.VAGA_NR_NUMERO == numero);
            }
            if (!String.IsNullOrEmpty(andar))
            {
                query = query.Where(p => p.VAGA_NR_ANDAR == andar);
            }
            if (unid > 0)
            {
                query = query.Where(p => p.UNID_CD_ID == unid);
            }
            if (idTipo > 0)
            {
                query = query.Where(p => p.TIVA_CD_ID == idTipo);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.VAGA_NR_ANDAR).ThenBy(b => b.VAGA_NR_NUMERO);
                lista = query.ToList<VAGA>();
            }
            return lista;
        }
    }
}
 