using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class OcorrenciaRepository : RepositoryBase<OCORRENCIA>, IOcorrenciaRepository
    {
        public OCORRENCIA GetItemById(Int32 id)
        {
            IQueryable<OCORRENCIA> query = Db.OCORRENCIA;
            query = query.Where(p => p.OCOR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<OCORRENCIA> GetAllItens(Int32 idAss)
        {
            IQueryable<OCORRENCIA> query = Db.OCORRENCIA.Where(p => p.OCOR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.OCOR_DT_OCORRENCIA);
            return query.ToList();
        }

        public List<OCORRENCIA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<OCORRENCIA> query = Db.OCORRENCIA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.OCOR_DT_OCORRENCIA);
            return query.ToList();
        }

        public List<OCORRENCIA> GetAllItensUser(Int32 id, Int32 idAss)
        {
            IQueryable<OCORRENCIA> query = Db.OCORRENCIA.Where(p => p.OCOR_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.OCOR_DT_OCORRENCIA);
            return query.ToList();
        }

        public List<OCORRENCIA> GetAllItensUnidade(Int32 id, Int32 idAss)
        {
            IQueryable<OCORRENCIA> query = Db.OCORRENCIA.Where(p => p.OCOR_IN_ATIVO == 1);
            query = query.Where(p => p.UNID_CD_ID == id);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.OCOR_DT_OCORRENCIA);
            return query.ToList();
        }

        public List<OCORRENCIA> GetOcorrenciasNovas(Int32 id, Int32 idAss)
        {
            IQueryable<OCORRENCIA> query = Db.OCORRENCIA.Where(p => p.OCOR_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.OCOR_DT_OCORRENCIA);
            return query.ToList();
        }

        public List<OCORRENCIA> ExecuteFilter(Int32? unidade, Int32? usuario, Int32? cat, String titulo, DateTime? data, String texto, Int32 idAss)
        {
            List<OCORRENCIA> lista = new List<OCORRENCIA>();
            IQueryable<OCORRENCIA> query = Db.OCORRENCIA;
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.OCOR_NM_TITULO.Contains(titulo));
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.OCOR_DT_OCORRENCIA) == DbFunctions.TruncateTime(data));
            }
            if (!String.IsNullOrEmpty(texto))
            {
                query = query.Where(p => p.OCOR_TX_TEXTO.Contains(texto));
            }
            if (unidade != null)
            {
                query = query.Where(p => p.UNID_CD_ID == unidade);
            }
            if (usuario != null)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (cat != null)
            {
                query = query.Where(p => p.CAOC_CD_ID == cat);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.OCOR_DT_OCORRENCIA);
                lista = query.ToList<OCORRENCIA>();
            }
            return lista;
        }
    }
}
