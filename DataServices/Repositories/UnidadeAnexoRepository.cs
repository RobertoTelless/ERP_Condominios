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
    public class UnidadeAnexoRepository : RepositoryBase<UNIDADE_ANEXO>, IUnidadeAnexoRepository
    {
        public List<UNIDADE_ANEXO> GetAllItens()
        {
            return Db.UNIDADE_ANEXO.ToList();
        }

        public UNIDADE_ANEXO GetItemById(Int32 id)
        {
            IQueryable<UNIDADE_ANEXO> query = Db.UNIDADE_ANEXO.Where(p => p.UNAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 