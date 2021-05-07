using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ISexoAppService : IAppServiceBase<SEXO>
    {
        Int32 ValidateCreate(SEXO item, USUARIO usuario);
        Int32 ValidateEdit(SEXO item, SEXO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(SEXO item, SEXO itemAntes);
        Int32 ValidateDelete(SEXO item, USUARIO usuario);
        Int32 ValidateReativar(SEXO item, USUARIO usuario);
        List<SEXO> GetAllItens();
        List<SEXO> GetAllItensAdm();
        SEXO GetItemById(Int32 id);
    }
}
