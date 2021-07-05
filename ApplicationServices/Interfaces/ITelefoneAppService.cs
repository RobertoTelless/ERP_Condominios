using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITelefoneAppService : IAppServiceBase<TELEFONE>
    {
        Int32 ValidateCreate(TELEFONE perfil, USUARIO usuario);
        Int32 ValidateEdit(TELEFONE perfil, TELEFONE perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(TELEFONE item, TELEFONE itemAntes);
        Int32 ValidateDelete(TELEFONE perfil, USUARIO usuario);
        Int32 ValidateReativar(TELEFONE perfil, USUARIO usuario);

        List<TELEFONE> GetAllItens(Int32 idAss);
        List<TELEFONE> GetAllItensAdm(Int32 idAss);
        TELEFONE GetItemById(Int32 id);
        TELEFONE CheckExist(TELEFONE conta, Int32 idAss);

        List<CATEGORIA_TELEFONE> GetAllTipos(Int32 idAss);
        Int32 ExecuteFilter(Int32? catId, String nome, String telefone, String cidade, Int32? uf, String celular, String email, Int32 idAss, out List<TELEFONE> objeto);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}
