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
    public class AgendaAnexoRepository : RepositoryBase<AGENDA_ANEXO>, IAgendaAnexoRepository
    {
        public List<AGENDA_ANEXO> GetAllItens()
        {
            return Db.AGENDA_ANEXO.ToList();
        }

        public AGENDA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<AGENDA_ANEXO> query = Db.AGENDA_ANEXO.Where(p => p.AGAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 