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
    public class EncomendaRepository : RepositoryBase<ENCOMENDA>, IEncomendaRepository
    {

        public ENCOMENDA GetItemById(Int32 id)
        {
            IQueryable<ENCOMENDA> query = Db.ENCOMENDA;
            query = query.Where(p => p.ENCO_CD_ID == id);
            query = query.Include(p => p.UNIDADE);
            query = query.Include(p => p.ENCOMENDA_ANEXO);
            query = query.Include(p => p.ENCOMENDA_COMENTARIO);
            return query.FirstOrDefault();
        }

        public List<ENCOMENDA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<ENCOMENDA> query = Db.ENCOMENDA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ENCOMENDA> GetAllItens(Int32 idAss)
        {
            IQueryable<ENCOMENDA> query = Db.ENCOMENDA.Where(p => p.ENCO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<ENCOMENDA> GetByUnidade(Int32 idUnid)
        {
            IQueryable<ENCOMENDA> query = Db.ENCOMENDA.Where(p => p.ENCO_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == idUnid);
            return query.ToList();
        }

        public List<ENCOMENDA> GetByData(DateTime data, Int32 idAss)
        {
            IQueryable<ENCOMENDA> query = Db.ENCOMENDA.Where(p => p.ENCO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => DbFunctions.TruncateTime(p.ENCO_DT_CHEGADA) == DbFunctions.TruncateTime(data));
            return query.ToList();
        }

        public List<ENCOMENDA> ExecuteFilter(Int32? unid, Int32? forma, Int32? tipo, DateTime? data,  Int32? status, Int32 idAss)
        {
            List<ENCOMENDA> lista = new List<ENCOMENDA>();
            IQueryable<ENCOMENDA> query = Db.ENCOMENDA;
            if (unid > 0)
            {
                query = query.Where(p => p.UNID_CD_ID == unid);
            }
            if (forma > 0)
            {
                query = query.Where(p => p.FOEN_CD_ID == forma);
            }
            if (tipo > 0)
            {
                query = query.Where(p => p.TIEN_CD_ID == tipo);
            }
            if (status > 0)
            {
                query = query.Where(p => p.ENCO_IN_STATUS == status);
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.ENCO_DT_CHEGADA).Value == DbFunctions.TruncateTime(data).Value);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.ENCO_DT_CHEGADA);
                lista = query.ToList<ENCOMENDA>();
            }
            return lista;
        }
    }
}
 