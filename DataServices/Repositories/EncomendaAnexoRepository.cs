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
    public class EncomendaAnexoRepository : RepositoryBase<ENCOMENDA_ANEXO>, IEncomendaAnexoRepository
    {
        public List<ENCOMENDA_ANEXO> GetAllItens(Int32 idAss)
        {
            return Db.ENCOMENDA_ANEXO.ToList();
        }

        public ENCOMENDA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<ENCOMENDA_ANEXO> query = Db.ENCOMENDA_ANEXO.Where(p => p.ENAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 