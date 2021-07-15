using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IBancoAppService : IAppServiceBase<BANCO>
    {
        Int32 ValidateCreate(BANCO perfil, USUARIO usuario);
        Int32 ValidateEdit(BANCO perfil, BANCO perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(BANCO perfil, USUARIO usuario);
        Int32 ValidateReativar(BANCO perfil, USUARIO usuario);

        BANCO CheckExist(BANCO conta, Int32 idAss);
        List<BANCO> GetAllItens(Int32 idAss);
        List<BANCO> GetAllItensAdm(Int32 idAss);
        BANCO GetItemById(Int32 id);
        BANCO GetByCodigo(String codigo);
        Int32 ExecuteFilter(String codigo, String nome, Int32 idAss, out List<BANCO> objeto);
    }
}
