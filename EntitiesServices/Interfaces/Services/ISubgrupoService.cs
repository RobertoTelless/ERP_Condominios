using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ISubgrupoService : IServiceBase<SUBGRUPO>
    {
        Int32 Create(SUBGRUPO item, LOG log);
        Int32 Create(SUBGRUPO item);
        Int32 Edit(SUBGRUPO item, LOG log);
        Int32 Edit(SUBGRUPO item);
        Int32 Delete(SUBGRUPO item, LOG log);
        SUBGRUPO GetItemById(Int32 id);
        SUBGRUPO CheckExist(SUBGRUPO item);
        List<SUBGRUPO> GetAllItens();
        List<SUBGRUPO> GetAllItensAdm();
        List<GRUPO> GetAllGrupos();
    }
}
