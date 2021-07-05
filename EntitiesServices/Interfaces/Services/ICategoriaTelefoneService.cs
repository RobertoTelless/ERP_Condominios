using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaTelefoneService : IServiceBase<CATEGORIA_TELEFONE>
    {
        Int32 Create(CATEGORIA_TELEFONE perfil, LOG log);
        Int32 Create(CATEGORIA_TELEFONE perfil);
        Int32 Edit(CATEGORIA_TELEFONE perfil, LOG log);
        Int32 Edit(CATEGORIA_TELEFONE perfil);
        Int32 Delete(CATEGORIA_TELEFONE perfil, LOG log);

        CATEGORIA_TELEFONE GetItemById(Int32 id);
        List<CATEGORIA_TELEFONE> GetAllItens(Int32 idAss);
        List<CATEGORIA_TELEFONE> GetAllItensAdm(Int32 idAss);
    }
}
