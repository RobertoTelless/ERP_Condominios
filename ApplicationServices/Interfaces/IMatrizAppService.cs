using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMatrizAppService : IAppServiceBase<MATRIZ>
    {
        Int32 ValidateCreate(MATRIZ perfil, USUARIO usuario);
        Int32 ValidateEdit(MATRIZ perfil, MATRIZ perfilAntes, USUARIO usuario);
        List<MATRIZ> GetAllItens();
        List<MATRIZ> GetAllItensAdm();
        MATRIZ GetItemById(Int32 id);
        List<TIPO_PESSOA> GetAllTipoPessoa();
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}
