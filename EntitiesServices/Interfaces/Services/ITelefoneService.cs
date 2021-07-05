using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITelefoneService : IServiceBase<TELEFONE>
    {
        Int32 Create(TELEFONE perfil, LOG log);
        Int32 Create(TELEFONE perfil);
        Int32 Edit(TELEFONE perfil, LOG log);
        Int32 Edit(TELEFONE perfil);
        Int32 Delete(TELEFONE perfil, LOG log);

        TELEFONE CheckExist(TELEFONE conta, Int32 idAss);
        TELEFONE GetItemById(Int32 id);
        List<TELEFONE> GetAllItens(Int32 idAss);
        List<TELEFONE> GetAllItensAdm(Int32 idAss);

        List<CATEGORIA_TELEFONE> GetAllTipos(Int32 idAss);
        List<TELEFONE> ExecuteFilter(Int32? catId, String nome, String telefone, String cidade, Int32? uf, String celular, String email, Int32 idAss);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}
