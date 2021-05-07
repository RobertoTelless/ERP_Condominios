using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoContaAppService : IAppServiceBase<TIPO_CONTA>
    {
        Int32 ValidateCreate(TIPO_CONTA item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_CONTA item, TIPO_CONTA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(TIPO_CONTA item, TIPO_CONTA itemAntes);
        Int32 ValidateDelete(TIPO_CONTA item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_CONTA item, USUARIO usuario);
        List<TIPO_CONTA> GetAllItens();
        List<TIPO_CONTA> GetAllItensAdm();
        TIPO_CONTA GetItemById(Int32 id);
    }
}
