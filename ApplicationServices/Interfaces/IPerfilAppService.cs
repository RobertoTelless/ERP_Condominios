using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPerfilAppService : IAppServiceBase<PERFIL>
    {
        Int32 ValidateCreate(PERFIL perfil, USUARIO usuario);
        Int32 ValidateEdit(PERFIL perfil, PERFIL perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(PERFIL perfil, USUARIO usuario);
        List<PERFIL> GetAllItens();
        PERFIL GetByID(Int32 id);
    }
}
