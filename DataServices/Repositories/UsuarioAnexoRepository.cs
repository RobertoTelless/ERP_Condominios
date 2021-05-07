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
    public class UsuarioAnexoRepository : RepositoryBase<USUARIO_ANEXO>, IUsuarioAnexoRepository
    {
        public List<USUARIO_ANEXO> GetAllItens()
        {
            return Db.USUARIO_ANEXO.ToList();
        }

        public USUARIO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<USUARIO_ANEXO> query = Db.USUARIO_ANEXO.Where(p => p.USAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 