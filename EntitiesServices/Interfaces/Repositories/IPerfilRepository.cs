using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPerfilRepository : IRepositoryBase<PERFIL>
    {
        PERFIL GetByName(String nome);
        USUARIO GetUserProfile(PERFIL perfil);
        List<PERFIL> GetAllItens();
    }
}
