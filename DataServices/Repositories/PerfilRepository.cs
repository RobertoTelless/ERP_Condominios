using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PerfilRepository : RepositoryBase<PERFIL>, IPerfilRepository
    {
        public PERFIL GetByName(String nome)
        {
            return Db.PERFIL.Where(p => p.PERF_NM_NOME == nome).FirstOrDefault();
        }

        public List<PERFIL> GetAllItens()
        {
            IQueryable<PERFIL> query = Db.PERFIL;
            return query.ToList();
        }

        public USUARIO GetUserProfile(PERFIL perfil)
        {
            return Db.USUARIO.Where(p => p.PERF_CD_ID == perfil.PERF_CD_ID).FirstOrDefault();
        }
    }
}
 