using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ISexoService : IServiceBase<SEXO>
    {
        Int32 Create(SEXO perfil, LOG log);
        Int32 Create(SEXO perfil);
        Int32 Edit(SEXO perfil, LOG log);
        Int32 Edit(SEXO perfil);
        Int32 Delete(SEXO perfil, LOG log);
        SEXO GetItemById(Int32 id);
        List<SEXO> GetAllItens();
        List<SEXO> GetAllItensAdm();
    }
}
