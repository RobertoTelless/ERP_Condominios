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
using CrossCutting;

namespace DataServices.Repositories
{
    public class AutorizacaoRepository : RepositoryBase<AUTORIZACAO_ACESSO>, IAutorizacaoRepository
    {
        public AUTORIZACAO_ACESSO CheckExist(AUTORIZACAO_ACESSO conta, Int32 idAss)
        {
            IQueryable<AUTORIZACAO_ACESSO> query = Db.AUTORIZACAO_ACESSO;
            query = query.Where(p => p.AUAC_NM_VISITANTE == conta.AUAC_NM_VISITANTE);
            query = query.Where(p => p.UNID_CD_ID == conta.UNID_CD_ID);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public AUTORIZACAO_ACESSO GetItemById(Int32 id)
        {
            IQueryable<AUTORIZACAO_ACESSO> query = Db.AUTORIZACAO_ACESSO;
            query = query.Where(p => p.AUAC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<AUTORIZACAO_ACESSO> GetAllItens(Int32 idAss)
        {
            IQueryable<AUTORIZACAO_ACESSO> query = Db.AUTORIZACAO_ACESSO.Where(p => p.AUAC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<AUTORIZACAO_ACESSO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<AUTORIZACAO_ACESSO> query = Db.AUTORIZACAO_ACESSO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<AUTORIZACAO_ACESSO> ExecuteFilter(Int32? unid, String nome, String documento, String empresa, Int32? tipo, DateTime? data, Int32 idAss)
        {
            List<AUTORIZACAO_ACESSO> lista = new List<AUTORIZACAO_ACESSO>();
            IQueryable<AUTORIZACAO_ACESSO> query = Db.AUTORIZACAO_ACESSO;
            if (unid != null)
            {
                query = query.Where(p => p.UNID_CD_ID == unid);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.AUAC_NM_VISITANTE.Contains(nome));
            }
            if (!String.IsNullOrEmpty(documento))
            {
                query = query.Where(p => p.AUAC_NR_DOCUMENTO.Contains(documento));
            }
            if (!String.IsNullOrEmpty(empresa))
            {
                query = query.Where(p => p.AUAC_NM_EMPRESA.Contains(empresa));
            }
            if (tipo != null)
            {
                query = query.Where(p => p.AUAC_IN_TIPO == tipo);
            }
            if (data != null)
            {
                query = query.Where(p => p.AUAC_DT_INICIO == data);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.AUAC_DT_INICIO);
                lista = query.ToList<AUTORIZACAO_ACESSO>();
            }
            return lista;
        }

    }
}
 