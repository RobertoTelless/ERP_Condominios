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
    public class NotificacaoAnexoRepository : RepositoryBase<NOTIFICACAO_ANEXO>, INotificacaoAnexoRepository
    {
        public List<NOTIFICACAO_ANEXO> GetAllItens()
        {
            return Db.NOTIFICACAO_ANEXO.ToList();
        }

        public NOTIFICACAO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<NOTIFICACAO_ANEXO> query = Db.NOTIFICACAO_ANEXO.Where(p => p.NOAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 