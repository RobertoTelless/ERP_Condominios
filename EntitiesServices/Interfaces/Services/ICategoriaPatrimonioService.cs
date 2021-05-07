using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaPatrimonioService : IServiceBase<CATEGORIA_PATRIMONIO>
    {
        Int32 Create(CATEGORIA_PATRIMONIO perfil, LOG log);
        Int32 Create(CATEGORIA_PATRIMONIO perfil);
        Int32 Edit(CATEGORIA_PATRIMONIO perfil, LOG log);
        Int32 Edit(CATEGORIA_PATRIMONIO perfil);
        Int32 Delete(CATEGORIA_PATRIMONIO perfil, LOG log);
        CATEGORIA_PATRIMONIO GetItemById(Int32 id);
        List<CATEGORIA_PATRIMONIO> GetAllItens();
        List<CATEGORIA_PATRIMONIO> GetAllItensAdm();
    }
}
