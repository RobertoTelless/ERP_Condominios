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
    public class NoticiaRepository : RepositoryBase<NOTICIA>, INoticiaRepository
    {
        public NOTICIA GetItemById(Int32 id)
        {
            IQueryable<NOTICIA> query = Db.NOTICIA;
            query = query.Where(p => p.NOTC_CD_ID == id);
            query = query.Include(p => p.NOTICIA_COMENTARIO);
            return query.FirstOrDefault();
        }

        public List<NOTICIA> GetAllItens(Int32 idAss)
        {
            IQueryable<NOTICIA> query = Db.NOTICIA.Where(p => p.NOTC_IN_ATIVO == 1);
            query = query.Include(p => p.NOTICIA_COMENTARIO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<NOTICIA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<NOTICIA> query = Db.NOTICIA;
            query = query.Include(p => p.NOTICIA_COMENTARIO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<NOTICIA> GetAllItensValidos(Int32 idAss)
        {
            IQueryable<NOTICIA> query = Db.NOTICIA;
            query = query.Where(p => DbFunctions.TruncateTime(p.NOTC_DT_VALIDADE) >= DbFunctions.TruncateTime(DateTime.Today));
            query = query.Include(p => p.NOTICIA_COMENTARIO);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(p => p.NOTC_DT_DATA_AUTOR);
            return query.ToList();
        }

        public List<NOTICIA> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss)
        {
            List<NOTICIA> lista = new List<NOTICIA>();
            IQueryable<NOTICIA> query = Db.NOTICIA;
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.NOTC_NM_TITULO.Contains(titulo));
            }
            if (!String.IsNullOrEmpty(autor))
            {
                query = query.Where(p => p.NOTC_NM_AUTOR.Contains(autor));
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.NOTC_DT_EMISSAO) == DbFunctions.TruncateTime(data));
            }
            if (!String.IsNullOrEmpty(texto))
            {
                query = query.Where(p => p.NOTC_TX_TEXTO.Contains(texto));
            }
            if (!String.IsNullOrEmpty(link))
            {
                query = query.Where(p => p.NOTC_LK_LINK.Contains(link));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.NOTC_DT_EMISSAO);
                lista = query.ToList<NOTICIA>();
            }
            return lista;
        }
    }
}
