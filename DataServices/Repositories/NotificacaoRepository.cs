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
    public class NotificacaoRepository : RepositoryBase<NOTIFICACAO>, INotificacaoRepository
    {
        public NOTIFICACAO GetItemById(Int32 id)
        {
            IQueryable<NOTIFICACAO> query = Db.NOTIFICACAO;
            query = query.Where(p => p.NOTI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<NOTIFICACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<NOTIFICACAO> query = Db.NOTIFICACAO.Where(p => p.NOTI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.NOTI_DT_EMISSAO);
            return query.ToList();
        }

        public List<NOTIFICACAO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<NOTIFICACAO> query = Db.NOTIFICACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderByDescending(a => a.NOTI_DT_EMISSAO);
            return query.ToList();
        }

        public List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss)
        {
            IQueryable<NOTIFICACAO> query = Db.NOTIFICACAO.Where(p => p.NOTI_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Where(p => DbFunctions.TruncateTime(p.NOTI_DT_VALIDADE) >= DbFunctions.TruncateTime(DateTime.Today));
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.NOTIFICACAO_ANEXO);
            query = query.OrderByDescending(a => a.NOTI_DT_EMISSAO);
            return query.ToList();
        }

        public List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss)
        {
            IQueryable<NOTIFICACAO> query = Db.NOTIFICACAO.Where(p => p.NOTI_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Where(p => DbFunctions.TruncateTime(p.NOTI_DT_VALIDADE) >= DbFunctions.TruncateTime(DateTime.Today));
            query = query.Where(p => p.NOTI_IN_VISTA == 0);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.NOTIFICACAO_ANEXO);
            query = query.OrderByDescending(a => a.NOTI_DT_EMISSAO);
            return query.ToList();
        }

        public List<NOTIFICACAO> ExecuteFilter(String titulo, DateTime? data, String texto, Int32 idAss)
        {
            List<NOTIFICACAO> lista = new List<NOTIFICACAO>();
            IQueryable<NOTIFICACAO> query = Db.NOTIFICACAO;
            if (!String.IsNullOrEmpty(titulo))
            {
                query = query.Where(p => p.NOTI_NM_TITULO.Contains(titulo));
            }
            if (data != null)
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.NOTI_DT_EMISSAO) == DbFunctions.TruncateTime(data));
            }
            if (!String.IsNullOrEmpty(texto))
            {
                query = query.Where(p => p.NOTI_TX_TEXTO.Contains(texto));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.NOTI_DT_EMISSAO);
                lista = query.ToList<NOTIFICACAO>();
            }
            return lista;
        }
    }
}
