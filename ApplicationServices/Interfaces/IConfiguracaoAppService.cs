using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IConfiguracaoAppService : IAppServiceBase<CONFIGURACAO>
    {
        Int32 ValidateEdit(CONFIGURACAO item, CONFIGURACAO itemAntes, USUARIO usuario);
        CONFIGURACAO GetItemById(Int32 id);
        List<CONFIGURACAO> GetAllItems(Int32 idAss);
        Int32 ValidateCreate(CONFIGURACAO item);
    }
}
