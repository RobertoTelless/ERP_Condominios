using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPerfilService : IServiceBase<PERFIL>
    {
        Int32 Create(PERFIL perfil, LOG log);
        Int32 Create(PERFIL perfil);
        Int32 Edit(PERFIL perfil, LOG log);
        Int32 Edit(PERFIL perfil);
        Int32 Delete(PERFIL perfil, LOG log);
        CONFIGURACAO CarregaConfiguracao();
        PERFIL GetByName(String nome);
        USUARIO GetUserProfile(PERFIL perfil);
        List<PERFIL> GetAllItens();
    }
}
