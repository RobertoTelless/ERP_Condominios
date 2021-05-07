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
    public class PatrimonioRepository : RepositoryBase<PATRIMONIO>, IPatrimonioRepository
    {
        public PATRIMONIO CheckExist(PATRIMONIO conta)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO;
            query = query.Where(p => p.PATR_NR_NUMERO_PATRIMONIO == conta.PATR_NR_NUMERO_PATRIMONIO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PATRIMONIO GetByNumero(String numero)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO.Where(p => p.PATR_IN_ATIVO == 1);
            query = query.Where(p => p.PATR_NR_NUMERO_PATRIMONIO == numero);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.ASSINANTE);
            query = query.Include(p => p.MATRIZ);
            query = query.Include(p => p.FILIAL);
            return query.FirstOrDefault();
        }

        public PATRIMONIO GetItemById(Int32 id)
        {
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO;
            query = query.Where(p => p.PATR_CD_ID == id);
            query = query.Include(p => p.ASSINANTE);
            query = query.Include(p => p.MATRIZ);
            query = query.Include(p => p.FILIAL);
            return query.FirstOrDefault();
        }

        public List<PATRIMONIO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO.Where(p => p.PATR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.ASSINANTE);
            query = query.Include(p => p.MATRIZ);
            query = query.Include(p => p.FILIAL);
            return query.ToList();
        }

        public List<PATRIMONIO> CalcularDepreciados()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO.Where(p => p.PATR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.AddDays(p.PATR_DT_COMPRA.Value, p.PATR_NR_VIDA_UTIL.Value) < DateTime.Today);
            query = query.Include(p => p.ASSINANTE);
            query = query.Include(p => p.MATRIZ);
            query = query.Include(p => p.FILIAL);
            return query.ToList();
        }

        public List<PATRIMONIO> CalcularBaixados()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO.Where(p => p.PATR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.PATR_DT_BAIXA != null);
            query = query.Include(p => p.ASSINANTE);
            query = query.Include(p => p.MATRIZ);
            query = query.Include(p => p.FILIAL);
            return query.ToList();
        }

        public List<PATRIMONIO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.ASSINANTE);
            query = query.Include(p => p.MATRIZ);
            query = query.Include(p => p.FILIAL);
            return query.ToList();
        }

        public List<PATRIMONIO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? filiId)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            List<PATRIMONIO> lista = new List<PATRIMONIO>();
            IQueryable<PATRIMONIO> query = Db.PATRIMONIO;
            if (catId != null)
            {
                query = query.Where(p => p.CAPA_CD_ID == catId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PATR_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(numero))
            {
                query = query.Where(p => p.PATR_NR_NUMERO_PATRIMONIO == numero);
            }
            if (filiId != null)
            {
                query = query.Where(p => p.FILI_CD_ID == filiId);
            }

            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.PATR_NR_NUMERO_PATRIMONIO);
                lista = query.ToList<PATRIMONIO>();
            }
            return lista;
        }
    }
}
 